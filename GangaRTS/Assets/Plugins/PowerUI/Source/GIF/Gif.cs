//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using BinaryIO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spa;


namespace Gif{
	
	/// <summary>
	/// GIF's are amazing. This loads them and oh dear I accidentally 96mb pleas help
	/// </summary>
	public partial class Gif{
		
		/// <summary>The width.</summary>
		public int Width;
		/// <summary>The height.</summary>
		public int Height;
		/// <summary>Colour res.</summary>
		public byte ColorResolution;
		/// <summary>The sort flags.</summary>
		public int SortFlags;
		/// <summary>Pixel aspect.</summary>
		public byte PixelAspect;
		/// <summary>A host SPA which will run animations for us.</summary>
		public SPA HostSPA;
		/// <summary>Gif frames. Note that these aren't necessarily actual frames as, in order to get true colour GIF's,
		/// a hack is used where multiple "frames" are layered on top of each other to form a single actual frame.</summary>
		public GifFrame[] Frames;
		/// <summary>Additional blocks in the GIF stream. Doesn't include graphic control blocks.</summary>
		public List<GifBlock> Blocks=new List<GifBlock>();
		/// <summary>The background colour.</summary>
		public Color32 BackgroundColour;
		
		
		/// <summary>Loads a GIF from the given stream.</summary>
		public Gif(System.IO.Stream stream):this(new Reader(stream)){}
		
		/// <summary>Loads a GIF from the given data.</summary>
		public Gif(byte[] data):this(new Reader(data)){}
		
		/// <summary>Loads a GIF from the given reader.</summary>
		public Gif(Reader reader){
			Load(reader);
		}
		
		public SPAInstance GetInstance(){
			return HostSPA.GetInstance();
		}
		
		public void Load(Reader reader){
			
			// Read the header:
			reader.ReadString(6);
			
			Width=reader.ReadInt16();
			Height=reader.ReadInt16();
			
			if(HostSPA==null){
				// Create the host SPA (default 100fps):
				HostSPA=new SPA(Width,Height,100);
			}
			
			// Apply parent:
			HostSPA.ParentGif=this;
			
			byte packed=(byte)reader.ReadByte();
			
			bool colorTableFlag=((packed & 0x80) >> 7) == 1;
			ColorResolution=(byte)((packed & 0x60) >> 5);
			SortFlags=((byte)(packed & 0x10)) >> 4;
			
			// Background index (unused):
			byte bgIndex=(byte)reader.ReadByte();
			
			PixelAspect=(byte)reader.ReadByte();
			
			Color32[] globalColorTable=null;
			BackgroundColour=new Color32(0,0,0,0);
			
			if(colorTableFlag){
				
				int colorTableSize=((int)2) << (packed & 7);
			
				globalColorTable=LoadPalette( reader.ReadBytes(colorTableSize * 3) );
				
				if(bgIndex<globalColorTable.Length){
					
					BackgroundColour=globalColorTable[bgIndex];
					
				}
				
			}
			
			byte nextFlag=(byte)reader.ReadByte();
			int frameCount=0;
			int gcbCount=0;
			
			Color32[] previousFrame=null;
			List<SPASprite> spaFrames=new List<SPASprite>();
			List<GifFrame> frames=new List<GifFrame>();
			
			while(nextFlag!=GifBlocks.Terminator){
				
				if(nextFlag==GifBlocks.ImageLabel){
					
					GifFrame frame;
					
					if(frames.Count<=frameCount){
						
						// Create the frame now:
						frame = new GifFrame(this,frameCount);
						frames.Add(frame);
						
					}else{
						
						// Load it:
						frame=frames[frameCount];
						
					}
					
					// Drop zero-delay frames from the output.
					// - This makes "full colour" GIFs work.
					if(frame.Delay!=0f || spaFrames.Count==0){
						spaFrames.Add(frame);
					}
					
					previousFrame=ReadFrame(previousFrame,frame,reader,globalColorTable);
					frameCount++;
					
				}else if (nextFlag == GifBlocks.ExtensionIntroducer){
				
					int gcl = reader.ReadByte();
					
					switch (gcl){
						case GifBlocks.GraphicControlLabel:
							
							int blockSize = reader.ReadByte();
							
							if (blockSize != 4){
								throw new Exception("A graphic extension block had the wrong size in a GIF stream.");
							}
							
							byte gPacked = (byte)reader.ReadByte();
							bool transparencyFlag = (gPacked & 0x01) == 1;
							int disposalMethod = (gPacked & 0x1C) >> 2;
							
							// Frame delay in seconds:
							float delay = (float)reader.ReadInt16() / 100f;
							
							byte transparencyIndex = (byte)reader.ReadByte();
							
							// Terminal 0:
							reader.ReadByte();
							
							GifFrame frame;
							
							if(frames.Count<=gcbCount){
								
								// Create the frame now:
								frame = new GifFrame(this,gcbCount);
								frames.Add(frame);
								
							}else{
								
								// Load it:
								frame=frames[gcbCount];
								
							}
							
							// Apply delay:
							frame.Delay=delay;
							frame.DisposalMethod=disposalMethod;
							
							if(transparencyFlag){
								frame.TransparencyIndex=transparencyIndex;
							}else{
								frame.TransparencyIndex=-1;
							}
							
							gcbCount++;
							
							break;
						case GifBlocks.CommentLabel:
							
							CommentBlock commentBlock=new CommentBlock();
							commentBlock.Load(reader);
							Blocks.Add(commentBlock);
							
							break;
						case GifBlocks.ApplicationExtensionLabel:
							
							ApplicationExtensionBlock appBlock=new ApplicationExtensionBlock();
							appBlock.Load(reader);
							Blocks.Add(appBlock);
							
							break;
						case GifBlocks.PlainTextLabel:
							
							PlainTextBlock texBlock=new PlainTextBlock();
							texBlock.Load(reader);
							Blocks.Add(texBlock);
							
							break;
					}
					
				}else if (nextFlag == GifBlocks.EndIntroducer){
					break;
				}
				
				nextFlag = reader.ReadByte();
			}
			
			Frames=frames.ToArray();
			
			// Apply the frames to the host SPA:
			HostSPA.Sprites=spaFrames.ToArray();
			
		}
		
