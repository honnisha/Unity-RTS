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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Blaze{
	
	public partial class VectorPath{
		
		/// <summary>True if any holes in this path have been sorted.</summary>
		protected bool HoleSorted=false;
		
		/// <summary>True if any holes in this path have been sorted. Read only.</summary>
		public bool WasHoleSorted{
			get{
				return HoleSorted;
			}
		}
		
		/// <summary>Finds all the separate contours in this path.</summary>
		public List<PathSegment> GetContours(){
			
			// All the first nodes of each contour:
			List<PathSegment> contours=new List<PathSegment>();
			PathSegment currentContour=null;
			
			// Find all the contours:
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				if(currentContour==null){
					// Create the contour:
					currentContour=new PathSegment(current,this);
				}
				
				if(current.Next==null || !current.Next.HasLine){
					
					// Ensure it's closed:
					current.IsClose=true;
					
					currentContour.Last=current;
					contours.Add(currentContour);
					currentContour=null;
					
				}
				
				current=current.Next;
			}
			
			return contours;
			
		}
		
		/// <summary>Sorts this path such that any holes it contains begin closest to it's containing contour.
		/// This essentially allows paths with holes (think hole in o!) to be correctly triangulated.</summary>
		public void HoleSort(){
			
			// Already sorted?
			if(HoleSorted){
				return;
			}
			
			HoleSorted=true;
			
			if(FirstPathNode==null){
				return;
			}
			
			// Have we actually got any holes?
			// Look for a close node followed by a node contained within the previously closed shape.
			
			// All the first nodes of each contour:
			List<PathSegment> contours=GetContours();
			
			// How many have we got?
			int contourCount=contours.Count;
			
			// Next, for each contour, check if it is contained in any of the previous contours:
			for(int i=0;i<contourCount;i++){
				
				// Grab the contour:
				PathSegment contour=contours[i];
				
				// Get it's coords:
				float currentX=contour.First.X;
				float currentY=contour.First.Y;
				
				// Do any of the other contours contain the first node of this one?
				for(int c=0;c<contourCount;c++){
					
					// Get the potential container contour:
					PathSegment container=contours[c];
					
					if(container==null || container==contour){
						continue;
					}
					
					// Does this contour contain the current point?
					if(container.Contains(currentX,currentY)){
						
						// Yep it does! We have a hole.
						// No longer a valid contour:
						contours[i]=null;
						
						// We're now going to shuffle it.
						// We need it to connect to the nearest node of it's containing contour.
						// Think of it as simply moving a virtual line which goes between the inner shape and the outer shape.
						
						// Get the nearest node:
						VectorPoint nearest=container.Nearest(currentX,currentY);
						
						while(!nearest.HasLine){
							nearest=nearest.Next;
						}
						
						// Last is no longer a close.
						// This will cause it to act as if there's two nodes on top of each other:
						contour.Last.IsClose=false;
						
						// We're about to:
						// - Remove the shape from the set
						// - Add a new node to the end of the shape located at nearest.
						// - Insert the result between nearest and it's follower.
					
						// Pop the shape out:
						contour.Remove();
						
						// Get current one after nearest:
						VectorPoint nearestNext=nearest.Next;
						
						// Add it back in at nearest.
						// Duplicate nearest first - it'll be the "receiving" side:
						MoveToPoint point=new MoveToPoint(nearest.X,nearest.Y);
						point.Previous=contour.Last;
						point.Next=nearestNext;
						PathNodeCount++;
					
						if(nearestNext==null){
							LatestPathNode=point;
						}else{
							nearestNext.Previous=point;
						}
						
						nearest.Next=contour.First;
						contour.First.Previous=nearest;
						contour.Last.Next=point;
						
						// All done with this contour:
						break;
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Gets the nearest node in this shape to the given point.</summary>
		public VectorPoint Nearest(float x,float y){
			return Nearest(x,y,FirstPathNode,LatestPathNode);
		}
		
		/// <summary>Gets the nearest node in the given section of this shape to the given point.</summary>
		public VectorPoint Nearest(float x,float y,VectorPoint from,VectorPoint to){
			
			VectorPoint nearest=null;
			float distance=float.MaxValue;
			
			// For each side, check if it is to the left of our point. if it is, flip contained.
			
			VectorPoint current=from;
			
			while(current!=null){
				
				float dx=current.X-x;
				float dy=current.Y-y;
				
				float dist=dx*dx + dy*dy;
				
				if(dist<distance){
					nearest=current;
					distance=dist;
				}
				
				if(current==to){
					// All done.
					break;
				}
				
				current=current.Next;
				
			}
			
			return nearest;
		}
		
		/// <summary>Does this path contain the given point?</summary>
		public bool Contains(float x,float y){
			return Contains(x,y,FirstPathNode,LatestPathNode);
		}
		
		/// <summary>Does the given section of this path contain the given point?</summary>
		public bool Contains(float x,float y,VectorPoint from,VectorPoint to){
			
			if(from==to){
				return false;
			}
			
			bool contained=false;
			
			// For each side, check if it is to the left of our point. if it is, flip contained.
			
			VectorPoint current=from;
		
			while(current!=null){
				
				VectorPoint pointB=(current==from)?to:current.Previous;
				
				// Figure out the bounding box of the line.
				// We're going to see if the point is outside it - if so, skip.
				
				float minX=(current.X<pointB.X)?current.X:pointB.X;
				
				// Point is to the left of tbe bounding box - ignore.
				if(minX>x){
					goto Next;
				}
				
				float maxX=(current.X>pointB.X)?current.X:pointB.X;
				
				// Point is to the right of this lines bounding box - ignore.
				// We do an inclusive ignore here as the line attached to this one might include it too.
				if(maxX<=x){
					goto Next;
				}
				
				float minY=(current.Y<pointB.Y)?current.Y:pointB.Y;
				
				// Point is below this lines bounding box - ignore.
				if(minY>y){
					goto Next;
				}
				
				// Special case if the point is above.
				float maxY=(current.Y>pointB.Y)?current.Y:pointB.Y;
				
				// We do an inclusive check here as the line attached to this one might include it too.
				if(maxY<=y){
					//The point is above for sure.
					contained=!contained;
					goto Next;
				}
				
				
				// It's sloping. What side of the line are we on? If we're on the right, the line is to the left.
				float gradient=(pointB.Y-current.Y)/(pointB.X-current.X);
				float c=current.Y-(gradient*current.X);
				
				// y<=mx+c means we're on the right, or on the line.
				if(((gradient*x)+c)<=y){
					contained=!contained;
				}
				
				Next:
				
				if(current==to){
					// All done.
					break;
				}
				
				current=current.Next;
				
			}
			
			return contained;
		}
		
		
		public void GetVertices(Vector3[] vertices,Vector3[] normals,float accuracy,float offsetX,float offsetY,float scale,ref int index,List<int> contourStarts){
			
			// We need to know where index starts at:
			int offset=index;
			
			// Next, we must consider all extra points caused by curves:
			VectorPoint current=FirstPathNode;
			
			if(current!=null){
				contourStarts.Add(0);
			}
			
			while(current!=null){
				
				if(current.IsCurve){
					
					// It's a curve - get the line length:
					VectorLine line=current as VectorLine;
					
					float length=line.Length;
					
					int count=(int)(length/accuracy);
					
					if(count>0){
						
						float deltaC=1f/(count+1);
						float c=deltaC;
						
						for(int i=0;i<count;i++){
							
							// Read curve at c:
							float x;
							float y;
							
							line.SampleAt(c,out x,out y);
							
							vertices[index]=new Vector3(offsetX+(x * scale),offsetY+(y * scale),0f);
							//normals[index]=new Vector3(current.NormalX,current.NormalY,0f);
							index++;
							
							c+=deltaC;
							
						}
						
					}
					
				}
				
				if(current.IsClose){
					
					// Immediately following current is the next first node, if it exists.
					if(current.Next!=null){
						// Great, add it:
						contourStarts.Add(index-offset);
					}
					
				}else{
				
					vertices[index]=new Vector3(offsetX+(current.X * scale),offsetY+(current.Y * scale),0f);
					
					//normals[index]=new Vector3(current.NormalX,current.NormalY,0f);
					
					index++;
					
				}
				
				// Hop to the next one:
				current=current.Next;
			}
			
		}
		
		public int GetVertexCount(float accuracy){
			
			int vertCount=0;
			
			// Next, we must consider all extra points caused by curves:
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				if(current.IsCurve){
					
					// It's a curve - get the line length:
					VectorLine line=current as VectorLine;
					
					float length=line.Length;
					
					// And just add on extra points:
					vertCount+=(int)(length/accuracy);
					
				}
				
				if(!current.IsClose){
					vertCount++;
				}
				
				// Hop to the next one:
				current=current.Next;
			}
			
			return vertCount;
			
		}
	
	}
	
}