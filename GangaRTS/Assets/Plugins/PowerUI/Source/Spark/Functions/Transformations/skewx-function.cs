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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the skew x transform function.
	/// </summary>
	
	public class SkewX:Transformation{
		
		public SkewX(){
			
			Name="skewx";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			// Get the tan of the radian values:
			float x=(float)Math.Tan( -this[0].GetDecimal(context,ValueAxis.X) );
		
			matrix[4]=x;
		
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"skewx"};
		}
		
		protected override Css.Value Clone(){
			SkewX result=new SkewX();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



