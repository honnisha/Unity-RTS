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
using Dom;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Blaze;
using Css;
using PowerUI;


namespace Svg{
	
	/// <summary>
	/// Used to describe how to render an SVG and where to.
	/// </summary>
	
	public class RenderContext{
		
		/// <summary>Current depth.</summary>
		public float Depth;
		/// <summary>The accuracy when triangulating paths In terms of 0-2 where 2 represents the lowest accuracy.</summary>
		public float PathAccuracy=0.05f;
		/// <summary>Region to clip by.</summary>
		public ScreenRegion ClipRegion;
		/// <summary>A stack of transformations to apply to elements. Updated during a layout event.</summary>
		public TransformationStack Transformations=new TransformationStack();
		
		/// <summary>Transform at the top of the stack.</summary>
		public Transformation Transform{
			get{
				
				// Get the transform at the top of the stack:
				return Transformations.Last;
				
			}
		}
		
		/// <summary>Only pops if the given element had a transform to add earlier.</summary>
		public void PopTransform(SVGElement by){
			
			// Get CS:
			Css.ComputedStyle computed=by.Style.Computed;
			Transformation transform=computed.TransformX;
			
			if(transform!=null){
				// Pop it off again:
				Transformations.Pop();
			}
			
		}
		
		/// <summary>Always pops something.</summary>
		public void PopTransform(){
			Transformations.Pop();
		}
		
		public void PushTransform(SVGElement by){
			
			// Get CS:
			Css.ComputedStyle computed=by.Style.Computed;
			
			Transformation transform=computed.TransformX;
			
			// Push the transform to our stack, if we have one.
			if(transform!=null){
				
				// Add it to the stack:
				Transformations.Push(transform);
				
				// Update it:
				transform.RecalculateMatrix(computed,computed.FirstBox);
				
			}
			
		}
		
		/// <summary>The last drawn width.</summary>
		public int Width{
			get{
				return DrawInfo.ImageX;
			}
		}
		
		/// <summary>The last drawn height.</summary>
		public int Height{
			get{
				return DrawInfo.ImageY;
			}
		}
		
		/// <summary>The root SVG element.</summary>
		public SVGSVGElement Drawing;
		/// <summary>The Loonim draw info.</summary>
		private Loonim.DrawInfo DrawInfo;
		/// <summary>The Loonim filter.</summary>
		public Loonim.SurfaceTexture Filter;
		/// <summary>True if the filter needs to be rebuilt on the next reflow.</summary>
		private bool FilterInvalid=true;
		
		
		public RenderContext(SVGSVGElement drawing){
			Drawing=drawing;
			
			// Create the tex too:
			Filter=new Loonim.SurfaceTexture();
			
			// Create the draw surface:
			DrawInfo=new Loonim.DrawInfo();
			
		}
		
		/// <summary>Builds the SVG as a Loonim filter.</summary>
		internal void BuildFilter(){
			
			// Clear DS:
			DrawStack=null;
			
			// Build up the filter:
			Drawing.BuildFilter(this);
			
			// Set DS as root:
			Filter.Root=DrawStack;
			
			if(DrawStack!=null){
				
				// PreAllocate it:
				Filter.PreAllocate(DrawInfo);
				
			}
			
		}
		
		internal void Draw(){
			
			// Update viewport (if SVG doc):
			SVGDocument svgDoc=(Drawing.document as SVGDocument);
			
			if(svgDoc!=null){
				svgDoc.Viewport.Update(Width,Height);
			}
			
			// Draw now!
			Filter.Draw(DrawInfo);
			
		}
		
		/// <summary>Requests a rebuild of the filter.</summary>
		public void RequestRebuild(){
			FilterInvalid=true;
		}
		
