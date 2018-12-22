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
	/// Represents the cubic-bezier function.
	/// </summary>
	
	public class CubicBezier:CssFunction{
		
		public CubicBezier(){
			
			Name="cubic-bezier";
			
		}
		
		/// <summary>The built path.</summary>
		private Blaze.VectorPath CachedPath_;
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			if(CachedPath_==null){
				
				CachedPath_=new VectorPath();
				
				// Create the curve now:
				CachedPath_.CurveTo(
					
					this[0].GetDecimal(context,property),
					this[1].GetDecimal(context,property),
					this[2].GetDecimal(context,property),
					this[3].GetDecimal(context,property),
					1f,1f
					
				);
				
			}
			
			return CachedPath_;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"cubic-bezier"};
		}
		
		protected override Css.Value Clone(){
			CubicBezier result=new CubicBezier();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}