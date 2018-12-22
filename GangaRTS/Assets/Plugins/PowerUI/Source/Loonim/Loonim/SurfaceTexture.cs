using System;
using Blaze;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Loonim{
	
	/// <summary>
	/// A texture which can be used to describe a surface.
	/// </summary>
	
	public class SurfaceTexture{
		
		/// <summary>True if this texture has changed in some way and it needs to be redrawn.
		/// Note! If you have RenderTexture's as inputs then this will be wrong
		/// (i.e. it doesn't know that the source properties are always changing).</summary>
		public bool Changed;
		/// <summary>Noise describing the surface.</summary>
		private TextureNode Root_;
		/// <summary>The raw set of properties if there are any.</summary>
		public SurfaceProperty[] Properties;
		/// <summary>Draw using the GPU as often as possible.</summary>
		public SurfaceDrawMode Mode=SurfaceDrawMode.GPU;
		/// <summary>A map of properties. Created if a property is requested by name. See this[string].</summary>
		public Dictionary<string,SurfaceProperty> PropertyMap;
		
		#if !NO_BLADE_RUNTIME
		/// <summmary>The current set of stacks.</summary>
		public List<DrawStack> Stacks=new List<DrawStack>();
		
		
		/// <summary>Gets the stack with the given index, or creates it.</summary>
		public DrawStack GetStack(int id,DrawInfo info){
			
			// Need to create it? Note that ID will only ever be equal to stack count.
			if(Stacks.Count==id){
				
				// Create it:
				DrawStack ds=new DrawStack(info);
				
				// Add it:
				Stacks.Add(ds);
				
				return ds;
			}
			
			// Get it:
			return Stacks[id];
			
		}
		
		#endif
		
		/// <summary>Reallocates all textures in the stacks.</summary>
		public void ReallocateSize(DrawInfo info){
			LatestImageX=info.ImageX;
			LatestImageY=info.ImageY;
			
			#if !NO_BLADE_RUNTIME
			
			// Tidy up the draw stacks:
			for(int i=0;i<Stacks.Count;i++){
				
				// Allocate now:
				Stacks[i].Allocate(info);
				
			}
			
			#endif
		}
		
		/// <summary>Clears out this texture destroying all internal textures.</summary>
		public void Clear(){
			
			#if !NO_BLADE_RUNTIME
			
			// Tidy up the draw stacks:
			for(int i=0;i<Stacks.Count;i++){
				
				Stacks[i].Destroy();
				
			}
			
			// Clear it:
			Stacks.Clear();
			
			#endif
			
			Changed=true;
			
		}
		
		/// <summary>The root node of the graph.</summary>
		public TextureNode Root{
			get{
				return Root_;
			}
			set{
				Root_=value;
				Changed=true;
			}
		}
		
		public SurfaceTexture(){}
		
		public void Load(byte[] data){
			
			// Create the reader:
			TextureReader reader=new TextureReader(data);
			
			// Magic number (ASCII 'LIM'):
			if(reader.ReadByte()!='L' || reader.ReadByte()!='I' || reader.ReadByte()!='M'){
				
				throw new Exception("That's not a native Loonim 'lim' file.");
				
			}
			
			// Version and any flags (should just be 0):
			ulong vflags=reader.ReadCompressed();
			
			if(vflags!=0){
				
				throw new Exception("Tried to load a newer Loonim file than this loader can handle.");
				
			}
			
			// Load properties:
			LoadProperties(reader);
			
			// Load the root:
			Root_=reader.ReadTextureNode();
			
		}
		
		#if BLADE_WRITE
		
		public byte[] Save(){
			return Save(true);
		}
		
		public byte[] Save(bool withNiceNames){
			
			// Create the writer:
			BinaryIO.Writer writer=new BinaryIO.Writer();
			
			// Magic number:
			writer.Write('L');
			writer.Write('I');
			writer.Write('M');
			
			// Loader version and flags:
			writer.WriteCompressed(0);
			
			// Save properties:
			SaveProperties(writer,withNiceNames,true);
			
			// Save the root:
			Root_.Write(writer);
			
			// Ok!
			return writer.GetResult();
			
		}
		
		/// <summary>Saves the property set to the given writer.</summary>
		public void SaveProperties(BinaryIO.Writer writer,bool withNiceNames,bool withValues){
			
			// Write property count and extra flags:
			int count=Properties==null?0:Properties.Length;
			
			uint flags=0;
			
			if(withNiceNames){
				flags|=1;
			}
			
			if(withValues){
				flags|=2;
			}
			
			// Write count and flags now:
			writer.WriteCompressed((((uint)count)<<3) | flags);
			
			// For each property..
			for(int i=0;i<count;i++){
				
				// Get:
				SurfaceProperty sp=Properties[i];
				
				if(withNiceNames){
					
					// Write the nice name:
					// (Optional because properties ref by index in the actual graph).
					writer.WriteString(sp.Name);
					
				}
				
				if(withValues){
					
					// Write the value:
					sp.Value.Write(writer);
					
				}
				
			}
			
		}
		
		#endif
		
		/// <summary>Loads the set of properties from the given reader.</summary>
		public void LoadProperties(TextureReader reader){
			
			// Note that each property has an optional name and optional default value.
			// (However, they serve as 'containers' which materials drop the current values into).
			
			// How many properties? Also contains 3 flags for names, defaults and unused.
			uint propertiesAndFlags=(uint)reader.ReadCompressed();
			
			// Chop off the 3 bits:
			int properties=(int)(propertiesAndFlags>>3);
			
			if(properties==0){
				return;
			}
			
			// Got names?
			// Got any defaults at all?
			bool gotNames=((propertiesAndFlags&1)==1);
			bool gotDefaults=((propertiesAndFlags&2)==2);
			
			// Create the set:
			Properties=new SurfaceProperty[properties];
			
			// For each one..
			for(int i=0;i<properties;i++){
				
				// Create:
				SurfaceProperty prop=new SurfaceProperty();
				
				// Apply ID:
				prop.ID=i;
				
				if(gotNames){
					
					// Read a nice name:
					prop.Name=reader.ReadString();
					
				}
				
				if(gotDefaults){
					
					// Read a value:
					prop.Value=reader.ReadPropertyValue();
					
				}
				
				// Add to set:
				Properties[i]=prop;
				
			}
			
		}
		
		/// <summary>Convenience function. It also helps to cache the original property value and straight edit that.</summary>
		public void Set(string property,Texture value){
			
			// Get the property:
			SurfaceProperty prop=GetProperty(property,true);
			
			// Get as tex:
			Values.TextureValue v=prop.Value as Values.TextureValue;
			
			// Set it:
			if(v==null){
				prop.Value=new Values.TextureValue(value);
			}else{
				v.Value=value;
				
				if(v.Changed){
					// No longer idling - changes detected.
					Changed=true;
				}
			}
			
		}
		
		/// <summary>Convenience function. It also helps to cache the original property value and straight edit that.</summary>
		public void Set(string property,Color value){
			
			// Get the property:
			SurfaceProperty prop=GetProperty(property,true);
			
			// Get as col:
			Values.ColourValue v=prop.Value as Values.ColourValue;
			
			// Set it:
			if(v==null){
				prop.Value=new Values.ColourValue(value);
			}else{
				v.Value=value;
				
				if(v.Changed){
					// No longer idling - changes detected.
					Changed=true;
				}
			}
			
		}
		
		/// <summary>Convenience function. It also helps to cache the original property value and straight edit that.</summary>
		public void Set(string property,float value){
			
			// Get the property:
			SurfaceProperty prop=GetProperty(property,true);
			
			// Get as float:
			Values.FloatValue v=prop.Value as Values.FloatValue;
			
			// Set it:
			if(v==null){
				prop.Value=new Values.FloatValue(value);
			}else{
				v.Value=value;
				
				if(v.Changed){
					// No longer idling - changes detected.
					Changed=true;
				}
			}
			
		}
		
		/// <summary>Converts an image (usually a RenderTexture) to a Texture2D. Optionally the resulting texture is HDR.</summary>
		public static Texture2D ToTexture2D(Texture img,bool hdr){
			
			if(img is Texture2D){
				return img as Texture2D;
			}
			
			// Get render texture:
			RenderTexture rt=img as RenderTexture;
			
			if(rt==null){
				throw new NotSupportedException("Must be a RenderTexture.");
			}
			
			// Create:
			Texture2D image=new Texture2D(rt.width,rt.height,hdr?TextureFormat.RGBAFloat : TextureFormat.ARGB32,false);
			
			// Copy:
			CopyTo(image,rt);
			
			// Ok!
			return image;
			
		}
		
		/// <summary>Copies a render texture into the given image.</summary>
		public static void CopyTo(Texture2D image,RenderTexture rt){
			
			int w=rt.width;
			int h=rt.height;
			
			// Cache active:
			RenderTexture prevActive=RenderTexture.active;
			
			// Apply and set:
			RenderTexture.active=rt;
			
			// Read pixels:
			image.ReadPixels(new Rect(0,0,w,h),0,0);
			image.Apply();
			
			// Restore:
			RenderTexture.active=prevActive;
			
		}
		
		public TextureNode getElementByInstanceId(int layerID){
			
			if(layerID==0){
				return null;
			}
			
			// Go up through the nodes looking for the one we want.
			return Root_.GetLayer(layerID,1);
			
		}
		
		/// <summary>Renders as a static image (albedo only, not HDR). Please don't spam me! It creates a texture and draw meta, so..</summary>
		public Texture2D DrawAlbedo(int size){
			return DrawAlbedo(size,false);
		}
		
		/// <summary>Always draws even if properties haven't changed. Typically responds with a RenderTexture 
		/// but it depends on your tree (i.e. if your tree is just a constant, then you'll actually get a Texture2D back).</summary> 
		public Texture ForceDraw(DrawInfo settings){
			
			// Draw the root now (which may also trigger the allocate):
			Root_.DrawGPU(settings,this);
			
			// Pull the resulting texture from root and return it:
			return Root_.DrawStore.Texture;
			
		}
		
		/// <summary>The image X/Y dimensions that the image was last drawn at.</summary>
		internal int LatestImageX=-1;
		internal int LatestImageY=-1;
		
		/// <summary>The fastest of all the drawing approaches. Typically responds with a RenderTexture 
		/// but it depends on your tree (i.e. if your tree is just a constant, then you'll actually get a Texture2D back).</summary> 
		public Texture Draw(DrawInfo settings){
			
			if(Root_==null){
				return null;
			}
			
			if(Changed || Root_.DrawStore==null){
				// Draw the root now (which may also trigger the allocate):
				Changed=false;
				Root_.DrawGPU(settings,this);
			}
			
			// Pull the resulting texture from root and return it:
			return Root_.DrawStore.Texture;
			
		}
		
		/// <summary>PreAllocates this texture.</summary>
		public void PreAllocate(DrawInfo info){
			
			if(Root==null){
				return;
			}
			
			Root.PreAllocate(info,this);
		}
		
		/// <summary>The output texture. Available after PreAllocate or Draw has been called.</summary>
		public Texture Texture{
			get{
				if(Root_==null){
					return null;
				}
				
				return Root_.DrawStore.Texture;
			}
		}
		
		/// <summary>Renders as a static image (albedo only, optionally HDR).
		/// Please don't spam me! It allocates a texture, draw meta and probably a pixel buffer.</summary>
		public Texture2D DrawAlbedo(int size,bool hdr){
			
			Changed=false;
			
			// Create:
			Texture2D albedo=new Texture2D(size,size,hdr?TextureFormat.RGBAFloat : TextureFormat.ARGB32,false);
			
			// Create draw info:
			DrawInfo settings=new DrawInfo(size,Mode);
			
			// Draw now:
			Color[] pixels=null;
			Root_.Draw(settings,this,albedo,ref pixels);
			
			return albedo;
			
		}
		
		/// <summary>Gets/sets a property by its nice name.</summary>
		public SurfaceProperty this[int property]{
			get{
				
				if(Properties==null){
					return null;
				}
				
				return Properties[property];
				
			}
			set{
				
				if(Properties==null){
					return;
				}
				
				Properties[property]=value;
				
				if(PropertyMap!=null){
					
					// Update map:
					PropertyMap[value.Name]=value;
					
				}
				
			}
		}
		
		/// <summary>Builds or rebuilds the property map.</summary>
		public void CreateMap(){
			
			if(Properties==null){
				PropertyMap=null;
				return;
			}
			
			// Create:
			PropertyMap=new Dictionary<string,SurfaceProperty>();
			
			// For each property..
			for(int i=0;i<Properties.Length;i++){
				
				// Get property:
				SurfaceProperty prop=Properties[i];
				
				// Got a name at all?
				if(string.IsNullOrEmpty(prop.Name)){
					
					// Nope - skip.
					continue;
					
				}
				
				// Push:
				PropertyMap[prop.Name]=prop;
				
			}
			
		}
		
		/// <summary>Removes the given property by its nice name.</summary>
		public void RemoveProperty(string property){
			
			property=property.ToLower();
			
			// Exists?
			SurfaceProperty exists=GetProperty(property,false);
			
			if(exists==null || Properties==null){
				return;
			}
			
			// One property?
			if(exists.ID==0 && Properties.Length==1){
				Properties=null;
				PropertyMap=null;
			}else{
				
				// Slice from the array at exists.ID
				SurfaceProperty[] newSet=new SurfaceProperty[Properties.Length-1];
				
				int index=0;
				
				for(int i=0;i<Properties.Length;i++){
					
					if(i==exists.ID){
						// Skip the one we're removing.
						continue;
					}
					
					SurfaceProperty movingProp=Properties[i];
					movingProp.ID=index;
					newSet[index]=movingProp;
					index++;
				}
				
				if(PropertyMap!=null){
					
					// Update map:
					PropertyMap.Remove(property);
					
				}
				
			}
			
		}
		
		/// <summary>Sets the named property.</summary>
		public void SetProperty(string property,Values.PropertyValue value){
			
			// Get or create:
			SurfaceProperty prop=GetProperty(property,true);
			
			// Apply value:
			prop.Value=value;
			
		}
		
		/// <summary>Gets the named property by nice name, optionally creating it.</summary>
		public SurfaceProperty GetProperty(string property,bool create){
			
			property=property.ToLower();
			
			// Already exists?
			SurfaceProperty result=null;
			
			if(Properties!=null){
				
				if(PropertyMap==null){
					CreateMap();
				}
				
				PropertyMap.TryGetValue(property,out result);
				
			}
			
			if(result!=null || !create){
				return result;
			}
			
			// Create now:
			result=new SurfaceProperty();
			result.Name=property;
			
			// Insert:
			if(Properties==null){
				Properties=new SurfaceProperty[1];
			}else{
				// Resize:
				SurfaceProperty[] set=new SurfaceProperty[Properties.Length+1];
				Array.Copy(Properties,0,set,0,Properties.Length);
				Properties=set;
			}
			
			// Add:
			result.ID=Properties.Length-1;
			Properties[result.ID]=result;
			
			if(PropertyMap!=null){
				
				// Update map:
				PropertyMap[property]=result;
				
			}
			
			return result;
			
		}
		
		/// <summary>Gets a property value by its nice name. Note that nice names are generally rare. Null if it doesn't exist.</summary>
		public Values.PropertyValue this[string property]{
			get{
				SurfaceProperty sp=GetProperty(property,false);
				
				if(sp==null){
					return null;
				}
				
				return sp.Value;
			}
			set{
				SetProperty(property,value);
			}
		}
		
	}
	
}