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
	/// Represents the text-decoration-style: css property.
	/// </summary>
	
	public class TextDecorationStyle:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"text-decoration-style"};
		}
		
		public TextDecorationStyle(){
			IsTextual=true;
			InitialValueText="solid";
		}
		
		public override void Aliases(){
			ColourAliases();
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			solid | double | dotted | dashed | wavy
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("solid"),
				new Spec.Literal("double"),
				new Spec.Literal("dotted"),
				new Spec.Literal("dashed"),
				new Spec.Literal("wavy")
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



