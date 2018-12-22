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
	
	/// <summary>
	/// A geolocation position.
	/// </summary>
	public class GeoPosition{
		
		/// <summary>MS time this was recorded at.</summary>
		public long timestamp;
		/// <summary>The coords.</summary>
		public GeoCoordinates coords;
		
		/// <summary>The age of this position.</summary>
		internal double age{
			
			get{
				
				// Get current ms:
				double msNow=(DateTime.UtcNow-new DateTime(1970,1,1,0,0,0,0,DateTimeKind.Utc)).TotalMilliseconds;
				
				// Get delta:
				return msNow - (double)( timestamp );
				
			}
			
		}
		
		/// <summary>Creates a geopos from the given location info.</summary>
		public GeoPosition(LocationInfo loc,bool highAccuracy){
			
			// Pull timestamp:
			timestamp=(long)loc.timestamp * 1000;
			
			double heading=double.NaN;
			double speed=0;
			
			GeoPosition prev=Geolocation.Latest;
			
			if(prev==null){
				heading=0;
				speed=0;
			}else{
				
				// Delta time (in seconds):
				double deltaTime=(double)(timestamp-prev.timestamp)/1000.0;
				
				if(deltaTime!=0.0){
					
					double distance;
					
					if(highAccuracy){
						
						// Distance and bearing:
						distance=distanceAccurate(prev.coords.longitude,prev.coords.latitude,prev.coords.altitude);
						
						if(distance!=0.0){
							// Get the bearing:
							heading=bearing(prev.coords.longitude,prev.coords.latitude);
						}
						
					}else{
					
						// Distance and bearing:
						distance=distanceFast(prev.coords.longitude,prev.coords.latitude,prev.coords.altitude,out heading);
						
					}
					
					// Figure out speed:
					speed=distance/deltaTime;
					
				}
				
			}
			
			// Create coords:
			coords=new GeoCoordinates(
				loc.longitude,
				loc.latitude,
				loc.altitude,
				loc.horizontalAccuracy,
				loc.verticalAccuracy,
				heading,
				speed
			);
			
		}
		
		/// <summary>Degrees to radians.</summary>
		private const double Deg2Rad=(double)Mathf.Deg2Rad;
		/// <summary>Radians to degrees.</summary>
		private const double Rad2Deg=(double)Mathf.Rad2Deg;
		/// <summary>The eccentricity of the WGS84 ellipsoid.</summary>
		private const double Eccentricity=298.257223563;
		/// <summary>The radius of the earth in meters.</summary>
		private const double EarthRadius=6378137.0;
		
		/// <summary>A quick distance between this point and the given one. Gets the bearing too.</summary>
		public double distanceFast(double lon2,double lat2,double alt2,out double bearing){
			
			double lon1=coords.longitude;
			double lat1=coords.latitude;
			double alt1=coords.altitude;
			
			double dLon=( (lon2-lon1) * Deg2Rad );
			double theta1 = lat1 * Deg2Rad;
			double theta2 = lat2 * Deg2Rad;
			double s1 = Math.Sin(theta2-theta1);
			double s2 = Math.Sin( dLon / 2.0);
			
			double a = s1 * s1 +
					Math.Cos(theta1) * Math.Cos(theta2) *
					s2 * s2;
			
			// Lateral distance:
			s2 = EarthRadius * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1.0-a));
			
			// Vertical distance (ignoring distortion - it'll be minor as the points are very close in this use case):
			s1=alt2 - alt1;
			
			// Sqrt for the distance:
			s1=Math.Sqrt(s2*s2 + s1*s1);
			
			if(s1==0.0){
				bearing=double.NaN;
			}else{
				// We can get a bearing:
				
				double dPhi = Math.Log(
					Math.Tan(theta2 / 2+Math.PI/4)/Math.Tan(theta1/2+Math.PI/4)
				);
				
				if (Math.Abs(dLon) > Math.PI){
					dLon = dLon > 0 ? -(2*Math.PI-dLon) : (2*Math.PI+dLon);
				}
				
				bearing=(Math.Atan2(dLon, dPhi) * Rad2Deg + 360) % 360;
				
			}
			
			return s1;
			
		}
		
		/// <summary>The bearing between this point and another. Output is in degrees from north. Clockwise.</summary>
		public double bearing(double lon2,double lat2){
			
			double lon1=coords.longitude;
			double lat1=coords.latitude;
			
			double dLon=( (lon2-lon1) * Deg2Rad );
			double theta1 = lat1 * Deg2Rad;
			double theta2 = lat2 * Deg2Rad;
			
			double dPhi = Math.Log(
				Math.Tan(theta2 / 2+Math.PI/4)/Math.Tan(theta1/2+Math.PI/4)
			);
			
			if (Math.Abs(dLon) > Math.PI){
				dLon = dLon > 0 ? -(2*Math.PI-dLon) : (2*Math.PI+dLon);
			}
			
			return (Math.Atan2(dLon, dPhi) * Rad2Deg + 360) % 360;
			
		}
		
		/// <summary>An accurate distance between this point and the given one using the WGS84 spheroid (as the geolocation API).</summary>
		public double distanceAccurate(double lon2,double lat2,double alt2){
			
			// With thanks to http://stackoverflow.com/questions/1108965/taking-altitude-into-account-when-calculating-geodesic-distance
			
			// Calculate proper geodesics for LLA paths
			double lon1=coords.longitude;
			double lat1=coords.latitude;
			double alt1=coords.altitude;
			
			// Meeus approximation
			double f = (lat1 + lat1) / 2 * Deg2Rad;
			double g = (lat1 - lat1) / 2 * Deg2Rad;
			double l = (lon1 - lon2) / 2 * Deg2Rad;

			double sinG = Math.Sin(g);
			double sinL = Math.Sin(l);
			double sinF = Math.Sin(f);

			double s, c, w, r, d, h1, h2;
			
			// Not perfect but use the average altitude
			double a = (Eccentricity + alt1 + Eccentricity + alt2) / 2.0;
			
			sinG *= sinG;
			sinL *= sinL;
			sinF *= sinF;
			
			s = sinG * (1 - sinL) + (1 - sinF) * sinL;
			c = (1 - sinG) * (1 - sinL) + sinF * sinL;

			w = Math.Atan(Math.Sqrt(s / c));
			r = Math.Sqrt(s * c) / w;
			d = 2 * w * a;
			h1 = (3 * r - 1) / 2 / c;
			h2 = (3 * r + 1) / 2 / s;
			
			return d * (1 + (1 / EarthRadius) * (h1 * sinF * (1 - sinG) - h2 * (1 - sinF) * sinG));
		
		}
		
	}
	
	/// <summary>A linked list of location requests.</summary>
	public class PendingLocationRequest{
		
		/// <summary>Time it's waited for.</summary>
		public float Duration;
		/// <summary>Max time it'll wait for.</summary>
		public float Timeout=float.MaxValue;
		/// <summary>Next in the list.</summary>
		public PendingLocationRequest Next;
		/// <summary>Success callback.</summary>
		public GeolocationEvent Success;
		/// <summary>Error callback.</summary>
		public GeolocationErrorEvent Error;
		/// <summary>Options.</summary>
		public GeoPositionOptions Options;
		
		
		public bool Update(float deltaTime){
			
			Duration+=deltaTime;
			
			if(Duration>=Timeout){
				// Timeout!
				
				if(Error!=null){
					Error(new GeoPositionError(GeoPositionError.TIMEOUT));
				}
				
				// Done:
				return true;
				
			}
			
			// Not ready yet.
			return false;
			
		}
		
		/// <summary>Called when the location service is no longer initialising.</summary>
		public void Ready(){
			
			// Call on ready:
			Geolocation.OnReady(Success,Error,Options);
			
		}
		
	}
	
	/// <summary>Geolocation position options.</summary>
	public class GeoPositionOptions{
		
		public bool enableHighAccuracy;
		public double timeout;
		public double maximumAge;
		
	}
	
	/// <summary>Geolocation error.</summary>
	public class GeoPositionError{
		
		public const ushort PERMISSION_DENIED=1;
		public const ushort POSITION_UNAVAILABLE=2;
		public const ushort TIMEOUT=3;
		
		/// <summary>The error code.</summary>
		public ushort code;
		
		
		public GeoPositionError(ushort c){
			code=c;
		}
		
		/// <summary>Debugging user friendly message.</summary>
		public string message{
			get{
				switch(code){
					case 1:
						return "Permission denied.";
					case 2:
						return "Position unavailable.";
					case 3:
						return "Geolocation API timed out.";
				}
				
				return "Unknown";
			}
		}
		
	}
	
	/// <summary>
	/// Geolocation coords.
	/// </summary>
	public struct GeoCoordinates{
		
		/// <summary>Longitude of the device in m.</summary>
		public double longitude;
		/// <summary>Latitude of the device in m.</summary>
		public double latitude;
		/// <summary>Altitude of the device in m.</summary>
		public double altitude;
		/// <summary>Accuracy of these coords in m.</summary>
		public double accuracy;
		/// <summary>Accuracy of the altitude in m.</summary>
		public double altitudeAccuracy;
		/// <summary>Direction the device is travelling in. 0 represents north.</summary>
		public double heading;
		/// <summary>Velocity of the device in m/s.</summary>
		public double speed;
		
		
		public GeoCoordinates(double lon,double lat,double alt,double acc,double altAcc,double head,double spd){
			
			longitude=lon;
			latitude=lat;
			altitude=alt;
			accuracy=acc;
			altitudeAccuracy=altAcc;
			heading=head;
			speed=spd;
			
		}
		
	}
	
}