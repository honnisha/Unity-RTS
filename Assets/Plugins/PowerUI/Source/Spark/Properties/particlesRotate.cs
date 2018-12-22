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
	/// Represents the particles-rotate: css property.
	/// </summary>
	
	public class ParticlesRotateProperty:CssProperty{
		
		public ParticlesRotateProperty(){
			
			// Just want 0-1%:
			RelativeTo=ValueRelativity.None;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"particles-rotate"};
		}
		
		public override void Aliases(){
			PointAliases3D();
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
				
				Quaternion rotation;
				
				if(value==null){
					
					rotation=Quaternion.identity;
					
				}else{
					
					rotation=value.GetQuaternion(style.RenderData,this);
					
				}
				
				// Update rotation:
				tag.Rotation=rotation;
				
				// Repos:
				tag.Relocate();
				
			});
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}