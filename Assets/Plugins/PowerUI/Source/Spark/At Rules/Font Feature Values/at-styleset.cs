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
	/// Represents the styleset rule.
	/// </summary>
	
	public class StyleSetRule:CssFontFeatureSubRule{
		
		public override string FeatureName{
			get{
				return "styleset";
			}
		}
		
		public override void ToOpenTypeFeature(Css.Value value,List<OpenTypeFeature> results){
			
			int count=value.Count;
			
			// For each one..
			for(int i=0;i<count;i++){
				
				// Build the feature string:
				results.Add(
					new OpenTypeFeature("ss"+((int)( value[i].GetRawDecimal() )).ToString("00"))
				);
				
			}
			
		}
		
		public override CssAtRule Copy(){
			StyleSetRule at=new StyleSetRule();
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
	}
	
}