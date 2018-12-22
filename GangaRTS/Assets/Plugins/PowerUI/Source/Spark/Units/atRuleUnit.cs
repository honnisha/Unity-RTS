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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;


namespace Css.Units{
	
	/// <summary>
	/// Represents an instance of a floating point value.
	/// </summary>
	
	public class AtRuleUnit:CssUnit{
		
		/// <summary>The at rule itself.</summary>
		public CssAtRule AtRule;
		
		
		public override Value ReadStartValue(CssLexer lexer){
			
			// Read off the at:
			lexer.Read();
			
			// Read the name now:
			string name=lexer.ReadString();
			
			// Skip any junk:
			lexer.SkipJunk();
			
			// Map to an at rule:
			CssAtRule rule=CssAtRules.GetLocal(name);
			
			if(rule==null){
				
				// Unrecognised - enter error recovery mode:
				lexer.ErrorRecovery();
				
				// Done:
				return null;
				
			}
			
			// Create the representitive unit:
			AtRuleUnit at=new AtRuleUnit();
			at.AtRule=rule;
			
			// Now in @ mode:
			rule.SetupParsing(lexer);
			
			return at;
		}
		
		/// <summary>The text that occurs before one of these in the stream.</summary> 
		public override string[] PreText{
			get{
				return new string[]{"@"};
			}
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			return ToString();
		}
		
		public override string ToString(){
			return "@"+AtRule.Name;
		}
		
		protected override Value Clone(){
			AtRuleUnit result=new AtRuleUnit();
			result.AtRule=AtRule;
			return result;
		}
		
	}
	
}



