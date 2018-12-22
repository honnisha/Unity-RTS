using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>A local in a WebAssembly function.</summary>
	public class FunctionLocal{
		
		/// <summary>The local type.</summary>
		public Type Type;
		/// <summary>The compiling variable. Created only when the function starts compiling.</summary>
		internal ILLocalVariable CompilerVariable;
		
		
		public FunctionLocal(Type type){
			Type=type;
		}
		
	}
	
	/// <summary>A WebAssembly function body.</summary>
	public class FunctionBody{
		
		/// <summary>The signature.</summary>
		public FuncType Signature;
		/// <summary>The types of local variables.</summary>
		public List<FunctionLocal> Locals;
		/// <summary>The stack which we add the instructions to.</summary>
		public CompilerStack<Instruction> Root = new CompilerStack<Instruction>();
		
		/// <summary>Is the given index a local var? False if it's a parameter.</summary>
		public bool IsLocal(int index){
			return index >= Signature.ParameterCount;
		}
		
		/// <summary>Gets the local info for the given index.
		/// Note: Due to parameters, it's *not* Locals[index].</summary>
		public FunctionLocal GetLocal(int index){
			return Locals[index - Signature.ParameterCount];
		}
		
		/// <summary>Defines the locals into the given generator. CompilerVariable is available after this.</summary>
		public void DefineLocals(ILGenerator gen){
			
			for(int i=0;i<Locals.Count;i++){
				FunctionLocal local = Locals[i];
				local.CompilerVariable = gen.DeclareVariable(local.Type,null);
			}
			
		}
		
		public FunctionBody(){}
		
		public FunctionBody(Reader reader,int index){
			
			// Apply current body:
			reader.FunctionBody = this;
			
			// Clear the block stack:
			reader.BlockStack.Count = 0;
			
			// Get the sig:
			Signature = reader.Module.FunctionSection.Types[index];
			
			// Sizes:
			int bodySize=(int)reader.VarUInt32();
			
			// Max (bodySize includes locals); -1 to exclude the final byte:
			long max = reader.Position + bodySize - 1;
			
			// Locals:
			int localCount=(int)reader.VarUInt32();
			
			List<FunctionLocal> ilLocal = new List<FunctionLocal>(localCount);
			Locals = ilLocal;
			
			for(int i=0;i<localCount;i++){
				
				// # of this and its type:
				int count = (int)reader.VarUInt32();
				Type type = reader.ValueTypeConverted();
				
				for(int c=0;c<count;c++){
					// Define it:
					ilLocal.Add(new FunctionLocal(type));
				}
				
			}
			
			// Create the root block instruction (no opcode):
			Root = new CompilerStack<Instruction>();
			
			// While there is more code..
			while(reader.Position < max){
				
				// Opcode:
				OpCode opcode=OpCodes.Get(reader.ReadByte());
				
				if(opcode==null || opcode.OnOutputIL==null){
					throw new Exception("WebAssembly function body went out of alignment (Function "+Signature.Index+")");
				}
				
				if(opcode.Ignored){
					
					// Don't create - just load immediates:
					if(opcode.HasImmediates){
						opcode.ReadImmediates(reader);
					}
					
				}else{
					
					// Create an instruction:
					Instruction instruction = new Instruction();
					instruction.OpCode = opcode;
					
					// Apply default in/out:
					instruction.InputCount = opcode.InputCount;
					instruction.ReturnType = opcode.ReturnType;
					reader.Instruction = instruction;
					
					// Add all of its immediates:
					if(opcode.HasImmediates){
						
						// Load the immediates now:
						instruction.Immediates = opcode.ReadImmediates(reader);
						
					}
					
					// Manage the stack now - if it has a fixed number of inputs 
					// we pop those from the stack and add them to the instruction:
					for(int i=0;i<instruction.InputCount;i++){
						
						// Add the input to the front of the input set:
						instruction.PrependInput(Root.Pop());
						
					}
					
					if(instruction.ReturnType!= typeof(void)){
						// Push to Root:
						Root.Push(instruction);
					}
					
				}
				
			}
			
			if(reader.ReadByte()!=0x0b){
				
				throw new Exception("A function body did not end correctly.");
				
			}
			
		}
		
	}
	
}