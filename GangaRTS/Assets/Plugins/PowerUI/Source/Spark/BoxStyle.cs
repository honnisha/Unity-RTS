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
using System.Collections;
using System.Collections.Generic;
using System.Text;


namespace Css{
	
	/// <summary>
	/// The classic CSS box. Used when computing the box model, such as for margins and padding.
	/// </summary>
	
	public struct BoxStyle{
		
		/// <summary>Value for the top edge.</summary>
		public float Top;
		/// <summary>Value for the right edge.</summary>
		public float Right;
		/// <summary>Value for the bottom edge.</summary>
		public float Bottom;
		/// <summary>Value for the left edge.</summary>
		public float Left;
		
		
		public BoxStyle(float t,float r,float b,float l){
			Top=t;
			Right=r;
			Bottom=b;
			Left=l;
		}
		
		/// <summary>Gets the size at a numeric index.</summary>
		/// <param name="i">The index. Goes around clockwise: 0=Top, 1=Right, 2=Bottom, 3=Left.</param>
		public float this[int i]{
			get{
				
				switch(i){
					case 0:
						return Top;
					case 1:
						return Right;
					case 2:
						return Bottom;
				}
				
				return Left;
			}
			set{
				switch(i){
					case 0:
						Top=value;
					break;
					case 1:
						Right=value;
					break;
					case 2:
						Bottom=value;
					break;
					case 3:
						Left=value;
					break;
				}
			}
		}
		
		public override string ToString(){
			
			return "Box(Top: "+Top+",Right: "+Right+", Bottom: "+Bottom+", Left: "+Left+")";
			
		}
		
	}
	
}