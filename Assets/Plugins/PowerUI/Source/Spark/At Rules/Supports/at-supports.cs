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
using System.Collections;
using System.Collections.Generic;


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the supports rule. Syntax support only at the moment.
	/// </summary>
	
	public class SupportsRule:CssAtRule,Rule{
		
		/// <summary>The constructed query.</summary>
		public SupportsQuery Query;
		/// <summary>The style to apply if the group is supported.</summary>
		public List<Rule> Rules;
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		public override string[] GetNames(){
			return new string[]{"supports"};
		}
		
		/// <summary>True if this @ rule uses nested selectors. Media and keyframes are two examples.</summary>
		public override void SetupParsing(CssLexer lexer){
			lexer.AtRuleMode=true;
		}
		
		public override CssAtRule Copy(){
			SupportsRule at=new SupportsRule();
			at.Rules=Rules;
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
		/// <summary>The rules.</summary>
		public List<Rule> cssRules{
			get{
				return Rules;
			}
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
				return 12;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			
			// Get the count:
			int count=value.Count;
			
			// Get the block:
			SelectorBlockUnit sBlock=value[count-1] as SelectorBlockUnit;
			
			if(sBlock==null){
				
				// This happens when it contains a *comma* or it's actually broken.
				
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
				
				// Clear it:
				set[set.Count-1]=null;
				
				// Clear the @media part too:
				value[0][0]=null;
				
				// For each part, build a query:
				List<SupportsQuery> results=new List<SupportsQuery>();
				
				for(int i=0;i<value.Count;i++){
					
					Value part=value[i];
					
					if(part==null){
						continue;
					}
					
					SupportsQuery q=SupportsQuery.Load(part,0,part.Count-1);
					
					if(q!=null){
						results.Add(q);
					}
					
				}
				
				if(results.Count==1){
					Query=results[0];
				}else{
					Query=new SupportsQueryList(results.ToArray());
				}
				
			}else{
				
				// Build the media query:
				Query=SupportsQuery.Load(value,1,count-2);
				
			}
			
			return this;
			
		}
		
	}
	
}



