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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the dir() css function.
	/// </summary>
	
	public class DirFunction:CssFunction{
		
		/// <summary>The direction to match.</summary>
		public int Direction=DirectionMode.RTL;
		
		
		public DirFunction(){
			
			Name="dir";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"dir"};
		}
		
		protected override Css.Value Clone(){
			DirFunction result=new DirFunction();
			result.Values=CopyInnerValues();
			result.Direction=Direction;
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// Get the direction:
			Direction=this[0].GetInteger(null,null);
			
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			
			// Create a local dir selector:
			return new DirMatcher(Direction);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for dir().
	/// </summary>
	sealed class DirMatcher : LocalMatcher{
		
		public int Direction;
		
		
		public DirMatcher(int dir){
			
			Direction=dir;
			
		}
		
		public override bool TryMatch(Dom.Node node){
			
			if(node==null){
				return false;
			}
			
			return ((node as IRenderableNode).ComputedStyle.DrawDirectionX == Direction);
			
		}
		
	}
	
}