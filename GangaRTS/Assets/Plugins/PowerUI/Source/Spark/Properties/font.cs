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
	/// Represents the font: composite css property.
	/// </summary>
	
	public class FontCompProperty:CssCompositeProperty{
		
		public static FontCompProperty GlobalProperty;
		
		
		public FontCompProperty(){
			GlobalProperty=this;
			Inherits=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"font"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[
				[
					<'font-style'> || <font-variant-css21> || <'font-weight'> || <'font-stretch'>
				]? <'font-size'> [ / <'line-height'> ]? <'font-family'>
			]
			| caption | icon | menu | message-box | small-caption | status-bar
			*/
			
			return new Spec.OneOf( // |
				
				new Spec.All( // Juxtaposition
					
					new Spec.Optional( // a?
						
						new Spec.AnyOf( // ||
							new Spec.Property(this,"font-style"),
							new Spec.PropertyAlt(
								this,
								"font-variant-caps",
								new Spec.OneOf(
									new Spec.Literal("normal"),
									new Spec.Literal("small-caps")
								)
							),
							new Spec.Property(this,"font-weight"),
							new Spec.Property(this,"font-stretch")
						)
						
					),
					new Spec.Property(this,"font-size"),
					new Spec.Optional( // a?
						new Spec.All( // Juxtaposition
							new Spec.Literal("/"),
							new Spec.Property(this,"line-height")
						)
					),
					new Spec.Property(this,"font-family")
				),
				new Spec.Literal("caption"),
				new Spec.Literal("icon"),
				new Spec.Literal("menu"),
				new Spec.Literal("message-box"),
				new Spec.Literal("small-caption"),
				new Spec.Literal("status-bar")
			);
			
		}
		
	}
	
}



