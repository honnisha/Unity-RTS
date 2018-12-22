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
	/// Represents the translate y transform function.
	/// </summary>
	
	public class TranslateY:Transformation{
		
		public TranslateY(){
			
			Name="translatey";
			
		}
		
		/// <summary>Recalculates the matrices if this transformation has changed.</summary>
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			matrix[13]=context.ScaleToWorldY( this[0].GetDecimal(context,ValueAxis.Y) );
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"translatey"};
		}
		
		protected override Css.Value Clone(){
			TranslateY result=new TranslateY();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



