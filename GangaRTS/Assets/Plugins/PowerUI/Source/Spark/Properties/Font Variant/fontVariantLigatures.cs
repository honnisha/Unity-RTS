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
	/// Represents the font-variant-ligatures: css property.
	/// </summary>
	
	public class FontVariantLigatures:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontVariantLigatures GlobalProperty;
		
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
				case "common-ligatures":
					
					features.Add(Features["clig"]);
					features.Add(Features["liga"]);
					
				break;
				case "discretionary-ligatures":
					
					features.Add(Features["dlig"]);
					
				break;
				case "historical-ligatures":
					
					features.Add(Features["hlig"]);
					
				break;
				case "contextual":
					
					features.Add(Features["calt"]);
					
				break;
			}
			
		}
		
		public FontVariantLigatures(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-variant-ligatures"};
		}
		
		/// <summary>The internal section of the specification for this property.</summary>
		public static Spec.Value InternalSpec(){
			
			return new Spec.AnyOf(
				new Spec.OneOf(
					new Spec.Literal("common-ligatures"),
					new Spec.Literal("no-common-ligatures")
				),
				new Spec.OneOf(
					new Spec.Literal("discretionary-ligatures"),
					new Spec.Literal("no-discretionary-ligatures")
				),
				new Spec.OneOf(
					new Spec.Literal("historical-ligatures"),
					new Spec.Literal("no-historical-ligatures")
				),
				new Spec.OneOf(
					new Spec.Literal("contextual"),
					new Spec.Literal("no-contextual")
				)
			);
			
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | none | [ <common-lig-values> || <discretionary-lig-values> || <historical-lig-values> || <contextual-alt-values> ]
			
			where 
			<common-lig-values> = [ common-ligatures | no-common-ligatures ]
			<discretionary-lig-values> = [ discretionary-ligatures | no-discretionary-ligatures ]
			<historical-lig-values> = [ historical-ligatures | no-historical-ligatures ]
			<contextual-alt-values> = [ contextual | no-contextual ]
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.Literal("none"),
				InternalSpec()
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



