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
	/// Represents the text-decoration-line: css property.
	/// </summary>
	
	public class TextDecorationLine:CssProperty{
		
		public static TextDecorationLine GlobalProperty;
		
		public TextDecorationLine(){
			IsTextual=true;
			GlobalProperty=this;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"text-decoration-line"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			none | [underline || overline || line-through || blink]
			*/
			
			return new Spec.AnyOf(
				new Spec.Literal("none"),
				new Spec.OneOf(
					new Spec.Literal("underline"),
					new Spec.Literal("overline"),
					new Spec.Literal("line-through"),
					new Spec.Literal("blink")
				)
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Apply the changes - doesn't change anything about the text:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}