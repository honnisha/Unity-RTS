using System;
using System.Collections;

namespace Dom{
	
	/// <summary>
	/// A Language document is used when loading localization files.
	/// </summary>
	
	public class LangDocument : Document{
		
		/// <summary>Cached reference for the Language namespace.</summary>
		private static MLNamespace _LangNamespace;
		
		/// <summary>The XML namespace for Languages.</summary>
		public static MLNamespace LangNamespace{
			get{
				if(_LangNamespace==null){
					
					// Setup the namespace (Doesn't request the URL; see XML namespaces for more info):
					_LangNamespace=Dom.MLNamespaces.Get("http://translate.kulestar.com/namespace/","lang","text/lang");
					
				}
				
				return _LangNamespace;
			}
		}
		
		/// <summary>The current group being loaded.</summary>
		public LanguageGroup Group;
		
		
		public LangDocument(){
			
			// Apply namespace:
			Namespace=LangNamespace;
			
		}
		
	}
	
}