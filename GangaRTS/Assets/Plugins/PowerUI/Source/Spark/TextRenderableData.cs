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
using UnityEngine;
using PowerUI;


namespace Css{
	
	public class TextRenderableData : RenderableData{
		
		public TextRenderableData(Node node):base(node){}
		
		/// <summary>Runs before reflow.</summary>
		public override void UpdateCss(Renderman renderer){
			
			// Clear the blocks:
			FirstBox=null;
			LastBox=null;
			
			// Get the text renderer (or create it):
			Css.TextRenderingProperty text=RequireTextProperty();
			
			// Get computed style:
			ComputedStyle cs=computedStyle;
			
			// Get the first box as it contains the fontface/ size:
			LayoutBox box=cs.FirstBox;
			
			// Colour too:
			Color fontColour=cs.Resolve(Css.Properties.ColorProperty.GlobalProperty).GetColour(this,Css.Properties.ColorProperty.GlobalProperty);
			
			// Colour:
			text.BaseColour=fontColour;
			
			// Font size update:
			float fontSize=box.FontSize;
			text.FontSize=fontSize;
			
			// Spacing:
			float wordSpacing=cs.ResolveDecimal(Css.Properties.WordSpacing.GlobalProperty);
			float letterSpacing=cs.ResolveDecimal(Css.Properties.LetterSpacing.GlobalProperty);
			
			// If word spacing is not 'normal', remove 1em from it (Note that letter spacing is always additive):
			if(wordSpacing==-1f){
				wordSpacing=0f;
			}else{
				wordSpacing-=fontSize;
			}
			
			text.WordSpacing=wordSpacing;
			text.LetterSpacing=letterSpacing;
			
			// Decoration:
			int decoration=cs.ResolveInt(Css.Properties.TextDecorationLine.GlobalProperty);
			
			if(decoration==0){
				
				// Remove a line if we have one:
				text.TextLine=null;
				
			}else{
				
				// Got a line!
				if(text.TextLine==null){
					text.TextLine=new TextDecorationInfo(decoration);
				}
				
				// Get the colour:
				Css.Value lineColour=cs.Resolve(Css.Properties.TextDecorationColor.GlobalProperty);
				
				if(lineColour==null || lineColour.IsType(typeof(Css.Keywords.CurrentColor))){
					
					// No override:
					text.TextLine.ColourOverride=false;
					
				}else{
					
					// Set the colour:
					text.TextLine.SetColour(lineColour.GetColour(this,Css.Properties.TextDecorationColor.GlobalProperty));
					
				}
			
			}
			
			// Get the font face:
			text.FontToDraw=box.FontFace;
			
			// Overflow-wrap mode (only active for 'break-word' which is just '1'):
			text.OverflowWrapActive=( cs.ResolveInt(Css.Properties.OverflowWrap.GlobalProperty) == 1 );
			
			// Check if the text is 'dirty'.
			// If it is, that means we'll need to rebuild the TextRenderingProperty's Glyph array.
			
			if(text.Dirty){
				
				// Setup text now:
				// (Resets text.Characters based on all the text related CSS properties like variant etc).
				text.LoadCharacters((Node as RenderableTextNode).characterData_,this);
				
			}
			
			if(text.Characters==null || text.AllEmpty){
				text.FontSize=0f;
				return;
			}
			
		}
		
