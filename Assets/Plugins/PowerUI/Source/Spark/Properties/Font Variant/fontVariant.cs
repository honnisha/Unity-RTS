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
	/// Represents the font-variant: css composite property.
	/// </summary>
	
	public class FontVariantCompProperty:CssCompositeProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontVariantCompProperty GlobalProperty;
		
		/// <summary>Use this to build all font-variant features.</summary>
		public static List<OpenTypeFeature> GetAllFeatures(TextRenderingProperty trp){
			
			// Feature set:
			List<OpenTypeFeature> features=new List<OpenTypeFeature>();
			
			// Get the cs:
			ComputedStyle cs=trp.RenderData.computedStyle;
			
			// Load each variant property into features:
			FontVariantCaps.LoadInto(trp,features,cs);
			FontVariantAlternates.LoadInto(trp,features,cs);
			FontVariantEastAsian.LoadInto(trp,features,cs);
			FontVariantLigatures.LoadInto(trp,features,cs);
			FontVariantNumeric.LoadInto(trp,features,cs);
			FontVariantPosition.LoadInto(trp,features,cs);
			FontFeatureSettings.LoadInto(trp,features,cs);
			
			return features;
			
		}
		
		public FontVariantCompProperty(){
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
			Inherits=true;
			InitialValueText="normal";
		}
		
		public override string[] GetProperties(){
			return new string[]{"font-variant"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | none |
			[
				<common-lig-values> || 
				<discretionary-lig-values> || 
				<historical-lig-values> || 
				<contextual-alt-values> || 
				stylistic( <feature-value-name> ) || 
				historical-forms || 
				styleset( <feature-value-name># ) || 
				character-variant( <feature-value-name># ) || 
				swash( <feature-value-name> ) || 
				ornaments( <feature-value-name> ) || 
				annotation( <feature-value-name> ) || 
				[ small-caps | all-small-caps | petite-caps | all-petite-caps | unicase | titling-caps ] || 
				<numeric-figure-values> || 
				<numeric-spacing-values> || 
				<numeric-fraction-values> || 
				ordinal || 
				slashed-zero || 
				<east-asian-variant-values> || 
				<east-asian-width-values> || 
				ruby 
			] 
			
			where 
			<common-lig-values> = [ common-ligatures | no-common-ligatures ]
			<discretionary-lig-values> = [ discretionary-ligatures | no-discretionary-ligatures ]
			<historical-lig-values> = [ historical-ligatures | no-historical-ligatures ]
			<contextual-alt-values> = [ contextual | no-contextual ]
			<feature-value-name> = IDENT
			<numeric-figure-values> = [ lining-nums | oldstyle-nums ]
			<numeric-spacing-values> = [ proportional-nums | tabular-nums ]
			<numeric-fraction-values> = [ diagonal-fractions | stacked-fractions ]
			<east-asian-variant-values> = [ jis78 | jis83 | jis90 | jis04 | simplified | traditional ]
			<east-asian-width-values> = [ full-width | proportional-width ]
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.Literal("none"),
			
				new Spec.AnyOf(
					
					// - Alternatives -
					
					new Spec.PropertyAlt(this,"font-variant-alternates",
						FontVariantAlternates.InternalSpec()
					),
					
					// - Caps -
					
					new Spec.PropertyAlt(this,"font-variant-caps",
						new Spec.OneOf(
							new Spec.Literal("small-caps"),
							new Spec.Literal("all-small-caps"),
							new Spec.Literal("petite-caps"),
							new Spec.Literal("all-petite-caps"),
							new Spec.Literal("unicase"),
							new Spec.Literal("titling-caps")
						)
					),
					
					// - Numeric -
					
					new Spec.PropertyAlt(this,"font-variant-numeric",
						FontVariantNumeric.InternalSpec()
					),
					
					// - Ligatures -
					
					new Spec.PropertyAlt(this,"font-variant-ligatures",
						FontVariantLigatures.InternalSpec()
					),
					
					// - East Asian -
					
					new Spec.PropertyAlt(this,"font-variant-east-asian",
						FontVariantEastAsian.InternalSpec()
					)
					
				)
			
			);
			
		}
		
	}
	
}



