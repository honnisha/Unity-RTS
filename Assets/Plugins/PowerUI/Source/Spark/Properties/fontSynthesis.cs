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
using InfiniText;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the font-synthesis: css property.
	/// </summary>
	
	public class FontSynthesis:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static FontSynthesis GlobalProperty;
		
		
		public FontSynthesis(){
			IsTextual=true;
			GlobalProperty=this;
			RelativeTo=ValueRelativity.FontSize;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-synthesis"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null || value==null){
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			int synthMode=0;
			
			for(int i=0;i<value.Count;i++){
			
				switch(value[i].Text){
					case "none":
						synthMode=FontSynthesisFlags.None;
					break;
					case "weight":
						synthMode|=FontSynthesisFlags.Weight;
					break;
					case "style":
						synthMode|=FontSynthesisFlags.Style;
					break;
					case "stretch":
						synthMode|=FontSynthesisFlags.Stretch;
					break;
				}
				
			}
			
			if(synthMode==FontSynthesisFlags.All){
				
				// Delete:
				style.Properties.Remove(GlobalProperty);
				
			}else{
				
				// Write the value to the style:
				style[GlobalProperty]=new Css.Units.CachedIntegerUnit(value,synthMode);
				
			}
			
			// Ok!
			return ApplyState.ReloadValue;
			
		}
		
	}
	
}



