//--------------------------------------
//                Blaze
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

namespace Blaze{

	/// <summary>
	/// Represents a texture atlas which is dynamically added to and modified. Must be used with an AtlasStack.
	/// It internally tracks which textures it has to prevent frequent rebuilds.
	/// By using a texture atlas like this, many drawcalls can be compressed to one.
	/// It also supports dynamic textures. These write directly to the atlas pixels.
	/// </summary>

	public class TextureAtlas{
		
		/// <summary>Spacing around objects on this atlas.</summary>
		private int RawSpacing;
		/// <summary>The parent stack.</summary>
		public AtlasStack Stack;
		/// <summary>Atlases are stored in a stack. The next one.</summary>
		public TextureAtlas Next;
		/// <summary>Atlases are stored in a stack. The previous one.</summary>
		public TextureAtlas Previous;
		/// <summary>The length of the sides of the atlas, in pixels.</summary>
		public int Dimension;
		/// <summary>The length of the sides of the atlas, in pixels, as a float.</summary>
		public float DimensionF;
		/// <summary>1/the dimension.</summary>
		public float InvertedSize;
		/// <summary>The block of all pixels of this atlas.</summary>
		public Color32[] Pixels;
		/// <summary>True if any pixels were changed.</summary>
		public bool PixelChange;
		/// <summary>True if an image was removed from the atlas and it now has a 'hole'.</summary>
		public bool CanOptimize;
		/// <summary>The pixels of the atlas as a displayable texture.</summary>
		public Texture2D Texture;
		/// <summary>The horizontal column progress. Used in column allocation mode.</summary>
		private int ColumnProgressX;
		/// <summary>The vertical column progress. Used in column allocation mode.</summary>
		private int ColumnProgressY;
		/// <summary>Current column width.</summary>
		private int ColumnWidth;
		/// <summary>True if something failed to be added that would otherwise have fit.</summary>
		public bool OptimizeRequested;
		/// <summary>Images which are removed are stored in a linked list. This is the tail of the list.</summary>
		public AtlasLocation LastEmpty;
		/// <summary>Images which are removed are stored in a linked list. This is the head of the list.</summary>
		public AtlasLocation FirstEmpty;
		/// <summary>The mode in which the atlases on this stack allocate images.</summary>
		public AtlasingMode Mode=AtlasingMode.SmallestSpace;
		
		
		/// <summary>Generates a new texture atlas of size 1024px x 1024px.</summary>
		public TextureAtlas():this(1024,FilterMode.Point,TextureFormat.ARGB32,true){}
		
		public TextureAtlas(int dimension):this(dimension,FilterMode.Point,TextureFormat.ARGB32,true){}
		
		/// <summary>Generates a new atlas with the given x/y dimension (all atlases are square).</summary>
		/// <param name="dimension">The length in pixels of the side of the atlas.</param>
		public TextureAtlas(int dimension,FilterMode filter,TextureFormat format,bool pixels){
			Dimension=dimension;
			
			// Create underlying texture:
			Texture=new Texture2D(dimension,dimension,format,false);
			Texture.filterMode=filter;
			Texture.wrapMode=TextureWrapMode.Clamp;
			
			if(pixels){
				// Setup pixel buffer:
				Pixels=new Color32[dimension*dimension];
			}
			
			// Cache dimension values:
			DimensionF=(float)dimension;
			InvertedSize=1f/DimensionF;
			
			// Setup initial atlas regions:
			Reset();
		}
		
		/// <summary>Creates a read-only atlas.</summary>
		public TextureAtlas(Texture2D image){
			
			// Apply the texture:
			Texture=image;
			
			// Get the dimension:
			Dimension=image.width;
			
			// Cache dimension values:
			DimensionF=(float)Dimension;
			InvertedSize=1f/DimensionF;
			
		}
		
		/// <summary>Spacing around objects on this atlas.</summary>
		public int Spacing{
			get{
				return RawSpacing;
			}
			set{
				RawSpacing=value;
			}
		}
		
