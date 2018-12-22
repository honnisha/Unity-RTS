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
	/// Represents the scale transform function.
	/// </summary>
	
	public class Scale:Transformation{
		
		public Scale(){
			
			Name="scale";
			
		}
		
		/// <summary>Sets the default params for this transformation.</summary>
		public override void SetDefaults(){
			Clear(1f);
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			int count=Count;
			
			matrix[0]=this[0].GetDecimal(context,ValueAxis.X,ValueRelativity.None);
			matrix[5]=this[1].GetDecimal(context,ValueAxis.Y,ValueRelativity.None);
			
			if(count>=3){
				matrix[10]=this[2].GetDecimal(context,ValueAxis.Z,ValueRelativity.None);
			}
			
			return matrix;
		}
		
		/// <summary>True if this is a 3D transform.</summary>
		public override bool Is3D{
			get{
				return Count>=3;
			}
		}
		
		public override string[] GetNames(){
			return new string[]{"scale","scale3d"};
		}
		
		protected override Css.Value Clone(){
			Scale result=new Scale();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



