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
	/// Represents the rotate z transform function.
	/// </summary>
	
	public class RotateZ:Transformation{
		
		public RotateZ(){
			
			Name="rotatez";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			return Rotate.BuildMatrix(0f,0f,-1f,this[0].GetDecimal(context,ValueAxis.Z,ValueRelativity.None));
			
		}
		
		public override string[] GetNames(){
			return new string[]{"rotatez"};
		}
		
		protected override Css.Value Clone(){
			RotateZ result=new RotateZ();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



