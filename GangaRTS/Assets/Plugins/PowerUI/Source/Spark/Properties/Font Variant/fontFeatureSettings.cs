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
	/// Represents the font-feature-settings: css property.
	/// </summary>
	
	public class FontFeatureSettings:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontFeatureSettings GlobalProperty;
		
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
			
			for(int i=0;i<value.Count;i++){
				
				Css.Value current=value[i];
				
				// The feature name:
				string name;
				
				if(current is Css.ValueSet){
					
					// E.g. "swsh" 2
					name=current[0].Text;
					
					// Get the value (may be 'on' or 'off'):
					Css.Value par=current[1];
					
					if(par.Type==ValueType.Text){
						if(par.Text=="off"){
							continue;
						}
						
						// name followed by 'on'.
						features.Add(Features[name]);
					
					}else{
						
						int param=(int)current[1].GetRawDecimal();
						
						features.Add(new OpenTypeFeature(name,param));
						
					}
					
				}else{
					
					// It's just a feature name:
					name=current.Text;
					
					features.Add(Features[name]);
					
				}
				
			}
			
			
		}
		
		public FontFeatureSettings(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-feature-settings"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



