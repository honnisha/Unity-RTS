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
	/// Represents the skew z transform function.
	/// </summary>
	
	public class SkewZ:Transformation{
		
		public SkewZ(){
			
			Name="skewz";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			// Get the tan of the radian values:
			float z=(float)Math.Tan( -this[0].GetDecimal(context,ValueAxis.Z) );
	
			matrix[1]=z;
			matrix[4]=-z;
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"skewz"};
		}
		
		protected override Css.Value Clone(){
			SkewZ result=new SkewZ();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



