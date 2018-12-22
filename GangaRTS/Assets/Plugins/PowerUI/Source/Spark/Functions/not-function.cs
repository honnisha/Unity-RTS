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
	/// Represents the not() css function.
	/// </summary>
	
	public class NotFunction:CssFunction{
		
		/// <summary>The matchers which will get inverted.</summary>
		public SelectorMatcher[] Matchers;
		
		
		public NotFunction(){
			
			Name="not";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"not"};
		}
		
		protected override Css.Value Clone(){
			NotFunction result=new NotFunction();
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
					// Entire not should be ignored.
					return;
				}
				
				multi.Add(sm);
				
			}
			
			Matchers=multi.ToArray();
			
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			
			if(Matchers==null){
				// Entire not should be ignored.
				return null;
			}
			
			// Create a local not selector:
			return new NotMatcher(Matchers);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for not().
	/// </summary>
	sealed class NotMatcher : LocalMatcher{
		
		public SelectorMatcher[] Matchers;
		
		public NotMatcher(SelectorMatcher[] matchers){
			
			Matchers=matchers;
			
		}
		
		public override bool TryMatch(Dom.Node e){
			
			for(int i=0;i<Matchers.Length;i++){
				
				// If any are true, we return false:
				if(Matchers[i].TryMatch(e)){
					return false;
				}
				
			}
			
			// Otherwise, they were all false - we're true:
			return true;
			
		}
		
	}
	
}