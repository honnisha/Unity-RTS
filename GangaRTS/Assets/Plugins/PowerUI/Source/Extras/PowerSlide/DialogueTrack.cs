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
	/// A dialogue track. They're simply a list of slides. They start at either a slide (in some other track),
	/// an item (typically an NPC) or an 2D element (using the slides CSS property).
	/// </summary>
	
	public partial class DialogueTrack : Track{
		
		public DialogueTrack(){}
		
		/// <summary>The name of this type of track. "style" and "dialogue" are common examples.</summary>
		public override string tagName{
			get{
				return "dialogue";
			}
		}
		
		/// <summary>Creates a slide of the correct type for this track.</summary>
		public override Slide createSlide(){
			return new DialogueSlide();
		}
		
		/// <summary>Can this track show up as an option? Yes by default.
		/// Add a handler for the 'allow' event and use preventDefault to block.
		/// Used when e.g. the player doesn't have the skill for something.</summary>
		public bool isAllowed{
			get{
				
				// Create the event:
				SlideEvent e=createEvent("allow");
				
				// Run it:
				return dispatchEvent(e);
				
			}
		}
		
	}
	
}	