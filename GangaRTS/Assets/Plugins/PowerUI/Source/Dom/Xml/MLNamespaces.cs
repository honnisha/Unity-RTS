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
	/// Namespaces allow multiple XML elements using the same tag name to be present in a single document.
	/// https://www.w3.org/TR/2006/REC-xml-names-20060816/
	/// </summary>
	
	public static class MLNamespaces{
		
		/// <summary>The available namespaces. Maps their unique name (typically a URI) to the NS instance.</summary>
		public static Dictionary<string,MLNamespace> All=new Dictionary<string,MLNamespace>();
		/// <summary>The available namespaces. Maps prefixes (e.g. 'svg') to the NS instance.
		/// This technically shouldn't be global, however, prefixes should always be the same.</summary>
		public static Dictionary<string,MLNamespace> Prefixes=new Dictionary<string,MLNamespace>();
		
		/// <summary>Gets a namespace by its mime type. Null if not found.</summary>
		public static MLNamespace GetByMime(string type){
			
			if(type==null){
				return null;
			}
			
			type=type.Trim().ToLower();
			
			foreach(KeyValuePair<string,MLNamespace> kvp in All){
				
				if(kvp.Value.MimeType==type){
					return kvp.Value;
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Gets or creates a namespace.</summary>
		public static MLNamespace Get(string name,string prefix,string mime){
		
			MLNamespace ns;
			if(!All.TryGetValue(name,out ns)){
				
				ns=new MLNamespace(name,prefix,mime);
				Prefixes[prefix]=ns;
				All[name]=ns;
				
			}
			
			return ns;
			
		}
		
		/// <summary>Gets a namespace by its name (usually a URL).</summary>
		public static MLNamespace Get(string name){
		
			MLNamespace ns;
			All.TryGetValue(name,out ns);
			return ns;
			
		}
		
		/// <summary>Gets a namespace by prefix.</summary>
		public static MLNamespace GetPrefix(string name){
		
			MLNamespace ns;
			Prefixes.TryGetValue(name,out ns);
			return ns;
			
		}
		
	}
	
}