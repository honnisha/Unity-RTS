using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>WebAssembly OpCode information.</summary>
	public class OpCode{
		
		/// <summary>The WebAssembly numeric opcode.</summary>
		public ushort Code;
		/// <summary>True if this opcode should be read but not created as an actual instruction.</summary>
		public bool Ignored;
		/// <summary>The number of inputs on this opcode. Overrideable by immediate types.</summary>
		public int InputCount;
		/// <summary>The default return type for this opcode. Void means none.</summary>
		public Type ReturnType;
		/// <summary>Used when compiling this opcode to .NET.
		/// Directly maps it through rather than going via e.g. a set of instructions.</summary>
		public Action<ILGenerator> OnOutputIL;
		/// <summary>Used just before outputting an instructions inputs.</summary>
		public Action<ILGenerator> OnBeforeOutputIL;
		/// <summary>Used when reading this opcode from the bytestream.</summary>
		public ImmediateReader OnReadImmediates;
		
		/// <summary>True if this OpCode has any 'immediates' (extra arguments).</summay>
		public bool HasImmediates{
			get{
				return (OnReadImmediates!=null);
			}
		}
		
		/// <summary>True if this OpCode can be present in an Init statement.</summary>
		public bool AllowedInInit;
		
		public OpCode(ushort wasmOpCode):this(wasmOpCode,typeof(void)){}
		
		public OpCode(ushort wasmOpCode,Type returnType){
			Code = wasmOpCode;
			ReturnType = returnType;
			
			// Add to OpCode set:
			OpCodes.All[Code] = this;
		}
		
		/// <summary>Sets the output action.</summary>
		public OpCode OnOutput(Action<ILGenerator> action){
			OnOutputIL = action;
			return this;
		}
		
		/// <summary>Sets the before output action.</summary>
		public OpCode OnBeforeOutput(Action<ILGenerator> action){
			OnBeforeOutputIL = action;
			return this;
		}
		
		/// <summary>Sets the on immediates action.</summary>
		public OpCode OnImmediates(ImmediateReader action){
			OnReadImmediates = action;
			return this;
		}
		
		/// <summary>May be used in initialisers.</summary>
		public OpCode CanInit(){
			AllowedInInit=true;
			return this;
		}
		
		/// <summary>Ignores this opcode.
		/// It's parsed but isn't added to the instruction stack.</summary>
		public OpCode Ignore(){
			Ignored = true;
			return this;
		}
		
		/// <summary>Sets the return type.</summary>
		public OpCode Returns(Type type){
			ReturnType = type;
			return this;
		}
		
		/// <summary>Sets the input count.</summary>
		public OpCode Inputs(int count){
			InputCount = count;
			return this;
		}
		
		/// <summary>2 inputs and a return type of int.</summary>
		public OpCode AsComparison(){
			InputCount = 2;
			ReturnType = typeof(int);
			return this;
		}
		
		/// <summary>As a conversion. One input and a return of the given type.</summary>
		public OpCode AsConversion(Type toType){
			InputCount = 1;
			ReturnType = toType;
			return this;
		}
		
		/// <summary>Specified inputs and a return type of int.</summary>
		public OpCode AsComparison(int inputCount){
			InputCount = inputCount;
			ReturnType = typeof(int);
			return this;
		}
		
		/// <summary>2 inputs and a return type as specified</summary>
		public OpCode AsNumeric(Type returnType){
			InputCount = 2;
			ReturnType = returnType;
			return this;
		}
		
		/// <summary>Reads the immediates. Null if it has none.</summary>
		public object ReadImmediates(Reader reader){
			
			if(OnReadImmediates==null){
				return null;
			}
			
			// Read it:
			return OnReadImmediates(reader);
		}
		
		/*
		/// <summary>Reads this opcode from the given reader.
		/// Not actually used for codegen (reference only).</summary>
		public Instruction Read(Reader reader){
			
			if(Immediates_!=null){
				
				// Read all the immediates:
				for(int i=0;i<Immediates_.Length;i++){
					Immediates_.OnRead(reader);
				}
				
			}
			
		}
		*/
		
	}
	
}