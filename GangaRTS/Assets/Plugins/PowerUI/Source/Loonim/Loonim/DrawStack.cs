#if !NO_BLADE_RUNTIME

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Loonim{
	
	/// <summary>
	/// A stack of nodes used for GPU drawing.
	/// Note that the stack is built "backwards" - i.e. we start by adding the top most node and go down.
	/// </summary>
	
	public class DrawStack{
		
		/// <summary>The current top.</summary>
		public DrawStackNode Top;
		/// <summary>The current bottom.</summary>
		public DrawStackNode Bottom;
		/// <summary>The render texture.</summary>
		public RenderTexture DrawZone;
		
		
		public DrawStack(DrawInfo info){
			Allocate(info);
		}
		
		public DrawStack(){}
		
		/// <summary>Allocates (or reallocates) the texture now.</summary>
		public void Allocate(int width,int height,bool hdr,FilterMode mode){
			if(DrawZone!=null){
				// Destroy it now:
				UnityEngine.Texture.Destroy(DrawZone);
			}
			
			// Create:
			DrawZone=new RenderTexture(width,height,0,hdr ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.ARGB32);
			DrawZone.wrapMode=TextureWrapMode.Repeat;
			DrawZone.filterMode=mode;
		}
		
		/// <summary>Allocates (or reallocates) the texture now.</summary>
		public void Allocate(DrawInfo info){
			if(DrawZone!=null){
				// Destroy it now:
				UnityEngine.Texture.Destroy(DrawZone);
			}
			
			// Create:
			DrawZone=new RenderTexture(info.ImageX,info.ImageY,0,info.HDR ? RenderTextureFormat.ARGBFloat : RenderTextureFormat.ARGB32);
			DrawZone.wrapMode=TextureWrapMode.Repeat;
			DrawZone.filterMode=info.FilterMode;
		}
		
		/// <summary>Tidies up this stack.</summary>
		public void Destroy(){
			
			if(DrawZone!=null){
				
				// Destroy it now:
				UnityEngine.Texture.Destroy(DrawZone);
				DrawZone=null;
				
			}
			
		}
		
	}
	
	/// <summary>
	/// A single node on the stack.
	/// </summary>
	
	public class DrawStackNode{
	
		public virtual Texture Texture{
			get{
				return null;
			}
		}
		
		public virtual Material NodeMaterial{
			get{
				return null;
			}
		}
		
		/// <summary>Set if this node should transparent clear (GL.Clear) right before it draws.
		/// Only applies to nodes which use MaterialStackNodeCleared.</summary>
		public virtual bool Clear{
			get{
				return false;
			}
			set{}
		}
		
		/// <summary>Draws this node now.</summary>
		public virtual void Draw(DrawInfo info){
		
		}
		
		/// <summary>Writes Texture to the given one, filling it.
		/// Might allocate a pixel buffer which is reffed so it can be reused in a secondary call.</summary>
		public virtual void WriteTo(Texture2D image,DrawInfo info,ref Color[] pixels){
			
		}
		
	}
	
	public class LiveStackNode : DrawStackNode{
		
		/// <summary>The live image.</summary>
		public Texture Image;
		/// <summary>The node to be baked.</summary>
		public TextureNode Node;
		
		
		public LiveStackNode(TextureNode node,Texture img){
			Node=node;
			
			// Raw image (no buffer though):
			Image=img;
			Image.wrapMode=TextureWrapMode.Repeat;
			
		}
		
		public override Texture Texture{
			get{
				return Image;
			}
		}
		
	}
	
	public class TextureStackNode : DrawStackNode{
		
		/// <summary>The graph/ constant.</summary>
		public Texture2D Image;
		/// <summary>The node to be baked.</summary>
		public TextureNode Node;
		/// <summary>The draw buffer.</summary>
		internal Color[] Buffer;
		
		
		public TextureStackNode(TextureNode node,bool hdr,int x){
			Node=node;
			
			// 0D (x==1) values potentially hold a 4 channel colour.
			// 1D (else) values will only ever hold a 1 channel colour.
			TextureFormat format;
			
			if(hdr){
				format=(x==1)?TextureFormat.RGBAFloat : TextureFormat.RFloat;
			}else{
				format=TextureFormat.ARGB32;
			}
			
			Image=new Texture2D(x,1,format,false);
			Image.wrapMode=TextureWrapMode.Repeat;
			Buffer=new Color[x];
			
		}
		
		public void Bake(){
			
			// Bake into the buffer:
			Node.Bake(Buffer);
			
			// Write out:
			Image.SetPixels(Buffer);
			Image.Apply();
			
		}
		
		/// <summary>Draws this node now.</summary>
		public override void Draw(DrawInfo info){
			
			int size=Buffer.Length;
			
			// Check the buffer is the right size:
			if(size!=1 && size!=info.ImageX){
				
				// Resize required.
				Buffer=new Color[info.ImageX];
				
				// Resize:
				Image.Resize(info.ImageX,1);
				
			}
			
			// Bake into the buffer:
			Node.Bake(Buffer);
			
			// Write out:
			Image.SetPixels(Buffer);
			Image.Apply();
			
		}
		
		/// <summary>Writes Texture to the given one, filling it.</summary>
		public override void WriteTo(Texture2D image,DrawInfo info,ref Color[] pixels){
			
			// We'll use 'Buffer' to blit the pixels into our pixel array.
			int max;
			int width=info.ImageX;
			
			if(pixels==null){
				max=width*info.ImageY;
				pixels=new Color[max];
			}else{
				max=pixels.Length;
			}
			
			if(Image.width==width){
				
				// Graph - blit the line down the pixel buffer:
				
				for(int index=0;index<max;index+=width){
					
					Array.Copy(Buffer,0,pixels,index,width);
					
				}
				
			}else{
				
				// Single pixel (const).
				for(int index=0;index<max;index++){
					pixels[index]=Buffer[0];
				}
				
			}
			
			image.SetPixels(pixels);
			image.Apply();
			
		}
		
		public override Texture Texture{
			get{
				return Image;
			}
		}
		
	}
	
	public class StackerStackNode : DrawStackNode{
		
		/// <summary>The stack that this outputs to.</summary>
		public DrawStack Stack;
		
		public override Texture Texture{
			get{
				return Stack.DrawZone;
			}
		}
		
	}
	
	/// <summary>
	/// The same as a MaterialStackNode only this optionally clears before it draws.
	/// </summary>
	public class MaterialStackNodeCleared : MaterialStackNode{
		
		/// <summary>Clears by default.</summary>
		private bool Clear_=true;
		
		public override bool Clear{
			get{
				return Clear_;
			}
			set{
				Clear_=value;
			}
		}
		
		/// <summary>Draws this node now.</summary>
		public override void Draw(DrawInfo info){
			
			// Select our render texture:
			RenderTexture.active=Stack.DrawZone;
			
			if(Clear_){
				// Clear now:
				GL.Clear(false,true,Color.clear,1f);
			}
			
			// Set material:
			Material.SetPass(0);
			
			// Draw now:
			UnityEngine.Graphics.DrawMeshNow(Mesh,UnityEngine.Matrix4x4.identity);
			
		}
		
	}
	
	public class MaterialStackNode : DrawStackNode{
		
		/// <summary>The mesh to draw.</summary>
		public Mesh Mesh;
		/// <summary>The stack that this material outputs to.</summary>
		public DrawStack Stack;
		/// <summary>The material to draw.</summary>
		public Material Material;
		
		
		public override Texture Texture{
			get{
				return Stack.DrawZone;
			}
		}
		
		/// <summary>Draws this node now.</summary>
		public override void Draw(DrawInfo info){
			
			// Select our render texture:
			RenderTexture.active=Stack.DrawZone;
			
			// Set material:
			Material.SetPass(0);
			
			// Draw now:
			UnityEngine.Graphics.DrawMeshNow(Mesh,UnityEngine.Matrix4x4.identity);
			
		}
		
		/// <summary>Writes Texture to the given one, filling it.</summary>
		public override void WriteTo(Texture2D image,DrawInfo info,ref Color[] pixels){
			
			// Read out from a RenderTexture.
			
			// Apply and set:
			RenderTexture.active=Stack.DrawZone;
			
			// Read pixels:
			image.ReadPixels(new Rect(0,0,info.ImageX,info.ImageY),0,0);
			image.Apply();
			
		}
		
		public override Material NodeMaterial{
			get{
				return Material;
			}
		}
		
	}
	
	public class BatchStackNode : DrawStackNode{
		
		/// <summary>The meshes to draw.</summary>
		public Mesh[] Meshes;
		/// <summary>The stack that this material outputs to.</summary>
		public DrawStack Stack;
		/// <summary>The material to draw.</summary>
		public Material Material;
		
		
		public override Texture Texture{
			get{
				return Stack.DrawZone;
			}
		}
		
		public override Material NodeMaterial{
			get{
				return Material;
			}
		}
		
		/// <summary>Draws this node now.</summary>
		public override void Draw(DrawInfo info){
			
			// Select our render texture:
			RenderTexture.active=Stack.DrawZone;
			
			// Set material:
			Material.SetPass(0);
			
			for(int i=0;i<Meshes.Length;i++){
				// Draw now:
				UnityEngine.Graphics.DrawMeshNow(Meshes[i],UnityEngine.Matrix4x4.identity);
			}
			
		}
		
		/// <summary>Writes Texture to the given one, filling it.</summary>
		public override void WriteTo(Texture2D image,DrawInfo info,ref Color[] pixels){
			
			// Read out from a RenderTexture.
			
			// Apply and set:
			RenderTexture.active=Stack.DrawZone;
			
			// Read pixels:
			image.ReadPixels(new Rect(0,0,info.ImageX,info.ImageY),0,0);
			image.Apply();
			
		}
		
	}
	
	public class BlockStackNode : DrawStackNode{
		
		/// <summary>The mesh to draw.</summary>
		public Mesh Mesh;
		/// <summary>The stack that this material outputs to.</summary>
		public DrawStack Stack;
		/// <summary>The materials to draw.</summary>
		public Material[] Materials;
		
		
		public override Texture Texture{
			get{
				return Stack.DrawZone;
			}
		}
		
		/// <summary>Draws this node now.</summary>
		public override void Draw(DrawInfo info){
			
			// Select our render texture:
			RenderTexture.active=Stack.DrawZone;
			
			for(int i=0;i<Materials.Length;i++){
				
				// Set material:
				Materials[i].SetPass(0);
				
				// Draw now:
				UnityEngine.Graphics.DrawMeshNow(Mesh,UnityEngine.Matrix4x4.identity);
				
			}
			
		}
		
		/// <summary>Writes Texture to the given one, filling it.</summary>
		public override void WriteTo(Texture2D image,DrawInfo info,ref Color[] pixels){
			
			// Read out from a RenderTexture.
			
			// Apply and set:
			RenderTexture.active=Stack.DrawZone;
			
			// Read pixels:
			image.ReadPixels(new Rect(0,0,info.ImageX,info.ImageY),0,0);
			image.Apply();
			
		}
		
	}
	
}
#endif