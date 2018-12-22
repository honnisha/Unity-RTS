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
using PowerUI.Http;
using Dom;
using Json;


namespace PowerUI{
	
	/// <summary>
	/// The XMLHttpRequest API. Use this from C# too.
	/// </summary>
	
	public partial class XMLHttpRequest : DataPackage{
		
		/// <summary>The unsent ready state.</summary>
		public const short UNSENT=0;
		/// <summary>The opened ready state.</summary>
		public const short OPENED=1;
		/// <summary>The received ready state.</summary>
		public const short HEADERS_RECEIVED=2;
		/// <summary>The loading ready state.</summary>
		public const short LOADING=3;
		/// <summary>The done ready state.</summary>
		public const short DONE=4;
		
		/// <summary>The host document.</summary>
		private HtmlDocument HostDocument;
		/// <summary>The method to use.</summary>
		private string Method="get";
		
		
		/// <summary>Creates a request relative to the given document.</summary>
		public XMLHttpRequest(HtmlDocument document){
			
			// Apply document:
			HostDocument=document;
			
		}
		
		/// <summary>Creates a request.
		/// Note that you must use an absolute URL with this one (pass a document otherwise).</summary>
		public XMLHttpRequest(){
			
		}
		
		/*
		public void overrideMimeType(string newType){
			
			// Change the Content-Type response header.
			
		}
		*/
		
		/// <summary>Opens the request.</summary>
		public void open(string method,string url){
			open(method,url,true,"","");
		}
		
		/// <summary>Opens the request.</summary>
		public void open(string method,string url,bool async){
			open(method,url,async,"","");
		}
		
		/// <summary>Opens the request.</summary>
		public void open(string method,string url,bool async,string user,string password){
			
			if(!async){
				
				throw new NotImplementedException("Don't use non-async XMLHttpRequests (they're depreciated in browsers and can crash your project).");
				
			}
			
			Method=method.Trim().ToLower();
			
			if(Method=="get" || Method=="post"){
				
				// Apply the path:
				location=new Location(
					url,
					(HostDocument==null)? null : HostDocument.basepath
				);
				
			}else{
				
				throw new NotImplementedException("GET or POST methods only at the moment. Awaiting better platform support for UnityWebRequest.");
				
			}
			
			// Got auth?
			if(!string.IsNullOrEmpty(user) || !string.IsNullOrEmpty(password)){
				
				if(user==null){
					user="";
				}
				
				if(password==null){
					password="";
				}
				
				// Apply authorization:
				location.authorization=user+":"+password;
				
			}
			
			// Opened:
			readyState=1;
			
		}
		
		/// <summary>Send this request with the given post data.</summary>
		public void send(byte[] data){
			
			if(Method=="post"){
				if(data==null){
					request=new byte[0];
				}else{
					request=data;
				}
			}
			
			// Send now!
			base.send();
			
		}
		
		/// <summary>Send this request with the given post data.</summary>
		public void send(JSObject data){
			
			if(Method=="post"){
				if(data==null){
					request=new byte[0];
				}else{
					request=System.Text.Encoding.UTF8.GetBytes(JSON.Stringify(data));
				}
			}
			
			// Send now!
			base.send();
			
		}
		
		/// <summary>Send this request with the given post data.</summary>
		public void send(HtmlDocument data){
			
			if(Method=="post"){
				if(data==null){
					request=new byte[0];
				}else{
					request=System.Text.Encoding.UTF8.GetBytes(data.innerHTML);
				}
			}
			
			// Send now!
			base.send();
			
		}
		
		/// <summary>Send this request with the given post data.</summary>
		public void send(string data){
			
			if(Method=="post"){
				if(data==null){
					request=new byte[0];
				}else{
					request=System.Text.Encoding.UTF8.GetBytes(data);
				}
			}
			
			// Send now!
			base.send();
			
		}
		
		/// <summary>Sets a request header.</summary>
		public void setRequestHeader(string header){
			setRequestHeader(header,"");
		}
		
		/// <summary>Sets a request header.</summary>
		public void setRequestHeader(string header,string value){
			
			if(header==null){
				return;
			}
			
			if(value==null){
				value="";
			}
			
			header=header.Trim().ToLower();
			
			// Restricted?
			if(HttpHeaders.Restricted(header)){
				throw new Exception("'"+header+"' is restricted.");
			}
			
			requestHeaders[header]=value;
			
		}
		
		/// <summary>Specified response header.</summary>
		public string getResponseHeader(string header){
			
			header=header.Trim().ToLower();
			
			// Restricted?
			if(HttpHeaders.Restricted(header)){
				throw new Exception("'"+header+"' is restricted.");
			}
			
			return responseHeaders[header];
			
		}
		
		/// <summary>Send this request.</summary>
		public override void send(){
			
			if(Method=="post"){
				request=new byte[0];
			}
			
			// Send now!
			base.send();
			
		}
		
		/// <summary>The complete header text.</summary>
		public string getAllResponseHeaders(){
			return responseHeaders.ToString();
		}
		
		/// <summary>The URL.</summary>
		public string responseURL{
			get{
				string u=location.absoluteNoHash;
				
				if(u==null){
					return "";
				}
				
				return u;
				
			}
		}
		
		/// <summary>The response type.</summary>
		public string responseType="";
		
		/// <summary>A type dependant response.</summary>
		public object response{
			get{
				
				if(responseType==null){
					responseType="";
				}
				
				switch(responseType){
					
					case "json":
						return JSON.Parse(responseText);
					case "blob":
						return responseBytes;
					
				}
				
				return responseText;
				
			}
		}
		
	}
	
}