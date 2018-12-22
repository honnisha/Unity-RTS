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

	public partial class VectorPath{
		
		/// <summary>The minimum X value.</summary>
		public float MinX;
		/// <summary>The minimum Y value.</summary>
		public float MinY;
		/// <summary>The width of this path.</summary>
		public float Width;
		/// <summary>The height of this path.</summary>
		public float Height=1f;
		/// <summary>The number of points in this vector path.</summary>
		public int PathNodeCount;
		/// <summary>The current node which will be used when the path is closed.</summary>
		public MoveToPoint CloseNode;
		/// <summary>When creating a path its nodes are stored as a linked list. The first node created.</summary>
		public VectorPoint FirstPathNode;
		/// <summary>When creating a path its nodes are stored as a linked list. The latest node created.</summary>
		public VectorPoint LatestPathNode;
		
		/// <summary>Moves the current pen location to the given point. Used when drawing paths.</summary>
		public void MoveTo(float x,float y){
			
			// We need to add the first end:
			MoveToPoint point=new MoveToPoint(x,y);
			
			AddPathNode(point);
			
			CloseNode=point;
			
		}
		
		/// <summary>"Simplifies" the curve values ensuring that it's possible to offset the parts of the path.
		/// Used by the path stroke system.</summary>
		public void SimplifyCurve(){
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
			
				if(current.IsCurve){
				
					CurveLinePoint clp=current as CurveLinePoint;
					
					if(clp!=null){
						
						// Simplify:
						clp.SimplifyCurve(this);
						
					}else{
						
						// Convert a QLP to a CLP:
						QuadLinePoint qlp=current as QuadLinePoint;
						
						if(qlp!=null){
							
							// Create but with both control points being the same:
							clp=new CurveLinePoint(qlp.X,qlp.Y);
							clp.Control1X=qlp.Control1X;
							clp.Control1Y=qlp.Control1Y;
							clp.Control2X=qlp.Control1X;
							clp.Control2Y=qlp.Control1Y;
							clp.IsClose=qlp.IsClose;
							
							// Replace the node now:
							if(qlp.Previous==null){
								FirstPathNode=clp;
							}else{
								qlp.Previous.Next=clp;
							}
							
							if(qlp.Next==null){
								LatestPathNode=clp;
							}else{
								qlp.Next.Previous=clp;
							}
							
						}
						
					}
					
				}
				
				current=current.Next;
			}
			
		}
		
		/// <summary>Selects the point at the given index of this path.</summary>
		public VectorPoint SelectPoint(int index){
			
			VectorPoint current=FirstPathNode;
			
			while(index!=0 && current!=null){
				
				index--;
				current=current.Next;
				
			}
			
			return current;
			
		}
		
		/// <summary>The length of this path.</summary>
		public float Length(){
			
			float length=0f;
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				if(current.HasLine){
					
					VectorLine line=current as VectorLine;
					
					length+=line.Length;
					
				}
				
				// Hop to the next one:
				current=current.Next;
			}
			
			return length;
			
		}
		
		/// <summary>Copies a section of this path.
		/// Note that if p2 is before p1, it will safely loop over a closed node.
		/// </summary>
		public VectorPath CopySection(VectorPoint p1,float c1,VectorPoint p2,float c2){
			
			// Split the line p1 at c1.
			// Split the line p2 at c2.
			// The section between these two nodes is the part we want.
			
			// Create the path:
			VectorPath path=new VectorPath();
			
			if(p1==null){
				p1=FirstPathNode;
				c1=0f;
			}
			
			// Add the first node:
			path.AddPathNode(p1.PointAt(c1,true));
			
			VectorPoint current=p1.Next;
			
			while(current!=p2 && current!=null){
				
				if(current==p1){
					// This occurs when p1 and p2 are not in the same shape.
					throw new Exception("Section start and end are not from the same path.");
				}
				
				// Add a copy:
				path.AddPathNode(current.Copy());
				
				// Go to next:
				current=current.Next;
				
				if(current==null && p2!=null){
					// Need to loop back around unless p2 is null.
					// Notice that we actively skip the moveTo itself as
					// current is in the same location.
					current=path.FirstPathNode.Next;
				}
				
			}
			
			if(current==p2 && p2!=null){
				
				// Add it:
				path.AddPathNode(p2.PointAt(c2,false));
				
			}
			
			return path;
			
		}
		
		/// <summary>Adds the given path onto the end of this one.</summary>
		public void Append(VectorPath path){
			
			if(LatestPathNode==null){
				
				// This path just becomes the same as the given one:
				FirstPathNode=path.FirstPathNode;
				LatestPathNode=path.LatestPathNode;
				PathNodeCount=path.PathNodeCount;
				CloseNode=path.CloseNode;
				
			}else{
				
				// Add the first node:
				AddPathNode(path.FirstPathNode);
				LatestPathNode=path.LatestPathNode;
				
				// -1 because Add already added the first node.
				PathNodeCount+=path.PathNodeCount-1;
				
				CloseNode=path.CloseNode;
				
			}
			
		}
		
		/// <summary>Copies this path.</summary>
		public VectorPath CopyPath(){
			
			// Create:
			VectorPath path=new VectorPath();
			
			// Copy into it:
			CopyInto(path);
			
			return path;
			
		}
		
		/// <summary>Copies this vector path into the given one.</summary>
		public void CopyInto(VectorPath path){
			
			VectorPoint point=FirstPathNode;
			
			while(point!=null){
				
				VectorPoint copiedPoint=point.Copy();
				
				path.AddPathNode(copiedPoint);
				
				// Copy close status:
				if(point.IsClose){
					
					copiedPoint.IsClose=true;
					path.CloseNode.ClosePoint=copiedPoint;
					
				}
				
				point=point.Next;
			}
			
			
		}
		
		/// <summary>Clears this path.</summary>
		public void Clear(){
			FirstPathNode=null;
			LatestPathNode=null;
			PathNodeCount=0;
		}
		
		/// <summary>Adds the given node to the end of path. See AddPathNodeStart to add to the start.</summary>
		public void AddPathNode(VectorPoint point){
			
			PathNodeCount++;
			
			if(FirstPathNode==null){
				
				if(point.Unloaded){
					
					FirstPathNode=LatestPathNode=point;
					
				}else{
				
					MoveToPoint move=point as MoveToPoint;
				
					if(move==null){
						
						// Add a blank MoveTo - this means that moveTo's are always the close nodes.
						move=new MoveToPoint(0f,0f);
						FirstPathNode=LatestPathNode=move;
						CloseNode=move;
						
						PathNodeCount++;
						
						point.Previous=move;
						LatestPathNode=move.Next=point;
						
					}else{
						
						FirstPathNode=LatestPathNode=point;
						CloseNode=move;
						
					}
				
				}
				
			}else{
				
				// Hook it onto the end:
				point.Previous=LatestPathNode;
				LatestPathNode=LatestPathNode.Next=point;
			}
			
		}
		
		/// <summary>Adds the given node to the start of the path. Must be a moveTo unless it's a temp thing.
		/// See AddPathNode to add to the end.</summary>
		public void AddPathNodeStart(VectorPoint point){
			
			PathNodeCount++;
			
			if(FirstPathNode==null){
				
				if(point.Unloaded){
					
					FirstPathNode=LatestPathNode=point;
					
				}else{
				
					MoveToPoint move=point as MoveToPoint;
				
					if(move==null){
						
						// Add a blank MoveTo - this means that moveTo's are always the close nodes.
						move=new MoveToPoint(0f,0f);
						FirstPathNode=LatestPathNode=move;
						CloseNode=move;
						
						PathNodeCount++;
						
						point.Previous=move;
						LatestPathNode=move.Next=point;
						
					}else{
						
						FirstPathNode=LatestPathNode=point;
						CloseNode=move;
						
					}
				
				}
				
			}else{
				
				// Hook it onto the start:
				point.Previous=null;
				point.Next=FirstPathNode;
				FirstPathNode.Previous=point;
				FirstPathNode=point;
				
			}
			
		}
		
		/// <summary>Closes the path quickly and safely.</summary>
		public void ClosePathFast(){
		
			if(CloseNode==null || LatestPathNode==null){
				return;
			}
			
			StraightLinePoint point=LineTo(CloseNode.X,CloseNode.Y);
			point.Close=true;
			CloseNode.ClosePoint=point;
			
		}
		
		/// <summary>Is this path closed?</summary>
		public bool Closed{
			get{
				return (LatestPathNode!=null && LatestPathNode.IsClose);
			}
		}
		
		/// <summary>True if this path is currently unclosed.</summary>
		public bool Unclosed{
			get{
				return (LatestPathNode==null || !LatestPathNode.IsClose);
			}
		}
		
		/// <summary>Closes the shape if the last point is the same as the close node.</summary>
		public void CheckClosed(){
			
			if(CloseNode==null || LatestPathNode==null){
				return;
			}
			
			if(LatestPathNode.X==CloseNode.X && LatestPathNode.Y==CloseNode.Y){
				LatestPathNode.IsClose=true;
				CloseNode.ClosePoint=LatestPathNode;
			}
		}
		
		/// <summary>A full path close.</summary>
		public void ClosePath(){
			
			if(CloseNode==null || LatestPathNode==null){
				return;
			}
			
			if(LatestPathNode.X==CloseNode.X && LatestPathNode.Y==CloseNode.Y){
				LatestPathNode.IsClose=true;
				CloseNode.ClosePoint=LatestPathNode;
			}else{
				StraightLinePoint point=LineTo(CloseNode.X,CloseNode.Y);
				point.Close=true;
				CloseNode.ClosePoint=point;
			}
			
		}
		
		/// <summary>Marks the last node as a close.</summary>
		public void CloseLast(){
			
			if(LatestPathNode==null){
				return;
			}
			
			LatestPathNode.IsClose=true;
			
			if(CloseNode!=null){
				CloseNode.ClosePoint=LatestPathNode;
			}
			
		}
		
		public StraightLinePoint LineTo(float x,float y){
			
			// Create the straight line:
			StraightLinePoint newNode=new StraightLinePoint(x,y);
			
			// Add it:
			AddPathNode(newNode);
			
			return newNode;
			
		}
		
		public QuadLinePoint QuadraticCurveTo(float cx,float cy,float x,float y){
			
			// Create the curve line:
			QuadLinePoint newNode=new QuadLinePoint(x,y);
			
			newNode.Control1X=cx;
			newNode.Control1Y=cy;
			
			// Add it:
			AddPathNode(newNode);
			
			return newNode;
		}
		
		public void CurveTo(float c1x,float c1y,float c2x,float c2y,float x,float y){
			
			// Create the curve line:
			CurveLinePoint newNode=new CurveLinePoint(x,y);
			
			newNode.Control1X=c1x;
			newNode.Control1Y=c1y;
			newNode.Control2X=c2x;
			newNode.Control2Y=c2y;
			
			// Add it:
			AddPathNode(newNode);
			
		}
		
		/// <summary>Recomputes path node count.</summary>
		public void CountNodes(){
			
			PathNodeCount=0;
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				// Add:
				PathNodeCount++;
				
				// Hop to the next one:
				current=current.Next;
			}
			
		}
		
		/// <summary>The value 2*PI.</summary>
		private const float TwoPI=(float)(Math.PI)*2f;
		
		/// <summary>Creates an arc around the given circle center. Note that nothing will
		/// be seen until you call a fill or stroke method.</summary>
		public void Arc(float centerX,float centerY,float radius,float sAngle,float eAngle,bool counterClockwise){
			
			VectorPoint previous=LatestPathNode;
			
			float x0;
			float y0;
			
			if(previous==null){
				x0=0f;
				y0=0f;
			}else{
				x0=previous.X;
				y0=previous.Y;
			}
			
			// Clockwise eAngle > sAngle; counter clockwise otherwise.
			if(eAngle>sAngle){
				if(counterClockwise){
					// Get them both in range:
					eAngle=eAngle%TwoPI;
					sAngle=sAngle%TwoPI;
					
					// Reduce eAngle by a full rotation so it's smaller:
					eAngle-=TwoPI;
				}
			}else if(!counterClockwise){
				// Get them both in range:
				eAngle=eAngle%TwoPI;
				sAngle=sAngle%TwoPI;
					
				// Reduce sAngle by a full rotation so it's smaller:
				sAngle-=TwoPI;
			}
			
			
			// First, figure out where the actual start is.
			// It's radius units to the right of center, then rotated through radius around center.
			// Thus we have a triangle with hyp length of 'radius' and an angle of sAngle:
			float startX=radius * (float) Math.Cos(sAngle);
			float startY=radius * (float) Math.Sin(sAngle);
			
			// Now find the end point, using exactly the same method:
			float endX=radius * (float) Math.Cos(eAngle);
			float endY=radius * (float) Math.Sin(eAngle);
			
			// We now have an arc from the current position to endX/endY.
			// The start and exit node angles are usefully just offset from the given ones.
			// This is because an sAngle of zero should create an arc which starts travelling downwards.
			// (Or upwards if it's counter clockwise):
			
			// Where does the arc start from?
			float arcStartX=centerX+startX;
			float arcStartY=centerY+startY;
			
			if(FirstPathNode==null){
				// This occurs if the arc is the first thing we draw. No line is drawn to it.
				MoveTo(arcStartX,arcStartY);
			}else if(arcStartX!=x0 || arcStartY!=y0){
				// Draw a line to this point:
				LineTo(arcStartX,arcStartY);
			}
			
			// Create the new arc node:
			ArcLinePoint arcNode=new ArcLinePoint(centerX+endX,centerY+endY);
			
			// Apply the radius:
			arcNode.Radius=radius;
			
			// Apply the angles:
			arcNode.StartAngle=sAngle;
			arcNode.EndAngle=eAngle;
			
			// Apply the center:
			arcNode.CircleCenterX=centerX;
			arcNode.CircleCenterY=centerY;
			
			// Add the other end:
			AddPathNode(arcNode);
			
		}
		
		/// <summary>Handles SVG arcs.</summary>
		public void EllipseArc(float rx,float ry,float xAxisRotation,float p1x,float p1y,bool largeArcFlag,bool sweepFlag){
			
			// In accordance to: http://www.w3.org/TR/SVG/implnote.html#ArcOutOfRangeParameters
			rx = Math.Abs(rx);
			ry = Math.Abs(ry);
			
			// If the endpoints are identical, do nothing.
			if(LatestPathNode.X == p1x && LatestPathNode.Y == p1y){
				return;
			}
			
			// If rx = 0 or ry = 0 then this arc is treated as a straight line segment joining the endpoints.    
			if(rx == 0f || ry == 0f) {
				LineTo(p1x,p1y);
				return;
			}
			
			// Normalize rotation from deg to radians:
			xAxisRotation =(xAxisRotation % 360f) * UnityEngine.Mathf.Deg2Rad;
			
			
			float cosXAxis=(float)Math.Cos(xAxisRotation);
			float sinXAxis=(float)Math.Sin(xAxisRotation);
			
			// Conversion from endpoint to center parameterization
			// http://www.w3.org/TR/SVG/implnote.html#ArcConversionEndpointToCenter
			
			// #1: Compute transformed point
			float dx = (LatestPathNode.X-p1x)/2f;
			float dy = (LatestPathNode.Y-p1y)/2f;
			
			float transformedX = (float)(cosXAxis*dx + sinXAxis*dy);
			float transformedY = (float)(-sinXAxis*dx + cosXAxis*dy);
			
			// Ensure radii are large enough
			float radiiCheck = (float)( 
				Math.Pow(transformedX, 2f)/Math.Pow(rx, 2f) + 
				Math.Pow(transformedY, 2f)/Math.Pow(ry, 2f)
			);
			
			if(radiiCheck > 1) {
				rx = (float)Math.Sqrt(radiiCheck)*rx;
				ry = (float)Math.Sqrt(radiiCheck)*ry;
			}
			
			// #2: Compute transformed center
			float cSquareNumerator = (float)(
				Math.Pow(rx, 2f)*Math.Pow(ry, 2f) - 
				Math.Pow(rx, 2f)*Math.Pow(transformedY, 2f) - 
				Math.Pow(ry, 2f)*Math.Pow(transformedX, 2f)
			);
			
			float cSquareRootDenom = (float)(
				Math.Pow(rx, 2f)*Math.Pow(transformedY, 2f) + 
				Math.Pow(ry, 2f)*Math.Pow(transformedX, 2f)
			);
			
			float cRadicand = cSquareNumerator / cSquareRootDenom;
			
			// Make sure this never drops below zero because of precision
			cRadicand = cRadicand < 0f ? 0f : cRadicand;
			
			float cCoef = (largeArcFlag != sweepFlag ? 1f : -1f) * (float)Math.Sqrt(cRadicand);
			
			float transformedCenterX = cCoef*((rx*transformedY)/ry);
			float transformedCenterY = cCoef*(-(ry*transformedX)/rx);
			
			// #3: Compute center
			
			float centerX=(float)( 
				cosXAxis*transformedCenterX - 
				sinXAxis*transformedCenterY + ((LatestPathNode.X+p1x)/2f)
			);
			
			float centerY=(float)(
				sinXAxis*transformedCenterX + 
				cosXAxis*transformedCenterY + ((LatestPathNode.Y+p1y)/2f)
			);
			
			// #4: Compute start/sweep angles
			// Start angle of the elliptical arc prior to the stretch and rotate operations.
			// Difference between the start and end angles
			UnityEngine.Vector2 startVector=new UnityEngine.Vector2(
				(transformedX-transformedCenterX) / rx,
				(transformedY-transformedCenterY) / ry
			);
			
			float startAngle = AngleBetween(new UnityEngine.Vector2(1f,0f), startVector);
			
			UnityEngine.Vector2 endVector=new UnityEngine.Vector2(
				(-transformedX-transformedCenterX)/rx,
				(-transformedY-transformedCenterY)/ry
			);
			
			float sweepAngle = AngleBetween(startVector, endVector);
			
			if(!sweepFlag && sweepAngle > 0) {
				sweepAngle -= 2f*(float)Math.PI;
			}else if(sweepFlag && sweepAngle < 0) {
				sweepAngle += 2f*(float)Math.PI;
			}
			
			// We use % instead of `mod(..)` because we want it to be -360deg to 360deg(but actually in radians)
			sweepAngle %= 2f*(float)Math.PI;
			
			// Ellipse point:
			EllipseLinePoint elp=new EllipseLinePoint(p1x,p1y);
			
			// Apply values:
			elp.SweepAngle=sweepAngle;
			elp.StartAngle=startAngle;
			elp.SinXAxis=sinXAxis;
			elp.CosXAxis=cosXAxis;
			elp.CenterX=centerX;
			elp.CenterY=centerY;
			elp.RadiusX=rx;
			elp.RadiusY=ry;
			
			// Add it:
			AddPathNode(elp);
		}
		
		/// <summary>Signed angle between vectors.</summary>
		private static float AngleBetween(UnityEngine.Vector2 a,UnityEngine.Vector2 b){
			return (float)System.Math.Atan2( 
				a.x*b.y - a.y*b.x,
				a.x*b.x + a.y*b.y
			);
		}
		
		/// <summary>Recalculates bounds and normals.</summary>
		public void RecalculateMeta(){
			RecalculateBounds();
		}
		
		/// <summary>Recalculates the minimum values and width/height of this path, taking curves into account.</summary>
		public void RecalculateBounds(){
			
			if(FirstPathNode==null){
				
				Width=0f;
				Height=0f;
				MinY=0f;
				MinX=0f;
				
				return;
				
			}
			
			// Our temp boundaries:
			MinX=float.MaxValue;
			MinY=float.MaxValue;
			
			// We'll be using width/height temporarily as max:
			Width=float.MinValue;
			Height=float.MinValue;
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				// Recalc bounds:
				current.RecalculateBounds(this);
				
				// Hop to the next one:
				current=current.Next;
			}
			
			// Remove min values from width/height:
			Width-=MinX;
			Height-=MinY;
			
		}
		
		/// <summary>Replaces one node with another.</summary>
		public void Replace(VectorPoint point,VectorPoint with){
			
			// Update prev/next:
			with.Previous=point.Previous;
			with.Next=point.Next;
			
			if(point.Previous==null){
				FirstPathNode=with;
			}else{
				point.Previous.Next=with;
			}
			
			if(point.Next==null){
				LatestPathNode=with;
			}else{
				point.Next.Previous=with;
			}
			
		}
		
		/// <summary>Remove a point and may insert a MoveTo 
		/// to ensure the visual appearance of the rest of the path is unaffected.</summary>
		public void RemoveVisually(VectorPoint point){
			
			// First, do the actual removal (inline version of Remove):
			if(point.Previous==null){
				FirstPathNode=point.Next;
			}else{
				point.Previous.Next=point.Next;
			}
			
			if(point.Next==null){
				LatestPathNode=point.Previous;
			}else{
				point.Next.Previous=point.Previous;
			}
			
			// If it's a MoveTo, do nothing else:
			if(point is MoveToPoint){
				return;
			}
			
			// If the previous node is a MoveTo, move it to points location:
			VectorPoint previous=point.Previous;
			
			if(previous!=null && (previous is MoveToPoint)){
				
				// Move that MoveTo to points location:
				previous.X=point.X;
				previous.Y=point.Y;
				
			}else{
				
				// Otherwise, create a MoveTo and insert where our point was.
				MoveToPoint moveTo=new MoveToPoint(point.X,point.Y);
				
				if(point.Previous==null){
					FirstPathNode=moveTo;
				}else{
					moveTo.Previous=point.Previous;
					point.Previous.Next=moveTo;
				}
				
				if(point.Next==null){
					LatestPathNode=moveTo;
				}else{
					moveTo.Next=point.Next;
					point.Next.Previous=moveTo;
				}
				
			}
			
		}
		
		/// <summary>Remove a point. 
		/// Just directly removes the point from the linked list unlike RemoveVisually.</summary>
		public void Remove(VectorPoint point){
			
			if(point.Previous==null){
				FirstPathNode=point.Next;
			}else{
				point.Previous.Next=point.Next;
			}
			
			if(point.Next==null){
				LatestPathNode=point.Previous;
			}else{
				point.Next.Previous=point.Previous;
			}
			
		}
		
		/// <summary>Sheers this path. Note that it's assumed to be at most 1 unit tall.</summary>
		public void Sheer(float by){
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				current.Sheer(by);
				
				// Hop to the next one:
				current=current.Next;
			}
			
		}
		
		/// <summary>Scales this path by the given value.</summary>
		public void Scale(float by){
			
			Scale(by,by);
			
		}
		
		
		/// <summary>Scales this path by the given value.</summary>
		public void Scale(float x,float y){
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				current.Multiply(x,y);
				
				// Hop to the next one:
				current=current.Next;
			}
			
			// Scale the dimensions:
			MinX*=x;
			MinY*=y;
			Width*=x;
			Height*=y;
			
		}
		
		/// <summary>Axis flip.</summary>
		public void Flip(){
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				current.Flip();
				
				// Hop to the next one:
				current=current.Next;
			}
			
			float x=Width;
			Width=Height;
			Height=x;
			
			x=MinX;
			MinX=MinY;
			MinY=x;
			
		}
		
		/// <summary>Scales this path by the given value.</summary>
		public void Move(float byX,float byY){
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				current.Move(byX,byY);
				
				// Hop to the next one:
				current=current.Next;
			}
			
			// Move bounds:
			MinX+=byX;
			MinY+=byY;
			
		}
		
		/// <summary>Gets the signed area of the "major" contour (the first one).
		/// It's signed as this can identify the winding order.</summary>
		public float GetSignedArea(){
			float sum=0f;
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				if(current.HasLine){
					sum+=current.SignedArea();
				}
				
				current=current.Next;
			}
			
			return sum/2f;
		}
		
		public override string ToString(){
			
			string text="";
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				text+=current.ToString()+"\r\n";
				
				// Hop to the next one:
				current=current.Next;
			}
			
			return text;
			
		}
		
	}

}