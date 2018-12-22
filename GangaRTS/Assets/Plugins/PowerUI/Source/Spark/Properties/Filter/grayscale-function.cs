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
	/// Represents the grayscale() css function.
	/// </summary>
	
	public class GrayscaleFunction:FilterFunction{
		
		public GrayscaleFunction(){
			
			Name="grayscale";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"grayscale"};
		}
		
		protected override Css.Value Clone(){
			GrayscaleFunction result=new GrayscaleFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
		public override Loonim.TextureNode ToLoonimNode(RenderableData context){
			
			// Create node:
			Loonim.Desaturate node=new Loonim.Desaturate();
			node.Method = Loonim.DesaturateMethod.Greyscale;
			
			// node.SaturationModule=new Loonim.Property((Count==0) ? 1f : this[0].GetRawDecimal());
			
			return node;
			
		}
		
	}
	
}



