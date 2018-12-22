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
	/// Represents the keyframes rule.
	/// </summary>
	
	public class Keyframes:CssAtRule{
		
		public override string[] GetNames(){
			return new string[]{"keyframes"};
		}
		
		public override CssAtRule Copy(){
			Keyframes at=new Keyframes();
			return at;
		}
		
		/// <summary>True if this @ rule uses nested selectors. Media and keyframes are two examples.</summary>
		public override void SetupParsing(CssLexer lexer){
			lexer.AtRuleMode=true;
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			
			// Read a name:
			Css.Value nameValue=value[1];
			
			// Get the name:
			string name=nameValue.Text;
			
			// Read the value (a selector block):
			Css.Value block=value[2];
			
			// Grab the block:
			SelectorBlockUnit sBlock=block as SelectorBlockUnit;
			
			// The block should now be a set of other blocks.
			// This set is accessible as sBlock.Rules.
			
			if(sBlock==null || name==null){
				// Broken keyframe set.
				return null;
			}
			
			// Get the frame set:
			List<Rule> rules=sBlock.Rules;
			
			if(rules==null || style.document==null){
				// Broken keyframe set.
				return null;
			}
			
			// Create it:
			return new KeyframesRule(style,value,name,rules);
			
		}
		
	}
	
}