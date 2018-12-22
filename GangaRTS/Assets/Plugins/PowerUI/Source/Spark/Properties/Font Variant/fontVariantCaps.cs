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
	/// Represents the font-variant-caps: css property.
	/// </summary>
	
	public class FontVariantCaps:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontVariantCaps GlobalProperty;
		
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
				case "normal":
					// Do nothing
				break;
				case "small-caps":
					
					features.Add(Features["smcp"]);
					
				break;
				case "all-small-caps":
					
					features.Add(Features["smcp"]);
					features.Add(Features["c2sc"]);
					
				break;
				case "petite-caps":
					
					features.Add(Features["pcap"]);
					
				break;
				case "all-petite-caps":
					
					features.Add(Features["pcap"]);
					features.Add(Features["c2pc"]);
					
				break;
				case "unicase":
					
					features.Add(Features["unic"]);
					
				break;
				case "titling-caps":
					
					features.Add(Features["titl"]);
					
				break;
			}
			
		}
		
		public FontVariantCaps(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-variant-caps"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | small-caps | all-small-caps | petite-caps | all-petite-caps | unicase | titling-caps
			*/
			
			return new Spec.OneOf(
				new Spec.Literal("normal"),
				new Spec.Literal("small-caps"),
				new Spec.Literal("all-small-caps"),
				new Spec.Literal("petite-caps"),
				new Spec.Literal("all-petite-caps"),
				new Spec.Literal("unicase"),
				new Spec.Literal("titling-caps")
			);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



