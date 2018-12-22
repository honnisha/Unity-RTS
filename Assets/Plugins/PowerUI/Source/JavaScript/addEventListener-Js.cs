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
using JavaScript;
using Jint;
using Jint.Native;
using Dom;


namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		/// <summary>Adds an event listener to this document.</summary>
		[JavaScript]
		public void addEventListener(string name, object jsMethod){
			var engine = (eventTargetDocument as HtmlDocument).JavascriptEngine.Engine;
			var thisObj = JsValue.FromObject(engine, this);
			addEventListener(name, new JsEventListener(jsMethod, thisObj, engine));
		}
		
		/// <summary>Adds an event listener to this document.</summary>
		[JavaScript]
		public void addEventListener(string name, object jsMethod, bool capture){
			var engine = (eventTargetDocument as HtmlDocument).JavascriptEngine.Engine;
			var thisObj = JsValue.FromObject(engine, this);
			addEventListener(name, new JsEventListener(jsMethod, thisObj, engine));
		}
		
	}
	
}

namespace PowerUI{
	
	/// Handler for events going to JavaScript functions.
	public class JsEventListener : EventListener{
		
		public object Listener;
		public JsValue ThisObj;
		public Jint.Engine Engine;
		
		
		public JsEventListener(object listener, JsValue thisObj, Jint.Engine engine){
			Listener=listener;
			ThisObj=thisObj;
			Engine=engine;
		}
		
		public override object Internal{
			get{
				return Listener;
			}
		}
		
		public override void handleEvent(Event e){
			var eventObj = JsValue.FromObject(Engine, e);
			var value = Listener as JsValue;
			if(value != null){
				value.Invoke(ThisObj, new JsValue[]{eventObj});
			}
			
			var func = Listener as Func<JsValue, JsValue[], JsValue>;
			if(func != null){
				func.Invoke(ThisObj, new JsValue[]{eventObj});
			}
		}
		
	}
	
}
