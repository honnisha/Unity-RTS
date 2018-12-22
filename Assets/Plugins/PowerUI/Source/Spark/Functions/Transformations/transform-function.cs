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
using Css.Units;
using UnityEngine;


namespace Css.Functions{
	
	/// <summary>
	/// Represents all transformation css functions, such as scale().
	/// </summary>
	
	public class Transformation:CssFunction{
		
		
		public Transformation(){
			
			Type=ValueType.RelativeNumber;
			
		}
		
		/// <summary>Sets the default params for this transformation.</summary>
		public virtual void SetDefaults(){
			Clear(0f);
		}
		
		/// <summary>Sets the raw decimal of all the parameters to the given value.</summary>
		protected void Clear(float to){
			for(int i=0;i<Count;i++){
				// Get as a raw value and set to 'to':
				this[i].SetRawDecimal(to);
			}
		}
		
		/// <summary>True if this is a 3D transform.</summary>
		public virtual bool Is3D{
			get{
				return false;
			}
		}
		
		/// <summary>Builds the transformation matrix.</summary>
		public virtual Matrix4x4 CalculateMatrix(RenderableData context){
			return Matrix4x4.identity;
		}
		
	}
	
}