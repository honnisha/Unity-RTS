using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>A delegate used to read some 'immediate' value from the given reader.</summary>
	public delegate object ImmediateReader(Reader reader);
	
	/// <summary>Various types of 'immediate' values. A WebAssembly 'immediate' is
	/// essentially some additional argument for an opcode.</summary>
	public class ImmediatesType{
		
		/// <summary>Reads a block_type (represented as a LanguageType).</summary>
		public static readonly ImmediateReader Block = delegate(Reader reader){
			// Blocks return type:
			Type returnType = reader.ValueTypeConverted();
			
			// Push a block now:
			return reader.PushBlock(returnType);
			
		};
		
		/// <summary>Reads a local/ arg reference and sets the instructions return type.</summary>
		public static readonly ImmediateReader LocalWithReturn = delegate(Reader reader){
			
			// param/local index:
			int index=(int)reader.VarUInt32();
			
			if( reader.FunctionBody.IsLocal(index) ){
				// Read local:
				FunctionLocal local = reader.FunctionBody.GetLocal(index);
				
				// Update the stack:
				reader.Instruction.ReturnType = local.Type;
				return local;
			}else{
				
				// Update the type stack:
				reader.Instruction.ReturnType = reader.FunctionBody.Signature.GetParameterType(index);
				
				return index;
			}
			
		};
		
		/// <summary>Reads a br_table immediate.</summary>
		public static readonly ImmediateReader BrTable = delegate(Reader reader){
			
			// Read the targets:
			LabelledBlock[] targets = new LabelledBlock[(int)reader.VarUInt32()];
			
			for(int i=0;i<targets.Length;i++){
				targets[i] = reader.ReadLabelledBlock();
			}
			
			// The default target:
			LabelledBlock defaultTarget = reader.ReadLabelledBlock();
			
			return new BrTableImmediate(
				targets,
				defaultTarget
			);
		};
		
		/// <summary>Reads a varuint1.</summary>
		public static readonly ImmediateReader VarUInt1 = delegate(Reader reader){
			return reader.VarUInt1();
		};
			
		/// <summary>Reads a varuint7.</summary>
		public static readonly ImmediateReader VarUInt7 = delegate(Reader reader){
			return reader.VarUInt7();
		};
		
		/// <summary>Reads a varint32.</summary>
		public static readonly ImmediateReader VarInt32 = delegate(Reader reader){
			return reader.VarInt32();
		};
		
		/// <summary>Reads a LabelledBlock.</summary>
		public static readonly ImmediateReader Label = delegate(Reader reader){
			return reader.ReadLabelledBlock();
		};
		
		/// <summary>Reads a .</summary>
		public static readonly ImmediateReader Method = delegate(Reader reader){
			int paramCount;
			Type returnType;
			MethodInfo method = reader.Method(out paramCount,out returnType);
			
			// Set input count:
			reader.Instruction.InputCount = paramCount;
			
			// Update return type too:
			reader.Instruction.ReturnType = returnType;
			
			return method;
		};
		
		/// <summary>Reads a varuint32.</summary>
		public static readonly ImmediateReader VarUInt32 = delegate(Reader reader){
			return reader.VarUInt32();
		};
	
		/// <summary>Reads a varint64.</summary>
		public static readonly ImmediateReader VarInt64 = delegate(Reader reader){
			return reader.VarInt64();
		};
		
		/// <summary>memory_immediate (as used by load instructions).</summary>
		public static readonly ImmediateReader MemoryLoad = delegate(Reader reader){
			
			// Always one input:
			reader.Instruction.InputCount = 1;
			
			return reader.Memory();
		};
		
		/// <summary>memory_immediate (as used by store instructions).</summary>
		public static readonly ImmediateReader MemoryStore = delegate(Reader reader){
			// Always two inputs:
			reader.Instruction.InputCount = 2;
			
			MemoryImmediate mem = reader.Memory();
			
			// Convert mem.Offset into an add on the first input here.
			// Instruction address = reader.FunctionBody.Root.Peek(1);
			if(mem.Offset!=0){
				throw new NotImplementedException("Can't use offsets with store opcodes at the moment.");
			}
			
			return mem;
		};
		
		/// <summary>uint32 interpreted as a single (float).</summary>
		public static readonly ImmediateReader Single = delegate(Reader reader){
			return reader.ReadSingle();
		};
		
		/// <summary>uint64 interpreted as a double.</summary>
		public static readonly ImmediateReader Double = delegate(Reader reader){
			return reader.ReadDouble();
		};
	
	}
	
}