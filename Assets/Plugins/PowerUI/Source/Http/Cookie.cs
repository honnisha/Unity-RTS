using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Dom;


namespace PowerUI.Http{
	
	/// <summary>
	/// A cookie. These are created when a request calls set-cookie, or when document.cookie is used.
	/// </summary>
	
	public class Cookie{
		
		/// <summary>The equals character used for splitting cookies up.</summary>
		private static char[] Delimiter=new char[]{'='};
		/// <summary>The pattern used when toString()'ing a date.</summary>
		public const string DateTimePattern="ddd, dd MMM yyyy HH':'mm':'ss 'GMT'";
		
		/// <summary>Cookie name.</summary>
		public string Name;
		/// <summary>Cookie contents.</summary>
		public string Value;
		/// <summary>Expiry date.</summary>
		public DateTime Expiry;
		/// <summary>The full domain the cookie is available for.</summary>
		public string Domain;
		/// <summary>True if this cookie isn't available in JS.</summary>
		public bool HttpOnly;
		/// <summary>Secure cookie (HTTPS only).</summary>
		public bool Secure;
		/// <summary>Specific path.</summary>
		public string Path;
		/// <summary>Cookie version.</summary>
		public string Version;
		
		
		/// <summary>Loads a cookie from the given cookie string.</summary>
		public Cookie(string cookieString){
			
			// Split it up:
			string[] pieces=cookieString.Split(';');
			
			for(int i=0;i<pieces.Length;i++){
				
				// Trim:
				string piece=pieces[i].Trim();
				
				if(piece==""){
					continue;
				}
				
				// Split at equals:
				string[] keyValuePair=piece.Split(Delimiter,2,StringSplitOptions.None);
				
				// Get the key:
				string key=keyValuePair[0].Trim();
				string value=null;
				
				if(keyValuePair.Length>1){
					
					// Got a value:
					value=keyValuePair[1].Trim();
					
					// Strip quotes:
					if(value.Length>0 && value[0]=='"' ){
						value=value.Substring(1,value.Length-2);
					}
					
				}
				
				if(i==0){
					
					// Key is the name of the cookie.
					Name=key;
					Value=Web.UrlDecode(value);
					continue;
					
				}
				
				// Lowercase it:
				key=key.ToLower();
				
				if(key=="max-age" || key=="expires" || key=="expiry"){
					
					value=value.Replace("-"," ");
					
					// Parse the expiry date (RFC 1123):
					if(!DateTime.TryParseExact(
						value,
						DateTimePattern,
						System.Globalization.CultureInfo.InvariantCulture,
						System.Globalization.DateTimeStyles.None,
						out Expiry
					)){
						Dom.Log.Add("Warning: Attempted to set a cookie with a non-standard expiry date '"+value+"'");
					}
					
				}else if(key=="secure"){
					Secure=true;
				}else if(key=="httponly"){
					HttpOnly=true;
				}else if(key=="comment"){
					// ..Does anybody use this?
				}else if(key=="version"){
					
					Version=value;
					
				}else if(key=="domain"){
					Domain=value.ToLower();
					
					if(!Domain.StartsWith(".")){
						// Supply the dot:
						Domain="."+Domain;
					}
					
				}else if(key=="path"){
					
					Path=value;
					
				}
				
				
			}
			
		}
		
		/// <summary>Gets or creates a jar for this cookie. Returns null if a page
		/// attempted to create a cookie for a domain it doesn't own.</summary>
		public bool SafeToSet(Location forDomain){
			
			string pathDomain=forDomain.host;
			
			if(string.IsNullOrEmpty(Domain)){
				
				// Using the domain from the path.
				// Note that the matching here *is different* - it doesn't start with a dot.
				Domain=pathDomain;
				
				// Always OK.
				return true;
				
			}
			
			// Explicit domain. Supply the leading . if we don't have one:
			if(Domain.StartsWith(".")){
				
				// Chop the dot off for our direct == comparison:
				if(Domain.Substring(1) == pathDomain){
					
					// Ok!
					return true;
					
				}
				
				// Ends with otherwise:
				return pathDomain.EndsWith(Domain);
				
			}
			
			// Literal match only down here:
			return (Domain==pathDomain);
			
		}
		
		/// <summary>Single session cookie if it's expiry isn't set.</summary>
		public bool SessionOnly{
			get{
				return (Expiry==DateTime.MinValue);
			}
		}
		
		/// <summary>Has this cookie expired?</summary>
		public bool Expired{
			get{
				if(SessionOnly){
					return false;
				}
				
				if(Expiry < DateTime.UtcNow){
					return true;
				}
				
				return false;
				
			}
		}
		
		/// <summary>Sets this cookie.</summary>
		public void SafeSet(Location url){
			
			if(!SafeToSet(url)){
				// Site attempted to set a cookie for another domain. 
				return;
			}
			
			// Note: At this point, cookie.Domain is always available.
			
			// In you go!
			CookieJar jar=CookieJar.Get(Domain,true);
			jar.Add(this);
			
			// Save the domain:
			jar.Save();
			
		}
		
		/// <summary>Gets it as just name/value.</summary>
		public string ToShortString(){
			return Name+"="+Web.UrlEncode(Value)+";";
		}
		
		/// <summary>The contents of the cookie.</summary>
		public override string ToString(){
			
			string content=Name+"="+Web.UrlEncode(Value)+";";
			
			if(!SessionOnly){
				content+=" max-age="+Expiry.ToString(DateTimePattern)+";";
			}
			
			if(!string.IsNullOrEmpty(Domain)){
				content+=" domain="+Domain+";";
			}
			
			if(!string.IsNullOrEmpty(Path)){
				content+=" path="+Path+";";
			}
			
			if(!string.IsNullOrEmpty(Version)){
				content+=" path="+Version+";";
			}
			
			if(Secure){
				content+=" secure";
			}else if(HttpOnly){
				content+=" httponly";
			}
			
			return content;
			
		}
		
	}
	
}