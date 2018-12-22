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
	/// Represents the background-color: css property.
	/// </summary>
	
	public class BackgroundColor:CssProperty{
		
		public BackgroundColor(){
			InitialValueText="transparent";
		}
		
		public override string[] GetProperties(){
			return new string[]{"background-color"};
		}
		
		public override void Aliases(){
			// A set of colour aliases, e.g. background-color-a:
			ColourAliases();
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				style.RenderData.BGColour=null;
			}else{
				
				UnityEngine.Color col=value.GetColour(style.RenderData,this);
				
				if(col.a==0f){
					style.RenderData.BGColour=null;
				}else{
				
					BackgroundColour colour=style.RenderData.BGColour;
					
					if(colour==null){
						// Create one:
						style.RenderData.BGColour=colour=new BackgroundColour(style.RenderData);
					}
					
					// Change the base colour:
					colour.BaseColour=col;
					
				}
				
			}
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



