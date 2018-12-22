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


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the historical-forms keyword.
	/// </summary>
	
	public class HistoricalForms:CssKeyword{
		
		private static OpenTypeFeature Hist;
		
		protected override Value Clone(){
			return new HistoricalForms();
		}
		
		public override void GetOpenTypeFeatures(TextRenderingProperty trp,List<OpenTypeFeature> features){
			
			// The 'hist' feature:
			if(Hist==null){
				Hist=new OpenTypeFeature("hist");
			}
			
			features.Add(Hist);
			
		}
		
		public override string Name{
			get{
				return "historical-forms";
			}
		}
		
	}
	
}



