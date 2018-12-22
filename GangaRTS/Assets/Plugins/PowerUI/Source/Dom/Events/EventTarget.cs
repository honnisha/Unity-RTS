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
	/// An event dispatch stack.
	/// Tracks the elements which an event is being dispatched to.
	/// </summary>
	internal class DispatchStack{
		
		/// <summary>The current stack size of the dispatch stack.</summary>
		internal int Size;
		/// <summary>Stacks can be pooled - the next one in the pool.</summary>
		internal DispatchStack Next;
		/// <summary>The current stack of targets being dispatched to.</summary>
		internal EventTarget[] Stack=new EventTarget[10];
		
		
		/// <summary>Doubles the size of the dispatch stack.</summary>
		internal void Resize(){
			
			EventTarget[] newStack=new EventTarget[Stack.Length*2];
			Array.Copy(Stack,0,newStack,0,Stack.Length);
			Stack=newStack;
			
		}
		
	}
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget : ExpandableObject, IEventTarget{
		
		/// <summary>A set of events for this document. See addEventListener.</summary>
		public EventsSet Events;
		
		/// <summary>Clears all events on this document.</summary>
		public void ClearEvents(){
			
			Events=null;
			
		}
		
		/// <summary>Gets the first delegate event handler for the given event name.</summary>
		protected T GetFirstDelegate<T>(string name){
			
			if(Events==null){
				return default(T);
			}
			
			return (T)(Events.GetFirstListener(name).Internal);
			
		}
		
		/// <summary>The current head of the pooled dispatched stacks.</summary>
		internal static DispatchStack PooledStack;
		/// <summary>The current dispatch stack.</summary>
		private static DispatchStack CurrentStack_;
		
		/// <summary>The active dispatch stack. Use event.deepPath to access it (available during dispatch only).</summary>
		internal static DispatchStack dispatchStackRef{
			get{
				return CurrentStack_;
			}
		}
		
		/// <summary>Handles triggering event handlers here. Elements can (and in PowerUI's case, do) override this.
		/// PowerUI overrides it to handle e.g. onclick="" attributes too.</summary>
		/// <return>Returns true if the local handlers killed the event.</return>
		protected virtual bool HandleLocalEvent(Event e,bool bubblePhase){
			
			if(Events!=null){
				Events.Run(e,!bubblePhase);
			}
			
			return e.cancelBubble;
			
		}
		
		/// <summary>Runs an event of the given name.</summary>
		public bool dispatchEvent(Event e){
			
			e.target=this;
			
			// First we must go all the way to the bottom of the DOM.
			// As we go, we'll build up a stack of the elements that we went past.
			
			// We'll store that stack in a DispatchStack object (which are pooled):
			DispatchStack stackNode=PooledStack;
			
			if(stackNode==null){
				stackNode=new DispatchStack();
			}else{
				// Pop from the pool:
				PooledStack=stackNode.Next;
			}
			
			
			int stackPointer=0;
			EventTarget[] stack=stackNode.Stack;
			int stackSize=stack.Length;
			
			EventTarget current=eventTargetParentNode;
			
			while(current!=null){
				
				// Resize required?
				if(stackPointer==stackSize){
					stackNode.Resize();
					stack=stackNode.Stack;
					stackSize=stack.Length;
				}
				
				// Push to stack:
				stack[stackPointer++]=current;
				
				// Up a level:
				current=current.eventTargetParentNode;
				
			}
			
			// Stack size (Both are for the Event API):
			stackNode.Size=stackPointer;
			CurrentStack_=stackNode;
			
			// Ok we've now got all the elements that we'll be attempting to trigger the event on.
			// Time for the event phases to begin!
			
			try{
				
				// Step 1 - Capture phase.
				e.eventPhase=Event.CAPTURING_PHASE;
				
				for(int i=stackPointer-1;i>=0;i--){
					
					// The current one:
					EventTarget currentTarget=stack[i];
					
					// Update currentTarget:
					e.currentTarget=currentTarget;
					
					// Run and test if it's been halted:
					if(currentTarget.HandleLocalEvent(e,false)){
						// Halt!
						e.eventPhase=Event.NONE;
						return !e._Cancelled;
					}
					
				}
				
				// Step 2 - Target phase (bubble events).
				e.eventPhase=Event.AT_TARGET;
				e.currentTarget=this;
				
				// Handle e.g. onclick attributes:
				if(HandleLocalEvent(e,true)){
					// It quit us
					return !e._Cancelled;
				}
				
				// Test if it's been halted:
				if(e.cancelBubble){
					// Halt!
					e.eventPhase=Event.NONE;
					return !e._Cancelled;
				}
				
				// Step 3 - Bubble phase.
				e.eventPhase=Event.BUBBLING_PHASE;
				
				for(int i=0;i<stackPointer;i++){
					
					// The current one:
					EventTarget currentTarget=stack[i];
					
					// Update currentTarget:
					e.currentTarget=currentTarget;
					
					// Run:
					if(currentTarget.HandleLocalEvent(e,true)){
						
						// Halt!
						e.eventPhase=Event.NONE;
						return !e._Cancelled;
						
					}
					
				}
				
			}catch(Exception ex){
				// Handler errored.
				Log.Add(ex);
			}
			
			// Pool the stack:
			stackNode.Next=PooledStack;
			PooledStack=stackNode;
			
			// Done!
			e.eventPhase=Event.NONE;
			
			return !e._Cancelled;
		}
		
		/// <summary>The document that this target belongs to.</summary>
		internal virtual Document eventTargetDocument{
			get{
				return null;
			}
		}
		
		/// <summary>The parent node as used by EventTarget during capture. Can be null.</summary>
		internal virtual EventTarget eventTargetParentNode{
			get{
				return null;
			}
		}
		
		/// <summary>The childNode set as used by EventTarget during capture. Can be null.</summary>
		internal virtual NodeList eventTargetChildren{
			get{
				return null;
			}
		}
		
		/// <summary>Gets a a tooltip.</summary>
		public virtual string getTitle(){
			return null;
		}
		
		/// <summary>Adds an event listener to this document.</summary>
		public void addEventListener(string name,EventListener listener){
			
			if(Events==null){
				Events=new EventsSet();
			}
			
			Events.Add(name,listener);
			
		}
		
		/// <summary>Adds an event listener to this document.</summary>
		public void addEventListener(string name,EventListener listener,bool useCapture){
			
			if(Events==null){
				Events=new EventsSet();
			}
			
			listener.Capture=useCapture;
			Events.Add(name,listener);
			
		}
		
		/// <summary>Removes an event listener from this document.</summary>
		public void removeEventListener(string name,object evtHandlerInternal){
			
			if(Events==null){
				return;
			}
			
			Events.Remove(name,evtHandlerInternal);
			
		}
		
	}
	
}