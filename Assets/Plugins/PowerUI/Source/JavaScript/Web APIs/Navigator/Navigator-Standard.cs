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
		
		/// <summary>The app code name.</summary>
		public string appCodeName{
			get{
				return "Kulestar";
			}
		}
		
		/// <summary>The app name.</summary>
		public string appName{
			get{
				return "PowerUI";
			}
		}
		
		/// <summary>The version.</summary>
		public string appVersion{
			get{
				return UI.Major+"."+UI.Minor+" ("+Application.platform+")";
			}
		}
		
		/// <summary>OS and CPU.</summary>
		public string oscpu{
			get{
				return SystemInfo.operatingSystem;
			}
		}
		
		/// <summary>The DNT header.</summary>
		public int doNotTrack{
			get{
				return 0;
			}
		}
		
		/// <summary>The user agent.</summary>
		public string userAgent{
			get{
				return UI.UserAgent;
			}
		}
		
		/// <summary>The revision.</summary>
		public string buildID{
			get{
				return UI.Revision+"";
			}
		}
		
		/// <summary>Cookies enabled?</summary>
		public bool cookieEnabled{
			get{
				return true;
			}
		}
		
		/// <summary>True if the browser currently has an active network connection.
		/// Similar to the Chrome/Safari implementation; only checks for a LAN link.</summary>
		public bool onLine{
			get{
				return Application.internetReachability!=NetworkReachability.NotReachable;
			}
		}
		
		/// <summary>The vendor.</summary>
		public string vendor{
			get{
				return "Kulestar";
			}
		}
		
		/// <summary>The vendor.</summary>
		public string vendorSub{
			get{
				return "";
			}
		}
		
		/// <summary>The web engine.</summary>
		public string product{
			get{
				return "Spark";
			}
		}
		
		/// <summary>The web engine.</summary>
		public string productSub{
			get{
				return UI.Revision+"";
			}
		}
		
	}
	
	public partial class Window{
		
		private Navigator navigator_;
		
		/// <summary>Information about the navigator.</summary>
		public Navigator navigator{
			get{
				if(navigator_==null){
					navigator_=new Navigator();
				}
				return navigator_;
			}
		}
		
	}
	
}
	