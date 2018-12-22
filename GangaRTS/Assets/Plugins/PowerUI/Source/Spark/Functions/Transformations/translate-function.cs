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
	/// Represents the translate transform function.
	/// </summary>
	
	public class Translate:Transformation{
		
		public Translate(){
			
			Name="translate";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			int count=Count;
			
			matrix[12]=context.ScaleToWorldX( -this[0].GetDecimal(context,ValueAxis.X) );
			
			if(count>=2){
				matrix[13]=context.ScaleToWorldY( this[1].GetDecimal(context,ValueAxis.Y) );
			}
			
			if(count>=3){
				matrix[14]=context.ScaleToWorldX( this[2].GetDecimal(context,ValueAxis.Z) );
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
			return new string[]{"translate","translate3d"};
		}
		
		protected override Css.Value Clone(){
			Translate result=new Translate();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



