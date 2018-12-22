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
using Dom;
using Blaze;
using UnityEngine;
using Svg;
using Css;

namespace Css{
	
	/// <summary>
	/// Represents SVGElement.style, a likely upcoming API.
	/// It hosts the computed style amongst other things.
	/// </summary>
	
	public partial class ElementStyle{
		
		/// <summary>The fill of this element.</summary>
		public string fill{
			set{
				Set("fill",value);
			}
			get{
				return GetString("fill");
			}
		}
		
		/// <summary>The stroke of this element.</summary>
		public string stroke{
			set{
				Set("stroke",value);
			}
			get{
				return GetString("stroke");
			}
		}
		
		/// <summary>The flood-color of this element (feFlood).</summary>
		public string floodColor{
			set{
				Set("flood-color",value);
			}
			get{
				return GetString("flood-color");
			}
		}
		
		/// <summary>The flood-opacity of this element (feFlood).</summary>
		public string floodOpacity{
			set{
				Set("flood-opacity",value);
			}
			get{
				return GetString("flood-opacity");
			}
		}
		
	}
	
}