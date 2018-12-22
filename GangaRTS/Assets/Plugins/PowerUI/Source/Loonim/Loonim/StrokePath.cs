//--------------------------------------
//	   Loonim Image Generator
//	Partly derived from LibNoise
//	See License.txt for more info
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using Blaze;
using System.Collections;
using System.Collections.Generic;


namespace Loonim{
	
	/// <summary>Helper class for generating stroke paths.</summary>
	public class StrokePath : PointReceiverStepped{
		
		/// <summary>The curve sampling accuracy (in terms of pixels).</summary>
		public float Accuracy=1f;
		/// <summary>The line cap mode.</summary>
		public int LineCapMode;
		/// <summary>The line join mode.</summary>
		public int LineJoinMode;
		/// <summary>The width of the stroke.</summary>
		public float Width;
		/// <summary>The miter limit.</summary>
		public float MiterLimit=4f;
		/// <summary>A width function. Varies the width of the stroke (between 1 and 0).
		/// It's multiplied by Width.</summary>
		public CurveSampler WidthFunction;
		/// <summary>The current line.</summary>
		private VectorLine CurrentLine;
		/// <summary>The current line length used when computing UV's.</summary>
		private float OuterLength;
		/// <summary>The current line length used when computing UV's.</summary>
		private float InnerLength;
		/// <summary>The number of verts on the inner edge. Not necessarily equal to InnerSet.Count.</summary>
		private int InnerCount;
		/// <summary>The number of verts on the outer edge. Not necessarily equal to OuterSet.Count.</summary>
		private int OuterCount;
		/// <summary>True if we're sampling the outer curve.</summary>
		private bool Outer=false;
		/// <summary>True when we just started a new line. Used to detect when to apply linejoin.</summary>
		private bool ApplyLineJoinNow;
		/// <summary>The end normal of the current line. Used to detect where to apply linejoin.</summary>
		private Vector2 EndNormal;
		/// <summary>The start normal of the current line. Used to detect where to apply linejoin.</summary>
		private Vector2 StartNormal;
		/// <summary>True when we've seen at least one line on this contour.</summray>
		private bool AtLeastOneLine;
		/// <summary>The computed miter length, after applying the miter limit.</summary>
		private float MiterLength;
		/// <summary>Used during linejoin.</summary>
		private Vector2 OriginalLinePoint;
		/// <summary>The angle between the two lines. Used by linejoin.</summary>
		private float LineAngle;
		/// <summary>The computed set of verts.</summary>
		private List<StrokePoint> OuterSet=new List<StrokePoint>();
		/// <summary>The computed set of verts.</summary>
		private List<StrokePoint> InnerSet=new List<StrokePoint>();
		
		
		/// <summary>Generates a stroke now using these settings.
		/// Note that the given path may be modified.</summary>
		public void Generate(VectorPath path){
			
			// First, simplify the path (eliminates loops etc):
			path.SimplifyCurve();
			
			// Always recalculate bounds:
			path.RecalculateBounds();
			
			// For each line, we can now generate the two 
			// offset curves and sample them individually.
			
			float halfWidth=Width / 2f;
			
			// Tidy values:
			OuterLength=0f;
			InnerLength=0f;
			InnerCount=0;
			OuterCount=0;
			float nx;
			float ny;
			
			// Are we using linejoin?
			bool lineJoinActive=(LineJoinMode & StrokeLineMode.JoinActive)!=0;
			
			VectorPoint current=path.FirstPathNode;
			
			while(current!=null){
				
				VectorLine line=current as VectorLine;
				
				// Set the current line:
				CurrentLine=line;
				
				if(line==null){
					
					// Move to. Complete the previous contour:
					CompleteContour((current as MoveToPoint).ClosePoint!=null);
					
					// Go to next:
					current=current.Next;
					continue;
					
				}
				
				if(lineJoinActive){
					// Get start normal:
					line.StartNormal(out nx,out ny);
					StartNormal=new Vector2(nx,ny);
				}
				
				// First of line:
				bool applyLineJoin=AtLeastOneLine && lineJoinActive;
				
				if(applyLineJoin){
					
					// Compare StartNormal with EndNormal and figure out if we're adding extra verts to
					// inner or outer.
					
					// Original point:
					OriginalLinePoint=new Vector2(
						current.Previous.X,
						current.Previous.Y
					);
					
					// Get the angle between them:
					float angle=(float)System.Math.Atan2( 
						StartNormal.x*EndNormal.y - StartNormal.y*EndNormal.x,
						StartNormal.x*EndNormal.x + StartNormal.y*EndNormal.y
					);
					
					// If we're in miter mode, find the miter length.
					// If it exceeds the miter limit, set angle to 0 so we just end up with a bevel.
					if(LineJoinMode==StrokeLineMode.Miter){
						
						// Compute the miter length now:
						float miterRatio = 1f / (float)System.Math.Sin ( angle / 2f );
						
						if(miterRatio>MiterLimit || -miterRatio>MiterLimit){
							// Out of range!
							angle=0f;
						}else{
							// Set the miter length now:
							MiterLength = miterRatio * halfWidth;
						}
						
					}
					
					LineAngle=angle;
					
					// Very close to zero -> Do nothing (this is ~5 degrees):
					if(angle>0.08f){
						
						// Apply join to outer:
						ApplyLineJoinNow=true;
						applyLineJoin=false;
						
					}else if(angle>-0.08f){
						applyLineJoin=false;
					}
					
					// Else apply join to inner (by leaving applyLineJoin true).
					
				}
				
				
				// Change to outer curve:
				Outer=true;
				
				// Extrude and sample it:
				current.ExtrudeAndSample(path,halfWidth,this);
				
				// Increase length so far:
				OuterLength+=line.Length;
				
				// Change to inner curve:
				Outer=false;
				
				if(applyLineJoin){
					// Apply join to inner:
					ApplyLineJoinNow=true;
				}
				
				// Extrude and sample it:
				current.ExtrudeAndSample(path,-halfWidth,this);
				
				// Increase length so far:
				InnerLength+=line.Length;
				
				if(lineJoinActive){
					// Get end normal:
					line.EndNormal(out nx,out ny);
					EndNormal=new Vector2(nx,ny);
				}
				
				// We've seen at least one line:
				AtLeastOneLine=true;
				
				// Next:
				current=current.Next;
				
			}
			
			// Complete the final contour:
			CompleteContour(path.LatestPathNode.IsClose);
			
		}
		
