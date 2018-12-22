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
	/// Represents the font-variant-numeric: css property.
	/// </summary>
	
	public class FontVariantNumeric:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontVariantNumeric GlobalProperty;
		
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
				case "lining-nums":
				
					features.Add(Features["lnum"]);
					
				break;
				
				case "oldstyle-nums":
				
					features.Add(Features["onum"]);
					
				break;
				
				case "proportional-nums":
				
					features.Add(Features["pnum"]);
					
				break;
				
				case "tabular-nums":
				
					features.Add(Features["tnum"]);
					
				break;
				
				case "diagonal-fractions":
				
					features.Add(Features["frac"]);
					
				break;
				
				case "stacked-fractions":
				
					features.Add(Features["afrc"]);
					
				break;
				
				case "ordinal":
				
					features.Add(Features["ordn"]);
					
				break;
				
				case "slashed-zero":
				
					features.Add(Features["zero"]);
					
				break;
				
			}
			
		}
		
		public FontVariantNumeric(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-variant-numeric"};
		}
		
		/// <summary>The internal section of the specification for this property.</summary>
		public static Spec.Value InternalSpec(){
			
			return new Spec.AnyOf(
				new Spec.OneOf(
					new Spec.Literal("lining-nums"),
					new Spec.Literal("oldstyle-nums")
				),
				new Spec.OneOf(
					new Spec.Literal("proportional-nums"),
					new Spec.Literal("tabular-nums")
				),
				new Spec.OneOf(
					new Spec.Literal("diagonal-fractions"),
					new Spec.Literal("stacked-fractions")
				),
				new Spec.Literal("ordinal"),
				new Spec.Literal("slashed-zero")
			);
			
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | [ <numeric-figure-values> || <numeric-spacing-values> || <numeric-fraction-values> || ordinal || slashed-zero ]
			
			where 
			<numeric-figure-values> = [ lining-nums | oldstyle-nums ]
			<numeric-spacing-values> = [ proportional-nums | tabular-nums ]
			<numeric-fraction-values> = [ diagonal-fractions | stacked-fractions ]
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



