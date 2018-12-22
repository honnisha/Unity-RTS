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
	/// Represents the border-image-outset: css property.
	/// </summary>
	
	public class BorderImageOutset:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-image-outset"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[<length> | <number>]{1,4}
			*/
			
			return new Spec.Repeated(
				new Spec.ValueType(typeof(Css.Units.DecimalUnit))
			,1,4);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



