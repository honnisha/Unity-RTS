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
	/// Called when a group of variables has been loaded.
	/// </summary>
	public delegate void LanguageEventDelegate(LanguageEvent e);
	public delegate void LanguageTextEvent(string result);
	
	public class LanguageEvent:Event{
		
		/// <summary>The loader.</summary>
		public LanguageLoader loader;
		/// <summary>A particular language if there is one.</summary>
		public Language language;
		/// <summary>A particular group if there is one.</summary>
		public LanguageGroup group;
		
		
		public LanguageEvent(string type):base(type){}
		
	}
	
	/// <summary>Standard information about a particular language.</summary>
	public partial class LanguageInfo{
		
		/// <summary>All available language infos. You must use a language loader to setup this
		/// as it can internally use any available file protocol.</summary>
		public static Dictionary<string,LanguageInfo> All;
		
		/// <summary>Gets meta by language code.</summary>
		public static LanguageInfo Get(string code){
			
			if(All==null){
				return null;
			}
			
			LanguageInfo result;
			All.TryGetValue(code,out result);
			return result;
		}
		
		public static void Load(string xml){
			
			All=new Dictionary<string,LanguageInfo>();
			
			if(xml!=null){
				
				// Create a LangDocument:
				LangDocument doc=new LangDocument();
				
				// Parse it using a HTML parser:
				doc.LoadHtml(xml,HtmlTreeMode.InBody);
				
			}
			
		}
		
		/// <summary>Either "ltr" or "rtl".</summary>
		public string dir;
		/// <summary>Lowercase ISO 639-1. E.g. "en".</summary>
		public string code;
		/// <summary>The language name in the Editor language (English).</summary>
		public string name;
		/// <summary>The local language name.</summary>
		public string localName;
		
		/// <summary>True if this language goes from right to left (e.g. arabic).</summary>
		public bool leftwards{
			get{
				return dir=="rtl";
			}
		}
		
	}
	
	public class Language{
		
		/// <summary>Raw language info.</summary>
		internal LanguageInfo info_;
		
		/// <summary>Lowercase ISO 639-1. E.g. "en".</summary>
		public string code{
			get{
				return info_.code;
			}
		}
		
		/// <summary>The language name in the Editor language (English).</summary>
		public string name{
			get{
				return info_.name;
			}
		}
		
		/// <summary>The local language name.</summary>
		public string localName{
			get{
				return info_.localName;
			}
		}
		
		/// <summary>The language name in the Editor language (English).</summary>
		public string dir{
			get{
				return info_.dir;
			}
		}
		
		/// <summary>True if this language goes from right to left (e.g. arabic).</summary>
		public bool leftwards{
			get{
				return info_.leftwards;
			}
		}
		
		/// <summary>The loader that has loaded this language.</summary>
		public LanguageLoader Loader;
		/// <summary>All available groups. A group with an empty string name is the default one.</summary>
		public Dictionary<string,LanguageGroup> Groups=new Dictionary<string,LanguageGroup>();
		
		
		public Language(string code,LanguageLoader loader){
			Loader=loader;
			info_=LanguageInfo.Get(code);
			
			if(info_==null){
				// Create an empty one:
				info_=new LanguageInfo();
				info_.code=code;
				LanguageInfo.All[code]=info_;
				Log.Add("Warning: Language was not found ("+code+").");
			}
			
		}
		
		/// <summary>Creates and adds a new language group using the given xml.</summary>
		public LanguageGroup addGroup(string groupName,string xml){
			
			// Create and add it now:
			LanguageGroup g=new LanguageGroup(groupName,this);
			Groups[groupName]=g;
			g.innerXML=xml;
			
			return g;
		}
		
		/// <summary>Loads a standard group by name.</summary>
		/// <param name="groupName">The name of the group.</param>
		public void getGroup(string groupName,LanguageEventDelegate groupReady){
			
			// Already available?
			LanguageGroup group;
			if(Groups.TryGetValue(groupName,out group)){
				
				// Create event:
				LanguageEvent e=Loader.CreateEvent("languagegroupready");
				e.language=this;
				e.group=group;
				
				// Callback:
				groupReady(e);
				
			}else{
				
				// Must now load its contents.
				string path=Loader.Path;
				
				if(groupName!=""){
					path+=groupName.Replace('.','/')+"/";
				}
				
				Loader.LoadFile(path+code+".xml",delegate(string fileText){
					
					// Create event:
					LanguageEvent e=Loader.CreateEvent("languagegroupready");
					e.language=this;
					
					if(fileText!=null){
						e.group=addGroup(groupName,fileText);
					}
					
					// Callback:
					groupReady(e);
					
				});
				
			}
			
		}
		
	}
	
	/// <summary>
	/// A particular group of variables within a language.
	/// E.g. Used by &GroupName.VariableName; with a LanguageGroup instance per language.
	/// </summary>
	public class LanguageGroup : VariableSet{
		
		/// <summary>The group name. This is the empty string for the 'default' group.</summary>
		public string Name;
		/// <summary>The language this is a group in.</summary>
		public Language Language;
		
		
		public LanguageGroup(string name,Language language){
			Name=name;
			Language=language;
		}
		
		public LanguageGroup(string xml){
			Name="";
			innerXML=xml;
		}
		
		/// <summary>The XML of this group.</summary>
		public string innerXML{
			set{
				
				// Create a LangDocument:
				LangDocument doc=new LangDocument();
				doc.Group=this;
				
				// Parse it using a HTML parser:
				doc.LoadHtml(value,HtmlTreeMode.InBody);
				
			}
		}
		
	}
	
	/// <summary>
	/// Override this class to provide methods for loading language files.
	/// They may be delivered in many different ways because of this.
	/// </summary>

	public class LanguageLoader{
	
		/// <summary>The path to the location of the languages.</summary>
		public string Path;
		/// <summary>The set of all languages if they've all been loaded. Indexed by lowercase language code.</summary>
		public Dictionary<string,Language> Languages;
		
		
		/// <summary>Creates a new loader for the given path.</summary>
		public LanguageLoader(string path){
			Path=path;
		}
		
		/// <summary>Loads a standard group by name.</summary>
		/// <param name="code">The language code.</param>
		/// <param name="groupName">The name of the group.</param>
		public void GetGroup(string code,string groupName,LanguageEventDelegate groupReady){
			
			// Get the group:
			Get(code).getGroup(groupName,groupReady);
			
		}
		
		/// <summary>Gets the language with the given code.</summary>
		/// <param name="code">The language code to look for.</param>
		/// <returns>Creates the language if it doesn't exist. Never null.</returns>
		public Language Get(string code){
			
			// Tidy up the code:
			code=code.Trim().ToLower();
			
			if(Languages==null){
				Languages=new Dictionary<string,Language>();
			}	
			
			Language result;
			if(!Languages.TryGetValue(code,out result)){
				
				// Create and add the language container:
				result=new Language(code,this);
				Languages[code]=result;
				
			}
			
			// Ok:
			return result;
			
		}
		
		/// <summary>Loads an XML file at the given path.</summary>
		/// <param name="onFileAvailable">A callback that runs when the file has been downloaded.</param>
		public virtual void LoadFile(string path,LanguageTextEvent onFileAvailable){
			
			throw new NotImplementedException("Use a PowerUILanguageLoader.");
			
		}
		
		/// <summary>
		/// Creates a language event for this loader.
		/// </summary>
		internal LanguageEvent CreateEvent(string type){
			LanguageEvent e=new LanguageEvent(type);
			e.loader=this;
			return e;
		} 
		
	}
	
}