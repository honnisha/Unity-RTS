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
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the particles-scale: css property.
	/// Note! Your particle system must be set to simulate in world space for this one to work
	/// (as it's the same as just scaling the transform).
	/// </summary>
	
	public class ParticlesScaleProperty:CssProperty{
		
		public ParticlesScaleProperty(){
			
			// Just want 0-1%:
			RelativeTo=ValueRelativity.None;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"particles-scale"};
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Main thread only:
			Callback.MainThread(delegate(){
				
				// Get the <particles> element:
				HtmlParticlesElement tag=style.Element as HtmlParticlesElement;
				
				if(tag==null){
					
					// Ok!
					return;
					
				}
				
				Vector3 scale;
				
				if(value==null){
					scale=Vector3.one;
				}else{
					float x=1f;
					float y=1f;
					float z=1f;
					
					if(value[0]!=null){
						x=value[0].GetDecimal(style.RenderData,this);
					}
					
					if(value[1]!=null){
						y=value[1].GetDecimal(style.RenderData,this);
					}
					
					if(value[2]!=null){
						z=value[2].GetDecimal(style.RenderData,this);
					}
					
					scale=new Vector3(x,y,z);
				}
				
				// Update scale:
				tag.Scale=scale;
				
				// Repos:
				tag.Relocate();
				
			});
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}