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
	/// Represents the clip-rule: css property.
	/// </summary>
	
	public class ClipRule:CssProperty{
		
		public ClipRule(){
			NamespaceName="svg";
		}
		
		public override string[] GetProperties(){
			return new string[]{"clip-rule"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



