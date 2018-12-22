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
	/// A node which immediately follows a bezier curve.
	/// </summary>
	
	public partial class CurveLinePoint:QuadLinePoint{
		
		/// <summary>Bounds this point to using real workable numbers.
		/// NaN and infinities are eliminated.</summary>
		public override void BoundToReal(){
			
			BoundToReal(ref X);
			BoundToReal(ref Y);
			BoundToReal(ref Control1X);
			BoundToReal(ref Control1Y);
			BoundToReal(ref Control2X);
			BoundToReal(ref Control2Y);
			
		}
		
		/// <summary>Slices off anything to the right of the given line.</summary>
		public override void SliceRight(float sliceLine,VectorPath path){
			
			float previous=Previous.X;
			float control1=Control1X;
			float control2=Control2X;
			float current=X;
			
			// All to the right?
			if(previous<=sliceLine && current<=sliceLine && control1<=sliceLine && control2<=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous>sliceLine && current>sliceLine && control1>sliceLine && control2>sliceLine){
				
				// All to the right.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			float alreadySplitAt=1f;
			bool deleteAll=false;
			
			// Third line intersects?
			if( (current>sliceLine != control2>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control2) / (current-control2)) * 1f/3f ) + 2f/3f;
				newPoint = Split(alreadySplitAt, path);
				X = sliceLine;
				
				// If the second part is to the right, delete it:
				if(current>sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					// Potentially deleting everything else.
					deleteAll=true;
				}
				
			}
			
			// Second line intersects?
			if( (control2>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt = ((((sliceLine-control1) / (control2-control1)) * 1f/3f) + 1f/3f) * alreadySplitAt;
				newPoint = Split(alreadySplitAt, path);
				X = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					deleteAll=true;
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 1f/3f * alreadySplitAt, path);
				X = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					// Delete whatever is left:
					path.RemoveVisually(this);
				}
				
			}else if(deleteAll){
				
				// Delete whatever is left:
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything to the left of the given line.</summary>
		public override void SliceLeft(float sliceLine,VectorPath path){
			
			float previous=Previous.X;
			float control1=Control1X;
			float control2=Control2X;
			float current=X;
			
			// All to the left?
			if(previous>=sliceLine && current>=sliceLine && control1>=sliceLine && control2>=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous<sliceLine && current<sliceLine && control1<sliceLine && control2<sliceLine){
				
				// All to the left.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			float alreadySplitAt=1f;
			bool deleteAll=false;
			
			// Third line intersects?
			if( (current>sliceLine != control2>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control2) / (current-control2)) * 1f/3f ) + 2f/3f;
				newPoint = Split(alreadySplitAt, path);
				X = sliceLine;
				
				// If the second part is to the left, delete it:
				if(current<sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					// Potentially deleting everything else.
					deleteAll=true;
				}
				
			}
			
			// Second line intersects?
			if( (control2>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt = ((((sliceLine-control1) / (control2-control1)) * 1f/3f) + 1f/3f) * alreadySplitAt;
				newPoint = Split(alreadySplitAt, path);
				X = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					deleteAll=true;
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 1f/3f * alreadySplitAt, path);
				X = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					// Delete whatever is left:
					path.RemoveVisually(this);
				}
				
			}else if(deleteAll){
				
				// Delete whatever is left:
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything below the given line.</summary>
		public override void SliceBottom(float sliceLine,VectorPath path){
			
			float previous=Previous.Y;
			float control1=Control1Y;
			float control2=Control2Y;
			float current=Y;
			
			// All below?
			if(previous>=sliceLine && current>=sliceLine && control1>=sliceLine && control2>=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous<sliceLine && current<sliceLine && control1<sliceLine && control2<sliceLine){
				
				// All below.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			float alreadySplitAt=1f;
			bool deleteAll=false;
			
			// Third line intersects?
			if( (current>sliceLine != control2>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control2) / (current-control2)) * 1f/3f ) + 2f/3f;
				newPoint = Split(alreadySplitAt, path);
				Y = sliceLine;
				
				// If the second part is to the left, delete it:
				if(current<sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					// Potentially deleting everything else.
					deleteAll=true;
				}
				
			}
			
			// Second line intersects?
			if( (control2>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt = ((((sliceLine-control1) / (control2-control1)) * 1f/3f) + 1f/3f) * alreadySplitAt;
				newPoint = Split(alreadySplitAt, path);
				Y = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					deleteAll=true;
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 1f/3f * alreadySplitAt, path);
				Y = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					// Delete whatever is left:
					path.RemoveVisually(this);
				}
				
			}else if(deleteAll){
				
				// Delete whatever is left:
				path.RemoveVisually(this);
				
			}
			
		}
		
		/// <summary>Slices off anything above the given line.</summary>
		public override void SliceTop(float sliceLine,VectorPath path){
			
			float previous=Previous.Y;
			float control1=Control1Y;
			float control2=Control2Y;
			float current=Y;
			
			// All above?
			if(previous<=sliceLine && current<=sliceLine && control1<=sliceLine && control2<=sliceLine){
				
				// Do nothing.
				return;
				
			}else if(previous>sliceLine && current>sliceLine && control1>sliceLine && control2>sliceLine){
				
				// All above.
				path.RemoveVisually(this);
				
				return;
				
			}
			
			VectorPoint newPoint;
			float alreadySplitAt=1f;
			bool deleteAll=false;
			
			// Third line intersects?
			if( (current>sliceLine != control2>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt =(((sliceLine-control2) / (current-control2)) * 1f/3f ) + 2f/3f;
				newPoint = Split(alreadySplitAt, path);
				Y = sliceLine;
				
				// If the second part is above, delete it:
				if(current>sliceLine){
					path.RemoveVisually(newPoint);
				}else{
					// Potentially deleting everything else.
					deleteAll=true;
				}
				
			}
			
			// Second line intersects?
			if( (control2>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				alreadySplitAt = ((((sliceLine-control1) / (control2-control1)) * 1f/3f) + 1f/3f) * alreadySplitAt;
				newPoint = Split(alreadySplitAt, path);
				Y = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					deleteAll=true;
				}
				
			}
			
			// First line intersects?
			if( (previous>sliceLine != control1>sliceLine) ){
				
				// Yep! Split at the intersect:
				newPoint = Split(((sliceLine-previous) / (control1-previous)) * 1f/3f * alreadySplitAt, path);
				Y = sliceLine;
				
				// If the first part is to the *other side*, delete newPoint:
				if(deleteAll){
					path.RemoveVisually(newPoint);
					deleteAll=false;
				}else{
					// Delete whatever is left:
					path.RemoveVisually(this);
				}
				
			}else if(deleteAll){
				
				// Delete whatever is left:
				path.RemoveVisually(this);
				
			}
			
		}
		
	}
	
}