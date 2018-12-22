//--------------------------------------
//          Kulestar Unity HTTP
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerUI;


namespace PowerUI.Http{
	
	/// <summary>
	/// Helpers for dealing with HTTP headers.
	/// </summary>
	
	public static class HttpHeaders{
		
		/// <summary>The set of restricted headers.</summary>
		private static Dictionary<string,bool> RestrictedSet;
		
		/// <summary>Sets up the restricted set.</summary>
		private static void SetupRestricted(){
			
			RestrictedSet=new Dictionary<string,bool>();
			
			// Request headers:
			Restrict("Accept-Charset");
			Restrict("Accept-Encoding");
			Restrict("Access-Control-Request-Headers");
			Restrict("Access-Control-Request-Method");
			Restrict("Connection");
			Restrict("Content-Length");
			Restrict("Cookie");
			Restrict("Cookie2");
			Restrict("Date");
			Restrict("DNT");
			Restrict("Expect");
			Restrict("Host");
			Restrict("Keep-Alive");
			Restrict("Origin");
			Restrict("Referer");
			Restrict("TE");
			Restrict("Trailer");
			Restrict("Transfer-Encoding");
			Restrict("Upgrade");
			Restrict("Via");
			
			// Response headers:
			Restrict("Set-Cookie");
			Restrict("Set-Cookie2");
			
			
		}
		
		/// <summary>Restricts the given header. It can't be used from e.g. XMLHttpRequest.</summary>
		private static void Restrict(string header){
			RestrictedSet[header.ToLower()]=true;
		}
		
		/// <summary>Checks if the given header is restricted.</summary>
		public static bool Restricted(string header){
			
			header=header.Trim().ToLower();
			
			if(header.StartsWith("proxy-") || header.StartsWith("sec-")){
				return true;
			}
			
			if(RestrictedSet==null){
				SetupRestricted();
			}
			
			return RestrictedSet.ContainsKey(header);
			
		}
		
	}
	
}