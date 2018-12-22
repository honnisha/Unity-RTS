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
using Css;


namespace Svg{
	
	/// <summary>
	/// Provides functionality for working with various valuetypes found in SVG.
	/// </summary>
	
	public static class ValueHelpers{
		
		/// <summary>Converts the given textual name into text path spacing.</summary>
		public static TextPathSpacing GetPathSpacing(string text){
			
			text=text.Trim().ToLower();
			
			return (TextPathSpacing)Enum.Parse(typeof(TextPathSpacing),text);
			
		}
		
		/// <summary>Converts the given textual name into a text path method.</summary>
		public static TextPathMethod GetPathMethod(string text){
			
			text=text.Trim().ToLower();
			
			return (TextPathMethod)Enum.Parse(typeof(TextPathMethod),text);
			
		}
		
		/// <summary>Converts the given textual name into a text anchor.</summary>
		public static TextAnchor GetAnchor(string text){
			
			text=text.Trim().ToLower();
			
			return (TextAnchor)Enum.Parse(typeof(TextAnchor),text);
			
		}
		
		/// <summary>Converts the given textual name into an overflow.</summary>
		public static Overflow GetOverflow(string text){
			
			text=text.Trim().ToLower();
			
			switch(text){
				
				case "inherit":
					return Overflow.Inherit;
				case "visible":
					return Overflow.Visible;
				case "hidden":
					return Overflow.Hidden;
				case "scroll":
					return Overflow.Scroll;
				default:
					return Overflow.Auto;
				
			}
			
		}
		
		/// <summary>Gets a CSS standard value. Typically e.g. "2in".</summary>
		public static Css.Value Get(string valueText){
			
			// Load it (never null):
			Css.Value value=Css.Value.Load(valueText);
			
			return value;
			
		}
		
		/// <summary>Converts the given textual name into a box region.</summary>
		public static BoxRegion GetViewbox(string valueText){
			
			string[] coords = valueText.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
			
			if (coords.Length != 4)
			{
				Dom.Log.Add("Warning: Broken SVG viewbox attribute of '"+valueText+"'.");
			}

			return new BoxRegion(
				float.Parse(coords[0]),
				float.Parse(coords[1]),
				float.Parse(coords[2]),
				float.Parse(coords[3])
			);
			
		}
		
	}
	
}