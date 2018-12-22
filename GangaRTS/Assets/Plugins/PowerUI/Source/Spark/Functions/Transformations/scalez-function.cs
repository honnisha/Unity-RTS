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
	/// Represents the scale z transform function.
	/// </summary>
	
	public class ScaleZ:Transformation{
		
		public ScaleZ(){
			
			Name="scalez";
			
		}
		
		/// <summary>Sets the default params for this transformation.</summary>
		public override void SetDefaults(){
			Clear(1f);
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			matrix[10]=this[0].GetDecimal(context,ValueAxis.Z,ValueRelativity.None);
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"scalez"};
		}
		
		protected override Css.Value Clone(){
			ScaleZ result=new ScaleZ();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