		public bool IncludeFirstNode{
			get{
				return true;
			}
		}
		
		public float SampleDistance{
			get{
				return Accuracy;
			}
			set{
				Accuracy=value;
			}
		}
		
		/// <summary>Called when we're starting to emit a mesh.</summary>
		protected virtual void StartMesh(bool closed,int lineCount){}
		
		/// <summary>Called when we're emitting a line segment.</summary>
		protected virtual void EmitLine(StrokePoint inner,StrokePoint outer){}
		
		/// <summary>Called when we're done emitting a mesh.</summary>
		protected virtual void EndMesh(){}
		
		public void MoveTo(float x,float y){}
		
		/// <summary>Called when a contour is completed.</summary>
		private void CompleteContour(bool closed){
			
			AtLeastOneLine=false;
			
			if(InnerCount==0 || OuterCount==0){
				return;
			}
			
			// We have a computed line to deal with!
			
			// First, make those C values relative to the total length of their lines.
			for(int i=0;i<OuterCount;i++){
				StrokePoint sp=OuterSet[i];
				sp.C /= OuterLength;
				OuterSet[i]=sp;
			}
			
			for(int i=0;i<InnerCount;i++){
				StrokePoint sp=InnerSet[i];
				sp.C /= InnerLength;
				InnerSet[i]=sp;
			}
			
			// Next, we'll step along the edges, pairing nodes together and emitting them.
			
			// Index in the other edge:
			int otherIndex=0;
			
			int innerMax=InnerCount-1;
			int outerMax=OuterCount-1;
			
			// Biggest number of verts:
			if(InnerCount > OuterCount){
				
				// We'll be emitting InnerCount lines:
				StartMesh(closed,InnerCount);
				
				// The inner edge is longer. It will set the pace.
				for(int i=0;i<InnerCount;i++){
					
					// Compare inner.C with the two nearest verts in OuterSet.
					
					// If this is the last index, we always emit both maxes:
					if(i==innerMax){
						otherIndex=outerMax;
					}else if(otherIndex!=outerMax){
						
						// Get the two C values to compare with:
						float currentC=InnerSet[i].C;
						float firstC=currentC-OuterSet[otherIndex].C;
						float secondC=OuterSet[otherIndex+1].C-currentC;
						
						// If firstC is -ve, then use otherIndex.
						// If secondC is -ve then use otherIndex+1.
						// Otherwise, the smallest +ve number wins.
						
						if(firstC>0f && (secondC<=0f || secondC < firstC) ){
							
							// Pair with otherIndex+1.
							otherIndex++;
							
						}
						
					}
					
					// Pair now:
					EmitLine(InnerSet[i],OuterSet[otherIndex]);
					
				}
				
			}else{
				
				// We'll be emitting OuterCount lines:
				StartMesh(closed,OuterCount);
				
				// The outer edge is longer. It will set the pace.
				for(int i=0;i<OuterCount;i++){
					
					// Compare outer.C with the two nearest verts in InnerSet.
					
					// If this is the last index, we always emit both maxes:
					if(i==outerMax){
						otherIndex=innerMax;
					}else if(otherIndex!=innerMax){
						
						// Get the two C values to compare with:
						float currentC=OuterSet[i].C;
						float firstC=currentC-InnerSet[otherIndex].C;
						float secondC=InnerSet[otherIndex+1].C-currentC;
						
						// If firstC is -ve, then use otherIndex.
						// If secondC is -ve then use otherIndex+1.
						// Otherwise, the smallest +ve number wins.
						
						if(firstC>0f && (secondC<=0f || secondC < firstC) ){
							
							// Pair with otherIndex+1.
							otherIndex++;
							
						}
						
					}
					
					// Pair now:
					EmitLine(InnerSet[otherIndex],OuterSet[i]);
					
				}
				
			}
			
			// Done!
			EndMesh();
			
			// Clear the values:
			InnerCount=0;
			OuterCount=0;
			InnerLength=0f;
			OuterLength=0f;
			
		}
		
