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
	/// Represents the background-clip: css property.
	/// </summary>
	
	public class BackgroundClip:CssProperty{
		
		public override string[] GetProperties(){
			return new string[]{"background-clip"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the background image (always created if needed):
			Css.BackgroundImage image=GetBackground(style);
			
			if(value==null){
				image.Clipping=BackgroundClipping.BorderBox;
			}else{
				
				switch(value.Text){
					case "text":
						image.Clipping=BackgroundClipping.Text;
					break;
					case "padding-box":
						image.Clipping=BackgroundClipping.PaddingBox;
					break;
					case "content-box":
						image.Clipping=BackgroundClipping.ContentBox;
					break;
					default:
						image.Clipping=BackgroundClipping.BorderBox;
					break;
				}
				
			}
			
			// Request a layout:
			image.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}

namespace Css{

	public partial class BackgroundImage{
		
		/// <summary>The clipping mode of this background image.</summary>
		public BackgroundClipping Clipping=BackgroundClipping.BorderBox;
		
	}
	
}



