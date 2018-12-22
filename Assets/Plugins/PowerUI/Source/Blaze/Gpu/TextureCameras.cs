//--------------------------------------
//                Blaze
//
//        For documentation or 
//    if you have any issues, visit
//        powerui.kulestar.com
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;


namespace Blaze{
	
	public static class TextureCameras{
		
		/// <summary>Should copying from the cameras be done on the CPU? 
		/// Required by InfiniText - unfortunately Alpha8 is not supported by ReadPixels.</summary>
		public static bool CPUCopyMode=true;
		/// <summary>Should the drawings be SD? SD/HD makes zero visual difference; it just affects how frequently the renderer samples the shapes.
		/// Generally only set this to false if you want to do unusual glows (by changing Spread per sample).</summary>
		public static bool SD=true;
		/// <summary>The scale applied to meshes being drawn.</summary>
		private static float Scale;
		/// <summary>The accuracy of the edges used by the drawing process. Don't set this directly.</summary>
		public static float Accuracy=0.025f;
		/// <summary>The accuracy of the edges used by the drawing process. Don't set this directly.</summary>
		public static float TriangulationAccuracy=Accuracy * 6f;
		/// <summary>The layer to draw on.</summary>
		public static int Layer=23;
		/// <summary>The current queue of pending drawings.</summary>
		public static DrawingTexture Pending;
		/// <summary>The main camera which will do the drawing.</summary>
		public static TextureCamera Camera;
		/// <summary>A linked list of mesh buffer objects.</summary>
		public static MeshBuffer BufferPool;
		
		
		/// <summary>Requests to draw the given path at the given atlas location.</summary>
		public static void RequestDraw(AtlasLocation location,VectorPath path,float offsetX,float offsetY,float drawHeight){
			DrawingTexture drawing=new DrawingTexture();
			drawing.Location=location;
			drawing.Path=path;
			drawing.OffsetX=offsetX;
			drawing.OffsetY=offsetY;
			
			if(Camera==null){
				Camera=new TextureCamera(CPUCopyMode);
				
				// Apply scale:
				Scale=drawHeight * Camera.WorldPerPixel.x;
				
			}
			
			if(Camera.IsDrawing || !Camera.TryFit(drawing)){
				
				// Add to global pending queue:
				drawing.NextDrawing=Pending;
				Pending=drawing;
				
			}
			
		}
		
		public static void Update(float deltaTime){
			
			if(Camera==null){
				return;
			}
			
			if(Camera.RequiresRender){
				
				// Draw it right now:
				// Note that this also resets timer.
				Camera.DrawNow();
				
			}
			
			// Advance timer so we can know when to destroy it:
			Camera.Timer+=deltaTime;
			
			// Been around too long?
			if(Camera.Timer>TextureCamera.MaxTime){
				
				// Destroy the camera - it's been unused for 5 seconds:
				Camera.Destroy();
				
			}
			
		}
		
		/// <summary>Adds a buffer to the pool.</summary>
		public static void PoolBuffer(MeshBuffer buffer){
			
			// Hide:
			buffer.SetActive(false);
			
			// Push:
			buffer.NextInPool=BufferPool;
			BufferPool=buffer;
			
		}
		
		/// <summary>Gets a pooled mesh buffer, or creates one if the pool is empty.</summary>
		public static MeshBuffer GetBuffer(){
			
			MeshBuffer current=BufferPool;
			
			if(current==null){
				current=new MeshBuffer();
				current.XScaleFactor=-Scale;
				current.YScaleFactor=Scale;
			}else{
				
				if(current.Gameobject==null){
					// Scene change:
					current.CreateGameObject();
				}
				
				BufferPool=current.NextInPool;
			}
			
			return current;
			
		}
		
	}
	
}