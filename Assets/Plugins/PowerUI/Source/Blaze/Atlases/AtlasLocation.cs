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


namespace Blaze{

	/// <summary>
	/// Represents the location of an image on an atlas.
	/// </summary>

	public class AtlasLocation:UVBlock{
		
		/// <summary>The number of times this location is in use.</summary>
		public int UsageCount;
		/// <summary>If this location is empty (texture was removed), it's stored in a linked list.
		/// This is the element after this one in the list.</summary>
		public AtlasLocation EmptyAfter;
		/// <summary>If this location is empty (texture was removed), it's stored in a linked list.
		/// This is the element before this one in the list.</summary>
		public AtlasLocation EmptyBefore;
		/// <summary>The x coord in pixels of the left edge of the image from the left of the atlas.</summary>
		public int X;
		/// <summary>The y coord in pixels of the bottom edge of the image from the bottom of the atlas.</summary>
		public int Y;
		/// <summary>The area of this image (width*height).</summary>
		public int Area;
		/// <summary>The width of the image in pixels.</summary>
		public int Width;
		/// <summary>The height of the image in pixels.</summary>
		public int Height;
		/// <summary>True if the texture in this location has been removed and it is now empty.</summary>
		public bool Empty;
		/// <summary>The image stored at this atlas location.</summary>
		public AtlasEntity Image;
		/// <summary>The ID of the entity stored in this location. Same as Image.GetAtlasID().</summary>
		public int AtlasID;
		/// <summary>The atlas this location is on.</summary>
		public TextureAtlas Atlas;
		/// <summary>1/the atlas width.</summary>
		public float InvertedSizeX;
		/// <summary>1/the atlas height.</summary>
		public float InvertedSizeY;
		/// <summary>Active spacing on this location.</summary>
		public int Spacing;
		
		/// <summary>Creates a new location on the given atlas with the given size.</summary>
		/// <param name="atlas">The atlas this location refers to.</param>
		/// <param name="x">The x coordinate of the left edge of this location in pixels from the left.</param>
		/// <param name="y">The y coordinate of the bototm edfe of this location in pixels from the bottom.</param>
		/// <param name="width">The width of the location in pixels.</param>
		/// <param name="height">The height of the location in pixels.</param>
		public AtlasLocation(TextureAtlas atlas,int x,int y,int width,int height){
			X=x;
			Y=y;
			Atlas=atlas;
			Width=width;
			Height=height;
			Area=Width*Height;
			
			// BakeUV isn't called here as its results will be always overriden when select calls it.
			// As a result, empty locations have invalid UVs; this is ok.
		}
		
		/// <summary>Creates a new atlas location without an atlas.</summary>
		/// <param name="x">The x coordinate of the left edge of this location in pixels from the left.</param>
		/// <param name="y">The y coordinate of the bototm edfe of this location in pixels from the bottom.</param>
		/// <param name="width">The width of the location in pixels.</param>
		/// <param name="height">The height of the location in pixels.</param>
		/// <param name="imageWidth">The width of the atlas.</param>
		/// <param name="imageHeight">The height of the atlas.</param>
		public AtlasLocation(int x,int y,int width,int height,float imageWidth,float imageHeight):this(null,x,y,width,height){
			
			InvertedSizeX=1f/imageWidth;
			InvertedSizeY=1f/imageHeight;
			
			// Immediately bake UV:
			MinX=(float)X * InvertedSizeX;
			MinY=(float)Y * InvertedSizeY;
			
			float UVheight=(float)Height * InvertedSizeY;
			float UVwidth=(float)Width * InvertedSizeX;
			
			MaxX=UVwidth+MinX;
			MaxY=UVheight+MinY;
			
		}
		
		/// <summary>A fixed atlas location with 0->1 UV's.</summary>
		public AtlasLocation(float imageWidth,float imageHeight){
			
			Width=(int)imageWidth;
			Height=(int)imageHeight;
			Area=Width*Height;
			
			InvertedSizeX=1f/imageWidth;
			InvertedSizeY=1f/imageHeight;
			
			// Immediately bake UV:
			MaxX=1f;
			MaxY=1f;
			
		}
		
		/// <summary>Updates a fixed image location.</summary>
		public void UpdateFixed(float imageWidth,float imageHeight){
			
			Width=(int)imageWidth;
			Height=(int)imageHeight;
			Area=Width*Height;
			
			InvertedSizeX=1f/imageWidth;
			InvertedSizeY=1f/imageHeight;
			
		}
		
		/// <summary>True if this UV block is a globally shared one.</summary>
		public override bool Shared{
			get{
				return true;
			}
		}
		
		/// <summary>Blocks this location from ever getting deallocated from being no longer "in use".</summary>
		public void PreventDeallocation(){
			UsageCount=-100;
		}
		
		/// <summary>Called when an instance of the image goes offscreen.</summary>
		/// <returns>True if this location is now completely unused.</returns>
		public bool DecreaseUsage(){
			
			if(UsageCount==-100){
				// Never deallocate.
				return false;
			}
			
			UsageCount--;
			
			if(UsageCount<=0){
				// Free this atlas location:
				Deselect();
				return true;
			}
			
			return false;
		}
		
