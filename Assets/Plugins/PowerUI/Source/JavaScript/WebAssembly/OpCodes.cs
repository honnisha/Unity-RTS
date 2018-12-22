using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>WebAssembly opcodes.</summary>
	public static class OpCodes{
		
		/// <summary>All available opcodes, indexed by WebAssembly OpCode.
		/// Don't access this directly - use Get instead.</summary>
		internal static Dictionary<ushort,OpCode> All = new Dictionary<ushort,OpCode>();
		
		
		/// <summary>Attempts to find an OpCode by its WebAssembly numeric form.</summary>
		/// <returns>Null if nothing was found.</returns>
		public static OpCode Get(ushort wasmCode){
			OpCode result;
			All.TryGetValue(wasmCode,out result);
			return result;
		}
		
		static OpCodes(){
			
			/* Control flow operators */
			
			Unreachable = Create(0x00).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(WebAssembly.ReflectionHelpers.Runtime_Trap);
					}
				);
			
			Nop = Create(0x01).
				OnOutput(
					delegate(ILGenerator gen){
						gen.NoOperation();
					}
				);
			
			Block = Create(0x02).
				OnBeforeOutput(LabelledBlock.Prepare).
				OnOutput(LabelledBlock.Complete).
				OnImmediates(ImmediatesType.Block);
			
			Loop = Create(0x03).
				OnBeforeOutput(LabelledBlock.Prepare).
				OnOutput(LabelledBlock.Complete).
				OnImmediates(ImmediatesType.Block);
			
			If = Create(0x04).
				OnBeforeOutput(LabelledBlock.Prepare).
				OnOutput(LabelledBlock.Complete).
				OnImmediates(ImmediatesType.Block);
			
			Else = Create(0x05).
				OnBeforeOutput(LabelledBlock.Prepare).
				OnOutput(LabelledBlock.Complete).
				OnImmediates(
					delegate(Reader reader){
						
						// End the previous block:
						LabelledBlock block = reader.PopBlock();
						
						// Return type matches previous block:
						reader.Instruction.ReturnType = block.ReturnType;
						
						// Start this one:
						return reader.PushBlock(block.ReturnType);
					}
				);
			
			End = Create(0x0b).
				Ignore(). // (End doesn't actually exist in the s-expr)
				OnImmediates(
					delegate(Reader reader){
						
						// Pop the block from the block stack:
						reader.PopBlock();
						
						return null;
					}
				);
			
			Br = Create(0x0c).
				OnImmediates(ImmediatesType.Label).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Branch( ((LabelledBlock)gen.Instruction.Immediates).Label );
					}
				);
			
			Br_if = Create(0x0d).
				Inputs(1).
				OnImmediates(ImmediatesType.Label).
				OnOutput(
					delegate(ILGenerator gen){
						gen.BranchIfTrue( ((LabelledBlock)gen.Instruction.Immediates).Label );
					}
				);
			
			Br_table = Create(0x0e).
				Inputs(1).
				OnImmediates(ImmediatesType.BrTable).
				OnOutput(
					delegate(ILGenerator gen){
						
						// Get the immediates:
						BrTableImmediate immediates = (BrTableImmediate)gen.Instruction.Immediates;
						
						// Transfer the labels set now:
						ILLabel[] labels = new ILLabel[immediates.Labels.Length];
						
						for(int i=0;i<labels.Length;i++){
							labels[i] = immediates.Labels[i].Label;
						}
						
						// Switch statement:
						gen.Switch(labels);
						
						// We fall through the switch if it did nothing - default down here:
						gen.Branch(immediates.Default.Label);
						
					}
				);
			
			Return = Create(0x0f).
				OnImmediates(
					delegate(Reader reader){
						
						// Get the return type:
						Type returnType = reader.FunctionBody.Signature.GetReturnType();
						
						// Apply it:
						reader.Instruction.ReturnType = returnType;
						
						return null;
					}
				).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Return();
					}
				);
			
			/* Call operators */
		
			Call = Create(0x10).
				OnImmediates(ImmediatesType.Method).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call( (MethodInfo)gen.Instruction.Immediates );
					}
				);
			
			Call_indirect = Create(0x11).
				OnImmediates(
					delegate(Reader reader){
						
						// The expected signature:
						FuncType expected=reader.FuncType();
						
						// Reserved (must be false in MVP):
						if(reader.VarUInt1()){
							reader.Failed("call_indirect expected a 0.");
						}
						
						// Actual input count/ return type is..
						reader.Instruction.InputCount = expected.ParameterCount;
						reader.Instruction.ReturnType = expected.GetReturnType();
						
						return expected;
					}
				).
				OnOutput(
					delegate(ILGenerator gen){
						
						// Get the func type:
						FuncType expected = (FuncType)gen.Instruction.Immediates;
						
						// Emit the module:
						EmitHelpers.LoadRuntime(gen);
						
						// Table index to method handle:
						gen.Call( ReflectionHelpers.OpcodeMethods_IndexToMethod );
						
						// IntPtr to native:
						gen.Call( ReflectionHelpers.IntPtr_NativePointer );
						
						// Calli:
						gen.CallIndirect(expected.GetReturnType(),expected.GetParameters());
						
					}
				);
			
			/* Parametric operators */
			
			Drop = Create(0x1a).
				Inputs(1).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Pop();
					}
				);
			
			Select = Create(0x1b).
				Inputs(2).
				OnImmediates(
					delegate(Reader reader){
						
						// Return type is the same as the inputs (already on the stack):
						reader.Instruction.ReturnType = reader.FunctionBody.Root.Peek().ReturnType;
						
						return null;
					}
				).
				OnOutput(
					delegate(ILGenerator gen){
						
						// Create a label:
						var PopIfOne = gen.CreateLabel();
						var end = gen.CreateLabel();
						var local = gen.DeclareVariable(gen.Instruction.ReturnType,null);
						
						gen.BranchIfTrue(PopIfOne);
							// stloc, pop A, ldloc.
							gen.StoreVariable(local);
							gen.Pop();
							gen.LoadVariable(local);
							gen.Branch(end);
						gen.DefineLabelPosition(PopIfOne);
							// If we have a 1 on the stack, just pop B.
							gen.Pop();
						gen.DefineLabelPosition(end);
						
					}
				);
			
			/* Variable access */
			
			Get_local = Create(0x20).
				OnImmediates(ImmediatesType.LocalWithReturn).
				OnOutput(
					delegate(ILGenerator gen){
						
						object im=gen.Instruction.Immediates;
						FunctionLocal local = im as FunctionLocal;
						
						if(local!=null){
							gen.LoadVariable(local.CompilerVariable);
						}else{
							// Argument index:
							int index = (int)im;
							
							// Read arg x:
							gen.LoadArgument(index);
						}
						
					}
				);
			
			Set_local = Create(0x21).
				Inputs(1).
				OnImmediates(
					delegate(Reader reader){
						
						// param/local index:
						int index=(int)reader.VarUInt32();
						
						if( reader.FunctionBody.IsLocal(index) ){
							// Read local:
							return reader.FunctionBody.GetLocal(index);
						}else{
							return index;
						}
						
					}
				).
				OnOutput(
					delegate(ILGenerator gen){
						
						object im=gen.Instruction.Immediates;
						FunctionLocal local = im as FunctionLocal;
						
						if(local!=null){
							// Store local:
							gen.StoreVariable(local.CompilerVariable);
						}else{
							// Argument index:
							int index = (int)im;
							
							// Store arg x:
							gen.StoreArgument(index);
						}
						
					}
				);
			
			Tee_local = Create(0x22).
				Inputs(1).
				OnImmediates(ImmediatesType.LocalWithReturn).
				OnOutput(
					delegate(ILGenerator gen){
						
						// Duplicate:
						gen.Duplicate();
						
						object im=gen.Instruction.Immediates;
						FunctionLocal local = im as FunctionLocal;
						
						if(local!=null){
							// Store local:
							gen.StoreVariable(local.CompilerVariable);
						}else{
							// Argument index:
							int index = (int)im;
							
							// Store arg x:
							gen.StoreArgument(index);
						}
						
					}
				);
				
			Get_global = Create(0x23).
				CanInit().
				OnImmediates(
					delegate(Reader reader){
						
						// Read the global:
						Type type;
						FieldInfo globe = reader.Global(out type);
						
						// Set the return type:
						reader.Instruction.ReturnType = type;
						
						return globe;
					}
				).
				OnOutput(
					delegate(ILGenerator gen){
						gen.LoadField(gen.Instruction.Immediates as FieldInfo);
					}
				);
				
			Set_global = Create(0x24).
				Inputs(1).
				OnImmediates(delegate(Reader reader){
					Type type;
					return reader.Global(out type);
				}).
				OnOutput(
					delegate(ILGenerator gen){
						gen.StoreField(gen.Instruction.Immediates as FieldInfo);
					}
				);
				
			/* Memory-related operators */
			
			I32_load = Create(0x28).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,2); // 2 means *4 bytes*.
						gen.AddressInt32();
					}
				);
				
			I64_load = Create(0x29).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,3); // 3 means *8 bytes*.
						gen.AddressInt64();
					}
				);
				
			F32_load = Create(0x2a).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,2); // 2 means *4 bytes*.
						gen.AddressSingle();
					}
				);
				
			F64_load = Create(0x2b).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,3); // 3 means *8 bytes*.
						gen.AddressDouble();
					}
				);
				
			I32_load8_s = Create(0x2c).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						// Ldind_I1
						gen.Reader.MemoryAddress(gen,0); // 0 means *1 byte*.
						gen.AddressInt8();
					}
				);
				
			I32_load8_u = Create(0x2d).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,0); // 0 means *1 byte*.
						gen.AddressUnsignedInt8();
					}
				);
				
			I32_load16_s = Create(0x2e).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,1); // 1 means *2 bytes*.
						gen.AddressInt16();
					}
				);
				
			I32_load16_u = Create(0x2f).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,1); // 1 means *2 bytes*.
						gen.AddressUnsignedInt16();
					}
				);
				
			I64_load8_s = Create(0x30).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,0); // 0 means *1 byte*.
						gen.AddressInt8();
						gen.ConvertToInt64();
					}
				);
			
			I64_load8_u = Create(0x31).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,0); // 0 means *1 byte*.
						gen.AddressUnsignedInt8();
						gen.ConvertToUnsignedInt64();
					}
				);
				
			I64_load16_s = Create(0x32).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,1); // 1 means *2 bytes*.
						gen.AddressInt16();
						gen.ConvertToInt64();
					}
				);
			
			I64_load16_u = Create(0x33).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,1); // 1 means *2 bytes*.
						gen.AddressUnsignedInt16();
						gen.ConvertToUnsignedInt64();
					}
				);
			
			I64_load32_s = Create(0x34).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,2); // 2 means *4 bytes*.
						gen.AddressInt32();
						gen.ConvertToInt64();
					}
				);
			
			I64_load32_u = Create(0x35).
				OnImmediates(ImmediatesType.MemoryLoad).
				Returns(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Reader.MemoryAddress(gen,2); // 2 means *4 bytes*.
						gen.AddressUnsignedInt32();
						gen.ConvertToUnsignedInt64();
					}
				);
			
			I32_store = Create(0x36).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			I64_store = Create(0x37).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			F32_store = Create(0x38).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			F64_store = Create(0x39).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			I32_store8 = Create(0x3a).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			I32_store16 = Create(0x3b).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			I64_store8 = Create(0x3c).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			I64_store16 = Create(0x3d).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			I64_store32 = Create(0x3e).
				OnImmediates(ImmediatesType.MemoryStore).
				OnOutput(
					delegate(ILGenerator gen){
						throw new NotImplementedException();
					}
				);
			
			Current_memory = Create(0x3f).
				OnImmediates(ImmediatesType.VarUInt1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						EmitHelpers.LoadRuntime(gen);
						gen.Call(ReflectionHelpers.Module_QueryMemory);
					}
				);
				
			Grow_memory = Create(0x40).
				OnImmediates(ImmediatesType.VarUInt1).
				Inputs(1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						EmitHelpers.LoadRuntime(gen);
						gen.Call(ReflectionHelpers.Module_GrowMemory);
					}
				);
			
			/* Constants */
			
			I32_const = Create(0x41).
				Returns(typeof(int)).
				OnImmediates(ImmediatesType.VarInt32).
				OnOutput(
					delegate(ILGenerator gen){
						gen.LoadInt32((int)gen.Instruction.Immediates);
					}
				);
			
			I64_const = Create(0x42).
				Returns(typeof(long)).
				CanInit().
				OnImmediates(ImmediatesType.VarInt64).
				OnOutput(
					delegate(ILGenerator gen){
						gen.LoadInt64((long)gen.Instruction.Immediates);
					}
				);
			
			F32_const = Create(0x43).
				Returns(typeof(float)).
				CanInit().
				OnImmediates(ImmediatesType.Single).
				OnOutput(
					delegate(ILGenerator gen){
						gen.LoadSingle((float)gen.Instruction.Immediates);
					}
				);
			
			F64_const = Create(0x44).
				Returns(typeof(double)).
				CanInit().
				OnImmediates(ImmediatesType.Double).
				OnOutput(
					delegate(ILGenerator gen){
						gen.LoadDouble((double)gen.Instruction.Immediates);
					}
				);
			
			/* Comparison operators */
			
			I32_eqz = Create(0x45).
				AsComparison(1).
				OnOutput(
					delegate(ILGenerator gen){
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I32_eq = Create(0x46).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
					}
				);
				
			I32_ne = Create(0x47).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I32_lt_s = Create(0x48).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareLessThan();
					}
				);
				
			I32_lt_u = Create(0x49).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareLessThanUnsigned();
					}
				);
				
			I32_gt_s = Create(0x4a).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareGreaterThan();
					}
				);
				
			I32_gt_u = Create(0x4b).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareGreaterThanUnsigned();
					}
				);
				
			I32_le_s = Create(0x4c).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a<=b becomes (a>b)==0
						gen.CompareGreaterThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I32_le_u = Create(0x4d).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a<=b becomes (a>b)==0
						gen.CompareGreaterThanUnsigned();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I32_ge_s = Create(0x4e).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a>=b becomes (a<b)==0
						gen.CompareLessThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I32_ge_u = Create(0x4f).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a>=b becomes (a<b)==0
						gen.CompareLessThanUnsigned();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I64_eqz = Create(0x50).
				AsComparison(1).
				OnOutput(
					delegate(ILGenerator gen){
						gen.LoadInt64(0);
						gen.CompareEqual();
					}
				);
				
			I64_eq = Create(0x51).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
					}
				);
				
			I64_ne = Create(0x52).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I64_lt_s = Create(0x53).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareLessThan();
					}
				);
				
			I64_lt_u = Create(0x54).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareLessThanUnsigned();
					}
				);
				
			I64_gt_s = Create(0x55).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareGreaterThan();
					}
				);
				
			I64_gt_u = Create(0x56).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareGreaterThanUnsigned();
					}
				);
				
			I64_le_s = Create(0x57).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a<=b becomes (a>b)==0
						gen.CompareGreaterThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I64_le_u = Create(0x58).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a<=b becomes (a>b)==0
						gen.CompareGreaterThanUnsigned();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I64_ge_s = Create(0x59).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a>=b becomes (a<b)==0
						gen.CompareLessThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			I64_ge_u = Create(0x5a).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a>=b becomes (a<b)==0
						gen.CompareLessThanUnsigned();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			F32_eq = Create(0x5b).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
					}
				);
				
			F32_ne = Create(0x5c).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
				
			F32_lt = Create(0x5d).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareLessThan();
					}
				);
				
			F32_gt = Create(0x5e).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareGreaterThan();
					}
				);
				
			F32_le = Create(0x5f).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a<=b becomes (a>b)==0
						gen.CompareGreaterThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
			
			F32_ge = Create(0x60).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a>=b becomes (a<b)==0
						gen.CompareLessThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
			
			F64_eq = Create(0x61).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
					}
				);
			
			F64_ne = Create(0x62).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareEqual();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
			
			F64_lt = Create(0x63).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareLessThan();
					}
				);
			
			F64_gt = Create(0x64).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						gen.CompareGreaterThan();
					}
				);
			
			F64_le = Create(0x65).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a<=b becomes (a>b)==0
						gen.CompareGreaterThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
			
			F64_ge = Create(0x66).
				AsComparison().
				OnOutput(
					delegate(ILGenerator gen){
						// a>=b becomes (a<b)==0
						gen.CompareLessThan();
						gen.LoadInt32(0);
						gen.CompareEqual();
					}
				);
			
			/* Numeric Operators */
			
			I32_clz = Create(0x67).
				Inputs(1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){	
						gen.Call(ReflectionHelpers.OpcodeMethods_Clz32);
					}
				);
				
			I32_ctz = Create(0x68).
				Inputs(1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){	
						gen.Call(ReflectionHelpers.OpcodeMethods_Ctz32);
					}
				);
				
			I32_popcnt = Create(0x69).
				Inputs(1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){	
						gen.Call(ReflectionHelpers.OpcodeMethods_Popcnt32);
					}
				);
				
			I32_add = Create(0x6a).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Add();
					}
				);
				
			I32_sub = Create(0x6b).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Subtract();
					}
				);
			
			I32_mul = Create(0x6c).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Multiply();
					}
				);
				
			I32_div_s = Create(0x6d).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Divide();
					}
				);
				
			I32_div_u = Create(0x6e).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.DivideUnsigned();
					}
				);
				
			I32_rem_s = Create(0x6f).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Remainder();
					}
				);
				
			I32_rem_u = Create(0x70).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.RemainderUnsigned();
					}
				);
				
			I32_and = Create(0x71).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.BitwiseAnd();
					}
				);
				
			I32_or = Create(0x72).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.BitwiseOr();
					}
				);
			
			I32_xor = Create(0x73).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.BitwiseXor();
					}
				);
				
			I32_shl = Create(0x74).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ShiftLeft();
					}
				);
				
			I32_shr_s = Create(0x75).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ShiftRight();
					}
				);
				
			I32_shr_u = Create(0x76).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ShiftRightUnsigned();
					}
				);
				
			I32_rotl = Create(0x77).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.OpcodeMethods_Rotl32);
					}
				);
				
			I32_rotr = Create(0x78).
				AsNumeric(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.OpcodeMethods_Rotr32);
					}
				);
				
			I64_clz = Create(0x79).
				Inputs(1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){	
						gen.Call(ReflectionHelpers.OpcodeMethods_Clz64);
					}
				);
				
			I64_ctz = Create(0x7a).
				Inputs(1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){	
						gen.Call(ReflectionHelpers.OpcodeMethods_Ctz64);
					}
				);
				
			I64_popcnt = Create(0x7b).
				Inputs(1).
				Returns(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){	
						gen.Call(ReflectionHelpers.OpcodeMethods_Popcnt64);
					}
				);
				
			I64_add = Create(0x7c).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Add();
					}
				);
				
			I64_sub = Create(0x7d).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Subtract();
					}
				);
				
			I64_mul = Create(0x7e).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Multiply();
					}
				);
				
			I64_div_s = Create(0x7f).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Divide();
					}
				);
				
			I64_div_u = Create(0x80).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.DivideUnsigned();
					}
				);
				
			I64_rem_s = Create(0x81).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Remainder();
					}
				);
				
			I64_rem_u = Create(0x82).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.RemainderUnsigned();
					}
				);
				
			I64_and = Create(0x83).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.BitwiseAnd();
					}
				);
				
			I64_or = Create(0x84).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.BitwiseOr();
					}
				);
				
			I64_xor = Create(0x85).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.BitwiseXor();
					}
				);
				
			I64_shl = Create(0x86).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ShiftLeft();
					}
				);
				
			I64_shr_s = Create(0x87).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ShiftRight();
					}
				);
				
			I64_shr_u = Create(0x88).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ShiftRightUnsigned();
					}
				);
				
			I64_rotl = Create(0x89).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.OpcodeMethods_Rotl64);
					}
				);
				
			I64_rotr = Create(0x8a).
				AsNumeric(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.OpcodeMethods_Rotr64);
					}
				);
				
			F32_abs = Create(0x8b).
				Inputs(1).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Abs32);
					}
				);
				
			F32_neg = Create(0x8c).
				Inputs(1).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Negate();
					}
				);
				
			F32_ceil = Create(0x8d).
				Inputs(1).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
						gen.Call(ReflectionHelpers.Math_Ceiling64);
						gen.ConvertToSingle();
					}
				);
				
			F32_floor = Create(0x8e).
				Inputs(1).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
						gen.Call(ReflectionHelpers.Math_Floor64);
						gen.ConvertToSingle();
					}
				);
				
			F32_trunc = Create(0x8f).
				Inputs(1).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
						gen.Call(ReflectionHelpers.Math_Truncate64);
						gen.ConvertToSingle();
					}
				);
				
			F32_nearest = Create(0x90).
				Inputs(1).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
						gen.Call(ReflectionHelpers.Math_Round64);
						gen.ConvertToSingle();
					}
				);
				
			F32_sqrt = Create(0x91).
				Inputs(1).
				Returns(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
						gen.Call(ReflectionHelpers.Math_Sqrt64);
						gen.ConvertToDouble();
					}
				);
				
			F32_add = Create(0x92).
				AsNumeric(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Add();
					}
				);
				
			F32_sub = Create(0x93).
				AsNumeric(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Subtract();
					}
				);
				
			F32_mul = Create(0x94).
				AsNumeric(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Multiply();
					}
				);
				
			F32_div = Create(0x95).
				AsNumeric(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Divide();
					}
				);
				
			F32_min = Create(0x96).
				AsNumeric(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Min32);
					}
				);
				
			F32_max = Create(0x97).
				AsNumeric(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Max32);
					}
				);
				
			F32_copysign = Create(0x98).
				AsNumeric(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.OpcodeMethods_Copysign32);
					}
				);
				
			F64_abs = Create(0x99).
				Inputs(1).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Abs64);
					}
				);
				
			F64_neg = Create(0x9a).
				Inputs(1).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Negate();
					}
				);
				
			F64_ceil = Create(0x9b).
				Inputs(1).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Ceiling64);
					}
				);
				
			F64_floor = Create(0x9c).
				Inputs(1).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Floor64);
					}
				);
				
			F64_trunc = Create(0x9d).
				Inputs(1).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Truncate64);
					}
				);
				
			F64_nearest = Create(0x9e).
				Inputs(1).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Round64);
					}
				);
				
			F64_sqrt = Create(0x9f).
				Inputs(1).
				Returns(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Sqrt64);
					}
				);
				
			F64_add = Create(0xa0).
				AsNumeric(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Add();
					}
				);
				
			F64_sub = Create(0xa1).
				AsNumeric(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Subtract();
					}
				);
			
			F64_mul = Create(0xa2).
				AsNumeric(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Multiply();
					}
				);
				
			F64_div = Create(0xa3).
				AsNumeric(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Divide();
					}
				);
			
			F64_min = Create(0xa4).
				AsNumeric(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Min64);
					}
				);
				
			F64_max = Create(0xa5).
				AsNumeric(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.Math_Max64);
					}
				);
				
			F64_copysign = Create(0xa6).
				AsNumeric(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.Call(ReflectionHelpers.OpcodeMethods_Copysign64);
					}
				);
			
			/* Conversions */
			
			I32_wrap_I64 = Create(0xa7).
				AsConversion(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToInt32();
					}
				);
				
			I32_trunc_s_F32 = Create(0xa8).
				AsConversion(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToInt32();
					}
				);
				
			I32_trunc_u_F32 = Create(0xa9).
				AsConversion(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToUnsignedInt32();
					}
				);
				
			I32_trunc_s_F64 = Create(0xaa).
				AsConversion(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToInt32();
					}
				);
				
			I32_trunc_u_F64 = Create(0xab).
				AsConversion(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToUnsignedInt32();
					}
				);
				
			I64_extend_s_I32 = Create(0xac).
				AsConversion(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToInt64();
					}
				);
				
			I64_extend_u_I32 = Create(0xad).
				AsConversion(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToUnsignedInt64();
					}
				);
				
			I64_trunc_s_F32 = Create(0xae).
				AsConversion(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToInt64();
					}
				);
				
			I64_trunc_u_F32 = Create(0xaf).
				AsConversion(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToUnsignedInt64();
					}
				);
				
			I64_trunc_s_F64 = Create(0xb0).
				AsConversion(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToInt64();
					}
				);
				
			I64_trunc_u_F64 = Create(0xb1).
				AsConversion(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToUnsignedInt64();
					}
				);
				
			F32_convert_s_I32 = Create(0xb2).
				AsConversion(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToSingle();
					}
				);
				
			F32_convert_u_I32 = Create(0xb3).
				AsConversion(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToSingle();
					}
				);
				
			F32_convert_s_I64 = Create(0xb4).
				AsConversion(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToSingle();
					}
				);
				
			F32_convert_u_I64 = Create(0xb5).
				AsConversion(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToSingle();
					}
				);
				
			F32_demote_F64 = Create(0xb6).
				AsConversion(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToSingle();
					}
				);
				
			F64_convert_s_I32 = Create(0xb7).
				AsConversion(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
					}
				);
				
			F64_convert_u_I32 = Create(0xb8).
				AsConversion(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertUnsignedToDouble();
					}
				);
				
			F64_convert_s_I64 = Create(0xb9).
				AsConversion(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
					}
				);
				
			F64_convert_u_I64 = Create(0xba).
				AsConversion(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertUnsignedToDouble();
					}
				);
				
			F64_promote_F32 = Create(0xbb).
				AsConversion(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						gen.ConvertToDouble();
					}
				);
			
			/* Reinterpretations */
			
			I32_reinterpret_F32 = Create(0xbc).
				AsConversion(typeof(int)).
				OnOutput(
					delegate(ILGenerator gen){
						// Float bits to int.
						gen.NewObject(ReflectionHelpers.FloatBits_ConstructFloat);
						gen.LoadField(ReflectionHelpers.FloatBits_Int);
					}
				);
				
			I64_reinterpret_F64 = Create(0xbd).
				AsConversion(typeof(long)).
				OnOutput(
					delegate(ILGenerator gen){
						// Double bits to long.
						gen.NewObject(ReflectionHelpers.DoubleBits_ConstructDouble);
						gen.LoadField(ReflectionHelpers.DoubleBits_Long);
					}
				);
				
			F32_reinterpret_I32 = Create(0xbe).
				AsConversion(typeof(float)).
				OnOutput(
					delegate(ILGenerator gen){
						// Int bits to float.
						gen.NewObject(ReflectionHelpers.FloatBits_ConstructInt);
						gen.LoadField(ReflectionHelpers.FloatBits_Float);
					}
				);
				
			F64_reinterpret_I64 = Create(0xbf).
				AsConversion(typeof(double)).
				OnOutput(
					delegate(ILGenerator gen){
						// Long bits to double.
						gen.NewObject(ReflectionHelpers.DoubleBits_ConstructULong);
						gen.LoadField(ReflectionHelpers.DoubleBits_Double);
					}
				);
			
		}
		
		private static OpCode Create(ushort opcode){
			return new OpCode(opcode);
		}
		
		/* Control flow operators */
		
		public static readonly OpCode Unreachable;
		public static readonly OpCode Nop;
		public static readonly OpCode Block;
		public static readonly OpCode Loop;
		public static readonly OpCode If;
		public static readonly OpCode Else;
		public static readonly OpCode End;
		public static readonly OpCode Br;
		public static readonly OpCode Br_if;
		public static readonly OpCode Br_table;
		public static readonly OpCode Return;
		
		/* Call operators */
		
		public static readonly OpCode Call;
		public static readonly OpCode Call_indirect;
		
		/* Parametric operators */
		
		public static readonly OpCode Drop;
		public static readonly OpCode Select;
		
		/* Variable access */
		
		public static readonly OpCode Get_local;
		public static readonly OpCode Set_local;
		public static readonly OpCode Tee_local;
		public static readonly OpCode Get_global;
		public static readonly OpCode Set_global;
		
		/* Memory-related operators */
		
		public static readonly OpCode I32_load;
		public static readonly OpCode I64_load;
		public static readonly OpCode F32_load;
		public static readonly OpCode F64_load;
		public static readonly OpCode I32_load8_s;
		public static readonly OpCode I32_load8_u;
		public static readonly OpCode I32_load16_s;
		public static readonly OpCode I32_load16_u;
		public static readonly OpCode I64_load8_s;
		public static readonly OpCode I64_load8_u;
		public static readonly OpCode I64_load16_s;
		public static readonly OpCode I64_load16_u;
		public static readonly OpCode I64_load32_s;
		public static readonly OpCode I64_load32_u;
		public static readonly OpCode I32_store;
		public static readonly OpCode I64_store;
		public static readonly OpCode F32_store;
		public static readonly OpCode F64_store;
		public static readonly OpCode I32_store8;
		public static readonly OpCode I32_store16;
		public static readonly OpCode I64_store8;
		public static readonly OpCode I64_store16;
		public static readonly OpCode I64_store32;
		public static readonly OpCode Current_memory;
		public static readonly OpCode Grow_memory;
		
		/* Constants */
		
		public static readonly OpCode I32_const;
		public static readonly OpCode I64_const;
		public static readonly OpCode F32_const;
		public static readonly OpCode F64_const;
		
		/* Comparison operators */
		
		public static readonly OpCode I32_eqz;
		public static readonly OpCode I32_eq;
		public static readonly OpCode I32_ne;
		public static readonly OpCode I32_lt_s;
		public static readonly OpCode I32_lt_u;
		public static readonly OpCode I32_gt_s;
		public static readonly OpCode I32_gt_u;
		public static readonly OpCode I32_le_s;
		public static readonly OpCode I32_le_u;
		public static readonly OpCode I32_ge_s;
		public static readonly OpCode I32_ge_u;
		public static readonly OpCode I64_eqz;
		public static readonly OpCode I64_eq;
		public static readonly OpCode I64_ne;
		public static readonly OpCode I64_lt_s;
		public static readonly OpCode I64_lt_u;
		public static readonly OpCode I64_gt_s;
		public static readonly OpCode I64_gt_u;
		public static readonly OpCode I64_le_s;
		public static readonly OpCode I64_le_u;
		public static readonly OpCode I64_ge_s;
		public static readonly OpCode I64_ge_u;
		public static readonly OpCode F32_eq;
		public static readonly OpCode F32_ne;
		public static readonly OpCode F32_lt;
		public static readonly OpCode F32_gt;
		public static readonly OpCode F32_le;
		public static readonly OpCode F32_ge;
		public static readonly OpCode F64_eq;
		public static readonly OpCode F64_ne;
		public static readonly OpCode F64_lt;
		public static readonly OpCode F64_gt;
		public static readonly OpCode F64_le;
		public static readonly OpCode F64_ge;
		
		/* Numeric operators */
		
		public static readonly OpCode I32_clz;
		public static readonly OpCode I32_ctz;
		public static readonly OpCode I32_popcnt;
		public static readonly OpCode I32_add;
		public static readonly OpCode I32_sub;
		public static readonly OpCode I32_mul;
		public static readonly OpCode I32_div_s;
		public static readonly OpCode I32_div_u;
		public static readonly OpCode I32_rem_s;
		public static readonly OpCode I32_rem_u;
		public static readonly OpCode I32_and;
		public static readonly OpCode I32_or;
		public static readonly OpCode I32_xor;
		public static readonly OpCode I32_shl;
		public static readonly OpCode I32_shr_s;
		public static readonly OpCode I32_shr_u;
		public static readonly OpCode I32_rotl;
		public static readonly OpCode I32_rotr;
		public static readonly OpCode I64_clz;
		public static readonly OpCode I64_ctz;
		public static readonly OpCode I64_popcnt;
		public static readonly OpCode I64_add;
		public static readonly OpCode I64_sub;
		public static readonly OpCode I64_mul;
		public static readonly OpCode I64_div_s;
		public static readonly OpCode I64_div_u;
		public static readonly OpCode I64_rem_s;
		public static readonly OpCode I64_rem_u;
		public static readonly OpCode I64_and;
		public static readonly OpCode I64_or;
		public static readonly OpCode I64_xor;
		public static readonly OpCode I64_shl;
		public static readonly OpCode I64_shr_s;
		public static readonly OpCode I64_shr_u;
		public static readonly OpCode I64_rotl;
		public static readonly OpCode I64_rotr;
		public static readonly OpCode F32_abs;
		public static readonly OpCode F32_neg;
		public static readonly OpCode F32_ceil;
		public static readonly OpCode F32_floor;
		public static readonly OpCode F32_trunc;
		public static readonly OpCode F32_nearest;
		public static readonly OpCode F32_sqrt;
		public static readonly OpCode F32_add;
		public static readonly OpCode F32_sub;
		public static readonly OpCode F32_mul;
		public static readonly OpCode F32_div;
		public static readonly OpCode F32_min;
		public static readonly OpCode F32_max;
		public static readonly OpCode F32_copysign;
		public static readonly OpCode F64_abs;
		public static readonly OpCode F64_neg;
		public static readonly OpCode F64_ceil;
		public static readonly OpCode F64_floor;
		public static readonly OpCode F64_trunc;
		public static readonly OpCode F64_nearest;
		public static readonly OpCode F64_sqrt;
		public static readonly OpCode F64_add;
		public static readonly OpCode F64_sub;
		public static readonly OpCode F64_mul;
		public static readonly OpCode F64_div;
		public static readonly OpCode F64_min;
		public static readonly OpCode F64_max;
		public static readonly OpCode F64_copysign;
		
		/* Conversions */
		
		public static readonly OpCode I32_wrap_I64;
		public static readonly OpCode I32_trunc_s_F32;
		public static readonly OpCode I32_trunc_u_F32;
		public static readonly OpCode I32_trunc_s_F64;
		public static readonly OpCode I32_trunc_u_F64;
		public static readonly OpCode I64_extend_s_I32;
		public static readonly OpCode I64_extend_u_I32;
		public static readonly OpCode I64_trunc_s_F32;
		public static readonly OpCode I64_trunc_u_F32;
		public static readonly OpCode I64_trunc_s_F64;
		public static readonly OpCode I64_trunc_u_F64;
		public static readonly OpCode F32_convert_s_I32;
		public static readonly OpCode F32_convert_u_I32;
		public static readonly OpCode F32_convert_s_I64;
		public static readonly OpCode F32_convert_u_I64;
		public static readonly OpCode F32_demote_F64;
		public static readonly OpCode F64_convert_s_I32;
		public static readonly OpCode F64_convert_u_I32;
		public static readonly OpCode F64_convert_s_I64;
		public static readonly OpCode F64_convert_u_I64;
		public static readonly OpCode F64_promote_F32;
		
		/* Reinterpretations */
		
		public static readonly OpCode I32_reinterpret_F32;
		public static readonly OpCode I64_reinterpret_F64;
		public static readonly OpCode F32_reinterpret_I32;
		public static readonly OpCode F64_reinterpret_I64;
		
	}
	
}