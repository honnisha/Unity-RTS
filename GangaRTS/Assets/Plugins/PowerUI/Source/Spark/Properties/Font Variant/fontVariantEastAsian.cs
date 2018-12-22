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
	/// Represents the font-variant-east-asian: css property.
	/// </summary>
	
	public class FontVariantEastAsian:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontVariantEastAsian GlobalProperty;
		
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
				
				case "jis78":
				
					features.Add(Features["jp78"]);
					
				break;
				case "jis83":
				
					features.Add(Features["jp83"]);
					
				break;
				case "jis90":
				
					features.Add(Features["jp90"]);
					
				break;
				case "jis04":
				
					features.Add(Features["jp04"]);
					
				break;
				case "simplified":
				
					features.Add(Features["smpl"]);
					
				break;
				case "traditional":
				
					features.Add(Features["trad"]);
					
				break;
				case "full-width":
				
					features.Add(Features["fwid"]);
					
				break;
				case "proportional-width":
				
					features.Add(Features["pwid"]);
					
				break;
				case "ruby":
				
					features.Add(Features["ruby"]);
					
				break;
				
			}
			
		}
		
		public FontVariantEastAsian(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-variant-east-asian"};
		}
		
		/// <summary>The internal section of the specification for this property.</summary>
		public static Spec.Value InternalSpec(){
			
			return new Spec.AnyOf(
				new Spec.OneOf(
					new Spec.Literal("jis78"),
					new Spec.Literal("jis83"),
					new Spec.Literal("jis90"),
					new Spec.Literal("jis04"),
					new Spec.Literal("simplified"),
					new Spec.Literal("traditional")
				),
				new Spec.OneOf(
					new Spec.Literal("full-width"),
					new Spec.Literal("proportional-width")
				),
				new Spec.Literal("ruby")
			);
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | [ <east-asian-variant-values> || <east-asian-width-values> || ruby ] 
			
			where 
			<east-asian-variant-values> = [ jis78 | jis83 | jis90 | jis04 | simplified | traditional ]
			<east-asian-width-values> = [ full-width | proportional-width ]
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				InternalSpec()
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



