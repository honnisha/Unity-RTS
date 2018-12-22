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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the nth-child() css function.
	/// </summary>
	
	public class NthChildFunction:CssFunction{
		
		public int Nth;
		public int ChildOffset;
		
		
		public NthChildFunction(){
			
			Name="nth-child";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"nth-child"};
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// Figure out nth and offset.
			Value nth=this[0];
			
			if(nth.Text=="odd"){
				Nth=2;
			}else if(nth.Text=="even"){
				Nth=2;
				ChildOffset=1;
			}else if(nth is DecimalUnit){
				
				ChildOffset=nth.GetInteger(null,null);
				
			}else{
			
				Nth=nth.GetInteger(null,null);
			
			}
			
			int count=Count;
			
			if(count==1){
				return;
			}
			
			if(count==2){
				ChildOffset=this[1].GetInteger(null,null);
			}else if(count==3){
				
				string op=this[1].Text;
				
				ChildOffset=this[2].GetInteger(null,null);
				
				if(op=="-"){
					ChildOffset=-ChildOffset;
				}
			}
			
		}
		
		protected override Css.Value Clone(){
			NthChildFunction result=new NthChildFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			
			// Create a local nth-child selector:
			return new NthChildMatcher(Nth,ChildOffset);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for nth-child.
	/// </summary>
	sealed class NthChildMatcher : LocalMatcher{
		
		public int Nth;
		public int ChildOffset;
		
		
		public NthChildMatcher(int nth,int offset){
			
			if(nth<=0){
				nth=2;
			}
			
			Nth=nth;
			ChildOffset=offset;
			
		}
		
		public override bool TryMatch(Dom.Node node){
			
			if(node==null){
				return false;
			}
			
			int index=node.childElementIndex;
			
			index-=ChildOffset;
			
			if(index<0){
				return false;
			}
			
			return ((index % Nth)==0);
			
		}
		
	}
	
}