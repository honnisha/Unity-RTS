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
	/// Represents the inverse section of round borders.
	/// Simply acts as a block allocation container, separating the "inverse" corner from the main coloured corner.
	/// </summary>
	
	public class RoundBorderInverseProperty:DisplayableProperty{
		
		
		/// <summary>Creates a new border property for the given element.</summary>
		/// <param name="data">The renderable object to give round border info to.</param>
		public RoundBorderInverseProperty(RenderableData data):base(data){
			
		}
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				return 90;
			}
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){}
		
		internal override void Layout(LayoutBox box,Renderman renderer){}
		
	}
	
}