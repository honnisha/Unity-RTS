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
using System.Reflection;
using Dom;
using Jint.Runtime.Interop;
using Jint.Native;


namespace PowerUI{
	
	/// <summary>
	/// The default script handler for text/javascript-x.
	/// </summary>
	
	public class JavaScriptEngine : PowerUI.ScriptEngine{
		
		/// <summary>The meta types that your engine will handle. E.g. "text/javascript".</summary>
		public override string[] GetTypes(){
			return new string[]{"text/javascript"};
		}
		
		/// <summary>An instance of the ScriptEngine on this page.</summary>
		public Jint.Engine Engine;
		
		public JavaScriptEngine(){}
		
		public JavaScriptEngine(bool safeHost,HtmlDocument doc,object window){
			
			Engine = new Jint.Engine(cfg => {
				cfg.AllowClr()
					#if NETFX_CORE
					.AllowClr(typeof(GameObject).GetTypeInfo().Assembly);
					#else
					.AllowClr(typeof(GameObject).Assembly);
					#endif
			});
			
			Engine.SetValue("document", doc)
				.SetValue("Promise", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Promise)))
				.SetValue("ArrayBuffer", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.ArrayBuffer)))
				.SetValue("DataView", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.DataView)))
				.SetValue("Float32Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Float32Array)))
				.SetValue("Float64Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Float64Array)))
				.SetValue("Int8Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Int8Array)))
				.SetValue("Int16Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Int16Array)))
				.SetValue("Int32Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Int32Array)))
				.SetValue("Uint8ClampedArray", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Uint8ClampedArray)))
				.SetValue("Uint8Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Uint8Array)))
				.SetValue("Uint16Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Uint16Array)))
				.SetValue("Uint32Array", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.Uint32Array)))
				.SetValue("XMLHttpRequest", TypeReference.CreateTypeReference(Engine, typeof(PowerUI.XMLHttpRequest)))
				.SetValue("Module", TypeReference.CreateTypeReference(Engine, typeof(WebAssembly.Module)))
				.SetValue("console", new JavaScript.console())
				.SetValue("window", Engine.Global)
				.SetValue("PowerUI", new NamespaceReference(Engine, "PowerUI"));
			
			var coreWindow = (window as PowerUI.Window);
			coreWindow.JsWindow = Engine.Global;
			
			if(coreWindow!=null){
				Engine.SetValue("location", coreWindow.location)
					.SetValue("innerWidth", coreWindow.innerWidth)
					.SetValue("innerHeight", coreWindow.innerHeight)
					.SetValue("atob", new Func<string, string>((string obj) => { return coreWindow.atob(obj); }))
					.SetValue("btoa", new Func<string, string>((string obj) => { return coreWindow.btoa(obj); }))
					.SetValue("addEventListener", new Action<string, object>((string evt, object method) => coreWindow.addEventListener(evt, method)))
					.SetValue("removeEventListener", new Action<string, object>((string evt, object method) => coreWindow.removeEventListener(evt, method)))
					.SetValue("dispatchEvent", new Func<UIEvent, bool>((UIEvent e) => {return coreWindow.dispatchEvent(e);}))
					.SetValue("clearInterval", new Action<UITimer>((UITimer obj) => coreWindow.clearInterval(obj)))
					.SetValue("alert", new Action<object>((object obj) => coreWindow.alert(obj)))
					.SetValue("prompt", new Func<object, string>((object obj) => {return coreWindow.prompt(obj);}))
					.SetValue("confirm", new Func<object, bool>((object obj) => {return coreWindow.confirm(obj);}))
					.SetValue("escapeHTML", new Func<string, string>((string obj) => {return coreWindow.escapeHTML(obj);}))
					.SetValue("setInterval", new DelegateWrapper(Engine, new Func<JsValue, object, UITimer>((JsValue method, object time) => {return coreWindow.setInterval(method, time);})))
					.SetValue("setTimeout", new DelegateWrapper(Engine, new Func<JsValue, object, UITimer>((JsValue method, object time) => {return coreWindow.setTimeout(method, time);})));
			}
			
			Document=doc;
			
		}
		
		/// <summary>Gets or sets script variable values.</summary>
		/// <param name="index">The name of the variable.</param>
		/// <returns>The variable value.</returns>
		public override object this[string global]{
			get{
				if(Engine==null){
					return null;
				}
				
				return Engine.GetValue(global);
			}
			set{
				if(Engine==null){
					return;
				}
				
				Engine.SetValue(global,value);
			}
		}
		
		public override PowerUI.ScriptEngine Instance(Document document){
			
			HtmlDocument doc=document as HtmlDocument;
			bool safeHost=true;
			
			if(doc!=null){
				
				Location location=doc.location;
				Window window=doc.window;
				
				// Iframe security check - can code from this domain run at all?
				// We have the Nitro runtime so it could run unwanted code.
				if(window.parent!=null && location!=null && !location.fullAccess){
					
					// It's an iframe to some unsafe location.
					safeHost = false;
					
				}
				
			}
			
			return new JavaScriptEngine(safeHost,doc,doc.window);
		}
		
		private readonly JsValue[] EmptyArgs = new JsValue[0];
		
		public JsValue Invoke(object method, JsValue thisObj, JsValue[] args){
			// Method could be a Func etc.
			var value = method as JsValue;
			if(value != null){
				return value.Invoke(thisObj, args == null ? EmptyArgs : args);
			}
			var func = method as Func<JsValue, JsValue[], JsValue>;
			if(func != null){
				return func.Invoke(thisObj,args == null ? EmptyArgs :  args);
			}
			return Jint.Native.Undefined.Instance;
		}
		
		/// <summary>Invokes a particular method by its name.</summary>
		public JsValue Run(string method, object thisObj, params object[] args){
			var mappedThis = JsValue.FromObject(Engine, thisObj);
			var mappedArgs = Map(args);
			return Engine.GetValue(method).Invoke(mappedThis, mappedArgs);
		}
		
		/// <summary>Invokes a particular method.</summary>
		public JsValue Run(JsValue method, object thisObj, params object[] args){
			var mappedThis = JsValue.FromObject(Engine, thisObj);
			var mappedArgs = Map(args);
			return method.Invoke(mappedThis, mappedArgs);
		}
		
		/// <summary>Maps a set of random objects to an array of JS values.</summary>
		public JsValue[] Map(object[] args){
			if(args == null){
				return EmptyArgs;
			}
			JsValue[] mappedArgs;
			if(args == null){
				mappedArgs = null;
			}else{
				mappedArgs = new JsValue[args.Length];
				for(var i=0;i<args.Length;i++){
					mappedArgs[i] = JsValue.FromObject(Engine, args[i]);
				}
			}
			return mappedArgs;
		}
		
		/// <summary>Maps a single object to a JS value.</summary>
		public JsValue Map(object arg){
			return JsValue.FromObject(Engine, arg);
		}
		
		public override object Compile(string source){
			
			try{
				// Trigger an event to say the engine is about to start:
				Dom.Event e=new Dom.Event("scriptenginestart");
				htmlDocument.dispatchEvent(e);
				
				// Run it now:
				Engine.Execute(source);
				return Engine.GetCompletionValue();
			}catch(Jint.Runtime.JavaScriptException je){
				
				Dom.Log.Add("JavaScript exception thrown (line "+je.LineNumber+"): "+je.ToString());
				
			}catch(Exception e){
				
				string scriptLocation=htmlDocument.ScriptLocation;
				
				if(string.IsNullOrEmpty(scriptLocation)){
					// Use document.basepath instead:
					scriptLocation=Document.basepath.ToString();
				}
				
				if(!string.IsNullOrEmpty(scriptLocation)){
					scriptLocation=" (At "+scriptLocation+")";
				}
				
				Dom.Log.Add("JavaScript compile error "+scriptLocation+": "+e);
			}
			
			return Jint.Native.Undefined.Instance;
		}
		
	}
	
}