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
	/// A node which immediately follows a quadratic curve.
	/// </summary>
	
	public partial class QuadLinePoint:VectorLine{
		
		/// <summary>Bounds this point to using real workable numbers.
		/// NaN and infinities are eliminated.</summary>
		public override void BoundToReal(){
			
			BoundToReal(ref X);
			BoundToReal(ref Y);
			BoundToReal(ref Control1X);
			BoundToReal(ref Control1Y);
			
		}
		
		/// <summary>Slices off anything to the right of the given line.</summary>
		public override void SliceRight(float sliceLine,VectorPath path){
			
			float previous=Previous.X;
			float control1=Control1X;
			float current=X;
			
			// All to the right?
			if(previous<=sliceLine && current<=sliceLine && control1<=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous>sliceLine && current>sliceLine && control1>sliceLine){
				
				// All to the right.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			
			if(control1==sliceLine){
				
				// Control point is on the line - split at 0.5:
				newPoint = Split(0.5f, path);
				
				// Which point is to the right?
				if(current>sliceLine){
					// 2nd half is to the right.
					path.RemoveVisually(newPoint);
				}else{
					path.RemoveVisually(this);
				}
				
				return;
				
			}
			
			// Second line intersects?
			bool removeThis=false;
			float alreadySplitAt=1f;
			
			if( (current>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control1) / (current-control1)) * 0.5f ) + 0.5f;
				newPoint = Split(alreadySplitAt, path);
				X = sliceLine;
				
				// If the second part is to the right, delete it:
				if(current>sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					
					// Remove this, but only after checking if first line intersects.
					removeThis=true;
					
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 0.5f * alreadySplitAt, path);
				X = sliceLine;
				
				// If the first part is to the right, delete it:
				if(previous>sliceLine){
					path.RemoveVisually(this);
				}else{
					// Remove newPoint:
					path.RemoveVisually(newPoint);
				}
				
			}else if(removeThis){
				
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything to the left of the given line.</summary>
		public override void SliceLeft(float sliceLine,VectorPath path){
			
			float previous=Previous.X;
			float control1=Control1X;
			float current=X;
			
			// All to the left?
			if(previous>=sliceLine && current>=sliceLine && control1>=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous<sliceLine && current<sliceLine && control1<sliceLine){
				
				// All to the left.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			
			if(control1==sliceLine){
				
				// Control point is on the line - split at 0.5:
				newPoint = Split(0.5f, path);
				
				// Which point is to the left?
				if(current<sliceLine){
					// 2nd half is to the left.
					path.RemoveVisually(newPoint);
				}else{
					path.RemoveVisually(this);
				}
				
				return;
				
			}
			
			// Second line intersects?
			bool removeThis=false;
			float alreadySplitAt=1f;
			
			if( (current>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control1) / (current-control1)) * 0.5f ) + 0.5f;
				newPoint = Split(alreadySplitAt, path);
				X = sliceLine;
				
				// If the second part is to the left, delete it:
				if(current<sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					
					// Remove this, but only after checking if first line intersects.
					removeThis=true;
					
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 0.5f * alreadySplitAt, path);
				X = sliceLine;
				
				// If the first part is to the left, delete it:
				if(previous<sliceLine){
					path.RemoveVisually(this);
				}else{
					// Remove newPoint:
					path.RemoveVisually(newPoint);
				}
				
			}else if(removeThis){
				
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything below the given line.</summary>
		public override void SliceBottom(float sliceLine,VectorPath path){
			
			float previous=Previous.Y;
			float control1=Control1Y;
			float current=Y;
			
			// All below?
			if(previous>=sliceLine && current>=sliceLine && control1>=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous<sliceLine && current<sliceLine && control1<sliceLine){
				
				// All below.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			
			if(control1==sliceLine){
				
				// Control point is on the line - split at 0.5:
				newPoint = Split(0.5f, path);
				
				// Which point is below?
				if(current<sliceLine){
					// 2nd half is below.
					path.RemoveVisually(newPoint);
				}else{
					path.RemoveVisually(this);
				}
				
				return;
				
			}
			
			// Second line intersects?
			bool removeThis=false;
			float alreadySplitAt=1f;
			
			if( (current>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control1) / (current-control1)) * 0.5f ) + 0.5f;
				newPoint = Split(alreadySplitAt, path);
				Y = sliceLine;
				
				// If the second part is below, delete it:
				if(current<sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					
					// Remove this, but only after checking if first line intersects.
					removeThis=true;
					
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 0.5f * alreadySplitAt, path);
				Y = sliceLine;
				
				// If the first part is below, delete it:
				if(previous<sliceLine){
					path.RemoveVisually(this);
				}else{
					// Remove newPoint:
					path.RemoveVisually(newPoint);
				}
				
			}else if(removeThis){
				
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything above the given line.</summary>
		public override void SliceTop(float sliceLine,VectorPath path){
			
			float previous=Previous.Y;
			float control1=Control1Y;
			float current=Y;
			
			// Both above?
			if(previous<=sliceLine && current<=sliceLine && control1<=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous>sliceLine && current>sliceLine && control1>sliceLine){
				
				// Both above.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			
			if(control1==sliceLine){
				
				// Control point is on the line - split at 0.5:
				newPoint = Split(0.5f, path);
				
				// Which point is above?
				if(current>sliceLine){
					// 2nd half is above.
					path.RemoveVisually(newPoint);
				}else{
					path.RemoveVisually(this);
				}
				
				return;
				
			}
			
			// Second line intersects?
			bool removeThis=false;
			float alreadySplitAt=1f;
			
			if( (current>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control1) / (current-control1)) * 0.5f ) + 0.5f;
				newPoint = Split(alreadySplitAt, path);
				Y = sliceLine;
				
				// If the second part is above, delete it:
				if(current>sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					
					// Remove this, but only after checking if first line intersects.
					removeThis=true;
					
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 0.5f * alreadySplitAt, path);
				Y = sliceLine;
				
				// If the first part is above, delete it:
				if(previous>sliceLine){
					path.RemoveVisually(this);
				}else{
					// Remove newPoint:
					path.RemoveVisually(newPoint);
				}
				
			}else if(removeThis){
				
				path.RemoveVisually(this);
				
			}
			
		}
		
	}
	
}