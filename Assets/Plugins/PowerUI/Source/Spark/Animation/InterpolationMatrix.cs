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


namespace Css{
	
	/// <summary>
	/// Represents an interpolatable matrix (https://www.w3.org/TR/css-transforms-1/#matrix-interpolation).
	/// </summary>
	
	public class InterpolationMatrix:Css.Functions.Transformation{
		
		/// <summary>True if the params have changed and needs to be reconstructed.</summary>
		internal bool Changed=false;
		/// <summary>The constructed raw matrix.</summary>
		private Matrix4x4 Matrix_=Matrix4x4.identity;
		
		/// <summary>A rotation.</summary>
        public float Angle{
			get{
				return Get(0);
			}
			set{
				Set(0,value);
			}
		}
		
		/// <summary>1,1 coordinate of 2x2 matrix</summary>
        public float M11{
			get{
				return Get(1);
			}
			set{
				Set(1,value);
			}
		}
		
		/// <summary>1,2 coordinate of 2x2 matrix</summary>
        public float M12{
			get{
				return Get(2);
			}
			set{
				Set(2,value);
			}
		}
		
		/// <summary>2,1 coordinate of 2x2 matrix</summary>
        public float M21{
			get{
				return Get(3);
			}
			set{
				Set(3,value);
			}
		}
		
		/// <summary>2,2 coordinate of 2x2 matrix</summary>
        public float M22{
			get{
				return Get(4);
			}
			set{
				Set(4,value);
			}
		}
		
		/// <summary>A 2 component translation.</summary>
		public Vector2 Translation{
			get{
				return new Vector2(Get(5),Get(6));
			}
			set{
				Set(5,value.x);
				Set(6,value.y);
			}
		}
		
		/// <summary>A 2 component scale.</summary>
        public Vector2 Scale{
			get{
				return new Vector2(Get(7),Get(8));
			}
			set{
				Set(7,value.x);
				Set(8,value.y);
			}
		}
		
		
		public InterpolationMatrix(){}
		
		public InterpolationMatrix(Matrix4x4 matrix){
			
			Name="-spark-matrix";
			
			// Set the composed form:
			Matrix_=matrix;
			
			// Params are 9 floats:
			Count=9;
			
			for(int i=0;i<9;i++){
				this[i]=new Css.Units.DecimalUnit(0f);
			}
			
			// Decompose now:
			Decompose();
			
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			if(Changed){
				Changed=false;
				Recompose();
			}
			
			return Matrix_;
		}
		
