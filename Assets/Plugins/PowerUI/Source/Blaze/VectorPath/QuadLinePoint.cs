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
		
		/// <summary>The x coordinate of the 1st control point.</summary>
		public float Control1X;
		/// <summary>The y coordinate of the 1st control point.</summary>
		public float Control1Y;
		
		/// <summary>Creates a new curve node for the given point.</summary>
		public QuadLinePoint(float x,float y):base(x,y){}
		
		public override void Transform(VectorTransform transform){
			
			float x=Control1X;
			Control1X=(transform.XScale * x + transform.Scale01 * Control1Y + transform.Dx);
			Control1Y=(transform.Scale10 * x + transform.YScale * Control1Y + transform.Dy);
			
			base.Transform(transform);
			
		}
		
		public override void Contains(float x,float y,ref bool contained){
			
			// Previous -> First Control:
			Contains(x,y,ref contained,Previous.X,Previous.Y,Control1X,Control1Y);
			
			// First Control -> Current:
			Contains(x,y,ref contained,Control1X,Control1Y,X,Y);
			
		}
		
		protected void Contains(float x,float y,ref bool contained,float prevX,float prevY,float curX,float curY){
			
			// Figure out the bounding box of the line.
			// We're going to see if the point is outside it - if so, skip.
			
			float minX=(curX<prevX)?curX:prevX;
			
			// Point is to the left of tbe bounding box - ignore.
			if(minX>x){
				return;
			}
			
			float maxX=(curX>prevX)?curX:prevX;
			
			// Point is to the right of this lines bounding box - ignore.
			// We do an inclusive ignore here as the line attached to this one might include it too.
			if(maxX<=x){
				return;
			}
			
			float minY=(curY<prevY)?curY:prevY;
			
			// Point is below this lines bounding box - ignore.
			if(minY>y){
				return;
			}
			
			// Special case if the point is above.
			float maxY=(curY>prevY)?curY:prevY;
			
			// We do an inclusive check here as the line attached to this one might include it too.
			if(maxY<=y){
				//The point is above for sure.
				contained=!contained;
				return;
			}
			
			
			// It's sloping. What side of the line are we on? If we're on the right, the line is to the left.
			float gradient=(prevY-curY)/(prevX-curX);
			float c=curY-(gradient*curX);
			
			// y<=mx+c means we're on the right, or on the line.
			if(((gradient*x)+c)<=y){
				contained=!contained;
			}
			
		}
		
		public override float SignedArea(){
			
			// Note: Expanding this can cancel a term, but the operation count is a lot higher.
			
			return( 
				(Control1Y+Previous.Y) * (Control1X-Previous.X) +
				(Y+Control1Y) * (X-Control1X)
			);
			
		}
		
		public override VectorPoint DeleteControl(int id,VectorPath path){
			
			// Create:
			VectorPoint pt=new StraightLinePoint(X,Y);
			
			// Remove this and add in it's place:
			ReplaceWith(pt,path);
			
			return pt;
			
		}

		public override VectorPoint AddControl(float x,float y,VectorPath path,out int id){
			
			// Create:
			CurveLinePoint pt=new CurveLinePoint(X,Y);
			
			// Get the "progress" of x/y along the line, vs control point progress.
			float C = X - Previous.X;
			float D = Y - Previous.Y;
			float len_sq = C * C + D * D;
			
			float newProg=ProgressAlongFast(x,y,C,D,len_sq);
			float p1Prog=ProgressAlongFast(Control1X,Control1Y,C,D,len_sq);
			
			// Should this new control be control point #1?
			bool first=(newProg < p1Prog);
			
			if(first){
				
				// Pt 1:
				pt.Control1X=x;
				pt.Control1Y=y;
				
				pt.Control2X=Control1X;
				pt.Control2Y=Control1Y;
				
				id=1;
				
			}else{
				
				// Pt 2:
				pt.Control1X=Control1X;
				pt.Control1Y=Control1Y;
				
				pt.Control2X=x;
				pt.Control2Y=y;
				
				id=2;
				
			}
			
			// Remove this and add in it's place:
			ReplaceWith(pt,path);
			
			return pt;
			
		}

		public override void StartNormal(out float x,out float y){
			StraightLineNormal(Control1X-Previous.X,Control1Y-Previous.Y,out x,out y);
		}
		
		public override void EndNormal(out float x,out float y){
			StraightLineNormal(X-Control1X,Y-Control1Y,out x,out y);
		}
		
		public override VectorPoint PointAt(float t,bool addNext){
			
			float invert=1f-t;
			
			float p0x=Previous.X;
			float p0y=Previous.Y;
			
			float p1x=Control1X;
			float p1y=Control1Y;
			
			float p2x=X;
			float p2y=Y;
			
			// The new points:
			float p3x=p0x * invert + p1x * t;
			float p3y=p0y * invert + p1y * t;
			
			float p4x=p1x * invert + p2x * t;
			float p4y=p1y * invert + p2y * t;
			
			float p5x=p3x * invert + p4x * t;
			float p5y=p3y * invert + p4y * t;
			
			// This curve will become the new 1st half:
			QuadLinePoint from=new QuadLinePoint(p5x,p5y);
			
			from.Control1X=p3x;
			from.Control1Y=p3y;
			
			if(addNext){
				
				// Create the next one:
				QuadLinePoint to=new QuadLinePoint(p2x,p2y);
				
				to.Control1X=p4x;
				to.Control1Y=p4y;
				
				// Update length:
				to.Length=invert * Length;
				
				to.Previous=from;
				from.Next=to;
				
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
			
			float p2x=X;
			float p2y=Y;
			
			// The new points:
			float p3x=p0x * invert + p1x * t;
			float p3y=p0y * invert + p1y * t;
			
			float p4x=p1x * invert + p2x * t;
			float p4y=p1y * invert + p2y * t;
			
			float p5x=p3x * invert + p4x * t;
			float p5y=p3y * invert + p4y * t;
			
			// This curve will become the new 1st half:
			Control1X=p3x;
			Control1Y=p3y;
			
			X=p5x;
			Y=p5y;
			
			path.PathNodeCount++;
			
			// Create the next one:
			QuadLinePoint point=new QuadLinePoint(p2x,p2y);
			
			point.Control1X=p4x;
			point.Control1Y=p4y;
			
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
		
		public virtual void NormalAt(float t,out float x,out float y){
			
			/*
			float t2=2f*t;
			float controlFactor=2f-(4f * t);
			float invertSquare=-2f+t2;
			
			// Compute the tangent and flip them (this is why x is on y):
			y=-(invertSquare*Previous.X + controlFactor*Control1X + t2*X);
			x=invertSquare*Previous.Y + controlFactor*Control1Y + t2*Y;
			
			// Normalise:
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
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
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
			
		}
		
		public void StraightLineNormal(float dx,float dy,out float x,out float y){
			
			x=-dy;
			y=dx;
			
			float length=(float)Math.Sqrt( (x*x)+(y*y) );
			
			x/=length;
			y/=length;
			
		}
		
		public override void ComputeLinePoints(PointReceiver output){
			
			// Get previous:
			float x=X;
			float y=Y;
			float previousX=Previous.X;
			float previousY=Previous.Y;
			float control1X2=Control1X * 2f;
			float control1Y2=Control1Y * 2f;
			
			// Divide length by the amount we advance per pixel to get the number of pixels on this line:
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			
			// From but not including previous:
			float t=deltaProgress;
			float invert=(1f-t);
			
			// For each of the pixels:
			for(int i=0;i<pixels;i++){
				
				float tSquare=t*t;
				float controlFactor=t*invert;
				float invertSquare=invert*invert;
				
				// Figure out the point:
				float pointX=invertSquare*previousX + controlFactor*control1X2 + tSquare*x;
				float pointY=invertSquare*previousY + controlFactor*control1Y2 + tSquare*y;
				
				// Add it:
				output.AddPoint(pointX,pointY);
				
				// Move progress:
				t+=deltaProgress;
				invert-=deltaProgress;
				
			}
			
		}
		
		public override void ComputeLinePoints(PointReceiverStepped output){
			
			// Get previous:
			float x=X;
			float y=Y;
			float previousX=Previous.X;
			float previousY=Previous.Y;
			float control1X2=Control1X * 2f;
			float control1Y2=Control1Y * 2f;
			
			// Divide length by the amount we advance per pixel to get the number of pixels on this line:
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			
			// From but not including previous:
			float t=deltaProgress;
			float invert=(1f-t);
			
			// For each of the pixels:
			for(int i=0;i<pixels;i++){
				
				float tSquare=t*t;
				float controlFactor=t*invert;
				float invertSquare=invert*invert;
				
				// Figure out the point:
				float pointX=invertSquare*previousX + controlFactor*control1X2 + tSquare*x;
				float pointY=invertSquare*previousY + controlFactor*control1Y2 + tSquare*y;
				
				// Add it:
				output.AddPoint(pointX,pointY,t);
				
				// Move progress:
				t+=deltaProgress;
				invert-=deltaProgress;
				
			}
			
		}
		
		public override void RecalculateBounds(VectorPath path){
			
			// Take control point into account too:
			if(Control1X<path.MinX){
				path.MinX=Control1X;
			}
			
			if(Control1Y<path.MinY){
				path.MinY=Control1Y;
			}
			
			// Width/height are used as max to save some memory:
			if(Control1X>path.Width){
				path.Width=Control1X;
			}
			
			if(Control1Y>path.Height){
				path.Height=Control1Y;
			}
			
			/*
			// Start figuring out the length..
			float vaX=Previous.X-(2f*Control1X)+X;
			float vaY=Previous.Y-(2f*Control1Y)+Y;
			
			float vbX=(2f*Control1X) - (2f*Previous.X);
			float vbY=(2f*Control1Y) - (2f*Previous.Y);
			
			float a=4f*((vaX*vaX) + (vaY*vaY));
			
			float b=4f*((vaX*vbX) + (vaY*vbY));
			
			float c=(vbX*vbX) + (vbY*vbY);
			
			float rootABC = 2f*(float)Math.Sqrt(a+b+c);
			float rootA = (float)Math.Sqrt(a);
			float aRootA = 2f*a*rootA;
			
			if(aRootA==0f){
				
				Length=0f;
				
			}else{
				
				float rootC = 2f*(float)Math.Sqrt(c);
				float bA = b/rootA;
				
				Length=(
					aRootA * rootABC + rootA*b*(rootABC-rootC) + (4f*c*a - b*b)*(float)Math.Log(
						(2f*rootA+bA+rootABC) / (bA+rootC)
					)
				) / (4f*aRootA);
			
			}
			*/
			
			// Start figuring out the length (very approximate)..
			
			float dx=Previous.X-Control1X;
			float dy=Previous.Y-Control1Y;
			
			double len=Math.Sqrt(dx*dx + dy*dy);
			
			dx=X-Control1X;
			dy=Y-Control1Y;
			
			len+=Math.Sqrt(dx*dx + dy*dy);
			
			Length=(float)len;
			
			base.RecalculateBounds(path);
			
		}
		
		public void BaseBounds(VectorPath path){
			base.RecalculateBounds(path);
		}
		
		/// <summary>Samples this line at the given t value.</summary>
		public override void SampleAt(float t,out float x,out float y){
			
			float invert=(1f-t);
			float tSquare=t*t;
			float controlFactor=t*invert*2f;
			float invertSquare=invert*invert;
			
			// Figure out the point:
			x=invertSquare*Previous.X + controlFactor*Control1X + tSquare*X;
			y=invertSquare*Previous.Y + controlFactor*Control1Y + tSquare*Y;
			
		}
		
		/// <summary>Is this a curve line?</summary>
		public override bool IsCurve{
			get{
				return true;
			}
		}
		
		public override VectorPoint Copy(){
			
			QuadLinePoint point=new QuadLinePoint(X,Y);
			point.Length=Length;
			
			point.Control1X=Control1X;
			point.Control1Y=Control1Y;
			
			return point;
			
		}
		
		public override string ToString(){
			string res="quadraticCurveTo("+Control1X+","+Control1Y+","+X+","+Y+")";
			
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
		}
		
		/// <summary>Axis flip.</summary>
		public override void Flip(){
			float x=Control1X;
			Control1X=Control1Y;
			Control1Y=x;
			base.Flip();
		}
		
		public override void Multiply(float x,float y){
			Control1X*=x;
			Control1Y*=y;
			base.Multiply(x,y);
		}
		
		public override void Squash(float by){
			Control1Y*=by;
			base.Squash(by);
		}
		
		public override void Sheer(float by){
			Control1X+=Control1Y*by;
			base.Sheer(by);
		}
		
	}
	
}