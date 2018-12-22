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


namespace Css.Units{
	
	/// <summary>
	/// Used by for example keyframes. This is where selector blocks {..} are used within another.
	/// </summary>
	
	public class SelectorBlockUnit:CssUnit{
		
		/// <summary>Underlying style block if this one.</summary>
		public Style Style;
		/// <summary>Used by e.g. @keyframes and @media. A list of selector rules in here.</summary>
		public List<Rule> Rules;
		
		
		public SelectorBlockUnit(){}
		
		public SelectorBlockUnit(Style style){
			Style=style;
		}
		
		public override string ToString(){
			
			if(Style==null){
				
				string str="{";
				
				foreach(Rule entry in Rules){
					
					str+=entry.ToString()+" ";
					
				}
				
				return str+"}";
				
			}
			
			return "{"+Style.ToString()+"}";
			
		}
		
		/// <summary>Loads the contents of this block as a set of rules.</summary>
		public List<Rule> LoadAsRules(){
			
			if(Rules==null){
				Rules=new List<Rule>();
			}
			
			return Rules;
		}
		
		public override Value ReadStartValue(CssLexer lexer){
			
			// No longer in selector mode:
			lexer.SelectorMode=false;
			
			// Skip the {:
			lexer.Read();
			
			if(lexer.PropertyMapMode){
				
				// Clear:
				lexer.PropertyMapMode=false;
				
				// Create map:
				PropertyMapUnit map=new PropertyMapUnit();
				
				// Create the style:
				Style mapStyle=new Style(lexer.Scope);
				
				// Read the values:
				mapStyle.LoadProperties(lexer,delegate(Style s,string pName,Css.Value value){
					
					return map.OnReadProperty(s,pName,value);
					
				});
				
				map.Style=mapStyle;
				
				return map;
				
			}
			
			// Result:
			SelectorBlockUnit block=new SelectorBlockUnit();
			
			if(lexer.AtRuleMode){
				
				// Clear at rule mode:
				lexer.AtRuleMode=false;
				
				List<Rule> rules=null;
				
				lexer.SkipJunk();
				
				while(lexer.Peek()!='}' && lexer.Peek()!='\0'){
				
					Rule[] set;
					Rule single=lexer.ReadRules(out set);
					
					if(single==null){
						
						if(set!=null){
							
							if(rules==null){
								rules=new List<Rule>();
							}
							
							for(int x=0;x<set.Length;x++){
								
								rules.Add(set[x]);
								
							}
							
						}
						
					}else{
						
						if(rules==null){
							rules=new List<Rule>();
						}
						
						rules.Add(single);
					}
					
					lexer.SkipJunk();
					
				}
				
				block.Rules=rules;
				
			}else{
				
				// Create the style:
				Style style=new Style(lexer.Scope);
				
				// Read the values:
				style.LoadProperties(lexer,null);
				
				block.Style=style;
			
			}
			
			// Note that we do not read off the close bracket.
			// This is so the lexer knows it's done reading the value
			// and can terminate accordingly.
			
			return block;
		}
		
		/// <summary>Called when this block reads a property from the CSS stream.</summary>
		protected virtual int OnReadProperty(Style style,string pName,Css.Value value){
			return 0;
		}
		
		protected override Value Clone(){
			SelectorBlockUnit result=new SelectorBlockUnit();
			result.Style=Style;
			return result;
		}
		
		public override string[] PreText{
			get{
				return new string[]{"{"};
			}
		}
		
	}
	
}



