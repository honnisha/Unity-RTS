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
using Css.Units;


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the styleset rule.
	/// </summary>
	
	public class CssFontFeatureSubRule:CssAtRule, Rule{
		
		/// <summary>The properties in the rule.</summary>
		public Dictionary<string,List<OpenTypeFeature>> Properties=new Dictionary<string,List<OpenTypeFeature>>();
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		/// <summary>The feature name e.g. 'styleset'.</summary>
		public virtual string FeatureName{
			get{
				return null;
			}
		}
		
		public override string[] GetNames(){
			return new string[]{FeatureName};
		}
		
		/// <summary>The CSS text of this rule.</summary>
		public string cssText{
			get{
				return RawValue.ToString();
			}
			set{
				throw new NotImplementedException("cssText is read-only on rules. Set it for a whole sheet instead.");
			}
		}
		
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet parentStyleSheet{
			get{
				return ParentSheet;
			}
		}
		
		/// <summary>Rule type.</summary>
		public int type{
			get{
				return 14;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
		}
		
		public override void SetupParsing(CssLexer lexer){
			
			// Load as a property map so we can grab the literal "identity" properties:
			lexer.PropertyMapMode=true;
			
		}
		
		/// <summary>Maps the given value to an OpenType feature.</summary>
		public virtual void ToOpenTypeFeature(Css.Value value,List<OpenTypeFeature> results){
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet sheet,Css.Value value){
			
			// Grab the sheet:
			ParentSheet=sheet;
			RawValue=value;
			
			FontFeatureValuesRule parentRule=parent as FontFeatureValuesRule;
			
			// Add to lookup:
			parentRule.FeatureLookup[FeatureName]=this;
			
			// Load the OpenType rules now:
			int count=value.Count;
			
			// Get the block:
			PropertyMapUnit sBlock=value[count-1] as PropertyMapUnit;
			
			if(sBlock==null){
				
				// Try as a set instead:
				ValueSet set=value[count-1] as ValueSet;
				
				if(set==null){
					// Invalid/ unrecognised block. Ignore it.
					return null;
				}
				
				// Get last one in the set:
				sBlock=set[set.Count-1] as PropertyMapUnit;
				
				// still null?
				if(sBlock==null){
					// Invalid/ unrecognised block. Ignore it.
					return null;
				}
				
			}
			
			// - For each CSS property (each of which is a user defined identifier)..
			foreach(KeyValuePair<string,Css.Value> kvp in sBlock.Properties){
				
				// Create our feature list:
				List<OpenTypeFeature> features=new List<OpenTypeFeature>();
				
				// Map to open type feature now:
				ToOpenTypeFeature(kvp.Value,features);
				
				// Put it into the properties set:
				Properties[kvp.Key]=features;
				
			}
			
			return this;
			
		}
		
	}
	
}



