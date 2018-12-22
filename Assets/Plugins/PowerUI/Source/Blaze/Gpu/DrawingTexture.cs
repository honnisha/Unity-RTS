//--------------------------------------
//                Blade
//
//        For documentation or 
//    if you have any issues, visit
//        blade.kulestar.com
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;


namespace Blaze{
	
	public class DrawingTexture{
		
		/// <summary>X position.</summary>
		public int X;
		/// <summary>Y position.</summary>
		public int Y;
		/// <summary>Y offset in path units.</summary>
		public float OffsetY;
		/// <summary>X offset in path units.</summary>
		public float OffsetX;
		/// <summary>The path to draw.</summary>
		public VectorPath Path;
		/// <summary>The actual mesh for the drawing.</summary>
		public MeshBuffer Mesh;
		/// <summary>The location on an atlas.</summary>
		public AtlasLocation Location;
		/// <summary>Linked list of drawings.</summary>
		public DrawingTexture NextDrawing;
		
		
		public void BuildMesh(float xOffset,float yOffset,TextureCamera camera){
			
			// Hole sort it if needed:
			Path.HoleSort();
			
			// How many samples?
			int samples=0;
			int triCount=0;
			int moveToCount=0;
			float accuracy=TextureCameras.Accuracy;
			
			VectorPoint current=Path.FirstPathNode;
			
			while(current!=null){
				
				if(current.IsClose || current.Next==null){
					moveToCount--;
				}
				
				if(!current.HasLine){
					// Ignore moveto's - Hop to the next one:
					moveToCount++;
					current=current.Next;
					continue;
				}
				
				if(current.IsCurve || !TextureCameras.SD){
					
					// It's a curve (or a HD line) - get the line length:
					VectorLine line=current as VectorLine;
					
					float length=line.Length;
					
					// And just add on extra points:
					int count=(int)(length/accuracy);
					
					if(count<=0){
						count=1;
					}
				
					samples+=count;
					
					// Anything that isn't a moveto also has 6 additional triangle indices (2 tris):
					triCount+=count*6;
					
				}else{
					
					// Add the end node:
					samples++;
					
					// Anything that isn't a moveto also has 6 additional triangle indices (2 tris):
					triCount+=6;
					
				}
				
				// Hop to the next one:
				current=current.Next;
			}
			
			
			// Each sample point has an associated line.
			// Each line generates 6 vertices and 4 triangles.
			int vertCount=samples * 6;
			triCount+=samples * 4 * 3;
			
			// Triangulation set next.
			int triangulatedVerts=0;
			
			current=Path.FirstPathNode;
			
			while(current!=null){
				
				if(current.HasLine && current.IsCurve){
					
					VectorLine line=current as VectorLine;
					
					int extraPoints=(int)(line.Length / TextureCameras.TriangulationAccuracy);
					
					if(extraPoints<=0){
						extraPoints=1;
					}
					
					triangulatedVerts+=extraPoints;
					
				}else{
					
					triangulatedVerts++;
					
				}
				
				if(current.IsClose || current.Next==null){
					
					// End of a main contour.
					
					vertCount+=triangulatedVerts;
					
					if(triangulatedVerts>2){
						triCount+=(triangulatedVerts-2) * 3;
					}
					
					triangulatedVerts=0;
					
				}
				
				// Hop to the next one:
				current=current.Next;
			}
			
			camera.Triangulator.Clockwise=true;
			
			Mesh=TextureCameras.GetBuffer();
			
			// For each MoveToPair (moveToCount/2), add 4 tri's [2 tris for each "gap"]:
			if(moveToCount!=0){
				moveToCount=moveToCount>>1;
				triCount+=12 * moveToCount;
			}
			
			Mesh.RequireSize(vertCount,triCount);
			
			// Map offsets:
			xOffset+=OffsetX * Mesh.XScaleFactor;
			yOffset+=OffsetY * Mesh.YScaleFactor;
			
			// Let's get generating!
			Mesh.AddMesh(Path,xOffset-((float)X * 0.1f),yOffset+((float)Y * 0.1f),camera.Triangulator);
			
		}
		
		public bool Active{
			get{
				return ((Path as AtlasEntity) == Location.Image);
			}
		}
		
	}
	
}