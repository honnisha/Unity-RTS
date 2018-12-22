//--------------------------------------
//          Kulestar Unity HTTP
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5
	#define PRE_UNITY4
#endif

#if PRE_UNITY4 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4
	#define PRE_UNITY4_5
#endif

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using PowerUI;
using Dom;


namespace PowerUI.Http{
	
	/// <summary>
	/// Represents a single http request. Follows redirections.
	/// Generally don't use this directly; instead create either an XMLHttpRequest
	/// or e.g. a DataPackage.
	/// </summary>
	
	public partial class HttpRequest : IAbortable{
		
		/// <summary>The url that was requested.</summary>
		public Location location{
			get{
				return Package.location;
			}
		}
		
		/// <summary>The url that was requested.</summary>
		public string url{
			get{
				return location.absolute;
			}
		}
		
		/// <summary>Number of redirects.</summary>
		public int RedirectionCount;
		#if UNITY_2017_1_OR_NEWER
		/// <summary>The unity WWW object which performs the underlying request.</summary>
		public UnityEngine.Networking.UnityWebRequest WWWRequest;
		#else
		/// <summary>The unity WWW object which performs the underlying request.</summary>
		public WWW WWWRequest;
		#endif
		#if !MOBILE && !UNITY_WEBGL && !UNITY_TVOS
		/// <summary>The video being downloaded, if any. Note: Pro only.</summary>
		public MovieTexture Movie;
		#endif
		
		/// <summary>HACK! Workaround for a long standing Unity bug that doesn't allow multiple SET-COOKIE headers.</summary>
		private static PropertyInfo ResponseHeadersString;
		
		/// <summary>The response header string.</summary>
		public string ResponseHeaderString{
			get{
				
				if(ResponseHeadersString==null){
					
					ResponseHeadersString = typeof(WWW).GetProperty(
						"responseHeadersString",
						BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance
					);
					
				}
				
				if(ResponseHeadersString==null){
					Debug.LogError( "www.responseHeadersString not found. Deprecated?" );
					return "";
				}
				
				// Get the header string:
				return ResponseHeadersString.GetValue( WWWRequest, null ) as string;
				
			}
		}
		
		/// <summary>Timeout if there is one.</summary>
		public float Timeout_=float.MaxValue;
		/// <summary>How long this request has taken so far.</summary>
		public float Duration;
		/// <summary>Active requests are in a linked list. The http request that follows this one.</summary>
		public HttpRequest RequestAfter;
		/// <summary>Active requests are in a linked list. The http request that is before this one.</summary>
		public HttpRequest RequestBefore;
		/// <summary>The enumerator which processes the unity WWW object.</summary>
		private IEnumerator LoadingEnumerator;
		/// <summary>The package this request originates from.</summary>
		public ContentPackage Package;
		/// <summary>The set of request headers. Pulled from the package.</summary>
		private Dictionary<string,string> RequestHeaders;
		
		
		/// <summary>Creates a new http request using the given package.</summary>
		/// <param name="package">The package that will receive the updates.</param>
		/// <param name="onDone">A method to call with the result.</param>
		public HttpRequest(ContentPackage package){
			
			// Set package:
			Package=package;
			
			// Hook up as abortable:
			package.abortableObject=this;
			
		}
		
		/// <summary>Sends this request.
		/// Note that this does not block and is thread safe. Instead, OnRequestDone will be called when it's done.</summary>
		public void Send(){
			
			// Clear RDC:
			RedirectionCount=0;
			
			// Get the cookie jar:
			CookieJar jar=location.CookieJar;
			
			if(jar!=null){
				// We've got a cookie jar!
				
				// Set cookie string:
				Package.requestHeaders["cookie"]=jar.GetCookieHeader(location);
				
			}
			
			// Got auth?
			if(location.authorization!=null){
				
				// Get bytes:
				byte[] authBytes=System.Text.Encoding.UTF8.GetBytes(location.authorization);
				
				// Apply the auth header:
				Package.requestHeaders["authorization"]=System.Convert.ToBase64String(authBytes);
				
			}
			
			// Request now:
			RequestHeaders=Package.requestHeaders.ToSingleSet();
			
			BeginRequest(url,Package.request,RequestHeaders);
			
			// Push this onto the front of our update queue:
			Web.Queue(this);
			
		}
		
