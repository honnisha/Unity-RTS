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
using Css;

namespace Css{
	
	/// <summary>
	/// Represents MathElement.style, a likely upcoming API.
	/// It hosts the computed style amongst other things.
	/// </summary>
	
	public partial class ElementStyle{
		
		/// <summary>-spark-script-min-size.</summary>
		public string scriptMinSize{
			set{
				Set("-spark-script-min-size",value);
			}
			get{
				return GetString("-spark-script-min-size");
			}
		}
		
		/// <summary>-spark-script-level.</summary>
		public string scriptLevel{
			set{
				Set("-spark-script-level",value);
			}
			get{
				return GetString("-spark-script-level");
			}
		}
		
	}
	
}