		/// <summary>Loads a palette as a block of Color32's.</summary>
		public static Color32[] LoadPalette(byte[] table){
			
			Color32[] tab = new Color32[table.Length / 3];
			
			int i = 0;
			int j = 0;
			
			while (i < table.Length){
				
				byte r = table[i++];
				byte g = table[i++];
				byte b = table[i++];
				Color32 c = new Color32(r,g,b,255);
				tab[j++] = c;
				
			}
			
			return tab;
			
		}
		
		/// <summary>Reads a frame from the stream.</summary>
		public Color32[] ReadFrame(Color32[] prevFrame,GifFrame frame,Reader reader,Color32[] globalTable){
			
			// Read the image header:
			short xOffset = reader.ReadInt16();
			short yOffset = reader.ReadInt16();
			short width = reader.ReadInt16();
			short height = reader.ReadInt16();
			
			byte packed = (byte)reader.ReadByte();
			bool lctFlag = ((packed & 0x80) >> 7) == 1;
			bool interlaceFlag = ((packed & 0x40) >> 6) == 1;
			// bool sortFlag = ((packed & 0x20) >> 5) == 1;
			int lctSize = (2 << (packed & 0x07));
			
			Color32[] colourTable;
			
			if(lctFlag){
				// It has a local colour table:
				colourTable=LoadPalette( reader.ReadBytes(lctSize*3) );
			}else{
				// Use the global colour table otherwise.
				colourTable=globalTable;
			}
			
			// Load the pixel data:
			int dataSize = reader.ReadByte();
			byte[] pixels = LzwDecoder.Decode(reader,width, height, dataSize);
			
			int blockSize = reader.ReadByte();
			
			if(blockSize>0){
				// Read the block:
				reader.ReadBytes(blockSize);
			}
			
			frame.OffsetY=yOffset;
			frame.OffsetX=xOffset;
			frame.Width=width;
			frame.Height=height;
			
			Color32[] fullImage=LoadFramePixels(prevFrame,pixels,colourTable,interlaceFlag,frame);
			
			return fullImage;
		}
		
