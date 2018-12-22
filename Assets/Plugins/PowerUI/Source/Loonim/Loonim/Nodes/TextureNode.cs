using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace Loonim{
	
	/// <summary>
	/// A node in a texture generation graph.
	/// </summary>
	[Values.Preserve]
	public class TextureNode{
		
		/// <summary>Unique ID for this node in its parent tree.</summary>
		public int InstanceID;
		/// <summary>Inputs to this node. May be null.</summary>
		public TextureNode[] Sources;
		/// <summary>True when this node changes.
		/// Note that it does not get set to true if the image draw size changes.</summary>
		public bool AllocateRequired=true;
		
		
		/// <summary>Bakes the x axis of this node as a 1D image.</summary>
		public float[] Bake(int size){
			
			// Create set:
			float[] values=new float[size];
			
			// Bake into it:
			Bake(values);
			
			return values;
			
		}
		
		/// <summary>Bakes the x axis of this node as a 1D image.</summary>
		public void Bake(float[] values){
			
			// Grab the size:
			int size=values.Length;
			
			double delta=1.0 / (double)(size-1);
			double x=0.0;
			
			for(int i=0;i<size;i++){
				
				// Read:
				values[i]=(float)GetValue(x);
				
				// Advance x:
				x+=delta;
				
			}
			
		}
		
		/// <summary>Bakes the x axis of this node as a 1D image. Note that the red channel is the most important.</summary>
		public void Bake(Color[] values){
			
			// Grab the size:
			int size=values.Length;
			
			double delta=1.0 / (double)(size-1);
			double x=0.0;
			
			for(int i=0;i<size;i++){
				
				// Read:
				values[i]=GetColour(x,0f);
				
				// Advance x:
				x+=delta;
				
			}
			
		}
		
		public override string ToString(){
			return ToString(0);
		}
		
		public string ToString(int depth){
			
			string res="";
			
			for(int t=0;t<depth;t++){
				res+="\t";
			}
			
			res+="<"+GetType()+">\r\n";
			
			if(Sources!=null){
				
				for(int i=0;i<Sources.Length;i++){
					
					if(Sources[i]==null){
						res+="[NULL SOURCE]\r\n";
					}else{
						res+=Sources[i].ToString(depth+1)+"\r\n";
					}
					
				}
				
			}
			
			for(int t=0;t<depth;t++){
				res+="\t";
			}
			
			res+="</"+GetType()+">\r\n";
			
			return res;
			
		}
		
		public List<Material> CollectMaterials(){
			
			List<Material> set=new List<Material>();
			CollectMaterials(set);
			return set;
			
		}
		
		public void CollectMaterials(List<Material> into){
			
			MaterialStackNode msn=(DrawStore as MaterialStackNode);
			
			if(msn!=null){
				// Add my material:
				into.Add(msn.Material);
			}
			
			if(Sources!=null){
				
				// For each source..
				for(int i=0;i<Sources.Length;i++){
					
					// Collect its materials too:
					Sources[i].CollectMaterials(into);
					
				}
				
			}
			
			
		}
		
		/// <summary>Adds a new source to this node.</summary>
		public void AddSource(TextureNode source){
			
			if(Sources==null){
				Sources=new TextureNode[]{source};
				return;
			}
			
			TextureNode[] newSet=new TextureNode[Sources.Length+1];
			
			System.Array.Copy(Sources,0,newSet,0,Sources.Length);
			
			newSet[Sources.Length]=source;
			
			Sources=newSet;
			
		}
		
		/// <summary>First input to this node. Errors if it doesn't exist.</summary>
		public TextureNode SourceModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		/// <summary>First input to this node. Errors if it doesn't exist.</summary>
		public TextureNode SourceModule1{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		/// <summary>Second input to this node. Errors if it doesn't exist.</summary>
		public TextureNode SourceModule2{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>Third input to this node. Errors if it doesn't exist.</summary>
		public TextureNode SourceModule3{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode():this(0){}
		
		/// <summary>Create a texture node with the given number of inputs.</summary>
		public TextureNode(int sourceCount){
			
			if(sourceCount==0){
				return;
			}
			
			Sources=new TextureNode[sourceCount];
			
		}
		
		/// <summary>By default, materials are named Loonim/Texture_node_id, however some nodes have "sub-materials"
		/// where they essentially have a bunch of different shaders. An example is the Blend node.</summary>
		public virtual int SubMaterialID{
			get{
				return -1;
			}
		}
		
		/// <summary>By default, materials are named Loonim/Texture_node_id. This allows nodes to essentially share shaders.</summary>
		public virtual int MaterialID{
			get{
				return TypeID;
			}
		}
		
		#if !NO_BLADE_RUNTIME
		
		public DrawStackNode DrawStore;
		
		/// <summary>Gets a shader of the given node ID and it's sub-type.</summary>
		protected static Shader GetShader(int type,int subType){
			
			string name;
			
			if(subType==-1){
				name="Loonim/"+type;
			}else{
				name="Loonim/"+type+"-"+subType;
			}
			
			// Make:
			return Shader.Find(name);
			
		}
		
		/// <summary>Called when this nodes SubMaterialID has changed.</summary>
		public void SubMaterialChanged(){
			
			// Get the stack node:
			MaterialStackNode matNode=DrawStore as MaterialStackNode;
			
			if(matNode!=null){
				
				// Update the shader now:
				matNode.Material.shader=GetShader(TypeID,SubMaterialID);
			}
			
		}
		
		/// <summary>Gets a sub-material of the given node ID.</summary>
		protected static Material GetMaterial(int type,int subType){
			return new Material(GetShader(type,subType));
		}
		
		/// <summary>The dimensions of the image that gets output from this node.
		/// A constant has 0 dimensions, a graph has 1 dimension and your ordinary
		/// image node has 2. Note that they vary based on inputs; for example an ADD
		/// node could add two graphs together (resulting in a 1D output) or it could
		/// add two images together (2D output).</summary>
		internal virtual int OutputDimensions{
			get{
				
				if(Sources==null){
					// Assume a 1D graph.
					return 1;
				}
				
				// Otherwise, get the largest of the input dimensions:
				// E.g. the add node adding two 1D graphs together.
				// The result should still be a 1D node.
				
				int largest=0;
				
				for(int i=0;i<Sources.Length;i++){
					
					int dim=Sources[i].OutputDimensions;
					
					if(dim>largest){
						largest=dim;
					}
					
					if(largest==2){
						break;
					}
					
				}
				
				return largest;
				
			}
		}
		
		/// <summary>Allocates GPU drawing meta now.</summary>
		public virtual void Draw(DrawInfo info){
			
			if(Sources!=null){
				
				for(int i=0;i<Sources.Length;i++){
					
					// Let the sources know we're the active target:
					info.CurrentParent=this;
					info.CurrentIndex=i;
					
					// Draw:
					TextureNode src=Sources[i];
					
					if(src!=null){
						src.Draw(info);
					}
					
				}
			}
			
			if(DrawStore!=null){
				DrawStore.Draw(info);
			}
			
		}
		
		protected void AllocateSources(Material material,DrawInfo info,SurfaceTexture tex,int targetStack,int count){
			
			int inputStacks=(targetStack==0)?1:0;
			
			for(int i=0;i<count;i++){
				
				// Get the input node:
				TextureNode input=Sources[i];
				
				// Allocate it now (must not allocate targetStack in the direct kids):
				
				DrawStackNode drawNode=input.Allocate(info,tex,ref inputStacks);
				
				if(inputStacks==targetStack){
					// Skip:
					inputStacks++;
				}
				
				// Apply it to our material:
				material.SetTexture("_Src"+i,drawNode.Texture);
				
			}
			
		}
		
		/// <summary>Allocates GPU drawing meta now.</summary>
		public virtual DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			// If the source is a constant or a graph
			// then we don't allocate a material for it.
			// We do, however, create a texture:
			
			// CONSTANT: 1 * 1 (Texture)
			// GRAPH   : info.ImageX * 1 (Texture)
			// ELSE    : info.ImageX * info.ImageY (Stack)
			
			int dimensions=OutputDimensions;
			
			if(dimensions==2){
				
				// Stack required.
				
				// Allocate a target stack now:
				int targetStack=stackID;
				DrawStack stack=tex.GetStack(targetStack,info);
				stackID++;
				
				// Create the material:
				Material material=GetMaterial(MaterialID,SubMaterialID);
				
				if(Sources!=null){
					
					int inputStacks=(targetStack==0)?1:0;
					
					for(int i=0;i<Sources.Length;i++){
						
						// Get the input node:
						TextureNode input=Sources[i];
						
						// Allocate it now (must not allocate targetStack in the direct kids):
						
						DrawStackNode drawNode=input.Allocate(info,tex,ref inputStacks);
						
						if(inputStacks==targetStack){
							// Skip:
							inputStacks++;
						}
						
						// Apply it to our material:
						material.SetTexture("_Src"+i,drawNode.Texture);
						
					}
					
				}
				
				// Create our node:
				MaterialStackNode matNode=DrawStore as MaterialStackNode;
				
				if(matNode==null){
					matNode=new MaterialStackNode();
					DrawStore=matNode;
					matNode.Mesh=info.Mesh;
				}
				
				matNode.Material=material;
				matNode.Stack=stack;
				
				return matNode;
				
			}
			
			// It'll be baked/ drawn by the CPU.
			
			// Get the width of the texture:
			int width=(dimensions==1)?info.ImageX : 1;
			
			TextureStackNode constNode=DrawStore as TextureStackNode;
			
			if(constNode==null){
				constNode=new TextureStackNode(this,info.HDR,width);
				DrawStore=constNode;
			}
			
			// Ok:
			return constNode;
			
		}
		
		/// <summary>Allocates now.</summary>
		public void PreAllocate(DrawInfo info,SurfaceTexture tex){
			
			// If the size has changed then require a stack (re)allocate:
			if(info.ImageX!=tex.LatestImageX || info.ImageY!=tex.LatestImageY){
				
				// Realloc textures:
				tex.ReallocateSize(info);
				
				// Must reallocate:
				AllocateRequired=true;
				
			}
			
			if(AllocateRequired){
				
				// Clear flag:
				AllocateRequired=false;
				
				// Allocate the first node:
				int stackCount=0;
				Allocate(info,tex,ref stackCount);
				
			}
			
		}
		
		/// <summary>Draws this node now (but doesn't transfer it into a texture).</summary>
		public void DrawGPU(DrawInfo info,SurfaceTexture tex){
			
			// Get current active RT (as draw will probably overwrite it):
			RenderTexture prevActive=RenderTexture.active;
			
			// If the size has changed then require a stack (re)allocate:
			if(info.ImageX!=tex.LatestImageX || info.ImageY!=tex.LatestImageY){
				
				// Realloc textures:
				tex.ReallocateSize(info);
				
				// Must reallocate:
				AllocateRequired=true;
				
			}
			
			if(AllocateRequired){
				
				// Clear flag:
				AllocateRequired=false;
				
				// Allocate the first node:
				int stackCount=0;
				Allocate(info,tex,ref stackCount);
				
			}
			
			// Draw now!
			Draw(info);
			info.CurrentParent=null;
			
			// Restore active:
			RenderTexture.active=prevActive;
			
		}
		
		/// <summary>Draws this node into the given texture and pixel buffer now.
		/// Note that the pixels buffer might not get used, so initially pass in null and cache the resulting one if you wish.</summary>
		public void Draw(DrawInfo info,SurfaceTexture tex,Texture2D image,ref Color[] pixels){
			
			// Max # of pixels:
			int max=info.ImageX * info.ImageY;
			
			// Length test:
			if(pixels!=null && pixels.Length!=max){
				
				pixels=null;
				
			}
			
			if(info.Mode==SurfaceDrawMode.GPU){
				
				// Get current active RT (as draw will probably overwrite it):
				RenderTexture prevActive=RenderTexture.active;
				
				if(AllocateRequired){
					
					// Clear flag:
					AllocateRequired=false;
					
					// Allocate the first node:
					int stackCount=0;
					Allocate(info,tex,ref stackCount);
					
				}
				
				// Draw now!
				Draw(info);
				info.CurrentParent=null;
				
				// Layers.DrawStore.Texture now contains our resulting image.
				// Transfer it to our Texture2D:
				DrawStore.WriteTo(image,info,ref pixels);
				
				// Restore active:
				RenderTexture.active=prevActive;
				
			}else{
				
				// Prepare:
				Prepare(info);
				
				if(pixels==null){
					
					// Create:
					pixels=new Color[max];
					
				}
				
				// Basic CPU render:
				DrawCPU(info,pixels);
				
				if(image!=null){
					
					image.SetPixels(pixels);
					image.Apply();
					
				}
				
			}
			
		}
		
		#endif
		
		/// <summary>Draws this node using the CPU.</summary>
		public void DrawCPU(DrawInfo info,Color[] pixels){
			
			int index=0;
			float fx=0f;
			float fy=0f;
			int maxY=info.ImageY;
			int maxX=info.ImageX;
			float deltaFX=info.DeltaX;
			float deltaFY=info.DeltaY;
			
			for(int y=0;y<maxY;y++){
				
				for(int x=0;x<maxX;x++){
					
					// Get the reading:
					pixels[index]=GetColour(fx,fy);
					
					// Map to pixel:
					index++;
					
					fx+=deltaFX;
					
				}
				
				fy+=deltaFY;
				fx=0f;
				
			}
			
		}
		
		/// <summary>Gets the node with the given ID.</summary>
		public TextureNode GetLayer(int id,int count){
			
			if(id==count){
				return this;
			}
			
			if(Sources==null){
				return null;
			}
			
			for(int i=0;i<Sources.Length;i++){
				
				count++;
				TextureNode result=Sources[i].GetLayer(id,count);
				
				if(result!=null){
					return result;
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Gets the node ready to draw.</summary>
		public virtual void Prepare(DrawInfo info){
			
			if(Sources==null){
				return;
			}
			
			for(int i=0;i<Sources.Length;i++){
				Sources[i].Prepare(info);
			}
			
		}
		
		/// <summary>Reads sources from the given reader into this node.</summary>
		public virtual void Read(TextureReader reader){
			
			// Source count:
			int sourceCount=(int)reader.ReadCompressed();
			
			// For each one..
			for(int i=0;i<sourceCount;i++){
				
				
				// Read it:
				Sources[i]=reader.ReadTextureNode();
				
			}
			
		}
		
		#if BLADE_WRITE
		
		/// <summary>Writes the node to the given writer.</summary>
		public void Write(BinaryIO.Writer writer){
			
			// Write the type:
			writer.WriteCompressed((uint)TypeID);
			
			// Write source count:
			writer.WriteCompressed((uint)Sources.Length);
			
			for(int i=0;i<Sources.Length;i++){
				
				// Write it too:
				Sources[i].Write(writer);
				
			}
			
		}
		
		#endif
		
		/// <summary>Gets a wrapped value from this node. Range of +/- 1.</summary>
		public virtual double GetWrapped(double x, double y, int wrap){
			return GetValue(x,y);
		}
		
		/// <summary>Gets the value from this node. This is required at a minimum. Range of +/- 1.</summary>
        public virtual double GetValue(double x, double y, double z){
			return GetValue(x,y);
		}
		
		/// <summary>Gets the value from this node. This is required at a minimum. Range of +/- 1.</summary>
        public virtual double GetValue(double x, double y){
			return 0;
		}
		
		/// <summary>Gets the 1D value from this node. Equiv to GetValue(t,0). Required for all graph nodes.</summary>
		public virtual double GetValue(double t){
			return GetValue(t,0);
		}
		
		/// <summary>Gets this nodes colour output.</summary>
		public virtual UnityEngine.Color GetColour(double x,double y){
			
			// Read and repeat:
			float b=(float)GetValue(x,y);
			
			return new Color(b,b,b,1f);
			
		}
		
		/// <summary>A globally constant ID for a given type of module.</summary>
		public virtual int TypeID{
			get{
				return -1;
			}
		}
		
	}
	
}
