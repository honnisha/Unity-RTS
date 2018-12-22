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
using Dom;
using PowerUI;
using Css;


namespace PowerSlide{
	
	/// <summary>
	/// A timing leader is something which PowerSlide will follow whilst animating.
	/// For example, it will follow the lead of a playing audio clip to make sure
	/// dialogue on the UI is perfectly in sync with the playing audio.
	/// </summary>
	
	public interface ITimingLeader{
		
		/// <summary>Interrupts the leader to indicate that the slide had a duration and it was reached.</summary>
		void Stop();
		
		/// <summary>The clip length in seconds. Returns -1 if it's unknown.</summary>
		float GetDuration();
		
		/// <summary>The current time in seconds.</summary>
		float GetCurrentTime();
		
	}
	
}