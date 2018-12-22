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
	/// Represents the skew y transform function.
	/// </summary>
	
	public class SkewY:Transformation{
		
		public SkewY(){
			
			Name="skewy";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			// Get the tan of the radian values:
			float y=(float)Math.Tan( -this[0].GetDecimal(context,ValueAxis.Y) );
	
			matrix[1]=y;
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"skewy"};
		}
		
		protected override Css.Value Clone(){
			SkewY result=new SkewY();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



