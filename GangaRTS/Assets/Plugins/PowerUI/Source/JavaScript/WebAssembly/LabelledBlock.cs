using System;
using System.Collections;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>
	/// A block which has a label and an optional return type.
	/// </summary>
	public class LabelledBlock{
		
		/// <summary>The label pointing at the start of this block.
		/// Created during the emit process.</summary>
		public ILLabel Label;
		/// <summary>The optional return type. Void for none.</summary>
		public Type ReturnType;
		/// <summary>The block instruction.</summary>
		public Instruction Instruction;
		
		
		public LabelledBlock(Type returnType,Instruction instruction){
			Instruction = instruction;
			ReturnType = returnType;
		}
		
		
		/// <summary>Called just before outputting a block.</summary>
		public static void Prepare(ILGenerator gen){
			// Create and mark the label:
			ILLabel label = gen.DefineLabelPosition();
			
			// LabelledBlock is the immediate:
			LabelledBlock block = (LabelledBlock)(gen.Instruction.Immediates);
			
			// Set the label:
			block.Label = label;
		}
		
		/// <summary>Called just after outputting a block.</summary>
		public static void Complete(ILGenerator gen){
			// LabelledBlock is the immediate:
			LabelledBlock block = (LabelledBlock)(gen.Instruction.Immediates);
			
			// Got a return?
			if(block.ReturnType != typeof(void)){
				
				// Yep! Add a return statement:
				gen.Return();
				
			}
		}
		
	}
	
}	