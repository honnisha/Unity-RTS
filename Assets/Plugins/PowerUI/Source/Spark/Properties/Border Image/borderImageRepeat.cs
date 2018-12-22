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
	/// Represents the border-image-repeat: css property.
	/// </summary>
	
	public class BorderImageRepeat:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-image-repeat"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[ stretch | repeat | round | space ]{1,2}
			*/
			
			return new Spec.Repeated(
				new Spec.OneOf(
					new Spec.Literal("stretch"),
					new Spec.Literal("repeat"),
					new Spec.Literal("round"),
					new Spec.Literal("space")
				)
			,1,2);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



