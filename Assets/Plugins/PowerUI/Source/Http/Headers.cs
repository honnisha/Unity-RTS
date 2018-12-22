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


namespace PowerUI.Http{
	
	/// <summary>
	/// Deals with HTTP headers. Internally the indices are always lowercase. Empty string is the status line.
	/// </summary>
	
	public class Headers{
		
		/// <summary>The raw headers set. Indices are always lowercase. Empty string is the status line.</summary>
		public Dictionary<string,List<string>> lines;
		
		
		/// <summary>The status line. Null if none set.</summary>
		public string status{
			get{
				return Get("");
			}
			set{
				Overwrite("",value);
			}
		}
		
		public void LoadFrom(Dictionary<string, string> headers, int statusCode){
			status="HTTP/1.1 " + statusCode + " ?";
			foreach(var kvp in headers){
				Overwrite(kvp.Key, kvp.Value);
			}
		}
		
		/// <summary>Sets the named header only if it's not already set.</summary>
		public void AddIfNew(string name,string value){
			
			name=name.ToLower().Trim();
			
			if(lines==null){
				lines=new Dictionary<string,List<string>>();
			}else if(lines.ContainsKey(name)){
				return;
			}
			
			// Add:
			List<string> set=new List<string>(1);
			set.Add(value);
			lines[name]=set;
			
		}
		
		/// <summary>Sets the named header with the given single value.</summary>
		public void Overwrite(string name,string value){
			
			if(lines==null){
				lines=new Dictionary<string,List<string>>();
			}
			
			// Add:
			List<string> set=new List<string>(1);
			set.Add(value);
			lines[name.ToLower().Trim()]=set;
			
		}
		
		/// <summary>Gets the last header with the named value.</summary>
		public string Get(string header){
			
			List<string> set=GetAll(header);
			
			if(set==null){
				return null;
			}
			
			return set[set.Count-1];
			
		}
		
		/// <summary>Gets the set of headers of the given type. E.g. all Set-Cookie headers.</summary>
		public List<string> GetAll(string header){
			
			if(header==null){
				throw new ArgumentException("Header was null","header");
			}
			
			if(lines==null){
				// Headers currently unavailable.
				return null;
			}
			
			// Tidy:
			header=header.Trim().ToLower();
			
			List<string> val;
			lines.TryGetValue(header,out val);
			return val;
			
		}
		
		/// <summary>Appends the named header.</summary>
		public void Add(string name,string value){
			
			// Tidy:
			name=name.ToLower().Trim();
			
			if(lines==null){
				lines=new Dictionary<string,List<string>>();
			}
			
			List<string> set;
			if(lines.TryGetValue(name,out set)){
				set=new List<string>();
				lines[name]=set;
			}
			
			set.Add(value);
			
		}
		
		/// <summary>Gets/ sets a header (overwrites rather than appends).</summary>
		public string this[string header]{
			get{
				return Get(header);
			}
			set{
				Overwrite(header,value);
			}
		}
		
		/// <summary>Applies the given raw headers, overwriting everything else in this set.</summary>
		public void Apply(string rawText){
			lines=ParseHeaders(rawText);
		}
		
		/// <summary>Gets this as a valid string of HTTP headers.</summary>
		public override string ToString(){
			return HeaderString(lines);
		}
		
		/// <summary>Gets the headers as a set for use by Unity's WWW.</summary>
		public Dictionary<string,string> ToSingleSet(){
			
			if(lines==null){
				return null;
			}
			
			Dictionary<string,string> sng=new Dictionary<string,string>();
			
			foreach(KeyValuePair<string,List<string>> kvp in lines){
				
				if(kvp.Key==""){
					continue;
				}
				
				// Add:
				sng[kvp.Key]=kvp.Value[kvp.Value.Count-1];
				
			}
			
			return sng;
			
		}
		
		/// <summary>Used to split headers up.</summary>
		private static string[] HeaderSplit=new string[]{"\r\n"};
		
		/// <summary>Converts a raw header string to a set of headers.
		/// The status line is indexed as the empty string.</summary>
		public static Dictionary<string,List<string>> ParseHeaders(string rawHeaders){
			
			if(rawHeaders==null || rawHeaders.Length==0){
				// None available.
				return null;
			}
			
			// Get the header lines:
			string[] lines=rawHeaders.Split(HeaderSplit,StringSplitOptions.None);
			
			// Setup our set now:
			Dictionary<string,List<string>> headers=new Dictionary<string,List<string>>();
			
			// Add the status line:
			List<string> set=new List<string>(1);
			set.Add(lines[0]);
			headers[""]=set;
			
			// (Skip the first line)
			for(int i=1;i<lines.Length;i++){
				
				string l=lines[i];
				
				int colIdx=l.IndexOf(':');
				
				if(colIdx<1){
					// E.g. empty line.
					continue;
				}
				
				// Get type and value:
				string headerType = l.Substring( 0,colIdx ).Trim().ToLower();
				
				string headerVal = l.Substring( colIdx+1 ).Trim();
				
				// Get/ create the set:
				if(!headers.TryGetValue(headerType,out set)){
					set=new List<string>();
					headers[headerType]=set;
				}
				
				// Add:
				set.Add(headerVal);
				
			}
			
			return headers;
			
		}
		
		/// <summary>Converts a list of headers to the raw header string. Always includes the status line.
		/// Note that the header set contains the status line indexed as the empty string.</summary>
		public static string HeaderString(Dictionary<string,List<string>> headers){
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			List<string> set;
			if(headers!=null && headers.TryGetValue("",out set)){
				
				// Status line:
				sb.Append(set[0]);
				
			}else{
				
				// Make up a status line:
				sb.Append("HTTP/1.1 200 OK");
				
			}
			
			sb.Append("\r\n");
			
			if(headers!=null){
				
				foreach(KeyValuePair<string,List<string>> kvp in headers){
					
					if(kvp.Key==""){
						continue;
					}
					
					// Correct the caps:
					string headerName=ToHeaderCapitals(kvp.Key);
					
					// Append each value as a new header:
					for(int i=0;i<kvp.Value.Count;i++){
						
						sb.Append(headerName);
						sb.Append(": ");
						sb.Append(kvp.Value[i]);
						sb.Append("\r\n");
						
					}
					
				}
				
			}
			
			// All done!
			return sb.ToString();
			
		}
		
		/// <summary>Converts a header name, e.g. "content-type", to its capitalized form.
		/// This is technically not required by the standard, but some servers are non-compliant
		/// and don't work correctly if you send broken headers.</summary>
		public static string ToHeaderCapitals(string h){
			
			// Get as chars:
			char[] headerName=h.ToCharArray();
			
			// UC first:
			headerName[0]=char.ToUpper(headerName[0]);
			
			// Find each dash and capitalize the letter after it:
			int max=headerName.Length-1;
			
			for(int i=1;i<max;i++){
				
				if(headerName[i]=='-'){
					
					// Because of max, this can't go out of range.
					headerName[i+1]=char.ToUpper(headerName[i+1]);
					i++;
					continue;
					
				}
				
			}
			
			return new string(headerName);
			
		}
		
	}
	
}