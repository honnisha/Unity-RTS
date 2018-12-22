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
using UnityEngine;


namespace Blaze{
	
	/// <summary>
	/// When Blaze is drawing graphics, it may spawn a camera to draw the image in 3D space.
	/// This in short means Blaze can use fully GPU accelerated drawing.
	/// </summary>
	
	public class TextureCamera{
		
		/// <summary>The max amount of time a texture camera can hang around.</summary>
		public const float MaxTime=5f;
		/// <summary>The default camera size.</summary>
		private const int DefaultSize=256;
		/// <summary>Global position offset.</summary>
		public static float GlobalOffset=-150f;
		
		/// <summary>A timer tracking how long this camera has been around. If it goes beyond MaxTime, this camera is destroyed.</summary>
		public float Timer=0f;
		/// <summary>The base X offset.</summary>
		public float XOffset;
		/// <summary>The base Y offset.</summary>
		public float YOffset;
		/// <summary>The camera itself.</summary>
		public Camera SourceCamera;
		/// <summary>The parent gameobject.</summary>
		public GameObject Gameobject;
		/// <summary>The cameras gameobject.</summary>
		public GameObject CameraObject;
		/// <summary>World units per pixel.</summary>
		public Vector2 WorldPerPixel=new Vector2(0.1f,0.1f);
		/// <summary>The size of the texture. Same as both pixelW and H.</summary>
		public int Size;
		/// <summary>True if this camera has letters that require rendering.</summary>
		public bool RequiresRender;
		/// <summary>The width of the texture space.</summary>
		public int pixelWidth;
		/// <summary>The amount of pixels that have been filled with content on the X axis.</summary>
		public int FillX;
		/// <summary>The amount of pixels that have been filled with content on the Y axis.</summary>
		public int FillY;
		/// <summary>True if all copying from this camera happens on the CPU.</summary>
		public bool CPUAccess;
		/// <summary>The pixel set used for copying from.</summary>
		private Color32[] CPUPixels;
		/// <summary>An awkward texture used as a temporary buffer for CPU copying.</summary>
		private Texture2D CPUBuffer;
		/// <summary>Current column width.</summary>
		public int MaxWidth;
		/// <summary>Is this camera available? (i.e. still got space)</summary>
		public bool Available;
		/// <summary>True if this camera is currently drawing.</summary>
		public bool IsDrawing;
		/// <summary>The height of the texture space.</summary>
		public int pixelHeight;
		/// <summary>Size divided by WorldPerPixel x.</summary>
		public float HalfSizeByWorld;
		/// <summary>A triangulator used for triangulating the interior of the shapes.</summary>
		public Triangulator Triangulator;
		/// <summary>The first object being drawn with this camera.</summary>
		public DrawingTexture FirstDrawing;
		
		
		/// <summary>Creates a new camera. 256px texture space.</summary>
		public TextureCamera(bool cpu):this(DefaultSize,cpu){}
		
		/// <summary>Creates a new camera with the given texture space.</summary>
		public TextureCamera(int size,bool cpu):this(size,size,cpu){
			
		}
		
		/// <summary>Creates a new camera with the given texture space.</summary>
		public TextureCamera(int sizeX,int sizeY,bool cpu){
		
			Size=sizeX;
			pixelWidth=sizeX;
			pixelHeight=sizeY;
			CPUAccess=cpu;
			
			// Get SBX:
			HalfSizeByWorld=((float)pixelWidth / 2f) * WorldPerPixel.x;
			
			// Create gameobject:
			CreateGameObject();
			
			// Apply offsets:
			XOffset=(float)sizeX * WorldPerPixel.x;
			
			// Create triangulator:
			Triangulator=new Triangulator(null,0,0);
			
			// CPU texture buffer:
			if(CPUAccess){
				CPUBuffer=new Texture2D(sizeX,sizeY,TextureFormat.ARGB32,false);
			}
			
		}
		
