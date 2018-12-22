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
	/// Represents the format() css function.
	/// </summary>
	
	public class Format:CssFunction{
		
		public Format(){
			
			Name="format";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"format"};
		}
		
		protected override Css.Value Clone(){
			Format result=new Format();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			
			if(Count==0){
				return "";
			}
			
			return this[0].Text;
			
		}
		
	}
	
}



