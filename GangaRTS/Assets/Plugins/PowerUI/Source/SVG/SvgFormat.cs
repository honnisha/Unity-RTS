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


namespace Svg{
	
	/// <summary>
	/// Represents the SVG Image format.
	/// </summary>
	
	public class SVGFormat:ImageFormat{
		
		/// <summary>The SVG retrieved.</summary>
		public SVG Svg;
		/// <summary>The latest render context.</summary>
		public RenderContext Context;
		/// <summary>An isolated material for this image.</summary>
		private Material IsolatedMaterial;
		
		
		public SVGFormat(){}
		
		public SVGFormat(SVG svg){
			Svg=svg;
			
			// Get the rendering context:
			Context=Svg.svgElement.Context;
			
		}
		
		public override bool LoadData(byte[] data,ImagePackage package){
			
			string xml=System.Text.Encoding.UTF8.GetString(data);
			
			// Create the SVG:
			Svg=new SVG(xml);
			
			// Get the rendering context:
			Context=Svg.svgElement.Context;
			
			return true;
		}
		
		/// <summary>Called just before the image is about to be drawn (only ever when it's actually visible).
		/// Note that everything else - e.g. ImageMaterial or Width/Height - is always called after this.</summary>
		public override void OnLayout(Css.RenderableData context,LayoutBox box,out float width,out float height){
			
			// Get the shape of the element:
			width=box.PaddedWidth;
			height=box.PaddedHeight;
			
			// Tell the context which may trigger a redraw:
			Context.SetSize((int)width,(int)height);
		}
		
		public override string[] GetNames(){
			return new string[]{"svgx"};
		}
		
		public override Material GetImageMaterial(Shader shader){
			
			if(IsolatedMaterial==null){
				IsolatedMaterial=new Material(shader);
				IsolatedMaterial.SetTexture("_MainTex",Context.Texture);
			}
			
			return IsolatedMaterial;
			
		}
		
		public override Texture Texture{
			get{
				if(Context==null){
					return null;
				}
				return Context.Texture;
			}
		}
		
		public override bool Isolate{
			get{
				return true;
			}
		}
		
		public override ImageFormat Instance(){
			return new SVGFormat();
		}
		
		public override int Height{
			get{
				return Svg.Height;
			}
		}
		
		public override int Width{
			get{
				return Svg.Width;
			}
		}
		
		public override bool Loaded{
			get{
				return (Svg!=null);
			}
		}
		
		public override void ClearX(){
			Svg=null;
		}
		
	}
	
	public partial class SVG{
		
		/// <summary>Implicitly converts an SVG into an SVGFormat object.</summary>
		public static implicit operator SVGFormat(SVG svg){
			return new SVGFormat(svg);
		}
		
	}
	
}