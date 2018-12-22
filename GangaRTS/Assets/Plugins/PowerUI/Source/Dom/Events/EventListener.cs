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
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Describes either a nitro event or a delegate.
	/// </summary>
	
	public class EventListener{
		
		/// <summary>True if this is a capture phase listener.</summary>
		public bool Capture;
		
		/// <summary>The internal specific delegate instance.</summary>
		public virtual object Internal{
			get{
				return null;
			}
		}
		
		/// <summary>Checks if the given method equals this listeners internal method handler.</summary>
		internal bool Matches(object mtd){
			return (Internal==mtd);
		}
		
		/// <summary>Call this to run the method.</summary>
		public virtual void handleEvent(Event e){
		}
		
	}
	
	/// <summary>
	/// Describes a delegate event listener for a 
	/// particular event type and its associated delegate type.
	/// </summary>
	public class EventListener<E> : EventListener where E:Dom.Event{
		
		/// <summary>The listener to use.</summary>
		public Action<E> Listener;
		
		public EventListener(Action<E> listener){
			Listener=listener;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(Dom.Event e){
			Listener((E)e);
		}
		
	}
	
}