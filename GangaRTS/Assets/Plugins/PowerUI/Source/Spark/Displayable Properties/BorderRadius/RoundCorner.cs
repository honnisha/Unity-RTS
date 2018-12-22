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
	/// Represents the a rounded corner of a border.
	/// This works by creating two sections - the "inverse" border and the border itself.
	/// The inverse border is essentially a series of transparent whose purpose is just to affect the depth buffer.
	/// </summary>
	
	public class RoundCorner{
		
		/// <summary>The amount of pixels per section of the corner. The lower this is, the smoother the corner (but the lower the performance).
		/// Below this, micro resolution is used instead.</summary>
		public static int Resolution=4;
		/// <summary>The corner resolution if a corner radius is equal to resolution or lower.</summary>
		public static int MicroResolution=1;
		
		/// <summary>Cached value scale.</summary>
		internal float ValueScale;
		/// <summary>The edge index that this corner goes to.</summary>
		public int ToIndex;
		/// <summary>The edge index that this corner goes from.</summary>
		public int FromIndex;
		/// <summary>The colour of this corner.</summary>
		public Color Colour;
		/// <summary>The radius of this corner.</summary>
		public float CornerRadius;
		/// <summary>How far the outer arc is from the corner of the original box at each block. Note that this is always round; One axis is CornerRadius-theOtherAxis.</summary>
		private Vector2[] OuterArc;
		/// <summary>How far the inner arc is from the corner of the original box at each block.</summary>
		internal Vector2[] InnerArc;
		/// <summary>The number of blocks (fragments of the curve; translate directly to MeshBlocks) that this corner is constructed of.</summary>
		private int BlocksRequired;
		/// <summary>The border this corner belongs to.</summary>
		public BorderProperty Border;
		/// <summary>The number of inverse blocks required.</summary>
		public int InverseBlocksRequired;
		/// <summary>The parent round corners set.</summary>
		public RoundedCorners RoundCorners;
		/// <summary>The position of this corner (e.g. top left).</summary>
		public RoundCornerPosition Position;
		/// <summary>The inverse property which inverse fragments are rendered to.</summary>
		public RoundBorderInverseProperty InverseBorder;
		
		
		/// <summary>Creates a new rounded corner in the given position.</summary>
		public RoundCorner(RoundedCorners roundCorners,RoundCornerPosition position){
			Position=position;
			RoundCorners=roundCorners;
			Border=roundCorners.Border;
			InverseBorder=roundCorners.InverseBorder;
			
			// Get the to index:
			ToIndex=(int)position;
			
			// Get the from index:
			FromIndex=ToIndex-1;
			
			// May need to wrap it:
			if(FromIndex==-1){
				FromIndex=3;
			}
			
		}
		
		/// <summary>The radius of this corner.</summary>
		public float Radius{
			get{
				return CornerRadius;
			}
			set{
				
				// Has it changed?
				if(CornerRadius==value){
					return;
				}
				
				CornerRadius=value;
				
				int resolution=Resolution;
				
				if(value<=resolution){
					
					// Use the micro resolution instead:
					resolution=MicroResolution;
					
				}
				
				// How many blocks does this corner require? Rounded up.
				BlocksRequired=(int)Math.Ceiling(value/resolution);
				
				// How many inverse blocks?
				InverseBlocksRequired=((BlocksRequired-1)/2)+1;
				
				// Clear both arcs:
				OuterArc=null;
				InnerArc=null;
				
			}
		}
		
		/// <summary>Recomputes both arcs simultaneously.</summary>
		private void RecomputeArcs(BoxStyle widths){
			
			// get the required arc size:
			int arcSizeRequired=BlocksRequired+1;
			
			// Create the outer arc:
			if(OuterArc==null || OuterArc.Length!=arcSizeRequired){
			
				// It's required:
				OuterArc=new Vector2[arcSizeRequired];
				
			}
			
			// How big is the outer arc?
			int size=OuterArc.Length;
			
			if(InnerArc==null || InnerArc.Length!=size){
				// Create the set now:
				InnerArc=new Vector2[size];
			}
			
			// We're next going to be doing some polars here. Everything is clockwise.
			// We're essentially going to rotate around the virtual center of a circle, changing our radius as we go.
			
			// The current angle in radians:
			float currentAngle;
			
			// The virtual center of the circle, relative to the corner:
			float centerX;
			float centerY;
			
			// The source border width:
			float sourceWidth;
			// The target border width:
			float targetWidth;
			
			// Get radius as a float:
			float cornerRadius=CornerRadius;
			
			// The starting radius:
			float radius=cornerRadius;
			
			switch(Position){
				case RoundCornerPosition.TopLeft:
					
					// The center is up on Y and up on X:
					centerX=radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI;
					
					// Get the widths:
					sourceWidth=widths.Left;
					targetWidth=widths.Top;
					
				break;
				case RoundCornerPosition.TopRight:
					
					// The center is up on Y and down on X:
					centerX=-radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*1.5f;
					
					// Get the widths:
					sourceWidth=widths.Top;
					targetWidth=widths.Right;
					
				break;
				case RoundCornerPosition.BottomRight:
					
					// The center is down on Y and down on X:
					centerX=-radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=0f;
					
					// Get the widths:
					sourceWidth=widths.Right;
					targetWidth=widths.Bottom;
					
				break;
				default:
				case RoundCornerPosition.BottomLeft:
					
					// The center is down on Y and up on X:
					centerX=radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*0.5f;
					
					// Get the widths:
					sourceWidth=widths.Bottom;
					targetWidth=widths.Left;
					
				break;
			}
			
			// Remove source width from radius:
			radius-=sourceWidth;
			
			// What's the maximum number of iterations?
			float maximumValue=(float)(size-1);
			
			// Figure out delta angle (we'll be travelling through PI/2 degrees):
			float deltaAngle=(Mathf.PI*0.5f)/maximumValue;
			
			// Figure out delta radius based on source/target:
			float deltaRadius=(sourceWidth-targetWidth)/maximumValue;
			
			// Next, for each point..
			for(int i=0;i<size;i++){
				
				// Get the cos/sin of the current angle:
				float cosAngle=Mathf.Cos(currentAngle);
				float sinAngle=Mathf.Sin(currentAngle);
				
				// Figure out the outer arc value:
				OuterArc[i]=new Vector2(centerX+(cosAngle*cornerRadius),centerY+(sinAngle*cornerRadius));
				
				// And also the inner arc value:
				InnerArc[i]=new Vector2(centerX+(cosAngle*radius),centerY+(sinAngle*radius));
				
				// Move the angle along:
				currentAngle+=deltaAngle;
				
				// Move the radius along:
				radius+=deltaRadius;
				
			}
			
		}
		
		/// <summary>Recomputes the inner arc. The inner arc is special because it depends on border width.
		/// The two borders that this corner connects may be different widths, so it may have to transition from one thickness to another.</summary>
		private void RecomputeInnerArc(BoxStyle widths){
			
			// How big is the outer arc?
			int size=OuterArc.Length;
			
			if(InnerArc==null || InnerArc.Length!=size){
				// Create the set now:
				InnerArc=new Vector2[size];
			}
			
			// We're next going to be doing some polars here. Everything is clockwise.
			// We're essentially going to rotate around the virtual center of a circle, changing our radius as we go.
			
			// The current angle in radians:
			float currentAngle;
			
			// The virtual center of the circle, relative to the corner:
			float centerX;
			float centerY;
			
			// The source border width:
			float sourceWidth;
			// The target border width:
			float targetWidth;
			
			// Get radius as a float:
			float cornerRadius=CornerRadius;
			
			// The starting radius:
			float radius=cornerRadius;
			
			switch(Position){
				case RoundCornerPosition.TopLeft:
					
					// The center is up on Y and up on X:
					centerX=radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI;
					
					// Get the widths:
					sourceWidth=widths.Left;
					targetWidth=widths.Top;
					
				break;
				case RoundCornerPosition.TopRight:
					
					// The center is up on Y and down on X:
					centerX=-radius;
					centerY=radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*1.5f;
					
					// Get the widths:
					sourceWidth=widths.Top;
					targetWidth=widths.Right;
					
				break;
				case RoundCornerPosition.BottomRight:
					
					// The center is down on Y and down on X:
					centerX=-radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=0f;
					
					// Get the widths:
					sourceWidth=widths.Right;
					targetWidth=widths.Bottom;
					
				break;
				default:
				case RoundCornerPosition.BottomLeft:
					
					// The center is down on Y and up on X:
					centerX=radius;
					centerY=-radius;
					
					// The angle starts at..
					currentAngle=Mathf.PI*0.5f;
					
					// Get the widths:
					sourceWidth=widths.Bottom;
					targetWidth=widths.Left;
					
				break;
			}
			
			// Remove source width from radius:
			radius-=sourceWidth;
			
			// What's the maximum number of iterations?
			float maximumValue=(float)(size-1);
			
			// Figure out delta angle (we'll be travelling through PI/2 degrees):
			float deltaAngle=(Mathf.PI*0.5f)/maximumValue;
			
			// Figure out delta radius based on source/target:
			float deltaRadius=(sourceWidth-targetWidth)/maximumValue;
			
			// Next, for each point..
			for(int i=0;i<size;i++){
				
				// Get the cos/sin of the current angle:
				float cosAngle=Mathf.Cos(currentAngle);
				float sinAngle=Mathf.Sin(currentAngle);
				
				// And also the inner arc value:
				InnerArc[i]=new Vector2(centerX+(cosAngle*radius),centerY+(sinAngle*radius));
				
				// Move the angle along:
				currentAngle+=deltaAngle;
				
				// Move the radius along:
				radius+=deltaRadius;
				
			}
			
		}
		
		/// <summary>Renders the inverse of this corner for the border.</summary>
		public void RenderInverse(LayoutBox box,Renderman renderer,float cornerX,float cornerY){
			
			float scale=RoundCorners.Computed.RenderData.ValueScale;
			
			if(scale!=ValueScale){
				// Value scale has changed - reset the radius:
				Radius = Radius * scale / ValueScale;
				ValueScale = scale;
			}
			
			if(OuterArc==null){
				RecomputeArcs(box.Border);
			}else if(InnerArc==null){
				RecomputeInnerArc(box.Border);
			}
			
			// Get the z-Index:
			float zIndex=RoundCorners.Computed.MaxZIndex+0.004f;
			
			// Grab the size of the outer arc array:
			int arcSize=OuterArc.Length;
			
			int currentIndex=0;
			
			// Resolve the corner:
			Vector3 corner=renderer.PixelToWorldUnit(cornerX,cornerY,zIndex);
			
			// Ensure a batch is available:
			renderer.SetupBatch(InverseBorder,null,null);
			
			// For each inverse block:
			for(int i=0;i<InverseBlocksRequired;i++){
				
				// Get a block:
				MeshBlock block=InverseBorder.Add(renderer);
				
				// Set the clear colour:
				block.SetColour(Color.clear);
				
				// Always going to be space to sample two. Sample the first:
				Vector2 outerPoint=InnerArc[currentIndex];
				
				// Apply the triangle:
				block.VertexTopRight=corner;
				
				// Apply the first:
				block.VertexTopLeft=renderer.PixelToWorldUnit(cornerX+outerPoint.x,cornerY+outerPoint.y,zIndex);
				
				// Sample the second:
				outerPoint=InnerArc[currentIndex+1];
				
				// Apply the second:
				block.VertexBottomLeft=renderer.PixelToWorldUnit(cornerX+outerPoint.x,cornerY+outerPoint.y,zIndex);
				
				if((currentIndex+2)>=arcSize){
					// Match the previous vertex:
					block.VertexBottomRight=block.VertexBottomLeft;
				}else{
					// Grab the next point along:
					outerPoint=InnerArc[currentIndex+2];
					
					// Resolve and apply the third:
					block.VertexBottomRight=renderer.PixelToWorldUnit(cornerX+outerPoint.x,cornerY+outerPoint.y,zIndex);
				}
				
				block.Done(renderer.Transform);
				
				// Move index along:
				currentIndex+=2;
				
			}
			
		}
		
		public void Render(LayoutBox box,Renderman renderer,float cornerX,float cornerY){
			
			if(OuterArc==null){
				RecomputeArcs(box.Border);
			}else if(InnerArc==null){
				RecomputeInnerArc(box.Border);
			}
			
			// Get the z-Index:
			float zIndex=RoundCorners.Computed.MaxZIndex+0.006f;
			
			// Figure out where half way is (divide by 2):
			int halfway=(BlocksRequired>>1);
			
			Color colour=Colour;
			
			// Grab the clipping boundary:
			BoxRegion clip=renderer.ClippingBoundary;
			
			// Make it relative to the corners location:
			float minClipX=clip.X-cornerX;
			float minClipY=clip.Y-cornerY;
			float maxClipX=clip.MaxX-cornerX;
			float maxClipY=clip.MaxY-cornerY;
			
			// For each block..
			for(int i=0;i<BlocksRequired;i++){
				
				// Read the outer arc:
				Vector2 outerPointA=OuterArc[i];
				
				// Figure out the bounding box (constant for a particular block).
				float minX=outerPointA.x;
				float maxX=minX;
				float minY=outerPointA.y;
				float maxY=minY;
				
				Vector2 outerPointB=OuterArc[i+1];
				
				// Update the bounding box:
				if(outerPointB.x<minX){
					minX=outerPointB.x;
				}else if(outerPointB.x>maxX){
					maxX=outerPointB.x;
				}
				
				if(outerPointB.y<minY){
					minY=outerPointB.y;
				}else if(outerPointB.y>maxY){
					maxY=outerPointB.y;
				}
				
				// Line segment A->B on the "outer" arc.
				
				// Read the inner arc:
				Vector2 innerPointA=InnerArc[i];
				
				// Update the bounding box:
				if(innerPointA.x<minX){
					minX=innerPointA.x;
				}else if(innerPointA.x>maxX){
					maxX=innerPointA.x;
				}
				
				if(innerPointA.y<minY){
					minY=innerPointA.y;
				}else if(innerPointA.y>maxY){
					maxY=innerPointA.y;
				}
				
				Vector2 innerPointB=InnerArc[i+1];
				
				// Update the bounding box:
				if(innerPointB.x<minX){
					minX=innerPointB.x;
				}else if(innerPointB.x>maxX){
					maxX=innerPointB.x;
				}
				
				if(innerPointB.y<minY){
					minY=innerPointB.y;
				}else if(innerPointB.y>maxY){
					maxY=innerPointB.y;
				}
				
				// How does our bounding box compare to the clipping region?
				if(maxX<minClipX){
					continue;
				}else if(minX>maxClipX){
					continue;
				}
				
				if(maxY<minClipY){
					continue;
				}else if(minY>maxClipY){
					continue;
				}
				
				// Line segment A->B on the "inner" arc.
				
				// Get a block:
				MeshBlock block=Border.Add(renderer);
				
				// Set the UV to that of the solid block colour pixel:
				block.SetSolidColourUV();
				
				// Get the border colour:
				if(i==halfway){
					// Get the next colour:
					
					if(Border.BaseColour!=null && Border.BaseColour.Count!=1){
						colour=Border.BaseColour[ToIndex].GetColour(Border.RenderData,Css.Properties.BorderColor.GlobalProperty) * renderer.ColorOverlay;
					}
				}
				
				// Set the border colour:
				block.SetColour(colour);
				
				// Apply the block region:
				block.VertexTopLeft=renderer.PixelToWorldUnit(cornerX+outerPointA.x,cornerY+outerPointA.y,zIndex);
				block.VertexTopRight=renderer.PixelToWorldUnit(cornerX+outerPointB.x,cornerY+outerPointB.y,zIndex); 
				
				block.VertexBottomLeft=renderer.PixelToWorldUnit(cornerX+innerPointA.x,cornerY+innerPointA.y,zIndex);
				block.VertexBottomRight=renderer.PixelToWorldUnit(cornerX+innerPointB.x,cornerY+innerPointB.y,zIndex);
				
				block.Done(renderer.Transform);
				
			}
			
			
		}
		
	}
	
}