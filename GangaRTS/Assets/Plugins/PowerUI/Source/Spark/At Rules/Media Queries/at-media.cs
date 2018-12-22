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
	/// Represents the media rule.
	/// </summary>
	
	public class Media:CssAtRule{
		
		/// <summary>The underlying media query.</summary>
		public MediaQuery Query;
		
		
		public override string[] GetNames(){
			return new string[]{"media"};
		}
		
		/// <summary>True if this @ rule uses nested selectors. Media and keyframes are two examples.</summary>
		public override void SetupParsing(CssLexer lexer){
			lexer.AtRuleMode=true;
		}
		
		public override CssAtRule Copy(){
			Media at=new Media();
			at.Query=Query;
			return at;
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			
			// Get the count:
			int count=value.Count;
			
			// Get the block:
			SelectorBlockUnit sBlock=value[count-1] as SelectorBlockUnit;
			
			if(sBlock==null){
				
				// This happens in the following situations:
				// @media screen,projection{} (when it contains a *comma*)
				// @media screen; (broken css without an actual block)
				
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
				List<MediaQuery> results=new List<MediaQuery>();
				
				for(int i=0;i<value.Count;i++){
					
					Value part=value[i];
					
					if(part==null){
						continue;
					}
					
					MediaQuery q=MediaQuery.Load(part,0,part.Count-1);
					
					if(q!=null){
						results.Add(q);
					}
					
				}
				
				if(results.Count==1){
					Query=results[0];
				}else{
					Query=new MediaQueryList(results.ToArray());
				}
				
			}else{
				
				// Build the media query:
				Query=MediaQuery.Load(value,1,count-2);
				
			}
			
			// Create the rule now:
			return new MediaRule(style,value,Query,sBlock);
			
		}
		
	}
	
}