		public static Color32[] LoadFramePixels(Color32[] prevImage,byte[] pixels, Color32[] colorTable, bool interlaceFlag,GifFrame frame){
			
			Gif gif=frame.Gif;
			
			int fw=frame.Width;
			int iw=gif.Width;
			int ih=gif.Height;
			
			
			// Get transparency index:
			int transIndex=frame.TransparencyIndex;
			
			// Based on disposal method, we may actually be using prevImage.
			Color32[] fullImage=prevImage;
			
			int disposal=frame.DisposalMethod;
			int maxY=ih-1;
			bool drawTransparent=(disposal==2);
			
			/*
			if(disposal==0){
				// Undefined - do nothing.
				// - Safe to reuse prevImage here.
			}else if(disposal==1){
				// Don't dispose. Return the frame from this function.
				// - Safe to reuse prevImage here.
			}else if(disposal==2){
				// Restore to background.
				// - Safe to reuse prevImage here.
			}
			*/
			
			if(disposal==3){
				
				// Restore to previous. Return prevImage.
				// - Can't reuse prevImage here.
				fullImage=null;
				
			}
			
			// Create the image area if one is needed:
			if(fullImage==null){
				fullImage=new Color32[iw * ih];
			}
			
			// Got a previous image?
			if(prevImage!=null && prevImage!=fullImage){
				
				// Blit previous into fullImage:
				Array.Copy(prevImage,0,fullImage,0,fullImage.Length);
				
			}
			
			int startRowPoint=((maxY-frame.OffsetY)*iw);
			
			int offSet = 0;
			
			if (interlaceFlag){
				
				int i = 0;
				int row=0;
				int rowPoint=startRowPoint;
				
				int pass = 0;	 
				while (pass < 4){
				   
					if (pass == 1){
						rowPoint=startRowPoint - (4 * iw);
						offSet += 4 * fw;
					}else if (pass == 2){
						rowPoint=startRowPoint - (2 * iw);
						offSet += 2 * fw;
					}else if (pass == 3){
						rowPoint=startRowPoint - (1 * iw);
						offSet += 1 * fw;
					}
					
					int rate = 1;
					
					if (pass == 0 | pass == 1){
						rate = 7;
					}else if (pass == 2){
						rate = 3;
					}
					
					while (i < pixels.Length){						 
						
						int colIndex=pixels[i++];
						
						if(colIndex==transIndex){
							
							if(drawTransparent){
								fullImage[rowPoint+row+frame.OffsetX] = new Color32(0,0,0,0);
							}
							
						}else{
							
							fullImage[rowPoint + row + frame.OffsetX] = colorTable[colIndex];
						}
						
						row++;
						
						offSet++;
						
						if(row == fw){
							// End of the row
							row=0;
							rowPoint-=(rate+1) * iw;
							offSet += (fw * rate);
							
							if ( offSet  >= pixels.Length)
							{
								pass++;
								offSet = 0;
								break;
							}
							
						}						
					}
				}
				
			}else{
				
				int row=0;
				int rowPoint=startRowPoint;
				
				for(int i = 0; i < pixels.Length; i++){						
					
					int colIndex=pixels[i];
					
					if(colIndex==transIndex){
						
						if(drawTransparent){
							
							fullImage[rowPoint+row+frame.OffsetX] = new Color32(0,0,0,0);
						
						}
						
					}else{
						
						fullImage[rowPoint+row+frame.OffsetX] = colorTable[colIndex];
						
					}
					
					row++;
					
					if(row==fw){
						row=0;
						rowPoint-=iw;
					}
					
				}
				
			}
			
			// Create frame texture:
			Texture2D tex=new Texture2D(frame.Gif.Width,frame.Gif.Height);
			tex.SetPixels32(fullImage);
			
			// Make sure it filters correctly.
			// This is so we don't see parts of other frames around the edge of the image onscreen:
			tex.filterMode=FilterMode.Point;
			
			tex.Apply();
			
			frame.Sprite=tex;
			
			if(disposal==3){
				return prevImage;
			}
			
			if(disposal==2){
				
				// Restore to background.
				int graphicSize=fw*frame.Height;
				int row=0;
				int rowPoint=startRowPoint;
				
				Color32 background=gif.BackgroundColour;
				
				for(int i=0;i<graphicSize;i++){
					
					fullImage[rowPoint+row+frame.OffsetX]=background;
					
					row++;
					
					if(row==fw){
						// End of the row
						row=0;
						rowPoint-=iw;
					}
					
				}
				
			}
			
			return fullImage;
			
		}
		
	}
	
}

namespace Spa{
	
	public partial class SPA{
		
		/// <summary>The GIF object that this SPA is for.</summary>
		public Gif.Gif ParentGif;
		
	}
	
}

namespace PowerUI{
	
	public partial class SpriteEvent{
		
		/// <summary>The GIF being played.</summary>
		public Gif.Gif gif{
			get{
				return spa.ParentGif;
			}
		}
		
	}
	
}