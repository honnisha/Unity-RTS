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
	/// Represents the skew transform function.
	/// </summary>
	
	public class Skew:Transformation{
		
		public Skew(){
			
			Name="skew";
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			int count=Count;
	
			// Get the tan of the radian values:
			float x=(float)Math.Tan( -this[0].GetDecimal(context,ValueAxis.X) );
			float y;
			
			if(count>=2){
				y=(float)Math.Tan( -this[1].GetDecimal(context,ValueAxis.Y) );
			}else{
				y=0f;
			}
			
			float z=0f;
			
			if(count>=3){
				z=(float)Math.Tan( -this[2].GetDecimal(context,ValueAxis.Z) );
				
				matrix[1]=z;
				matrix[2]=-y;
				
				matrix[4]=-z;
				matrix[6]=x;
				
				matrix[8]=y;
				matrix[9]=-x;
				
			}else{
			
				matrix[4]=x;
				matrix[1]=y;
			
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
			return new string[]{"skew","skew3d"};
		}
		
		protected override Css.Value Clone(){
			Skew result=new Skew();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



