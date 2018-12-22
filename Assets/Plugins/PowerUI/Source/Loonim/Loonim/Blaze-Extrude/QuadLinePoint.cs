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
		
		public override void ExtrudeAndSample(VectorPath path,float extrudeBy,PointReceiverStepped sampler){
			
			// First, cache all the points:
			float prevX=Previous.X;
			float prevY=Previous.Y;
			float c1x=Control1X;
			float c1y=Control1Y;
			float x=X;
			float y=Y;
			
			// Get the normals and extrude along them.
			float normalX;
			float normalY;
			
			// First point:
			StartNormal(out normalX,out normalY);
			Previous.X+=normalX * extrudeBy;
			Previous.Y+=normalY * extrudeBy;
			
			// First control:
			NormalAt(0.5f,out normalX,out normalY);
			Control1X+=normalX * extrudeBy;
			Control1Y+=normalY * extrudeBy;
			
			// Last point:
			EndNormal(out normalX,out normalY);
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
			Control1X=c1x;
			Control1Y=c1y;
			X=x;
			Y=y;
			
		}
		
	}
	
}