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
	/// Represents a transformation applied to the vertices of an element on the UI.
	/// It generates a matrix when applied. This matrix is then applied to any existing matrix
	/// (the current top of a stack) and the result is pushed to the same stack.
	/// The matrix at the top of the stack is the one which is actually applied to elements.
	/// </summary>

	public class Transformation{
		
		/// <summary>Has this transformation got an active raw matrix?</summary>
		private bool HasRawMatrix;
		/// <summary>True if any component of this transformation changed.</summary>
		private bool _Changed;
		/// <summary>A raw matrix to apply.</summary>
		private Matrix4x4 _RawMatrix=Matrix4x4.identity;
		/// <summary>The location of the transform origin.</summary>
		private Vector3 _Origin;
		/// <summary>The fully resolved matrix (i.e. the parent matrix and my local one).</summary>
		private Matrix4x4 _Matrix;
		/// <summary>A translation to apply in world units.</summary>
		private Vector3 _Translate;
		/// <summary>Rotation to apply.</summary>
		private Quaternion _Rotation=Quaternion.identity;
		/// <summary>The parent transform, if any.</summary>
		public Transformation Parent;
		/// <summary>The location of the origin relative to the top corner
		/// of the element this transformation is on.</summary>
		private Value _OriginOffset=null;
		/// <summary>The matrix that represents only this transformation.</summary>
		private Matrix4x4 _LocalMatrix;
		/// <summary>Scale to apply.</summary>
		private Vector3 _Scale=Vector3.one;
		/// <summary>The location of the origin. May be relative (to this element)
		/// or fixed (fixed place on the screen).</summary>
		private int _OriginPosition=PositionMode.Relative;
		
		
		public Transformation(){}
		
		public Transformation(Matrix4x4 matrix){
			
			// Set raw:
			RawMatrix=matrix;
			
		}
		
		/// <summary>Read only version of the fully resolved matrix - parent one included.</summary>
		public Matrix4x4 Matrix{
			get{
				return _Matrix;
			}
		}
		
		/// <summary>Read only version of the matrix that represents this transformation.</summary>
		public Matrix4x4 LocalMatrix{
			get{
				return _LocalMatrix;
			}
		}
		
		/// <summary>Calculates where the transformation origin should go in screen space.</summary>
		/// <param name="relativeTo">The computed style of the element that the origin will be
		/// relative to if the origin position is 'Relative'</param>
		private Vector3 CalculateOrigin(ComputedStyle relativeTo,LayoutBox box){
			// We need to figure out where the origin is and then apply the parent transformation to it.
			
			Vector3 origin;
			
			if(_OriginOffset==null){
				
				// Default (50%,50%,0):
				origin=new Vector3(0.5f * box.Width, 0.5f * box.Height, 0f);
				
			}else if(_OriginOffset is CssFunction){
				
				RenderableData rd=relativeTo.RenderData;
				
				origin=new Vector3(
					_OriginOffset.GetDecimal(rd,ValueAxis.X),
					_OriginOffset.GetDecimal(rd,ValueAxis.Y),
					0f
				);
				
			}else{
				
				float z=0f;
				RenderableData rd=relativeTo.RenderData;
				
				if(_OriginOffset.Count==3){
					z=_OriginOffset[2].GetDecimal(rd,ValueAxis.X);
				}
				
				origin=new Vector3(
					_OriginOffset[0].GetDecimal(rd,ValueAxis.X),
					_OriginOffset[1].GetDecimal(rd,ValueAxis.Y),
					z
				);
			
			}
			
			if(_OriginPosition==PositionMode.Relative){
				
				origin.x+=box.X;
				origin.y+=box.Y;
				
			}
			
			// Map origin to world space:
			Renderman renderer=(relativeTo.Element.document as PowerUI.HtmlDocument).Renderer;
			origin=renderer.PixelToWorldUnit(origin.x,origin.y,relativeTo.ZIndex);
			
			if(Parent!=null){
				origin=Parent.Apply(origin);
			}
			
			return origin;
			
		}
		
		/// <summary>Recalculates the matrices if this transformation has changed.</summary>
		public void RecalculateMatrix(ComputedStyle style,LayoutBox box){
			
			// New origin:
			Vector3 origin=CalculateOrigin(style,box);
			
			if(origin!=_Origin){
				_Changed=true;
				_Origin=origin;
			}
			
			if(Changed){
				
				_Changed=false;
				
				_LocalMatrix=Matrix4x4.TRS(_Origin,Quaternion.identity,Vector3.one);
				
				// Raw matrix (i.e. usually originates from translate or skew):
				if(HasRawMatrix){
					_LocalMatrix*=_RawMatrix;
				}
				
				_LocalMatrix*=Matrix4x4.TRS(_Translate,_Rotation,_Scale);
				
				_LocalMatrix*=Matrix4x4.TRS(-_Origin,Quaternion.identity,Vector3.one);
			}
			
			if(Parent!=null){
				_Matrix=Parent.Matrix*_LocalMatrix;
			}else{
				_Matrix=_LocalMatrix;
			}
		}
		
		/// <summary>True if any property of this transformation changed.</summary>
		public bool Changed{
			get{
				return _Changed;
			}
		}
		
		/// <summary>The PositionMode of the origin. May be fixed (i.e. in exact screen pixels) or
		/// relative to the element that this transformation is on.</summary>
		public int OriginPosition{
			get{
				return _OriginPosition;
			}
			set{
				_OriginPosition=value;
				_Changed=true;
			}
		}
		
		/// <summary>The position of the origin relative to the top left corner of the element.</summary>
		public void SetOriginOffset(Value value){
			
			_OriginOffset=value;
			_Changed=true;
			
		}
		
		/// <summary>A scale transformation to apply (post process).</summary>
		public Vector3 Scale{
			get{
				return _Scale;
			}
			set{
				_Scale=value;
				_Changed=true;
			}
		}
		
		/// <summary>A translate transformation to apply (post process).</summary>
		public Vector3 Translate{
			get{
				return _Translate;
			}
			set{
				_Translate=value;
				_Changed=true;
			}
		}
		
		/// <summary>A rotation to apply (post process).</summary>
		public Quaternion Rotation{
			get{
				return _Rotation;
			}
			set{
				_Rotation=value;
				_Changed=true;
			}
		}
		
		/// <summary>A skew to apply (post process).</summary>
		public Matrix4x4 Skew{
			get{
				return _RawMatrix;
			}
			set{
				_RawMatrix=value;
				_Changed=true;
				HasRawMatrix=(value!=Matrix4x4.identity);
			}
		}
		
		/// <summary>A raw matrix to apply (originates from transform: CSS). Post process.</summary>
		public Matrix4x4 RawMatrix{
			get{
				return _RawMatrix;
			}
			set{
				_RawMatrix=value;
				_Changed=true;
				HasRawMatrix=(value!=Matrix4x4.identity);
			}
		}
		
		/// <summary>Applies this transformation to the given vertex.</summary>
		/// <param name="point">The vertex to transform.</param>
		/// <returns>The transformed vertex.</returns>
		public Vector3 Apply(Vector4 point){
			point.w=1f;
			return _Matrix*point;
		}
		
		/// <summary>Applies this transformation to the given vertex.</summary>
		/// <param name="point">The vertex to transform.</param>
		/// <returns>The transformed vertex.</returns>
		public Vector3 ApplyInverse(Vector4 point){
			point.w=1f;
			return _Matrix.inverse*point;
		}
		
	}
	
}