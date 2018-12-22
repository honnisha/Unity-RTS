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
	/// Represents the perspective transform function.
	/// </summary>
	
	public class Perspective:Transformation{
		
		public Perspective(){
			
			Name="perspective";
			
		}
		
		/// <summary>True if this is a 3D transform.</summary>
		public override bool Is3D{
			get{
				return true;
			}
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			float d=this[0].GetDecimal(context,ValueAxis.None);
			
			matrix[11]=-1f/d;
			
			return matrix;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"perspective"};
		}
		
		protected override Css.Value Clone(){
			Perspective result=new Perspective();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



