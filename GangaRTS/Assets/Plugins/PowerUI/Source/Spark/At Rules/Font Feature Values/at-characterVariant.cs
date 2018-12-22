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
using InfiniText;


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the character-variant rule.
	/// </summary>
	
	public class CharacterVariantRule:CssFontFeatureSubRule{
		
		public override string FeatureName{
			get{
				return "character-variant";
			}
		}
		
		public override void ToOpenTypeFeature(Css.Value value,List<OpenTypeFeature> results){
			
			// Param is #1:
			int param=(int)( value[1].GetRawDecimal() );
			
			// Add the feature:
			results.Add(
				new OpenTypeFeature("cv"+((int)( value[0].GetRawDecimal() )).ToString("00"),param)
			);
			
		}
		
		public override CssAtRule Copy(){
			CharacterVariantRule at=new CharacterVariantRule();
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
	}
	
}