		public override void Reflow(Renderman renderer){
			
			// Get the text renderer (or create it):
			Css.TextRenderingProperty text=RequireTextProperty();
			
			if(text.Characters==null || text.AllEmpty){
				return;
			}
			
			LayoutBox box=null;
			
			// Get the baseline offset:
			float baseline=text.FontSize * text.FontToDraw.Descender;
			
			// Compute our line boxes based on text.Characters and the available space.
			// Safely ignore direction here because either way the selected characters are the same.
			// Note that things like first-letter are also considered.
			
			// Get the top of the stack:
			LineBoxMeta lbm=renderer.TopOfStack;
			float cssLineHeight=lbm.CssLineHeight;
			
			// Is it justify (if so, every word is its own box):
			bool breakAllWords=(lbm.HorizontalAlign==HorizontalAlignMode.Justify);
			
			// Offset the baseline:
			float lineHeightOffset=(cssLineHeight-text.FontSize)/2f;
			
			text.LineHeightOffset=lineHeightOffset;
			baseline+=lineHeightOffset;
			
			float wordWidth=0f;
			float boxWidth=0f;
			bool wrappable=((lbm.WhiteSpace & WhiteSpaceMode.Wrappable)!=0);
			int i=0;
			int latestBreakpoint=-1;
			// bidi text direction (0 = Not set yet, UnicodeBidiMode.LeftwardsNormal, UnicodeBidiMode.RightwardsNormal)
			int direction=0;
			// Direction before the current block of weak chars.
			int beforeWeak=0;
			
			
			while(i<text.Characters.Length){
				
				// Get the glyph:
				InfiniText.Glyph glyph=text.Characters[i];
				
				if(glyph==null){
					// Skip!
					i++;
					continue;
				}
				
				// The glyph's width is..
				float width=(glyph.AdvanceWidth * text.FontSize)+text.LetterSpacing;
				
				if(box==null){
					
					// The box always has an inner height of 'font size':
					if(renderer.FirstLetter!=null){
						
						// Clear FL immediately (so it can't go recursive):
						SparkInformerNode firstLetter=renderer.FirstLetter;
						renderer.FirstLetter=null;
						
						// Update its internal text node:
						RenderableTextNode textNode=firstLetter.firstChild as RenderableTextNode;
						
						// Note that we have to do it this way as the node might
						// change the *font*.
						textNode.characterData_=((char)glyph.Charcode)+"";
						Css.TextRenderingProperty textData=textNode.RenderData.RequireTextProperty();
						textData.Dirty=true;
						
						// Ask it to reflow right now (must ask the node so it correctly takes the style into account):
						firstLetter.RenderData.UpdateCss(renderer);
						firstLetter.RenderData.Reflow(renderer);
						
						i++;
						continue;
						
					}
					
					// Create the box now:
					box=new LayoutBox();
					box.PositionMode=PositionMode.Static;
					box.DisplayMode=DisplayMode.Inline;
					box.Baseline=baseline;
					box.TextStart=i;
					
					// Adopt current direction:
					box.UnicodeBidi=direction;
					
					if(FirstBox==null){
						FirstBox=box;
						LastBox=box;
					}else{
						// add to this element:
						LastBox.NextInElement=box;
						LastBox=box;
					}
					
					// line-height is the inner height here:
					box.InnerHeight=cssLineHeight;
					boxWidth=0f;
					wordWidth=0f;
				}
				
				// Are we breaking this word?
				bool implicitNewline=(glyph.Charcode==(int)'\n');
				int breakMode=implicitNewline ? 1 : 0;
				
				// direction:
				int dir=glyph.Directionality;
				
				if(dir == InfiniText.BidiBlock.Rightwards){
					// Rightwards (includes weak ones):
					
					// Was the previous one 'weak'?
					if(dir == InfiniText.BidiBlock.WeakLeftToRight){
						
						// Update before weak:
						beforeWeak = direction;
						
					}else{
						
						// Not weak:
						beforeWeak = 0;
						
					}
					
					if(direction!=UnicodeBidiMode.RightwardsNormal){
						
						if(direction==0){
							
							// Not set yet - Set the mode into the box:
							box.UnicodeBidi=UnicodeBidiMode.RightwardsNormal;
							
						}else{
							
							// Changed from leftwards to rightwards. Break the boxes here.
							breakMode=3;
							
						}
						
						// Update dir:
						direction=UnicodeBidiMode.RightwardsNormal;
						
					}
					
					
				}else if(dir == InfiniText.BidiBlock.RightToLeft){
					// Leftwards
					
					if(direction!=UnicodeBidiMode.LeftwardsNormal){
						
						if(direction==0){
							
							// Not set yet - Set the mode into the box:
							box.UnicodeBidi=UnicodeBidiMode.LeftwardsNormal;
							
						}else{
							
							// Changed from rightwards to leftwards. Break the boxes here.
							breakMode=3;
							
						}
						
						// Update dir:
						direction=UnicodeBidiMode.LeftwardsNormal;
						
					}
					
					// Never weak:
					beforeWeak = 0;
					
				}else if(beforeWeak != 0){
					
					// Neutral otherwise
					// (adopts whatever the current is, unless the previous one was *weak*, 
					// in which case, it adopts whatever the direction was before that):
					if(direction != beforeWeak){
						
						// Change direction!
						direction = beforeWeak;
						breakMode=3;
						
					}
					
				}
				
				// Got a space?
				bool space=(glyph.Charcode==(int)' ');
				
				if(space){
					
					// Advance width:
					width+=text.WordSpacing;
					
					if(breakAllWords){
						breakMode=3;
					}else{
						latestBreakpoint=i;
					}
					
					// Lock in the previous text:
					boxWidth+=wordWidth+width;
					wordWidth=0f;
					
				}else{
					
					// Advance word width now:
					wordWidth+=width;
					
				}
				
				// Word wrapping next:
				if(breakMode==0 && wrappable && i!=box.TextStart){
					
					// Test if we can break here:
					breakMode=lbm.GetLineSpace(wordWidth,boxWidth);
					
					// Return to the previous space if we should.
					if(breakMode==2 && text.OverflowWrapActive){
						
						// The word doesn't fit at all (2) and we're supposed to break it.
						boxWidth+=wordWidth-width;
						wordWidth=0f;
					
					}else if(breakMode!=0){
						
						if(latestBreakpoint==-1){
							
							// Isn't a previous space!
							
							if(breakMode==2){
								
								// Instead, we'll try and break a parent.
								// This typically happens with inline elements 
								// which are right on the end of the host line.
								lbm.TryBreakParent();
								
								// Don't break the node:
								breakMode=0;
								
							}else if(breakMode==3){
								
								// Break but no newline - just advance to the following char:
								i++;
								
							}else if(lbm.PenX!=lbm.LineStart){
								
								// Newline:
								lbm.CompleteLine(LineBreakMode.Normal);
								
								// Don't break the node:
								breakMode=0;
								
							}else{
								
								// Don't break the node:
								breakMode=0;
								
							}
							
						}else{
							i=latestBreakpoint+1;
						}
						
					}
					
				}
				
				if(breakMode!=0){
					
					// We're breaking!
					box.InnerWidth=boxWidth;
					box.TextEnd=i;
					latestBreakpoint=i;
					
					// If the previous glyph is a space, update EndSpaceSize:
					if(space){
						// Update ending spaces:
						box.EndSpaceSize=width;
					}
					
					// Ensure dimensions are set:
					box.SetDimensions(false,false);
					
					// Add the box to the line:
					lbm.AddToLine(box);
					
					// Update dim's:
					lbm.AdvancePen(box);
					
					// Clear:
					box=null;
					boxWidth=0f;
					
					if(breakMode!=3){
						
						// Newline:
						if(implicitNewline){
							
							// Also the last line:
							lbm.CompleteLine(LineBreakMode.Normal | LineBreakMode.Last);
							
						}else{
							
							// Normal:
							lbm.CompleteLine(LineBreakMode.Normal);
							
							// Process it again.
							continue;
							
						}
						
					}
					
				}
				
				// Next character:
				i++;
				
			}
			
			if(box!=null){
				
				// Always apply inner width:
				box.InnerWidth=boxWidth+wordWidth;
				box.TextEnd=text.Characters.Length;
				
				// Ensure dimensions are set:
				box.SetDimensions(false,false);
				
				// Add the box to the line:
				lbm.AddToLine(box);
				
				// Update dim's:
				lbm.AdvancePen(box);
				
			}
			
		}
		
	}
	
}