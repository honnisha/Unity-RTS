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
	/// Represents the matrix transform function.
	/// </summary>
	
	public class MatrixFunction:Transformation{
		
		/// <summary>The raw matrix.</summary>
		public Matrix4x4 Matrix=Matrix4x4.identity;
		
		
		public MatrixFunction(){
			Name="matrix";
		}
		
		/// <summary>Sets the default params for this transformation.</summary>
		public override void SetDefaults(){
			Clear(0f);
			Matrix=Matrix4x4.identity;
			
			// Apply main diag:
			
			if(Count<=6){
				
				// 2D matrix.
				this[0].SetRawDecimal(1f); // Scale X
				this[3].SetRawDecimal(1f); // Scale Y
				return;
				
			}
			
			// 3D matrix
			this[0].SetRawDecimal(1f); // Scale X
			this[5].SetRawDecimal(1f); // Scale Y
			this[10].SetRawDecimal(1f); // Scale Z
			
		}
		
		private float Get(int i,float def){
			if(Count<i){
				return def;
			}
			
			return Values[i].GetRawDecimal();
		}
		
		/// <summary>True if this is a 3D transform.</summary>
		public override bool Is3D{
			get{
				return Count>6;
			}
		}
		
		public override Matrix4x4 CalculateMatrix(RenderableData context){
			
			int count=Count;
			
			if(count<=6){
				
				Matrix[12]=context.ScaleToWorldX( -Get(4,0f) ); // Trans X
				Matrix[13]=context.ScaleToWorldY( Get(5,0f) ); // Trans Y
				
				return Matrix;
				
			}else if(count==16){
				
				Matrix[12]=context.ScaleToWorldX( -Get(12,0f) ); // Trans X
				Matrix[13]=context.ScaleToWorldY( Get(13,0f) ); // Trans Y
				Matrix[14]=context.ScaleToWorldX( Get(14,0f) ); // Trans Z
			}
			
			return Matrix;
			
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			int count=Count;
			
			if(count<=6){
				
				// 2D matrix.
				Matrix[0]=Get(0,1f); // Scale X
				Matrix[1]=-Get(1,0f); // Skew Y
				
				Matrix[4]=-Get(2,0f); // Skew X
				Matrix[5]=Get(3,1f); // Scale Y
				
				return;
				
			}
			
			// 3D matrix (Hackey! Got to somehow apply a valid mapping from CSS space to world space)
			Matrix[0]=Get(0,1f); // Scale X
			Matrix[1]=-Get(1,0f); // Skew Y
			Matrix[2]=Get(2,0f);
			Matrix[3]=Get(3,0f);
			Matrix[4]=-Get(4,0f); // Skew X
			Matrix[5]=Get(5,1f); // Scale Y
			Matrix[6]=Get(6,0f);
			Matrix[7]=-Get(7,0f);
			Matrix[8]=Get(8,0f);
			Matrix[9]=Get(9,0f);
			Matrix[10]=Get(10,1f); // Scale Z
			Matrix[11]=Get(11,0f); // Perspective
			Matrix[15]=Get(15,1f);
			
		}
		
		public override string[] GetNames(){
			return new string[]{"matrix","matrix3d"};
		}
		
		protected override Css.Value Clone(){
			MatrixFunction result=new MatrixFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



