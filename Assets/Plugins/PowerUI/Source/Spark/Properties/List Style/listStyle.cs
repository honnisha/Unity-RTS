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
	/// Represents the list-style: composite property.
	/// </summary>
	
	public class ListStyle:CssCompositeProperty{
		
		public ListStyle(){
			Inherits=true;
			// none
		}
		
		public override string[] GetProperties(){
			return new string[]{"list-style"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[list-style-type || list-style-position || list-style-image] | inherit
			*/
			
			return new Spec.AnyOf(
				
				new Spec.Property(this,"list-style-type"),
				
				new Spec.Property(this,"list-style-position"),
				
				new Spec.Property(this,"list-style-image")
				
			);
		
		}
		
	}
	
}



