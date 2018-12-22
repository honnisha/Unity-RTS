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
	/// Holds a colour.
	/// </summary>
	
	public class ColourValue:PropertyValue{
		
		internal Color Value_;
		
		public Color Value{
			get{
				return Value_;
			}
			set{
				if(value.r==Value_.r && value.g==Value_.g && value.b==Value_.b && value.a==Value_.a){
					return;
				}
				
				Value_=value;
				Changed=true;
			}
		}
		
		public ColourValue(){}
		
		public ColourValue(Color32 value){
			Value_=value;
		}
		
		public ColourValue(string hexString){ // E.g. #000000 or just 000000
			
			if(hexString.StartsWith("#")){
				hexString=hexString.Substring(1);
			}
			
			byte r=255;
			byte g=255;
			byte b=255;
			byte a=255;
			
			if(hexString.Length>=2){
				
				// Read R:
				r=Hex.ToByte(hexString.Substring(0,2));
				
			}
			
			if(hexString.Length>=4){
				
				// Read G:
				g=Hex.ToByte(hexString.Substring(2,2));
				
			}
			
			if(hexString.Length>=6){
				
				// Read B:
				b=Hex.ToByte(hexString.Substring(4,2));
				
			}
			
			if(hexString.Length>=8){
				
				// Read A:
				a=Hex.ToByte(hexString.Substring(6,2));
				
			}
			
			Value_=new Color32(r,g,b,a);
			
		}
		
		public override int GetID(){
			return 16;
		}
		
		public override PropertyValue Create(){
			return new ColourValue();
		}
		
		public override PropertyValue Copy(){
			
			ColourValue value=new ColourValue();
			value.Value_=Value_;
			return value;
			
		}
		
		public override void Read(Reader reader){
			
			byte r=reader.ReadByte();
			byte g=reader.ReadByte();
			byte b=reader.ReadByte();
			byte a=reader.ReadByte();
			
			Value_=new Color32(r,g,b,a);
			
		}
		
		public override void Write(Writer writer){
			
			writer.WriteByte((byte)(Value_.r * 255f));
			writer.WriteByte((byte)(Value_.g * 255f));
			writer.WriteByte((byte)(Value_.b * 255f));
			writer.WriteByte((byte)(Value_.a * 255f));
			
		}
		
	}
	
}