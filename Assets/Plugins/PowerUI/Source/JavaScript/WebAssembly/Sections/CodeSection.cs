using System;
using System.IO;


namespace WebAssembly{
	
	/// <summary>A WebAssembly code section.</summary>
	public class CodeSection : Section{
		
		/// <summary>Function bodies.</summary>
		public FunctionBody[] Bodies;
		
		
		public CodeSection():base(10){}
		
		
		public override void Load(Reader reader,int length){
			
			// Create set:
			Bodies=new FunctionBody[(int)reader.VarUInt32()];
			
			for(int i=0;i<Bodies.Length;i++){
				
				// Load it:
				Bodies[i]=new FunctionBody(reader,i);
				
			}
			
		}
		
		/// <summary>Compiles all the methods in this code section.</summary>
		public void Compile(){
			
			if(Bodies==null){
				return;
			}
			
			for(int b=0;b<Bodies.Length;b++){
				
				// Get the body:
				FunctionBody body=Bodies[b];
				
				// Get the function signature:
				FuncType sig = body.Signature;
				
				// Create the builder:
				ILGenerator gen = sig.GetGenerator(Module);
				
				// Define the locals now:
				body.DefineLocals(gen);
				
				// Set body to the generator:
				gen.FunctionBody = body;
				
				// For each instruction in the input set, output it now:
				for(int i=0;i<body.Root.Count;i++){
					
					// Get the instruction:
					Instruction instruction = body.Root[i];
					
					// Emit it:
					instruction.Output(gen);
					
				}
				
				// Complete it:
				gen.Complete(sig.Name,sig.GetParameters(),sig.GetReturnType(),true);
				
			}
			
		}
		
	}
	
}