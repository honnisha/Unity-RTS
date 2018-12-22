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
using Dom;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the font-size: css property.
	/// </summary>
	
	public class FontSize:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontSize GlobalProperty;
		
		
		public FontSize(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
			Inherits=true;
			InitialValueText="medium";
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-size"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			xx-small | x-small | small | medium | large | x-large | xx-large | larger | smaller | <length-percentage>
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("xx-small"),
				new Spec.Literal("x-small"),
				new Spec.Literal("small"),
				new Spec.Literal("medium"),
				new Spec.Literal("large"),
				new Spec.Literal("x-large"),
				new Spec.Literal("xx-large"),
				new Spec.Literal("larger"),
				new Spec.Literal("smaller"),
				new Spec.ValueType(Css.ValueType.Number)
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request a redraw:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



