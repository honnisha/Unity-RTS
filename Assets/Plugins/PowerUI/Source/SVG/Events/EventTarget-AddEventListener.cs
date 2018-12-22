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
using PowerUI;
using Svg;


namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		// Lots of event-specific addEventListener overloads.
		// This avoids needing to manually create e.g. a EventListener<KeyboardEvent> object.
		
		/// <summary>Adds an event listener to this document.</summary>
		public void addEventListener(string name,Action<SVGEvent> method){
			addEventListener(name,new EventListener<SVGEvent>(method));
		}
		
		public void addEventListener(string name,Action<SVGZoomEvent> method){
			addEventListener(name,new EventListener<SVGZoomEvent>(method));
		}
		
	}
	
}