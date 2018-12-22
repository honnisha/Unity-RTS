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

using PowerSlide;


namespace PowerUI{
	
	/// <summary>
	/// An object which holds and retrieves different types of audio
	/// such as for synthesis or various formats (mp3, ogg etc).
	/// </summary>
	
	public partial class AudioPackage:ITimingLeader{
		
		public float GetCurrentTime(){
			return Contents.CurrentTime;
		}
		
		public float GetDuration(){
			return Contents.Duration;
		}
		
	}
	
}