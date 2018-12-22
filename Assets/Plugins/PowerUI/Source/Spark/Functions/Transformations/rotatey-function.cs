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
	/// Represents the rotate y transform function.
	/// </summary>
	
	public class RotateY:Transformation{
		
		public RotateY(){
			
			Name="rotatey";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			return Rotate.BuildMatrix(0f,-1f,0f,this[0].GetDecimal(context,ValueAxis.Y,ValueRelativity.None));
			
		}
		
		public override string[] GetNames(){
			return new string[]{"rotatey"};
		}
		
		protected override Css.Value Clone(){
			RotateY result=new RotateY();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



