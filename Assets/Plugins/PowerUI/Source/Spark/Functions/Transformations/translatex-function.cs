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
	/// Represents the translate x transform function.
	/// </summary>
	
	public class TranslateX:Transformation{
		
		public TranslateX(){
			
			Name="translatex";
			
		}
		
		/// <summary>Recalculates the matrices if this transformation has changed.</summary>
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			matrix[12]=context.ScaleToWorldX( -this[0].GetDecimal(context,ValueAxis.X) );
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"translatex"};
		}
		
		protected override Css.Value Clone(){
			TranslateX result=new TranslateX();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



