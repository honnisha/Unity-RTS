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
using UnityEngine;
using PowerUI;
using Dom;
using Json;


namespace PowerSlide{
	
	/// <summary>
	/// A cue track. These signal pause points.
	/// Each time a pause occurs, you must use element.cue() to start it back up again.
	/// </summary>
	
	public partial class CueTrack : Track{
		
		public CueTrack(){}
		
		/// <summary>The name of this type of track. "style" and "dialogue" are common examples.</summary>
		public override string tagName{
			get{
				return "cue";
			}
		}
		
		/// <summary>Creates a slide of the correct type for this track.</summary>
		public override Slide createSlide(){
			return new CuePoint();
		}
		
	}
	
}	