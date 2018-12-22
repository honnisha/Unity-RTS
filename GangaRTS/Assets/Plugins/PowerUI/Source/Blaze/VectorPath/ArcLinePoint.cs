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
	/// A node which immediately follows an arc.
	/// </summary>
	
	public partial class ArcLinePoint:VectorLine{
		
		/// <summary>The radius of the arc.</summary>
		public float Radius;
		/// <summmary>The target angle of the arc.</summary>
		public float EndAngle;
		/// <summmary>The starting angle.</summary>
		public float StartAngle;
		/// <summary>The x location of the center of the circle.</summary>
		public float CircleCenterX;
		/// <summary>The y location of the center of the circle.</summary>
		public float CircleCenterY;
		
		/// <summary>Creates a new arc node for the given point.</summary>
		public ArcLinePoint(float x,float y):base(x,y){}
		
		
		public override void Transform(VectorTransform transform){
			
			float x=CircleCenterX;
			CircleCenterX=(transform.XScale * x + transform.Scale01 * CircleCenterY + transform.Dx);
			CircleCenterY=(transform.Scale10 * x + transform.YScale * CircleCenterY + transform.Dy);
			
			base.Transform(transform);
			
		}
		
		public override void RecalculateBounds(VectorPath path){
			
			// Take control point into account too:
			if(CircleCenterX<path.MinX){
				path.MinX=CircleCenterX;
			}
			
			if(CircleCenterY<path.MinY){
				path.MinY=CircleCenterY;
			}
			
			// Width/height are used as max to save some memory:
			if(CircleCenterX>path.Width){
				path.Width=CircleCenterX;
			}
			
			if(CircleCenterY>path.Height){
				path.Height=CircleCenterY;
			}
			
			// How much must we rotate through overall?
			float angleToRotateThrough=EndAngle-StartAngle;
			
			// How long is the arc?
			// First, what portion of a full circle is it:
			float circlePortion=angleToRotateThrough/((float)Math.PI*2f);
			
			// Next, what's the circumference of that circle
			// (and the above portion of it, thus the length of the arc):
			Length=2f*(float)Math.PI*Radius * circlePortion;
			
			base.RecalculateBounds(path);
			
		}
		
		public override void ComputeLinePoints(PointReceiver output){
			
			float radius=Radius;
			
			// How much must we rotate through overall?
			float angleToRotateThrough=EndAngle-StartAngle;
			
			// The number of pixels:
			int pixelCount=(int)Math.Ceiling(Length/output.SampleDistance);
			
			if(pixelCount<0){
				// Going anti-clockwise. Invert deltaAngle and the pixel count:
				pixelCount=-pixelCount;
			}
			
			// So arc length is how many pixels long the arc is.
			// Thus to step that many times, our delta angle is..
			float deltaAngle=angleToRotateThrough/(float)pixelCount;
			
			// The current angle:
			float currentAngle=StartAngle;
			
			// Step pixel count times:
			for(int i=0;i<pixelCount;i++){
				// Map from polar angle to coords:
				float x=radius * (float) Math.Cos(currentAngle);
				float y=radius * (float) Math.Sin(currentAngle);
				
				x+=CircleCenterX;
				y+=CircleCenterY;
				
				output.AddPoint(x,y);
				
				// Rotate the angle:
				currentAngle+=deltaAngle;
			}
			
		}
		
		public override void ComputeLinePoints(PointReceiverStepped output){
			
			float radius=Radius;
			
			// How much must we rotate through overall?
			float angleToRotateThrough=EndAngle-StartAngle;
			
			// The number of pixels:
			int pixelCount=(int)Math.Ceiling(Length/output.SampleDistance);
			
			if(pixelCount<0){
				// Going anti-clockwise. Invert deltaAngle and the pixel count:
				pixelCount=-pixelCount;
			}
			
			// So arc length is how many pixels long the arc is.
			// Thus to step that many times, our delta angle is..
			float deltaAngle=angleToRotateThrough/(float)pixelCount;
			
			// The current angle:
			float currentAngle=StartAngle;
			
			// From but not including previous:
			float deltaC=1f/(float)pixelCount;
			float c=deltaC;
			
			// Step pixel count times:
			for(int i=0;i<pixelCount;i++){
				// Map from polar angle to coords:
				float x=radius * (float) Math.Cos(currentAngle);
				float y=radius * (float) Math.Sin(currentAngle);
				
				x+=CircleCenterX;
				y+=CircleCenterY;
				
				output.AddPoint(x,y,c);
				
				// Rotate the angle:
				currentAngle+=deltaAngle;
				
				c+=deltaC;
			}
			
		}
		
		/// <summary>Samples this line at the given t value.</summary>
		public override void SampleAt(float t,out float x,out float y){
			
			// How much must we rotate through overall?
			float angleToRotateThrough=EndAngle-StartAngle;
			
			float currentAngle=StartAngle + t*angleToRotateThrough;
			
			x=Radius * (float) Math.Cos(currentAngle);
			y=Radius * (float) Math.Sin(currentAngle);
			
			x+=CircleCenterX;
			y+=CircleCenterY;
			
		}
		
		/// <summary>Is this a curve line?</summary>
		public override bool IsCurve{
			get{
				return true;
			}
		}
		
		public override VectorPoint Copy(){
			
			ArcLinePoint point=new ArcLinePoint(X,Y);
			point.Length=Length;
			
			point.Length=Length;
			point.Radius=Radius;
			point.EndAngle=EndAngle;
			point.StartAngle=StartAngle;
			point.CircleCenterX=CircleCenterX;
			point.CircleCenterY=CircleCenterY;
			
			return point;
			
		}
		
		public override string ToString(){
			return "arcTo("+CircleCenterX+","+CircleCenterY+","+X+","+Y+","+Radius+")";
		}
		
		/*
		public override void Multiply(float by){
			Control1X*=by;
			Control1Y*=by;
			base.Multiply(by);
		}
		
		public override void Squash(float by){
			Control1Y*=by;
			base.Squash(by);
		}
		
		public override void Sheer(float by){
			Control1X+=Control1Y*by;
			base.Sheer(by);
		}
		*/
		
	}
	
}