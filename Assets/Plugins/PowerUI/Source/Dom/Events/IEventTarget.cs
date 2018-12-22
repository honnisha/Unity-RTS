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


namespace Dom{
	
	/// <summary>
	/// An interface for something that can receive events.
	/// Typically used by custom objects (such as gameworld objects) to receive DOM events.
	/// </summary>
	
	public interface IEventTarget{
		
		/// <summary>Dispatches an event to this target.</summary>
		bool dispatchEvent(Event e);
		
	}
	
}