		public GameObject CreateGameObject(){
			
			// Create the root gameobject:
			Gameobject=new GameObject();
			
			Gameobject.name="#RenderDeck-PowerUI";
			
			// Create camera gameobject:
			CameraObject=new GameObject();
			
			// Parent the camera to the root:
			CameraObject.transform.parent=Gameobject.transform;
			
			// Add a camera:
			SourceCamera=CameraObject.AddComponent<Camera>();
			
			// Set the clear flags:
			SourceCamera.clearFlags=CameraClearFlags.Depth;
			
			// Set the culling mask:
			SourceCamera.cullingMask=(1<<TextureCameras.Layer);
			
			// Make it ortho:
			SourceCamera.orthographic=true;
			
			// Disable OC:
			SourceCamera.useOcclusionCulling=false;
			
			// Disable the camera:
			SourceCamera.enabled=false;
			
			// Make it clear colour too:
			SourceCamera.clearFlags=CameraClearFlags.SolidColor;
			SourceCamera.backgroundColor=new Color(0,0,0,0);
			
			// Set the orthographic size:
			SetOrthographicSize();
			
			// Set the distance:
			SourceCamera.farClipPlane=2f;
			CameraObject.transform.localRotation=Quaternion.AngleAxis(180f,Vector3.up);
			CameraObject.transform.localPosition=new Vector3(HalfSizeByWorld,HalfSizeByWorld,1.75f);
			
			Gameobject.transform.rotation=Quaternion.AngleAxis(-90f,new Vector3(1f,0f,0f));
			Gameobject.transform.position=new Vector3(100f,GlobalOffset,0f);
			GlobalOffset-=(float)pixelHeight * WorldPerPixel.y;
			
			// Set depth just incase:
			SourceCamera.depth=-1;
			
			if(Texture==null){
				
				// Get temp RT:
				Texture=RenderTexture.GetTemporary(pixelWidth,pixelHeight,16,RenderTextureFormat.ARGB32,RenderTextureReadWrite.Default,1);
				
				// Create the context now:
				Texture.Create();
				
			}
			
			// Apply it:
			SourceCamera.targetTexture=Texture;
			
			return Gameobject;
		}
		
		public void Clear(){
			FirstDrawing=null;
			FillX=0;
			FillY=0;
			MaxWidth=0;
		}
		
		public bool TryFit(DrawingTexture drawing){
			
			int height=drawing.Location.Height;
			int width=drawing.Location.Width;
			
			if((FillY+height) > pixelHeight){
				
				// Too high - end the current column.
				FillX+=MaxWidth;
				MaxWidth=0;
				FillY=0;
				
			}
			
			if((FillX+width) > pixelWidth){
				return false;
			}
			
			if(width>MaxWidth){
				MaxWidth=width;
			}
			
			// Going at FillX/FillY:
			drawing.X=FillX;
			drawing.Y=FillY;
			
			FillY+=height;
			
			// Add:
			drawing.NextDrawing=FirstDrawing;
			FirstDrawing=drawing;
			
			RequiresRender=true;
			
			// Build the mesh now:
			drawing.BuildMesh(XOffset,YOffset,this);
			
			return true;
		}
		
		/// <summary>Adds the given drawing texture to the stack.</summary>
		public void Add(DrawingTexture drawing){
			
			// Add:
			drawing.NextDrawing=FirstDrawing;
			FirstDrawing=drawing;
			
			// Requires a render:
			RequiresRender=true;
			
		}
		
		private RenderTexture Texture;
		
