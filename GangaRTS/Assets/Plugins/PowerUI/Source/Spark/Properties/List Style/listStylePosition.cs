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
using System.Collections;
using System.Collections.Generic;
using Css.AtRules;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the list-style-position: css property.
	/// </summary>
	
	public class ListStylePosition:CssProperty{
		
		public ListStylePosition(){
			Inherits=true;
			InitialValueText="outside";
		}
		
		public override string[] GetProperties(){
			return new string[]{"list-style-position"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			inside | outside | inherit
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("inside"),
				new Spec.Literal("outside")
			);
			
		}
		
	}
	
}



