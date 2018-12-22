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
	/// Represents the alignment-baseline: css property.
	/// </summary>
	
	public class AlignmentBaseline:CssProperty{
		
		public static AlignmentBaseline GlobalProperty;
		
		
		public AlignmentBaseline(){
			GlobalProperty=this;
			InitialValueText="baseline";
		}
		
		public override string[] GetProperties(){
			return new string[]{"alignment-baseline"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



