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
using PowerUI;


namespace Spa{
	
	/// <summary>
	/// Represents the SPA animation format.
	/// </summary>
	
	public class SpaFormat:ImageFormat{
		
		/// <summary>The animation that is in use, if any.</summary>
		public SPA SPAFile;
		/// <summary>An instance of the animation retrieved.</summary>
		public SPAInstance Animation;
		
		
		public SpaFormat(){}
		
		public SpaFormat(SPA spa){
			SPAFile=spa;
		}
		
		public override string[] GetNames(){
			return new string[]{"spa"};
		}
		
		public override int Height{
			get{
				// We want the height of a frame:
				return SPAFile.FrameHeight;
			}
		}
		
		public override int Width{
			get{
				// We want the width of a frame:
				return SPAFile.FrameWidth;
			}
		}
		
		public override bool Loaded{
			get{
				return (SPAFile!=null);
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
			SPAFile=new SPA(package.location.absoluteNoHash,data);
			
			return true;
			
		}
		
		public override ImageFormat Instance(){
			return new SpaFormat();
		}
		
		public override bool InternallyCached(Location path,ImagePackage package){
			
			// Is it an animation that has been cached?
			
			// Might already be loaded - let's check:
			SPAFile=SPA.Get(path.absoluteNoHash);
			
			if(SPAFile!=null){
				//It's already been loaded - use that.
				package.Done();
				return true;
			}
			
			return false;
			
		}
		
		public override void ClearX(){
			SPAFile=null;
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
			if(SPAFile==null){
				throw new Exception("Tried to load a broken SPA.");
			}
			
			if(Animation==null){
				Animation=SPAFile.GetInstance();
			}
			
			// Setup using the given shader:
			Animation.Setup(shader);
			
			return Animation.AnimatedMaterial;
		}
		
		public override void GoingOnDisplay(Css.RenderableData context){
			
			if(Animation==null && SPAFile!=null){
				Animation=SPAFile.GetInstance();
				
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
	
	public partial class SPA{
		
		/// <summary>Implicitly converts an SPA into an SPAFormat object.
		/// Used when adding an SPA to the image cache.</summary>
		public static implicit operator SpaFormat(SPA spa){
			return new SpaFormat(spa);
		}
		
	}
	
}