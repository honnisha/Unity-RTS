//--------------------------------------
//          Blaze Rasteriser
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;


namespace Blaze{
	
	/// <summary>The delegate used.</summary>
	public delegate float OnGetDistance(float x,float y,int vertexIndex,int totalVertices);
	
	/// <summary>
	/// Blaze has a unique way of rendering distance fields - the things used to display text.
	/// Because of how it works, it has an interesting side effect - you can vary the distance spread
	/// at a vertex level. This can be used for some awesome glow effects or even for some complex terrain heightmap generation.
	/// </summary>
	
	public static class DistanceSpread{
		
		/// <summary>Have you got a custom distance spread functions? Use SetFunctions to apply this.</summary>
		public static bool Custom;
		/// <summary>The delegate to run on the inner spread. Use SetFunction to apply this.</summary>
		public static OnGetDistance GetInner;
		/// <summary>The delegate to run on the outer spread. Use SetFunction to apply this.</summary>
		public static OnGetDistance GetOuter;
		/// <summary>True if inner/outer spreads are different.</summary>
		public static bool InnerOuterDiff=false;
		/// <summary>The default inner spread.</summary>
		public static float DefaultInner=0.2f;
		/// <summary>The default outer spread.</summary>
		public static float DefaultOuter=0.2f;
		
		
		/// <summary>Sets a custom distance spread function for both inner and outer spreads.</summary>
		public static void SetFunction(OnGetDistance method){
			
			SetFunctions(method,method,(method!=null));
			
		}
		
		/// <summary>Sets a custom distance spread function for inner and outer spreads.</summary>
		public static void SetFunctions(OnGetDistance inner,OnGetDistance outer,bool hd){
			
			GetInner=inner;
			Custom=(inner!=null || outer!=null);
			
			TextureCameras.SD=!hd;
			
			InnerOuterDiff=(inner!=outer);
			
		}
		
	}

}