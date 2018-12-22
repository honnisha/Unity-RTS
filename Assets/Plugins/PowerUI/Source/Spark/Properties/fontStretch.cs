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
	/// Represents the font-stretch: css property.
	/// </summary>
	
	public class FontStretch:CssProperty{
		
		public override float GetNormalValue(RenderableData context){
			return FontStretchMode.Normal;
		}
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontStretch GlobalProperty;
		
		
		public FontStretch(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
			Inherits=true;
			InitialValueText="normal";
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-stretch"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | ultra-condensed | extra-condensed | condensed | semi-condensed | semi-expanded | expanded | extra-expanded | ultra-expanded
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.Literal("ultra-condensed"),
				new Spec.Literal("extra-condensed"),
				new Spec.Literal("condensed"),
				new Spec.Literal("semi-condensed"),
				new Spec.Literal("semi-expanded"),
				new Spec.Literal("expanded"),
				new Spec.Literal("extra-expanded"),
				new Spec.Literal("ultra-expanded")
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Redraw:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}

namespace Css{
	
	/// <summary>
	/// The available values for font-stretch.
	/// </summary>
	public static class FontStretchMode{
	
		public const int UltraCondensed=1;
		public const int ExtraCondensed=2;
		public const int Condensed=3;
		public const int SemiCondensed=4;
		public const int Normal=5;
		public const int SemiExpanded=6;
		public const int Expanded=7;
		public const int ExtraExpanded=8;
		public const int UltraExpanded=9;
		
	}
	
}