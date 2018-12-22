//--------------------------------------
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Used when diverting a group. E.g. &items.1.name; - the group 'items' can be diverted so this
	/// variable can be loaded in a custom way.
	/// </summary>
	public delegate void GroupResolveEvent(string variableName,LanguageTextEvent onResolved);
	
	
	/// <summary>
	/// This set represents a set of variables in text, denoted by &varName;
	/// Implemented at e.g. UI.Variables/Speech.Variables.
	/// The event OnFind can be used to resolve a variable not found in either custom variables or the language (for localization) set.
	/// Nested variables (variables in a variables text) are evaluated at the point of replacement, not load.
	/// </summary>
	
	public class FullVariableSet{
		
		/// <summary>The custom set of variables added and edited at runtime.</summary>
		public VariableSet Custom;
		/// <summary>The final fallback. Called when a variable hasn't been found anywhere else.</summary>
		public event GroupResolveEvent OnFind;
		/// <summary>A callback called when a custom variable in this set is changed.</summary>
		public event LanguageTextEvent OnChange;
		/// <summary>All group diversions.</summary>
		public Dictionary<string,GroupResolveEvent> Diverts;
		
		
		/// <summary>Creates a new complete variable set.</summary>
		public FullVariableSet(){
			Custom=new VariableSet();
		}
		
		/// <summary>Diverts the named group and processes it with the given handler.</summary>
		public void Divert(string groupName,GroupResolveEvent handler){
			
			if(Diverts==null){
				Diverts=new Dictionary<string,GroupResolveEvent>();
			}
			
			Diverts[groupName]=handler;
			
		}
		
		/// <summary>Resolves a variable value, calling the given function when it's done.</summary>
		public void GetValue(string variableName,Document document,LanguageTextEvent onResolved){
			
			if(Custom!=null){
				// Custom variables take top priority - is it overriden?
				string value=Custom.GetValue(variableName);
				
				if(value!=null){
					onResolved(value);
					return;
				}
				
			}
			
			// Are we loading from a group?
			int dotIndex=variableName.IndexOf('.');
			string groupName="";
			
			if(dotIndex!=-1){
				
				// We're loading from a specific group.
				
				// Get the name up to (but not including) the first dot:
				groupName=variableName.Substring(0,dotIndex);
				
				// Is it diverted? E.g. &items.1.name; could be handled in a custom way 
				// rather than having a huge group of all items.
				
				GroupResolveEvent handler;
				if(Diverts!=null && Diverts.TryGetValue(groupName,out handler)){
					
					// Chop off the rest of the variable name:
					variableName=variableName.Substring(dotIndex+1);
					
					// It's diverted! Call the handler:
					handler(variableName,onResolved);
					
					return;
					
				}
				
				// Not diverted. It might be a sub-group (&a.b.c;) so check for that:
				int lastIndex=variableName.LastIndexOf('.');
				
				if(lastIndex!=dotIndex){
					
					// Sub-group; get its name again:
					groupName=variableName.Substring(0,lastIndex);
					
				}
				
				// Chop off the rest of the variable name:
				variableName=variableName.Substring(lastIndex+1);
				
			}
			
			// Load a var from the language set now:
			document.LoadLanguageVariable(groupName,variableName,delegate(string variableValue){
				
				if(variableValue==null){
					
					// Nope! Not found in either custom or language.
					
					if(OnFind!=null){
						OnFind(variableName,onResolved);
						return;
					}
					
				}
				
				// Ok!
				onResolved(variableValue);
				
			});
			
		}
		
		/// <summary>Sets a custom variable with the given name and value. This will take top priority.</summary>
		/// <param name="code">The name of the variable to set.</param>
		/// <param name="value">The value to set it to.</param>
		public void SetValue(string code,string value){
			if(Custom==null){
				Custom=new VariableSet();
			}
			
			Custom[code]=value;
			
			if(OnChange!=null){
				OnChange(code);
			}
		}
		
		/// <summary>Gets or sets custom variables.</summary>
		/// <param name="code">The name of the variable to get/set.</param>
		public string this[string code]{
			get{
				if(Custom==null){
					return null;
				}
				
				return Custom[code];
			}
			set{
				SetValue(code,value);
			}
		}
		
	}
	
}