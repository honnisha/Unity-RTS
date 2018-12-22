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
	/// Represents the gt() css function. GT rocks :)
	/// </summary>
	
	public class GtFunction:CssFunction{
		
		public int Index;
		
		
		public GtFunction(){
			
			Name="gt";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"gt"};
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
			
			// Create a local gt selector:
			return new GtMatcher(Index);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for gt().
	/// </summary>
	sealed class GtMatcher : LocalMatcher{
		
		public int Index;
		
		
		public GtMatcher(int index){
			
			Index=index;
			
		}
		
		public override bool TryMatch(Dom.Node node){
			
			if(node==null){
				return false;
			}
			
			return (node.childIndex>Index);
			
		}
		
	}
	
}