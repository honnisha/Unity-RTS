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
	/// Represents the rotate transform function.
	/// </summary>
	
	public class Rotate:Transformation{
		
		public static Matrix4x4 BuildMatrix(float x,float y,float z,float angle){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			// Normalize:
			float magnitude=(float)Math.Sqrt(x*x + y*y + z*z);
			
			if(magnitude==0f){
				// Can't normalise - don't apply.
				return matrix;
			}
			
			x/=magnitude;
			y/=magnitude;
			z/=magnitude;
			
			// Originated from Mozilla Firefox.
			
			float c=(float)Math.Cos(angle);
			float s=(float)Math.Sin(angle);
			float x2=x*x;
			float y2=y*y;
			float z2=z*z;
			float inverseC=1f-c;
			float yzIc=y*z*inverseC;
			float xyIc=x*y*inverseC;
			float xzIc=x*z*inverseC;
			float xS=x*s;
			float yS=y*s;
			float zS=z*s;
			
			matrix[0]=1f + inverseC*(x2-1f);
			matrix[1]=zS + xyIc;
			matrix[2]=-yS + xzIc;
			
			matrix[4]=-zS + xyIc;
			matrix[5]=1f + inverseC*(y2 - 1f);
			matrix[6]=xS + yzIc;
			
			matrix[8]=yS + xyIc;
			matrix[9]=-xS + yzIc;
			matrix[10]=1f + inverseC*(z2-1f);
			
			return matrix;
		}
		
		public Rotate(){
			
			Name="rotate";
			
		}
		
		/// <summary>True if this is a 3D transform.</summary>
		public override bool Is3D{
			get{
				return Count==4;
			}
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			int count=Count;
			
			if(count==1){
				// rotate(z):
				
				return BuildMatrix(0f,0f,-1f,this[0].GetDecimal(context,ValueAxis.Z,ValueRelativity.None));
				
			}else if(count==4){
				// rotate3d(x,y,z,angle):
				
				return BuildMatrix(
					this[0].GetDecimal(context,ValueAxis.X,ValueRelativity.None),
					-this[1].GetDecimal(context,ValueAxis.Y,ValueRelativity.None),
					-this[2].GetDecimal(context,ValueAxis.Z,ValueRelativity.None),
					this[3].GetDecimal(context,ValueAxis.None,ValueRelativity.None)
				);
				
			}
			
			return Matrix4x4.identity;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"rotate","rotate3d"};
		}
		
		protected override Css.Value Clone(){
			Rotate result=new Rotate();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



