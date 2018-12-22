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
	/// Represents the flood-color: css property.
	/// </summary>
	
	public class FloodColor:CssProperty{
		
		public static FloodColor GlobalProperty;
		
		
		public FloodColor(){
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="black";
		}
		
		public override string[] GetProperties(){
			return new string[]{"flood-color"};
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



