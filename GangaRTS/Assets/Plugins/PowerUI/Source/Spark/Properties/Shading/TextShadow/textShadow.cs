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
	/// Represents the text-shadow: css property.
	/// </summary>
	
	public class TextShadow:CssProperty{
		
		public static TextShadow GlobalProperty;
		
		
		public TextShadow(){
			GlobalProperty=this;
			IsTextual=true;
			Inherits=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"text-shadow"};
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
			TextShadowProperty tsp=data.GetProperty(typeof(TextShadowProperty)) as TextShadowProperty;
			
			// Apply the property:
			if(value!=null && !(value.IsType(typeof(Css.Keywords.None))) && value.Type==ValueType.Set){
				
				// The glow properties:
				float blur=0;
				Color colour=Color.black;
				
				if(tsp==null){
					
					// Required - Create:
					tsp=new TextShadowProperty(data);
					tsp.Text=text;
					
					// Add it:
					data.AddOrReplaceProperty(tsp,typeof(TextShadowProperty));
					
				}
				
				tsp.HOffset=value[0].GetDecimal(data,this);
				tsp.VOffset=value[1].GetDecimal(data,this);
				
				// Grab the blur:
				Value innerValue=value[2];
				
				if(innerValue.Type==ValueType.Set){
					colour=innerValue.GetColour(data,this);
				}else{
					blur=innerValue.GetDecimal(data,this);
					
					// Grab the colour:
					innerValue=value[3];
					
					if(innerValue.Type==ValueType.Set){
						colour=innerValue.GetColour(data,this);
					}
					
				}
			
				if(colour.a==1f){
					// Default transparency:
					colour.a=0.8f;
				}
				
				tsp.Colour=colour;
				tsp.Blur=blur;
				
			}else if(tsp!=null){
				
				// Remove:
				data.AddOrReplaceProperty(null,typeof(TextShadowProperty));
			
			}
			
		}
		
	}
	
}