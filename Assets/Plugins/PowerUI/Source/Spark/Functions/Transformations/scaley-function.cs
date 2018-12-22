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
	/// Represents the scale y transform function.
	/// </summary>
	
	public class ScaleY:Transformation{
		
		public ScaleY(){
			
			Name="scaley";
			
		}
		
		/// <summary>Sets the default params for this transformation.</summary>
		public override void SetDefaults(){
			Clear(1f);
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			matrix[5]=this[0].GetDecimal(context,ValueAxis.Y,ValueRelativity.None);
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"scaley"};
		}
		
		protected override Css.Value Clone(){
			ScaleY result=new ScaleY();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



