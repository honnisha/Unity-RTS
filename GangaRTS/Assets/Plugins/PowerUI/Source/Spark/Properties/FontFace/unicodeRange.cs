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
	/// Represents the unicode-range: css property.
	/// </summary>
	
	public class UnicodeRange:CssProperty{
		
		public UnicodeRange(){
			NamespaceName="svg";
		}
		
		public override string[] GetProperties(){
			return new string[]{"unicode-range"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



