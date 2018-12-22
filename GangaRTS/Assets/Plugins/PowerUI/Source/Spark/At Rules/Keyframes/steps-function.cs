//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using Blaze;


namespace Css.Functions{
	
	/// <summary>
	/// Represents the steps function.
	/// </summary>
	
	public class Steps:CssFunction{
		
		public Steps(){
			
			Name="steps";
			
		}
		
		/// <summary>The built path.</summary>
		private Blaze.VectorPath CachedPath_;
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			if(CachedPath_==null){
				
				CachedPath_=new VectorPath();
				
				// Get the # of steps:
				int stepCount=this[0].GetInteger(context,property);
				
				if(stepCount<1){
					stepCount=1;
				}
				
				// The step first riser offset (from the left edge).
				// Deals with 'start', 'end', and potentially any number in between.
				float offset=0f;
				
				if(Count>1){
					offset=this[1].GetDecimal(context,property);
				}
				
				// Find the step size:
				float stepSize=1f / (float)stepCount;
				
				// For each step..
				float currentX=offset;
				float currentY=0f;
				
				for(int i=0;i<stepCount;i++){
					
					// Rise!
					currentY+=stepSize;
					
					// Don't add the first riser if we're not offset at all
					if(currentX>0f){
						// Add the riser:
						CachedPath_.LineTo(currentX,currentY);
					}else{
						// Just a MoveTo instead:
						CachedPath_.MoveTo(0f,currentY);
					}
					
					// Move over!
					currentX+=stepSize;
					
					// Don't add the last top if we're offset all the way over
					if(currentX<1f){
						// Add the top:
						CachedPath_.LineTo(currentX,currentY);
					}
					
				}
				
			}
			
			return CachedPath_;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"steps"};
		}
		
		protected override Css.Value Clone(){
			Steps result=new Steps();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}