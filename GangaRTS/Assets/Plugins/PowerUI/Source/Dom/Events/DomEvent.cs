using System;


namespace Dom{
	
	/// <summary>
	/// The root of all DOM events.
	/// </summary>
	public partial class Event{
		
		/// <summary>Event phase (none).</summary>
		public const int NONE=0;
		/// <summary>Event capturing phase.</summary>
		public const int CAPTURING_PHASE=1;
		/// <summary>Event target phase.</summary>
		public const int AT_TARGET=2;
		/// <summary>Event bubbling phase.</summary>
		public const int BUBBLING_PHASE=3;
		
		/// <summary>The type of this event. Only set if you directly use the UIEvent(type) constructor. See type instead.</summary>
		internal string EventType;
		/// <summary>The x location of the mouse, as measured from the left of the screen.</summary>
		public float clientX;
		/// <summary>The y location of the mouse, as measured from the top of the screen.</summary>
		public float clientY;
		/// <summary>The current phase of this event.</summary>
		public int eventPhase;
		/// <summary>Modifiers such as shift. Use &x to access a particular one,
		/// where x is from e.g. EventModifierInit.MODIFIER_SHIFT_ALT_GRAPH</summary>
		public uint Modifiers;
		/// <summary>The keycode of the key pressed.</summary>
		public int keyCode;
		/// <summary>True if the mouse button or key is currently down.</summary>
		public bool heldDown;
		/// <summary>The character that has been typed.</summary>
		public char character;
		/// <summary>True if this event has been cancelled via preventDefault.</summary>
		internal bool _Cancelled;
		/// <summary>True if this has been immediately cancelled.</summary>
		internal bool _CancelImmediate;
		/// <summary>Set to true if you do not want this event to bubble any further.</summary>
		private bool _CancelBubble;
		/// <summary>True if this event bubbles or not. Doesn't bubble by default.</summary>
		public bool bubbles=false;
		/// <summary>True if this event can be cancelled (from bubbling further). All are by default.</summary>
		public bool cancelable=true;
		/// <summary>True if this was created by the UA. Use isTrusted.</summary>
		internal bool _IsTrusted;
		
		
		public Event(){
			_Timestamp=DateTime.Now;
		}
		
		public Event(string type){
			_Timestamp=DateTime.Now;
			EventType=type;
		}
		
		public Event(string type,object init){
			_Timestamp=DateTime.Now;
			EventType=type;
			Setup(init);
		}
		
		/// <summary>Reset an event so it can be reused. Doesn't affect bubbles/ trusted etc.</summary>
		public virtual void Reset(){
			_Cancelled=false;
			_CancelImmediate=false;
			_CancelBubble=false;
		}
		
		/// <summary>used to initialize the value of an event created using Document.createEvent()</summary>
		public void initEvent(string type,bool bubb,bool canc){
			
			EventType=type;
			bubbles=bubb;
			cancelable=canc;
			
		}
		
		/// <summary>Set to true if you do not want this event to bubble any further.</summary>
		public bool cancelBubble{
			get{
				return _CancelBubble;
			}
			set{
				if(cancelable){
					_CancelBubble=value;
				}
			}
		}
		/// <summary>True if this was created by the UA. Use isTrusted.</summary>
		public bool isTrusted{
			get{
				return _IsTrusted;
			}
		}
		
		internal void SetTrusted(){
			_IsTrusted=true;
			bubbles=true;
		}
		
		internal void SetTrusted(bool bubbles){
			_IsTrusted=true;
			this.bubbles=bubbles;
		}
		
		/// <summary>Sets up the init dictionary. Note that it can be null.</summary>
		public virtual void Setup(object init){}
		
		/// <summary>Stops the event bubbling to any other elements.</summary>
		public void stopPropagation(){
			
			if(cancelable){
				_CancelBubble=true;
			}
			
		}
		
		/// <summary>Stops the event bubbling to any other elements.</summary>
		public void stopImmediatePropagation(){
			
			if(cancelable){
				_CancelBubble=true;
				_CancelImmediate=true;
			}
			
		}
		
		/// <summary>The deep path of this event. (Unclear working draft spec; subject to change).
		/// http://w3c.github.io/webcomponents/spec/shadow/#widl-Event-deepPath-sequence-EventTarget</summary>
		public EventTarget[] deepPath(){
			
			if(eventPhase==NONE){
				// Not been dispatched or isn't currently being dispatched - empty set:
				return new EventTarget[0];
			}
			
			// Create the set:
			EventTarget[] set=new EventTarget[EventTarget.dispatchStackRef.Size];
			
			// Copy the dispatch stack:
			Array.Copy(EventTarget.dispatchStackRef.Stack,0,set,0,set.Length);
			
			// Ok!
			return set;
			
		}
		
		/// <summary>Stops the default event handler occuring.</summary>
		public void preventDefault(){
			
			if(cancelable){
				_Cancelled=true;
			}
			
		}
		
		/// <summary>The node that was clicked on or focused. It's a node because documents generate it too.
		/// Use htmlTarget if you're sure it's a HtmlElement of some kind.</summary>
		public EventTarget target;
		/// <summary>Current target of this event.</summary>
		public EventTarget currentTarget;
		
		/// <summary>The type of this event.</summary>
		public string type{
			get{
				
				return EventType;
				
			}
			set{
				EventType=value;
			}
		}
		
		/// <summary>The node that was clicked on or focused.</summary>
		public Node srcElement{
			get{
				return target as Node;
			}
		}
		
		/// <summary>True if the default has been prevented.</summary>
		public bool defaultPrevented{
			get{
				return _Cancelled;
			}
		}
		
		private DateTime _Timestamp;
		
		/// <summary>Current time of this event.</summary>
		public DateTime timeStamp{
			get{
				return _Timestamp;
			}
		}
		
		/// <summary>The printable character if it is one.</summary>
		public string @char{
			get{
				return ""+character;
			}
		}
		
		/// <summary>The mouse button that was pressed. See isLeftMouse and isRightMouse for clearer ways of using this value.</summary>
		public int button{
			get{
				return keyCode;
			}
		}
		
		/// <summary>Mouseup/down only. Was it the left mouse button?</summary>
		public bool isLeftMouse{
			get{
				return (button==0);
			}
		}
		
		/// <summary>Mouseup/down only. Was it the right mouse button?</summary>
		public bool isRightMouse{
			get{
				return (button==2);
			}
		}
		
		/// <summary>The document that this event has come from, if any.</summary>
		public Document document{
			get{
				
				Node node=target as Node;
				
				if(node==null){
					return null;
				}
				
				return node.document;
			}
		}
		
	}
	
	/// <summary>
	/// An event which triggers when the #hash of a location changes.
	/// </summary>
	public class HashChangeEvent : Event{
		
		public string oldURL;
		public string newURL;
		
		public HashChangeEvent():base("hashchange"){}
		public HashChangeEvent(string type,object init):base(type,init){}
		
	}
	
}