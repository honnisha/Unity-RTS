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
		
		/// <summary>The x coordinate of the 2nd control point.</summary>
		public float Control2X;
		/// <summary>The y coordinate of the 2nd control point.</summary>
		public float Control2Y;
		
		
		/// <summary>Creates a new curve node for the given point.</summary>
		public CurveLinePoint(float x,float y):base(x,y){}
		
		public override void Transform(VectorTransform transform){
			
			float x=Control2X;
			Control2X=(transform.XScale * x + transform.Scale01 * Control2Y + transform.Dx);
			Control2Y=(transform.Scale10 * x + transform.YScale * Control2Y + transform.Dy);
			
			base.Transform(transform);
			
		}
		
		public override void Contains(float x,float y,ref bool contained){
			
			// Previous -> First Control:
			Contains(x,y,ref contained,Previous.X,Previous.Y,Control1X,Control1Y);
			
			// First Control -> Second control:
			Contains(x,y,ref contained,Control1X,Control1Y,Control2X,Control2Y);
			
			// Second control -> Current:
			Contains(x,y,ref contained,Control2X,Control2Y,X,Y);
			
		}
		
		public override float SignedArea(){
			
			// Note: Expanding this can cancel a term, but the operation count is a lot higher.
			
			return( 
				(Control1Y+Previous.Y) * (Control1X-Previous.X) +
				(Control2Y+Control1Y) * (Control2X-Control1X) + 
				(Y+Control2Y) * (X-Control2X)
			);
			
		}
		
		public override void RecalculateBounds(VectorPath path){
			
			// Take control point into account too:
			if(Control2X<path.MinX){
				path.MinX=Control2X;
			}
			
			if(Control2Y<path.MinY){
				path.MinY=Control2Y;
			}
			
			// Width/height are used as max to save some memory:
			if(Control2X>path.Width){
				path.Width=Control2X;
			}
			
			if(Control2Y>path.Height){
				path.Height=Control2Y;
			}
			
			// Start figuring out the length (very approximate)..
			
			float dx=Previous.X-Control1X;
			float dy=Previous.Y-Control1Y;
			
			double len=Math.Sqrt(dx*dx + dy*dy);
			
			dx=Control2X-Control1X;
			dy=Control2Y-Control1Y;
			
			len+=Math.Sqrt(dx*dx + dy*dy);
			
			dx=X-Control2X;
			dy=Y-Control2Y;
			
			len+=Math.Sqrt(dx*dx + dy*dy);
			
			Length=(float)len;
			
			BaseBounds(path);
			
		}
		
		public override VectorPoint AddControl(float x,float y,VectorPath path,out int id){
			
			id=0;
			
			// Just split the line:
			
			// Get the "progress" of x/y along the line:
			float C = X - Previous.X;
			float D = Y - Previous.Y;
			float len_sq = C * C + D * D;
			float t=ProgressAlongFast(x,y,C,D,len_sq);
			
			return Split(t,path);
			
		}

		public override VectorPoint DeleteControl(int id,VectorPath path){
			
			// Create:
			QuadLinePoint pt=new QuadLinePoint(X,Y);
			
			if(id==1){
				
				// Deleting control point 1.
				pt.Control1X=Control2X;
				pt.Control1Y=Control2Y;
				
			}else{
				
				// Deleting control point 2.
				pt.Control1X=Control1X;
				pt.Control1Y=Control1Y;
				
			}
			
			// Remove this and add in it's place:
			ReplaceWith(pt,path);
			
			return pt;
			
		}

		/// <summary>Samples this line at the given t value.</summary>
		public override void SampleAt(float t,out float x,out float y){
			
			float previousX3=Previous.X*3f;
			float control1X3=Control1X*3f;
			float control2X3=Control2X*3f;
			
			float previousY3=Previous.Y*3f;
			float control1Y3=Control1Y*3f;
			float control2Y3=Control2Y*3f;
			
			float tSquare=t*t;
			float tCube=tSquare*t;
			
			x = Previous.X + (-previousX3 + t * (previousX3 - Previous.X * t)) * t
			+ (control1X3 + t * (-2f * control1X3 + control1X3 * t)) * t
			+ (control2X3 - control2X3 * t) * tSquare
			+ X * tCube;
			
			y = Previous.Y + (-previousY3 + t * (previousY3 - Previous.Y * t)) * t
			+ (control1Y3 + t * (-2f * control1Y3 + control1Y3 * t)) * t
			+ (control2Y3 - control2Y3 * t) * tSquare
			+ Y * tCube;
			
		}
		
		/// <summary>Makes sure that this curve is a "simple" one
		/// (which can then be used to very quickly find offset curves).</summary>
		public void SimplifyCurve(VectorPath path){
			
			// Quick definition of what I'm calling a simple curve:
			// - Control points overlap, or (both of):
			// - No inflection point
			// - No loop
			
			// Loops are detected by checking if the line from 
			// Start->Control1 intersects Control2->End
			
			// Inflection points are detected by checking if the line 
			// from Control1->Control2 intersects the curve itself.
			// This simplifies to simply checking if the middle of that line
			// is 'equal' to the curve at 0.5c.
			
			// Conveniently, we need that midpoint if we need to simplify it anyway.
			// The simplification process involves only splitting the curve at 0.25, 0.5 and 0.75
			// which will always result in 4 "simple" curves.
			
			// 1. Control points overlap?
			if(Control1X == Control2X && Control1Y == Control2Y){
				// Yep! Simple already.
				return;
			}
			
			// 2. Inflection test.
			float x;
			float y;
			SampleAt(0.5f,out x,out y);
			
			float midX=Control1X + ((Control2X - Control1X) * 0.5f);
			float midY=Control1Y + ((Control2Y - Control1Y) * 0.5f);
			
			// Check if they're equal. If they are, 
			// we have an inflection and the curve is not simple.
			if(IsEqual(x,midX) && IsEqual(y,midY)){
				
				// Has an inflection point - simplify:
				SimplifyNow(path);
				return;
				
			}
			
			// Next, test if our lines intersect.
			// First though, let's define the actual lines (y=mx+c):
			float aM=(Control1X - Previous.X);
			float bM=(Control2X - X);
			
			if(aM==0f && bM==0f){
				// Both vertical.
				return;
			}
			
			// x/y will now represent the intersection point.
			
			// Either vertical?
			if(aM==0f){
				// A is vertical. Does B cross this X value?
				bM=(Control2Y - Y) / bM;
				float bC=Control2Y - (bM * Control2X);
				
				x=Control1X;
				y=bC+bM*x;
				
			}else if(bM==0f){
				// B is vertical. Does A cross this X value?
				aM=(Control1Y - Previous.Y) / aM;
				float aC=Control1Y - (aM * Control1X);
				
				x=X;
				y=aC+aM*x;
				
			}else{
				
				// Full intersect test:
				aM=(Control1Y - Previous.Y) / aM;
				bM=(Control2Y - Y) / bM;
				
				float aC=Control1Y - (aM * Control1X);
				float bC=Control2Y - (bM * Control2X);
				
				x=((bC-aC)/(aM-bM));
				y=(aC+aM*x);
				
			}
			
			// Bounds check! Is the intersect within the 
			// bounds of both lines? (C2->End first):
			float min=X;
			float max=Control2X;
			
			if(min>max){
				// Flip them over
				float T=min;
				min=max;
				max=T;
			}
			
			if(x>max||x<min){
				// No intersect.
				return;
			}
			
			min=Y;
			max=Control2Y;
			
			if(min>max){
				// Flip them over
				float T=min;
				min=max;
				max=T;
			}
			
			if(y>max||y<min){
				// No intersect.
				return;
			}
			
			// In range of the other line? (Start->C1)
			min=Previous.X;
			max=Control1X;
			
			if(min>max){
				// Flip them over
				float T=min;
				min=max;
				max=T;
			}
			
			if(x>max||x<min){
				// No intersect.
				return;
			}
			
			min=Previous.Y;
			max=Control1Y;
			
			if(min>max){
				// Flip them over
				float T=min;
				min=max;
				max=T;
			}
			
			if(y>max||y<min){
				// No intersect.
				return;
			}
			
			// They intersect!
			SimplifyNow(path);
			
		}
		
		/// <summary>Checks if two floats are equal within a predefined tolerance.</summary>
		private static bool IsEqual(float a,float b){
			return a==b || ( (b - 0.00001f) < a && a < (b + 0.00001f) );
		}
		
		/// <summary>Simplifies this path now using the given midpoint of the line.</summary>
		internal void SimplifyNow(VectorPath path){
			
			// Split down the middle, then do it again on those two sublines.
			Split(0.5f,path);
			
			// Split 2nd half:
			Next.Split(0.5f,path);
			
			// Split 1st half (this object again):
			Split(0.5f,path);
			
		}
		
		public override VectorPoint PointAt(float t,bool addNext){
			
			float invert=1f-t;
			
			float p0x=Previous.X;
			float p0y=Previous.Y;
			
			float p1x=Control1X;
			float p1y=Control1Y;
			
			float p2x=Control2X;
			float p2y=Control2Y;
			
			float p3x=X;
			float p3y=Y;
			
			// The new points:
			float p4x=p0x * invert + p1x * t;
			float p4y=p0y * invert + p1y * t;
			
			float p5x=p1x * invert + p2x * t;
			float p5y=p1y * invert + p2y * t;
			
			float p6x=p2x * invert + p3x * t;
			float p6y=p2y * invert + p3y * t;
			
			float p7x=p4x * invert + p5x * t;
			float p7y=p4y * invert + p5y * t;
			
			float p8x=p5x * invert + p6x * t;
			float p8y=p5y * invert + p6y * t;
			
			float p9x=p7x * invert + p8x * t;
			float p9y=p7y * invert + p8y * t;
			
			
			// Create the new first half:
			CurveLinePoint from=new CurveLinePoint(p9x,p9y);
			
			from.Control1X=p4x;
			from.Control1Y=p4y;
			from.Control2X=p7x;
			from.Control2Y=p7y;
			
			if(addNext){
				
				// Create the next one:
				CurveLinePoint to=new CurveLinePoint(p3x,p3y);
				
				to.Control1X=p8x;
				to.Control1Y=p8y;
				to.Control2X=p6x;
				to.Control2Y=p6y;
				
				to.Previous=from;
				from.Next=to;
				
				// Update length:
				to.Length=invert * Length;
				
				if(IsClose){
					to.IsClose=true;
				}
				
			}
			
			from.Length=t*Length;
			
			return from;
		}
		
		public override VectorPoint Split(float t,VectorPath path){
			
			float invert=1f-t;
			
			float p0x=Previous.X;
			float p0y=Previous.Y;
			
			float p1x=Control1X;
			float p1y=Control1Y;
			
			float p2x=Control2X;
			float p2y=Control2Y;
			
			float p3x=X;
			float p3y=Y;
			
			// The new points:
			float p4x=p0x * invert + p1x * t;
			float p4y=p0y * invert + p1y * t;
			
			float p5x=p1x * invert + p2x * t;
			float p5y=p1y * invert + p2y * t;
			
			float p6x=p2x * invert + p3x * t;
			float p6y=p2y * invert + p3y * t;
			
			float p7x=p4x * invert + p5x * t;
			float p7y=p4y * invert + p5y * t;
			
			float p8x=p5x * invert + p6x * t;
			float p8y=p5y * invert + p6y * t;
			
			float p9x=p7x * invert + p8x * t;
			float p9y=p7y * invert + p8y * t;
			
			
			// This curve will become the new 1st half:
			Control1X=p4x;
			Control1Y=p4y;
			
			Control2X=p7x;
			Control2Y=p7y;
			
			X=p9x;
			Y=p9y;
			
			// Create the next one:
			CurveLinePoint point=new CurveLinePoint(p3x,p3y);
			
			point.Control1X=p8x;
			point.Control1Y=p8y;
			point.Control2X=p6x;
			point.Control2Y=p6y;
			
			path.PathNodeCount++;
			
			// Insert after this:
			if(Next==null){
				path.LatestPathNode=point;
			}else{
				point.Next=Next;
				Next.Previous=point;
			}
			
			// Update lengths:
			point.Length=invert * Length;
			Length=t*Length;
			
			point.Previous=this;
			Next=point;
			
			if(IsClose){
				IsClose=false;
				point.IsClose=true;
				path.CloseNode.ClosePoint=point;
			}
			
			return point;
		}
		
		public override void NormalAt(float t,out float x,out float y){
			
			/*
			float tSquare=t*t;
			float inverse=1f-t;
			float inverseSquare=inverse*inverse;
			
			inverseSquare*=3f;
			inverse*=6f;
			tSquare*=3f;
			
			float p1=(inverseSquare - inverse * t);
			float p2=(inverse*t - tSquare);
			
			y=-(- Previous.X * inverseSquare
			+ Control1X * p1
			+ Control2X * p2
			+ X * tSquare);
			
			x=- Previous.Y * inverseSquare
			+ Control1Y * p1
			+ Control2Y * p2
			+ Y * tSquare;
			*/
			
			float x0;
			float y0;
			float x1;
			float y1;
			float x2;
			float y2;
			
			SampleAt(t-0.01f,out x0,out y0);
			SampleAt(t,out x1,out y1);
			SampleAt(t+0.01f,out x2,out y2);
			
			StraightLineNormal(x1-x0,y1-y0,out x0,out y0);
			StraightLineNormal(x2-x1,y2-y1,out x2,out y2);
			
			x=(x0+x2)/2f;
			y=(y0+y2)/2f;
			
			/*
			// Normalise:
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
			*/
		}
		
		public override void StartNormal(out float x,out float y){
			StraightLineNormal(Control1X-Previous.X,Control1Y-Previous.Y,out x,out y);
		}
		
		public override void EndNormal(out float x,out float y){
			StraightLineNormal(X-Control2X,Y-Control2Y,out x,out y);
		}
		
		public override void ComputeLinePoints(PointReceiver output){
			
			// Divide length by the amount we advance per pixel to get the number of pixels on this line:
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			
			// From but not including previous:
			float t=deltaProgress;
			
			float x=X;
			float y=Y;
			float previousX=Previous.X;
			float previousY=Previous.Y;
			
			float control1X3=Control1X*3f;
			float control2X3=Control2X*3f;
			float control1Y3=Control1Y*3f;
			float control2Y3=Control2Y*3f;
			
			float previousX3=previousX*3f;
			float previousY3=previousY*3f;
			
			// For each of the pixels:
			for(int i=0;i<pixels;i++){
				
				float tSquare=t*t;
				float tCube=tSquare*t;
				
				float pointX = previousX + (-previousX3 + t * (previousX3 - previousX * t)) * t
				+ (control1X3 + t * (-2f * control1X3 + control1X3 * t)) * t
				+ (control2X3 - control2X3 * t) * tSquare
				+ x * tCube;
				
				float pointY = previousY + (-previousY3 + t * (previousY3 - previousY * t)) * t
				+ (control1Y3 + t * (-2f * control1Y3 + control1Y3 * t)) * t
				+ (control2Y3 - control2Y3 * t) * tSquare
				+ y * tCube;
				
				// Add it:
				output.AddPoint(pointX,pointY);
				
				// Move progress:
				t+=deltaProgress;
				
			}
			
		}
		
		public override void ComputeLinePoints(PointReceiverStepped output){
			
			// Divide length by the amount we advance per pixel to get the number of pixels on this line:
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			
			// From but not including previous:
			float t=deltaProgress;
			
			float x=X;
			float y=Y;
			float previousX=Previous.X;
			float previousY=Previous.Y;
			
			float control1X3=Control1X*3f;
			float control2X3=Control2X*3f;
			float control1Y3=Control1Y*3f;
			float control2Y3=Control2Y*3f;
			
			float previousX3=previousX*3f;
			float previousY3=previousY*3f;
			
			// For each of the pixels:
			for(int i=0;i<pixels;i++){
				
				float tSquare=t*t;
				float tCube=tSquare*t;
				
				float pointX = previousX + (-previousX3 + t * (previousX3 - previousX * t)) * t
				+ (control1X3 + t * (-2f * control1X3 + control1X3 * t)) * t
				+ (control2X3 - control2X3 * t) * tSquare
				+ x * tCube;
				
				float pointY = previousY + (-previousY3 + t * (previousY3 - previousY * t)) * t
				+ (control1Y3 + t * (-2f * control1Y3 + control1Y3 * t)) * t
				+ (control2Y3 - control2Y3 * t) * tSquare
				+ y * tCube;
				
				// Add it:
				output.AddPoint(pointX,pointY,t);
				
				// Move progress:
				t+=deltaProgress;
				
			}
			
		}
		
		public override VectorPoint Copy(){
			
			CurveLinePoint point=new CurveLinePoint(X,Y);
			point.Length=Length;
			
			point.Control1X=Control1X;
			point.Control1Y=Control1Y;
			point.Control2X=Control2X;
			point.Control2Y=Control2Y;
			
			return point;
			
		}
		
		public override string ToString(){
			string res="bezierCurveTo("+Control1X+","+Control1Y+","+Control2X+","+Control2Y+","+X+","+Y+")";
			
			if(Close){
				return res+"\r\nclosePath()";
			}
			
			return res;
		}
		
		public override void Move(float x,float y){
			X+=x;
			Y+=y;
			Control1X+=x;
			Control1Y+=y;
			Control2X+=x;
			Control2Y+=y;
		}
		
		/// <summary>Axis flip.</summary>
		public override void Flip(){
			float x=Control2X;
			Control2X=Control2Y;
			Control2Y=x;
			base.Flip();
		}
		
		public override void Multiply(float x,float y){
			Control2X*=x;
			Control2Y*=y;
			base.Multiply(x,y);
		}
		
		public override void Squash(float by){
			Control2Y*=by;
			base.Squash(by);
		}
		
		public override void Sheer(float by){
			Control2X+=Control2Y*by;
			base.Sheer(by);
		}
		
	}
	
}