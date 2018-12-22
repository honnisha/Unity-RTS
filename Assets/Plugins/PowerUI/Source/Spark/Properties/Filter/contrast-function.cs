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
	/// Represents the contrast() css function.
	/// </summary>
	
	public class ContrastFunction:FilterFunction{
		
		public ContrastFunction(){
			
			Name="contrast";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"contrast"};
		}
		
		protected override Css.Value Clone(){
			ContrastFunction result=new ContrastFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
		public override Loonim.TextureNode ToLoonimNode(RenderableData context){
			
			// Create node:
			Loonim.Contrast node=new Loonim.Contrast();
			
			node.ContrastModule=new Loonim.Property(this[0].GetRawDecimal());
			
			return node;
			
		}
		
	}
	
}



