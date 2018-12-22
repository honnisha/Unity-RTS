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
using System.Collections;
using System.Collections.Generic;
using PowerUI.Http;
using Dom;


namespace PowerUI{
	
	/// <summary>The JS DOM 'screen' object.</summary>
	public partial class JSScreen{
		
		/// <summary>Called when the ready state changes.</summary>
		public Action<DeviceOrientationEvent> onorientationchange{
			get{
				return GetFirstDelegate<Action<DeviceOrientationEvent>>("orientationchange");
			}
			set{
				addEventListener("orientationchange",new EventListener<DeviceOrientationEvent>(value));
			}
		}
		
	}
	
}