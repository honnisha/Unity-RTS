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
	
	/// <summary>Helper class for generating stroke meshes.</summary>
	public class StrokePathMesh : StrokePath{
		
		/// <summary>True if we're receiving the first line.</summary>
		private bool FirstLine;
		/// <summary>The computed UVs.</summary>
		private Vector2[] UV1;
		/// <summary>The computed vertices.</summary>
		private Vector3[] Vertices;
		/// <summary>The computed triangles set.</summary>
		private int[] Triangles;
		/// <summary>The UV's of the stroke go from 0 to 1. This is multiplied by it.
		/// Note that the mapping is such that x goes from 0-1 "along" the stroke.</summary>
		public Vector2 UvMultiplier=Vector2.one;
		/// <summary>Current vertex index.</summary>
		private int VertexIndex=0;
		/// <summary>Current triangle index.</summary>
		private int TriangleIndex=0;
		/// <summary>The output set.</summary>
		public List<Mesh> Output;
		
		
		/// <summary>Generates one or more stroke meshes for the given path.</summary>
		public List<Mesh> GenerateMeshes(VectorPath path){
			
			// Output set:
			Output=new List<Mesh>();
			
			// Generate:
			Generate(path);
			
			return Output;
			
		}
		
		/// <summary>Called when we're starting to emit a mesh.</summary>
		protected override void StartMesh(bool closed,int lineCount){
			
			FirstLine=true;
			
			// Each pair of lines has 2 triangles between them.
			// So, we have (lineCount-1) * 2 triangles, unless it's a *closed* contour.
			int triIndexCount=lineCount * 2 * 3;
			
			if(!closed){
				triIndexCount-=6;
			}
			
			// Vert count is the number of lines * 2:
			int vertCount=lineCount * 2;
			
			// Create the arrays now:
			UV1=new Vector2[vertCount];
			Vertices=new Vector3[vertCount];
			Triangles=new int[triIndexCount];
			VertexIndex=0;
			TriangleIndex=0;
			
		}
		
		/// <summary>Called when we're emitting a line segment.</summary>
		protected override void EmitLine(StrokePoint inner,StrokePoint outer){
			
			// Emit two verts and UVs:
			Vertices[VertexIndex]=new Vector3(inner.X,inner.Y,0f);
			UV1[VertexIndex]=new Vector2(inner.C * UvMultiplier.x,0f);
			VertexIndex++;
			
			Vertices[VertexIndex]=new Vector3(outer.X,outer.Y,0f);
			UV1[VertexIndex]=new Vector2(outer.C * UvMultiplier.x,UvMultiplier.y);
			VertexIndex++;
			
			// Emit the triangles next (if we can):
			if(FirstLine){
				FirstLine=false;
				return;
			}
			
			// 4 important verts are..
			// [VertexIndex-1, VertexIndex-2], [VertexIndex-3, VertexIndex-4]
			Triangles[TriangleIndex++]=VertexIndex-1;
			Triangles[TriangleIndex++]=VertexIndex-3;
			Triangles[TriangleIndex++]=VertexIndex-2;
			
			Triangles[TriangleIndex++]=VertexIndex-2;
			Triangles[TriangleIndex++]=VertexIndex-3;
			Triangles[TriangleIndex++]=VertexIndex-4;
			
		}
		
		/// <summary>Called when we're done emitting a mesh.</summary>
		protected override void EndMesh(){
			
			// Build the mesh and add it to the set now:
			Mesh mesh=new Mesh();
			mesh.vertices=Vertices;
			mesh.uv=UV1;
			mesh.triangles=Triangles;
			mesh.RecalculateBounds();
			
			// Ok!
			Output.Add(mesh);
			
			// Tidy up:
			Vertices=null;
			UV1=null;
			Triangles=null;
			
		}
		
		
	}
	
}