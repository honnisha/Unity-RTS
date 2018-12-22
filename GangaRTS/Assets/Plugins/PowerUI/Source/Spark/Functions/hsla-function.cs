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
using Css.Units;


namespace Css.Functions{
	
	/// <summary>
	/// Represents the hsl() and hsla() css functions.
	/// </summary>
	
	public class Hsla:CssFunction{
		
		
		public Hsla(){
			
			Name="hsla";
			Type=ValueType.Set;
			
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			int length=Count;
			
			if(length<3){
				// Bad call!
				return;
			}
			
			// Re-map the parameters now:
			float h=this[0].GetRawDecimal() / 360f;
			float s=this[1].GetRawDecimal();
			float l=this[2].GetRawDecimal();
			
			if(s==0f){
				this[0].SetRawDecimal(l);
				this[1].SetRawDecimal(l);
				this[2].SetRawDecimal(l);
			}else{
				float q;
				
				if(l < 0.5f){
					q=l * (1f + s);
				}else{
					q=l + s - l * s;
				}
				
				float p = 2f * l - q;
				this[0].SetRawDecimal(HueToRgb(p, q, h + 1f/3f));
				this[1].SetRawDecimal(HueToRgb(p, q, h));
				this[2].SetRawDecimal(HueToRgb(p, q, h - 1f/3f));
			}
			
		}
		
		private float HueToRgb(float p,float q,float t){
			
			if(t < 0f){
				t += 1f;
			}else if(t > 1f){
				t -= 1f;
			}

			if(t < 1f/6f){
				return p + (q - p) * 6f * t;
			}
			
			if(t < 1f/2f){
				return q;
			}
			
			if(t < 2f/3f){
				return p + (q - p) * (2f/3f - t) * 6f;
			}
			
			return p;
		}
		
		public override string[] GetNames(){
			return new string[]{"hsl","hsla"};
		}
		
		protected override Css.Value Clone(){
			Hsla result=new Hsla();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}