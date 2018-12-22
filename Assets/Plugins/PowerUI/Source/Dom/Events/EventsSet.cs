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
	/// A set of events for a given object (document or element). Used by addEventListener.
	/// </summary>
	
	public class EventsSet{
		
		/// <summary>The handlers in this set. May be null.</summary>
		public Dictionary<string,List<EventListener>> Handlers;
		
		
		/// <summary>Gets the set of handlers for the given event name.</summary>
		public List<EventListener> Get(string name){
			
			if(Handlers==null){
				return null;
			}
			
			// Try getting the handler set:
			List<EventListener> list;
			Handlers.TryGetValue(name,out list);
			
			return list;
		}
		
		/// <summary>Runs the event with the given name by passing it the given event object.</summary>
		/// <returns>True if at least one handler was triggered.</returns>
		public bool Run(Event e,bool capture){
			
			if(Handlers==null){
				return false;
			}
			
			if(e==null || e.type==null){
				Dom.Log.Add("Failed event dispatch (The event did not have a type!)");
				return false;
			}
			
			// Try getting the handler:
			List<EventListener> list;
			if(Handlers.TryGetValue(e.type,out list)){
				
				// How many?
				int count=list.Count;
				
				// Run each one:
				for(int i=0;i<count;i++){
					
					EventListener listener=list[i];
					
					if(capture!=listener.Capture){
						continue;
					}
					
					listener.handleEvent(e);
					
					if(e._CancelImmediate){
						// Halt there.
						break;
					}
					
				}
				
				// Ok
				return true;
				
			}
			
			// No handlers.
			return false;
			
		}
		
		/// <summary>Internally adds an event handler for the given named event.</summary>
		public void Add(string name,EventListener handler){
			
			if(Handlers==null){
				Handlers=new Dictionary<string,List<EventListener>>();
			}
			
			List<EventListener> list;
			if(!Handlers.TryGetValue(name,out list)){
				
				// Create a new list:
				list=new List<EventListener>();
				
				// Add it:
				Handlers[name]=list;
				
			}
			
			list.Add(handler);
			
		}
		
		/// <summary>Gets the first delegate event handler for the given event name.</summary>
		public EventListener GetFirstListener(string name){
			
			// Get the set:
			List<EventListener> handlers=Get(name);
			
			if(handlers==null){
				return null;
			}
			
			return handlers[0];
			
		}
		
		/// <summary>Removes the given method from this set.</summary>
		public void Remove(string name,object mtd){
			
			// Get the set:
			List<EventListener> handlerSet=Get(name);
			
			if(handlerSet==null){
				return;
			}
			
			for(int i=0;i<handlerSet.Count;i++){
				
				if(handlerSet[i].Matches(mtd)){
					
					if(handlerSet.Count==1){
						// Remove it all:
						Handlers.Remove(name);
					}else{
						// Remove just this:
						handlerSet.RemoveAt(i);
					}
					
				}
				
			}
			
		}
		
	}
	
}