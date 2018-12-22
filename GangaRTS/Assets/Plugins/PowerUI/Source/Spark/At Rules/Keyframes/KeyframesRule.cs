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
using System.Collections;
using System.Collections.Generic;
using Css.Units;


namespace Css{
	
	/// <summary>
	/// A loaded @keyframes animation.
	/// These are available from Document.Animations; a dictionary from animation name to these objects.
	/// Note that the Document.Animations entry is defined within this file at the bottom.
	/// </summary>
	
	public class KeyframesRule : Rule{
		
		/// <summary>Name.</summary>
		public string Name;
		/// <summary>The raw CSS value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		/// <summary>The number of frames in this animation.</summary>
		public int FrameCount;
		/// <summary>The frames set.</summary>
		public KeyframesKeyframe[] Frames;
		
		
		public KeyframesRule(StyleSheet sheet,Css.Value rawValue,string name,List<Rule> frames){
			
			Name=name;
			ParentSheet=sheet;
			RawValue=rawValue;
			
			SortedDictionary<float,KeyframesKeyframe> sortedSet=new SortedDictionary<float,KeyframesKeyframe>();
			
			// Sort the set by % order. Exceptions for from and to.
			foreach(Rule rule in frames){
				
				StyleRule styleRule=rule as StyleRule;
				
				if(styleRule==null){
					continue;
				}
				
				// Create the frame:
				KeyframesKeyframe frame=new KeyframesKeyframe(styleRule.Style);
				
				if(frame.Style==null){
					// Empty keyframe - ignore it.
					continue;
				}
				
				// What is its percentage position?
				Css.Value cssValue=styleRule.Selector.Value[0];
				
				float percF=0f;
				
				if(cssValue.Type==ValueType.Text){
					
					// Get as text:
					string text=cssValue.Text.ToLower();
					
					if(text=="from"){
						percF=0f;
					}else if(text=="to"){
						percF=1f;
					}else{
						continue;
					}
					
				}else{
					percF=cssValue.GetRawDecimal() / 100f;
				}
				
				// Apply the time:
				frame.Time=percF;
				
				// Add to Frames set in order of percF.
				sortedSet[percF]=frame;
				
			}
			
			// Create the frames set:
			FrameCount=sortedSet.Count;
			Frames=new KeyframesKeyframe[FrameCount];
			int index=0;
			
			foreach(KeyValuePair<float,KeyframesKeyframe> kvp in sortedSet){
				
				if(index!=0){
					
					// Get the frame before:
					KeyframesKeyframe before=Frames[index-1];
					
					// Figure out the gap between them:
					float delay=kvp.Value.Time - before.Time;
					
					// Update the delay that occurs after it:
					before.AfterDelay=delay;
					
					// And update the delay that occurs before this:
					kvp.Value.BeforeDelay=delay;
					
				}else{
					
					// Apply before:
					kvp.Value.BeforeDelay=kvp.Value.Time;
					
				}
				
				Frames[index]=kvp.Value;
				index++;
				
			}
			
		}
		
		/// <summary>Gets the given frame by ID.</summary>
		public KeyframesKeyframe GetFrame(int id){
			return Frames[id];
		}
		
		/// <summary>The CSS text of this rule.</summary>
		public string cssText{
			get{
				return RawValue.ToString();
			}
			set{
				throw new NotImplementedException("cssText is read-only on rules. Set it for a whole sheet instead.");
			}
		}
		
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet parentStyleSheet{
			get{
				return ParentSheet;
			}
		}
		
		/// <summary>Rule type.</summary>
		public int type{
			get{
				return 7;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
			// Put it into the animations set in the document:
			if(document.Animations==null){
				
				// Create it:
				document.Animations=new Dictionary<string,KeyframesRule>();
				
			}
			
			document.Animations[Name.ToLower()] = this;
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
			if(document.GetAnimation(Name)==this){
				// Remove it:
				document.Animations.Remove(Name.ToLower());
			}
			
		}
		
	}
	
}