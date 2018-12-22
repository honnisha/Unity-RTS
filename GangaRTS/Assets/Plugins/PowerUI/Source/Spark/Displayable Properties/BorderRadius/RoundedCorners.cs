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

namespace Css{
	
	/// <summary>
	/// Represents the set of round corners for a particular border.
	/// </summary>
	
	public class RoundedCorners{
		
		/// <summary>A rounded corner in the top left, if any.</summary>
		public RoundCorner TopLeft;
		/// <summary>A rounded corner in the top right, if any.</summary>
		public RoundCorner TopRight;
		/// <summary>The parent border this set belongs to.</summary>
		public BorderProperty Border;
		/// <summary>A rounded corner in the bottom left, if any.</summary>
		public RoundCorner BottomLeft;
		/// <summary>The computed style of the parent element.</summary>
		public ComputedStyle Computed;
		/// <summary>A rounded corner in the bottom right, if any.</summary>
		public RoundCorner BottomRight;
		/// <summary>The inverse fragment renderer for this set.</summary>
		public RoundBorderInverseProperty InverseBorder;
		
		public RoundedCorners(BorderProperty border){
			Border=border;
			
			// Grab the computed style:
			Computed=border.RenderData.computedStyle;
			
			// Create the inverse border set:
			InverseBorder=new RoundBorderInverseProperty(border.RenderData);
		}
		
		/// <summary>Sets a round corner to this border.</summary>
		/// <param name="corner">The property that it's going onto or coming from.</param>
		/// <param name="position">The type of corner that it is.</param>
		/// <param name="radius">The border radius.</param>
		public void SetCorner(ref RoundCorner corner,RoundCornerPosition position,float radius){
			
			if(radius<=0){
				
				if(corner!=null){
					// Clear it:
					corner=null;
					
					// Got rounded corners now?
					bool hasRoundedCorners=(
						TopLeft!=null || TopRight!=null ||
						BottomLeft!=null || BottomRight!=null
					);
					
					if(!hasRoundedCorners){
						// Clear the corner set:
						Border.Corners=null;
					}
					
				}
				
				return;
			}
			
			// A corner is now required:
			
			if(corner==null){
				
				// Create it now:
				corner=new RoundCorner(this,position);
				
			}
			
			// Apply the radius:
			corner.Radius=radius;
			corner.ValueScale=Computed.RenderData.ValueScale;
			
		}
		
		/// <summary>Called right before round corners are about to be layed out.</summary>
		public void PreLayout(){
			
			// Clear the blocks of the inverse border:
			InverseBorder.ClearBlocks();
			
		}
		
		/// <summary>Renders and performs a layout of the round corners.</summary>
		public void Layout(Color colour,LayoutBox box,Renderman renderer,int i){
			
			// Get the co-ord of the top edge:
			float top=box.Y;
			float left=box.X;
			
			// Get border widths:
			BoxStyle border=box.Border;
			
			// And the dimensions of the lines:
			// Note: boxwidth doesn't include the left/right widths to prevent overlapping.
			float boxWidth=box.PaddedWidth;
			float boxHeight=box.PaddedHeight+border.Top+border.Bottom;
			
			switch(i){
				case 0:
					// Top:
					
					// Move over by the top-left corner:
					if(TopLeft!=null){
						
						TopLeft.Colour=colour;
						
						// Render the top left corner:
						TopLeft.RenderInverse(box,renderer,left,top);
						
					}
					
				break;
				case 1:
					// Right:
					
					if(TopRight!=null){
						
						TopRight.Colour=colour;
						
						// Render the top right corners inverse now:
						TopRight.RenderInverse(box,renderer,left+boxWidth+border.Left+border.Right,top);
						
					}
					
				break;
				case 2:
					// Bottom:
					
					if(BottomRight!=null){
						
						BottomRight.Colour=colour;
						
						// Render the bottom right corners inverse now:
						BottomRight.RenderInverse(box,renderer,left+boxWidth+border.Left+border.Right,top+boxHeight);
						
					}
					
				break;
				case 3:
					// Left:
					
					if(BottomLeft!=null){
						
						BottomLeft.Colour=colour;
						
						// Render the bottom left corners inverse now:
						BottomLeft.RenderInverse(box,renderer,left,top+boxHeight);
						
					}
					
				break;
			}
			
		}
		
		public void ClearCorners(){
			
			if(TopLeft!=null){
				
				// Got a top left corner - recompute it's inner arc:
				TopLeft.InnerArc=null;
				
			}
			
			if(TopRight!=null){
				
				// Got a top left corner - render it now:
				TopRight.InnerArc=null;
				
			}
			
			if(BottomRight!=null){
				
				// Got a top left corner - render it now:
				BottomRight.InnerArc=null;
				
			}
			
			if(BottomLeft!=null){
				
				// Got a top left corner - render it now:
				BottomLeft.InnerArc=null;
				
			}
			
		}
		
		/// <summary>Renders round corners.</summary>
		public void RenderCorners(LayoutBox box,Renderman renderer){
			
			// Get the co-ord of the top edge:
			float top=box.Y;
			float left=box.X;
			
			// And the dimensions of the lines:
			float boxWidth=box.PaddedWidth+box.Border.Left+box.Border.Right;
			float boxHeight=box.PaddedHeight+box.Border.Top+box.Border.Bottom;
			
			if(TopLeft!=null){
				
				// Got a top left corner - render it now:
				TopLeft.Render(box,renderer,left,top);
				
			}
			
			if(TopRight!=null){
				
				// Got a top left corner - render it now:
				TopRight.Render(box,renderer,left+boxWidth,top);
				
			}
			
			if(BottomRight!=null){
				
				// Got a top left corner - render it now:
				BottomRight.Render(box,renderer,left+boxWidth,top+boxHeight);
				
			}
			
			if(BottomLeft!=null){
				
				// Got a top left corner - render it now:
				BottomLeft.Render(box,renderer,left,top+boxHeight);
				
			}
		}
		
		/// <summary>Top left radius.</summary>
		public float TopLeftRadius{
			get{
				if(TopLeft==null){
					return 0f;
				}
				
				return TopLeft.Radius;
			}
		}
		
		/// <summary>Top right radius.</summary>
		public float TopRightRadius{
			get{
				if(TopRight==null){
					return 0f;
				}
				
				return TopRight.Radius;
			}
		}
		
		/// <summary>Bottom right radius.</summary>
		public float BottomRightRadius{
			get{
				if(BottomRight==null){
					return 0f;
				}
				
				return BottomRight.Radius;
			}
		}
		
		/// <summary>Bottom left radius.</summary>
		public float BottomLeftRadius{
			get{
				if(BottomLeft==null){
					return 0f;
				}
				
				return BottomLeft.Radius;
			}
		}
		
	}
	
}