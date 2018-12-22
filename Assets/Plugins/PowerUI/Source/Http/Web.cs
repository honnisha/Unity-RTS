//--------------------------------------
//          Kulestar Unity HTTP
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;

namespace PowerUI.Http{
	
	/// <summary>
	/// Performs Http requests independently.
	/// Note that you must call Update to keep all the requests active.
	/// </summary>
	
	public static class Web{
		
		/// <summary>The time since the last update in seconds.</summary>
		public static float UpdateTimer;
		/// <summary>The max time in seconds between updates.</summary>
		private static float TimerLimit=0.05f;
		/// <summary>Active http requests are stored in a linked list. This is the tail of the list.</summary>
		public static HttpRequest LastRequest;
		/// <summary>Active http requests are stored in a linked list. This is the head of the list.</summary>
		public static HttpRequest FirstRequest;
		/// <summary>The amount of requests that are currently active.</summary>
		public static int CurrentActiveRequests;
		/// <summary>Waiting http requests are stored in a linked list - added here when MaxSimultaneousRequests is exceeded. 
		/// This is the tail of the list.</summary>
		public static HttpRequest LastWaitingRequest;
		/// <summary>Waiting http requests are stored in a linked list - added here when MaxSimultaneousRequests is exceeded. 
		/// This is the head of the list.</summary>
		public static HttpRequest FirstWaitingRequest;
		/// <summary>The maximum amount of simultaneous requests that will be allowed.
		/// Exceeding this will result in the additional requests entering the waiting list. -1 (default) is no limit.</summary>
		public static int MaxSimultaneousRequests=-1;
		
		/// <summary>Sets the rate at which http requests are advanced.</summary>
		/// <param name="fps">The rate in frames per second.</param>
		public static void SetRate(int fps){
			
			if(fps<=0){
				// Default rate is 20fps.
				fps=20;
			}
			
			TimerLimit=1f/(float)fps;
		}
		
		/// <summary>Clears all active requests.</summary>
		public static void Clear(){
			FirstRequest=LastRequest=null;
		}
		
		/// <summary>Encodes the given piece of text so it's suitable to go into a post or get string.</summary>
		/// <param name="text">The text to encode.</param>
		/// <returns>The url encoded text.</returns>
		public static string UrlEncode(string text){
			return System.Uri.EscapeDataString(text);
		}
		
		/// <summary>Decodes the given piece of text from a post or get string.</summary>
		/// <param name="text">The text to decode.</param>
		/// <returns>The url decoded text.</returns>
		public static string UrlDecode(string text){
			return System.Uri.UnescapeDataString(text);
		}
		
		/// <summary>Metered. Advances all the currently active http requests.</summary>
		public static void Update(float deltaTime){
			
			UpdateTimer+=deltaTime;
			
			if(UpdateTimer<TimerLimit){
				return;
			}
			
			Flush();
		}
		
		/// <summary>Advances all the currently active http requests.</summary>
		public static void Flush(){
			UpdateTimer=0f;
			
			if(FirstRequest==null){
				return;
			}
			
			HttpRequest current=FirstRequest;
			
			while(current!=null){
				current.Update(TimerLimit);
				current=current.RequestAfter;
			}
			
		}
		
		public static void UpdateWaitingList(){
			// Dequeue from the waiting list.
			if(FirstWaitingRequest==null){
				return;
			}
			
			// Grab the one at the front:
			HttpRequest waiting=FirstWaitingRequest;
			// Pop it from the list:
			waiting.Remove(true);
			// And queue it up in the main list:
			Queue(waiting);
		}
		
		public static void Queue(HttpRequest request){
			
			if(MaxSimultaneousRequests!=-1 && CurrentActiveRequests==MaxSimultaneousRequests){
				// Add to the waiting list:
				if(FirstWaitingRequest==null){
					LastWaitingRequest=FirstWaitingRequest=request;
				}else{
					request.RequestBefore=LastWaitingRequest;
					LastWaitingRequest=LastWaitingRequest.RequestAfter=request;
				}
				// The waiting list will be updated when an active request completes.
			}else{
				// Bump up the active count:
				CurrentActiveRequests++;
				
				// Add to main queue:
				if(FirstRequest==null){
					LastRequest=FirstRequest=request;
				}else{
					request.RequestBefore=LastRequest;
					LastRequest=LastRequest.RequestAfter=request;
				}
				
			}
			
		}
		
	}
	
}