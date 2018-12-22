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
		
		/// <summary>Slices off anything to the right of the given line.</summary>
		public override void SliceRight(float sliceLine,VectorPath path){
			
		}
		
		/// <summary>Slices off anything to the left of the given line.</summary>
		public override void SliceLeft(float sliceLine,VectorPath path){
			
		}
		
		/// <summary>Slices off anything below the given line.</summary>
		public override void SliceBottom(float sliceLine,VectorPath path){
			
		}
		
		/// <summary>Slices off anything above the given line.</summary>
		public override void SliceTop(float sliceLine,VectorPath path){
			
		}
		
	}
	
}