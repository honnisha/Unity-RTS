//--------------------------------------
//          Kulestar Unity HTTP
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
	
	public partial class ContentPackage{
		
		/// <summary>Called when the ready state changes.</summary>
		public Action<Dom.Event> onreadystatechange{
			get{
				return GetFirstDelegate<Action<Dom.Event>>("readystatechange");
			}
			set{
				addEventListener("readystatechange",new EventListener<Dom.Event>(value));
			}
		}
		
		/// <summary>Called when it's done loading.</summary>
		public Action<UIEvent> onload{
			get{
				return GetFirstDelegate<Action<UIEvent>>("load");
			}
			set{
				addEventListener("load",new EventListener<UIEvent>(value));
			}
		}
		
		/// <summary>Called when the request times out.</summary>
		public Action<UIEvent> ontimeout{
			get{
				return GetFirstDelegate<Action<UIEvent>>("timeout");
			}
			set{
				addEventListener("timeout",new EventListener<UIEvent>(value));
			}
		}
		
		/// <summary>Called when the request errors.</summary>
		public Action<UIEvent> onerror{
			get{
				return GetFirstDelegate<Action<UIEvent>>("error");
			}
			set{
				addEventListener("error",new EventListener<UIEvent>(value));
			}
		}
		
		/// <summary>Called when the request is aborted.</summary>
		public Action<UIEvent> onabort{
			get{
				return GetFirstDelegate<Action<UIEvent>>("abort");
			}
			set{
				addEventListener("abort",new EventListener<UIEvent>(value));
			}
		}
		
	}
	
}