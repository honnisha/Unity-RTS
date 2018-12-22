using System;
using BinaryIO;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;


namespace Loonim{
	
	/// <summary>
	/// Used to load procedural texture descriptions from the binary format.
	/// They're made up of a series of "nodes" called modules. Each module has it's own loader.
	/// Some modules make use of 2D graphs - these graphs use graph loaders, and the concept there 
	/// is exactly the same too; i.e. each graph can actually be a series of conneted modules too.
	/// Both graph modules and texture modules are dealt with here. The functions which deal with this
	/// at a more abstract level are in TextureReader.cs
	/// </summary>
	
	public static class TextureNodes{
		
		/// <summary>The global instance set of all loaders.</summary>
		public static Dictionary<int,TextureNodeMeta> All;
		
		
		/// <summary>Adds the given loader to the global set.</summary>
		private static void Add(Type type){
			
			if(All==null){
				All=new Dictionary<int,TextureNodeMeta>();
			}
			
			// Create an instance to pull TypeID and any other settings from:
			TextureNode node=Activator.CreateInstance(type) as TextureNode;
			
			int id=node.TypeID;
			
			if(id==-1){
				return;
			}
			
			// Create the meta:
			TextureNodeMeta meta=new TextureNodeMeta();
			meta.Type=type;
			meta.ID=id;
			meta.Name=type.Name;
			
			// Push to set:
			All[id]=meta;
			
		}
		
		/// <summary>Gets a loader by ID.</summary>
		public static TextureNodeMeta Get(int id){
			
			if(All==null){
				
				// Load them now!
				Modular.AssemblyScanner.FindAllSubTypesNow(typeof(TextureNode),
					delegate(Type t){
						// Add it as a node:
						Add(t);
					}
				);
				
			}
			
			TextureNodeMeta type;
			All.TryGetValue(id,out type);
			
			return type;
			
		}
		
	}
	
}