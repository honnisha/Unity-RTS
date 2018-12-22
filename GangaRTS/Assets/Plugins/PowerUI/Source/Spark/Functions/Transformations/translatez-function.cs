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
	/// Represents the translate z transform function.
	/// </summary>
	
	public class TranslateZ:Transformation{
		
		public TranslateZ(){
			
			Name="translatez";
			
		}
		
		/// <summary>Recalculates the matrices if this transformation has changed.</summary>
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			matrix[14]=context.ScaleToWorldX( this[0].GetDecimal(context,ValueAxis.Z) );
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"translatez"};
		}
		
		protected override Css.Value Clone(){
			TranslateZ result=new TranslateZ();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



