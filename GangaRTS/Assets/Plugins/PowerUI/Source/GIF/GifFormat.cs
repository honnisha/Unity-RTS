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
using Dom;
using Spa;
using PowerUI;


namespace Gif{
	
	/// <summary>
	/// Represents the GIF format.
	/// </summary>
	
	public class GifFormat:ImageFormat{
		
		/// <summary>The GIF that is in use, if any.</summary>
		public Gif GifFile;
		/// <summary>An instance of the animation retrieved.</summary>
		public SPAInstance Animation;
		
		
		public GifFormat(){
		}
		
		public GifFormat(Gif gif){
			GifFile=gif;
		}
		
		public override string[] GetNames(){
			return new string[]{"gif"};
		}
		
		public override int Height{
			get{
				return GifFile.Height;
			}
		}
		
		public override int Width{
			get{
				return GifFile.Width;
			}
		}
		
		public override bool Loaded{
			get{
				return (GifFile!=null);
			}
		}
		
		public override bool Isolate{
			get{
				return true;
			}
		}
		
		public override Material GetImageMaterial(Shader shader){
			
			if(Animation==null){
				GoingOnDisplay(null);
			}
			
			Material m=Animation.AnimatedMaterial;
			
			if(m==null){
				Animation.Setup(shader);
				m=Animation.AnimatedMaterial;
			}else{
				// Update shader:
				m.shader=shader;
			}
			
			return m;
			
		}
		
		public override bool LoadData(byte[] data,ImagePackage package){
			
			// Create it now:
			GifFile=new Gif(data);
			
			return true;
			
		}
		
		public override ImageFormat Instance(){
			return new GifFormat();
		}
		
		public override bool InternallyCached(Location path,ImagePackage package){
			
			// GIF uses SPA's so it could cache like they do.
			
			return false;
			
		}
		
		public override void ClearX(){
			GifFile=null;
		}
		
		/// <summary>A shortcut for instancing it.</summary>
		public Material Start(){
			return Start(false);
		}
		
		/// <summary>A shortcut for instancing it.</summary>
		public Material Start(bool lit){
			return Start(lit ? ShaderSet.StandardLit.Isolated : ShaderSet.Standard.Isolated);
		}
		
		/// <summary>A shortcut for instancing it.</summary>
		public Material Start(Shader shader){
			if(GifFile==null){
				throw new Exception("Tried to load a broken GIF.");
			}
			
			if(Animation==null){
				Animation=GifFile.GetInstance();
			}
			
			// Setup using the given shader:
			Animation.Setup(shader);
			
			return Animation.AnimatedMaterial;
		}
		
		public override void GoingOnDisplay(Css.RenderableData context){
			
			if(Animation==null && GifFile!=null){
				Animation=GifFile.GetInstance();
				
				// Set the event context now:
				Animation.SetContext(context);
			}
			
		}
		
		public override void GoingOffDisplay(){
		
			if(Animation!=null){
				Animation.Stop();
				Animation=null;
			}
			
		}
		
	}
	
}

namespace Gif{
	
	public partial class Gif{
		
		/// <summary>Implicitly converts a GIF into a GifFormat object.
		/// Used when adding a GIF to the image cache.</summary>
		public static implicit operator GifFormat(Gif gif){
			return new GifFormat(gif);
		}
		
	}
	
}