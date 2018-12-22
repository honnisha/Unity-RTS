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


namespace Blaze{
	
	/// <summary>
	/// This generates an extruded path.
	/// </summary>
	
	public class MeshBufferExtruded:PointReceiver{
		
		/// <summary>A number to scale the mesh by.</summary>
		public float XScaleFactor=1f;
		/// <summary>A number to scale the mesh by.</summary>
		public float YScaleFactor=1f;
		/// <summary>A number to offset the mesh by on x.</summary>
		public float XOffset;
		/// <summary>A number to offset the mesh by on y.</summary>
		public float YOffset;
		/// <summary>True if there is a close before the current node.</summary>
		private bool CloseBeforeThis=true;
		/// <summary>The underlying unity mesh.</summary>
		public Mesh Mesh;
		/// <summary>Is the current rendering node a close?</summary>
		private bool IsClosing;
		/// <summary>The close x coordinate.</summary>
		private float CloseX;
		/// <summary>The close y coordinate.</summary>
		private float CloseY;
		/// <summary>The previous x coordinate.</summary>
		private float PreviousX;
		/// <summary>The previous y coordinate.</summary>
		private float PreviousY;
		/// <summary>The total number of verts being used. Note that it's not necessarily the same as Vertices.Length.</summary>
		public int TotalVertices;
		/// <summary>The computed vertices.</summary>
		public Vector3[] Vertices;
		/// <summary>The vertex colour set.</summary>
		public Color32[] Colours;
		/// <summary>Triangle set.</summary>
		public int[] Triangles;
		/// <summary>Current vertex progress.</summary>
		public int CurrentVertex;
		/// <summary>Current triangle progress.</summary>
		public int CurrentTriangle;
		/// <summary>Vertex index of the "first" tent on a path.</summary>
		private int StartIndex;
		/// <summary>The gameobject the mesh is attached to.</summary>
		internal GameObject Gameobject;
		/// <summary>True if a MoveTo just occured.</summary>
		private bool Moved=true;
		
		public float ExtrudeAmount=0.5f;
		/// <summary>The stack of non-closed MoveTo nodes.</summary>
		private MoveFromPoint LatestMoveFrom;
		/// <summary>The current curve sampling accuracy.</summary>
		private float CurrentAccuracy;
		public bool Winding=true;
		private Triangulator Triangulator=null;
		/// <summary>The colour of the front face.</summary>
		public UnityEngine.Color FrontColor;
		/// <summary>The colour of the back face.</summary>
		public UnityEngine.Color BackColor;
		/// <summary>The colour of the sides.</summary>
		public UnityEngine.Color SideColor;
		
		
		public MeshBufferExtruded(Transform parent){
			
			Mesh=new Mesh();
			CreateGameObject(parent);
			
		}
		
		public void CreateGameObject(Transform parent){
			
			GameObject go=new GameObject();
			Gameobject=go;
			go.transform.parent=parent;
			go.transform.localPosition=Vector3.zero;
			go.transform.localRotation=Quaternion.identity;
			go.transform.localScale=Vector3.one;
			go.layer=TextureCameras.Layer;
			MeshFilter filter=go.AddComponent<MeshFilter>();
			filter.sharedMesh=Mesh;
			go.AddComponent<MeshRenderer>();
			
		}
		
		/// <summary>Sets the material for this mesh.</summary>
		public void SetMaterial(Material material){
			// Update the renderer:
			Gameobject.GetComponent<MeshRenderer>().sharedMaterial=material;
		}
		
