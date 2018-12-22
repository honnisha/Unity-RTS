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


namespace Css{

	/// <summary>
	/// The viewport of a document. These are typically used to resolve % which are relative to the "screen".
	/// </summary>

	public class Viewport : ScreenRegion{
		
		private float _Width;
		private float _Height;
		private float _Diagonal;
		
		
		public float Width{
			get{
				return _Width;
			}
			set{
				_Width=value;
				Recalculate();
			}
		}
		
		public float Height{
			get{
				return _Height;
			}
			set{
				_Height=value;
				Recalculate();
			}
		}
		
		public float Diagonal{
			get{
				return _Diagonal;
			}
		}
		
		public Viewport(){}
		
		public Viewport(float w,float h){
			_Width=w;
			_Height=h;
			Recalculate();
		}
		
		public void Update(float w,float h){
			_Width=w;
			_Height=h;
			Recalculate();
		}
		
		private void Recalculate(){
			
			// "Normalised" diagonal
			_Diagonal=(float)Math.Sqrt(_Width*_Width + _Height*_Height) / 1.41421f; // sqrt(2)
			
		}
		
	}
	
}