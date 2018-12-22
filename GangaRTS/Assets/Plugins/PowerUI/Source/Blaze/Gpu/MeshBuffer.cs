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
	/// A pool of mesh buffers are used during GPU shape draws.
	/// They store generated meshes and display them to a TextureCamera.
	/// </summary>
	
	public class MeshBuffer:PointReceiver{
		
		private static readonly Color32 MidGrey=new Color32(255,255,255,127);
		private static readonly Color32 White=new Color32(255,255,255,255);
		private static readonly Color32 Black=new Color32(255,255,255,0);
		/// <summary>The material shared by all mesh buffers.</summary>
		public static Material SharedMaterial;
		
		/// <summary>A number to scale the mesh by.</summary>
		public float XScaleFactor=-10f;
		/// <summary>A number to scale the mesh by.</summary>
		public float YScaleFactor=10f;
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
		/// <summary>The inner spread used by the previous segment.</summary>
		private float PreviousInnerSpread;
		/// <summary>The outer spread used by the previous segment.</summary>
		private float PreviousOuterSpread;
		/// <summary>Current vertex progress.</summary>
		public int CurrentVertex;
		/// <summary>Current triangle progress.</summary>
		public int CurrentTriangle;
		/// <summary>Vertex index of the "first" tent on a path.</summary>
		private int StartIndex;
		/// <summary>Pooled meshes are stored as a linked list.</summary>
		public MeshBuffer NextInPool;
		/// <summary>The gameobject the mesh is attached to.</summary>
		internal GameObject Gameobject;
		/// <summary>True if a MoveTo just occured.</summary>
		private bool Moved=true;
		/// <summary>The stack of non-closed MoveTo nodes.</summary>
		private MoveFromPoint LatestMoveFrom;
		/// <summary>The current curve sampling accuracy.</summary>
		private float CurrentAccuracy;
		private bool Winding=true;
		private Triangulator Triangulator=null;
		
		
		public MeshBuffer(){
			
			if(SharedMaterial==null){
				SharedMaterial=new Material(Shader.Find("Blaze Raster Draw"));
			}
			
			Mesh=new Mesh();
			CreateGameObject();
			
		}
		
		public void CreateGameObject(){
			
			GameObject cameraGO=TextureCameras.Camera.Gameobject;
			
			if(cameraGO==null){
				// Scene change - recreate:
				cameraGO=TextureCameras.Camera.CreateGameObject();
			}
			
			GameObject go=new GameObject();
			Gameobject=go;
			go.transform.parent=cameraGO.transform;
			go.transform.localPosition=Vector3.zero;
			go.transform.localRotation=Quaternion.identity;
			go.layer=TextureCameras.Layer;
			MeshFilter filter=go.AddComponent<MeshFilter>();
			filter.sharedMesh=Mesh;
			MeshRenderer renderer=go.AddComponent<MeshRenderer>();
			renderer.sharedMaterial=SharedMaterial;
			go.SetActive(false);
			
		}
		
		public void AddMesh(VectorPath path,float xOffset,float yOffset,Triangulator triangulator){
			
			XOffset=xOffset;
			YOffset=yOffset;
			
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
			CurrentAccuracy=TextureCameras.TriangulationAccuracy;
			
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
					CurrentAccuracy=TextureCameras.TriangulationAccuracy;
					
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
		
		public void AddPoint(float x,float y){
			
			if(Triangulator!=null){
				
				x*=XScaleFactor;
				y*=YScaleFactor;
				
				x+=XOffset;
				y+=YOffset;
				
				// Create vert:
				Vector3 vert=new Vector3(x,y,0f);
				
				// Add triangulator vertex:
				Triangulator.AddVertex(vert,ref FirstInContour,ref LastInContour);
				
				// Add it:
				Vertices[CurrentVertex]=vert;
				Colours[CurrentVertex]=White;
				CurrentVertex++;
				
				return;
			}
			
			if(IsClosing && x==CloseX && y==CloseY){
				
				// Add the connection triangles back to the move to node:
				Triangles[CurrentTriangle++]=CurrentVertex+1;
				Triangles[CurrentTriangle++]=StartIndex+2;
				Triangles[CurrentTriangle++]=CurrentVertex+3;
				
				Triangles[CurrentTriangle++]=CurrentVertex+1;
				Triangles[CurrentTriangle++]=CurrentVertex+5;
				Triangles[CurrentTriangle++]=StartIndex+4;
				
			}
			
			x*=XScaleFactor;
			y*=YScaleFactor;
			
			x+=XOffset;
			y+=YOffset;
			
			// Get deltas:
			float dx=x-PreviousX;
			float dy=y-PreviousY;
			
			// Dx/dy (inner):
			float dxInner;
			float dyInner;
			
			// Previous values:
			float previousOuterDY;
			float previousOuterDX;
			float previousInnerDY;
			float previousInnerDX;
			
			// Spread of the SDF distance:
			float innerSpread;
			float outerSpread;
			
			if(DistanceSpread.Custom){
				innerSpread=DistanceSpread.GetInner(x,y,CurrentVertex,TotalVertices);
				
				if(DistanceSpread.InnerOuterDiff){
					outerSpread=DistanceSpread.GetOuter(x,y,CurrentVertex,TotalVertices);
				}else{
					outerSpread=innerSpread;
				}
				
			}else{
				innerSpread=DistanceSpread.DefaultInner;
				outerSpread=DistanceSpread.DefaultOuter;
			}
			
			if(Moved){
				
				// Previous is the same:
				PreviousOuterSpread=outerSpread;
				PreviousInnerSpread=innerSpread;
				
			}
			
			// Previous got a different spread?
			bool previousDiff=(PreviousInnerSpread!=innerSpread);
			float length;
			
			if(DistanceSpread.InnerOuterDiff){
				
				float root=(float)Math.Sqrt((dx*dx) + (dy*dy));
				
				// Normalise inner:
				length=innerSpread/root;
				dxInner=dx*length;
				dyInner=dy*length;
				
				if(previousDiff){
					// Normalise them:
					length=PreviousInnerSpread/root;
					previousInnerDX=dx*length;
					previousInnerDY=dy*length;
				}else{
					previousInnerDY=dyInner;
					previousInnerDX=dxInner;
				}
				
				// Normalise outer:
				length=outerSpread/root;
				dx*=length;
				dy*=length;
				
				if(PreviousOuterSpread==outerSpread){
					previousOuterDY=dy;
					previousOuterDX=dx;
				}else{
					// Normalise them:
					length=PreviousOuterSpread/outerSpread;
					previousOuterDX=dx*length;
					previousOuterDY=dy*length;
				}
				
			}else if(innerSpread==0f){
				
				if(previousDiff){
					
					// Normalise using previous spread:
					// Note that because its different, we also know it's non-zero.
					length=PreviousInnerSpread/(float)Math.Sqrt((dx*dx) + (dy*dy));
					
					previousOuterDY=dy*length;
					previousOuterDX=dx*length;
					
				}else{
					
					// Previous spread is also zero - results are all zero:
					previousOuterDY=0f;
					previousOuterDX=0f;
					
				}
				
				// Inner is the same:
				previousInnerDY=previousOuterDY;
				previousInnerDX=previousOuterDX;
				
				// All zero:
				dxInner=0f;
				dyInner=0f;
				dx=0f;
				dy=0f;
				
			}else{
			
				// Normalise them:
				length=innerSpread/(float)Math.Sqrt((dx*dx) + (dy*dy));
				dx*=length;
				dy*=length;
				
				// Dx/dy (inner):
				dxInner=dx;
				dyInner=dy;
				
				if(previousDiff){
					
					// What's the difference?
					length=PreviousInnerSpread/innerSpread;
					
					// Apply:
					previousOuterDY=dy*length;
					previousOuterDX=dx*length;
					
				}else{
					previousOuterDY=dy;
					previousOuterDX=dx;
					
				}
				
				// Inner is the same:
				previousInnerDY=previousOuterDY;
				previousInnerDX=previousOuterDX;
				
			}
			
			// Triangles (connections):
			if(Moved){
				
				// Clear moved:
				Moved=false;
				
			}else{
				
				Triangles[CurrentTriangle++]=CurrentVertex;
				Triangles[CurrentTriangle++]=CurrentVertex+2;
				Triangles[CurrentTriangle++]=CurrentVertex-3;
				
				Triangles[CurrentTriangle++]=CurrentVertex;
				Triangles[CurrentTriangle++]=CurrentVertex+4;
				Triangles[CurrentTriangle++]=CurrentVertex-1;
				
			}
			
			// Triangles (outer):
			Triangles[CurrentTriangle++]=CurrentVertex;
			Triangles[CurrentTriangle++]=CurrentVertex+2;
			Triangles[CurrentTriangle++]=CurrentVertex+1;
			
			Triangles[CurrentTriangle++]=CurrentVertex+1;
			Triangles[CurrentTriangle++]=CurrentVertex+2;
			Triangles[CurrentTriangle++]=CurrentVertex+3;
			
			// Triangles (inner):
			Triangles[CurrentTriangle++]=CurrentVertex;
			Triangles[CurrentTriangle++]=CurrentVertex+1;
			Triangles[CurrentTriangle++]=CurrentVertex+4;
			
			Triangles[CurrentTriangle++]=CurrentVertex+1;
			Triangles[CurrentTriangle++]=CurrentVertex+5;
			Triangles[CurrentTriangle++]=CurrentVertex+4;
			
			Color32 inner;
			Color32 outer;
			
			if(Winding){
				inner=White;
				outer=Black;
			}else{
				inner=Black;
				outer=White;
			}
			
			// Top line:
			Colours[CurrentVertex]=MidGrey;
			Vertices[CurrentVertex]=new Vector3(PreviousX,PreviousY,1f);
			CurrentVertex++;
			Colours[CurrentVertex]=MidGrey;
			Vertices[CurrentVertex]=new Vector3(x,y,1f);
			CurrentVertex++;
			
			// Bottom line (outer):
			Colours[CurrentVertex]=outer;
			Vertices[CurrentVertex]=new Vector3(PreviousX+previousOuterDY,PreviousY-previousOuterDX,0f);
			CurrentVertex++;
			Colours[CurrentVertex]=outer;
			Vertices[CurrentVertex]=new Vector3(x+dy,y-dx,0f);
			CurrentVertex++;
			
			// Bottom line (inner):
			Colours[CurrentVertex]=inner;
			Vertices[CurrentVertex]=new Vector3(PreviousX-previousInnerDY,PreviousY+previousInnerDX,0f);
			CurrentVertex++;
			Colours[CurrentVertex]=inner;
			Vertices[CurrentVertex]=new Vector3(x-dyInner,y+dxInner,0f);
			CurrentVertex++;
			
			PreviousX=x;
			PreviousY=y;
			PreviousOuterSpread=outerSpread;
			PreviousInnerSpread=innerSpread;
			
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
					int prevVertex=CurrentVertex-6;
					int prevMoveFrom=mfp.Index-6;
					
					// First pair:
					Triangles[CurrentTriangle++]=prevMoveFrom+1;
					Triangles[CurrentTriangle++]=CurrentVertex+2;
					Triangles[CurrentTriangle++]=prevMoveFrom+3;
					
					Triangles[CurrentTriangle++]=prevMoveFrom+1;
					Triangles[CurrentTriangle++]=prevMoveFrom+5;
					Triangles[CurrentTriangle++]=CurrentVertex+4;
					
					// 2nd pair:
					Triangles[CurrentTriangle++]=prevVertex+1;
					Triangles[CurrentTriangle++]=mfp.Index+2;
					Triangles[CurrentTriangle++]=prevVertex+3;
					
					Triangles[CurrentTriangle++]=prevVertex+1;
					Triangles[CurrentTriangle++]=prevVertex+5;
					Triangles[CurrentTriangle++]=mfp.Index+4;
					
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
			
		}
		
	}
	
	internal class MoveFromPoint{
		internal float X;
		internal float Y;
		internal int Index;
		internal MoveFromPoint Previous;
	}
	
}