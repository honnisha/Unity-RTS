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
	/// Represents the line-height: css property.
	/// </summary>
	
	public class LineHeight:CssProperty{
		
		public static LineHeight GlobalProperty;
		
		public LineHeight(){
			GlobalProperty=this;
			IsTextual=true;
			RelativeTo=ValueRelativity.FontSize;
			Inherits=true;
			InitialValueText="normal";
		}
		
		public override string[] GetProperties(){
			return new string[]{"line-height"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | <number> | <length> | <percentage>
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.ValueType(typeof(Css.Units.DecimalUnit))
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request a new layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