		/// <summary>Prepares this matrix for interpolation with the given one.</summary>
		public void PrepareForInterpolate(InterpolationMatrix b){
			
			// 20.2.2
			Vector2 scaleA=Scale;
			Vector2 scaleB=b.Scale;
			float angleAi=Angle;
			float angleBi=b.Angle;
			float angleA=angleAi;
			float angleB=angleBi;
			
			
			if ((scaleA.x < 0f && scaleB.y < 0f) || (scaleA.y < 0f && scaleB.x < 0f)){
				scaleA.x = -scaleA.x;
				scaleA.y = -scaleA.y;
				angleA += angleA < 0f ? 180f : -180f;
				
				// Write out scale:
				Scale=scaleA;
				
			}
			
			// Don't rotate the long way around.
			if(angleA==0f){
				angleA = 360f;
			}
			
			if(angleB==0f){
				angleB = 360f;
			}
			
			if (Math.Abs(angleA - angleB) > 180f){
				if(angleA > angleB){
					angleA -= 360f;
				}else {
					angleB -= 360f;
				}	
			}
			
			// Write out angles:
			if(angleAi!=angleA){
				Angle=angleA;
			}
			
			if(angleBi!=angleB){
				b.Angle=angleB;
			}
			
		}
		
		
		/// <summary>Decompose the matrix now.</summary>
		public void Decompose(){
			
			// 20.2.1
			float row0x = Matrix_[0,0];
			float row0y = Matrix_[0,1];
			float row1x = Matrix_[1,0];
			float row1y = Matrix_[1,1];
			
			// Output translate:
			Translation = new Vector2(Matrix_[3,0],Matrix_[3,1]);
			
			// Build initial scale:
			Vector2 scale = new Vector2(
				(float)Math.Sqrt(row0x * row0x + row0y * row0y),
				(float)Math.Sqrt(row1x * row1x + row1y * row1y)
			);
			
			// If determinant is negative, one axis was flipped.
			double determinant = row0x * row1y - row0y * row1x;
			
			if (determinant < 0f){
				// Flip axis with minimum unit vector dot product.
				if(row0x < row1y){
					scale.x = -scale.x;
				}else{
					scale.y = -scale.y;
				}
			}
			
			// Write out scale:
			Scale=scale;
			
			// Renormalize matrix to remove scale. 
			if(scale[0]!=0f){
				row0x *= 1f / scale[0];
				row0y *= 1f / scale[0];
			}
			
			if(scale[1]!=0f){
				row1x *= 1f / scale[1];
				row1y *= 1f / scale[1];
			}
			
			// Compute rotation and renormalize matrix. 
			float angle = (float)Math.Atan2(row0y, row0x); 
			
			if(angle!=0f){
				// Rotate(-angle) = [cos(angle), sin(angle), -sin(angle), cos(angle)]
				//                = [row0x, -row0y, row0y, row0x]
				// Thanks to the normalization above.
				float sn = -row0y;
				float cs = row0x;
				float m11 = row0x;
				float m12 = row0y;
				float m21 = row1x;
				float m22 = row1y;
				row0x = cs * m11 + sn * m21;
				row0y = cs * m12 + sn * m22;
				row1x = -sn * m11 + cs * m21;
				row1y = -sn * m12 + cs * m22;
			}
			
			// Write out the 2x2 matrix:
			M11 = row0x;
			M12 = row0y;
			M21 = row1x;
			M22 = row1y;

			// Convert into degrees because our rotation functions expect it.
			Angle = Mathf.Rad2Deg * angle;
			
		}
		
		/// <summary>Recompose the matrix now.</summary>
		public void Recompose(){
			
			float m11=M11;
			float m12=M12;
			float m21=M21;
			float m22=M22;
			
			// Init as identity:
			Matrix_=Matrix4x4.identity;
			
			Matrix_[0,0] = m11;
			Matrix_[0,1] = m12;
			Matrix_[1,0] = m21;
			Matrix_[1,1] = m22;

			// Translate matrix.
			Vector2 translate=Translation;
			
			Matrix_[3,0] = translate[0] * m11 + translate[1] * m21;
			Matrix_[3,1] = translate[0] * m12 + translate[1] * m22;

			// Rotate matrix.
			float angle = Mathf.Deg2Rad * Angle;
			
			float cosAngle = (float)Math.Cos(angle);
			float sinAngle = (float)Math.Sin(angle);
			
			// New temporary, identity initialized, 4x4 matrix rotateMatrix:
			Matrix4x4 rotateMatrix=Matrix4x4.identity;
			
			rotateMatrix[0,0] = cosAngle;
			rotateMatrix[0,1] = sinAngle;
			rotateMatrix[1,0] = -sinAngle;
			rotateMatrix[1,1] = cosAngle;
			
			Matrix_ = Matrix_ * rotateMatrix;

			// Scale matrix.
			Vector2 scale=Scale;
			
			Matrix_[0,0] *= scale.x;
			Matrix_[0,1] *= scale.x;
			Matrix_[1,0] *= scale.y;
			Matrix_[1,1] *= scale.y;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"-spark-matrix"};
		}
		
		/// <summary>Gets the float value from the given param index.</summary>
		private float Get(int index){
			return this[index].GetRawDecimal();
		}
		
		/// <summary>Sets the float value at the given param index.</summary>
		private void Set(int index,float value){
			this[index].SetRawDecimal(value);
			Changed=true;
		}
		
		protected override Css.Value Clone(){
			InterpolationMatrix result=new InterpolationMatrix(Matrix_);
			return result;
		}
		
	}
	
}



