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
using Dom;
using System.Text;
using UnityEngine;


namespace PowerUI{
	
	public delegate void GeolocationEvent(GeoPosition position);
	public delegate void GeolocationErrorEvent(GeoPositionError error);
	
	/// <summary>
	/// The Geolocation Web API.
	/// </summary>
	public class Geolocation{
		
		/// <summary>The last recorded position. Stored to compute approximate speed.</summary>
		public static GeoPosition Latest;
		/// <summary>First waiting request.</summary>
		internal static PendingLocationRequest FirstQueued;
		/// <summary>Calls our update function for us, but only when we actually need it to.</summary>
		internal static OnUpdateCallback Updater;
		
		
		/// <summary>Update waiting requests.</summary>
		internal static void Update(){
			
			// Get service:
			LocationService ls=UnityEngine.Input.location;
			
			// Get first in queue:
			PendingLocationRequest req=FirstQueued;
			
			if(ls.status!=LocationServiceStatus.Initializing){
				
				// Ready! All will clear now:
				FirstQueued=null;
				
				while(req!=null){
					
					// Call ready:
					req.Ready();
					
					// Next:
					req=req.Next;
					
				}
				
			}else{
				
				// Handle timeouts:
				PendingLocationRequest prev=null;
				
				float limit=Updater.deltaTime;
				
				while(req!=null){
					
					// Update it:
					if(req.Update(limit)){
						
						// Done; remove from queue:
						if(prev==null){
							FirstQueued=req.Next;
						}else{
							prev.Next=req.Next;
						}
						
						// Don't update prev:
						req=req.Next;
						continue;
						
					}
					
					// Next:
					prev=req;
					req=req.Next;
					
				}
				
			}
			
			if(FirstQueued==null){
				// Kill the updater:
				Updater.Stop();
				Updater=null;
			}
			
		}
		
		/// <summary>Gets the current position.</summary>
		public void getCurrentPosition(GeolocationEvent success,GeolocationErrorEvent error,GeoPositionOptions options){
			
			// Get service:
			LocationService ls=UnityEngine.Input.location;
			
			if(!ls.isEnabledByUser){
				
				// Denied.
				if(error!=null){
					error(new GeoPositionError(GeoPositionError.PERMISSION_DENIED));
				}
				
				return;
				
			}
			
			// Started yet?
			if(ls.status==LocationServiceStatus.Stopped){
				
				// Start now:
				ls.Start();
				
			}else if(ls.status!=LocationServiceStatus.Initializing){
				
				// Call ready now:
				OnReady(success,error,options);
				
				return;
				
			}
			
			// Enqueue, taking our timeout into account.
			PendingLocationRequest node=new PendingLocationRequest();
			
			// Apply settings:
			node.Success=success;
			node.Error=error;
			node.Options=options;
			
			if(options!=null && options.timeout!=0f){
				
				// Get timeout in seconds:
				node.Timeout=(float)options.timeout/1000f;
				
			}
			
			node.Next=FirstQueued;
			FirstQueued=node;
			
			if(Updater==null){
				
				// Start calling update now at 10fps:
				OnUpdate.Add(Update,10f);
				
			}
			
		}
		
		internal static void OnReady(GeolocationEvent success,GeolocationErrorEvent error,GeoPositionOptions options){
			
			// High accuracy:
			bool highAcc=(options!=null && options.enableHighAccuracy);
			
			// Age:
			double maxAge=(options==null)?0 : options.maximumAge;
			
			if(options!=null && options.maximumAge!=0 && Latest!=null){
				
				// Can we recycle latest?
				if(Latest.age<maxAge){
					
					// yep!
					if(success!=null){
						success(Latest);
					}
					
					return;
					
				}
				
			}
			
			// Get service:
			LocationService ls=UnityEngine.Input.location;
			
			if(ls.status==LocationServiceStatus.Failed){
				// Error - failed.
				if(error!=null){
					error(new GeoPositionError(GeoPositionError.POSITION_UNAVAILABLE));
				}
				
				return;
			}
			
			// Create:
			GeoPosition pos=new GeoPosition(ls.lastData,highAcc);
			
			// Ok!
			if(success!=null){
				success(pos);
			}
			
		}
		
	}
	
}