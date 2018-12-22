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


namespace Blaze{
	
	/// <summary>
	/// A node which immediately follows a straight line.
	/// </summary>
	
	public partial class StraightLinePoint:VectorLine{
		
		/// <summary>Slices off anything to the right of the given line.</summary>
		public override void SliceRight(float sliceLine,VectorPath path){
			
			float previous=Previous.X;
			float current=X;
			
			// Both to the right?
			if(previous>sliceLine && current>sliceLine){
				
				// Both to the right.
				path.RemoveVisually(this);
				
				return;
				
			}else if(previous<=sliceLine && current<=sliceLine){
				
				// Do nothing.
				return;
				
			}
			
			// Split where the slice line is.
			VectorPoint newPoint = Split((sliceLine-previous) / (current-previous), path);
			
			// Delete the segment to the left/right.
			if(current>sliceLine){
				
				// Delete this -> newPoint
				path.RemoveVisually(newPoint);
				
			}else{
				
				// Delete previous -> this
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything to the left of the given line.</summary>
		public override void SliceLeft(float sliceLine,VectorPath path){
			
			float previous=Previous.X;
			float current=X;
			
			// Both to the left?
			if(previous<sliceLine && current<sliceLine){
				
				// Both to the left.
				path.RemoveVisually(this);
				
				return;
				
			}else if(previous>=sliceLine && current>=sliceLine){
				
				// Do nothing.
				return;
				
			}
			
			// Split where the slice line is.
			VectorPoint newPoint = Split((sliceLine-previous) / (current-previous), path);
			
			// Delete the segment to the left/right.
			if(current<sliceLine){
				
				// Delete this -> newPoint
				path.RemoveVisually(newPoint);
				
			}else{
				
				// Delete previous -> this
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything below the given line.</summary>
		public override void SliceBottom(float sliceLine,VectorPath path){
			
			float previous=Previous.Y;
			float current=Y;
			
			// Both below?
			if(previous<sliceLine && current<sliceLine){
				
				// Both below.
				path.RemoveVisually(this);
				
				return;
				
			}else if(previous>=sliceLine && current>=sliceLine){
				
				// Do nothing.
				return;
				
			}
			
			// Split where the slice line is.
			VectorPoint newPoint = Split((sliceLine-previous) / (current-previous), path);
			
			// Delete the segment to the left/right.
			if(current<sliceLine){
				
				// Delete this -> newPoint
				path.RemoveVisually(newPoint);
				
			}else{
				
				// Delete previous -> this
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything above the given line.</summary>
		public override void SliceTop(float sliceLine,VectorPath path){
			
			float previous=Previous.Y;
			float current=Y;
			
			// Both above?
			if(previous>sliceLine && current>sliceLine){
				
				// Both above.
				path.RemoveVisually(this);
				
				return;
				
			}else if(previous<=sliceLine && current<=sliceLine){
				
				// Do nothing.
				return;
				
			}
			
			// Split where the slice line is.
			VectorPoint newPoint = Split((sliceLine-previous) / (current-previous), path);
			
			// Delete the segment to the left/right.
			if(current>sliceLine){
				
				// Delete this -> newPoint
				path.RemoveVisually(newPoint);
				
			}else{
				
				// Delete previous -> this
				path.RemoveVisually(this);
				
			}
			
		}
		
	}
	
}