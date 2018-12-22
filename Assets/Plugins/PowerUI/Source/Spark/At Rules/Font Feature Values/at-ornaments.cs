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
	/// Represents the ornaments rule.
	/// </summary>
	
	public class OrnamentsRule:CssFontFeatureSubRule{
		
		public override string FeatureName{
			get{
				return "ornaments";
			}
		}
		
		public override void ToOpenTypeFeature(Css.Value value,List<OpenTypeFeature> results){
			
			// Param is #0:
			int param=(int)( value[0].GetRawDecimal() );
			
			// Add it:
			results.Add(new OpenTypeFeature("ornm",param));
			
		}
		
		public override CssAtRule Copy(){
			OrnamentsRule at=new OrnamentsRule();
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
	}
	
}