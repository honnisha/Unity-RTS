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
	
	public partial class StraightLinePoint{
		
		
		public override void SampleNormalMapped(MappedShapeSampler sampler,float percent,out float x,out float y){
			
			StartNormal(out x,out y);
		}
		
		public override float SampleMapped(MappedShapeSampler sampler,float progress){
			
			return Previous.Y + (progress * sampler.DeltaOne);
			
		}
		
		public override void SampleFullMapped(MappedShapeSampler sampler,float progress,out float x,out float y){
			
			x=Previous.X + (progress * sampler.DeltaOne);
			y=Previous.Y + (progress * sampler.DeltaOne);
			
		}
		
		public override void SetupSampler(MappedShapeSampler sampler){
			
			sampler.DeltaOne=Y-Previous.Y;
			
		}
		
	}
	
}