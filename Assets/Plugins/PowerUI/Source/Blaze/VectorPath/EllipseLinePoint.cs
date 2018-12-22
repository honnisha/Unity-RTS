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
	/// A node which immediately follows an ellipse.
	/// </summary>
	
	public partial class EllipseLinePoint:VectorLine{
		
		/// <summary>The computed sweep angle.</summary>
		public float SweepAngle;
		/// <summary>The computed start angle.</summary>
		public float StartAngle;
		/// <summary>Sin of the x axis rotation.</summary>
		public float SinXAxis;
		/// <summary>Cos of the x axis rotation.</summary>
		public float CosXAxis;
		/// <summary>The computed center X.</summary>
		public float CenterX;
		/// <summary>The computed center Y.</summary>
		public float CenterY;
		/// <summary>The resultant X radius.</summary>
		public float RadiusX;
		/// <summary>The resultant Y radius.</summary>
		public float RadiusY;
		
		/*
		/// <summary>The large arc flag.</summary>
		public bool LargeArc;
		/// <summary>The sweep flag.</summary>
		public bool Sweep;
		/// <summary>The raw x axis rotation.</summary>
		public float RotationX;
		*/
		
		
		/// <summary>EllipseLinePoint a new arc node for the given point.</summary>
		public EllipseLinePoint(float x,float y):base(x,y){}
		
		public override void RecalculateBounds(VectorPath path){
			
			// Take control point into account too:
			if(CenterX<path.MinX){
				path.MinX=CenterX;
			}
			
			if(CenterY<path.MinY){
				path.MinY=CenterY;
			}
			
			// Width/height are used as max to save some memory:
			if(CenterX>path.Width){
				path.Width=CenterX;
			}
			
			if(CenterY>path.Height){
				path.Height=CenterY;
			}
			
			// Proportion of the overall length (0-1):
			float lengthPortion=SweepAngle / 2f * (float)Math.PI;
			
			// Approximate ellipse length:
			float h = (float) ( Math.Pow((RadiusX-RadiusY), 2f) / Math.Pow((RadiusX+RadiusY), 2f) );
			
			Length = lengthPortion * ( ((float)Math.PI * ( RadiusX + RadiusY )) * 
				(1f + ( (3f * h) / ( 10f + (float)Math.Sqrt( 4f - (3f * h) )) )) );
			
			base.RecalculateBounds(path);
			
		}
		
		public override void ComputeLinePoints(PointReceiver output){
			
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			float c=deltaProgress;
			
			// Step pixel count times:
			for(int i=0;i<pixels;i++){
				
				// From http://www.w3.org/TR/SVG/implnote.html#ArcParameterizationAlternatives
				float angle = StartAngle+(SweepAngle*c);
				float ellipseComponentX = RadiusX*(float)Math.Cos(angle);
				float ellipseComponentY = RadiusY*(float)Math.Sin(angle);
				
				float x = CosXAxis*ellipseComponentX - SinXAxis*ellipseComponentY + CenterX;
				float y = SinXAxis*ellipseComponentX + CosXAxis*ellipseComponentY + CenterY;
				
				output.AddPoint(x,y);
				
				c+=deltaProgress;
			}
			
		}
		
		public override void ComputeLinePoints(PointReceiverStepped output){
			
			int pixels=(int)(Length/output.SampleDistance);
			
			if(pixels<=0){
				pixels=1;
			}
			
			// Run along the line as a 0-1 progression value.
			float deltaProgress=1f/(float)pixels;
			float c=deltaProgress;
			
			// Step pixel count times:
			for(int i=0;i<pixels;i++){
				
				// From http://www.w3.org/TR/SVG/implnote.html#ArcParameterizationAlternatives
				float angle = StartAngle+(SweepAngle*c);
				float ellipseComponentX = RadiusX*(float)Math.Cos(angle);
				float ellipseComponentY = RadiusY*(float)Math.Sin(angle);
				
				float x = CosXAxis*ellipseComponentX - SinXAxis*ellipseComponentY + CenterX;
				float y = SinXAxis*ellipseComponentX + CosXAxis*ellipseComponentY + CenterY;
				
				output.AddPoint(x,y,c);
				
				c+=deltaProgress;
			}
			
		}
		
		/// <summary>Samples this line at the given t value.</summary>
		public override void SampleAt(float t,out float x,out float y){
			
			// From http://www.w3.org/TR/SVG/implnote.html#ArcParameterizationAlternatives
			float angle = StartAngle+(SweepAngle*t);
			float ellipseComponentX = RadiusX*(float)Math.Cos(angle);
			float ellipseComponentY = RadiusY*(float)Math.Sin(angle);
			
			x = CosXAxis*ellipseComponentX - SinXAxis*ellipseComponentY + CenterX;
			y = SinXAxis*ellipseComponentX + CosXAxis*ellipseComponentY + CenterY;
			
		}
		
		/// <summary>Is this a curve line?</summary>
		public override bool IsCurve{
			get{
				return true;
			}
		}
		
		public override VectorPoint Copy(){
			
			EllipseLinePoint point=new EllipseLinePoint(X,Y);
			point.Length=Length;
			point.SweepAngle=SweepAngle;
			point.StartAngle=StartAngle;
			point.SinXAxis=SinXAxis;
			point.CosXAxis=CosXAxis;
			point.CenterX=CenterX;
			point.CenterY=CenterY;
			point.RadiusX=RadiusX;
			point.RadiusY=RadiusY;
		
			return point;
			
		}
		
		public override string ToString(){
			return "ellipseTo("+CenterX+","+CenterY+","+X+","+Y+","+RadiusX+","+RadiusY+")";
		}
		
	}
	
}