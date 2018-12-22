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
using Css.Units;


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the font-feature-values rule.
	/// </summary>
	
	public class FontFeatureValuesRule:CssAtRule, Rule{
		
		/// <summary>The inner rules.</summary>
		public List<Rule> Entries;
		/// <summary>Font name being targeted.</summary>
		public string FontName;
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		/// <summary>A lookup by @name to the rule itself.</summary>
		public Dictionary<string,CssFontFeatureSubRule> FeatureLookup=new Dictionary<string,CssFontFeatureSubRule>();
		
		
		public override string[] GetNames(){
			return new string[]{"font-feature-values"};
		}
		
		/// <summary>True if this @ rule uses nested selectors. Media and keyframes are two examples.</summary>
		public override void SetupParsing(CssLexer lexer){
			lexer.AtRuleMode=true;
		}
		
		public override CssAtRule Copy(){
			FontFeatureValuesRule at=new FontFeatureValuesRule();
			at.FontName=FontName;
			at.Entries=Entries;
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
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
			
			if(document.FontFeatures==null){
				
				document.FontFeatures=new Dictionary<string,FontFeatureValuesRule>();
				
			}
			
			// Add to features lookup:
			document.FontFeatures[FontName]=this;
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
			// Remove from features lookup:
			document.FontFeatures.Remove(FontName);
			
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet sheet,Css.Value value){
			
			// Grab the sheet:
			ParentSheet=sheet;
			RawValue=value;
			
			// Read a value:
			Css.Value val=value[1];
			
			// Get the value as constant text:
			FontName=val.Text;
			
			int count=value.Count;
			
			// Get the block:
			SelectorBlockUnit sBlock=value[count-1] as SelectorBlockUnit;
			
			if(sBlock==null){
				
				// Try as a set instead:
				ValueSet set=value[count-1] as ValueSet;
				
				if(set==null){
					// Invalid/ unrecognised block. Ignore it.
					return null;
				}
				
				// Get last one in the set:
				sBlock=set[set.Count-1] as SelectorBlockUnit;
				
				// still null?
				if(sBlock==null){
					// Invalid/ unrecognised block. Ignore it.
					return null;
				}
				
			}
			
			Entries=sBlock.LoadAsRules();
			
			return this;
			
		}
		
	}
	
}