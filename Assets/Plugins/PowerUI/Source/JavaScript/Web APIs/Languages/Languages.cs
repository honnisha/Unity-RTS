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
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// The non-standard document.languages API. It provides localization information.
	/// </summary>
	public class Languages{
		
		/// <summary>The raw language set which is loaded from this documents meta tag.</summary>
		internal Language[] all_;
		
		/// <summary>A loader which pulls localised text.
		/// This loader is used whenever the default URI resources://Languages/ is in use.</summary>
		private static PowerUILanguageLoader globalLoader_;
		
		/// <summary>Gets a loader which tells PowerUI where to look for
		/// language specific UI text (&variable; content).</summary>
		internal static PowerUILanguageLoader globalLoader{
			get{
				if(globalLoader_==null){
					globalLoader_=new PowerUILanguageLoader("resources://Languages/");
				}
				
				return globalLoader_;
			}
		}
		
		public static readonly string DEFAULT="/Languages/";
		
		private string location_;
		/// <summary>The document that this languages API is for.</summary>
		public HtmlDocument document;
		/// <summary>Cached current language.</summary>
		internal Language current_;
		/// <summary>A loader which pulls localised text.</summary>
		private PowerUILanguageLoader loader_;
		
		/// <summary>Gets a loader which tells PowerUI where to look for
		/// language specific UI text (&variable; content).</summary>
		internal PowerUILanguageLoader loader{
			get{
				if(loader_==null){
					
					// Is the location the same as our global one?
					string loc=location;
					
					if(loc==DEFAULT && document.location.protocol=="resources:"){
						
						// Use global:
						loader_=globalLoader;
						
					}else{
						
						// Create one now:
						loader_=new PowerUILanguageLoader(new Location(loc,document.location).absolute);
						
					}
				}
				
				return loader_;
			}
		}
		
		/// <summary>Get a language by its code.</summary>
		public Language get(string code){
			
			return loader.Get(code);
			
		}
		
		/// <summary>The currently active language for this document. Note that it's never null.</summary>
		public Language current{
			get{
				// Get the current language:
				if(current_==null){
					current_=get(Text.Language);
				}
				
				return current_;
			}
		}
		
		public Languages(HtmlDocument doc){
			document=doc;
		}
		
		/// <summary>Set this either with a <meta name='languages' src='x'/> tag, or use this API directly.
		/// If it's not been set, it will return the default value which is the domain relative URI "/Languages/".</summary>
		public string location{
			get{
				if(location_==null){
					return DEFAULT;
				}
				
				return location_;
			}
			set{
				
				// Clear caches:
				loader_=null;
				current_=null;
				
				// Set location:
				location_=value;
				
			}
		}
		
		/// <summary>Gets all languages available to this document. Originates from your meta name='languages' tag
		/// or a parent document if this is in an iframe.</summary>
		public Language[] all{
			get{
				
				if(all_==null){
					
					// Try a parent document.
					Window window=document.window;
					
					while(window.parent!=null){
						
						Languages langs=window.document.languages_;
						
						if(langs!=null && langs.all_!=null){
							
							// Inherited!
							return all_;
							
						}
						
						// Let's go up again:
						window=window.parent;
						
					}
					
				}
				
				return all_;
			}
		}
		
	}
	
	public partial class HtmlDocument{
		
		/// <summary>The document.languages API. Use languages instead.</summary>
		internal Languages languages_;
		
		/// <summary>Called when the language has been changed.</summary>
		internal void LanguageChanged(){
			
			// Trigger a language change event:
			window.dispatchEvent(new Dom.Event("languagechange"));
			
			if(languages_!=null){
				
				// Localization in use - Clear cached current:
				languages_.current_=null;
				
			}
			
		}
		
		/// <summary>The document.languages API.</summary>
		public Languages languages{
			get{
				if(languages_==null){
					languages_=new Languages(this);
				}
				
				return languages_;
			}
		}
		
		/// <summary>Loads a variable value from the language set. Note that this occurs after checking 'diverted' variables
		/// and after custom ones too - it only looks for languages. This is essentially handled by the document.languages API.</summary>
		public override void LoadLanguageVariable(string groupName,string variableName,LanguageTextEvent onResolved){
			
			// Get the group:
			languages.current.getGroup(groupName,delegate(LanguageEvent e){
				
				// Load the variable and run the event:
				string variableValue=null;
				
				if(e.group!=null){
					variableValue=e.group[variableName];
				}
				
				// Run the callback:
				onResolved(variableValue);
				
			});
			
		}
		
	}
	
}