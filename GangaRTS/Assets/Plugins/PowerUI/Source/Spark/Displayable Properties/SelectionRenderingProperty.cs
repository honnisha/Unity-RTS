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
	/// Represents the solid background colour of an element when it is selected.
	/// </summary>
	
	public partial class SelectionRenderingProperty:DisplayableProperty{
		
		/// <summary>The "host" text rendering property.</summary>
		public TextRenderingProperty Text;
		/// <summary>The colour the selection should be with no colour overlay.</summary>
		public Color BaseColour=new Color32(51,153,255,255);
		/// <summary>Selection zone width.</summary>
		public float Width;
		/// <summary>The end index.</summary>
		public int EndIndex;
		/// <summary>The start index.</summary>
		public int StartIndex;
		
		
		/// <summary>Creates a new solid background colour property for the given element.</summary>
		/// <param name="data">The renderable object to give selection info to.</param>
		public SelectionRenderingProperty(RenderableData data):base(data){}
		
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				// Between shadow and stroke:
				return 385;
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
			
			// Does the boxes text region overlap our selected range?
			if(box.TextEnd<=StartIndex || EndIndex<=box.TextStart){
				return;
			}
			
			// It overlaps! We need to figure out which section we're selecting (and how large it is).
			
			// Get the top left inner corner (inside margin and border):
			// Assume we've selected the whole thing; we remove from the start and remove from the end if needed.
			float width=box.PaddedWidth;
			float height=box.PaddedHeight;
			float top=box.Y+box.Border.Top;
			float left=box.X+box.Border.Left;
			float fs=Text.FontSize;
			
			// If the selected indices totally contain the box..
			if(StartIndex>box.TextStart){
				
				// Trim the start. We need to chop off the width of these characters.
				
				float characterWidth=0f;
				
				int max=StartIndex;
				
				if(max>Text.Characters.Length){
					max=Text.Characters.Length;
				}
				
				for(int i=box.TextStart;i<max;i++){
					
					InfiniText.Glyph character=Text.Characters[i];
					
					if(character==null){
						continue;
					}
					
					// Advance over the glyph:
					if(Text.Kerning!=null){
						characterWidth+=Text.Kerning[i] * fs;
					}
					
					characterWidth+=(character.AdvanceWidth * fs)+Text.LetterSpacing;
					
					if(character.Charcode==(int)' '){
						characterWidth+=Text.WordSpacing;
					}
					
				}
				
				// Chop it off:
				left+=characterWidth;
				width-=characterWidth;
				
			}
			
			if(EndIndex<box.TextEnd){
				
				// Trim the end. We need to chop off the width of these characters.
				
				float characterWidth=0f;
				
				int max=box.TextEnd;
				
				if(max>Text.Characters.Length){
					max=Text.Characters.Length;
				}
				
				for(int i=EndIndex;i<max;i++){
					
					InfiniText.Glyph character=Text.Characters[i];
					
					if(character==null){
						continue;
					}
					
					// Advance over the glyph:
					if(Text.Kerning!=null){
						characterWidth+=Text.Kerning[i] * fs;
					}
					
					characterWidth+=(character.AdvanceWidth * fs)+Text.LetterSpacing;
					
					if(character.Charcode==(int)' '){
						characterWidth+=Text.WordSpacing;
					}
					
				}
				
				// Chop it off:
				width-=characterWidth;
				
			}
			
			// Is it clipped?
			if(renderer.IsInvisible(left,top,width,height)){
				// Totally not visible.
				return;
			}
			
			// Ensure we have a batch (doesn't change graphics or font thus both nulls):
			renderer.SetupBatch(this,null,null);
			
			// Allocate the block:
			MeshBlock block=Add(renderer);
			
			// Using firstblock as our block here.
			// Set the UV to that of the solid block colour pixel:
			block.SetSolidColourUV();
			// Set the colour:
			block.SetColour(BaseColour);
			
			// And finally sort out the verts:
			block.SetClipped(renderer.ClippingBoundary,new BoxRegion(left,top,width,height),renderer,RenderData.computedStyle.ZIndex-0.004f);
			
			// Flush it:
			block.Done(renderer.Transform);
			
		}
		
	}
	
}