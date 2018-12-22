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
using PowerSlide;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the timeline: or slides: composite css property.
	/// </summary>
	
	public class TimelineCompProperty:CssCompositeProperty{
		
		public override string[] GetProperties(){
			return new string[]{"slides","timeline"};
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			Css.Value name=null;
			Css.Value duration=null;
			Css.Value timing=null;
			Css.Value delay=null;
			Css.Value itCount=null;
			Css.Value direction=null;
			
			// Get the count:
			int count=value==null? 0 : value.Count;
			
			for(int i=0;i<count;i++){
				
				// Get the value:
				Css.Value innerValue=value[i];
				
				// If it's a time..
				if(innerValue is Css.Units.TimeUnit){
					
					// First or second time?
					if(duration==null){
						duration=innerValue;
						continue;
					}else if(delay==null){
						delay=innerValue;
						continue;
					}
					
				}
				
				// Just a number? Iteration count:
				if(itCount==null && innerValue is Css.Units.IntegerUnit){
					
					itCount=innerValue;
					continue;
					
				}
				
				// URL?
				if(name==null && innerValue is Css.Functions.UrlFunction){
					name=innerValue;
					continue;
				}
				
				// Function?
				if(timing==null && innerValue is Css.CssFunction){
					
					timing=innerValue;
					continue;
					
				}
				
				// Textual name:
				string text=innerValue.Text;
				
				// infinite etc.
				if(text=="normal" || text=="reverse" || text=="alternate" || text=="alternate-reverse"){
					
					direction=innerValue;
					continue;
					
				}
				
				if(text=="none" || text=="forwards" || text=="backwards" || text=="both"){
					
					// fillMode=innerValue;
					continue;
					
				}
				
				if(text=="running" || text=="paused"){
					
					// playState=innerValue;
					continue;
					
				}
				
				if(text=="infinite"){
					
					itCount=innerValue;
					continue;
					
				}
				
				// Timing function keywords:
				if(timing==null && innerValue is Css.CssKeyword){
					
					// Assume it's a timing function:
					timing=innerValue;
					continue;
					
				}
				
				if(name==null){
					name=innerValue;
				}
				
			}
			
			styleBlock.SetComposite("timeline-duration",duration,value);
			styleBlock.SetComposite("timeline-timing-function",timing,value);
			styleBlock.SetComposite("timeline-delay",delay,value);
			styleBlock.SetComposite("timeline-iteration-count",itCount,value);
			styleBlock.SetComposite("timeline-direction",direction,value);
			// styleBlock.SetComposite("timeline-fill-mode",fillMode,value);
			// styleBlock.SetComposite("timeline-play-state",playState,value);
			
			// Declaring name last prevents weird jitters:
			styleBlock.SetComposite("timeline-name",name,value);
		}
		
	}
	
}

namespace Css{
	
	public partial class ElementStyle{
		
		/// <summary>The PowerSlide CSS property. Works just like "animation" does.</summary>
		public string slides{
			set{
				Set("slides",value);
			}
			get{
				return GetString("slides");
			}
		}
		
		/// <summary>The PowerSlide CSS property. Works just like "animation" does.</summary>
		public string timeline{
			set{
				Set("timeline",value);
			}
			get{
				return GetString("timeline");
			}
		}
		
	}
	
}



