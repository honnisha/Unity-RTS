using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>A WebAssembly global section.</summary>
	public class GlobalSection : Section{
		
		/// <summary>All globals in this section.</summary>
		public GlobalVariable[] Globals;
		
		
		public GlobalSection():base(6){}
		
		
		public override void Load(Reader reader,int length){
			
			// Create set:
			Globals=new GlobalVariable[(int)reader.VarUInt32()];
			
			for(int i=0;i<Globals.Length;i++){
				
				// Load it:
				Globals[i]=new GlobalVariable(reader,i);
				
			}
			
		}
		
		/// <summary>Compiles all the globals in this code section (sets up their field and initialiser).</summary>
		public void Compile(ILGenerator gen){
			
			if(Globals==null){
				return;
			}
			
			for(int i=0;i<Globals.Length;i++){
				
				// Get the gv:
				GlobalVariable global = Globals[i];
				
				// Field:
				Type type;
				FieldInfo field = global.GetField(Module,out type);
				
				object init=global.Init;
				
				if(init!=null){
					
					// Apply the initialiser now:
					
					if(init is FieldInfo){
						
						// Global (must be imported)
						gen.LoadField(init as FieldInfo);
						
					}else{
						
						// Must be one of the four constant expr's:
						if(init is int){
							gen.LoadInt32((int)init);
						}else if(init is long){
							gen.LoadInt64((long)init);
						}else if(init is float){
							gen.LoadSingle((float)init);
						}else if(init is double){
							gen.LoadDouble((double)init);
						}
						
					}
					
					gen.StoreField(field);
					
				}
				
			}
			
		}
		
	}
	
}