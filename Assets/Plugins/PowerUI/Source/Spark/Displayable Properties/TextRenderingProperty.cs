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
	/// This class manages rendering text to the screen.
	/// </summary>
	
	public partial class TextRenderingProperty:DisplayableProperty{
		
		/// <summary>The glyph for the newline character.</summary>
		private static Glyph NEWLINE_GLYPH;
		
		/// <summary>The size of the font in pixels.</summary>
		public float FontSize;
		/// <summary>The font-size adjustment. -1 is none.</summary>
		public float FontSizeAdjust=-1f;
		/// <summary>The colour that the font should be without the colour overlay.</summary>
		public Color BaseColour; 
		/// <summary>Should text be automatically aliased? Auto is MaxValue. Set with font-smoothing.</summary>
		public float Alias=float.MaxValue;
		/// <summary>True if all characters are whitespaces. No batches will be generated.</summary>
		public bool AllEmpty;
		/// <summary>Fill for the text.</summary>
		public BackgroundOverlay Background;
		/// <summary>Additional spacing to apply around letters.</summary>
		public float LetterSpacing;
		/// <summary>Additional spacing to apply around words.</summary>
		public float WordSpacing;
		/// <summary>Overflow wrap active.</summary>
		public bool OverflowWrapActive;
		/// <summary>How and where a line should be drawn if at all (e.g. underline, overline etc.)</summary>
		public TextDecorationInfo TextLine;
		/// <summary>The font face to use when rendering.</summary>
		internal FontFace FontToDraw;
		/// <summary>The set of characters to render. Note that the characters are shared globally.</summary>
		public Glyph[] Characters;
		/// <summary>Kern values for this text, if it has any. Created only if it's needed.</summary>
		internal float[] Kerning;
		/// <summary>True if this needs to have its characters loaded.</summary>
		internal bool Dirty=true;
		/// <summary>The line height offset.</summary>
		public float LineHeightOffset;
		
		
		/// <summary>Creates a new text rendering property. Note that this must not be called directly 
		/// - it's only ever used by the "content" CSS property.
		/// Set content: instead; if you're doing that from a tag, take a look at BR.</summary>
		/// <param name="data">The renderable object that this is rendering text for.</param>
		public TextRenderingProperty(RenderableData data):base(data){}
		
		/// <summary>Applies this TRP to the given computed style.
		/// Only ever used by the "content" CSS property.</summary>
		public void Setup(ComputedStyle style){
		
			// Update text:
			RenderData.Text=this;
			
			// Next, apply every textual style immediately.
			// Note: properties with multiple names are applied multiple times.
			// This is fine. It's expected to be such a rare case and it will work fine anyway.
			foreach(KeyValuePair<string,CssProperty> kvp in CssProperties.AllText){
				SetValue(kvp.Value,style);
			}
			
		}
		
		/// <summary>Sets the named css property from the given style if the property exists in the style.</summary>
		/// <param name="property">The css property, e.g. color.</param>
		/// <param name="style">The style to load value of the property from. This should be the computed style for the parent element.</param>
		private void SetValue(CssProperty property,ComputedStyle style){
			// Get the current value:
			Value value=style[property];
			
			if(value!=null){
				// Apply it:
				property.ApplyText(this,RenderData,style,value);
			}
		}
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				return 400;
			}
		}
		
		/// <summary>Does this word not end with a whitespace?</summary>
		public bool NoEndingSpace{
			get{
				
				if(Characters==null || Characters.Length==0){
					return true;
				}
				
				return !Characters[Characters.Length-1].Space;
				
			}
		}
		
		/// <summary>The width in pixels of the last whitespace of this element, if it's got one.</summary>
		public int EndSpaceSize{
			get{
				
				if(Characters==null || Characters.Length==0){
					return 0;
				}
				
				Glyph character=Characters[Characters.Length-1];
				
				if(character!=null && character.Space){
					// It ends in a space! Return the width of a space as measured by this renderer.
					return (int)(character.AdvanceWidth * FontSize);
				}
				
				return 0;
			}
		}
		
		/// <summary>Gets how many letters are being rendered.</summary>
		/// <returns>The number of letters.</returns>
		public int LetterCount(){
			return Characters.Length;
		}
		
		internal override void NowOffScreen(){
			
			if(Characters==null){
				return;
			}
			
			foreach(Glyph character in Characters){
				
				if(character==null){
					continue;
				}
				
				character.OffScreen();
			}
			
		}
		
		internal override bool NowOnScreen(){
			
			if(Characters==null){
				return false;
			}
			
			foreach(Glyph character in Characters){
				
				if(character==null){
					continue;
				}
				
				character.OnScreen();
			}
			
			return true;
			
		}
		
		/// <summary>Called when an @font-face font fully loads.</summary>
		public void FontLoaded(DynamicFont font){
			
			if(FontToDraw==null || Characters==null || FontToDraw.Family==null){
				return;
			}
			
			Kerning=null;
			Dirty=true;
			
		}
		
		/// <summary>Clears the computed dimensions so they'll get recalculated during the layout pass.</summary>
		public void ClearDimensions(){
			
			// Get CS:
			ComputedStyle computed=RenderData.computedStyle;
			
			// Remove:
			computed.Properties.Remove(Css.Properties.Width.GlobalProperty);
			computed.Properties.Remove(Css.Properties.Height.GlobalProperty);
			
		}
		
		public void ClearText(){
			Dirty=true;
		}
		
		/// <summary>Loads the character array (<see cref="Css.TextRenderingProperty.Characters"/>) from the given text string.</summary>
		internal void LoadCharacters(string text,RenderableData renderable){
			
			if(Characters!=null && Visible){
				
				// Make sure each char goes offscreen:
				NowOffScreen();
				
				// Clear visible:
				Visible=false;
				
			}
			
			// Get the computed style:
			ComputedStyle computedStyle=renderable.computedStyle;
			
			// No longer dirty:
			Dirty=false;
			
			char[] characters=text.ToCharArray();
			
			// Get text transform:
			int textTransform=computedStyle.ResolveInt(Css.Properties.TextTransform.GlobalProperty);
			
			if(textTransform!=TextTransformMode.None && characters.Length>0){
				
				switch(textTransform){
					
					case TextTransformMode.Capitalize:
						
						// Uppercase the first character:
						characters[0]=char.ToUpper(characters[0]);
						
					break;
					
					case TextTransformMode.Lowercase:
						
						// Lowercase the whole string:
						for(int i=0;i<characters.Length;i++){
							
							characters[i]=char.ToLower(characters[i]);
						
						}
						
					break;
					
					case TextTransformMode.Uppercase:
						
						// Uppercase the whole string:
						for(int i=0;i<characters.Length;i++){
							
							characters[i]=char.ToUpper(characters[i]);
						
						}
						
					break;
					
				}
				
			}
			
			// Get the whitespace mode:
			int whiteSpaceMode=computedStyle.WhiteSpaceX;
			
			// Purge characters that we don't want:
			if((whiteSpaceMode & WhiteSpaceMode.NormalOrNoWrap)!=0){
				
				// Dump breaks and repeated whitespace.
				bool dumpWhiteSpace=false;
				
				for(int i=0;i<characters.Length;i++){
					
					char character=characters[i];
					
					if(character=='\t'){
						// Dump:
						characters[i]='\0';
						
					}else if(character=='\r' || character=='\n'){
						// Dump:
						characters[i]='\0';
						
						// Dump whitespaces immediately after these:
						dumpWhiteSpace=true;
						continue;
						
					}else if(character=='\u00A0'){
						
						// NBSP:
						characters[i]=' ';
						
					}else if(character==' '){
						
						if(dumpWhiteSpace){
							// Dump:
							characters[i]='\0';
						}else{
							// Dump any consecutive whitespace (except for &nbsp;):
							dumpWhiteSpace=true;
						}
						
						continue;
					}
					
					dumpWhiteSpace=false;
					
				}
				
			}else if(whiteSpaceMode==WhiteSpaceMode.PreLine){
				
				// Dump repeated whitespace (and \r) only.
				bool dumpWhiteSpace=false;
				
				for(int i=0;i<characters.Length;i++){
					
					char character=characters[i];
					
					if(character==' '){
						
						if(dumpWhiteSpace){
							// Dump:
							characters[i]='\0';
						}else{
							dumpWhiteSpace=true;
						}
						
						continue;
						
					}else if(character=='\u00A0'){
						
						// NBSP:
						characters[i]=' ';
						
					}else if(character=='\r'){
						characters[i]='\0';
						dumpWhiteSpace=true;
						continue;
					}else if(character=='\n'){
						dumpWhiteSpace=true;
						continue;
					}
					
					dumpWhiteSpace=false;
					
				}
				
			}else{
				
				// \r and nbsp only.
				
				for(int i=0;i<characters.Length;i++){
					
					char character=characters[i];
					
					if(character=='\r'){
						characters[i]='\0';
					}else if(character=='\u00A0'){
						// NBSP:
						characters[i]=' ';
					}
					
				}
				
			}
			
			// Create characters if they're needed:
			if(Characters==null || Characters.Length!=characters.Length){
				Characters=new Glyph[characters.Length];
				Kerning=null;
			}
			
			// Considered all empty until shown otherwise.
			AllEmpty=true;
			
			// Next, for each character, find its dynamic character.
			// At the same time we want to find out what dimensions this word has so it can be located correctly.
			Glyph previous=null;
			
			for(int i=0;i<characters.Length;i++){
				char rawChar=characters[i];
				
				if(rawChar=='\0'){
					// It got dumped.
					continue;
				}
				
				Glyph character=null;
				
				// Is it a unicode high/low surrogate pair?
				if(char.IsHighSurrogate(rawChar) && i!=characters.Length-1){
					// Low surrogate follows:
					char lowChar=characters[i+1];
					
					// Get the full charcode:
					int charcode=char.ConvertToUtf32(rawChar,lowChar);
					
					// Grab the surrogate pair char:
					character=FontToDraw.GetGlyphOrEmoji(charcode);
					
					// Make sure there is no char in the low surrogate spot:
					Characters[i+1]=null;
					// Update this character:
					Characters[i]=character;
					// Skip the low surrogate:
					i++;
				}else if(rawChar=='\n'){
					
					// Special case for newlines (They don't show up in host fonts).
					if(NEWLINE_GLYPH==null){
						NEWLINE_GLYPH=new Glyph();
						NEWLINE_GLYPH.RawCharcode=(int)'\n';
					}
					
					Characters[i]=NEWLINE_GLYPH;
					
				}else{
					character=FontToDraw.GetGlyphOrEmoji((int)rawChar);
					Characters[i]=character;
				}
				
				
				if(character==null){
					continue;
				}
				
				if(previous!=null){
					
					// Look for a kern pair:
					if(character.Kerning!=null){
						
						float offset;
						
						if(character.Kerning.TryGetValue(previous,out offset)){
							// Got a kern!
							if(Kerning==null){
								Kerning=new float[characters.Length];
							}
							
							Kerning[i]=offset;
						}
						
					}
					
				}
				
				previous=character;
				AllEmpty=false;
				
			}
			
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			// Resolve colour:
			BaseColour=RenderData.computedStyle.Resolve(Css.Properties.ColorProperty.GlobalProperty)
					.GetColour(RenderData,Css.Properties.ColorProperty.GlobalProperty);
			
			Color colour=BaseColour * renderer.ColorOverlay;
			
			MeshBlock block=GetFirstBlock(renderer);
			
			// For each block..
			for(int i=0;i<BlockCount;i++){
				
				// Paint the colour:
				block.PaintColour(colour);
				
				// Go to next block:
				block.Next();
				
			}
			
		}
		
		/// <summary>Gets the letter at the given local position in pixels.</summary>
		/// <param name="widthOffset">The position in pixels from the left of this element.</param>
		/// <returns>The number of the letter at this position.</returns>
		public int LetterIndex(float widthOffset){
			if(widthOffset<=0f||Characters==null){
				return 0;
			}
			
			if(widthOffset>=RenderData.InnerWidth){
				return Characters.Length;
			}
			
			for(int i=0;i<Characters.Length;i++){
				Glyph character=Characters[i];
				
				if(character==null){
					continue;
				}
				
				widthOffset-=(character.AdvanceWidth * FontSize)+LetterSpacing;
				
				if(character.Charcode==(int)' '){
					widthOffset-=WordSpacing;
				}
				
				if(widthOffset<=0f){
					return i;
				}
			}
			return Characters.Length;
		}
		
		/// <summary>Gets the horizontal position in pixels of the numbered letter.</summary>
		/// <param name="letterID">The letter to get.</param>
		/// <returns>The position of the left edge of the numbered letter in pixels, relative to the left edge of the UI.</returns>
		public float PositionOf(int letterID){
			LayoutBox box=RenderData.FirstBox;
			
			if(box==null){
				// Not run yet
				return 0f;
			}
			
			float width=box.X + box.StyleOffsetLeft;
			return width+LocalPositionOf(letterID);
		}
		
		/// <summary>Gets the horizontal position in pixels of the numbered letter relative to this element.</summary>
		/// <param name="letterID">The letter to get.</param>
		/// <returns>The position of the left edge of the numbered letter in pixels, relative to the left of this element.</returns>
		public float LocalPositionOf(int letterID){
			
			if(Characters==null){
				return 0f;
			}
			
			if(letterID>Characters.Length){
				letterID=Characters.Length;
			}
			
			float result=0f;
			
			for(int i=0;i<letterID;i++){
				Glyph character=Characters[i];
				
				if(character==null){
					continue;
				}
				
				if(character.Charcode==(int)' '){
					result+=WordSpacing;
				}
				
				result+=(character.AdvanceWidth * FontSize)+LetterSpacing;
			}
			
			return result;
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
			
			if(Characters==null || Characters.Length==0){
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
			
			// If we've got a background, set it up now:
			if(Background!=null){
				Background.Layout(box,renderer);
			}
			
			// Get the font colour:
			Color fontColour=BaseColour * renderer.ColorOverlay;
			
			// The blocks we allocate here come from FontToDraw.
			// They use the same renderer and same layout service, but just a different mesh.
			// This is to enable potentially very large font atlases with multiple fonts.
			
			float top=box.Y + box.StyleOffsetTop + LineHeightOffset;
			float left=box.X + box.StyleOffsetLeft;
			
			// Should we auto-alias the text?
			
			// Note that this property "drags" to following elements which is correct.
			// We don't really want to break batching chains for aliasing.
			
			if(Alias==float.MaxValue){
				// Yep!
				
				// Get quick ref to constants set:
				float[] hints=Fonts.AutoAliasHints;
				
				if(FontSize<=hints[0]){
					renderer.FontAliasingBottom=hints[1];
					renderer.FontAliasingTop=hints[2];
				}else if(FontSize>=hints[12]){
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
					if(FontSize<=hints[3]){
						// Between 3 and 0.
						minBottom=hints[1];
						minTop=hints[2];
						deltaSize=hints[3]-hints[0];
						deltaBottom=hints[4]-hints[1];
						deltaTop=hints[5]-hints[2];
						relative=(FontSize-hints[0])/deltaSize;
						
					}else if(FontSize<=hints[6]){
						// Between 6 and 3.
						minBottom=hints[4];
						minTop=hints[5];
						deltaSize=hints[6]-hints[3];
						deltaBottom=hints[7]-hints[4];
						deltaTop=hints[8]-hints[5];
						relative=(FontSize-hints[3])/deltaSize;
						
					}else if(FontSize<=hints[9]){
						// Between 9 and 6.
						minBottom=hints[7];
						minTop=hints[8];
						deltaSize=hints[9]-hints[6];
						deltaBottom=hints[10]-hints[7];
						deltaTop=hints[11]-hints[8];
						relative=(FontSize-hints[6])/deltaSize;
						
					}else{
						
						// Must be between 12 and 9.
						minBottom=hints[10];
						minTop=hints[11];
						deltaSize=hints[12]-hints[9];
						deltaBottom=hints[13]-hints[10];
						deltaTop=hints[14]-hints[11];
						relative=(FontSize-hints[9])/deltaSize;
						
					}
					
					// Note: Most of the above is constant. Inlined for speed.
					
					renderer.FontAliasingBottom=(relative * deltaBottom)+minBottom;
					renderer.FontAliasingTop=(relative * deltaTop)+minTop;
					
				}
				
			}else{
				
				// Write aliasing:
				renderer.FontAliasingTop=InfiniText.Fonts.OutlineLocation+Alias;
				renderer.FontAliasingBottom=InfiniText.Fonts.OutlineLocation-Alias;
				
			}
			
			if(!AllEmpty){
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
			renderer.TextScaleFactor=FontSize/Fonts.Rasteriser.ScalarX;
			renderer.TextAscender=(FontToDraw.Ascender * FontSize);
			
			// First up, underline.
			if(TextLine!=null){
				// We have one. Locate it next.
				float lineWeight=(FontToDraw.StrikeSize * FontSize);
				float yOffset=0f;
				
				switch(TextLine.Type){
				
					case TextDecorationLineMode.Underline:
						yOffset=renderer.TextAscender + (lineWeight * 2f);
					break;
					case TextDecorationLineMode.LineThrough:
						yOffset=(FontToDraw.StrikeOffset * FontSize);
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
					
					if(TextLine.ColourOverride){
						renderer.FontColour=TextLine.BaseColour * renderer.ColorOverlay;
					}else{
						renderer.FontColour=fontColour;
					}
					
					renderer.TextDepth=zIndex;
					
					// Draw the underline now!
					DrawUnderline(renderer);
					
				}
				
			}
			
			// Update font colour:
			renderer.FontColour=fontColour;
			
			// Next, render the characters.
			// If we're rendering from right to left, flip the punctuation over.
			
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
			
			Glyph character=Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Kerning!=null){
				left+=Kerning[index] * FontSize;
			}
			
			// Get atlas location (if it has one):
			AtlasLocation locatedAt=character.Location;
			
			if(locatedAt!=null){
				
				// We're on the atlas!
				
				float y=top+renderer.TextAscender-((character.Height+character.MinY) * FontSize);
			
				float scaleFactor=renderer.TextScaleFactor;
				
				screenRegion.Set(left + (character.LeftSideBearing * FontSize),y,locatedAt.Width * scaleFactor,locatedAt.Height * scaleFactor);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					// True if this character is visible.
					
					// Ensure correct batch:
					renderer.SetupBatch(this,null,locatedAt.Atlas);
				
					MeshBlock block=Add(renderer);
					block.SetColour(renderer.FontColour);
					block.ApplyOutline();
					
					// And clip our meshblock to fit within boundary:
					
					// Clip our meshblock to fit within boundary:
					if(Background!=null && Isolated){
						
						// Setup the batch material for this char:
						Material imageMaterial=Background.Image.Contents.GetImageMaterial(renderer.CurrentShaderSet.Normal);
						SetBatchMaterial(renderer,imageMaterial);
						
						// Reapply text atlas:
						renderer.CurrentBatch.SetFontAtlas(locatedAt.Atlas);
						
						// Apply the image UV's (we're always isolated so these can tile by going out of range):
						block.ImageUV=block.SetClipped(
							renderer.ClippingBoundary,
							screenRegion,
							renderer,
							RenderData.computedStyle.ZIndex,
							Background.ImageLocation,
							block.ImageUV
						);
						
					}else{
						
						block.ImageUV=null;
						
					}
					
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
			
			left+=(character.AdvanceWidth * FontSize)+LetterSpacing;
			
			if(character.Charcode==(int)' '){
				left+=WordSpacing;
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
				screenRegion.Set(left,top,FontSize,FontSize);
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
			
			left+=(character.AdvanceWidth)+LetterSpacing;
			
			if(character.Charcode==(int)' '){
				left+=WordSpacing;
			}
			
		}
		
		/// <summary>Draws a character and advances the pen onwards.</summary>
		protected virtual void DrawCharacter(ref float left,Renderman renderer){
			
			BoxRegion screenRegion=renderer.CurrentRegion;
			Color fontColour=renderer.FontColour;
			float top=renderer.TopOffset;
			int index=renderer.CharacterIndex;
			
			Glyph character=Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Kerning!=null){
				left+=Kerning[index] * FontSize;
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
				
				float y=top+renderer.TextAscender-((character.Height+character.MinY) * FontSize);
				
				float scaleFactor=renderer.TextScaleFactor;
				
				screenRegion.Set(left + (character.LeftSideBearing * FontSize),y,locatedAt.Width * scaleFactor,locatedAt.Height * scaleFactor);
				
				if(screenRegion.Overlaps(renderer.ClippingBoundary)){
					// True if this character is visible.
					
					// Ensure correct batch:
					renderer.SetupBatch(this,null,locatedAt.Atlas);
					
					MeshBlock block=Add(renderer);
					block.SetColour(fontColour);
					block.ApplyOutline();
					
					// Clip our meshblock to fit within boundary:
					if(Background!=null && Isolated){
						
						// Setup the batch material for this char:
						Material imageMaterial=Background.Image.Contents.GetImageMaterial(renderer.CurrentShaderSet.Normal);
						SetBatchMaterial(renderer,imageMaterial);
						
						// Reapply text atlas:
						renderer.CurrentBatch.SetFontAtlas(locatedAt.Atlas);
						
						// Apply the image UV's (we're always isolated so these can tile by going out of range):
						block.ImageUV=block.SetClipped(
							renderer.ClippingBoundary,
							screenRegion,
							renderer,
							RenderData.computedStyle.ZIndex,
							Background.ImageLocation,
							block.ImageUV
						);
						
					}else{
						
						block.ImageUV=null;
						
					}
					
					block.TextUV=block.SetClipped(renderer.ClippingBoundary,screenRegion,renderer,RenderData.computedStyle.ZIndex,locatedAt,block.TextUV);
					
					block.Done(renderer.Transform);
				}
				
			}
			
			left+=(character.AdvanceWidth * FontSize)+LetterSpacing;
			
			if(character.Charcode==(int)' '){
				left+=WordSpacing;
			}
			
		}
		
	}
	
}