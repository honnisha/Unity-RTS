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
	/// Represents the text-decoration: composite css property.
	/// </summary>
	
	public class TextDecoCompProperty:CssCompositeProperty{
		
		public override string[] GetProperties(){
			return new string[]{"text-decoration"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[text-decoration-line || text-decoration-style || text-decoration-color]
			*/
			
			return new Spec.AnyOf(
				
				new Spec.Property(this,"text-decoration-line"),
				
				new Spec.Property(this,"text-decoration-style"),
				
				new Spec.Property(this,"text-decoration-color")
				
			);
			
		}
		
	}
	
}



