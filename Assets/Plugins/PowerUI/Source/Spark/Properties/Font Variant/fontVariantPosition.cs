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
using System.Collections;
using System.Collections.Generic;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the font-variant-position: css property.
	/// </summary>
	
	public class FontVariantPosition:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontVariantPosition GlobalProperty;
		
		/// <summary>Cached feature refs.</summary>
		private static OpenTypeFeatureSet Features;
		
		
		public static void LoadInto(TextRenderingProperty trp,List<OpenTypeFeature> features,ComputedStyle cs){
			
			Css.Value value=cs[GlobalProperty];
			
			if(value==null){
				return;
			}
			
			if(Features==null){
				Features=new OpenTypeFeatureSet();
			}
			
			switch(value.Text){
				
				default:
					// Do nothing
				break;
				case "sub":
				
					features.Add(Features["subs"]);
					features.Add(Features["dnom"]);
					
				break;
				
				case "super":
				
					features.Add(Features["sups"]);
					features.Add(Features["numr"]);
					
				break;
				
			}
			
		}
		
		public FontVariantPosition(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-variant-position"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | sub | super
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.Literal("sub"),
				new Spec.Literal("super")
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



