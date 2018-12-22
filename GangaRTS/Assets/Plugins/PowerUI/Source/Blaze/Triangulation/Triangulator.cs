//--------------------------------------
//          Blaze Rasteriser
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
	
	/// <summary>Delegate used when the triangulator wants to add a triangle but it's out of range.
	/// This can be used to allocate another triangle buffer etc.</summary>
	public delegate void OnTriangulatorRange();
	
	public class Triangulator{

		/// <summary>True if the verts go around clockwise.
		/// In most cases this depends on which direction the polygon was drawn in.</summary>
		private bool RawClockwise;
		public bool OutputClockwise;
		public int[] Triangles;
		public Vector3[] Vertices;
		public int TriangleIndex;
		public int VertexCount;
		public int VertexOffset;
		public TriangulationVertex Current;
		public bool UseZ;
		/// <summary>Delegate used when the triangulator wants to add an out of range triangle.</summary>
		public OnTriangulatorRange OutOfRange;
		
		
		public Triangulator(Vector3[] vertices):this(vertices,0,vertices.Length){}
		
		public Triangulator(Vector3[] vertices,int start,int vertexCount){
			Vertices=vertices;
			Select(start,vertexCount);
		}
		
		public Triangulator(Vector3[] vertices,int start,int vertexCount,bool useZ){
			Vertices=vertices;
			UseZ=useZ;
			Select(start,vertexCount);
		}
		
		/// <summary>Note that you must have set Vertices first. Triangulates each declared contour individually.</summary>
		public int[] Triangulate(List<int> contours){
			return Triangulate(contours,true);
		}
		
		/// <summary>Note that you must have set Vertices first. Triangulates each declared contour individually.</summary>
		/// <param name="testWinding">Optionally tests the winding direction of the first contour.
		/// Assumes all contours wind the same way.</param>
		public int[] Triangulate(List<int> contours,bool testWinding){
			
			// First count the # of tri's:
			int max=contours.Count-1;
			
			int triangleCount=0;
			
			for(int i=0;i<=max;i++){
				
				// Next vert index:
				int next=(i==max)?Vertices.Length : contours[i+1];
				
				// Current vert index:
				int current=contours[i];
				
				// Count is therefore..
				int triCount=(next-current)-2;
				
				if(triCount>0){
					// Add the # of indices:
					triangleCount+=triCount*3;
				}
				
			}
			
			// Create the array now:
			int[] tris=new int[triangleCount];
			
			// Reset index:
			TriangleIndex=0;
			
			// Triangulate each contour next, selecting it as we go.
			
			for(int i=0;i<=max;i++){
				
				// Next contour start index:
				int next=(i==max)?Vertices.Length : contours[i+1];
				
				// Current vert index:
				int current=contours[i];
				
				// Count is therefore..
				int triCount=(next-current)-2;
				
				if(triCount<=0){
					continue;
				}
				
				// Select the group of verts (resets for us too):
				Select(current,next);
				
				if(i==0 && testWinding){
					FindWinding();
				}
				
				// Triangulate now:
				Triangulate(tris,triCount,TriangleIndex);
				
			}
			
			return tris;
			
		}
		
		/// <summary>Set this true if the verts wind clockwise.</summary>
		public bool Clockwise{
			get{
				return RawClockwise;
			}
			set{
				RawClockwise=value;
				OutputClockwise=value;
			}
		}
		
		/// <summary>Resets the triangulator.</summary>
		public void Reset(){
			VertexCount=0;
			VertexOffset=0;
			Current=null;
		}
		
		/// <summary>Adds a vertex to the current set of open vertices.</summary>
		public void AddVertex(Vector3 vert,ref TriangulationVertex first,ref TriangulationVertex last){
			
			TriangulationVertex newVertex=new TriangulationVertex(vert,VertexOffset+VertexCount,UseZ);
			
			if(VertexCount==0){
				first=last=newVertex;
			}else{
				newVertex.Previous=last;
				last=last.Next=newVertex;
			}
			
			VertexCount++;
			
		}
		
		/// <summary>Completes an externally built triangulation set.</summary>
		public void Complete(TriangulationVertex first,TriangulationVertex last){
			
			last.Next=first;
			first.Previous=last;
			Current=first;
			
		}
		
		public void Select(int start,int vertexCount){
			
			VertexCount=vertexCount;
			VertexOffset=start;
			Current=null;
			
			if(vertexCount<=0){
				return;
			}
			
			TriangulationVertex first=null;
			TriangulationVertex last=null;
			
			int max=start+vertexCount;
			
			for(int i=start;i<max;i++){
				TriangulationVertex newVertex=new TriangulationVertex(Vertices[i],i,UseZ);
				if(i==start){
					first=last=newVertex;
				}else{
					newVertex.Previous=last;
					last=last.Next=newVertex;
				}
			}
			
			last.Next=first;
			first.Previous=last;
			Current=first;
		}
		
		/// <summary>Call this to find the winding order of the polygon.</summary>
		public void FindWinding(){
			RawClockwise=(GetSignedArea()>0f);
			OutputClockwise=RawClockwise;
		}
		
		/// <summary>Gets the area of the polygon to triangulate.</summary>
		public float GetArea(){
			float area=GetSignedArea();
			if(area<0f){
				area=-area;
			}
			
			return area;
		}
		
		/// <summary>Gets the area of the polygon to triangulate.
		/// Note: may be negative.</summary>
		private float GetSignedArea(){
			float sum=0f;
			
			TriangulationVertex node=Current;
			
			for(int i=0;i<VertexCount;i++){
				
				// Grab the next one:
				TriangulationVertex nextNode=node.Next;
				
				// Add them into the sum:
				sum+=node.X*nextNode.Y - nextNode.X*node.Y;
				
				// Hop to next one:
				node=nextNode;
				
			}
			
			return sum/2f;
		}
		
		public void AddTriangle(int a,int b,int c){
			
			int triangleIndex=TriangleIndex;
			
			if(triangleIndex>=Triangles.Length){
				
				// Got an outta range function?
				if(OutOfRange!=null){
					OutOfRange();
					
					// Pull triangle index again (hopefully now something else!):
					triangleIndex=TriangleIndex;
					
				}
				
			}
			
			if(OutputClockwise){
				Triangles[triangleIndex++]=b;
				Triangles[triangleIndex++]=a;
				Triangles[triangleIndex++]=c;
			}else{
				Triangles[triangleIndex++]=a;
				Triangles[triangleIndex++]=b;
				Triangles[triangleIndex++]=c;
			}
			
			TriangleIndex=triangleIndex;
			
		}
		
		public int[] Triangulate(){
			if(VertexCount<3){
				return new int[0];
			}else if(VertexCount==3){
				return new int[]{0,1,2};
			}
			
			int triangleCount=(VertexCount-2);
			
			Triangles=new int[triangleCount*3];
			
			Triangulate(Triangles,triangleCount,0);
			
			return Triangles;
		}
		
		public void Triangulate(int[] triangles,int triangleCount,int offset){
			
			Triangles=triangles;
			TriangleIndex=offset;
			
			int vertexMaximum=VertexOffset+VertexCount-1;
			
			for(int i=0;i<triangleCount;i++){
				// Foreach triangle..
				// Find a set of 3 vertices that make a valid 'ear'.
				
				// This keeps a linked loop (start is linked to end) of vertices going.
				TriangulationVertex current=Current;
				
				for(int vert=vertexMaximum;vert>=VertexOffset;vert--){
					TriangulationVertex A=current;
					TriangulationVertex B=current.Previous;
					TriangulationVertex C=current.Next;
					// Is this triangle convex?
					
					if(RawClockwise){
						if(((B.Y - A.Y) * (C.X - A.X))<(((B.X - A.X) * (C.Y - A.Y)))){
							// Yes, not a valid option.
							current=current.Next;
							continue;
						}
						
						// Only vertices left in the loop can potentially go inside this triangle.
						// So, starting from the vertex after C, loop around until B is found.
						TriangulationVertex contained=current.Next.Next;
						
						while(contained!=current.Previous){
							if(InsideTriangle(A,B,C,contained)){
								goto NextVert;
							}
							contained=contained.Next;
						}
						
					}else{
						if(((B.Y - A.Y) * (C.X - A.X))>(((B.X - A.X) * (C.Y - A.Y)))){
							// Yes, not a valid option.
							current=current.Next;
							continue;
						}
						
						// Only vertices left in the loop can potentially go inside this triangle.
						// So, starting from the vertex after C, loop around until B is found.
						TriangulationVertex contained=current.Next.Next;
						
						while(contained!=current.Previous){
							
							if(InsideTriangleAnti(A,B,C,contained)){
								goto NextVert;
							}
							
							contained=contained.Next;
						}
						
					}
					
					AddTriangle(current.Index,current.Next.Index,current.Previous.Index);
					current.Remove();
					
					if(current==Current){
						Current=current.Next;
					}
					
					break;
					
					NextVert:
					current=current.Next;
				}
				
			}
			
		}
		
		private bool InsideTriangleAnti(TriangulationVertex A, TriangulationVertex B,TriangulationVertex C,TriangulationVertex P){
	 
			float ax = C.X - B.X;
			float ay = C.Y - B.Y;
			float bpx = P.X - B.X;
			float bpy = P.Y - B.Y;
			
			float cross=(ax * bpy - ay * bpx); //A crossed with BP.
			
			if(cross<0f){
				return false;
			}else if(cross==0f){
				// Is B or C==P?
				if( (B.X==P.X && B.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			float bx = A.X - C.X;
			float by = A.Y - C.Y;
			float cpx = P.X - C.X;
			float cpy = P.Y - C.Y;
			
			cross=(bx * cpy - by * cpx); //B crossed with CP.
			
			if(cross<0f){
				return false;
			}else if(cross==0f){
				// Is A or C==P?
				if( (A.X==P.X && A.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			
			float cx = B.X - A.X;
			float cy = B.Y - A.Y;
			float apx = P.X - A.X;
			float apy = P.Y - A.Y;
			
			cross=(cx * apy - cy * apx); //C crossed with AP.
			
			if(cross<0f){
				return false;
			}else if(cross==0f){
				// Is A or B==P?
				if( (A.X==P.X && A.Y==P.Y) || (B.X==P.X && B.Y==P.Y) ){
					return false;
				}
			}
			
			
			return true;
		}
		
		private bool InsideTriangle(TriangulationVertex A, TriangulationVertex B,TriangulationVertex C,TriangulationVertex P){
	 
			float ax = C.X - B.X;
			float ay = C.Y - B.Y;
			float bpx = P.X - B.X;
			float bpy = P.Y - B.Y;
			
			float cross=(ax * bpy - ay * bpx); //A crossed with BP.
			
			if(cross>0f){
				return false;
			}else if(cross==0f){
				// Is B or C==P?
				if( (B.X==P.X && B.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			float bx = A.X - C.X;
			float by = A.Y - C.Y;
			float cpx = P.X - C.X;
			float cpy = P.Y - C.Y;
			
			cross=(bx * cpy - by * cpx); //B crossed with CP.
			
			if(cross>0f){
				return false;
			}else if(cross==0f){
				// Is A or C==P?
				if( (A.X==P.X && A.Y==P.Y) || (C.X==P.X && C.Y==P.Y) ){
					return false;
				}
			}
			
			float cx = B.X - A.X;
			float cy = B.Y - A.Y;
			float apx = P.X - A.X;
			float apy = P.Y - A.Y;
			
			cross=(cx * apy - cy * apx); //C crossed with AP.
			
			if(cross>0f){
				return false;
			}else if(cross==0f){
				// Is A or B==P?
				if( (A.X==P.X && A.Y==P.Y) || (B.X==P.X && B.Y==P.Y) ){
					return false;
				}
			}
			
			
			return true;
		}
		
	}
	
}