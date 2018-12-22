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
using Json;


namespace PowerSlide{
	
	/// <summary>
	/// Used to start dialogue (speech).
	/// </summary>
	
	public static class Dialogue{
		
		/// <summary>The location that dialogue is relative to by default (resources://Dialogue/).</summary>
		internal static Dom.Location basePath = new Dom.Location("resources://Dialogue/",null);
		
		/// <summary>
		/// Loads a set of dialogue options at the given path.
		/// </summary>
		internal static Promise open(string startPath){
			
			// Load a timeline now:
			return Timeline.open(startPath,basePath);
			
		}
		
	}
	
}

namespace PowerUI{
	
	public partial class HtmlDocument{
		
		/// <summary>
		/// Starts the dialogue at the given path. It's relative to resources://Dialogue/ by default 
		/// and ".json" is appended if it contains no dot.
		/// E.g. "joey" => "resources://Dialogue/joey.json".
		/// "Hello/joey" => "resources://Dialogue/Hello/joey.json".
		/// "cdn://...joey.json" => as is.
		/// Use {language} in your path to localise the file.
		/// Kills any already running dialogue in the document.
		/// </summary>
		/// <param name="template">The widget template to use. 
		/// Note that the widget doesn't need to be visual - it could, for example,
		/// manage a bunch of WorldUI's instead.</param>
		/// <returns>A promise which runs when the dialogue loaded and started up.</returns>
		public Promise startDialogue(string startPath,string template){
			return startDialogue(startPath,template,true);
		}
		
		/// <summary>
		/// Starts the dialogue at the given path. It's relative to resources://Dialogue/ by default 
		/// and ".json" is appended if it contains no dot.
		/// E.g. "joey" => "resources://Dialogue/joey.json".
		/// "Hello/joey" => "resources://Dialogue/Hello/joey.json".
		/// "cdn://...joey.json" => as is.
		/// Use {language} in your path to localise the file.
		/// </summary>
		/// <param name="template">The widget template to use. 
		/// Note that the widget doesn't need to be visual - it could, for example,
		/// manage a bunch of WorldUI's instead.</param>
		/// <param name="killRunning">Kills any open dialogue in the document.</param>
		/// <returns>A promise which runs when the dialogue loaded and started up.</returns>
		public Promise startDialogue(string startPath,string template,bool killRunning){
			
			if(killRunning){
				
				// Find all timelines in the document itself (and marked with 'isDialogue') and stop them:
				PowerSlide.Timeline current=PowerSlide.Timeline.first;
				
				while(current!=null){
					if(current.isDialogue && current.document==this){
						// Kill it!
						current.stop(false);
					}
					
					current=current.after;
				}
				
			}
			
			Promise p=new Promise();
			
			// Load the slides:
			PowerSlide.Dialogue.open(startPath).then(delegate(object o){
				
				// It's a timeline:
				PowerSlide.Timeline timeline=o as PowerSlide.Timeline;
				
				// Set the default template/doc:
				timeline.isDialogue=true;
				timeline.template=template;
				timeline.document=this;
				
				// Start it (which may open widgets):
				timeline.start();
				
				// Just resolve the promise here:
				p.resolve(timeline);
				
			},p);
			
			return p;
			
		}
		
		/// <summary>
		/// The same as startDialogue, only the promise this one returns runs when the dialogue is over.
		/// (It hooks up to the timelineend event triggered on the timeline itself).
		/// </summary>
		/// <param name="template">The widget template to use. 
		/// Note that the widget doesn't need to be visual - it could, for example,
		/// manage a bunch of WorldUI's instead.</param>
		/// <returns>A promise which runs when the dialogue finished.</returns>
		public Promise playDialogue(string startPath,string template){
			return playDialogue(startPath,template,true);
		}
		
		/// <summary>
		/// The same as startDialogue, only the promise this one returns runs when the dialogue is over.
		/// (It hooks up to the timelineend event triggered on the timeline itself).
		/// </summary>
		/// <param name="template">The widget template to use. 
		/// Note that the widget doesn't need to be visual - it could, for example,
		/// manage a bunch of WorldUI's instead.</param>
		/// <param name="killRunning">Kills any open dialogue in the document.</param>
		/// <returns>A promise which runs when the dialogue finished.</returns>
		public Promise playDialogue(string startPath,string template,bool killRunning){
			
			Promise p=new Promise();
			
			startDialogue(startPath,template,killRunning).then(delegate(object o){
				
				PowerSlide.Timeline timeline=(o as PowerSlide.Timeline);
				
				// Add a done event handler:
				timeline.addEventListener("timelineend",delegate(PowerSlide.SlideEvent e){
					
					// Ok!
					p.resolve(timeline);
					
				});
				
				// Catch the slides cancel event (called when it was quit early):
				timeline.addEventListener("timelinecancel",delegate(PowerSlide.SlideEvent se){
					
					// Resolve it now:
					p.reject(timeline);
					
				});
				
			},p);
			
			return p;
		}
		
	}
	
}