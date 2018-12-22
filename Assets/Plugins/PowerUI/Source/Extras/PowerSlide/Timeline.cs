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
using PowerUI;
using Json;
using System.Collections;
using System.Collections.Generic;
using Css;


namespace PowerSlide{
	
	/// <summary>
	/// A loaded slides file. They contain one or more tracks (usually just one).
	/// A track (e.g a style track or a dialogue track) is a series of slides.
	/// </summary>
	
	public partial class Timeline : Dom.EventTarget{
		
		public const short TIMELINE_LOADING = 0;
		public const short TIMELINE_INVALID = 1;
		public const short TIMELINE_UNAVAILABLE = 2;
		public const short TIMELINE_READY = 3;
		public const short TIMELINE_STARTED = 4;
		public const short TIMELINE_ENDED = 5;
		public const short TIMELINE_CANCELLED = 6;
		
		/// <summary>Loads a slides JSON file at the given path (relative to the given location).</summary>
		internal static Promise open(string startPath,Dom.Location relativeTo){
			return open(startPath,relativeTo,null);
		}
		
		/// <summary>Loads a slides JSON file at the given path (relative to the given location)
		/// optionally into the given timeline.</summary>
		internal static Promise open(string startPath,Dom.Location relativeTo,Timeline into){
			
			// Use this to establish when it's ready:
			Promise p=new Promise();
			
			// Append .json if it's needed:
			if(!startPath.Contains(".") && !startPath.Contains("://")){
				
				startPath+=".json";
				
			}
			
			// Localise it:
			startPath=startPath.Replace("{language}",UI.Language);
			
			// Load the file now:
			DataPackage req=new DataPackage(
				startPath,
				relativeTo
			);
			
			if(into==null){
				into=new Timeline(null);
			}
			
			// Delegate:
			req.onload=delegate(UIEvent e){
				
				try{
					// Response is a block of options:
					JSObject options=JSON.Parse(req.responseText);
					
					// Load it up and run the callback:
					into.load(options);
					
				}catch{
					// Invalid json.
					p.reject(into);
					return;
				}
				
				// Run the promise now:
				p.resolve(into);
				
			};
			
			req.onerror=delegate(UIEvent e){
				into.status_=TIMELINE_UNAVAILABLE;
				p.reject(into);
			};
			
			// Send!
			req.send();
			
			return p;
		}
		
		/// <summary>The timeline status.</summary>
		private short status_;
		/// <summary>True if this is a dialogue timeline.</summary>
		internal bool isDialogue=false;
		/// <summary>The default language for this timeline.</summary>
		public string defaultLanguage="en";
		/// <summary>An optional declared duration. Overriden by the slides-duration value.
		/// If not declared at all then it defaults to 1.</summary>
		public Css.Value duration;
		/// <summary>All the tracks in this timeline.</summary>
		public Track[] tracks;
		/// <summary>Source URL.</summary>
		public string src;
		/// <summary>Default template to use.</summary>
		public string template;
		
		/// <summary>A time leading slide (usually one with audio/ video) 
		/// which this timeline will follow for timing purposes.</summary>
		public Slide timingLeader;
		/// <summary>The ComputedStyle that this was applied to (can be null).</summary>
		public ComputedStyle style;
		/// <summary>A list of event listeners that *must* be destroyed 
		/// when either this timeline is killed or is un-paused.</summary>
		internal List<CueElementData> cueElements;
		/// <summary>If this timeline is using a widget, the widget itself.
		/// Used by dialogue.</summary>
		public Widgets.Widget currentWidget;
		/// <summary>Host HTML document.</summary>
		public HtmlDocument document;
		
		
		public Timeline(ComputedStyle style){
			this.style=style;
			
			if(style!=null){
				document=style.document as HtmlDocument;
			}
			
		}
		
		/// <summary>The status of this timeline.</summary>
		public short status{
			get{
				return status_;
			}
		}
		
		/// <summary>The node that this timeline is on.</summary>
		public Dom.Node node{
			get{
				if(style!=null){
					return style.Element;
				}
				
				return document;
			}
		}
		
