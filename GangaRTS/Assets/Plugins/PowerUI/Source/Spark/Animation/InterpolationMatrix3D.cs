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
	/// Represents an interpolatable matrix for 3D transforms (https://www.w3.org/TR/css-transforms-1/#matrix-interpolation).
	/// </summary>
	
	public class InterpolationMatrix3D:Css.Functions.Transformation{
		
		/// <summary>True if the params have changed and needs to be reconstructed.</summary>
		private bool Changed=false;
		/// <summary>The constructed raw matrix.</summary>
		private Matrix4x4 Matrix_=Matrix4x4.identity;
		/// <summary>A 4 component rotation.
		/// Note that this is not stored in the params so it can be correctly slerped.</summary>
		private Quaternion Rotation_;
		/// <summary>A 4 component rotation.
		/// Note that this is not stored in the params so it can be correctly slerped.</summary>
		public Quaternion Rotation{
			get{
				return Rotation_;
			}
			set{
				Rotation_=value;
				Changed=true;
			}
		}
		
		
		/// <summary>A 4 component perspective vector.</summary>
        public Vector4 Perspective{
			get{
				return new Vector4(Get(0),Get(1),Get(2),Get(3));
			}
			set{
				Set(0,value.x);
				Set(1,value.y);
				Set(2,value.z);
				Set(3,value.w);
			}
		}
		
		/// <summary>A 3 component translation.</summary>
		public Vector3 Translation{
			get{
				return new Vector3(Get(4),Get(5),Get(6));
			}
			set{
				Set(4,value.x);
				Set(5,value.y);
				Set(6,value.z);
			}
		}
		
		/// <summary>A 3 component scale.</summary>
		public Vector3 Scale{
			get{
				return new Vector3(Get(7),Get(8),Get(9));
			}
			set{
				Set(7,value.x);
				Set(8,value.y);
				Set(9,value.z);
			}
		}
		
		/// <summary>A 3 component skew.</summary>
		public Vector3 Skew{
			get{
				return new Vector3(Get(10),Get(11),Get(12));
			}
			set{
				Set(10,value.x);
				Set(11,value.y);
				Set(12,value.z);
			}
		}
		
		public InterpolationMatrix3D(){}
		
		public InterpolationMatrix3D(Matrix4x4 matrix){
			
			Name="-spark-matrix3d";
			
			// Set the composed form:
			Matrix_=matrix;
			
			// Params are 13 floats:
			Count=13;
			
			for(int i=0;i<13;i++){
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
		
		/// <summary>The determinant of the given matrix. Built in to newer versions of Unity.</summary>
		public static float Determinant(Matrix4x4 m){
			return m[0,3] * m[1,2] * m[2,1] * m[3,0] - m[0,2] * m[1,3] * m[2,1] * m[3,0] -
			 m[0,3] * m[1,1] * m[2,2] * m[3,0] + m[0,1] * m[1,3] * m[2,2] * m[3,0] +
			 m[0,2] * m[1,1] * m[2,3] * m[3,0] - m[0,1] * m[1,2] * m[2,3] * m[3,0] -
			 m[0,3] * m[1,2] * m[2,0] * m[3,1] + m[0,2] * m[1,3] * m[2,0] * m[3,1] +
			 m[0,3] * m[1,0] * m[2,2] * m[3,1] - m[0,0] * m[1,3] * m[2,2] * m[3,1] -
			 m[0,2] * m[1,0] * m[2,3] * m[3,1] + m[0,0] * m[1,2] * m[2,3] * m[3,1] +
			 m[0,3] * m[1,1] * m[2,0] * m[3,2] - m[0,1] * m[1,3] * m[2,0] * m[3,2] -
			 m[0,3] * m[1,0] * m[2,1] * m[3,2] + m[0,0] * m[1,3] * m[2,1] * m[3,2] +
			 m[0,1] * m[1,0] * m[2,3] * m[3,2] - m[0,0] * m[1,1] * m[2,3] * m[3,2] -
			 m[0,2] * m[1,1] * m[2,0] * m[3,3] + m[0,1] * m[1,2] * m[2,0] * m[3,3] +
			 m[0,2] * m[1,0] * m[2,1] * m[3,3] - m[0,0] * m[1,2] * m[2,1] * m[3,3] -
			 m[0,1] * m[1,0] * m[2,2] * m[3,3] + m[0,0] * m[1,1] * m[2,2] * m[3,3];
		}
		
		/// <summary>The combine function (20.1).</summary>
		public static Vector3 Combine(Vector3 a, Vector3 b, float ascl, float bscl){
			return new Vector3(
				(ascl * a.x) + (bscl * b.x),
				(ascl * a.y) + (bscl * b.y),
				(ascl * a.z) + (bscl * b.z)
			);
		}
		
		/// <summary>Decompose the matrix now.</summary>
		public bool Decompose(){
			
			// Normalize the matrix.
			
			float normaliser=Matrix_[3,3];
			
			if (normaliser == 0f){
				return false;
			}
			
			for (int i = 0; i < 4; i++){
				for (int j = 0; j < 4; j++){
					Matrix_[i,j] /= normaliser;
				}
			}
			
			// perspectiveMatrix is used to solve for perspective, but it also provides
			// an easy way to test for singularity of the upper 3x3 component.
			Matrix4x4 perspectiveMatrix = Matrix_;
			
			for(int i = 0; i < 3; i++){
				perspectiveMatrix[i,3] = 0f;
			}
			
			perspectiveMatrix[3,3] = 1f;

			if (Determinant(perspectiveMatrix) == 0f){
				return false;
			}
			
			// First, isolate perspective.
			if (Matrix_[0,3] != 0 || Matrix_[1,3] != 0 || Matrix_[2,3] != 0){
				
				// rightHandSide is the right hand side of the equation.
				Vector4 rightHandSide=new Vector4(
					Matrix_[0,3],
					Matrix_[1,3],
					Matrix_[2,3],
					Matrix_[3,3]
				);
				
				// Solve the equation by inverting perspectiveMatrix and multiplying
				// rightHandSide by the inverse.
				Matrix4x4 inversePerspectiveMatrix = perspectiveMatrix.inverse;
				Matrix4x4 transposedInversePerspectiveMatrix = inversePerspectiveMatrix.transpose;
				Perspective = transposedInversePerspectiveMatrix * rightHandSide;
				
			}else{
				// No perspective.
				Perspective = new Vector4(0f,0f,0f,1f);
			}
			
			// Next take care of translation:
			Translation=new Vector3(
				Matrix_[3,0],
				Matrix_[3,1],
				Matrix_[3,2]
			);
			
			// Now get scale and shear. 'row' is a 3 element array of 3 component vectors:
			Vector3[] row=new Vector3[3];
			
			for (int i = 0; i < 3; i++){
				
				row[i]=new Vector3(
					Matrix_[i,0],
					Matrix_[i,1],
					Matrix_[i,2]
				);
				
			}
			
			// Compute X scale factor and normalize first row.
			Vector3 scale=new Vector3(1f,1f,1f);
			Vector3 skew=new Vector3(0f,0f,0f);
			
			scale.x = row[0].magnitude;
			row[0] = row[0].normalized;
			
			// Compute XY shear factor and make 2nd row orthogonal to 1st.
			skew.x = Vector3.Dot(row[0], row[1]);
			row[1] = Combine(row[1], row[0], 1f, -skew.x);
			
			// Now, compute Y scale and normalize 2nd row.
			scale.y = row[1].magnitude;
			row[1] = row[1].normalized;
			skew.x /= scale.y;
			
			// Compute XZ and YZ shears, orthogonalize 3rd row
			skew.y = Vector3.Dot(row[0], row[2]);
			row[2] = Combine(row[2], row[0], 1f, -skew.y);
			skew.z = Vector3.Dot(row[1], row[2]);
			row[2] = Combine(row[2], row[1], 1f, -skew.z);
			
			// Next, get Z scale and normalize 3rd row.
			scale.z = row[2].magnitude;
			row[2] = row[2].normalized;
			skew.y /= scale.z;
			skew.z /= scale.z;
			
			// At this point, the matrix (in rows) is orthonormal.
			// Check for a coordinate system flip.  If the determinant
			// is -1, then negate the matrix and the scaling factors.
			Vector3 pdum3 = Vector3.Cross(row[1], row[2]);
			
			if (Vector3.Dot(row[0], pdum3) < 0f){
				for (int i = 0; i < 3; i++){
					scale[i] *= -1f;
					row[i][0] *= -1f;
					row[i][1] *= -1f;
					row[i][2] *= -1f;
				}
			}
			
			// Write out scale and skew now:
			Scale=scale;
			Skew=skew;
			
			// Now, get the rotations out:
			Quaternion quaternion=new Quaternion(
				0.5f * (float)Math.Sqrt(Math.Max(1 + row[0].x - row[1].y - row[2].z, 0)),
				0.5f * (float)Math.Sqrt(Math.Max(1 - row[0].x + row[1].y - row[2].z, 0)),
				0.5f * (float)Math.Sqrt(Math.Max(1 - row[0].x - row[1].y + row[2].z, 0)),
				0.5f * (float)Math.Sqrt(Math.Max(1 + row[0].x + row[1].y + row[2].z, 0))
			);
			
			if (row[2].y > row[1].z){
				quaternion.x = -quaternion.x;
			}
			
			if (row[0].z > row[2].x){
				quaternion.y = -quaternion.y;
			}
			
			if (row[1].x > row[0].y){
				quaternion.z = -quaternion.z;
			}
			
			// Apply rotation:
			Rotation=quaternion;
			
			return true;
			
		}
		
		/// <summary>Recompose the matrix now.</summary>
		public void Recompose(){
			
			Matrix_=Matrix4x4.identity;
			
			// apply perspective:
			Vector4 perspective=Perspective;
			
			for (int i = 0; i < 4; i++){
				Matrix_[i,3] = perspective[i];
			}
			
			// apply translation
			Vector3 translation=Translation;
			
			for (int i = 0; i < 3; i++){
				Matrix_[3,i] += translation.x * Matrix_[0,i];
				Matrix_[3,i] += translation.y * Matrix_[1,i];
				Matrix_[3,i] += translation.z * Matrix_[2,i];
			}
			
			// apply rotation
			float x = Rotation.x;
			float y = Rotation.y;
			float z = Rotation.z;
			float w = Rotation.w;
			
			// Construct a composite rotation matrix from the quaternion values
			// rotationMatrix is a identity 4x4 matrix initially:
			Matrix4x4 rotationMatrix = Matrix4x4.identity;
			
			rotationMatrix[0,0] = 1f - 2f * (y * y + z * z);
			rotationMatrix[0,1] = 2f * (x * y - z * w);
			rotationMatrix[0,2] = 2f * (x * z + y * w);
			rotationMatrix[1,0] = 2f * (x * y + z * w);
			rotationMatrix[1,1] = 1f - 2f * (x * x + z * z);
			rotationMatrix[1,2] = 2f * (y * z - x * w);
			rotationMatrix[2,0] = 2f * (x * z - y * w);
			rotationMatrix[2,1] = 2f * (y * z + x * w);
			rotationMatrix[2,2] = 1f - 2f * (x * x + y * y);
			
			Matrix_ *= rotationMatrix;
			
			// apply skew
			Vector3 skew=Skew;
			
			// temp is a identity 4x4 matrix initially
			Matrix4x4 temp=Matrix4x4.identity;
			
			if (skew.z!=0f){
				temp[2,1] = skew.z;
				Matrix_*=temp;
			}
			
			if (skew.y!=0f){
				temp[2,1] = 0f;
				temp[2,0] = skew.y;
				Matrix_*=temp;
			}
			
			if (skew.x!=0f){
				temp[2,0] = 0f;
				temp[1,0] = skew.x;
				Matrix_*=temp;
			}
			
			// apply scale
			Vector3 scale=Scale;
			
			for (int i = 0; i < 3; i++){
				for (int j = 0; j < 3; j++){
					Matrix_[i,j] *= scale[i];
				}
			}
			
		}
		
		/// <summary>True if this is a 3D transform.</summary>
		public override bool Is3D{
			get{
				return true;
			}
		}
		
		public override string[] GetNames(){
			return new string[]{"-spark-matrix3d"};
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
			InterpolationMatrix3D result=new InterpolationMatrix3D(Matrix_);
			return result;
		}
		
	}
	
}



