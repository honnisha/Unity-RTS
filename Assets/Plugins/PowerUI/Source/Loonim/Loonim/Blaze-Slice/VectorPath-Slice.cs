using System;
using System.Collections;
using System.Collections.Generic;


namespace Blaze{
	
	public partial class VectorPath{
		
		/// <summary>Bounds the points in this path to being real workable numbers.
		/// NaN and infinities are eliminated.</summary>
		public void BoundToReal(){
			
			// Eliminate NaN/ infinities:
			VectorPoint point = FirstPathNode;
			
			while(point!=null){
				point.BoundToReal();
				point = point.Next;
			}
			
		}
		
		/// <summary>Chops off any sections of this path which are outside the given bounds.
		/// This is important when rendering paths which could include infinite points.
		/// If we don't chop off path segments which are out of range then the engine would 
		/// spend very large amounts of time walking along those extremely long paths.</summary>
		public void Clip(float minX,float minY,float maxX,float maxY){
			
			// Eliminate NaN/ infinities:
			VectorPoint point = FirstPathNode;
			
			while(point!=null){
				point.BoundToReal();
				point = point.Next;
			}
			
			// Each one is checked individually as an actual 
			// slice may generate new points which would need testing.
			
			// Slice left:
			point = FirstPathNode;
			
			while(point!=null){
				
				VectorPoint next = point.Next;
				
				if(point.HasLine){
					point.SliceLeft(minX,this);
				}
				
				point = next;
			}
			
			// Slice right:
			point = FirstPathNode;
			
			while(point!=null){
				
				VectorPoint next = point.Next;
				
				if(point.HasLine){
					point.SliceRight(maxX,this);
				}
				
				point = next;
			}
			
			// Slice bottom:
			point = FirstPathNode;
			
			while(point!=null){
				
				VectorPoint next = point.Next;
				
				if(point.HasLine){
					point.SliceBottom(minY,this);
				}
				
				point = next;
			}
			
			// Slice top:
			point = FirstPathNode;
			
			while(point!=null){
				
				VectorPoint next = point.Next;
				
				if(point.HasLine){
					point.SliceTop(maxY,this);
				}
				
				point = next;
			}
			
		}
		
	}
	
}