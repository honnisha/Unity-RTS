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
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Represents a path to a file with a protocol.
	/// e.g. http://www.site.com/aFile.png
	/// The path may also be relative to some other path (aFile.png relative to http://www.site.com).
	/// </summary>
	
	public partial class Location : ExpandableObject{
		
		/// <summary>The default location that relative paths are relative to.</summary>
		private static Location DefaultRelativeTo = new Location("resources://", null);
		
		/// <summary>The raw Url/Path</summary>
		public string RawUrl;
		/// <summary>The protocol to use when accessing this file.</summary>
		public string Protocol;
		/// <summary>The pieces of the path split up by /.</summary>
		public string[] Segments;
		/// <summary>The raw Url that this path is relative to.</summary>
		public string RelativeTo;
		/// <summary>The hash section of the URL, if there is one.
		/// Separated here because it should not be sent to the server.</summary>
		public string Hash;
		/// <summary>The parent document that this location is used by, if any.</summary> 
		public Document document;
		/// <summary>The ?queryString if there is one.</summary>
		public string Query;
		/// <summary>Auth string alongside the hostname. 'user:pass'</summary>
		public string authorization;
		
		
		/// <summary>Creates a new filepath from the given url.</summary>
		/// <param name="url">The url of the file.</param>
		public Location(string url){
			SetPath(url,null);
		}
		
		/// <summary>Creates a new filepath from the given url.</summary>
		/// <param name="url">The url of the file.</param>
		/// <param name="relativeTo">The path that url is relative to.</param>
		public Location(string url,Location relativeTo){
			SetPath(url,relativeTo);
		}
		
		/// <summary>Applies the given url to this filepath object.</summary>
		/// <param name="url">The url of the file.</param>
		/// <param name="relativeTo">The path that url is relative to, or null if none.</param>
		private void SetPath(string url,Location relativeTo){
			
			if(relativeTo == null){
				relativeTo = DefaultRelativeTo;
			}
			
			if(string.IsNullOrEmpty(url)){
				url="";
				RawUrl="";
				
				// Apply protocol:
				Protocol=relativeTo.Protocol;
				
				// Equal to relativeTo:
				Segments=new string[relativeTo.Segments.Length];
				
				// Copy segments over:
				System.Array.Copy(
					relativeTo.Segments,0,
					Segments,0,
					Segments.Length
				);
				
				return;
			}
			
			RawUrl=url;
			int trailIndex;
			int colonIndex;
			
			// Find the protocol - we want to avoid over-processing data URLs.
			if(url.StartsWith("//")){
				
				// Network-path reference. (Aka protocol-relative URL).
				
				// Swap out any &amp;'s for &:
				url=url.Replace("&amp;","&");
				
				// Apply protocol:
				Protocol=relativeTo.Protocol;
				
				// Chop off the double slash and any spaces:
				url=url.Substring(2).Trim();
				
				// Not relative.
				colonIndex=0;
				
			}else{
				
				// Get colon index:
				colonIndex=url.IndexOf(':');
				
				if(colonIndex==-1){
					
					// Relative URL
					Protocol=relativeTo.Protocol;
					
					// Swap out any &amp;'s for &:
					url=url.Replace("&amp;","&");
					
				}else{
					
					// Get the protocol name:
					Protocol=url.Substring(0,colonIndex).Trim().ToLower();
					
					if(Protocol=="data"){
						
						// Optimization - Special case here. There won't be a hash or query string.
						// Find the comma:
						trailIndex=url.IndexOf(',');
						string header;
						string data;
						
						if(trailIndex==-1){
							header="";
							data=url;
						}else{
							
							// header:
							header=url.Substring(colonIndex+1,trailIndex);
							
							// Actual data:
							data=url.Substring(trailIndex+1);
						}
						
						// Apply to segments:
						Segments=new string[]{header,data};
						
						return;
						
					}else{
						
						// Swap out any &amp;'s for &:
						url=url.Replace("&amp;","&");
						
					}
					
					if(colonIndex+2<url.Length && url[colonIndex+1]=='/' && url[colonIndex+2]=='/'){
						
						// Chop off the protocol:
						url=url.Substring(colonIndex+3);
						
					}else{
						
						// Chop off the protocol:
						url=url.Substring(colonIndex+1);
						
					}
					
				}
				
			}
			
			// Chop off the # at the end of the URL if there is one:
			trailIndex=url.LastIndexOf('#');
			
			// Did we have one?
			if(trailIndex!=-1){
				
				// Yes - got a fragment:
				Hash=url.Substring(trailIndex+1);
				
				// And set the url to parse to being the rest of it:
				url=url.Substring(0,trailIndex);
				
			}
			
			// Query string next:
			trailIndex=url.LastIndexOf('?');
			
			// Did we have one?
			if(trailIndex!=-1){
				
				// Yes - grab it:
				Query=url.Substring(trailIndex+1);
				
				// And set the url to parse to being the rest of it:
				url=url.Substring(0,trailIndex);
				
			}
			
			// Remove trailing slashes:
			if(url.Length>1 && url[url.Length-1]=='/'){
				url=url.Substring(0,url.Length-1);
			}
			
			if(colonIndex==-1 && url.Trim().Length==0){
				
				// This occurs when using e.g. href='#...'; we're relative to all of it:
				Segments = (string[])relativeTo.Segments.Clone();
				
			}else if(colonIndex == -1){
				
				// Relative URL
				
				// Split now:
				Segments=url.Split('/');
				
				// Number of relative segments:
				int rsCount;
				int urlStart=0;
				
				if(Segments[0]==""){
					
					// '/test'
					// url=url.Substring(1);
					
					// Only relative to the hostname (if it has one).
					if(relativeTo.hasHostname){
						rsCount=1;
					}else{
						rsCount=0;
					}
					
					// Skip the blank segment:
					urlStart=1;
					
				}else{
					
					// Relative to the whole thing minus however many ..'s there are and minus a filename.
					rsCount=relativeTo.Segments.Length;
					
					int minCount=relativeTo.hasHostname?1:0;
					
					if(rsCount==0){
					}else if(relativeTo.Segments[rsCount-1].IndexOf('.')!=-1){
						// It's a filename (or doesn't exist) - chop it off.
						rsCount--;
					}
					
					// Do we have enough segments available?
					if(rsCount>=minCount){
						
						// 'test', '../test', '../../test' etc
						for(int i=0;i<Segments.Length;i++){
							
							if(Segments[i]==".."){
								
								rsCount--;
								urlStart++;
								
							}else{
								
								break;
								
							}
							
						}
						
						if(rsCount<minCount){
							rsCount=minCount;
						}
						
					}
					
				}
				
				// Create segments set:
				string[] newSegments=new string[Segments.Length - urlStart + rsCount];
				
				// Copy segments over:
				System.Array.Copy(
					relativeTo.Segments,0,
					newSegments,0,
					rsCount
				);
				
				System.Array.Copy(
					Segments,urlStart,
					newSegments,rsCount,
					Segments.Length - urlStart
				);
				
				// Apply segments:
				Segments=newSegments;
				
			}else if(url==""){
				Segments=new string[0];
			}else{
				
				// Split now:
				Segments=url.Split('/');
				
				// Got auth in hostname?
				string authHost=Segments[0];
				
				int atIndex=authHost.IndexOf('@');
				
				if(atIndex!=-1){
					
					// Got auth - pull it out:
					authorization=authHost.Substring(0,atIndex).Trim();
					Segments[0]=authHost.Substring(atIndex+1).Trim();
					
				}
				
			}
			
		}
		
		/// <summary>True if this path has a hostname.</summary>
		public bool hasHostname{
			get{
				
				return (Protocol!="resources" && Protocol!="scene" && Protocol!="data" && Protocol!="bundle" && Protocol!="blob");
				
			}
		}
		
		/// <summary>This path as its resolved absolute form, without the #hash (if there is one).</summary>
		public string absoluteNoHash{
			get{
				string result=Protocol+"://";
				
				int segmentCount=Segments.Length;
				
				for(int i=0;i<segmentCount;i++){
					
					result+=Segments[i];
					
					if(i==segmentCount-1){
						
						// Last one - we add a forward slash if the original raw URI had one:
						if(Query!=null){
							
							if(RawUrl!="" && RawUrl[RawUrl.IndexOf('?')-1]=='/'){
								result+="/";
							}
							
						}else if(RawUrl!="" && RawUrl[RawUrl.Length-1]=='/'){
							result+="/";
						}
						
					}else{
						result+="/";
					}
					
				}
				
				if(Query!=null){
					result+="?"+Query;
				}
				
				return result;
		
			}
		}
		
		/// <summary>This path as its resolved absolute form, including the #hash.</summary>
		public string absolute{
			get{
				string result=absoluteNoHash;
				
				if(Hash!=null){
					result+="#"+Hash;
				}
				
				return result;
				
			}
		}
		
		/// <summary>Reloads the current URL.</summary>
		public void reload(){
			assign(ToString(),false);
		}
		
		/// <summary>Assign (go to) the given URL.</summary>
		public void assign(string url){
			assign(url,true);
		}
		
		/// <summary>Assign (go to) the given URL.</summary>
		public void assign(string url,bool addHistory){
			
			if(document==null){
				Dom.Log.Add("Unable to load URL [this doesn't belong to any document].");
				return;
			}
			
			// Navigate there now:
			document.SetLocation(new Location(url,this),addHistory);
			
		}
		
		/// <summary>A colon for splitting.</summary>
		private static string[] Colon=new string[]{":"};
		
		/// <summary>The authorization string split by colon.</summary>
		private string[] authParts{
			get{
				return authorization.Split(Colon,2,StringSplitOptions.None);
			}
		}
		
		/// <summary>Username.</summary>
		public string username{
			get{
				
				if(authorization==null){
					return "";
				}
				
				return authParts[0];
				
			}
			set{
				if(authorization==null){
					authorization=value+":";
				}else{
					authorization=value+":"+password;
				}
			}
		}
		
		/// <summary>Password.</summary>
		public string password{
			get{
				
				if(authorization==null){
					return "";
				}
				
				return authParts[1];
				
			}
			set{
				if(authorization==null){
					// Silently fail
					return;
				}
				
				authorization=username+":"+value;
			}
		}
		
		/// <summary>Replaces the current URL with the given one. The same as assign in PowerUI.</summary>
		public void replace(string url){
			assign(url);
		}
		
		/// <summary>Gets the filename without the filetype.</summary>
		public string Filename{
			get{
				string[] pieces=File.Split('.');
				if(pieces.Length==1){
					return pieces[0];
				}
				// Return all but the last one.
				int top=pieces.Length-1;
				string filename=null;
				for(int i=0;i<top;i++){
					if(i==0){
						filename=pieces[i];
					}else{
						filename+="."+pieces[i];
					}
				}
				return filename;
			}
		}
		
		/// <summary>Gets the full filename with the type included.</summary>
		public string File{
			get{
				if(Segments.Length==0){
					return "";
				}
				return Segments[Segments.Length-1];
			}
			set{
				
				if(Segments==null || Segments.Length==0){
					Segments=new string[1]{value};
				}else{
					
					Segments[Segments.Length-1]=value;
					
					// Update URL:
					string url="";
					
					for(int i=0;i<Segments.Length;i++){
						
						if(i!=0){
							url+="/";
						}
						
						url+=Segments[i];
					}
					
				}
				
			}
		}
		
		/// <summary>Gets the type of file this path points to.</summary>
		public string Filetype{
			get{
				string[] pieces=File.Split('.');
				return pieces[pieces.Length-1];
			}
		}
		
		/// <summary>Gets the full directory path this file is in.</summary>
		public string Directory{
			get{
				string result="";
				int segmentCount=Segments.Length-1;
				
				for(int i=0;i<segmentCount;i++){
					result+=Segments[i]+"/";
				}
				
				return result;
			}
		}
		
		/// <summary>Gets the url of this file without the protocol.</summary>
		public string Path{
			get{
				return Directory+File;
			}
		}
		
		/// <summary>The host name of this path.</summary>
		public string host{
			get{
				if(Segments!=null && Segments.Length>0){
					return Segments[0];
				}
				
				return "";
			}
			set{
				if(Segments==null || Segments.Length==0){
					Segments=new string[1];
				}
				
				Segments[0]=value;
			}
		}
		
		/// <summary>The hostname with no port number.</summary>
		public string hostname{
			get{
				// Grab the full host:
				string hostName=host;
				
				if(hostName==""){
					return "";
				}
				
				// Split by colon (if there is one):
				string[] pieces=hostName.Split(new string[]{":"},2,StringSplitOptions.None);
				
				// Return the first piece:
				return pieces[0];
			}
			set{
				string portNum=port;
				
				if(port!=""){
					host=value+":"+portNum;
				}else{
					host=value;
				}
			}
		}
		
		/// <summary>The port number, if there is one.</summary>
		public string port{
			get{
				// Grab the full host:
				string hostName=host;
				
				if(hostName==""){
					return "";
				}
				
				// Split by colon (if there is one):
				string[] pieces=hostName.Split(new string[]{":"},2,StringSplitOptions.None);
				
				if(pieces.Length==1){
					// Return the second piece:
					return pieces[1];
				}
				
				return "";
			}
			set{
				// Hostname no port:
				string h=hostname;
				
				if(string.IsNullOrEmpty(value)){
					host=h;
				}else{
					host=h+":"+value;
				}
			}
		}
		
		/// <summary>The path, as it was declared.</summary>
		public string href{
			get{
				return RawUrl;
			}
			set{
				assign(value);
			}
		}
		
		/// <summary>The hostname combined with the protocol.</summary>
		public string origin{
			get{
				return Protocol+"://"+host;
			}
		}
		
		/// <summary>This locations protocol, including the trailing colon (":").</summary>
		public string protocol{
			get{
				return Protocol+":";
			}
			set{
				Protocol=value.Replace(":","");
			}
		}
		
		/// <summary>The hash section of the URL, if there is one.</summary>
		public string hash{
			get{
				if(Hash==null){
					return "";
				}
				
				return Hash;
			}
			set{
				
				if(Hash==value){
					return;
				}
				
				if(document!=null){
					
					// Hash change!
					HashChangeEvent hce=new HashChangeEvent();
					hce.oldURL=absolute;
					Hash=value;
					hce.newURL=absolute;
					hce.SetTrusted();
					
					EventTarget window=document.windowTarget;
					
					if(window!=null){
						window.dispatchEvent(hce);
					}
					
				}else{
					Hash=value;
				}
				
			}
		}
		
		/// <summary>Gets everything between host and the search string.</summary>
		public string pathname{
			get{
				string result="/";
				int segmentCount=Segments.Length-1;
				
				for(int i=1;i<segmentCount;i++){
					result+=Segments[i]+"/";
				}
				
				if(Segments.Length>0){
					// Also got a file - stick that on the end too.
					result+=file;
				}
				
				return result;
			}
			set{
				if(string.IsNullOrEmpty(value)){
					Segments=new string[1]{host};
					return;
				}
				
				// Always start with fwdslash:
				if(!value.StartsWith("/")){
					value="/"+value;
				}
				
				// Split and insert host:
				string[] pieces=value.Split('/');
				pieces[0]=host;
				Segments=pieces;
			}
		}
		
		/// <summary>The file without a query string.</summary>
		public string file{
			get{
				string[] pieces=File.Split(new string[]{"?"},2,StringSplitOptions.None);
				
				return pieces[0];
			}
		}
		
		/// <summary>The search parameters in a parsed set.</summary>
		public Dictionary<string,string> searchParams{
			get{
				
				// Create the set:
				Dictionary<string,string> set=new Dictionary<string,string>();
				
				// Load:
				LoadUrlString(search,set);
				
				return set;
				
			}
		}
		
		/// <summary>Loads a set of parameters from a typical URL formatted string. E.g. hello=1&hi=2.
		/// Loads into the given set - it will overwrite any existing values.</summary>
		public static void LoadUrlString(string getString,Dictionary<string,string> set){
			
			if(getString==null){
				return;
			}
			
			string[] pieces=getString.Split('&');
			
			foreach(string piece in pieces){
				string[] parts=piece.Split(new string[]{"="},2,StringSplitOptions.None);
				
				if(parts.Length==2){
					set[parts[0]]=Uri.UnescapeDataString(parts[1]);
				}else{
					set[parts[0]]="";
				}
			}
			
		}
		
		/// <summary>This location relative to host.</summary>
		public string relative{
			get{
				string result=pathname;
				
				if(Query!=null){
					result+="?"+Query;
				}
				
				return result;
			}
		}
		
		/// <summary>Gets the query string, if there is one.</summary>
		public string search{
			get{
				return Query;
			}
			set{
				Query=value;
			}
		}
		
		/// <summary>Is this a web location?</summary>
		public bool web{
			get{
				string proto=Protocol;
				
				return (proto=="http" || proto=="https");
			}
		}
		
		/// <summary>Gets the full path as a string.</summary>
		public override string ToString(){
			return RawUrl;
		}
		
		/// <summary>Used to map punycode domains.</summary>
		private static System.Globalization.IdnMapping PunyMapper_;
		
		/// <summary>Used to map punycode domains.</summary>
		public static System.Globalization.IdnMapping PunyMapper{
			get{
				if(PunyMapper_==null){
					PunyMapper_=new System.Globalization.IdnMapping();
				}
				
				return PunyMapper_;
			}
		}
		
		/// <summary>Converts a host to punycode.</summary>
		public static string ToPunycode(string host){
			
			if(host==null){
				// not suitable.
				return host;
			}
			
			System.Globalization.IdnMapping mapper=PunyMapper;
			
			return mapper.GetAscii(host);
			
		}
		
		/// <summary>Converts punycode back to unicode.</summary>
		public static string FromPunycode(string host){
			
			if(host==null){
				// not suitable.
				return host;
			}
			
			System.Globalization.IdnMapping mapper=PunyMapper;
			
			return mapper.GetUnicode(host);
			
		}
		
	}
	
}