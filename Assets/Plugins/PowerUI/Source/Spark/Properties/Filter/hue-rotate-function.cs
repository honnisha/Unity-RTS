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
	/// Represents the hue-rotate() css function.
	/// </summary>
	
	public class HueRotateFunction:FilterFunction{
		
		public HueRotateFunction(){
			
			Name="hue-rotate";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"hue-rotate"};
		}
		
		protected override Css.Value Clone(){
			HueRotateFunction result=new HueRotateFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
		public override Loonim.TextureNode ToLoonimNode(RenderableData context){
			
			// Create node:
			Loonim.Hue node=new Loonim.Hue();
			
			// Amount to rotate by:
			node.HueModule=new Loonim.Property(this[0].GetRawDecimal() / ((float)Math.PI * 2f));
			
			return node;
			
		}
		
	}
	
}