		/// <summary>Change the size of this context. This can potentially trigger a redraw.</summary>
		public void SetSize(int width,int height){
			
			if(FilterInvalid){
				// Build the filter now:
				FilterInvalid=false;
				BuildFilter();
			}
			
			// Resize (internally checks if a resize is actually required, and also may create the texture):
			if(DrawInfo.SetSize(width,height)){
				
				// Redraw:
				Draw();
				
			}
			
		}
		
		/// <summary>The current Loonim stack.</summary>
		private Loonim.Stack DrawStack;
		
		public void FillPath(Loonim.TextureNode fill,VectorPath transformedPath,bool evenOddFillMethod){
			
			// Create Loonim node:
			Loonim.CustomMesh path=new Loonim.CustomMesh(fill,transformedPath);
			
			// Add to draw stack:
			if(DrawStack==null){
				DrawStack=new Loonim.Stack(1);
				DrawStack.Sources[0]=path;
			}else{
			
				// Add a path to the draw stack:
				DrawStack.Add(path);
			
			}
			
		}
		
		/// <summary>Helps build stroke meshes.</summary>
		private Loonim.StrokePathMesh StrokeHelper=null;
		
		/// <summary>Computes a stroke path now and adds it.</summary>
		public void StrokePath(Loonim.TextureNode stroke,VectorPath transformedPath,float width,SVGElement settings){
			
			if(StrokeHelper==null){
				StrokeHelper=new Loonim.StrokePathMesh();
			}
			
			// Get other settings:
			StrokeHelper.LineCapMode=settings.StrokeLineCap;
			// Css.Value dashArray=settings.StrokeDashArray;
			StrokeHelper.LineJoinMode=settings.StrokeLineJoin;
			StrokeHelper.MiterLimit=settings.StrokeMiterLimit;
			StrokeHelper.Accuracy=Loonim.CustomMesh.DefaultAccuracy;
			StrokeHelper.Width=width;
			
			// Generate now!
			List<Mesh> meshes=StrokeHelper.GenerateMeshes(transformedPath);
			
			foreach(Mesh mesh in meshes){
				
				// Create Loonim node for each mesh:
				Loonim.CustomMesh path=new Loonim.CustomMesh(stroke,mesh);
				
				// Add to draw stack:
				if(DrawStack==null){
					DrawStack=new Loonim.Stack(1);
					DrawStack.Sources[0]=path;
				}else{
				
					// Add a path to the draw stack:
					DrawStack.Add(path);
				
				}
				
			}
			
		}
		
