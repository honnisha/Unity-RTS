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
	/// Represents the drop-shadow() css function.
	/// </summary>
	
	public class DropShadowFunction:FilterFunction{
		
		public DropShadowFunction(){
			
			Name="drop-shadow";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"drop-shadow"};
		}
		
		protected override Css.Value Clone(){
			DropShadowFunction result=new DropShadowFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
	}
	
}



