using System;
using UnityEngine;

namespace Loonim{
	
	public enum BlurMethod:int{
		Box=0,
		Gaussian=1
	}
	
	public class Blur : BitmapNode{
		
		/// <summary>The blur method to use.</summary>
		private int Method_;
		
		private Color[] Buffer2;
		
		/// <summary>Horizontal radius.</summary>
		public TextureNode RadiusX{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>Vertical radius.</summary>
		public TextureNode RadiusY{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The blur method to use.</summary>
		public TextureNode Method{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		/// <summary>
		/// The blur method.
		/// </summary>
		public BlurMethod BlurMethod{
			get{
				return (BlurMethod)Method_;
			}
			set{
				Method = new Property((int)value);
			}
		}
		
		/// <summary>By default, materials are named Loonim/Texture_node_id, however some nodes have "sub-materials"
		/// where they essentially have a bunch of different shaders. An example is the Blend node.</summary>
		public override int SubMaterialID{
			get{
				return Method_;
			}
		}
		
		public Blur():base(4){}
		
		public Blur(TextureNode src,TextureNode rx,TextureNode ry,TextureNode method):base(4){
			SourceModule=src;
			RadiusX=rx;
			RadiusY=ry;
			Method=method;
		}
		
		public override void Draw(DrawInfo info){
			
			// Always pull the latest method, checking if it's changed:
			Property pv=Method as Property;
			
			if(pv!=null){
				
				// Update now if we can:
				pv.UpdateIfChanged(info);
			
			}
			
			int method=(int)(Method.GetValue(0,0));
			
			if(method!=Method_){
				
				// Change!
				Method_=method;
				
				// Allocate shader now:
				SubMaterialChanged();
				
			}
			
			// Check for changes on radius:
			bool updateData=false;
			pv=RadiusX as Property;
			
			if(pv!=null){
				
				// Update now if we can:
				if(pv.UpdateIfChanged(info)){
					
					updateData=true;
					
				}
				
			}
			
			pv=RadiusY as Property;
			
			if(pv!=null){
				
				// Update now if we can:
				if(pv.UpdateIfChanged(info)){
					
					updateData=true;
					
				}
				
			}
			
			if(updateData){
				
				MaterialStackNode msn=DrawStore as MaterialStackNode;
				
				if(msn!=null){
					
					msn.Material.SetVector("_Data",GetDataVector(info));
					
				}
				
			}
			
			// Draw now:
			base.Draw(info);
			
		}
		
		#if !NO_BLADE_RUNTIME
		
		/// <summary>The special _Data vector used by the noise group of materials.</summary>
		private Vector4 GetDataVector(DrawInfo info){
			
			// Radius in UV space:
			float rX=1f / (float)info.ImageX;//(float)RadiusX.GetValue(0,0) / (float)info.ImageX;
			float rY=1f / (float)info.ImageY;//(float)RadiusY.GetValue(0,0) / (float)info.ImageY;
			
			return new Vector4(rX,rY,rX,-rY);
		}
		
		public override DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			// Stack required.
			
			// Allocate a target stack now:
			int targetStack=stackID;
			DrawStack stack=tex.GetStack(targetStack,info);
			stackID++;
			
			int subID=SubMaterialID;
			
			// Create the material:
			Material material=GetMaterial(TypeID,subID);
			
			// Allocate Src0:
			TextureNode input=Sources[0];
			int inputStacks=(targetStack==0)?1:0;
			DrawStackNode drawNode=input.Allocate(info,tex,ref inputStacks);
			
			// Apply it to our material:
			material.SetTexture("_Src0",drawNode.Texture);
			
			// _Data (Rx,Ry,Rx,-Ry):
			material.SetVector(
				"_Data",
				GetDataVector(info)
			);
			
			// Create our node:
			MaterialStackNode matNode=new MaterialStackNode();
			DrawStore=matNode;
			matNode.Mesh=info.Mesh;
			matNode.Material=material;
			matNode.Stack=stack;
			
			return matNode;
			
		}
		
		#endif
		
		public override void Prepare(DrawInfo info){
			
			base.Prepare(info);
			
			if(info.Mode==SurfaceDrawMode.CPU){
				
				// Prerender source now!
				
				int width=info.ImageX;
				int height=info.ImageY;
				
				// Buffer etc:
				Setup(info);
				
				// Setup secondary buffer:
				if(Buffer2==null || Buffer2.Length!=Buffer.Length){
					Buffer2=new Color[Buffer.Length];
				}
				
				// Draw source now:
				SourceModule.DrawCPU(info,Buffer2);
				
				Width=width;
				Height=height;
				
				// Get radii:
				float hRadius=(float)( width * RadiusX.GetValue(0.0,0.0) );
				float vRadius=(float)( height * RadiusY.GetValue(0.0,0.0) );
				
				// Box or gaus blur:
				if(Method_ == (int)BlurMethod.Box){
					
					// Blurs and flips (horizontal for vertical):
					BoxBlurFilter.BlurAndTranspose( Buffer2, Buffer, width, height, (int)hRadius );
					BoxBlurFilter.BlurAndTranspose( Buffer, Buffer2, height, width, (int)vRadius );
					
				}else{
					
					Kernel kernelH=GaussianFilter.MakeKernel(hRadius);
					Kernel kernelV;
					
					if(vRadius==hRadius){
						kernelV=kernelH;
					}else{
						kernelV=GaussianFilter.MakeKernel(vRadius);
					}
					
					// Blurs and flips (horizontal for vertical):
					GaussianFilter.ConvolveAndTranspose(kernelH, Buffer2, Buffer, width, height,true,ConvolveWrapping.ClampEdges);
					GaussianFilter.ConvolveAndTranspose(kernelV, Buffer, Buffer2, height, width,true,ConvolveWrapping.ClampEdges);
					
				}
				
			}
			
		}
		
		public override int TypeID{
			get{
				return 35;
			}
		}
		
	}
	
}