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
	/// Represents the sepia(x%) css function.
	/// </summary>
	
	public class SepiaFunction:FilterFunction{
		
		public SepiaFunction(){
			
			Name="sepia";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"sepia"};
		}
		
		protected override Css.Value Clone(){
			SepiaFunction result=new SepiaFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
		public override Loonim.TextureNode ToLoonimNode(RenderableData context){
			
			// Create node:
			Loonim.Sepia node=new Loonim.Sepia();
			
			// Apply intensity:
			node.IntensityModule=new Loonim.Property((Count==0) ? 1f : this[0].GetRawDecimal());
			
			return node;
			
		}
		
	}
	
}



