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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the stroke: css property.
	/// </summary>
	
	public class Stroke:CssProperty{
		
		public static Stroke GlobalProperty;
		
		
		public Stroke(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"stroke"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Got a value?
			if(value!=null && !(value.IsType(typeof(Css.Keywords.None)))){
				
				// Cache the value now:
				style[this]=new TextureNodeValue(value);
				
				// Must reload it:
				return ApplyState.ReloadValue;
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