		/// <summary>Removes this atlas from the stack.</summary>
		public void RemoveFromStack(){
			
			if(Previous==null){
				Stack.First=Next;
			}else{
				Previous.Next=Next;
			}
			
			if(Next==null){
				Stack.Last=Previous;
			}else{
				Next.Previous=Previous;
			}
			
		}
		
		/// <summary>The filter mode of this atlas.</summary>
		public FilterMode FilterMode{
			get{
				return Texture.filterMode;
			}
			set{
				Texture.filterMode=value;
			}
		}
		
		/// <summary>Clears all content from this atlas</summary>
		internal void Reset(){
			FirstEmpty=LastEmpty=null;
			ColumnProgressX=0;
			ColumnProgressY=0;
			ColumnWidth=0;
			
			// Add the root atlas location (NB: it adds itself internally)
			AtlasLocation root=new AtlasLocation(this,0,0,Dimension,Dimension);
			
			// Immediately mark this as empty:
			root.AddToEmptySet();
		}
		
		/// <summary>Flushes changes to the pixel set to the texture.</summary>
		public void Flush(){
			if(!PixelChange){
				// No pixels changed anyway.
				return;
			}
			
			PixelChange=false;
			
			if(Pixels!=null){
				Texture.SetPixels32(Pixels);
			}
			
			Texture.Apply(false);
		}
		
		/// <summary>Attempts to add the "remote" atlas location to this atlas.
		/// The original location object is retained.</summary>
		/// <returns>True if this atlas accepted the location.</summary>
		internal bool OptimiseAdd(AtlasLocation location){
			
			int fitFactor=0;
			int area=location.Area;
			AtlasLocation currentAccepted=null;
			AtlasLocation currentEmpty=FirstEmpty;
			
			while(currentEmpty!=null){
				int factor=currentEmpty.FitFactor(location.Width,location.Height,area);
				
				if(factor==0){
					// Perfect fit - break right now; can't beat that!
					currentAccepted=currentEmpty;
					break;
					
				}else if(factor!=-1){
					// We can possibly fit here - is it the current smallest?
					if(currentAccepted==null||factor<fitFactor){
						// Yep! select it.
						fitFactor=factor;
						currentAccepted=currentEmpty;
					}
				}
				
				currentEmpty=currentEmpty.EmptyAfter;
			}
			
			if(currentAccepted==null){
				return false;
			}
			
			// We've got a block of space that we'll be "adding" to.
			// Note that we're going to *keep* the original location.
			currentAccepted.OptimiseSelect(location);
			
			
			return true;
			
		}
		
