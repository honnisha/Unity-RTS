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
	/// Represents the panose-1: css property.
	/// </summary>
	
	public class Panose1:CssProperty{
		
		public Panose1(){
			NamespaceName="svg";
		}
		
		public override string[] GetProperties(){
			return new string[]{"panose-1"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



