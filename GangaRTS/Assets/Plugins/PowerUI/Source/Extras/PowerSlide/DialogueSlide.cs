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
	/// A dialogue slide. Essentially this is like a single cue card in a series of dialogue.
	/// </summary>
	
	public class DialogueSlide : Slide{
		
		/// <summary>
		/// Loads the given json as a set of options.
		/// </summary>
		public static DialogueSlide[] loadOptions(JSObject json,DialogueSlide parent){
			
			if(json==null){
				return null;
			}
			
			// If it's just a string, then we have just a goto.
			if(json is JSValue){
				
				DialogueSlide slide=new DialogueSlide();
				slide.slidesToGoTo=json.ToString();
				slide.track=parent.track;
				return new DialogueSlide[1]{slide};
				
			}
			
			// Create the set:
			DialogueSlide[] set=new DialogueSlide[json.length];
			
			foreach(KeyValuePair<string,JSObject> kvp in json){
				
				JSObject entry=kvp.Value;
				
				// Get the index:
				int index;
				int.TryParse(kvp.Key,out index);
				
				// Create the option:
				DialogueSlide slide=new DialogueSlide();
				
				// Template:
				slide.template=entry.String("template");
				
				// Markup:
				slide.markup=entry.String("markup");
				
				// Goto:
				slide.slidesToGoTo=entry.String("goto");
				slide.index=index;
				slide.track=parent.track;
				slide.parent=parent;
				
				// Add it:
				set[index]=slide;
				
			}
			
			return set;
			
		}
		
		/// <summary>True if this slide waits for a cue. It's true if the slide does not have a defined duration.
		/// (because progression of dialogue is almost never regular enough for automatic durations to be usable).</summary>
		public bool waitForCue;
		/// <summary>
		/// One or more speakers who say this. Note that a single 'speaker' object can
		/// be multiple actual speakers. For example, it might be referring to a 'clan member' object.
		/// In the scene, there could be 5 clan members.
		/// </summary>
		public Speaker[] speakers;
		/// <summary>
		/// The parent slide of an option slide.
		/// </summary>
		public DialogueSlide parent;
		/// <summary>
		/// An optional template to use.
		/// </summary>
		public string template;
		/// <summary>
		/// When this slide runs, the set of slides to go to next.
		/// </summary>
		public string slidesToGoTo;
		/// <summary>
		/// Path to an audio file.
		/// If an audio file runs throughout the duration of dialogue, put it in a second dialogue track.
		/// </summary>
		public string audioFilePath;
		/// <summary>
		/// The mood of the speaker. Can be extracted from the SSML or directly declared in the slide.
		/// Note that plain text adopts the mood of the previous slide.
		/// </summary>
		public string mood;
		/// <summary>
		/// The raw options provided by this slide, if it is an options slide.
		/// </summary>
		private DialogueSlide[] options_;
		/// <summary>
		/// Options can potentially be "remote" so dialogue trees can be easily extended.
		/// These are the options expanded fully (i.e. remote URLs are loaded).
		/// </summary>
		private DialogueSlide[] expandedOptions_;
		/// <summary>
		/// The markup to show.
		/// </summary>
		public string markup;
		
		
		/// <summary>Gets a slide by a unique ID.</summary>
		public override Slide getSlideByID(int uniqueID){
			
			if(this.uniqueID==uniqueID){
				return this;
			}
			
			// Options?
			if(options_!=null){
				
				for(int i=0;i<options_.Length;i++){
					
					// Get from the option:
					Slide r=options_[i].getSlideByID(uniqueID);
					
					if(r!=null){
						return r;
					}
					
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Begins playing any audio</summary>
		private void playAudio(){
			
			// Get a full path (also uses {language} and {mood} and is relative to resources://Dialogue):
			
			// Localise it:
			string audioPath=audioFilePath.Replace("{language}",UI.Language);
			
			// Load the file now:
			AudioPackage req=new AudioPackage(
				audioPath,
				new Dom.Location("resources://Dialogue/",null)
			);
			
			// Apply the audio package to the slides set:
			timingLeadBy(req);
			
			req.onload=delegate(UIEvent e){
				
				// Play immediately - the system has been waiting for it
				// (if it's from resources then the delay would've been generally unnoticeable):
				req.Start(track.timeline.node);
				
			};
			
			req.onerror=delegate(UIEvent e){
				
				// Skip this frame:
				Dom.Log.Add("Audio file unavailable - PowerSlide will playback without it.");
				
				endTimingLead();
				
			};
			
			// Send it off:
			req.send();
			
		}
		
		/// <summary>Swaps {mood} with the mood of this slide.</summary>
		public string applyMood(string url){
			
			if(url==null){
				return null;
			}
			
			if(mood==null){
				return url.Replace("{mood}","neutral");
			}
			
			return url.Replace("{mood}",mood);
			
		}
		
		/// <summary>The number of speakers. Checks if speakers is null.</summary>
		public int speakerCount{
			get{
				if(speakers==null){
					return 0;
				}
				
				return speakers.Length;
			}
		}
		
		/// <summary>The complete options set.</summary>
		public DialogueSlide[] options{
			get{
				// No remote ones for now:
				return options_;
			}
		}
		
		/// <summary>A dialogue slide is classed as cued if it waits for a cue and it's not an options menu.
		/// Note that options menu's are cued, but you almost never want to visually display 
		/// something like 'click to continue' on them.</summary>
		public bool cued{
			get{
				return waitForCue && !isOptions;
			}
		}
		
		/// <summary>This slide is now starting.</summary>
		internal override void start(){
			base.start();
			
			Timeline tl=track.timeline;
			
			// Display this dialogue slide now!
			
			// Any audio?
			if(!string.IsNullOrEmpty(audioFilePath)){
				
				// Play the audio now.
				// Note that PowerSlide will follow the lead of the audio engine.
				playAudio();
				
			}
			
			// Get the template to use:
			string templateToUse=(template==null)? tl.template:template;
			
			// Open the widget (which closes the prev one for us):
			Widgets.Widget widget=tl.openWidget(templateToUse);
			
			if(widget!=null){
				// Trigger a dialogue start event:
				SlideEvent s=new SlideEvent("dialoguestart",null);
				s.slide=this;
				tl.dispatchEvent(s);
			}
			
			if(waitForCue){
				
				// Wait! Advance to the end of the slide now too 
				// (because it has an auto duration which doesn't have any meaning).
				tl.setPause(true);
				
				if(tl.backwards){
					tl.currentTime=computedStart;
				}else{
					tl.currentTime=computedEnd;
				}
				
			}
			
		}
		
		/// <summary>This dialogue is now offscreen.</summary>
		internal override void end(){
			base.end();
			
			// Trigger dialogue end event if we have a widget:
			Timeline tl=track.timeline;
			
			// Trigger a dialogue end event:
			SlideEvent s=new SlideEvent("dialogueend",null);
			s.slide=this;
			tl.dispatchEvent(s);
			
			if(tl.currentWidget==null){
				return;
			}
			
			// Close the widget if this is the last slide non-ignored slide.
			bool last=true;
			
			for(int i=index+1;i<track.slides.Length;i++){
				
				if(!track.slides[i].ignore){
					last=false;
					break;
				}
				
			}
			
			if(last){
				// Close the widget now:
				tl.currentWidget.close();
				tl.currentWidget=null;
			}
			
		}
		
		/// <summary>True if this is an options slide.</summary>
		public bool isOptions{
			get{
				return options_!=null;
			}
		}
		
		/// <summary>Gets a value from a localised set at the given language code.
		/// Optionally falls back onto the default language choice (usually 'en').</summary>
		private string getFromSet(string language,bool fallbackOnDefault,Dictionary<string,string> set){
			
			if(language==null){
				// Default language:
				language=track.timeline.defaultLanguage;
			}
			
			language=language.Trim().ToLower();
			
			if(set==null){
				return null;
			}
			
			string value;
			if(!set.TryGetValue(language,out value)){
				
				if(fallbackOnDefault){
					
					// Use default language:
					set.TryGetValue(track.timeline.defaultLanguage,out value);
					
				}
				
			}
			
			return value;
		}
		
		/// <summary>Uses the speaker set from a previous slide which defined them.</summary>
		private void usePrevious(bool mood){
			
			// Step backwards until we hit one
			// (because of how load works, this should *always* stop at the previous slide).
			for(int i=index-1;i>=0;i--){
				
				DialogueSlide slide=track.slides[i] as DialogueSlide;
				
				if(slide!=null && slide.speakers!=null){
					
					speakers=slide.speakers;
					
					if(mood){
						this.mood=slide.mood;
					}
					
					break;
					
				}
				
			}
			
		}
		
		public override void load(JSObject json){
			
			// Was duration set?
			waitForCue=(json["duration"]==null);
			
			// If it's just a string, then we've only got markup here:
			// ["Hello","I'm Dave"]
			if(json is JSValue){
				
				// Use the previous slide to obtain the speakers and mood.
				usePrevious(true);
				
				// markup only:
				markup=json.ToString();
				
				return;
			}
			
			// Template:
			template=json.String("template");
			
			// Markup:
			markup=json.String("markup");
			
			// Raw options:
			options_=loadOptions(json["options"],this);
			
			if(options_!=null){
				// If options are present we'll almost always be waiting for a cue 
				// (from the player when they select an option).
				waitForCue=true;
			}
			
			// Audio track:
			audioFilePath=json.String("audio");
			
			// Mood:
			mood=json.String("mood");
			
			// Allow overriding WaitForCue:
			string wfc=json.String("wait-for-cue");
			
			if(wfc=="false"){
				waitForCue=false;
			}else if(wfc=="true"){
				waitForCue=true;
			}
			
			// Speakers:
			JSObject speakers=json["speakers"];
			
			if(speakers==null){
				speakers=json["speaker"];
			}
			
			if(speakers==null){
				
				// Use previous speakers (but not previous mood):
				usePrevious(false);
				
			}else{
				
				if(markup==null){
					// Only a speaker slide - ignore it:
					ignore=true;
				}
				
				JSArray speakerSet=speakers as JSArray;
				
				if(speakerSet==null){
					
					// Just one.
					loadSpeaker(0,speakers);
					
				}else if(speakerSet.IsIndexed){
					
					// Multiple speakers.
					this.speakers=new Speaker[speakers.length];
					
					// For each one..
					foreach(KeyValuePair<string,JSObject> kvp in speakerSet.Values){
						
						// index is..
						int index;
						int.TryParse(kvp.Key,out index);
						
						// Set it up:
						loadSpeaker(index,kvp.Value);
						
					}
					
				}else{
					
					// Just one (but as an object, with a type).
					loadSpeaker(0,speakers);
					
				}
				
			}
			
			// Load everything else:
			base.load(json);
			
		}
		
		/// <summary>Sets up a speaker at the given index in the Speakers set.</summary>
		internal void loadSpeaker(int index,JSObject data){
			
			if(speakers==null){
				speakers=new Speaker[1];
			}
			
			// Load the info:
			SpeakerType type;
			string reference=Speaker.LoadReference(data,out type);
			
			// Create and add:
			Speaker speaker=null;
			
			if(Speaker.OnGetInfo!=null){
				speaker=Speaker.OnGetInfo(this,type,reference);
			}
			
			if(speaker==null){
				speaker=new Speaker();
			}
			
			speaker.Type=type;
			speaker.Reference=reference;
			
			speakers[index]=speaker;
			
		}
		
	}
	
}