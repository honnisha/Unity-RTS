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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the CSS4 matches() and -spark-any() css functions.
	/// </summary>
	
	public class MatchesFunction:CssFunction{
		
		/// <summary>The matchers which will get checked.</summary>
		public SelectorMatcher[] Matchers;
		
		
		public MatchesFunction(){
			
			Name="matches";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"matches","-moz-any","-spark-any"};
		}
		
		protected override Css.Value Clone(){
			MatchesFunction result=new MatchesFunction();
			result.Values=CopyInnerValues();
			result.Matchers=Matchers;
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// Create the set:
			List<SelectorMatcher> multi=new List<SelectorMatcher>();
			
			for(int i=0;i<Count;i++){
				
				// Read and add:
				SelectorMatcher sm=Css.CssLexer.ReadSelectorMatcher(this[i]);
				
				if(sm==null){
					// Entire any should be ignored.
					return;
				}
				
				multi.Add(sm);
				
			}
			
			Matchers=multi.ToArray();
			
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			
			if(Matchers==null){
				// Entire any should be ignored.
				return null;
			}
			
			// Create a local any selector:
			return new AnyMatcher(Matchers);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for matches().
	/// </summary>
	sealed class AnyMatcher : LocalMatcher{
		
		public SelectorMatcher[] Matchers;
		
		public AnyMatcher(SelectorMatcher[] matchers){
			
			Matchers=matchers;
			
		}
		
		public override bool TryMatch(Dom.Node e){
			
			for(int i=0;i<Matchers.Length;i++){
				
				// If any are true, we return true:
				if(Matchers[i].TryMatch(e)){
					return true;
				}
				
			}
			
			// Otherwise, they were all false - we're false:
			return false;
			
		}
		
	}
	
}