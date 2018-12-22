using System;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace Dom{

	/// <summary>
	/// A web API for parsing XML, HTML and SVG documents.
	/// Just a convenience wrapper over the HtmlParser.
	/// </summary>
	public partial class DOMParser{
		
		/// <summary>Parses a complete document from the given string.</summary>
		public Dom.Document parseFromString(string text,string type){
			
			// Try making the document by the given mime type:
			MLNamespace ns=MLNamespaces.GetByMime(type);
			
			// Create the document:
			Document document=(ns==null)? new Document() : ns.CreateDocument();
			
			// Parse the contents:
			if(text!=null){
				document.innerML=text;
			}
			
			return document;
			
		}
		
	}
	
}