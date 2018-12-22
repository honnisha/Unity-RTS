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
	/// Represents the stemh: css property.
	/// </summary>
	
	public class Stemh:CssProperty{
		
		public Stemh(){
			
			// This is along the x axis:
			Axis=ValueAxis.X;
			NamespaceName="svg";
		
		}
		
		public override string[] GetProperties(){
			return new string[]{"stemh"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



