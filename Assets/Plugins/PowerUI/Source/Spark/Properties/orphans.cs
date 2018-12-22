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
	/// Represents the orphans: css property.
	/// </summary>
	
	public class Orphans:CssProperty{
		
		public static Orphans GlobalProperty;
		
		
		public Orphans(){
			Inherits=true;
			InitialValueText="2";
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"orphans"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



