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
using Css;
using System.Collections;
using System.Collections.Generic;


namespace Css{
	
	public partial class ReflowDocument{
		
		/// <summary>The set of @keyframes animations on this document.</summary>
		public Dictionary<string,KeyframesRule> Animations;
		
		
		/// <summary>Gets an @keyframes animation by name. Any case.</summary>
		public KeyframesRule GetAnimation(string name){
			
			if(name==null || Animations==null){
				return null;
			}
			
			name=name.ToLower();
			
			KeyframesRule animation;
			Animations.TryGetValue(name,out animation);
			return animation;
		}
		
	}
	
}

namespace Css{
	
	public partial class ComputedStyle{
		
		/// <summary>The live keyframes animation, if there is one.</summary>
		public KeyframesAnimationInstance AnimationInstance;
		
	}
	
}



