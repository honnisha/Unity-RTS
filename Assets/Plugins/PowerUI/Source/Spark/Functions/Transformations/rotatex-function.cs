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
	/// Represents the rotate x transform function.
	/// </summary>
	
	public class RotateX:Transformation{
		
		public RotateX(){
			
			Name="rotatex";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			return Rotate.BuildMatrix(1f,0f,0f,this[0].GetDecimal(context,ValueAxis.X,ValueRelativity.None));
			
		}
		
		public override string[] GetNames(){
			return new string[]{"rotatex"};
		}
		
		protected override Css.Value Clone(){
			RotateX result=new RotateX();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



