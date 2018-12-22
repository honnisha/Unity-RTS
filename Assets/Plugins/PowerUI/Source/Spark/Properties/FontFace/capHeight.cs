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
	/// Represents the cap-height: css property.
	/// </summary>
	
	public class CapHeight:CssProperty{
		
		public CapHeight(){
			
			// This is along the y axis:
			Axis=ValueAxis.Y;
			NamespaceName="svg";
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"cap-height"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



