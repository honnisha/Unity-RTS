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
	/// Represents the units-per-em: css property.
	/// </summary>
	
	public class UnitsPerEm:CssProperty{
		
		public UnitsPerEm(){
			NamespaceName="svg";
		}
		
		public override string[] GetProperties(){
			return new string[]{"units-per-em"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