		/// <summary>Gets a slide by its unique ID.</summary>
		public Slide getSlide(int uniqueID){
			
			// For each track..
			for(int i=0;i<tracks.Length;i++){
			
				Track track=tracks[i];
				
				// For each slide..
				for(int s=0;s<track.slides.Length;s++){
					
					// Get it:
					Slide slide=track.slides[s];
					
					// Get by unique ID:
					Slide res=slide.getSlideByID(uniqueID);
					
					if(res!=null){
						// Got it!
						return res;
					}
					
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Opens a widget, passing this timeline as a global.</summary>
		internal Widgets.Widget openWidget(string template){
			
			if(document==null || template==null){
				Dom.Log.Add("PowerSlide requested to open a widget without a document/ template. Request was ignored.");
				return null;
			}
			
			if(currentWidget!=null){
				
				if(currentWidget.Type==template){
					// Unchanged template.
					return currentWidget;
				}
				
				currentWidget.close();
				currentWidget=null;
			}
			
			// Open it now:
			currentWidget=document.widgets.open(template,null,"timeline",this);
			
			return currentWidget;
		}
		
		
		/// <summary>
		/// Tidies up any event handlers added by cue nodes.
		/// </summary>
		public void clearCues(){
			
			if(cueElements==null){
				return;
			}
			
			foreach(CueElementData ced in cueElements){
				
				Dom.Element node=ced.target;
				
				// Remove it:
				node.removeEventListener(ced.eventName,ced.eventListener);
				
			}
			
			// Ok!
			cueElements=null;
			
		}
		
		/// <summary>Resets this timeline so it can start over.</summary>
		public void reset(){
			
			// Clear cues:
			clearCues();
			
			if(first==null && updater!=null){
				
				// Stop the updater:
				updater.Stop();
				updater=null;
				
			}
			
			// Kill any running slides:
			Slide current=firstRunning;
			
			while(current!=null){
				
				// Done!
				current.end();
				paused=false;
				
				current=current.nextRunning;
			}
			
			// Widget:
			if(currentWidget!=null){
				currentWidget.close();
				currentWidget=null;
			}
			
			// Clear various settings:
			currentTime=0f;
			backwards=false;
			direction=KeyframesAnimationDirection.Forward;
			repeatCount=1;
			status_=TIMELINE_LOADING;
			
		}
		
		/// <summary>Loads a timeline from the given JSON.</summary>
		public void load(Json.JSObject json){
			
			if(started){
				// Reset if needed:
				reset();
			}
			
			duration=null;
			
			// First, check for the 'tracks' field:
			Json.JSObject trackData=json["tracks"];
			
			if(trackData==null){
				
				// Considered to be a single track.
				// Try loading it now:
				Track track=Track.loadFromJson(this,json);
				
				if(track==null){
					
					// Empty:
					tracks=new Track[0];
					
				}else{
					// We have at least 1 entry in a track.
					tracks=new Track[1];
					
					// We now need to detect what kind of track it is based on what it provides.
					// If it's a style track, the first element 
					tracks[0]=track;
				}
				
			}else{
				
				// Optional default language (must be before tracks load):
				string lang=json.String("lang");
				
				if(!string.IsNullOrEmpty(lang)){
					defaultLanguage=lang.Trim().ToLower();
				}
				
				// Must be an indexed array:
				var trackArray=trackData as Json.JSIndexedArray;
				
				if(trackArray==null){
					loadFailed("'tracks' must be an indexed array.",this);
				}
				
				// Fully defined timeline.
				int length=trackArray.length;
				
				// Create track set:
				tracks=new Track[length];
				
				// Load each one:
				for(int i=0;i<length;i++){
					
					// Load the track now:
					tracks[i]=Track.loadFromJson(this,trackArray[i]);
					
				}
				
				// Optional duration:
				string durationText=json.String("duration");
				
				if(!string.IsNullOrEmpty(durationText)){
					
					// Load it as a CSS value:
					duration=Css.Value.Load(durationText);
					
				}
				
			}
			
			status_=TIMELINE_STARTED;
		}
		
		/// <summary>Called when a timeline fails to load.</summary>
		internal static void loadFailed(string message,Timeline timeline){	
			
			string src=null;
			
			if(timeline!=null){
				src=timeline.src;
				timeline.status_=TIMELINE_INVALID;
			}
			
			message="PowerSlide timeline ";
			
			if(src!=null){
				message+="(at '"+src+"') ";
			}
			
			message+="failed to load: "+message;
			
			throw new Exception(message);
		}
		
		/// <summary>The maximum defined duration.</summary>
		public float maxDefinedDuration{
			get{
				
				float max=0f;
				
				for(int i=0;i<tracks.Length;i++){
					
					// Get the duration:
					float dur=tracks[i].definedDuration;
					
					if(dur>max){
						max=dur;
					}
					
				}
				
				return max;
			}
		}
		
		/// <summary>Gets the first track by the name of the track. "style" or "dialogue" for example.</summary>
		public Track getTrackByTagName(string name){
			
			// Ensure it's lowercase:
			name=name.ToLower();
			
			for(int i=0;i<tracks.Length;i++){
				
				Track track=tracks[i];
				
				if(track.tagName==name){
					return track;
				}
				
			}
			
			return null;
			
		}
		
		
		/// <summary>Gets all tracks by the name of the track. "style" or "dialogue" for example.</summary>
		public List<Track> getTracksByTagName(string name){
			
			List<Track> results=new List<Track>();
			
			getTracksByTagName(name,results);
			
			return results;
			
		}
		
		/// <summary>Gets all tracks by the name of the track. 
		/// "style" or "dialogue" for example. Adds the results to the given set.</summary>
		public void getTracksByTagName(string name,List<Track> results){
			
			// Ensure it's lowercase:
			name=name.ToLower();
			
			for(int i=0;i<tracks.Length;i++){
				
				Track track=tracks[i];
				
				if(track.tagName==name){
					results.Add(track);
				}
				
			}
			
		}
		
	}
	
}