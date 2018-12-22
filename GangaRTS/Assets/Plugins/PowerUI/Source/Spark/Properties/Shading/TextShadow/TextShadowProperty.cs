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
using System.Collections;
using System.Collections.Generic;
using InfiniText;
using Blaze;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Handles the rendering of text shadows.
	/// </summary>
	
	public partial class TextShadowProperty:DisplayableProperty{
		
		/// <summary>The "host" text property.</summary>
		public TextRenderingProperty Text;
		/// <summary>How blurry the shadow is.</summary>
		public float Blur;
		/// <summary>Horizontal shadow offset.</summary>
		public float HOffset;
		/// <summary>Vertical shadow offset.</summary>
		public float VOffset;
		/// <summary>The shadow colour.</summary>
		public Color Colour;
		/// <summary>Text.FontSize * A blur scale.</summary>
		private float FontSizeScaled;
		/// <summary>The horizontal/vertical text offset factor.</summary>
		private float TextOffsetFactor;
		
		
		/// <summary>Creates a new text rendering property. Note that this must not be called directly 
		/// - it's only ever used by the "content" CSS property.
		/// Set content: instead; if you're doing that from a tag, take a look at BR.</summary>
		/// <param name="data">The renderable object that this is rendering text for.</param>
		public TextShadowProperty(RenderableData data):base(data){}
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				// Before text (400)
				return 380;
			}
		}
		
		internal override bool NowOnScreen(){
			
			// Occurs before text does.
			// Must implicitly nudge text:
			if(Text!=null){
				Text.SetVisibility(true);
			}
			
			return true;
			
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			Color colour=Colour * renderer.ColorOverlay;
			
			MeshBlock block=GetFirstBlock(renderer);
			
			// For each block..
			for(int i=0;i<BlockCount;i++){
				
				// Paint the colour:
				block.PaintColour(colour);
				
				// Go to next block:
				block.Next();
				
			}
			
		}
		
		/// <summary>Draws an underline (or a strikethrough).</summary>
		public virtual void DrawUnderline(Renderman renderer){
			
			// Ensure we have a batch:
			renderer.SetupBatch(this,null,null);
			
			// And get our block ready:
			MeshBlock block=Add(renderer);
			
			// Set the UV to that of the solid block colour pixel:
			block.SetSolidColourUV();
			
			// Set the colour:
			block.SetColour(renderer.FontColour);
			
			// Set the verts:
			block.SetClipped(renderer.ClippingBoundary,renderer.CurrentRegion,renderer,renderer.TextDepth);
			
			// Ok!
			block.Done(renderer.Transform);
			
		}
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			if(Text.Characters==null || Text.Characters.Length==0){
				
				// Not ready yet or nothing here.
				return;
				
			}
			
			// Get indices:
			int startIndex=box.TextStart;
			int maxIndex=box.TextEnd;
			
			if(startIndex>=maxIndex){
				// No text selected.
				return;
			}
			
			// Get the font colour:
			Color fontColour=Colour * renderer.ColorOverlay;
			
			// The blocks we allocate here come from FontToDraw.
			// They use the same renderer and same layout service, but just a different mesh.
			// This is to enable potentially very large font atlases with multiple fonts.
			
			float top=box.Y + box.StyleOffsetTop + Text.LineHeightOffset + VOffset;
			float left=box.X + box.StyleOffsetLeft+HOffset;
			
			float fontSize=Text.FontSize;
			
			// Note that this property "drags" to following elements which is correct.
			// We don't really want to break batching chains for aliasing.
			
			if(Text.Alias==float.MaxValue){
				// Yep!
				
				// Get quick ref to constants set:
				float[] hints=Fonts.AutoAliasHints;
				
				if(fontSize<=hints[0]){
					renderer.FontAliasingBottom=hints[1];
					renderer.FontAliasingTop=hints[2];
				}else if(fontSize>=hints[12]){
					renderer.FontAliasingBottom=hints[13];
					renderer.FontAliasingTop=hints[14];
				}else{
					
					float minBottom;
					float minTop;
					float deltaSize;
					float deltaBottom;
					float deltaTop;
					float relative;
					
					// Note: Just about everything here is constant. Inlined for speed.
					
					// Interpolate:
					if(fontSize<=hints[3]){
						// Between 3 and 0.
						minBottom=hints[1];
						minTop=hints[2];
						deltaSize=hints[3]-hints[0];
						deltaBottom=hints[4]-hints[1];
						deltaTop=hints[5]-hints[2];
						relative=(fontSize-hints[0])/deltaSize;
						
					}else if(fontSize<=hints[6]){
						// Between 6 and 3.
						minBottom=hints[4];
						minTop=hints[5];
						deltaSize=hints[6]-hints[3];
						deltaBottom=hints[7]-hints[4];
						deltaTop=hints[8]-hints[5];
						relative=(fontSize-hints[3])/deltaSize;
						
					}else if(fontSize<=hints[9]){
						// Between 9 and 6.
						minBottom=hints[7];
						minTop=hints[8];
						deltaSize=hints[9]-hints[6];
						deltaBottom=hints[10]-hints[7];
						deltaTop=hints[11]-hints[8];
						relative=(fontSize-hints[6])/deltaSize;
						
					}else{
						
						// Must be between 12 and 9.
						minBottom=hints[10];
						minTop=hints[11];
						deltaSize=hints[12]-hints[9];
						deltaBottom=hints[13]-hints[10];
						deltaTop=hints[14]-hints[11];
						relative=(fontSize-hints[9])/deltaSize;
						
					}
					
					// Note: Most of the above is constant. Inlined for speed.
					
					renderer.FontAliasingBottom=(relative * deltaBottom)+minBottom;
					renderer.FontAliasingTop=(relative * deltaTop)+minTop;
					
				}
				
			}else{
				
				// Write aliasing:
				renderer.FontAliasingTop=InfiniText.Fonts.OutlineLocation+Text.Alias;
				renderer.FontAliasingBottom=InfiniText.Fonts.OutlineLocation-Text.Alias;
				
			}
			
			FontSizeScaled=fontSize;
			
			if(Blur!=0f){
				
				// Scale the letters:
				// (Used to display chars with bigger spreads)
				FontSizeScaled*=1f+(Blur/fontSize);
				
				// Top tends to 1:
				renderer.FontAliasingTop=1f;
				
				// Bottom tends to 0:
				renderer.FontAliasingBottom=0f;
				
			}
			
			if(!Text.AllEmpty){
				// Firstly, make sure the batch is using the right font texture.
				// This may generate a new batch if the font doesn't match the previous or existing font.
				
				// Get the full shape of the element:
				float width=box.PaddedWidth;
				float height=box.PaddedHeight;
				float minY=box.Y+box.Border.Top;
				float minX=box.X+box.Border.Left;
				
				BoxRegion boundary=new BoxRegion(minX,minY,width,height);
				
				if(!boundary.Overlaps(renderer.ClippingBoundary)){
					
					if(Visible){
						SetVisibility(false);
					}
					
					return;
				}else if(!Visible){
					
					// ImageLocation will allocate here if it's needed.
					SetVisibility(true);
					
				}
				
			}
			
			float zIndex=RenderData.computedStyle.ZIndex;
			BoxRegion screenRegion=new BoxRegion();
			
			// Update renderer with various text properties:
			renderer.CurrentRegion=screenRegion;
			renderer.CurrentBox=box;
			renderer.TopOffset=top;
			renderer.TextScaleFactor=FontSizeScaled/Fonts.Rasteriser.ScalarX;
			renderer.TextAscender=(Text.FontToDraw.Ascender * Text.FontSize);
			
			// Offset factors (used to center the shadow on the letter):
			TextOffsetFactor=(FontSizeScaled - Text.FontSize) / (2f * Fonts.Rasteriser.ScalarX);
			
			
			// First up, underline.
			if(Text.TextLine!=null){
				// We have one. Locate it next.
				float lineWeight=(Text.FontToDraw.StrikeSize * FontSizeScaled);
				float yOffset=0f;
				
				switch(Text.TextLine.Type){
				
					case TextDecorationLineMode.Underline:
						yOffset=renderer.TextAscender + lineWeight;
					break;
					case TextDecorationLineMode.LineThrough:
						yOffset=(Text.FontToDraw.StrikeOffset * FontSizeScaled);
						yOffset=renderer.TextAscender - yOffset;
					break;
					case TextDecorationLineMode.Overline:
						yOffset=(lineWeight * 2f);
					break;
				}
				
				// Note: The integer rounding is required here to prevent underlines from alternating between
				// a thick line and a thin one as you scroll around.
				screenRegion.Set(left,(int)(top+yOffset),box.Width,lineWeight);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					
					// This region is visible. Clip it:
					screenRegion.ClipBy(renderer.ClippingBoundary);
					
					renderer.FontColour=fontColour;
					renderer.TextDepth=zIndex;
					
					// Draw the underline now!
					DrawUnderline(renderer);
					
				}
				
			}
			
			// Update font colour:
			renderer.FontColour=fontColour;
			
			// Next, render the characters.
			// If we're rendering from right to left, flip the punctuation over.
			
			// Right to left (e.g. arabic):
			switch(box.UnicodeBidi){
				case UnicodeBidiMode.LeftwardsNormal:
					
					// Render the text from the last char backwards.
					
					for(int i=maxIndex-1;i>=startIndex;i--){
						
						renderer.CharacterIndex=i;
						DrawCharacter(ref left,renderer);
					}
				
				break;
				case UnicodeBidiMode.LeftwardsMirrored:
					
					// Render the characters with horizontally inverted uv's.
					
					for(int i=maxIndex-1;i>=startIndex;i--){
						renderer.CharacterIndex=i;
						DrawInvertCharacter(ref left,renderer);
					}
				
				break;
				case UnicodeBidiMode.RightwardsMirrored:
					
					// Render the characters with horizontally inverted uv's.
					
					for(int i=startIndex;i<maxIndex;i++){
						renderer.CharacterIndex=i;
						DrawInvertCharacter(ref left,renderer);
					}
					
				break;
				default:
					
					// Draw it as is.
					
					for(int i=startIndex;i<maxIndex;i++){
						renderer.CharacterIndex=i;
						DrawCharacter(ref left,renderer);
					}
					
				break;
			}
			
		}
		
		/// <summary>Draws a character with x-inverted UV's. Used for rendering e.g. "1 < 2" in right-to-left.</summary>
		protected virtual void DrawInvertCharacter(ref float left,Renderman renderer){
			
			BoxRegion screenRegion=renderer.CurrentRegion;
			float top=renderer.TopOffset;
			int index=renderer.CharacterIndex;
			
			Glyph character=Text.Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Text.Kerning!=null){
				left+=Text.Kerning[index] * Text.FontSize;
			}
			
			// Get atlas location (if it has one):
			AtlasLocation locatedAt=character.Location;
			
			if(locatedAt!=null){
				
				// We're on the atlas!
				
				float y=top+renderer.TextAscender-((character.Height+character.MinY) * Text.FontSize);
				float charLeft=left + (character.LeftSideBearing * FontSizeScaled);
				float scaleFactor=renderer.TextScaleFactor;
				
				y-=locatedAt.Height * TextOffsetFactor;
				charLeft-=locatedAt.Width * TextOffsetFactor;
				
				screenRegion.Set(charLeft,y,locatedAt.Width * scaleFactor,locatedAt.Height * scaleFactor);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					// True if this character is visible.
					
					// Ensure correct batch:
					renderer.SetupBatch(this,null,locatedAt.Atlas);
					
					MeshBlock block=Add(renderer);
					block.SetColour(renderer.FontColour);
					block.ApplyOutline();
					
					// And clip our meshblock to fit within boundary:
					block.ImageUV=null;
					UVBlock uvs=block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,RenderData.computedStyle.ZIndex,locatedAt,block.TextUV);
					
					if(uvs.Shared){
						uvs=new UVBlock(uvs);
					}
					
					// Invert along X:
					float temp=uvs.MinX;
					uvs.MinX=uvs.MaxX;
					uvs.MaxX=temp;
					
					// Assign to the block:
					block.TextUV=uvs;
					
					block.Done(renderer.Transform);
				}
				
			}
			
			left+=(character.AdvanceWidth * Text.FontSize)+Text.LetterSpacing;
			
			if(character.Charcode==(int)' '){
				left+=Text.WordSpacing;
			}
			
		}
		
		/// <summary>Draws the given Emoji character.</summary>
		public void DrawEmoji(Glyph character,ref float left,Renderman renderer){
			
			if(!character.Image.Loaded){
				return;
			}
			
			BoxRegion screenRegion=renderer.CurrentRegion;
			float top=renderer.TopOffset;
			
			// It's an image (e.g. Emoji).
			AtlasLocation locatedAt=RequireImage(character.Image);
			
			if(locatedAt==null){
				// It needs to be isolated. Big emoji image!
				return;
			}
			
			if(CharacterProviders.FixHeight){
				// Set the region:
				screenRegion.Set(left,top,locatedAt.Width,locatedAt.Height);
			}else{
				screenRegion.Set(left,top,FontSizeScaled,FontSizeScaled);
			}
			
			if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					
				// Ensure correct batch:
				renderer.SetupBatch(this,locatedAt.Atlas,null);
				
				// If the two overlap, this means it's actually visible.
				MeshBlock block=Add(renderer);
				
				// Set it's colour:
				block.SetColour(renderer.ColorOverlay);
				
				// And clip our meshblock to fit within boundary:
				block.TextUV=null;
				
				block.ImageUV=block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,RenderData.computedStyle.ZIndex,locatedAt,block.ImageUV);
				
				block.Done(renderer.Transform);
				
			}
			
			left+=(character.AdvanceWidth)+Text.LetterSpacing;
			
			if(character.Charcode==(int)' '){
				left+=Text.WordSpacing;
			}
			
		}
		
		/// <summary>Draws a character and advances the pen onwards.</summary>
		protected virtual void DrawCharacter(ref float left,Renderman renderer){
			
			BoxRegion screenRegion=renderer.CurrentRegion;
			Color fontColour=renderer.FontColour;
			float top=renderer.TopOffset;
			int index=renderer.CharacterIndex;
			
			Glyph character=Text.Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Text.Kerning!=null){
				left+=Text.Kerning[index] * Text.FontSize;
			}
			
			AtlasLocation locatedAt;
			
			if(character.Image!=null){
				DrawEmoji(character,ref left,renderer);
				return;
			}
			
			// Get atlas location:
			locatedAt=character.Location;
			
			// Does this character have a visual glyph? E.g. a space does not.
			if(locatedAt!=null){
				
				float y=top+renderer.TextAscender-((character.Height+character.MinY) * Text.FontSize);
				float charLeft=left + (character.LeftSideBearing * FontSizeScaled);
				float scaleFactor=renderer.TextScaleFactor;
				
				y-=locatedAt.Height * TextOffsetFactor;
				charLeft-=locatedAt.Width * TextOffsetFactor;
				
				screenRegion.Set(charLeft,y,locatedAt.Width * scaleFactor,locatedAt.Height * scaleFactor);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					// True if this character is visible.
					
					// Ensure correct batch:
					renderer.SetupBatch(this,null,locatedAt.Atlas);
					
					MeshBlock block=Add(renderer);
					block.SetColour(fontColour);
					block.ApplyOutline();
					
					// And clip our meshblock to fit within boundary:
					block.ImageUV=null;
					block.TextUV=block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,RenderData.computedStyle.ZIndex,locatedAt,block.TextUV);
					
					block.Done(renderer.Transform);
				}
				
			}
			
			left+=(character.AdvanceWidth * Text.FontSize)+Text.LetterSpacing;
			
			if(character.Charcode==(int)' '){
				left+=Text.WordSpacing;
			}
			
		}
		
	}
	
}