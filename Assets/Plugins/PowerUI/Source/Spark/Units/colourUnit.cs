//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;


namespace Css.Units{
	
	/// <summary>
	/// Represents an instance of a floating point value.
	/// </summary>
	
	public class ColourUnit:ValueSet{
		
		public ColourUnit(){}
		
		public ColourUnit(Color col){
			
			Count=4;
			
			this[0]=new DecimalUnit(col.r);
			this[1]=new DecimalUnit(col.g);
			this[2]=new DecimalUnit(col.b);
			this[3]=new DecimalUnit(col.a);
			
		}
		
		public ColourUnit(float r,float g,float b,float a){
			
			Count=4;
			
			this[0]=new DecimalUnit(r);
			this[1]=new DecimalUnit(g);
			this[2]=new DecimalUnit(b);
			this[3]=new DecimalUnit(a);
			
		}
		
		public ColourUnit(float r,float g,float b){
			
			Count=4;
			
			this[0]=new DecimalUnit(r);
			this[1]=new DecimalUnit(g);
			this[2]=new DecimalUnit(b);
			this[3]=new DecimalUnit(1f);
			
		}
		
		public override Value ReadStartValue(CssLexer lexer){
			
			// Skip the hash:
			lexer.Read();
			
			if(lexer.SelectorMode){
				// Just a hash (ID selector):
				return new TextUnit("#");
			}
			
			// Read the hex string now:
			string hex=lexer.ReadString();
			
			ColourUnit result=new ColourUnit();
			result.SetHex(hex);
			return result;
		}
		
		/// <summary>Sets this colour from the given hex string.</summary>
		public void SetHex(string hex){
			
			int r;
			int g;
			int b;
			int a;
			ColourMap.GetHexColour(hex,out r,out g,out b,out a);
			
			Count=4;
			
			this[0]=new DecimalUnit((float)r/255f);
			this[1]=new DecimalUnit((float)g/255f);
			this[2]=new DecimalUnit((float)b/255f);
			this[3]=new DecimalUnit((float)a/255f);
			
		}
		
		/// <summary>Checks if this is a suitable colour.</summary>
		public override bool IsColour{
			get{
				return true;
			}
		}
		
		/// <summary>The text that occurs before one of these in the stream.</summary> 
		public override string[] PreText{
			get{
				return new string[]{"#"};
			}
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			return ToString();
		}
		
		public override string ToString(){
			if(Count==3){
				return "rgb("+this[0]+","+this[1]+","+this[2]+")";
			}
			return "rgba("+this[0]+","+this[1]+","+this[2]+","+this[3]+")";
		}
		
		/// <summary>Gets a 2 character hex value.</summary>
		private string ToHex(Css.Value value){
			
			int v=((int)(value.GetDecimal(null,null) * 255f));
			
			// Clip:
			if(v>255){
				v=255;
			}else if(v<0){
				v=0;
			}
			
			return v.ToString("X2");
		}
		
		public override string HexString{
			get{
				
				if(Values==null){
					return "000000";
				}
				
				int count=Count;
				
				string result=ToHex(this[0])+ToHex(this[1])+ToHex(this[2]);
				Value alpha=this[3];
				
				if(alpha!=null && count==4){
					
					string alphaStr=ToHex(alpha);
					
					if(alphaStr!="FF"){
						result+=alphaStr;
					}
					
				}
				
				return result;
			}
		}
		
		protected override Value Clone(){
			ColourUnit result=new ColourUnit();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



