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
using Css.Units;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InfiniText;
using Css.AtRules;
using Dom;


namespace Css.Functions{
	
	/// <summary>
	/// Represents all font variant css functions, such as styleset().
	/// </summary>
	
	public class FontVariant:CssFunction{
		
		public FontVariant(){
			
		}
		
		public override void GetOpenTypeFeatures(TextRenderingProperty trp,List<OpenTypeFeature> features){
			
			// For each parameter to this function..
			for(int i=0;i<Count;i++){
				
				// Lookup the param via the @ declaration with the same name as this function:
				LookupParameter(features,trp,Name,this[i].Text);
				
			}
			
		}
		
		/// <summary>Looks up the user-defined parameter in the documents available @ rules.</summary>
		public void LookupParameter(List<OpenTypeFeature> features,TextRenderingProperty trp,string atProperty,string keyword){
			
			RenderableData rd=trp.RenderData;
			
			// Get the host at rule(s):
			string fontName=trp.FontToDraw.FamilyName;
			
			FontFeatureValuesRule hostRule=rd.Document.FontFeatures[fontName];
			
			if(hostRule==null){
				return;
			}
			
			// Get the feature (e.g. @styleset):
			CssFontFeatureSubRule feature=hostRule.FeatureLookup[atProperty];
			
			if(feature==null){
				return;
			}
			
			List<OpenTypeFeature> feats=feature.Properties[keyword];
			
			// Add each into features:
			for(int f=0;f<feats.Count;f++){
				
				// Add it:
				features.Add(feats[f]);
				
			}
			
		}
		
	}
	
}



