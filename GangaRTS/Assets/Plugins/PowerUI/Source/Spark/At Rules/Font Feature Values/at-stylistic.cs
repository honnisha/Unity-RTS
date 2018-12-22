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
	/// Represents the stylistic rule.
	/// </summary>
	
	public class StylisticRule:CssFontFeatureSubRule{
		
		public override string FeatureName{
			get{
				return "stylistic";
			}
		}
		
		public override void ToOpenTypeFeature(Css.Value value,List<OpenTypeFeature> results){
			
			// Param is #0:
			int param=((int)( value[0].GetRawDecimal() ));
			
			results.Add(
				new OpenTypeFeature("salt",param)
			);
			
		}
		
		public override CssAtRule Copy(){
			StylisticRule at=new StylisticRule();
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
	}
	
}