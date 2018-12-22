//--------------------------------------
//           Property values 
// standard set of referenceable values
//   Used mainly by Blade and Loonim.
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using BinaryIO;
using UnityEngine;


namespace Values{
	
	/// <summary>
	/// Holds a 'live' image (either a texture2D or a render texture).
	/// </summary>
	
	public class TextureValue:PropertyValue{
		
		internal Texture Value_;
		
		
		public Texture Value{
			get{
				return Value_;
			}
			set{
				if(value==Value_){
					return;
				}
				
				Value_=value;
				Changed=true;
			}
		}
		
		
		public TextureValue(){}
		
		public TextureValue(Texture texture){
			Value_=texture;
			Changed=false;
		}
		
		public override int GetID(){
			return 206;
		}
		
		public override PropertyValue Create(){
			return new TextureValue();
		}
		
		public override PropertyValue Copy(){
			
			TextureValue value=new TextureValue();
			value.Value_=Value_;
			return value;
			
		}
		
		public override void Read(Reader reader){
			
			int size=(int)reader.ReadCompressed();
			byte[] pngData=reader.ReadBytes(size);
			
			// Create and load:
			Texture2D tex=new Texture2D(0,0);
			tex.LoadImage(pngData);
			Value_=tex;
			
		}
		
		public override void Write(Writer writer){
			
			// Write as a PNG:
			byte[] data=(Value_ as Texture2D).EncodeToPNG();
			writer.WriteCompressed((uint)data.Length);
			writer.Write(data);
			
		}
		
	}
	
}