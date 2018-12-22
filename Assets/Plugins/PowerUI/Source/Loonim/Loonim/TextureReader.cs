using System;
using Blaze;
using BinaryIO;
using System.IO;
using Values;


namespace Loonim{
	
	/// <summary>
	/// A loader which is used to load textures.
	/// </summary>
	
	public class TextureReader:Reader{
	
		/// <summary>The current surface being read.</summary>
		public SurfaceTexture CurrentSurface;
		
		public TextureReader(byte[] str):base(str){}
		public TextureReader(Stream str):base(str){}
		
		
		public TextureNode ReadTextureNode(){
			
			// Get the type:
			int type=(int)ReadCompressed();
			
			if(type==0){
				return null;
			}
			
			
			// Get the node meta:
			TextureNodeMeta meta=TextureNodes.Get(type);
			
			// Instance it:
			TextureNode instance=meta.GetInstance();
			
			// Load now:
			instance.Read(this);
			
			return instance;
			
		}
		
		/// <summary>Reads a value from the stream.</summary>
		public virtual PropertyValue ReadPropertyValue(){
			
			// What value type is it?
			int type=(int)ReadCompressed();
			
			// Create it:
			PropertyValue value=Values.PropertyValues.Get(type);
			
			// Read it:
			value.Read(this);
			
			return value;
		}
		
	}
	
}