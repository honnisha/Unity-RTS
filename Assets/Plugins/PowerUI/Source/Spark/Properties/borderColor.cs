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
	/// Represents the border-color: css property.
	/// </summary>
	
	public class BorderColor:CssProperty{
		
		/// <summary>A fast reference to the instance of this property.</summary>
		public static BorderColor GlobalProperty;
		
		public BorderColor(){
			
			GlobalProperty=this;
			// Auto means 'use the color property'
			InitialValueText="white";
			 
		}
		
		public override string[] GetProperties(){
			return new string[]{"border-color"};
		}
		
		public override void Aliases(){
			
			// E.g. border-top-color:
			Alias("border-top-color",ValueAxis.Y,0);
			Alias("border-right-color",ValueAxis.X,1);
			Alias("border-bottom-color",ValueAxis.Y,2);
			Alias("border-left-color",ValueAxis.X,3);
			
			// border-top-color-r etc:
			ColourAliases("border-top-color",ValueAxis.Y,0);
			ColourAliases("border-right-color",ValueAxis.X,1);
			ColourAliases("border-bottom-color",ValueAxis.Y,2);
			ColourAliases("border-left-color",ValueAxis.X,3);
			
			// E.g. border-color-a (used by the animation system - must be last):
			ColourAliases();
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the border:
			BorderProperty border=GetBorder(style);
			
			if(value.IsAuto){
				// Use color:
				value=style[Properties.ColorProperty.GlobalProperty];
			}
			
			// Apply the base colour:
			if(value.IsColour){
				value=new ValueSet(new Value[]{value});
			}
			
			border.BaseColour=value;
			
			// Request paint:
			border.RequestPaint();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



