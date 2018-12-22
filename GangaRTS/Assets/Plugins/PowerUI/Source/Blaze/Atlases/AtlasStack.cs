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
	/// A stack of atlases. All graphics from all your UI's are grouped together into one of two stacks like this.
	/// </summary>
	
	public class AtlasStack{
		
		/// <summary>Spacing around entries on the atlases.</summary>
		public int Spacing;
		/// <summary>The mode in which the atlases on this stack allocate images.</summary>
		public AtlasingMode Mode=AtlasingMode.SmallestSpace;
		/// <summary>True if this atlas stack cannot change filter mode.</summary>
		public bool FilterLocked;
		/// <summary>True if any pixels in any atlases changed.</summary>
		public bool PixelChange;
		/// <summary>The last atlas on the stack. This is the top.</summary>
		public TextureAtlas Last;
		/// <summary>The first atlas on the stack. This is the bottom.</summary>
		public TextureAtlas First;
		/// <summary>The initial size of atlases in this stack.</summary>
		public int InitialSize;
		/// <summary>Is this a CPU atlas set?</summary>
		public bool CPUAccess=true;
		/// <summary>Set true when an image on this stack wants to optimize.</summary>
		internal bool OptimizeRequested;
		/// <summary>The filter mode of all atlases in this stack. See FilterMode.</summary>
		private FilterMode FilteringMode=FilterMode.Point;
		/// <summary>The format of the atlases in this stack.</summary>
		public TextureFormat Format=TextureFormat.ARGB32;
		/// <summary>A map of current textures to their location on the atlas.</summary>
		public Dictionary<int,AtlasLocation> ActiveImages;
		
		
		public AtlasStack(int initialSize){
			InitialSize=initialSize;
			
			// Setup images:
			ActiveImages=new Dictionary<int,AtlasLocation>();
			
		}
		
		
		public AtlasStack(TextureFormat format,int initialSize):this(initialSize){
			Format=format;
		}
		
		/// <summary>Require the given image on any atlas. Note that this may reject the requirement if the image is too big and isn't worthwhile on an atlas.</summary>
		public AtlasLocation RequireImage(AtlasEntity image){
			
			int entityID=image.GetAtlasID();
			
			AtlasLocation result;
			if(ActiveImages.TryGetValue(entityID,out result)){
				// Most calls fall through here.
				return result;
			}
			
			int width;
			int height;
			
			image.GetDimensionsOnAtlas(out width,out height);
			
			if(width>InitialSize || height>InitialSize){
				// Won't fit or is unsuitable for atlasing anyway.
				return null;
			}
			
			if(Last==null){
				Create();
			}else{
				
				// Fast check - was this texture recently removed from any atlas?
				// We might have the chance of restoring it.
				// Their added at the back of the empty queue, so naturally, start at the end of the empty set
				// and go back until we hit one with a null texture.
				
				TextureAtlas currentAtlas=Last;
				
				while(currentAtlas!=null){
					
					AtlasLocation currentE=currentAtlas.LastEmpty;
					
					while(currentE!=null){
						
						if(currentE.Image==null){
							// Nope! Shame.
							break;
						}else if(currentE.AtlasID==entityID){
							// Ace! Time to bring it back from the dead.
							currentE.Select(image,width,height,Spacing);
							ActiveImages[entityID]=currentE;
							
							return currentE;
						}
						
						currentE=currentE.EmptyBefore;
					}
					
					currentAtlas=currentAtlas.Previous;
				}
				
			}
			
			// Push to top of stack:
			result=Last.Add(image,entityID,width,height);
			
			if(result!=null){
				return result;
			}
			
			// Non-fitter - try fitting in lower stack frames:
			
			TextureAtlas current=Last.Previous;
			
			while(current!=null){
				
				result=current.Add(image,entityID,width,height);
				
				if(result!=null){
					return result;
				}
				
				current=current.Previous;
			}
			
			// Still not fitting! Create a new stack frame:
			Create();
			
			return Last.Add(image,entityID,width,height);
			
		}
		
		/// <summary>Optimises the frames on this stack if it's needed.</summary>
		public bool OptimiseIfNeeded(){
			
			if(!OptimizeRequested){
				return false;
			}
			
			OptimizeRequested=false;
			
			TextureAtlas requiresOptimise=null;
			TextureAtlas current=First;
			
			while(current!=null){
				
				if(current.OptimizeRequested){
					// Pop it out:
					current.RemoveFromStack();
					
					// And add it to our temp stack:
					current.Next=requiresOptimise;
					requiresOptimise=current;
					
				}
				
				current=current.Next;
				
			}
			
			if(requiresOptimise==null){
				return false;
			}
		
			Dictionary<int,AtlasLocation> allImages=ActiveImages;
			
			// Next, for each one..
			current=requiresOptimise;
			
			while(current!=null){
				
				// Grab the next one:
				TextureAtlas next=current.Next;
				
				// Offload its images into the "remaining" stack. Note that we must retain the actual Location objects so we don't have to re-request/ re-generate all of them.
				// If none fit, we re-add current as the new top of stack and add the images back onto it.
				
				// Have we re-added current?
				bool added=false;
				
				// Next up, add them all back in, and that's it!
				// The optimizing comes from them trying to fit in the smallest possible gap they can when added.
				foreach(KeyValuePair<int,AtlasLocation> kvp in allImages){
					AtlasLocation location=kvp.Value;
					
					if(location.Atlas==current){
						
						// Try adding to the stack:
						TextureAtlas stackAtlas=Last;
						
						bool noAdd=true;
						
						while(stackAtlas!=null){
							
							// Try adding to the current frame:
							if(stackAtlas.OptimiseAdd(location)){
								noAdd=false;
								break;
							}
							
							stackAtlas=stackAtlas.Previous;
						}
						
						if(noAdd && !added){
							added=true;
							
							// Didn't fit in any of them! We now clear out the current atlas and re-add it like so:
							Add(current);
							
							// Ensure it's cleared:
							current.Reset();
							
							// Add to it instead:
							current.OptimiseAdd(location);
							
						}
						
					}
					
				}
				
				if(!added){
					// Destroy it:
					current.Destroy();
				}
				
				current=next;
			}
			
			return true;
		}
		
		/// <summary>How many atlases are on this stack.</summary>
		public int Count{
			get{
				TextureAtlas current=First;
				int count=0;
				
				while(current!=null){
					count++;
					current=current.Next;
					
				}
				
				return count;
			}
		}
		
		public void LockFilterMode(FilterMode mode){
			FilteringMode=mode;
			FilterLocked=true;
		}
		
		/// <summary>Empties this atlas stack.</summary>
		public void Clear(){
			Last=null;
			First=null;
			// Clear all actives:
			ActiveImages.Clear();
		}
		
		/// <summary>Flushes the atlases that require it.</summary>
		public void Flush(){
			
			if(!PixelChange){
				return;
			}
			
			PixelChange=false;
			
			TextureAtlas current=First;
			
			while(current!=null){
				
				current.Flush();
				current=current.Next;
				
			}
			
		}
		
		/// <summary>Removes a texture from the atlas.</summary>
		/// <param name="texture">The texture to remove.</param>
		public void Remove(AtlasEntity texture){
			if(texture==null){
				return;
			}
			
			AtlasLocation location=Get(texture.GetAtlasID());
			
			if(location==null){
				return;
			}
			
			// Make the location available:
			location.Deselect();
		}
		
		/// <summary>Gets the location of the given image on any atlases in this stack.</summary>
		/// <param name="entityID">The entity ID to find.</param>
		/// <returns>The location on an atlas of the image. Null if it wasn't found.</returns>
		public AtlasLocation Get(int entityID){
			AtlasLocation result;
			ActiveImages.TryGetValue(entityID,out result);
			return result;
		}
		
		/// <summary>Adds the given atlas to the top of this stack.</summary>
		public void Add(TextureAtlas atlas){
			
			atlas.Next=null;
			atlas.Stack=this;
			
			if(First==null){
				atlas.Previous=null;
				First=Last=atlas;
				return;
			}
			
			atlas.Previous=Last;
			Last=Last.Next=atlas;
			
		}
		
		public FilterMode FilterMode{
			get{
				return FilteringMode;
			}
			set{
				if(FilterLocked){
					return;
				}
				
				if(value==FilteringMode){
					return;
				}
				
				FilteringMode=value;
				
				TextureAtlas current=First;
				
				while(current!=null){
					
					current.FilterMode=value;
					current=current.Next;
					
				}
				
			}
		}
		
		public TextureAtlas Create(){
			
			// Create the atlas:
			TextureAtlas atlas=new TextureAtlas(InitialSize,FilteringMode,Format,CPUAccess);
			atlas.Spacing=Spacing;
			atlas.Mode=Mode;
			Add(atlas);
			
			return atlas;
			
		}
		
		/// <summary>Gets the texture atlas at the top of the stack. Creates it if the stack is empty.</summary>
		public TextureAtlas Top{
			get{
				if(Last==null){
					return Create();
				}
				
				return Last;
			}
		}
		
	}
	
}