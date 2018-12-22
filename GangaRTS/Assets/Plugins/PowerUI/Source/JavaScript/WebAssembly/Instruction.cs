using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A parsed WebAssembly instruction.
	/// This primarily uses linked lists as some of the most common instructions
	/// frequently insert into the s-expr.</summary>
	public class Instruction{
		
		/// <summary>The instructions opcode.</summary>
		public OpCode OpCode;
		/// <summary>The instructions output target.</summary>
		public Instruction Target;
		/// <summary>The inputs as a linked list.</summary>
		public Instruction LastInput;
		/// <summary>The inputs as a linked list.</summary>
		public Instruction FirstInput;
		/// <summary>The next instruction in the list.</summary>
		public Instruction SiblingAfter;
		/// <summary>The previous instruction in the list.</summary>
		public Instruction SiblingBefore;
		/// <summary>The immediate(s).</summary>
		public object Immediates;
		/// <summary>The required number of inputs (used during parsing only).</summary>
		internal int InputCount;
		/// <summary>The return type. Void for none. (used during parsing only).</summary>
		internal Type ReturnType;
		
		
		/// <summary>Adds the given instruction as an input to this one.</summary>
		public void AddInput(Instruction instruction){
			instruction.Target = this;
			
			if(FirstInput==null){
				FirstInput = instruction;
				LastInput = instruction;
				instruction.SiblingBefore = null;
			}else{
				instruction.SiblingBefore = LastInput;
				LastInput.SiblingAfter = instruction;
				LastInput = instruction;
			}
			
			instruction.SiblingAfter=null;	
		}
		
		/// <summary>Adds the given instruction as an input to this one (pushing to the start)</summary>
		public void PrependInput(Instruction instruction){
			instruction.Target = this;
			
			if(FirstInput==null){
				FirstInput = instruction;
				LastInput = instruction;
				instruction.SiblingAfter = null;
			}else{
				instruction.SiblingAfter = FirstInput;
				FirstInput.SiblingBefore = instruction;
				FirstInput = instruction;
			}
			
			instruction.SiblingBefore = null;	
		}
		
		/// <summary>Outputs this instruction.</summary>
		public void Output(ILGenerator generator){
			
			// Set current instruction:
			generator.Instruction = this;
			
			// Call before:
			if(OpCode.OnBeforeOutputIL!=null){
				
				// Call the before output method:
				OpCode.OnBeforeOutputIL(generator);
				
			}
			
			// Output the input set:
			Instruction input = FirstInput;
			
			while(input!=null){
				
				// Output it too:
				input.Output(generator);
				
				// Next one:
				input = input.SiblingAfter;
				
			}
			
			// Reset current instruction:
			generator.Instruction = this;
			
			if(OpCode.OnOutputIL!=null){
				
				// Call the output method:
				OpCode.OnOutputIL(generator);
				
			}
			
		}
		
	}
	
}