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
	/// Represents the border-image-source: css property.
	/// </summary>
	
	public class BorderImageSource:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-image-source"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			none | <image>
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("none"),
				new Spec.ValueType(Css.ValueType.Image),
				new Spec.ValueType(typeof(Css.Functions.UrlFunction))
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



