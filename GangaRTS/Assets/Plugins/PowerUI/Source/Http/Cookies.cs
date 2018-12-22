// With thanks to UnityCookies.cs by Sam McGrath
	
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Dom;


namespace PowerUI.Http{
	
	public static class Cookies{
		
		
		/// <summary>Gets the complete cookie string for the given data package as well as the HTTP status.</summary>
		public static void Handle(ContentPackage package){
			
			// Foreach header in "set-cookie":
			List<string> setCookie=package.responseHeaders.GetAll("set-cookie");
			
			if(setCookie==null){
				return;
			}
			
			// Get the url:
			Location url=package.location;
			
			for(int i=0;i<setCookie.Count;i++){
				
				// Load the cookie now:
				Cookie cookie=new Cookie(setCookie[i]);
				
				// Try adding it, with a safety check:
				cookie.SafeSet(url);
				
			}
			
		}
		
	}

}