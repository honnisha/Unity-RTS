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
using UnityEngine;
using Css;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the text-stroke: css property. Used to add outlines to text.
	/// </summary>
	
	public class TextStroke:CssProperty{
		
		public static TextStroke GlobalProperty;
		
		
		public TextStroke(){
			GlobalProperty=this;
			IsTextual=true;
			Inherits=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"text-stroke","text-outline"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Apply the changes - doesn't change anything about the actual text, so we just want a layout:
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>
		/// Applies to text nodes.
		/// </summary>
		public override void ApplyText(TextRenderingProperty text,RenderableData data,ComputedStyle style,Value value){
			
			// Get:
			TextStrokeProperty tsp=data.GetProperty(typeof(TextStrokeProperty)) as TextStrokeProperty;
			
			// Apply the property:
			if(value!=null && !(value.IsType(typeof(Css.Keywords.None)))){
				
				// The stroke properties:
				float thickness=value[0].GetDecimal(style.RenderData,this);
				
				if(thickness!=0){
					
					if(tsp==null){
						
						// Required - Create:
						tsp=new TextStrokeProperty(data);
						tsp.Text=text;
						
						// Add it:
						data.AddOrReplaceProperty(tsp,typeof(TextStrokeProperty));
						
					}
					
					tsp.Thickness=thickness;
					
					tsp.Colour=value[1].GetColour(style.RenderData,this);
					
					return;
					
				}
				
			}
			
			// Remove if it falls down here:
			
			if(tsp!=null){
				
				// Remove:
				data.AddOrReplaceProperty(null,typeof(TextStrokeProperty));
			
			}
			
		}
		
	}
	
}