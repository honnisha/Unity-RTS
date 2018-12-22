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
using System.Collections;
using System.Collections.Generic;
using Css;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the shader-family: shaderset("familyName") css property.
	/// This can be used to assign custom shaders.
	/// Note that it's a "family" because there is a group of shaders which can potentially be used depending on other CSS settings.
	/// Each shader must be named e.g:
	///
	/// FamilyName Normal
	/// - The main shader most commonly used. Required.
	///
	/// FamilyName Isolated
	/// - The fallback shader when no others are suitable. Required.
	///
	/// FamilyName Extruded
	/// - Used with e.g. text-extrude:0.5
	/// Note that you should also put shaders in Resources so Unity doesn't accidentally remove them from your project.
	/// </summary>
	
	public class ShaderFamily:CssProperty{
		
		public static ShaderFamily GlobalProperty;
		
		public ShaderFamily(){
			GlobalProperty=this;
			Inherits=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"shader-family"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request a layout:
			style.RequestLayout();
			
			// Convert to shaderset if it's text:
			Css.Units.TextUnit text=(value as Css.Units.TextUnit);
			
			if(text!=null){
				
				// is style[GlobalProperty]==value?
				// Otherwise we've just got inherit.
				if(style[GlobalProperty]==value){
					
					// Auto-wrap in shaderset("name")
					style[GlobalProperty]=new ShaderSetFunction(text.Text);
					
					// Reload value from computed:
					return ApplyState.ReloadValue;
					
				}
				
			}
			
			// Ok!
			return ApplyState.Ok;
		}
		
	}
	
}

namespace Css{
	
	public partial class ComputedStyle{
		
		/// <summary>A custom shader set on this element.</summary>
		public ShaderSet CustomShaders{
			get{
				
				// Get the family:
				ShaderSetFunction value=this[Css.Properties.ShaderFamily.GlobalProperty] as ShaderSetFunction;
				
				if(value==null){
					// None
					
					return null;
				}
				
				// Got it!
				return value.Shaders;
				
			}
		}
		
	}
	
}