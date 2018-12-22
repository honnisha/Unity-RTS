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
using Dom;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the resize: css property.
	/// Used by the resizer tab (which appears when there's two scrollbars via the overflow property).
	/// </summary>
	
	public class Resize:CssProperty{
		
		public static Resize GlobalProperty;
		
		
		public Resize(){
			
			GlobalProperty=this;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"resize"};
		}
		
		/// <summary>Gets the resize value from the given style.</summary>
		public static void Compute(ComputedStyle cs,out bool x,out bool y){
			
			// Get the raw value:
			Css.Value resize=cs[GlobalProperty];
			
			x=false;
			y=false;
			
			if(resize==null){
				return;
			}
			
			// horizontal, both, vertical etc:
			string text=resize.Text;
			
			switch(text){
				case "both":
					x=y=true;
				break;
				case "horizontal":
					x=true;
				break;
				case "vertical":
					y=true;
				break;
				case "block":
					
					if(cs.WritingSystemInverted){
						x=true;
					}else{
						y=true;
					}
					
				break;
				case "inline":
					
					if(cs.WritingSystemInverted){
						y=true;
					}else{
						x=true;
					}
					
				break;
			}
			
		}
		
	}
	
}