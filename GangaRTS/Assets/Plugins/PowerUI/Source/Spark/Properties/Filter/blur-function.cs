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
	/// Represents the blur() css function.
	/// </summary>
	
	public class BlurFunction:FilterFunction{
		
		public BlurFunction(){
			
			Name="blur";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"blur"};
		}
		
		protected override Css.Value Clone(){
			BlurFunction result=new BlurFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
		public override Loonim.TextureNode ToLoonimNode(RenderableData context){
			
			// Create node:
			Loonim.Blur node=new Loonim.Blur();
			node.BlurMethod = Loonim.BlurMethod.Gaussian;
			
			// Apply radius:
			Loonim.Property radius=new Loonim.Property(this[0].GetRawDecimal());
			node.RadiusX = radius;
			node.RadiusY = radius;
			
			return node;
			
		}
		
	}
	
}



