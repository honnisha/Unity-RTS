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


namespace Blaze{
	
	/// <summary>
	/// A segment of a path.
	/// Sometimes paths contain multiple distinctive sub-paths; these segments are used to represent those.
	/// (Blaze calls them contours).
	/// </summary>
	
	public class PathSegment{
		
		public VectorPath Path;
		public VectorPoint Last;
		public VectorPoint First;
		
		public PathSegment(VectorPoint first,VectorPath path){
			First=first;
			Path=path;
		}
		
		public bool Contains(float x,float y){
			return Path.Contains(x,y,First,Last);
		}
		
		public VectorPoint Nearest(float x,float y){
			return Path.Nearest(x,y,First,Last);
		}
		
		/// <summary>Removes this segment from the parent path.</summary>
		public void Remove(){
			
			// Get previous:
			VectorPoint previous=First.Previous;
			
			// Get next:
			VectorPoint next=Last.Next;
			
			if(previous==null){
				Path.FirstPathNode=next;
			}else{
				previous.Next=next;
			}
			
			if(next==null){
				Path.LatestPathNode=previous;
			}else{
				next.Previous=previous;
			}
			
		}
		
	}

}