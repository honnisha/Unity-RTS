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
using UnityEngine;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Represents the solid background colour of an element.
	/// </summary>
	
	public partial class BackgroundColour:DisplayableProperty{
		
		/// <summary>The colour the background should be with no colour overlay.</summary>
		public Color BaseColour;
		
		
		/// <summary>Creates a new solid background colour property for the given element.</summary>
		/// <param name="data">The renderable object to give a bg colour to.</param>
		public BackgroundColour(RenderableData data):base(data){}
		
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				return 200;
			}
		}
		
		/// <summary>True if this paints across the whole background of the element.</summary>
		public override bool IsBackground{
			get{
				return true;
			}
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			MeshBlock block=GetFirstBlock(renderer);
			
			if(block==null){
				// This can happen if an animation is requesting that a now offscreen element gets painted only.
				return;
			}
			
			block.PaintColour(BaseColour * renderer.ColorOverlay);
			
		}
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			float width;
			float height;
			float top;
			float left;
			bool clip=true;
			
			if(renderer.ViewportBackground){
				
				// Applying to whole background:
				BoxRegion viewport=renderer.Viewport;
				
				top=viewport.Y;
				left=viewport.X;
				width=viewport.Width;
				height=viewport.Height;
				
				renderer.ViewportBackground=false;
				clip=false;
				
			}else{
				
				// Get the top left inner corner (inside margin and border):
				width=box.PaddedWidth;
				height=box.PaddedHeight;
				top=box.Y+box.Border.Top;
				left=box.X+box.Border.Left;
				
				// Is it clipped?
				if(renderer.IsInvisible(left,top,width,height)){
					// Totally not visible.
					return;
				}
				
			}
			
			// Ensure we have a batch (doesn't change graphics or font thus both nulls):
			renderer.SetupBatch(this,null,null);
			
			// Allocate the block:
			MeshBlock block=Add(renderer);
			
			// Using firstblock as our block here.
			// Set the UV to that of the solid block colour pixel:
			block.SetSolidColourUV();
			// Set the (overlay) colour:
			block.SetColour(BaseColour * renderer.ColorOverlay);
			
			// And finally sort out the verts:
			if(clip){
				block.SetClipped(renderer.ClippingBoundary,new BoxRegion(left,top,width,height),renderer,RenderData.computedStyle.ZIndex-0.006f);
			}else{
				block.ApplyVertices(new BoxRegion(left,top,width,height),renderer,RenderData.computedStyle.ZIndex-0.006f);
			}
			
			// Flush it:
			block.Done(renderer.Transform);
			
		}
		
	}
	
}