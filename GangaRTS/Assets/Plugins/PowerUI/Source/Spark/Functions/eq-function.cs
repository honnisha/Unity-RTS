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
	/// Represents the eq() css function.
	/// </summary>
	
	public class EqFunction:CssFunction{
		
		public int Index;
		
		
		public EqFunction(){
			
			Name="eq";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"eq"};
		}
		
		protected override Css.Value Clone(){
			EqFunction result=new EqFunction();
			result.Values=CopyInnerValues();
			result.Index=Index;
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// Read the index:
			Index=this[0].GetInteger(null,null);
			
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			
			// Create a local eq selector:
			return new EqMatcher(Index);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for eq().
	/// </summary>
	sealed class EqMatcher : LocalMatcher{
		
		public int Index;
		
		
		public EqMatcher(int index){
			
			Index=index;
			
		}
		
		public override bool TryMatch(Dom.Node node){
			
			if(node==null){
				return false;
			}
			
			return (node.childIndex==Index);
			
		}
		
	}
	
}