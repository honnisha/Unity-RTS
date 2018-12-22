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

namespace Svg{
	
	// SVG event objects.
	
	public delegate void SVGEventDelegate(SVGEvent e);
	public delegate void SVGZoomEventDelegate(SVGZoomEvent e);
	
	public class SVGEvent : Dom.Event{
		
		public SVGEvent(){}
		
		public SVGEvent(string type,object init):base(type,init){}
		
	}
	
	public class SVGZoomEvent : SVGEvent{
		
		public SVGZoomEvent(){}
		
		public SVGZoomEvent(string type,object init):base(type,init){}
		
	}
	
}