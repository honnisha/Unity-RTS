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
	/// A node which immediately follows a straight line.
	/// </summary>
	
	public partial class StraightLinePoint:VectorLine{
		
		public override void ExtrudeAndSample(VectorPath path,float extrudeBy,PointReceiverStepped sampler){
			
			// First, cache all the points:
			float prevX=Previous.X;
			float prevY=Previous.Y;
			float x=X;
			float y=Y;
			
			// Get the normal and extrude along it.
			float normalX;
			float normalY;
			StartNormal(out normalX,out normalY);
			
			Previous.X+=normalX * extrudeBy;
			Previous.Y+=normalY * extrudeBy;
			X+=normalX * extrudeBy;
			Y+=normalY * extrudeBy;
			
			// Rebound:
			RecalculateBounds(path);
			
			if(sampler.IncludeFirstNode){
				sampler.AddPoint(Previous.X,Previous.Y,0f);
			}
			
			// Sample now:
			ComputeLinePoints(sampler);
			
			// Restore all the points:
			Previous.X=prevX;
			Previous.Y=prevY;
			X=x;
			Y=y;
			
		}
		
	}
	
}