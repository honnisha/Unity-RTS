//--------------------------------------
//                Blade
//
//        For documentation or 
//    if you have any issues, visit
//        blaze.kulestar.com
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Blaze{
	
	/// <summary>
	/// A single buffer which holds a fixed number of blocks. Used whilst building a mesh.
	/// </summary>
	
	public partial class BlockBuffer{
		
		/// <summary>When pooled, this is a linked list of buffers in the pool.
		/// When in use, this is the linked list of buffers which collectively hold the full set.</summary>
		public BlockBuffer Next;
		/// <summary>When in use, this is the previous buffer in the linked list.</summary>
		public BlockBuffer Previous;
		/// <summary>The normals of the current rendering mesh.</summary>
		public Vector3[] Normals;
		/// <summary>The vertices of the current rendering mesh.</summary>
		public Vector3[] Vertices;
		/// <summary>The colours for the current rendering mesh.</summary>
		public Color[] Colours;
		/// <summary>The triangles for the current rendering mesh. Structured around 2 tris per 4 verts.</summary>
		public int[] Triangles;
		/// <summary>The UV1 set of the current rendering mesh.</summary>
		public Vector2[] UV1;
		/// <summary>The UV1 set of the current rendering mesh. X is used for AO.</summary>
		public Vector2[] UV2;
		/// <summary>The UV1 set of the current rendering mesh.</summary>
		public Vector2[] UV3;
		/// <summary>The number of verts that are before this buffer.</summary>
		public int Offset;
		/// <summary>The number of blocks that are before this buffer.</summary>
		public int BlocksBefore;
		/// <summary>The UV1 set of the current rendering mesh.</summary>
		// public Vector2[] UV4;
		/// <summary>The tangents for the current rendering mesh.</summary>
		// public Vector4[] Tangents;
		
		
		public BlockBuffer(){
			
			// Create the standard buffers:
			Triangles=new int[MeshDataBufferPool.TriangleBufferSize];
			
			Vertices=new Vector3[MeshDataBufferPool.VertexBufferSize];
			Colours=new Color[MeshDataBufferPool.VertexBufferSize];
			UV1=new Vector2[MeshDataBufferPool.VertexBufferSize];
			UV2=new Vector2[MeshDataBufferPool.VertexBufferSize];
			
			/*
			Tangents=new Vector4[MeshDataBufferPool.VertexBufferSize];
			UV4=new Vector2[MeshDataBufferPool.VertexBufferSize];
			*/
			
		}
		
		public void RequireNormals(){
			
			if(Normals==null){
				Normals=new Vector3[MeshDataBufferPool.VertexBufferSize];
			}
			
		}
		
		public void RequireUV3(){
			
			if(UV3==null){
				UV3=new Vector2[MeshDataBufferPool.VertexBufferSize];
			}
			
		}
		
	}

}