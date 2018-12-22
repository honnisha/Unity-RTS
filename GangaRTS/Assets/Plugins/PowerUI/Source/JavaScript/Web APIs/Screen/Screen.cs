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
	public partial class JSScreen : EventTarget{
		
		/// <summary>Available top (can't be diffferent here in Unity).</summary>
		public int availTop{
			get{
				return 0;
			}
		}
		
		/// <summary>Available left (can't be diffferent here in Unity).</summary>
		public int availLeft{
			get{
				return 0;
			}
		}
		
		/// <summary>top.</summary>
		public int top{
			get{
				return 0;
			}
		}
		
		/// <summary>Current screen orientation.</summary>
		public JSScreenOrientation orientation{
			get{
				return new JSScreenOrientation();
			}
		}
		
		/// <summary>left.</summary>
		public int left{
			get{
				return 0;
			}
		}
		
		/// <summary>Pixel depth. Always 24 per CSSOM.</summary>
		public int pixelDepth{
			get{
				return 24;
			}
		}
		
		/// <summary>Screen DPI.</summary>
		public int dpi{
			get{
				return ScreenInfo.Dpi;
			}
		}
		
		/// <summary>Screen width.</summary>
		public int width{
			get{
				return ScreenInfo.ScreenX;
			}
		}
		
		/// <summary>Screen height.</summary>
		public int height{
			get{
				return ScreenInfo.ScreenY;
			}
		}
		
		/// <summary>Available screen width (can't be diffferent here in Unity).</summary>
		public int availWidth{
			get{
				return ScreenInfo.ScreenX;
			}
		}
		
		/// <summary>Available screen height (can't be diffferent here in Unity).</summary>
		public int availHeight{
			get{
				return ScreenInfo.ScreenY;
			}
		}
		
		/// <summary>Locks the orientation.</summary>
		public bool lockOrientation(string orientation){
			
			// Get orientation:
			ScreenOrientation orient=JSScreenOrientation.toUnity(orientation);
			
			// Apply:
			UnityEngine.Screen.orientation=orient;
			
			return true;
			
		}
		
		/// <summary>Unlocks the orientation.</summary>
		public void unlockOrientation(){
			
			// Apply:
			UnityEngine.Screen.orientation=ScreenOrientation.AutoRotation;
			
		}
		
	}
	
}