		public void DrawNow(){
			
			RequiresRender=false;
			IsDrawing=true;
			
			if(Gameobject==null){
				
				// Scene change - recreate:
				Gameobject=CreateGameObject();
				
			}
			
			// Clear timer because a draw was requested.
			// This makes the texture camera system survive a little longer.
			Timer=0f;
		
			SourceCamera.enabled=true;
			
			// Ensure it's the right size - Force it to be square:
			SourceCamera.pixelRect=new Rect(0,0,pixelWidth,pixelHeight);
			
			// Get current active RT:
			RenderTexture prevActive=RenderTexture.active;
			
			// Apply and set:
			RenderTexture.active=Texture;
			
			// For each actual drawing..
			DrawingTexture draw=FirstDrawing;
			
			while(draw!=null){
				
				if(draw.Active){
					
					// Flush mesh:
					draw.Mesh.Flush();
					
					// Display the GO:
					draw.Mesh.SetActive(true);
					
				}
				
				// Hop to next one:
				draw=draw.NextDrawing;
				
			}
			
			// Render it right now:
			SourceCamera.Render();
			
			if(CPUAccess){
				
				// Read pixels:
				CPUBuffer.ReadPixels(new Rect(0,0,pixelWidth,pixelHeight),0,0);
				CPUBuffer.Apply();
				// Get them:
				CPUPixels=CPUBuffer.GetPixels32();
				
			}
			
			// Disable:
			SourceCamera.enabled=false;
			
			// For each actual drawing..
			DrawingTexture drawing=FirstDrawing;
			
			while(drawing!=null){
				
				// Add mesh to pool:
				TextureCameras.PoolBuffer(drawing.Mesh);
				
				if(drawing.Active){
					
					// Copy results over into target:
					ReadInto(drawing.Location,drawing.X,drawing.Y);
					
				}
				
				// Hop to next one:
				drawing=drawing.NextDrawing;
				
			}
			
			// All done!
			
			// Clear:
			RenderTexture.active=prevActive;
			
			// Clear this cameras meta data:
			Clear();
			
			// No longer drawing:
			IsDrawing=false;
			
			// Check if there's any waiting textures:
			if(TextureCameras.Pending==null){
				return;
			}
			
			// Try drawing them now:
			DrawingTexture pending=TextureCameras.Pending;
			
			while(pending!=null){
				
				DrawingTexture next=pending.NextDrawing;
				
				if(TryFit(pending)){
					
					// Next one:
					pending=next;
				
					// Remove current from pending:
					TextureCameras.Pending=pending;
					
				}else{
					break;
				}
				
			}
			
			// Any want drawing?
			if(RequiresRender){
				
				// Yep - Draw again.
				DrawNow();
				
			}
			
		}
		
		public void ReadInto(AtlasLocation atlas,int targetX,int targetY){
			
			if(CPUAccess){
				
				// CPU copy
				
				// Get the initial index:
				int startIndex=(targetY * Size) + targetX;
				
				// Get size:
				int height=atlas.Height;
				int width=atlas.Width;
				
				if(width<=0){
					// Safety check.
					return;
				}
				
				Color32[] target=atlas.Atlas.Pixels;
				int targetIndex=atlas.BottomLeftPixel();
				int targetDelta=atlas.Atlas.Dimension;
				int spacing=atlas.Spacing;
				int max=CPUPixels.Length;
				
				// For each row..
				for(int y=0;y<height;y++){
					
					if(startIndex>=max){
						// It can sometimes be a row of pixels oversubscribed.
						break;
					}
					
					// Blit:
					Array.Copy(CPUPixels,startIndex,target,targetIndex,width);
					
					// Clear the spacing:
					Array.Clear(target,targetIndex+width,spacing);
					
					// Seek a row:
					startIndex+=pixelWidth;
					targetIndex+=targetDelta;
					
				}
				
				for(int y=0;y<spacing;y++){
					
					// Clear the spacing:
					Array.Clear(target,targetIndex,width+spacing);
					targetIndex+=targetDelta;
					
				}
				
			}else{
			
				atlas.Atlas.Texture.ReadPixels(new Rect(targetX,Size - targetY - atlas.Height,atlas.Width,atlas.Height),atlas.X,atlas.Y);
			
			}
			
			// Atlas location updated:
			atlas.AtlasChanged();
			
		}
		
		/// <summary>Set the ortho size of the camera.</summary>
		public void SetOrthographicSize(){
			
			if(SourceCamera==null){
				return;
			}
			
			SourceCamera.orthographicSize=HalfSizeByWorld;
			
		}
		
		/// <summary>Permanently destroys this UI camera.</summary>
		public void Destroy(){
			
			// Clear texture:
			if(CPUBuffer!=null){
				Texture2D.Destroy(CPUBuffer);
				CPUBuffer=null;
			}
			
			CPUPixels=null;
			
			if(Texture!=null){
				
				// Return:
				RenderTexture.ReleaseTemporary(Texture);
				
			}
			
			if(Gameobject!=null){
				GameObject.Destroy(Gameobject);
				Gameobject=null;
				CameraObject=null;
			}
			
			// Clear TC:
			TextureCameras.Camera=null;
			
			// Clear BP:
			TextureCameras.BufferPool=null;
			
		}
		
	}
	
}