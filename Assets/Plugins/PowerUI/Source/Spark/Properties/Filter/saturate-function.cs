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
	/// Represents the saturate() css function.
	/// </summary>
	
	public class SaturateFunction:FilterFunction{
		
		public SaturateFunction(){
			
			Name="saturate";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"saturate"};
		}
		
		protected override Css.Value Clone(){
			SaturateFunction result=new SaturateFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
		public override Loonim.TextureNode ToLoonimNode(RenderableData context){
			
			// Create node:
			Loonim.Saturation node=new Loonim.Saturation();
			
			node.SaturationModule=new Loonim.Property((Count==0) ? 1f : this[0].GetRawDecimal());
			
			return node;
			
		}
		
	}
	
}



