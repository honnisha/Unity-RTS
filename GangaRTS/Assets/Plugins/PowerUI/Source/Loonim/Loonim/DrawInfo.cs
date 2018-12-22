using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Loonim{
	
	public class DrawInfo{
		
		/// <summary>On some OpenGL platforms, we need to invert the Y coords of the meshes.</summary>
		private static float InvertY_;
		
		/// <summary>On some OpenGL platforms, we need to invert the Y coords of the meshes.</summary>
		public static float InvertY{
			get{
				
				if(InvertY_==0f){
				
					#if !NO_BLADE_RUNTIME
					
					// Get the graphics device version:
					string device = SystemInfo.graphicsDeviceVersion;
					
					// If it's OpenGL and it's not emulated..
					bool isOpenGL=device.IndexOf("OpenGL") > -1 && device.IndexOf("[emulated]")==-1;
					
					// Flip on Y:
					InvertY_ = isOpenGL ? -1f : 1f;
					
					#else
					
					InvertY_=1f;
					
					#endif
					
				}
				
				return InvertY_;
				
			}
		}
		
		/// <summary>Default image size is just 1px.</summary>
		public int ImageX=1;
		/// <summary>Default image size is just 1px.</summary>
		public int ImageY=1;
		/// <summary>The type of image to draw.</summary>
		public SurfaceTypeMode SurfaceType=SurfaceTypeMode.Albedo;
		/// <summary>The HDR mode.</summary>
		public bool HDR=true;
		/// <summary>The selected draw mode.</summary>
		public SurfaceDrawMode Mode;
		/// <summary>The pipeline filter mode to use.</summary>
		public FilterMode FilterMode=FilterMode.Bilinear;
		/// <summary>Only available during Draw() passes. This is the current parent node.</summary>
		internal TextureNode CurrentParent;
		/// <summary>Only available during Draw() passes. The current index in parents source nodes.</summary>
		internal int CurrentIndex;
		/// <summary>ImageX * ImageY.</summary>
		public int PixelCount{
			get{
				return ImageX * ImageY;
			}
		}
		/// <summary>A value to advance by on X when scanning the image.</summary>
		public float DeltaX;
		/// <summary>A value to advance by on Y when scanning the image.</summary>
		public float DeltaY;
		/// <summary>True if this is drawing a square image.</summary>
		public bool IsSquare{
			get{
				return ImageX==ImageY;
			}
		}
		
		/// <summary>No size - GPU mode.</summary>
		public DrawInfo(){
			Mode=SurfaceDrawMode.GPU;
			
			#if !NO_BLADE_RUNTIME
			Mesh=Quad;
			#endif
		}
		
		/// <summary>Non-square GPU draw info.</summary>
		public DrawInfo(int width,int height){
			Mode=SurfaceDrawMode.GPU;
			SetSize(width,height);
			
			#if !NO_BLADE_RUNTIME
			Mesh=Quad;
			#endif
		}
		
		/// <summary>Square GPU draw info.</summary>
		public DrawInfo(int size){
			Mode=SurfaceDrawMode.GPU;
			SetSize(size,size);
			
			#if !NO_BLADE_RUNTIME
			Mesh=Quad;
			#endif
		}
		
		/// <summary>Square GPU draw info.</summary>
		public DrawInfo(int size,SurfaceDrawMode mode){
			Mode=mode;
			SetSize(size,size);
			
			#if !NO_BLADE_RUNTIME
			Mesh=Quad;
			#endif
		}
		
		/// <summary>Non-square texture. Returns true if the size changed.</summary>
		public bool SetSize(int x,int y){
			
			if(x<=1){
				x=2;
			}
			
			if(y<=1){
				y=2;
			}
			
			if(x==ImageX && y==ImageY){
				return false;
			}
			
			ImageX=x;
			ImageY=y;
			
			DeltaX=1f/((float)x-1f);
			DeltaY=1f/((float)y-1f);
			
			return true;
		}
		
		/// <summary>Square texture (ImageX and ImageY are set equal to size).</summary>
		public void Square(int size){
			SetSize(size,size);
		}
		
		#if !NO_BLADE_RUNTIME
		
		/// <summary>The mesh to use for standard pipeline nodes.</summary>
		public Mesh Mesh;
		
		/// <summary>The cached fullscreen quad. See Quad.</summary>
		private static Mesh SharedQuad;
		
		/// <summary>A series of strips, each pixelsPerLine high.
		/// It's used for generating audio with the Loonim pipeline. A 256x256 image
		/// can safely generate roughly 33k samples (or about 0.75 seconds of audio) using the GPU.
		/// This method may end up being bumped into the SoundSynth module instead.</summary>
		public static Mesh CreateLines(int pixelsPerLine,int imageY){
			
			// Make:
			Mesh m=new Mesh();
			
			// Line #:
			int lines=imageY / pixelsPerLine;
			
			// Setup - each line is 4 distinct verts:
			Vector3[] verts=new Vector3[lines * 4];
			Vector2[] uv=new Vector2[verts.Length];
			Vector3[] normals=new Vector3[verts.Length];
			int[] tris=new int[lines * 6];
			
			int vertIndex=0;
			int triIndex=0;
			
			// Each line occupies a segment of the X UV space.
			// Those segments are this long (in UV units):
			float uvLineSize=1f / (float)lines;
			
			// Similarly, as lines are placed one below another,
			// they occupy a fixed amount of space on Y. As Y ranges from -1 to +1, then it's:
			float vertLineHeight=uvLineSize * 2f;
			
			// For each line..
			for(int i=0;i<lines;i++){
				
				float iF=(float)i;
				
				// UV x offset:
				float uvX=uvLineSize * iF;
				
				// The starting vert Y value:
				float vertY=(vertLineHeight * iF)-1f;
				
				// The triangles:
				tris[triIndex++]=vertIndex+1;
				tris[triIndex++]=vertIndex;
				tris[triIndex++]=vertIndex+2;
				tris[triIndex++]=vertIndex+2;
				tris[triIndex++]=vertIndex;
				tris[triIndex++]=vertIndex+3;
				
				// First corner:
				normals[vertIndex]=Vector3.up;
				uv[vertIndex]=new Vector2(uvX,uvLineSize);
				verts[vertIndex++]=new Vector3(-1f,vertY,0f);
				
				// Second corner:
				normals[vertIndex]=Vector3.up;
				uv[vertIndex]=new Vector2(uvX,0f);
				verts[vertIndex++]=new Vector3(-1f,vertY+vertLineHeight,0f);
				
				// Now the end of the line - move UVX:
				uvX+=uvLineSize;
				
				// Third corner:
				normals[vertIndex]=Vector3.up;
				uv[vertIndex]=new Vector2(uvX,0f);
				verts[vertIndex++]=new Vector3(1f,vertY+vertLineHeight,0f);
				
				// Fourth corner:
				normals[vertIndex]=Vector3.up;
				uv[vertIndex]=new Vector2(uvX,uvLineSize);
				verts[vertIndex++]=new Vector3(1f,vertY,0f);
				
			}
			
			m.vertices=verts;
			m.uv=uv;
			m.normals=normals;
			m.triangles=tris;
			m.UploadMeshData(true);
			
			return m;
			
		}
		
		/// <summary>A full screen quad. This is the most commonly used mesh type 
		/// (it gets set to the DrawInfo.Mesh property by default).</summary>
		public static Mesh Quad{
			
			get{
			
				if(SharedQuad==null){
					
					float mul = InvertY;
					
					// Make:
					Mesh m=new Mesh();
					
					// Setup:
					m.vertices=new Vector3[]{
						new Vector3(-1f,-1f * mul,0f),
						new Vector3(-1f,1f * mul,0f),
						new Vector3(1f,1f * mul,0f),
						new Vector3(1f,-1f * mul,0f)
					};
					
					m.uv=new Vector2[]{
						new Vector2(0f,1f),
						new Vector2(0f,0f),
						new Vector2(1f,0f),
						new Vector2(1f,1f)
					};
					
					m.normals=new Vector3[]{
						Vector3.up,
						Vector3.up,
						Vector3.up,
						Vector3.up
					};
					
					m.triangles=new int[]{
						1,0,2,2,0,3
					};
				
					SharedQuad=m;
					m.UploadMeshData(true);
					
				}
				
				return SharedQuad;
			
			}
			
		}
		
		#endif
		
	}
	
}