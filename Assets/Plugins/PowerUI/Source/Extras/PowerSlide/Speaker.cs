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


namespace PowerSlide{
	
	/// <summary>
	/// Resolves the given type/id into a speaker.
	/// </summary>
	public delegate Speaker OnSpeakerDelegate(DialogueSlide slide,SpeakerType type,string id);
	
	/// <summary>
	/// A speaker. Either an item (which includes NPCs), an item instance or a user.
	/// Note that if there are multiple instances of an item in the scene, all of them are selected.
	/// </summary>
	
	public class Speaker{
		
		/// <summary>
		/// Used when resolving the information for a speaker (their name etc).
		/// </summary>
		public static OnSpeakerDelegate OnGetInfo;
		
		/// <summary>Loads the speaker meta from the given JSON data.</summary>
		public static string LoadReference(JSObject data,out SpeakerType type){
			
			string typeName;
			string reference=null;
			type=SpeakerType.Other;
			
			if(data is JSValue){
				
				reference=data.ToString();
				typeName=reference.ToLower();
				
				if(typeName=="player" || typeName=="user"){
					type=SpeakerType.Player;
					reference=null;
				}else if(typeName=="system"){
					type=SpeakerType.System;
					reference=null;
				}else{
					// It's the reference value:
					reference=typeName;
				}
				
			}else{
				
				// Load the type:
				typeName=data.CaseString("type",false);
				
				if(typeName=="user" || typeName=="player"){
					type=SpeakerType.Player;
				}else if(typeName=="system"){
					type=SpeakerType.System;
				}else{
					type=SpeakerType.Other;
				}
				
				// Ref:
				reference=data.String("id");
				
				// Check for 'self' or 0:
				if(type==SpeakerType.Player){
					
					string referenceLC=reference.ToLower().Trim();
					
					if(referenceLC=="self" || referenceLC=="0"){
						// Got it! Just use null:
						reference=null;
					}
					
				}
				
			}
			
			return reference;
			
		}
		
		/// <summary>
		/// Typically an ID but can also be a custom reference used when resolving who the speaker is.
		/// </summary>
		public string Reference;
		/// <summary>The type of speaker.</summary>
		public SpeakerType Type;
		/// <summary>The full name of this speaker.</summary>
		public string FullName;
		/// <summary>The URL of a chathead. Use {$mood} to include emotion (e.g. happy, sad etc).</summary>
		public string ChatHeadUrl;
		
		
		/// <summary>True if this speaker represents the current user. Use 'self', null or 0 as your reference.</summary>
		public bool IsCurrentUser{
			get{
				return (Type==SpeakerType.Player && Reference==null);
			}
		}
		
	}
	
	/// <summary>
	/// The type of a speaker.
	/// </summary>
	public enum SpeakerType{
		/// <summary>Referencing some player. Use 'self'/null/0 to refer to *this* player.</summary>
		Player,
		/// <summary>A system speaker. Acts like a narrator.</summary>
		System,
		/// <summary>Some custom speaker type.</summary>
		Other
	}
	
}