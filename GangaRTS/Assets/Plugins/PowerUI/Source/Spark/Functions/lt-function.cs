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
	/// Represents the lt() css function.
	/// </summary>
	
	public class LtFunction:CssFunction{
		
		public int Index;
		
		
		public LtFunction(){
			
			Name="lt";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"lt"};
		}
		
		protected override Css.Value Clone(){
			GtFunction result=new GtFunction();
			result.Values=CopyInnerValues();
			result.Index=Index;
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// Read the index:
			Index=this[0].GetInteger(null,null);
			
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			
			// Create a local lt selector:
			return new LtMatcher(Index);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for gt().
	/// </summary>
	sealed class LtMatcher : LocalMatcher{
		
		public int Index;
		
		
		public LtMatcher(int index){
			
			Index=index;
			
		}
		
		public override bool TryMatch(Dom.Node node){
			
			if(node==null){
				return false;
			}
			
			return (node.childIndex<Index);
			
		}
		
	}
	
}