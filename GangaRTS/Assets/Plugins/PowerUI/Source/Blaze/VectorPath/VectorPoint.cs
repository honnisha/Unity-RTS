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

	public partial class VectorPoint{
		
		/// <summary>The X coordinate of this path node.</summary>
		public float X;
		/// <summary>The Y coordinate of this path node.</summary>
		public float Y;
		/// <summary>Path nodes are stored as a linked list. The one after this node.</summary>
		public VectorPoint Next;
		/// <summary>Path nodes are stored as a linked list. The one before this node.</summary>
		public VectorPoint Previous;
		
		
		/// <summary>Creates an empty path node.</summary>
		public VectorPoint(){}
		
		/// <summary>Creates a node at the given point.</summary>
		/// <param name="x">The X coordinate of this path node.</param>
		/// <param name="y">The Y coordinate of this path node.</param>
		public VectorPoint(float x,float y){
			X=x;
			Y=y;
		}
		
		public virtual void Contains(float x,float y,ref bool contained){
			
			float prevX=Previous.X;
			float prevY=Previous.Y;
			
			// Figure out the bounding box of the line.
			// We're going to see if the point is outside it - if so, skip.
			
			float minX=(X<prevX)?X:prevX;
			
			// Point is to the left of tbe bounding box - ignore.
			if(minX>x){
				return;
			}
			
			float maxX=(X>prevX)?X:prevX;
			
			// Point is to the right of this lines bounding box - ignore.
			// We do an inclusive ignore here as the line attached to this one might include it too.
			if(maxX<=x){
				return;
			}
			
			float minY=(Y<prevY)?Y:prevY;
			
			// Point is below this lines bounding box - ignore.
			if(minY>y){
				return;
			}
			
			// Special case if the point is above.
			float maxY=(Y>prevY)?Y:prevY;
			
			// We do an inclusive check here as the line attached to this one might include it too.
			if(maxY<=y){
				//The point is above for sure.
				contained=!contained;
				return;
			}
			
			
			// It's sloping. What side of the line are we on? If we're on the right, the line is to the left.
			float dx=(prevX-X);
			
			if(dx==0f){
				
				// Vertical line - simple check:
				if(Y<=y){
					contained=!contained;
				}
				
				return;
			}
			
			float gradient=(prevY-Y)/dx;
			float c=Y-(gradient*X);
			
			// y<=mx+c means we're on the right, or on the line.
			if(((gradient*x)+c)<=y){
				contained=!contained;
			}
			
		}
		
		public virtual float SignedArea(){
			return (Y+Previous.Y) * (X-Previous.X);
		}
		
		/// <summary>Adds a control point here.</summary>
		public virtual VectorPoint AddControl(float x,float y,VectorPath path,out int id){
			id=0;
			return null;
		}
		
		/// <summary>Deletes a control point here.</summary>
		public virtual VectorPoint DeleteControl(int id,VectorPath path){
			return null;
		}
		
		/// <summary>Gets a "close enough" progress point along this line. Essentially converts x,y to curve param t.</summary>
		public float ProgressAlongFast(float x,float y,float C,float D,float len_sq){
			
			float x1=Previous.X;
			float y1=Previous.Y;
			
			float A = x - x1;
			float B = y - y1;

			float dot = A * C + B * D;
			float param = 0;
			
			if (len_sq != 0){
				param = dot / len_sq;
			}
			
			return param;
		}

		/// <summary>Replaces this point with another.</summary>
		public void ReplaceWith(VectorPoint replacement,VectorPath path){
			
			if(replacement==null){
				return;
			}
			
			// Update next/prev:
			replacement.Next=Next;
			replacement.Previous=Previous;
			replacement.IsClose=IsClose;
			
			if(Next==null){
				path.LatestPathNode=replacement;
			}else{
				Next.Previous=replacement;
			}
			
			if(Previous==null){
				path.FirstPathNode=replacement;
			}else{
				Previous.Next=replacement;
			}
			
		}

		/// <summary>True if this path is unloaded. Used to improve font load time of large fonts.</summary>
		public virtual bool Unloaded{
			get{
				return false;
			}
		}
		
		/// <summary>Recalculates the minimum values and width/height of this path, taking curves into account.</summary>
		public virtual void RecalculateBounds(VectorPath path){
			
			if(X<path.MinX){
				path.MinX=X;
			}
			
			if(Y<path.MinY){
				path.MinY=Y;
			}
			
			// Width/height are used as max to save some memory:
			if(X>path.Width){
				path.Width=X;
			}
			
			if(Y>path.Height){
				path.Height=Y;
			}
			
		}
		
		/// <summary>Gets the point at the given t location.
		/// Similar to Split but doesn't apply the point to the path.
		/// Instead, the following point is added too.</summary>
		public virtual VectorPoint PointAt(float t,bool addNext){
			
			return null;
			
		}
		
		/// <summary>Splits this vector line into two at the given parametric point.</summary>
		public virtual VectorPoint Split(float t,VectorPath path){
			
			return null;
			
		}
		
		/// <summary>Steps along the line between this point and previous point at a fixed step, adding the points to the scanner as it goes.
		/// This one also informs the stepper of the current step.</summary>
		public virtual void ComputeLinePoints(PointReceiverStepped output){
			
		}
		
		/// <summary>Steps along the line between this point and previous point at a fixed step, adding the points to the scanner as it goes.</summary>
		public virtual void ComputeLinePoints(PointReceiver output){
			
		}
		
		/// <summary>Is this a curve line?</summary>
		public virtual bool IsCurve{
			get{
				return false;
			}
		}
		
		/// <summary>Is there a line from this point to the previous one?</summary>
		public virtual bool HasLine{
			get{
				return false;
			}
		}
		
		/// <summary>Gets the normal at the start of the line from this to previous.</summary>
		public virtual void StartNormal(out float x,out float y){
			x=0f;
			y=0f;
		}
		
		/// <summary>Gets the normal at the end of the line from this to previous.</summary>
		public virtual void EndNormal(out float x,out float y){
			x=0f;
			y=0f;
		}
		
		public virtual VectorPoint Copy(){
			
			return null;
			
		}
		
		public VectorPoint GetShapeEnd(){
			
			if(Next==null){
				return this;
			}
			
			VectorPoint current=Next;
			
			while(current!=null){
				
				// Nothing after it or the next one is a moveto:
				if(current.Next==null || !current.Next.HasLine){
					return current;
				}
				
				current=current.Next;
				
			}
			
			// This is actually unreachable:
			return null;
		}
		
		public virtual void Transform(VectorTransform transform){
			
			float x=X;
			X=(transform.XScale * x + transform.Scale01 * Y + transform.Dx);
			Y=(transform.Scale10 * x + transform.YScale * Y + transform.Dy);
			
		}
		
		public virtual bool IsClose{
			get{
				return false;
			}
			set{}
		}
		
		public override string ToString(){
			return "";
		}
		
		public virtual void Move(float x,float y){
			X+=x;
			Y+=y;
		}
		
		public virtual void Multiply(float x,float y){
			X*=x;
			Y*=y;
		}
		
		/// <summary>Axis flip.</summary>
		public virtual void Flip(){
			float x=X;
			X=Y;
			Y=x;
		}
		
		public virtual void Squash(float by){
			Y*=by;
		}
		
		public virtual void Sheer(float by){
			
			X+=Y*by;
			
		}
		
	}

}