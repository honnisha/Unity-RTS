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
	/// Represents the font-variant-alternates: css property.
	/// </summary>
	
	public class FontVariantAlternates:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontVariantAlternates GlobalProperty;
		
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
			
			if(value is CssFunction){
				value.GetOpenTypeFeatures(trp,features);
				return;
			}
			
			for(int i=0;i<value.Count;i++){
				
				Css.Value current=value[i];
				
				if(current is CssFunction){
					
					// E.g. stylistic(..)
					current.GetOpenTypeFeatures(trp,features);
					
				}else{
				
					switch(current.Text){
						
						default:
						case "normal":
							// Do nothing
						break;
						case "historical-forms":
							
							features.Add(Features["hist"]);
							
						break;
						
					}
					
				}
				
			}
			
		}
		
		
		public FontVariantAlternates(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-variant-alternates"};
		}
		
		/// <summary>The internal section of the specification for this property.</summary>
		public static Spec.Value InternalSpec(){
			
			return new Spec.AnyOf( // ||
				new Spec.FunctionCall("stylistic"),
				new Spec.Literal("historical-forms"),
				new Spec.FunctionCall("styleset"),
				new Spec.FunctionCall("character-variant"),
				new Spec.FunctionCall("swash"),
				new Spec.FunctionCall("ornaments"),
				new Spec.FunctionCall("annotation")
			);
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			normal | 
			[
				stylistic( <feature-value-name> ) || 
				historical-forms || 
				styleset( <feature-value-name># ) || 
				character-variant( <feature-value-name># ) || 
				swash( <feature-value-name> ) || 
				ornaments( <feature-value-name> ) || 
				annotation( <feature-value-name> ) 
			]
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



