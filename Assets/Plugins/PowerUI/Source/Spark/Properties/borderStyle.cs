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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the border-style: css property.
	/// </summary>
	
	public class BorderStyle:CssProperty{
		
		public static BorderStyle GlobalProperty;
		public static CssProperty Top;
		public static CssProperty Right;
		public static CssProperty Bottom;
		public static CssProperty Left;
		
		
		public BorderStyle(){
			GlobalProperty=this;
			// None is the default (and it'll act like a rect for us anyway).
		}
		
		public override string[] GetProperties(){
			return new string[]{"border-style"};
		}
		
		public override void Aliases(){
			Alias("border-top-style",ValueAxis.Y,0);
			Alias("border-right-style",ValueAxis.X,1);
			Alias("border-bottom-style",ValueAxis.Y,2);
			Alias("border-left-style",ValueAxis.X,3);
			
			// Quick references:
			Top=GetAliased(0);
			Right=GetAliased(1);
			Bottom=GetAliased(2);
			Left=GetAliased(3);
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			// Request a layout:
			border.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



