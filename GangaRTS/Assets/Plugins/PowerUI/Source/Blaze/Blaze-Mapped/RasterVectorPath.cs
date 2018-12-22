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
	
	/// <summary>
	/// A path that only consists of straight lines and moveTo nodes.
	/// </summary>
	public class RasterVectorPath : VectorPath{
		
		
		
	}
	
	public partial class VectorPath{
		
		/// <summary>Converts this path to straight lines only.
		/// Assumes values range from 0-1 and uses an accuracy of 0.05 (roughly 20 segments).</summary>
		public void ToStraightLines(){
			ToStraightLines(0.05f);
		}
		
		/// <summary>Converts this path to straight lines only.
		/// Accuracy is the approx average length of each line segment.</summary>
		public void ToStraightLines(float accuracy){
			
			if(Width==0f){
				// Calc lengths etc:
				RecalculateBounds();
			}
			
			MoveToPoint prevMoveTo=null;
			VectorPoint point=FirstPathNode;
			
			while(point!=null){
				
				// If it's straight/ a MoveTo, skip:
				if(point.IsCurve){
					
					VectorLine line=point as VectorLine;
					
					// Replace it with n line segments:
					int segmentCount=(int)( line.Length / accuracy );
					
					if(segmentCount<1){
						segmentCount=1;
					}
					
					// Setup:
					float delta=1f / (float)segmentCount;
					float progress=delta;
					
					// Sample it segmentCount times:
					VectorPoint previous=point.Previous;
					
					for(int i=0;i<segmentCount;i++){
						
						float x;
						float y;
						line.SampleAt(progress,out x,out y);
						
						// Create line segment:
						StraightLinePoint slp=new StraightLinePoint(x,y);
						slp.Previous=previous;
						
						if(previous==null){
							FirstPathNode=slp;
						}else{
							previous.Next=slp;
						}
						
						previous=slp;
						progress+=delta;
						
					}
					
					// Increase node count:
					PathNodeCount+=segmentCount-1;
					
					// Link up after too:
					if(point.Next==null){
						LatestPathNode=previous;
					}else{
						point.Next.Previous=previous;
					}
					
					if(point.IsClose){
						previous.IsClose=true;
						
						if(prevMoveTo!=null){
							prevMoveTo.ClosePoint=previous;
						}
					}
					
				}else if(point is MoveToPoint){
					prevMoveTo=point as MoveToPoint;
				}
				
				// Next one:
				point=point.Next;
				
			}
			
			// Recalc:
			RecalculateBounds();
			
		}
		
	}
	
}