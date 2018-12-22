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
using UnityEngine;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Represents a special image format used by the CSS engine. For example, gradients use this.
	/// </summary>
	
	public class SparkSpecialImageFormat:ImageFormat{
		
		/// <summary>The underlying image.</summary>
		public Texture2D Image;
		/// <summary>An isolated material for this image.</summary>
		private Material IsolatedMaterial;
		
		
		public override string[] GetNames(){
			return new string[]{"-spark-img"};
		}
		
		public override Material GetImageMaterial(Shader shader){
			
			if(IsolatedMaterial==null){
				IsolatedMaterial=new Material(shader);
				IsolatedMaterial.SetTexture("_MainTex",Image);
			}
			
			return IsolatedMaterial;
		}
		
		public override Texture Texture{
			get{
				return Image;
			}
		}
		
		public override bool Isolate{
			get{
				return true;
			}
		}
		
		public override ImageFormat Instance(){
			return new SparkSpecialImageFormat();
		}
		
		/// <summary>Called when the host element is drawing.</summary>
		public override void OnLayout(RenderableData context,LayoutBox box,out float width,out float height){
			
			// Same size as the box:
			width=box.PaddedWidth;
			height=box.PaddedHeight;
			
		}
		
		public override int Height{
			get{
				return Image.height;
			}
		}
		
		public override int Width{
			get{
				return Image.width;
			}
		}
		
		public override bool Loaded{
			get{
				return (Image!=null);
			}
		}
		
		public override void ClearX(){
			Image=null;
		}
		
	}
	
}