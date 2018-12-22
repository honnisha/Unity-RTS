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
	/// Represents the font-weight: css property.
	/// </summary>
	
	public class FontWeight:CssProperty{
		
		public static FontWeight GlobalProperty;
		
		public FontWeight(){
			IsTextual=true;
			GlobalProperty=this;
			Inherits=true;
			InitialValueText="normal";
		}
		
		/// <summary>The value of the 'normal' keyword.</summary>
		public override float GetNormalValue(RenderableData context){
			return 400f;
		}
		
		public override string[] GetProperties(){
			return new string[]{"font-weight"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | bold | bolder | lighter | 100 | 200 | 300 | 400 | 500 | 600 | 700 | 800 | 900
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.Literal("bold"),
				new Spec.Literal("bolder"),
				new Spec.Literal("lighter"),
				new Spec.LiteralNumber(100),
				new Spec.LiteralNumber(200),
				new Spec.LiteralNumber(300),
				new Spec.LiteralNumber(400),
				new Spec.LiteralNumber(500),
				new Spec.LiteralNumber(600),
				new Spec.LiteralNumber(700),
				new Spec.LiteralNumber(800),
				new Spec.LiteralNumber(900)
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request a layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}