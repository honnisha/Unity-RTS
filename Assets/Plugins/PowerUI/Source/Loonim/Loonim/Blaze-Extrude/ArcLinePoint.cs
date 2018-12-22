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
		
		public override void ExtrudeAndSample(VectorPath path,float extrudeBy,PointReceiverStepped sampler){
			
			// Cache radius:
			float radius=Radius;
			
			// Extrude:
			Radius+=extrudeBy;
			
			// Clamp:
			if(Radius<0f){
				Radius=0f;
			}
			
			// Rebound:
			RecalculateBounds(path);
			
			if(sampler.IncludeFirstNode){
				sampler.AddPoint(Previous.X,Previous.Y,0f);
			}
			
			// Sample now:
			ComputeLinePoints(sampler);
			
			// Restore:
			Radius=radius;
			
		}
		
	}
	
}