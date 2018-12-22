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
using UnityEngine;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the text-decoration-color: css property.
	/// </summary>
	
	public class TextDecorationColor:CssProperty{
		
		public static TextDecorationColor GlobalProperty;
		
		public override string[] GetProperties(){
			return new string[]{"text-decoration-color"};
		}
		
		public TextDecorationColor(){
			IsTextual=true;
			GlobalProperty=this;
			InitialValueText="currentColor";
		}
		
		public override void Aliases(){
			ColourAliases();
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			rgb() | rgba() | hsl() | hsla() | hex-col | named-col
			*/
			
			return new Spec.OneOf(
				new Spec.FunctionCall("rgb"),
				new Spec.FunctionCall("rgba"),
				new Spec.FunctionCall("hsl"),
				new Spec.FunctionCall("hsla"),
				new Spec.Literal("currentColor"),
				new Spec.ValueType(typeof(Css.Units.ColourUnit))
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Let it know a colour changed:
			style.RequestPaint();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



