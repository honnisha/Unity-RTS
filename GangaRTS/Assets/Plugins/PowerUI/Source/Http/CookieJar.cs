using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Dom;


namespace PowerUI.Http{
	
	/// <summary>
	/// A jar to hold any assigned cookies.
	/// </summary>
	
	public class CookieJar : DomainEntry{
		
		/// <summary>Gets the cookie jar for the given domain. Recursively checks parent domains.</summary>
		public static CookieJar Get(string domain){
			return Get(domain,false);
		}
		
		/// <summary>Gets the cookie jar for the given domain. Recursively checks parent domains.
		/// Optionally creates one if it wasn't found.</summary>
		public static CookieJar Get(string domain, bool create){
			
			// If the domain starts with a '.' then ignore it:
			if(domain.StartsWith(".")){
				domain=domain.Substring(1);
			}
			
			DomainData domainData=Cache.GetDomain(domain);
			CookieJar jar=domainData.Cookies;
			
			if(jar!=null){
				return jar;
			}
			
			// jar not found!
			
			if(create){
				// Create it here:
				jar=new CookieJar(domainData);
				domainData.Cookies=jar;
				return jar;
			}
			
			// Try parent domains:
			int indexOf=domain.IndexOf('.');
			
			while(indexOf!=-1){
				
				// Chop off the first part:
				domain=domain.Substring(indexOf+1);
				
				// Get the next index of:
				indexOf=domain.IndexOf('.');
				
				if(indexOf!=-1){
					
					// We still have a dot in the domain - try again:
					domainData=Cache.GetDomain(domain);
					jar=domainData.Cookies;
					
					if(jar!=null){
						
						return jar;
					}
					
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>The cookies.</summary>
		public Dictionary<string,Cookie> CookieSet=new Dictionary<string,Cookie>();
		
		
		public CookieJar(DomainData domain):base(domain){
		}
		
		/// <summary>Add or replace the given cookie in the jar.</summary>
		public void Add(Cookie cookie){
			
			if(cookie.Expired){
				CookieSet.Remove(cookie.Name);
				return;
			}
			
			CookieSet[cookie.Name]=cookie;
			
		}
		
		/// <summary>Builds a header-friendly string.</summary>
		public string GetCookieHeader(Location forPath){
			
			var str = new StringBuilder();
			
			foreach( KeyValuePair<string,Cookie> kvp in CookieSet ){
				
				// Get the cookie:
				Cookie cookie=kvp.Value;
				
				// Match?
				// Either starts with a '.' or it's an exact match.
				// This blocks a.site.com from receiving a cookie explicitly for "site.com" only.
				if(cookie.Domain[0]!='.' && cookie.Domain!=forPath.host){
					// Nope!
					continue;
				}
				
				if ( str.Length > 0 ){
					str.Append( "; " );
				}
				
				str.Append(cookie.ToShortString());
				
			}
			
			return str.ToString();
			
		}
		
		/// <summary>Empties the jar.</summary>
		public void Empty(){
			CookieSet.Clear();
		}
		
		public override bool IsEmpty{
			get{
				return CookieSet==null || CookieSet.Count==0;
			}
		}
		
		public override string JsonIndex{
			get{
				return "cookies";
			}
		}
		
		public override void LoadFromJson(Json.JSObject obj){
			
			// It should be an array:
			Json.JSIndexedArray arr=obj as Json.JSIndexedArray;
			
			if(arr==null){
				return;
			}
			
			// For each one..
			for(int i=0;i<arr.length;i++){
				
				// Add it as a cookie:
				Add(new Cookie(arr[i].ToString()));
				
			}
			
		}
		
		public override Json.JSObject ToJson(){
			
			// Create array:
			Json.JSIndexedArray arr=new Json.JSIndexedArray();
			
			// Add each cookie:
			foreach( KeyValuePair<string,Cookie> kvp in CookieSet ){
				
				// Get the cookie:
				Cookie cookie=kvp.Value;
				
				if(cookie.SessionOnly){
					continue;
				}
				
				arr.push(new Json.JSValue(cookie.ToString()));
				
			}
			
			return arr;
			
		}
		
	}
	
}