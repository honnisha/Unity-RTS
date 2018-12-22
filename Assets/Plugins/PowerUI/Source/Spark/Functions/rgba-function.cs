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
using Css;
using Css.Units;


namespace Css.Functions{
	
	/// <summary>
	/// Represents the rgb() and rgba() css functions.
	/// </summary>
	
	public class RgbaFunction:CssFunction{
		
		
		public RgbaFunction(){
			
			Name="rgba";
			Type=ValueType.Set;
			
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			int length=Count;
			
			if(length==0){
				return;
			}
			
			bool mapAll=true;
			
			// If any of the parameters are decimals or are just 0 and 1, remap them to INTS.
			for(int i=0;i<length;i++){
				
				// Get the decimal value [either a percentage or a 0-1 value]:
				DecimalUnit dec=this[i] as DecimalUnit;
				
				if(dec==null){
					continue;
				}
				
				if(dec.RawValue>1f){
					// No remapping required.
					mapAll=false;
					break;
				}
				
			}
			
			// Re-map the parameters now:
			for(int i=0;i<length;i++){
				
				// Get the decimal value:
				DecimalUnit dec=this[i] as DecimalUnit;
				
				if(dec==null){
					continue;
				}
				
				if(i==3 || dec.RawValue<1f || mapAll){
				}else{
					dec.RawValue /= 255f;
				}
				
			}
			
		}
		
		public override string[] GetNames(){
			return new string[]{"rgb","rgba"};
		}
		
		protected override Css.Value Clone(){
			RgbaFunction result=new RgbaFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override bool IsColour{
			get{
				return true;
			}
		}
		
	}
	
}



