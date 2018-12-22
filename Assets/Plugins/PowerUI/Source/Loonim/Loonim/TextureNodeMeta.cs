using System;


namespace Loonim{
	
	public class TextureNodeMeta{
		
		/// <summary>The node ID.</summary>
		public int ID;
		/// <summary>The node type.</summary>
		public Type Type;
		/// <summary>The node name.</summary>
		public string Name;
		
		/// <summary>Creates an instance of this type.</summary>
		public TextureNode GetInstance(){
			
			return Activator.CreateInstance(Type) as TextureNode;
			
		}
		
	}
	
}