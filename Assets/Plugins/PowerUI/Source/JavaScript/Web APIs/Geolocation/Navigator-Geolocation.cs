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


namespace PowerUI{

	/// <summary>
	/// Used by window.navigator.
	/// </summary>
	
	public partial class Navigator{
		
		/// <summary>Geolocation services.</summary>
		private Geolocation geolocation_;
		
		/// <summary>Geolocation services.</summary>
		public Geolocation geolocation{
			get{
				if(geolocation_==null){
					geolocation_=new Geolocation();
				}
				
				return geolocation_;
			}
		}
		
	}
	
}	