		/// <summary>Gets the number of verts/tris to use.</summary>
		public int GetVertCount(VectorPath path,out int triCount){
			
			// Hole sort it if needed:
			path.HoleSort();
		
			if(path.Width==0f){
				path.RecalculateMeta();
			}
			
			// How many samples?
			int samples=0;
			triCount=0;
			int moveToCount=0;
			float accuracy=TextureCameras.Accuracy;
			
			VectorPoint current=path.FirstPathNode;
			
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
			// Each line generates 2 vertices and 2 triangles.
			int vertCount=samples * 2;
			triCount+=samples * 2 * 3;
			
			// Triangulation set next.
			int triangulatedVerts=0;
			
			current=path.FirstPathNode;
			
			while(current!=null){
				
				if(current.HasLine && current.IsCurve){
					
					VectorLine line=current as VectorLine;
					
					int extraPoints=(int)(line.Length / TextureCameras.Accuracy);
					
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
			
			// For each MoveToPair (moveToCount/2), add 2 tri's [Completes the shape]:
			if(moveToCount!=0){
				moveToCount=moveToCount>>1;
				triCount+=2 * 3 * moveToCount;
			}
			
			return vertCount;
			
		}
		
		public void AddMesh(VectorPath path,float xOffset,float yOffset,Triangulator triangulator){
			
			XOffset=xOffset;
			YOffset=yOffset;
			StartIndex=CurrentVertex;
			
			// Start triangulator (also clears VC):
			triangulator.Reset();
		
			// Update triangulator vert offset:
			triangulator.VertexOffset=CurrentVertex;
			
			// Triangulator chain:
			FirstInContour=null;
			LastInContour=null;
			VectorPoint contourStart=path.FirstPathNode;
			VectorPoint current=contourStart;
			
			// Update acc:
			Triangulator=triangulator;
			CurrentAccuracy=TextureCameras.Accuracy;
			
			Winding=true;
			
			while(current!=null){
				
				if(current.IsClose || current.Next==null){
					
					// Triangulate current listing:
					if(FirstInContour!=null){
						
						// Triangulate:
						int triangleCount=triangulator.VertexCount-2;
						
						// Complete the triangulator chain:
						triangulator.Complete(FirstInContour,LastInContour);
						
						#if !TEXT_IS_CLOCKWISE
							// Text directionality:
							triangulator.FindWinding();
							
							Winding=triangulator.Clockwise;
						#endif
						
						if(triangleCount>0){
							triangulator.Triangulate(Triangles,triangleCount,CurrentTriangle);
							CurrentTriangle+=triangleCount*3;
						}
						
					}
					
					FirstInContour=null;
					LastInContour=null;
					
					// Add the edges:
					VectorPoint edge=contourStart;
					
					// Reset:
					Moved=true;
					CloseBeforeThis=true;
					
					// Update acc:
					Triangulator=null;
					CurrentAccuracy=TextureCameras.Accuracy;
					
					while(edge!=null){
						
						if(edge.IsClose || edge.Next==null){
							IsClosing=true;
							CloseX=edge.X;
							CloseY=edge.Y;
						}else{
							IsClosing=false;
						}
						
						if(edge.IsCurve || !edge.HasLine){
							
							// Curves and moveto's
							edge.ComputeLinePoints(this);
							
						}else{
							
							if(TextureCameras.SD){
								// Add the last point only:
								AddPoint(edge.X,edge.Y);
							}else{
								edge.ComputeLinePoints(this);
							}
						
						}
						
						if(IsClosing){
							// End of this contour:
							break;
						}
						
						CloseBeforeThis=false;
						
						// Hop to the next one:
						edge=edge.Next;
					}
					
					// Clear VC:
					triangulator.VertexCount=0;
					
					// Update triangulator vert offset:
					triangulator.VertexOffset=CurrentVertex;
					
					// Update start:
					contourStart=current.Next;
					
					// Restore acc/ triangulator:
					Triangulator=triangulator;
					CurrentAccuracy=TextureCameras.Accuracy;
					
				}else if(current.HasLine && current.IsCurve){
					
					current.ComputeLinePoints(this);
					
				}else{
					
					AddPoint(current.X,current.Y);
					
				}
				
				
				// Hop to the next one:
				current=current.Next;
			}
			
		}
		
		private TriangulationVertex FirstInContour;
		private TriangulationVertex LastInContour;
		
		public void PaintColours(Color32 front,Color32 back,Color32 sides){
			
			for(int i=0;i<Colours.Length;i++){
				Colours[i]=front;
			}
			
			Mesh.colors32=Colours;
			
		}
		
		public void BuildCube(Vector3 a,Vector3 b,Vector3 c,Vector3 d){
			
			// Require the sizes:
			RequireSize(16,12 * 3);
			
			// Back face:
			Vertices[0]=a;
			Vertices[1]=b;
			Vertices[2]=c;
			Vertices[3]=d;
			
			Triangles[0]=2;
			Triangles[1]=1;
			Triangles[2]=0;
			Triangles[3]=3;
			Triangles[4]=2;
			Triangles[5]=0;
			
			float fDepth=a.z - ExtrudeAmount;
			
			// Front face:
			Vertices[4]=new Vector3(a.x,a.y,fDepth);
			Vertices[5]=new Vector3(b.x,b.y,fDepth);
			Vertices[6]=new Vector3(c.x,c.y,fDepth);
			Vertices[7]=new Vector3(d.x,d.y,fDepth);
			
			Triangles[6]=4;
			Triangles[7]=5;
			Triangles[8]=6;
			Triangles[9]=6;
			Triangles[10]=7;
			Triangles[11]=4;
			
			// Dupe verts for the sides:
			for(int i=0;i<8;i++){
				Vertices[8+i]=Vertices[i];
			}
			
			// Side triangles:
			int index=12;
			
			// 3 sides:
			for(int i=0;i<3;i++){
				
				// i+8 == "base" vert index.
				int vertIndex=i+8;
				
				Triangles[index++]=vertIndex;
				Triangles[index++]=vertIndex+1;
				Triangles[index++]=vertIndex+4;
				
				Triangles[index++]=vertIndex+4;
				Triangles[index++]=vertIndex+1;
				Triangles[index++]=vertIndex+5;
				
			}
			
			// 4th side, completing the loop:
			Triangles[index++]=15;
			Triangles[index++]=11;
			Triangles[index++]=8;
			
			Triangles[index++]=12;
			Triangles[index++]=15;
			Triangles[index++]=8;
			
			// Colours:
			for(int i=0;i<Colours.Length;i++){
				Colours[i]=FrontColor;
			}
			
			
			Flush();
			
		}
		
		public void AddPoint(float x,float y){
			
			if(Triangulator!=null){
				
				x*=XScaleFactor;
				y*=YScaleFactor;
				
				x+=XOffset;
				y+=YOffset;
				
				// Create vert:
				Vector3 vert=new Vector3(x,y,-ExtrudeAmount-0.05f);
				
				// Add triangulator vertex:
				Triangulator.AddVertex(vert,ref FirstInContour,ref LastInContour);
				
				// Add it:
				Vertices[CurrentVertex]=vert;
				Colours[CurrentVertex]=FrontColor;
				CurrentVertex++;
				
				return;
			}
			
			if(IsClosing && x==CloseX && y==CloseY){
				
				Triangles[CurrentTriangle++]=CurrentVertex;
				Triangles[CurrentTriangle++]=StartIndex;
				Triangles[CurrentTriangle++]=CurrentVertex+1;
				
				Triangles[CurrentTriangle++]=StartIndex;
				Triangles[CurrentTriangle++]=StartIndex+1;
				Triangles[CurrentTriangle++]=CurrentVertex+1;
				
			}
			
			x*=XScaleFactor;
			y*=YScaleFactor;
			
			x+=XOffset;
			y+=YOffset;
			
			// Triangles (connections):
			if(Moved){
				
				// Clear moved:
				Moved=false;
				
			}else{
				
				Triangles[CurrentTriangle++]=CurrentVertex;
				Triangles[CurrentTriangle++]=CurrentVertex+1;
				Triangles[CurrentTriangle++]=CurrentVertex-2;
				
				Triangles[CurrentTriangle++]=CurrentVertex-1;
				Triangles[CurrentTriangle++]=CurrentVertex-2;
				Triangles[CurrentTriangle++]=CurrentVertex+1;
				
			}
			
			// Top line:
			Colours[CurrentVertex]=FrontColor;
			Vertices[CurrentVertex]=new Vector3(x,y,-0.05f);
			CurrentVertex++;
			Colours[CurrentVertex]=FrontColor;
			Vertices[CurrentVertex]=new Vector3(x,y,-ExtrudeAmount-0.05f);
			CurrentVertex++;
			
			PreviousX=x;
			PreviousY=y;
			
		}
		
		public void MoveTo(float x,float y){
			
			x*=XScaleFactor;
			y*=YScaleFactor;
			
			x+=XOffset;
			y+=YOffset;
			
			Moved=true;
			
			// If this is the first move to, or there is a close before the move to,
			// update StartIndex. That means the next close point will link up with this move to.
			if(CloseBeforeThis){
				StartIndex=CurrentVertex;
			}else{
				
				// This move to does not have a close before it (and isn't the first one).
				// Therefore we have a hole sorted shape.
				// We move to a shape and back again.
				// But note that they may be nested - i.e. we might move, move again, then move back twice.
				// Because of this, we have a stack of moved-from points.
				
				// First though, does the point match the one on the top of the stack - are we moving back?
				MoveFromPoint mfp=LatestMoveFrom;
				
				if(mfp!=null && mfp.X==x && mfp.Y==y){
					// Moving back!
					
					// Pop:
					LatestMoveFrom=LatestMoveFrom.Previous;
					
					// Add the triangles as if we just did a close:
					int prevVertex=CurrentVertex-2;
					int prevMoveFrom=mfp.Index-2;
					
					Triangles[CurrentTriangle++]=CurrentVertex+1;
					Triangles[CurrentTriangle++]=prevMoveFrom;
					Triangles[CurrentTriangle++]=CurrentVertex;
					
					Triangles[CurrentTriangle++]=prevMoveFrom+1;
					Triangles[CurrentTriangle++]=prevMoveFrom;
					Triangles[CurrentTriangle++]=CurrentVertex+1;
					
					Triangles[CurrentTriangle++]=prevVertex;
					Triangles[CurrentTriangle++]=prevMoveFrom+2;
					Triangles[CurrentTriangle++]=prevMoveFrom+3;
					
					Triangles[CurrentTriangle++]=prevVertex;
					Triangles[CurrentTriangle++]=prevMoveFrom+3;
					Triangles[CurrentTriangle++]=prevVertex+1;
					
				}else{
					
					// Create:
					mfp=new MoveFromPoint();
					mfp.X=PreviousX;
					mfp.Y=PreviousY;
					mfp.Index=CurrentVertex;
					
					// Push:
					mfp.Previous=LatestMoveFrom;
					LatestMoveFrom=mfp;
					
				}
				
			}
			
			PreviousX=x;
			PreviousY=y;
			
		}
		
		public float SampleDistance{
			get{
				return CurrentAccuracy;
			}
			set{
			}
		}
		
		public void SetActive(bool active){	
			
			#if SHOW_RENDER_DECK
			if(!active){
				return;
			}
			#endif
			
			Gameobject.SetActive(active);
		}
		
		public void Destroy(){
			if(Gameobject!=null){
				GameObject.Destroy(Gameobject);
				Gameobject=null;
			}
			
			Vertices=null;
			Colours=null;
		}
		
		public void RequireSize(int verts,int tris){
			
			CurrentVertex=0;
			CurrentTriangle=0;
			TotalVertices=verts;
			
			if(Vertices==null || Vertices.Length<verts){
				Vertices=new Vector3[verts];
				Colours=new Color32[verts];
			}else if(Vertices.Length>verts){
				
				Array.Clear(Vertices,0,Vertices.Length);
				Array.Clear(Colours,0,Vertices.Length);
				
			}
			
			if(Triangles==null || Triangles.Length<tris){
				Triangles=new int[tris];
			}else if(Triangles.Length>tris){
				
				Array.Clear(Triangles,0,Triangles.Length);
				
			}
			
		}
		
		public void Flush(){
			
			Mesh.triangles=null;
			Mesh.colors32=null;
			Mesh.vertices=Vertices;
			Mesh.colors32=Colours;
			Mesh.triangles=Triangles;
			
			Mesh.RecalculateBounds();
			Mesh.RecalculateNormals();
			
		}
		
	}
	
}