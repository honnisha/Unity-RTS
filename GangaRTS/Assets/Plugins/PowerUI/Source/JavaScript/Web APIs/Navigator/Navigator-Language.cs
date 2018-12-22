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
using System.Globalization;


namespace PowerUI{
	
	public partial class Navigator{
		
		/// <summary>The preferred (system) language.</summary>
		public string language{
			get{
				return CultureInfo.CurrentUICulture.Name;
			}
		}
		
		/// <summary>A set of preferred languages.</summary>
		public string[] languages{
			get{
				return new string[]{language};
			}
		}
		
	}
	
}