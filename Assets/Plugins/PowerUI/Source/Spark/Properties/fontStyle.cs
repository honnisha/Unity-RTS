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
using InfiniText;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the font-style: css property.
	/// </summary>
	
	public class FontStyle:CssProperty{
		
		public static FontStyle GlobalProperty;
		
		public FontStyle(){
			IsTextual=true;
			Inherits=true;
			InitialValueText="normal";
			GlobalProperty=this;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-style"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | italic | oblique
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.Literal("italic"),
				new Spec.Literal("oblique")
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Apply the changes:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



