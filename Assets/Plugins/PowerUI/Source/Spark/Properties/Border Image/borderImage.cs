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
using Css.Spec;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the border-image: composite css property.
	/// </summary>
	
	public class BorderImageCompProperty:CssCompositeProperty{
		
		public static BorderImageCompProperty GlobalProperty;
		
		
		public BorderImageCompProperty(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"border-image"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			<'border-image-source'> || 
			<'border-image-slice'> [ / <'border-image-width'> | / <'border-image-width'>? / <'border-image-outset'> ]? || 
			<'border-image-repeat'>
			*/
			
			return new Spec.AnyOf( // ||
				
				new Spec.Property(this,"border-image-source"),
				
				new Spec.All( // Juxtaposition
					new Spec.Property(this,"border-image-slice"),
					
					new Spec.Optional( // [ .. ]?
						
						new Spec.OneOf( // |
							
							new Spec.All(
								new Spec.Literal("/"),
								new Spec.Property(this,"border-image-width")
							),
							
							new Spec.All(
								new Spec.Optional(
									new Spec.All(
										new Spec.Literal("/"),
										new Spec.Property(this,"border-image-width")
									)
								),
								new Spec.Literal("/"),
								new Spec.Property(this,"border-image-outset")
							)
							
						)
						
					)
					
				),
				
				new Spec.Property(this,"border-image-repeat")
			);
			
		}
		
	}
	
}



