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
	/// Represents the annotation rule.
	/// </summary>
	
	public class AnnotationRule:CssFontFeatureSubRule{
		
		public override string FeatureName{
			get{
				return "annotation";
			}
		}
		
		public override void ToOpenTypeFeature(Css.Value value,List<OpenTypeFeature> results){
			
			// Param is #0:
			int param=((int)( value[0].GetRawDecimal() ));
			
			results.Add(
				new OpenTypeFeature("nalt",param)
			);
			
		}
		
		public override CssAtRule Copy(){
			AnnotationRule at=new AnnotationRule();
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
	}
	
}