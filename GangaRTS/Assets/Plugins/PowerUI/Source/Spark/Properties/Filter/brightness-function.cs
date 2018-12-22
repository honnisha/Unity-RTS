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
	/// Represents the brightness() css function.
	/// </summary>
	
	public class BrightnessFunction:FilterFunction{
		
		public BrightnessFunction(){
			
			Name="brightness";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"brightness"};
		}
		
		protected override Css.Value Clone(){
			BrightnessFunction result=new BrightnessFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
	}
	
}



