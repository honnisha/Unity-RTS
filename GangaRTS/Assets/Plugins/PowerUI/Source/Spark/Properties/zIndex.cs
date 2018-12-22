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
	/// Represents the z-index: css property.
	/// </summary>
	
	public class ZIndex:CssProperty{
		
		public static ZIndex GlobalProperty;
		
		
		public ZIndex(){
			GlobalProperty=this;
			InitialValue=AUTO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-z-index"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