		public void AddViewBoxTransform(BoxRegion region,AspectRatio aspectRatio, SVGSVGElement frag){
			
			// Get the tags computed style:
			Css.ComputedStyle tagStyle=(frag==null) ? null : frag.Style.Computed;
			
			float x = (tagStyle == null ? 0 : tagStyle.OffsetLeft);
			float y = (tagStyle == null ? 0 : tagStyle.OffsetTop);

			if(region.IsEmpty){
				PushMatrix(TranslateMatrix(x,y));
				return;
			}

			float width = (tagStyle == null ? region.Width : tagStyle.PixelWidth);
			float height = (tagStyle == null ? region.Height : tagStyle.PixelHeight);
			
			float fScaleX = width / region.Width;
			float fScaleY = height / region.Height; //(this.MinY < 0 ? -1 : 1) * 
			float fMinX = -region.X * fScaleX;
			float fMinY = -region.Y * fScaleY;

			if(aspectRatio == null){
				aspectRatio = new AspectRatio(SVGPreserveAspectRatio.xMidYMid, false);
			}
			
			if (aspectRatio.Align != SVGPreserveAspectRatio.none){
				
				if(aspectRatio.Slice){
					
					fScaleX = (float)Math.Max(fScaleX, fScaleY);
					fScaleY = (float)Math.Max(fScaleX, fScaleY);
					
				}else{
					
					fScaleX = (float)Math.Min(fScaleX, fScaleY);
					fScaleY = (float)Math.Min(fScaleX, fScaleY);
				}
				
				float fViewMidX = (region.Width / 2) * fScaleX;
				float fViewMidY = (region.Height / 2) * fScaleY;
				float fMidX = width / 2;
				float fMidY = height / 2;
				fMinX = -region.X * fScaleX;
				fMinY = -region.Y * fScaleY;

				switch (aspectRatio.Align)
				{
					case SVGPreserveAspectRatio.xMinYMin:
						break;
					case SVGPreserveAspectRatio.xMidYMin:
						fMinX += fMidX - fViewMidX;
						break;
					case SVGPreserveAspectRatio.xMaxYMin:
						fMinX += width - region.Width * fScaleX;
						break;
					case SVGPreserveAspectRatio.xMinYMid:
						fMinY += fMidY - fViewMidY;
						break;
					case SVGPreserveAspectRatio.xMidYMid:
						fMinX += fMidX - fViewMidX;
						fMinY += fMidY - fViewMidY;
						break;
					case SVGPreserveAspectRatio.xMaxYMid:
						fMinX += width - region.Width * fScaleX;
						fMinY += fMidY - fViewMidY;
						break;
					case SVGPreserveAspectRatio.xMinYMax:
						fMinY += height - region.Height * fScaleY;
						break;
					case SVGPreserveAspectRatio.xMidYMax:
						fMinX += fMidX - fViewMidX;
						fMinY += height - region.Height * fScaleY;
						break;
					case SVGPreserveAspectRatio.xMaxYMax:
						fMinX += width - region.Width * fScaleX;
						fMinY += height - region.Height * fScaleY;
						break;
					default:
						break;
				}
			}
			
			// Clip now:
			SetClip(new BoxRegion(x, y, width, height),false);
			
			Matrix4x4 matrix=ScaleMatrix(fScaleX, fScaleY);
			matrix*=TranslateMatrix(x, y);
			matrix*=TranslateMatrix(fMinX, fMinY);
			
			// Push it:
			PushMatrix(matrix);
			
		}
		
		public void PushMatrix(Matrix4x4 matrix){
			
			Transformations.Push(matrix);
			
		}
		
		/// <summary>Builds a scale matrix.</summary>
		public Matrix4x4 ScaleMatrix(float x,float y){
			
			return Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(x,y,1f));
			
		}
		
		/// <summary>Builds a translate matrix.</summary>
		public Matrix4x4 TranslateMatrix(float x,float y){
			
			return Matrix4x4.TRS(new Vector3(x,y,0f),Quaternion.identity,Vector3.one);
			
		}
		
		public void SetClip(ScreenRegion region,bool reset){
			
			if(reset){
				
				ClipRegion=region;
				
			}else if(ClipRegion!=null && region!=null){
				
				// Must combine them both.
				// Note that if ClipRegion is already a set, it can't re-use the same set object.
				// However, we want to avoid an unnecessary tree of regions, so we do look out for sets.
				ScreenRegionGroup existGroup=(ClipRegion as ScreenRegionGroup);
				ScreenRegionGroup newGroup=(region as ScreenRegionGroup);
				
				List<ScreenRegion> resultSet=new List<ScreenRegion>();
				
				if(existGroup==null){
					
					// Straight add:
					resultSet.Add(ClipRegion);
					
				}else{
					
					// Copy over:
					existGroup.CopyInto(resultSet);
					
				}
				
				if(newGroup==null){
					
					// Straight add:
					resultSet.Add(region);
					
				}else{
					
					// Copy over:
					newGroup.CopyInto(resultSet);
					
				}
				
				// Update CR:
				ClipRegion=new ScreenRegionGroup(resultSet);
				
			}else if(region!=null){
				
				// Just apply region.
				ClipRegion=region;
				
			}
			
			// No changes otherwise.
			
		}
		
		/// <summary>The underlying texture.
		/// Remains the same even when SetSize is called; i.e. you only need to grab this once.</summary>
		public Texture Texture{
			get{
				return Filter.Texture;
			}
		}
		
	}
	
}