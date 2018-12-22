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
	/// Represents the border around an element.
	/// </summary>
	
	public class BorderProperty:DisplayableProperty{
		
		/// <summary>The colour of the border without the colour overlay. Black if undefined.</summary>
		public Css.Value BaseColour=null;
		/// <summary>The set of round corners if any.</summary>
		public RoundedCorners Corners=null;
		
		
		/// <summary>Creates a new border property for the given element.</summary>
		/// <param name="data">The renderable object to give a border to.</param>
		public BorderProperty(RenderableData data):base(data){}
		
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				return 100;
			}
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			// Any meshes in my queue should now change colour:
			MeshBlock block=GetFirstBlock(renderer);
			
			if( BaseColour==null || BaseColour.Count==1 ){
				
				// Most common case. This is a single colour border.
				
				// Get the default colour - that's the same as the text colour:
				Color colour=Color.black;
				
				// Does this border have a colour?
				if(BaseColour==null){
					
					// Grab the text colour if there is one:
					if(RenderData.Text!=null){
						
						// It's the same as the font colour:
						colour=RenderData.Text.BaseColour * renderer.ColorOverlay;
						
					}else{
					
						// Nope - We need to set alpha:
						colour.a=renderer.ColorOverlay.a;
						
					}
					
				}else{
					colour=BaseColour[0].GetColour(RenderData,Css.Properties.BorderColor.GlobalProperty) * renderer.ColorOverlay;
				}
				
				// For each block..
				for(int i=0;i<BlockCount;i++){
					
					// Paint the colour:
					block.PaintColour(colour);
					
					// Go to next block:
					block.Next();
					
				}
				
				return;
			}
			
		}
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			if(Corners!=null){
				Corners.PreLayout();
			}
			
			ComputedStyle computed=RenderData.computedStyle;
			
			// Find the zIndex:
			// NB: At same depth as BGColour - right at the back.
			float zIndex=(computed.ZIndex-0.006f);
			
			// Get the co-ord of the top edge:
			float top=box.Y;
			float left=box.X;
			
			// Get the border widths:
			BoxStyle width=box.Border;
			
			// Move top by the widths:
			top+=width.Top;
			left+=width.Left;
			
			// And the dimensions of the lines:
			float boxWidth=box.PaddedWidth;
			float boxHeight=box.PaddedHeight;
			
			// Get the other dimensions:
			float topY=top-width.Top;
			float right=left+boxWidth;
			float rightX=right+width.Right;
			float bottom=top+boxHeight;
			float bottomY=bottom+width.Bottom;
			float leftX=left-width.Left;
			int segment=renderer.Segment;
			
			Transformation transform=renderer.Transform;
			
			BoxRegion screenRegion=new BoxRegion();
			
			// Get the default colour - that's the same as the text colour:
			Color colour=Color.black;
			
			// Is the border multicoloured?
			bool multiColour=false;
			
			// Does this border have a colour?
			if(BaseColour==null){
				
				// Grab the text colour if there is one:
				if(RenderData.Text!=null){
					
					// It's the same as the font colour:
					colour=RenderData.Text.BaseColour * renderer.ColorOverlay;
					
				}else{
				
					// Nope - We need to set alpha:
					colour.a=renderer.ColorOverlay.a;
					
				}
				
			}else if(BaseColour.Count==1){
				
				colour=BaseColour[0].GetColour(RenderData,Css.Properties.BorderColor.GlobalProperty) * renderer.ColorOverlay;
				
			}else{
				multiColour=true;
			}
			
			// Handle border-radius:
			if(Corners!=null){
				
				for(int i=0;i<4;i++){
					
					if(multiColour){
						colour=BaseColour[i].GetColour(RenderData,Css.Properties.BorderColor.GlobalProperty) * renderer.ColorOverlay;
					}
					
					Corners.Layout(colour,box,renderer,i);
				
				}
				
			}
			
			// Get clipper:
			BoxRegion clip=renderer.ClippingBoundary;
			
			float origLeftX=leftX;
			float origTopY=topY;
			float origBottomY=bottomY;
			float origRightX=rightX;
			
			// top and topY:
			if(top<clip.Y){
				top=clip.Y;
			}else if(top>clip.MaxY){
				top=clip.MaxY;
			}
			
			if(topY<clip.Y){
				topY=clip.Y;
			}
			
			// bottom and bottomY:
			if(bottom>clip.MaxY){
				bottom=clip.MaxY;
			}else if(bottom<clip.Y){
				bottom=clip.Y;
			}
			
			if(bottomY>clip.MaxY){
				bottomY=clip.MaxY;
			}
			
			// right and rightX:
			if(right<clip.X){
				right=clip.X;
			}else if(right>clip.MaxX){
				right=clip.MaxX;
			}
			
			// rightX vs clip.MaxX
			if(rightX>clip.MaxX){
				rightX=clip.MaxX;
			}
			
			// left and leftX:
			if(left<clip.X){
				left=clip.X;
			}else if(left>clip.MaxX){
				left=clip.MaxX;
			}
			
			if(leftX<clip.X){
				leftX=clip.X;
			}
			
			float cornerPointA;
			float cornerPointB;
			
			for(int i=0;i<4;i++){
				
				// Does this border have multiple colours?
				if(multiColour){
					colour=BaseColour[i].GetColour(RenderData,Css.Properties.BorderColor.GlobalProperty) * renderer.ColorOverlay;
				}
				
				// Add to region:
				switch(i){
					case 0:
						// Top.
						screenRegion.SetPoints(leftX,topY,rightX,top);
					break;
					case 1:
						// Right.
						
						// We only draw the right border if 'segment' includes 'end'
						if((segment & LineBoxSegment.End)==0){
							goto NextLine;
						}
						
						screenRegion.SetPoints(right,topY,rightX,bottomY);
					break;
					case 2:
						// Bottom.
						
						screenRegion.SetPoints(leftX,bottom,rightX,bottomY);
					break;
					case 3:
						// Left.
						
						// Similarly, we only draw left if segment includes 'start':
						if((segment & LineBoxSegment.Start)==0){
							goto NextLine;
						}
						
						screenRegion.SetPoints(leftX,topY,left,bottomY);
					break;
				}
				
				if(screenRegion.Overlaps(clip)){
					
					// It's visible.
					
					// Ensure we have a batch (doesn't change graphics or font textures, thus both null):
					renderer.SetupBatch(this,null,null);
					
					// And get our block ready:
					MeshBlock block=Add(renderer);
					
					// Set the UV to that of the solid block colour pixel:
					block.SetSolidColourUV();
					
					// Set the border colour:
					block.SetColour(colour);
					
					// Apply verts:
					
					switch(i){
						case 0:
							
							// Top:
							if(Corners==null){
								
								block.VertexTopLeft=renderer.PixelToWorldUnit(leftX,topY,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(rightX,topY,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(left,top,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(right,top,zIndex);
								
							}else{
								
								// Top left/right corners:
								cornerPointA=origLeftX+Corners.TopLeftRadius;
								cornerPointB=origRightX-Corners.TopRightRadius;
								
								if(cornerPointA<clip.X){
									cornerPointA=clip.X;
								}
								
								if(cornerPointB>clip.MaxX){
									cornerPointB=clip.MaxX;
								}
								
								// Note that we use leftX/rightX for all of them.
								// That's because the corner has a 'straight' edge.
								block.VertexTopLeft=renderer.PixelToWorldUnit(cornerPointA,topY,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(cornerPointB,topY,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(cornerPointA,top,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(cornerPointB,top,zIndex);
								
							}
							
						break;
						case 1:
							
							// Right:
							if(Corners==null){
								
								block.VertexTopLeft=renderer.PixelToWorldUnit(right,top,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(rightX,topY,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(right,bottom,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(rightX,bottomY,zIndex);
							
							}else{
								
								// Top right/ bottom right corners:
								cornerPointA=origTopY+Corners.TopRightRadius;
								cornerPointB=origBottomY-Corners.BottomRightRadius;
								
								
								if(cornerPointA<clip.Y){
									cornerPointA=clip.Y;
								}
								
								if(cornerPointB>clip.MaxY){
									cornerPointB=clip.MaxY;
								}
								
								// Note that we use topY/bottomY for all of them.
								block.VertexTopLeft=renderer.PixelToWorldUnit(right,cornerPointA,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(rightX,cornerPointA,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(right,cornerPointB,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(rightX,cornerPointB,zIndex);
								
							}
							
						break;
						case 2:
							
							// Bottom:
							if(Corners==null){
								
								block.VertexTopLeft=renderer.PixelToWorldUnit(left,bottom,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(right,bottom,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(leftX,bottomY,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(rightX,bottomY,zIndex);
								
							}else{
								
								// Bottom left/ bottom right corners:
								// Note that we use leftX/rightX for all of them.
								cornerPointA=origLeftX+Corners.BottomLeftRadius;
								cornerPointB=origRightX-Corners.BottomRightRadius;
								
								if(cornerPointA<clip.X){
									cornerPointA=clip.X;
								}
								
								if(cornerPointB>clip.MaxX){
									cornerPointB=clip.MaxX;
								}
								
								block.VertexTopLeft=renderer.PixelToWorldUnit(cornerPointA,bottom,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(cornerPointB,bottom,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(cornerPointA,bottomY,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(cornerPointB,bottomY,zIndex);
								
							}
							
						break;
						case 3:
							
							// Left:
							if(Corners==null){
								
								block.VertexTopLeft=renderer.PixelToWorldUnit(leftX,topY,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(left,top,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(leftX,bottomY,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(left,bottom,zIndex);
								
							}else{
								
								// Top right/ bottom right corners:
								cornerPointA=origTopY+Corners.TopLeftRadius;
								cornerPointB=origBottomY+width.Bottom-Corners.BottomLeftRadius;
								
								if(cornerPointA<clip.Y){
									cornerPointA=clip.Y;
								}
								
								if(cornerPointB>clip.MaxY){
									cornerPointB=clip.MaxY;
								}
								
								block.VertexTopLeft=renderer.PixelToWorldUnit(leftX,cornerPointA,zIndex);
								block.VertexTopRight=renderer.PixelToWorldUnit(left,cornerPointA,zIndex);
								block.VertexBottomLeft=renderer.PixelToWorldUnit(leftX,cornerPointB,zIndex);
								block.VertexBottomRight=renderer.PixelToWorldUnit(left,cornerPointB,zIndex);
								
							}
							
						break;
					}
					
					// Done!
					block.Done(transform);
					
				}
				
				NextLine:
					continue;
				
			}
			
		}
		
		public override bool Render(bool first,LayoutBox box,Renderman renderer){
			
			if(first){
				ClearBlocks();
			}
			
			Layout(box,renderer);
			
			// Return true if we've got border-radius corners - that'll trigger a PostProcess event:
			return (Corners!=null);
			
		}
		
		/// <summary>Transforms all the blocks that this property has allocated. Note that transformations are a post process.
		/// Special case for borders as it may also internally transform its corners.</summary>
		/// <param name="topTransform">The transform that should be applied to this property.</param>
		public override void PostProcess(LayoutBox box,Renderman renderer){
			
			if(Corners==null){
				return;
			}
			
			// Render them:
			Corners.RenderCorners(box,renderer);
			
		}
		
		public void SetCorner(RoundCornerPosition position,float radius){
			
			if(Corners==null){
				
				if(radius<=0){
					return;
				}
			
				// Create the corner set:
				Corners=new RoundedCorners(this);
				
			}
			
			// Set the corner:
			switch(position){
				case RoundCornerPosition.TopLeft:
					// Top left corner:
					Corners.SetCorner(ref Corners.TopLeft,position,radius);
				break;
				case RoundCornerPosition.TopRight:
					// Top right corner:
					Corners.SetCorner(ref Corners.TopRight,position,radius);
				break;
				case RoundCornerPosition.BottomRight:
					// Bottom right corner:
					Corners.SetCorner(ref Corners.BottomRight,position,radius);
				break;
				case RoundCornerPosition.BottomLeft:
					// Bottom left corner:
					Corners.SetCorner(ref Corners.BottomLeft,position,radius);
				break;
			}
			
		}
		
	}
	
}