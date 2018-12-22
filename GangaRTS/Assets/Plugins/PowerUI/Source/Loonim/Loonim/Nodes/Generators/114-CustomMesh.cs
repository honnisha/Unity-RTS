//--------------------------------------
//	   Loonim Image Generator
//	Partly derived from LibNoise
//	See License.txt for more info
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using Blaze;
using System.Collections;
using System.Collections.Generic;


namespace Loonim{
	
	/// <summary>
	/// Draws a custom 2D mesh with a given fill.
	/// Note that these work best with the Stack node (because a Stack node clears the target).
	/// If you don't use a stack node, then it will draw on top of whatever is already in the target.
	/// The path ranges from +1 to -1 on x and y.
	/// You must consider OpenGL vs Direct3D by multiplying your Y coordinates by DrawInfo.InvertY.
	/// </summary>
	
	public class CustomMesh: TextureNode{
		
		/// <summary>The default triangulation accuracy in terms of 0-1 (screen space).</summary>
		public const float DefaultAccuracy=0.05f;
		/// <summary>Shared triangulator for these paths. Created on demand.</summary>
		private static Triangulator Triangulator;
		
		/// <summary>True if the user directly set a mesh.</summary>
		public bool MeshSet=false;
		/// <summary>Triangulation accuracy in terms of 0-1 in screen space.</summary>
		public float Accuracy=DefaultAccuracy;
		/// <summary>The path in use. Ranges from +1 to -1 on x and y.</summary>
		private VectorPath Path_;
		
		/// <summary>The path in use. Ranges from +1 to -1 on x and y.</summary>
		public VectorPath Path{
			get{
				return Path_;
			}
			set{
				Path_=value;
				Refresh();
			}
		}
		
		#if !NO_BLADE_RUNTIME
		/// <summary>The computed mesh to display. The path ranges from +1 to -1 on x and y.
		/// You must consider OpenGL vs Direct3D by multiplying your Y coordinates by DrawInfo.InvertY.</summary>
		public Mesh Mesh{
			get{
				return Mesh_;
			}
			set{
				MeshSet=value!=null;
				Mesh_=value;
				value.UploadMeshData(true);
			}
		}
		
		/// <summary>The computed mesh to display. The path ranges from +1 to -1 on x and y.
		/// You must consider OpenGL vs Direct3D by multiplying your Y coordinates by DrawInfo.InvertY.</summary>
		private Mesh Mesh_;
		#endif
		
		
		public CustomMesh():base(1){}
		
		public CustomMesh(TextureNode fill,Mesh mesh):base(1){
			SourceModule=fill;
			Mesh=mesh;
		}
		
		public CustomMesh(TextureNode fill,VectorPath path):base(1){
			SourceModule=fill;
			Path=path;
		}
		
		/// <summary>Updates the internal triangulated version of the path. Occurs whenever Path is set.</summary>
		public void Refresh(){
			
			#if !NO_BLADE_RUNTIME
			if(MeshSet){
				// Do nothing.
				return;
			}
			
			if(Path_==null){
				Mesh_=null;
				return;
			}
			
			if(Mesh_==null){
				// Create it now:
				Mesh_=new Mesh();
			}
			
			
			// Build the verts first:
			int vertCount=Path_.GetVertexCount(Accuracy);
			
			Vector3[] verts=new Vector3[vertCount];
			List<int> contours=new List<int>();
			int index=0;
			Path_.GetVertices(verts,null,Accuracy,0f,0f,1f,ref index,contours);
			
			// Create uv set:
			Vector2[] uvs=new Vector2[vertCount];
			
			// UV values are directly based on verts, just a little remapped.
			// UV is 0-1, verts are -1 to +1. Y is also inverted (Because Unity stores textures "upside down").
			bool invertY = (DrawInfo.InvertY == -1f);
			
			for(int i=0;i<vertCount;i++){
				
				Vector3 vert=verts[i];
				
				// Multiply Y on OpenGL:
				if(invertY){
					verts[i].y=-vert.y;
				}
				
				uvs[i]=new Vector2(
					((vert.x+1f)/2f), // Map
					((1f-vert.y)/2f) // Map and invert
				);
				
			}
			
			// Triangulate each contour next.
			
			// Create triangulator:
			if(Triangulator==null){
				Triangulator=new Triangulator(null,0,0);
			}
			
			// Set verts:
			Triangulator.Vertices=verts;
			
			// Triangulate:
			int[] tris=Triangulator.Triangulate(contours);
			
			// Set verts etc now:
			Mesh_.triangles=null;
			Mesh_.vertices=verts;
			Mesh_.uv=uvs;
			Mesh_.triangles=tris;
			
			// Bounds and normals aren't required, but we do need to upload it:
			Mesh_.UploadMeshData(true);
			
			#endif
			
		}
		
		internal override int OutputDimensions{
			get{
				// 2D image.
				return 2;
			}
		}
		
		#if !NO_BLADE_RUNTIME
		
		public override DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			// Stack required.
			
			// Get a material:
			UnityEngine.Material material=GetMaterial(TypeID,SubMaterialID);
			
			// Allocate a target stack now:
			int targetStack=stackID;
			DrawStack stack=tex.GetStack(targetStack,info);
			stackID++;
			
			// Allocate sources:
			AllocateSources(material,info,tex,targetStack,1);
			
			// Create our node:
			MaterialStackNode matNode=DrawStore as MaterialStackNode;
			
			if(matNode==null){
				matNode=new MaterialStackNodeCleared();
				DrawStore=matNode;
				// This is where the magic happens! Set mesh to being our custom one:
				matNode.Mesh=Mesh_;
			}
			
			matNode.Material=material;
			matNode.Stack=stack;
			
			return matNode;
			
		}
		
		#endif
		
		public override int TypeID{
			get{
				return 114;
			}
		}
		
	}
}
