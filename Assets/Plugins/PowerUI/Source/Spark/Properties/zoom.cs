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
	/// Represents the zoom: css property.
	/// </summary>
	
	public class Zoom:CssProperty{
		
		public static Zoom GlobalProperty;
		
		
		public Zoom(){
			GlobalProperty=this;
			InitialValue=AUTO;
			Inherits=true;
			RelativeTo=ValueRelativity.None;
		}
		
		public override string[] GetProperties(){
			return new string[]{"zoom"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