		private void BeginRequest(string url,byte[] data,Dictionary<string,string> headers){
		
			// Got to run on the main thread:
			Callback.MainThread(delegate(){
				
				#if UNITY_2017_1_OR_NEWER
				WWWRequest = UnityEngine.Networking.UnityWebRequest.Get(url);
				
				if(data != null){
					WWWRequest.method = "POST";
					var handler = new UnityEngine.Networking.UploadHandlerRaw(data);
					string contentType = null;
					if(headers == null || !headers.TryGetValue("content-type", out contentType)){
						contentType="application/json";
					}
					handler.contentType = contentType;
					
					if(headers != null){
						foreach(var headerPair in headers){
							
							var key = headerPair.Key;
							
							if(
								key == "accept-charset" || key == "access-control-request-headers" || 
								key == "access-control-request-method" || key == "connection" || key =="date" || 
								key == "dnt" || key=="expect" || key=="host" || key =="keep-alive" || 
								key == "origin" || key== "referer" || key=="te" || key=="trailer" || 
								key == "transfer-encoding" || key== "upgrade" || key== "via"
							){
								continue;
							}
							
							WWWRequest.SetRequestHeader(headerPair.Key, headerPair.Value);
						}
					}
					
					WWWRequest.uploadHandler = handler;
				}
				
				#else
				// Create the request:
				if(headers == null){
					WWWRequest=new WWW(url,data);
				}else{
					WWWRequest=new WWW(url,data,headers);
				}
				#endif
			});
			
		}
		
		/// <summary>Timeout in ms. Default is 0.</summary>
		public float timeout{
			get{
				return Timeout_;
			}
			set{
				Timeout_=value;
			}
		}
		
		/// <summary>Aborts this request (IAbortable interface).</summary>
		public void abort(){
			Remove(false);
		}
		
		/// <summary>Removes this request from the active linked list. It won't be updated anymore.</summary>
		public void Remove(){
			Remove(false);
		}
		
		/// <summary>Removes this request from a linked list.</summary>
		/// <param name="waitingList">True if it should be removed from the waiting queue; false for the active queue.</param>
		public void Remove(bool waitingList){
			if(RequestBefore==null){
				if(waitingList){
					Web.FirstWaitingRequest=RequestAfter;
				}else{
					Web.FirstRequest=RequestAfter;
				}
			}else{
				RequestBefore.RequestAfter=RequestAfter;
			}
			
			if(RequestAfter==null){
				if(waitingList){
					Web.LastWaitingRequest=RequestBefore;
				}else{
					Web.LastRequest=RequestBefore;
				}
			}else{
				RequestAfter.RequestBefore=RequestBefore;
			}
			
			if(waitingList){
				RequestAfter=null;
				RequestBefore=null;
			}else{
				Web.CurrentActiveRequests--;
				Web.UpdateWaitingList();
			}
		}
		
		/// <summary>True if the request had an issue. <see cref="PowerUI.HttpRequest.Error"/> is the error.</summary>
		public bool Errored{
			get{
				return !string.IsNullOrEmpty(Error);
			}
		}
		
		/// <summary>True if there was no Unity error.</summary>
		public bool Ok{
			get{
				return string.IsNullOrEmpty(Error);
			}
		}
		
		/// <summary>The error, if any, that occured whilst attempting to load the url.</summary>
		public string Error{
			get{
				return WWWRequest.error;
			}
		}
		
		/// <summary>The method which performs the loading of the unity WWW object.</summary>
		public IEnumerator Loader(){
			#if UNITY_2017_1_OR_NEWER
			yield return WWWRequest.Send();
			#else
			yield return WWWRequest;
			#endif
		}
		
		/// <summary>The response as text. Empty string if there was an error. </summary>
		public string Text{
			get{
				if(Errored){
					return "";
				}
				#if UNITY_2017_1_OR_NEWER
				return WWWRequest.downloadHandler.text;
				#else
				return WWWRequest.text;
				#endif
			}
		}
		
		/// <summary>The raw bytes of the response. Null if there was an error.</summary>
		public byte[] Bytes{
			get{
				if(Errored){
					return null;
				}
				#if UNITY_2017_1_OR_NEWER
				return WWWRequest.downloadHandler.data;
				#else
				return WWWRequest.bytes;
				#endif
			}
		}
		
		#if !MOBILE && !UNITY_WEBGL && !UNITY_TVOS
		/// <summary>The response as a video. Null if there was an error.</summary>
		public MovieTexture Video{
			get{
				if(Errored){
					return null;
				}
				return Movie;
			}
		}
		#endif
		
		/*
		/// <summary>The response as an image. Null if there was an error.</summary>
		public Texture2D Image{
			get{
				if(Errored){
					return null;
				}
				#if UNITY_2017_1_OR_NEWER
				var tex = new Texture2D(0,0);
				tex.LoadImage(WWWRequest.downloadHandler.data);
				return tex;
				#else
				return WWWRequest.texture;
				#endif
			}
		}
		*/
		