		/// <summary>Adds the given texture to the atlas if it's not already on it,
		/// taking up a set amount of space on the atlas.</summary>
		/// <param name="texture">The texture to add.</param>
		/// <param name="width">The x amount of space to take up on the atlas.</param>
		/// <param name="height">The y amount of space to take up on the atlas.</param>
		/// <returns>The location of the texture on the atlas.</returns>
		internal AtlasLocation Add(AtlasEntity texture,int entityID,int width,int height){
			
			// Pad width/height:
			int spacedWidth=width+RawSpacing;
			int spacedHeight=height+RawSpacing;
			
			// Look for a spot to park this texture in the set of empty blocks.
			
			// The aim is to make it fit in the smallest empty block possible to save space.
			// This is done with a 'fitFactor' - this is simply the difference between the blocks area and the textures area.
			// We want this value to be as small as possible.
			AtlasLocation currentAccepted=null;
			
			int area=spacedWidth*spacedHeight;
			
			if(Mode==AtlasingMode.Columns){
				
				// Space in this column?
				int max=ColumnProgressY + spacedHeight;
				
				if(max<=Dimension){
					
					// Yep! Create a location here:
					currentAccepted=new AtlasLocation(this,ColumnProgressX,ColumnProgressY,spacedWidth,spacedHeight);
					
					// Move it:
					ColumnProgressY+=spacedHeight;
					
					if(spacedWidth>ColumnWidth){
						// Update width:
						ColumnWidth=spacedWidth;
					}
					
					// As it's a new location, it's empty by default:
					// (Note that it must be in the empty set, otherwise Select will throw the empty set entirely)
					currentAccepted.AddToEmptySet();
					
				}else{
					
					// Space to generate a new column?
					max=ColumnProgressX + ColumnWidth + spacedWidth;
					
					if(max<=Dimension){
						
						// Set Y:
						ColumnProgressY=spacedHeight;
						
						// Move X:
						ColumnProgressX+=ColumnWidth;
						
						// Yep! Create a location here:
						currentAccepted=new AtlasLocation(this,ColumnProgressX,0,spacedWidth,spacedHeight);
						
						// Reset width:
						ColumnWidth=spacedWidth;
						
						// As it's a new location, it's empty by default:
						// (Note that it must be in the empty set, otherwise Select will throw the empty set entirely)
						currentAccepted.AddToEmptySet();
						
					}
						
					// Otherwise, the atlas is practically full.
					// We're gonna just reject the add (by falling below), and state that it can be optimised.
					// This triggers another atlas to get created and this one will 
					// be optimised at some point in the near future.
					
				}
				
			}else{
				
				int fitFactor=0;
				
				AtlasLocation currentEmpty=FirstEmpty;
				
				while(currentEmpty!=null){
					int factor=currentEmpty.FitFactor(spacedWidth,spacedHeight,area);
					
					if(factor==0){
						// Perfect fit - break right now; can't beat that!
						currentAccepted=currentEmpty;
						break;
						
					}else if(factor!=-1){
						// We can possibly fit here - is it the current smallest?
						if(currentAccepted==null||factor<fitFactor){
							// Yep! select it.
							fitFactor=factor;
							currentAccepted=currentEmpty;
						}
					}
					
					currentEmpty=currentEmpty.EmptyAfter;
				}
				
			}
			
			if(currentAccepted==null){
				// No space in this atlas to fit it in. Stop there.
				
				if(CanOptimize){
					// Request an optimise:
					OptimizeRequested=true;
					Stack.OptimizeRequested=true;
				}
				
				return null;
			}
			
			Stack.ActiveImages[entityID]=currentAccepted;
			
			// And burn in the texture to the location (nb: it internally also writes the pixels to the atlas).
			currentAccepted.Select(texture,width,height,RawSpacing);
			
			return currentAccepted;
		}
		
		/// <summary>Optimizes the atlas by removing all 'holes' (removed images) from the atlas.
		/// It reconstructs the whole atlas (only when there are actually holes), so this method should be considered expensive.
		/// This is only ever called when we fail to add something to the atlas; Theres no performace issues of a non-optimized atlas.
		/// Instead it just simply has very fragmented space available.</summary>
		public bool Optimize(){
			
			if(!CanOptimize){
				// It'll do as it is.
				return false;
			}
			
			// Make sure it's not called again:
			CanOptimize=false;
			OptimizeRequested=false;
			
			Dictionary<int,AtlasLocation> allImages=Stack.ActiveImages;
			
			// Clear the textures and add in the starting empty location.
			Reset();
			
			// Next up, add them all back in, and that's it!
			// The optimizing comes from them trying to fit in the smallest possible gap they can when added.
			foreach(KeyValuePair<int,AtlasLocation> kvp in allImages){
				AtlasLocation location=kvp.Value;
				
				if(location.Atlas==this){
					
					AtlasEntity image=location.Image;
					
					int entityID=image.GetAtlasID();
					
					int width;
					int height;
					
					image.GetDimensionsOnAtlas(out width,out height);
					
					Add(image,entityID,width,height);
				}
				
			}
			
			return true;
		}
		
		/// <summary>Destroys this atlas when it's no longer needed.</summary>
		public void Destroy(){
			Pixels=null;
			
			if(Texture!=null){
				GameObject.Destroy(Texture);
				Texture=null;
			}
			
		}
		
	}
	
}