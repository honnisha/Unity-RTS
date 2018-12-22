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
using UnityEngine;
using Dom;
using Spa;


namespace PowerUI{
	
	/// <summary>
	/// Used by both GIFs and sprite animations.
	/// </summary>
	public partial class SpriteEvent : Dom.Event{
		
		/// <summary>The SPAInstance.</summary>
		public SPAInstance instance;
		
		/// <summary>The total time that the SPA has been running for.</summary>
		public float elapsedTime{
			get{
				return instance.Elapsed;
			}
		}
		
		/// <summary>The SPA being played.</summray>
		public SPA spa{
			get{
				return instance.Animation;
			}
		}
		
		/// <summary>Does this sprite loop?</summary>
		public bool loop{
			get{
				return instance.Loop;
			}
			set{
				instance.Loop=value;
			}
		}
		
		public SpriteEvent(string type):base(type){}
		public SpriteEvent(string type,object init):base(type,init){}
		
		public override void Setup(object init){
			
			if(init==null){
				return;
			}
			
		}
		
	}
	
}

namespace Dom{
	
	/// <summary>
	/// An event target can receive events and have event handlers.
	/// <summary>
	
	public partial class EventTarget{
		
		public void addEventListener(string name,Action<PowerUI.SpriteEvent> method){
			addEventListener(name,new EventListener<PowerUI.SpriteEvent>(method));
		}
		
	}
	
}