		/// <summary>Adds points to the given set for a circle of the given angle, starting at a point about a center.</summary>
		private void DrawCircle(List<StrokePoint> set,ref int count,float angle,float pX,float pY,float c,Vector2 center){
			
			// The length of the arc:
			float arc = (Width / 2f) * angle;
			
			if(arc<0f){
				arc=-arc;
			}
			
			// Divide it by accuracy to get the # of points to add:
			int segmentsToAdd=(int)( arc / Accuracy )-1;
			
			if(segmentsToAdd<=0){
				return;
			}
			
			// Delta angle:
			float deltaAngle=-angle / (float)(segmentsToAdd+1);
			
			// The point to rotate:
			float rotateX=pX - center.x;
			float rotateY=pY - center.y;
			
			arc=deltaAngle;
			
			for(int i=0;i<segmentsToAdd;i++){
				
				// Rotate the previous point about center
				// by 'arc'
				
				// Get cos/sin:
				float cs = (float)System.Math.Cos(arc);
				float sn = (float)System.Math.Sin(arc);
				
				// Compute the point:
				float rotatedX = (rotateX * cs - rotateY * sn) + center.x;
				float rotatedY = (rotateX * sn + rotateY * cs) + center.y;
				
				if(count==set.Count){
					set.Add(new StrokePoint(rotatedX,rotatedY,c));
				}else{
					set[count]=new StrokePoint(rotatedX,rotatedY,c);
				}
				
				count++;
				arc+=deltaAngle;
				
			}
			
		}
		
		public void AddPoint(float x,float y,float c){
			
			// Note that we don't know the total line length yet because of how extrude works.
			List<StrokePoint> set;
			int count;
			
			if(Outer){
				set=OuterSet;
				count=OuterCount;
				
				// Make c relative to the whole line (in terms of its length):
				c=OuterLength + c * CurrentLine.Length;
				
			}else{
				set=InnerSet;
				count=InnerCount;
				
				// Make c relative to the whole line (in terms of its length):
				c=InnerLength + c * CurrentLine.Length;
				
			}
			
			if(ApplyLineJoinNow){
				
				// Clear:
				ApplyLineJoinNow=false;
				
				if(count>0){
					
					// Get the previous point:
					StrokePoint previous=set[count-1];
					
					// Apply join now! Compare set[count-1] to our incoming point.
					if(LineJoinMode==StrokeLineMode.Miter){
						
						// Add a single point which occurs at the miter distance from the original line point
						// along the average of StartNormal and EndNormal.
						
						// Note that we've already made sure miter length is a suitable distance away.
						
						// So, first, average that normal:
						Vector2 avg = new Vector2(
							(StartNormal.x + EndNormal.x) / 2f,
							(StartNormal.y + EndNormal.y) / 2f
						);
						
						float length=MiterLength / avg.magnitude;
						
						// The point to add is therefore..
						float miterX=OriginalLinePoint.x + (avg.x * length);
						float miterY=OriginalLinePoint.y + (avg.y * length);
						float miterC=previous.C + ((c-previous.C) * 0.5f);
						
						if(count==set.Count){
							set.Add(new StrokePoint(miterX,miterY,miterC));
						}else{
							set[count]=new StrokePoint(miterX,miterY,miterC);
						}
						
						count++;
						
					}else if(LineJoinMode==StrokeLineMode.Round){
						
						// Make a circle of radius halfWidth about the original line point.
						DrawCircle(
							set,
							ref count,
							LineAngle,
							previous.X,previous.Y,c,
							OriginalLinePoint
						);
						
					}
					
				}
				
			}
			
			if(count==set.Count){
				set.Add(new StrokePoint(x,y,c));
			}else{
				set[count]=new StrokePoint(x,y,c);
			}
			
			if(Outer){
				OuterCount=count+1;
			}else{
				InnerCount=count+1;
			}
			
		}
		
		
	}
	
	/// <summary>
	/// A point on the stroke.
	/// </summary>
	public struct StrokePoint{
		
		public float X;
		public float Y;
		public float C;
		
		
		public StrokePoint(float x,float y,float c){
			X=x;
			Y=y;
			C=c;
		}
		
	}
	
}