		/// <summary>The current download progress.</summary>
		public float Progress{
			get{
				#if UNITY_2017_1_OR_NEWER
				return WWWRequest.downloadProgress;
				#else
				return WWWRequest.progress;
				#endif
			}
		}
		
		/// <summary>Checks if headers are available yet and handles them if they are.</summary>
		private void HandleHeaders(){
			
			#if UNITY_2017_1_OR_NEWER
			// Back to the old format, which unfortunately drops headers like Set-Cookie and builds the header set repeatedly.
			var headers = WWWRequest.GetResponseHeaders();
			if(headers == null || headers.Count == 0) {
				return;
			}
			Package.responseHeaders.LoadFrom(headers, (int)WWWRequest.responseCode);
			Package.ReceivedHeaders();
			bool redirect = false;
			#else
			string rawHeaderString=ResponseHeaderString;
			
			if(string.IsNullOrEmpty(rawHeaderString)){
				
				// Not available yet.
				return;
			}
			
			// Received headers:
			bool redirect=Package.ReceivedHeaders(rawHeaderString);
			#endif
			
			if(redirect){
				
				// Redirection. We'll most likely be making another request, unless we've redirected too many times:
				RedirectionCount++;
				
				if(RedirectionCount>=20){
					
					// Failed. Too many redirects.
					Package.statusCode=ErrorHandlers.TooManyRedirects;
					
				}else{
					
					// Redirect now (note that ready state was unchanged - redirects are supposed to be silent):
					Duration=0f;
					
					// Get the location:
					string redirectedTo=Package.responseHeaders["location"];
					
					// Set redir to:
					Package.redirectedTo=new Location(redirectedTo,location);
					
					// Get absolute:
					redirectedTo=Package.redirectedTo.absoluteNoHash;
					
					if(string.IsNullOrEmpty(redirectedTo) || redirectedTo.Trim()==""){
						
						// Pop it from the update queue:
						Remove();
						
						// Failed!
						Package.Failed(500);
						
					}else{
						
						if(Package.statusCode==307 || Package.statusCode==308){
							
							// Resend as-is to the new URI:
							BeginRequest(redirectedTo,Package.request,RequestHeaders);
							
						}else{
							
							// GET request to the new URI:
							BeginRequest(redirectedTo,null,RequestHeaders);
							
						}
						
						return;
						
					}
					
				}
				
			}
			
			#if !MOBILE && !UNITY_WEBGL && !UNITY_2017_1_OR_NEWER && !UNITY_TVOS
			//  We might be streaming video content.
			if(ContentType=="video/ogg"){
				Movie=WWWRequest.movie;
			}
			#endif
			
		}
		
		/// <summary>Response content type.</summary>
		public string ContentType{
			get{
				return Package.contentType;
			}
		}
		
		/// <summary>Advances this request by checking in on it's progress.</summary>
		public void Update(float deltaTime){
			
			if(WWWRequest==null){
				// Not setup yet.
				return;
			}
			
			if(LoadingEnumerator==null){
				
				// Let's go!
				LoadingEnumerator=Loader();
				
				// Advance the first time:
				LoadingEnumerator.MoveNext();
				
			}
			
			Duration+=deltaTime;
			
			if(Duration>=Timeout_){
				
				// Timeout!
				Package.TimedOut();
				
				// Done:
				Remove();
				return;
				
			}
			
			if(WWWRequest.isDone){
				
				if(Package.readyState_<2){
					
					// Load the headers:
					HandleHeaders();
					
				}
				
				if(Errored){
					
					// Figure out a suitable error code:
					Package.statusCode=ErrorHandlers.GetUnityErrorCode(Error);
					
				}
				
				if(!WWWRequest.isDone){
					// Yes, this is actually possible! Quit and go around again:
					return;
				}
				
				// Received:
				#if UNITY_2017_1_OR_NEWER
				byte[] bytes=WWWRequest.downloadHandler.data;
				#else
				byte[] bytes=WWWRequest.bytes;
				#endif
				Package.ReceivedData(bytes,0,bytes.Length);
				
				// Pop it from the update queue:
				Remove();
				
			}else if(Progress!=0f){
				
				if(Package.readyState_<2){
					
					// Got headers yet?
					HandleHeaders();
				
				#if !MOBILE && !UNITY_WEBGL && !UNITY_TVOS
				}else if(Package.readyState_==2 && Movie!=null && Movie.isReadyToPlay){
					
					// Downloaded it far enough to try playing it - let the package know:
					Package.ReceivedMovieTexture(Movie);
					
				#endif
				
				}
				
			}
		}
		
	}
	
}