		/// <summary>Empties this location and frees it up for use by other textures.</summary>
		public void Deselect(){
			
			AddToEmptySet();
			
			// Remove from the stack images:
			Atlas.Stack.ActiveImages.Remove(AtlasID);
			
			Atlas.CanOptimize=true;
			
			if(Atlas.Next!=null){
				// Request an optimise:
				Atlas.OptimizeRequested=true;
				Atlas.Stack.OptimizeRequested=true;
			}
			
		}
		
		/// <summary>Adds this location to the set of empty locations on the atlas.
		/// This allows other textures to use this space on the atlas.</summary>
		public void AddToEmptySet(){
			if(Empty){
				return;
			}
			Empty=true;
			UsageCount=0;
			// Make sure nothing follows/preceeds this:
			EmptyBefore=EmptyAfter=null;
			
			if(Spacing!=0){
				Width+=Spacing;
				Height+=Spacing;
				Spacing=0;
			}
			
			if(Atlas.FirstEmpty==null){
				Atlas.FirstEmpty=Atlas.LastEmpty=this;
			}else{
				// If we have no texture, this is an empty block.
				// Empty blocks go to the start, deallocated texture blocks go to the end.
				// This allows for a potential preference on empty blocks, and the fast
				// restoration of a texture if it e.g. went temporarily off screen.
				// I.e. by putting them at the end, they have the highest chance of survival.
				// Chances are the newest ones are also the smallest too.
				if(Image==null){
					// Push to start of empty queue:
					EmptyAfter=Atlas.FirstEmpty;
					Atlas.FirstEmpty=Atlas.FirstEmpty.EmptyBefore=this;
				}else{
					// Push to end of empty queue:
					EmptyBefore=Atlas.LastEmpty;
					Atlas.LastEmpty=Atlas.LastEmpty.EmptyAfter=this;
				}
			}
		}
		
		/// <summary>A value which represents how well the given image dimensions fit in this location.
		/// Select the location with this as close to zero as possible.
		/// It's essentially pixels wasted using this location.</summary>
		/// <param name="width">The width of what you want to fit in this location.</param>
		/// <param name="height">The height of what you want to fit in this location.</param>
		/// <param name="area">width*height.</param>
		/// <returns>A number that should be as close to zero as possible.</returns>
		public int FitFactor(int width,int height,int area){
			if(area>Area||width>Width||height>Height){
				// Too many pixels, or too big in some dimension - it'll never fit here.
				return -1;
			}
			return Area-area;
		}
		
		/// <summary>This texture has selected this location to fit into. This adds it to the atlas.</summary>
		/// <param name="texture">The texture that wants to go here.</param>
		/// <param name="width">The width of the texture in pixels.</param>
		/// <param name="height">The height of the texture in pixels.</param>
		public void Select(AtlasEntity image,int width,int height,int spacing){
			// The given texture wants to go in this location.
			// Width and height are given for dynamic textures - textures who's pixels are actually written straight to the atlas.
			Empty=false;
			
			int spacedWidth=width+spacing;
			int spacedHeight=height+spacing;
			
			// Remove from empty queue:
			if(EmptyBefore==null){
				Atlas.FirstEmpty=EmptyAfter;
			}else{
				EmptyBefore.EmptyAfter=EmptyAfter;
			}
			
			if(EmptyAfter==null){
				Atlas.LastEmpty=EmptyBefore;
			}else{
				EmptyAfter.EmptyBefore=EmptyBefore;
			}
			
			int entityID=image.GetAtlasID();
			
			if(AtlasID==entityID){
				// This is a restore.
				
				// Set the new size of this location for UV baking:
				Width=width;
				Height=height;
				Spacing=spacing;
				
				// Update the UV's:
				BakeUV();
				
				return;
			}
			
			Image=image;
			AtlasID=entityID;
			// If it's not a perfect fit, generate new atlas locations to the right and above of this one.
			
			if(Atlas.Mode==AtlasingMode.SmallestSpace){
			
				if(Width>spacedWidth){
					// The textures a little thin.
					
					// Generate a new area to the right of this one (NB: 0,0 is bottom left)
					AtlasLocation newRight=new AtlasLocation(Atlas,X+spacedWidth,Y,Width-spacedWidth,height);
					// Immediately mark this as empty:
					newRight.AddToEmptySet();
				}
				
				if(Height>spacedHeight){
					// The textures a little short.
					
					// Generate a new area above this one (NB: 0,0 is bottom left)
					AtlasLocation newTop=new AtlasLocation(Atlas,X,Y+spacedHeight,Width,Height-spacedHeight);
					// Immediately mark this as empty:
					newTop.AddToEmptySet();
				}
				
			}
			
			// Set the new size of this location for UV baking:
			Width=width;
			Height=height;
			Spacing=spacing;
			
			// Update the UV's:
			BakeUV();
			
			// Make sure the area is up to date:
			Area=spacedWidth*spacedHeight;
			
			// Write it in:
			Flush();
		}
		
