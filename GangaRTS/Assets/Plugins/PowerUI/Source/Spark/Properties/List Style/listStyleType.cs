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
using System.Collections;
using System.Collections.Generic;
using Css.AtRules;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the list-style-type: css property.
	/// </summary>
	
	public class ListStyleType:CssProperty{
		
		public static ListStyleType GlobalProperty;
		
		
		public ListStyleType(){
			Inherits=true;
			GlobalProperty=this;
			InitialValueText="disc";
		}
		
		public override string[] GetProperties(){
			return new string[]{"list-style-type"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// special case for list-item as we'll be creating the marker too:
			if(style.DisplayX==DisplayMode.ListItem){
				
				// Update the marker selector:
				MarkerSelector.Update(style);
				
			}else{
				
				// (Remove marker if there is one)
				style.RemoveVirtual(MarkerSelector.Priority);
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			disc | circle | square | decimal | decimal-leading-zero | lower-roman | upper-roman | 
			lower-greek | lower-alpha | lower-latin | upper-alpha | upper-latin | hebrew | armenian |
			georgian | cjk-ideographic | hiragana | katakana | hiragana-iroha | katakana-iroha | none | inherit
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("disc"),
				new Spec.Literal("circle"),
				new Spec.Literal("square"),
				new Spec.Literal("decimal"),
				new Spec.Literal("decimal-leading-zero"),
				new Spec.Literal("lower-roman"),
				new Spec.Literal("upper-roman"),
				new Spec.Literal("lower-greek"),
				new Spec.Literal("lower-alpha"),
				new Spec.Literal("lower-latin"),
				new Spec.Literal("upper-alpha"),
				new Spec.Literal("upper-latin"),
				new Spec.Literal("hebrew"),
				new Spec.Literal("armenian"),
				new Spec.Literal("georgian"),
				new Spec.Literal("cjk-ideographic"),
				new Spec.Literal("hiragana"),
				new Spec.Literal("katakana"),
				new Spec.Literal("hiragana-iroha"),
				new Spec.Literal("katakana-iroha"),
				new Spec.Literal("none")
			);
			
		}
		
	}
	
}



