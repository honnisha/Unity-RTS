using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>A WebAssembly start section.</summary>
	public class StartSection : Section{
		
		/// <summary>Function index.</summary>
		public uint Index;
		
		
		public StartSection():base(8){}
		
		public StartSection(bool empty):base(8){
			if(empty){
				Index=uint.MaxValue;
			}
		}
		
		
		public override void Load(Reader reader,int length){
			
			// Just an index:
			Index=reader.VarUInt32();
			
		}
		
		/// <summary>Compiles the main method.</summary>
		public void Compile(){
			
			// Create the main method builder now:
			var main = Module.DefineMethod("__.main",typeof(void),new Type[0]);
			
			// Invocation for the 'start' method:
			if(Index!=uint.MaxValue){
				
				// Get the function:
				int paramCount;
				Type returnType;
				MethodInfo method = Module.GetFunction((int)Index,out paramCount,out returnType);
				
				if(method!=null){
					
					// It shouldn't take any args:
					if(paramCount!=0 || returnType!=typeof(void)){
						
						throw new InvalidOperationException(
							"WebAssembly start method must not have params or a return type."
						);
						
					}
					
					// Call as-is:
					main.Call(method);
					
				}
				
			}
			
			// Done!
			main.Complete("__.main",null,typeof(void),true);
			
		}
		
	}
	
}