		/// <summary>Used during the optimise process. This texture has selected this location to fit into. This adds it to the atlas.</summary>
		/// <param name="location">The new location to write this region to.</param>
		public void OptimiseSelect(AtlasLocation location){
			// The given texture wants to go in this location.
			
			// Remove from empty queue:
			if(EmptyBefore==null){
				Atlas.FirstEmpty=EmptyAfter;
			}else{
				EmptyBefore.EmptyAfter=EmptyAfter;
			}
			
			if(EmptyAfter==null){
				Atlas.LastEmpty=EmptyBefore;
			}else{
				EmptyAfter.EmptyBefore=EmptyBefore;
			}
			
			int width=location.Width;
			int height=location.Height;
			Spacing=location.Spacing;
			int spacedWidth=width+Spacing;
			int spacedHeight=height+Spacing;
			
			if(Atlas.Mode==AtlasingMode.SmallestSpace){
				
				// Create the new empty zones:
				if(Width>width){
					// The textures a little thin.
					
					// Generate a new area to the right of this one (NB: 0,0 is bottom left)
					AtlasLocation newRight=new AtlasLocation(Atlas,X+spacedWidth,Y,Width-spacedWidth,height);
					// Immediately mark this as empty:
					newRight.AddToEmptySet();
				}
				
				if(Height>height){
					// The textures a little short.
					
					// Generate a new area above this one (NB: 0,0 is bottom left)
					AtlasLocation newTop=new AtlasLocation(Atlas,X,Y+spacedHeight,Width,Height-spacedHeight);
					// Immediately mark this as empty:
					newTop.AddToEmptySet();
				}
				
			}
			
			// Update atlas:
			location.Atlas=Atlas;
			
			// Move location so it acts like its in the same location as this:
			location.X=X;
			location.Y=Y;
			
			// Update the UV's:
			location.BakeUV();
			
			// Write it in:
			location.Flush();
		}
		
		/// <summary>Figures out where the UV's are.
		/// Note: stored as Min and Max because it's most useful for rendering and memory usage.</summary>
		private void BakeUV(){
			
			InvertedSizeX=InvertedSizeY=Atlas.InvertedSize;
			
			MinX=(float)X * InvertedSizeX;
			MinY=(float)Y * InvertedSizeY;
			
			float UVheight=(float)Height * InvertedSizeY;
			float UVwidth=(float)Width * InvertedSizeX;
			
			MaxX=UVwidth+MinX;
			MaxY=UVheight+MinY;
			
		}
		
		/// <summary>Gets the horizontal component of the UV co-ordinate of the given pixel.</summary>
		/// <param name="atPixel">The pixel on the atlas to use.</param>
		/// <returns>The value as a float from 0->1.</returns>
		public float GetU(float atPixel){
			if(atPixel<0f){
				atPixel=Width-((-atPixel)%Width);
			}else if(atPixel>Width){
				atPixel=(atPixel%Width);
			}
			// UVwidth/(float)Width => InvertedSize. +0.5 aims for the middle of the pixel.
			return MinX + (atPixel * InvertedSizeX);
		}
		
		/// <summary>Gets the vertical component of the UV co-ordinate of the given pixel.</summary>
		/// <param name="atPixel">The pixel on the atlas to use.</param>
		/// <returns>The value as a float from 0->1.</returns>
		public float GetV(float atPixel){
			if(atPixel<0f){
				atPixel=Height-((-atPixel)%Height);
			}else if(atPixel>Height){
				atPixel=(atPixel%Height);
			}
			// UVheight/(float)Height => InvertedSize. +0.5 aims for the middle of the pixel.
			return MinY + (atPixel * InvertedSizeY);
		}
		
		/// <summary>The index in the atlas of the pixel in the bottom left corner of this location.</summary>
		/// <returns>The index in the atlas.</returns>
		public int BottomLeftPixel(){
			return (Atlas.Dimension*Y)+X;
		}
		
		/// <summary>How many pixels should be gained to go up one row of pixels in the global atlas.</summary>
		/// <returns>The number of pixels.</returns>
		public int RowPixelDelta(){
			return Atlas.Dimension-Width;
		}
		
		/// <summary>Converts a local pixel coordinate into a global atlas pixel index.</summary>
		/// <param name="x">The x coordinate of the pixel in pixels from the left edge.</param>
		/// <param name="y">The y coordinate of the pixel in pixels from the bottom edge.</param>
		/// <returns>The index in the atlas.</returns>
		public int AtlasIndex(int x,int y){
			// Converts local texture x/y to atlas pixel index.
			return BottomLeftPixel()+x+(y*Atlas.Dimension);
		}
		
		/// <summary>Writes the texture to the pixels of the atlas.</summary>
		public void Flush(){
			if(Empty){
				// Junk sector - ignore
				return;
			}
			
			bool result=Image.DrawToAtlas(Atlas,this);
			
			if(!result){
				return;
			}
			
			Atlas.PixelChange=true;
			Atlas.Stack.PixelChange=true;
			
		}
		
		public void AtlasChanged(){
			
			Atlas.PixelChange=true;
			Atlas.Stack.PixelChange=true;
			
		}
		
	}
	
}