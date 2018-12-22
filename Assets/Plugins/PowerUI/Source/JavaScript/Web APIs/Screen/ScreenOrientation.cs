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


namespace PowerUI{
	
	/// <summary>The JS DOM 'screen orientation' object.</summary>
	public partial class JSScreenOrientation{
		
		public string type;
		
		
		public JSScreenOrientation(string t){
			type=t;
		}
		
		public JSScreenOrientation(){
			
			// Get orientation:
			UnityEngine.DeviceOrientation orient=UnityEngine.Input.deviceOrientation;
			
			if(orient==UnityEngine.DeviceOrientation.Unknown){
				
				// Unknown - check for landscape based on width being bigger:
				if(UnityEngine.Screen.width>UnityEngine.Screen.height){
					orient=UnityEngine.DeviceOrientation.LandscapeLeft;
				}else{
					orient=UnityEngine.DeviceOrientation.LandscapeRight;
				}
				
			}
			
			type=fromUnity(orient);
		}
		
		/// <summary>The unity orientation type.</summary>
		public ScreenOrientation unityType{
			get{
				return toUnity(type);
			}
		}
		
		/// <summary>Converts from a Unity type.</summary>
		public static string fromUnity(UnityEngine.DeviceOrientation so){
			
			switch(so){
				
				case DeviceOrientation.Portrait:
					return "portrait-primary";
				case DeviceOrientation.PortraitUpsideDown:
					return "portrait-secondary";
				case DeviceOrientation.LandscapeRight:
					return "landscape-secondary";
				
			}
			
			// Assume landscape:
			return "landscape-primary";
			
		}
		
		/// <summary>Converts to a Unity type.</summary>
		public static ScreenOrientation toUnity(string type){
			
			switch(type){
				
				case "default":
					
					// Check dimensions:
					if(UnityEngine.Screen.width<UnityEngine.Screen.height){
						
						// Tall - portrait:
						return ScreenOrientation.Portrait;
						
					}
					
				break;
				case "portrait":
				case "portrait-primary":
					return ScreenOrientation.Portrait;
				case "portrait-secondary":
					return ScreenOrientation.PortraitUpsideDown;
				case "landscape-secondary":
					return ScreenOrientation.LandscapeRight;
				
			}
			
			// Assume landscape:
			return ScreenOrientation.LandscapeLeft;
			
		}
		
		/// <summary>Orientation angle.</summary>
		public float angle{
			get{
				
				// Check dimensions:
				bool portraitFirst=(UnityEngine.Screen.width<UnityEngine.Screen.height);
				
				// Get orientation:
				ScreenOrientation so=toUnity(type);
				
				if(portraitFirst){
					
					switch(so){
						case ScreenOrientation.LandscapeLeft:
							return 270f;
						case ScreenOrientation.PortraitUpsideDown:
							return 180f;
						case ScreenOrientation.LandscapeRight:
							return 90f;
					}
					
					// Portrait
					return 0f;
					
				}
				
				// Landscape
				switch(so){
					case ScreenOrientation.LandscapeLeft:
						return 0f;
					case ScreenOrientation.Portrait:
						return 90f;
					case ScreenOrientation.LandscapeRight:
						return 180f;
					case ScreenOrientation.PortraitUpsideDown:
						return 270f;
				}
				
				// Landscape left.
				return 0f;
				
			}
		}
		
		/// <summary>Unlocks the orientation.</summary>
		public void unlock(){
			UnityEngine.Screen.orientation=ScreenOrientation.AutoRotation;
		}
		
		/// <summary>Locks the orientation.</summary>
		public bool @lock(){
			UnityEngine.Screen.orientation=toUnity(type);
			return true;
		}
		
		/// <summary>Used by === in JS.</summary>
		public override string ToString(){
			return type;
		}
		
	}
	
}