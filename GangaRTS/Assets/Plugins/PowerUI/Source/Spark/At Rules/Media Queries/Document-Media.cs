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
	
	/// <summary>
	/// Represents a HTML Document. UI.document is the main UI document.
	/// Use PowerUI.Document.innerHTML to set it's content.
	/// </summary>

	public partial class ReflowDocument{
		
		/// <summary>The media type of this document. Created on first use.</summary>
		private MediaType _Media;
		/// <summary>A fast lookup of the media rules on this document.</summary>
		public List<MediaRule> MediaRules;
		
		/// <summary>'Media' is created on first use if you access it. This is the cached media object.
		/// I.e. this can be null.</summary>
		public MediaType MediaIfExists{
			get{
				return _Media;
			}
		}
		
		/// <summary>The media type of this document. Created on first use.</summary>
		public MediaType Media{
			get{
				
				if(_Media==null){
					RequireMediaType();
				}
				
				return _Media;
				
			}
			set{
				_Media=value;
			}
		}
		
		/// <summary>Creates the media type if it's not already been set.</summary>
		public void RequireMediaType(){
			if(_Media==null){
				
				// Assume a screen media type:
				_Media=new ScreenMediaType(this);
				
			}
		}
		
		/// <summary>Matches the given CSS media string.</summary>
		public MediaQueryList matchMedia(string mediaQuery){
			
			// Load as a CSS value:
			Value val=Value.Load(mediaQuery);
			
			// Build the media query:
			MediaQuery mqry=MediaQuery.Load(val,0,val.Count);
			
			// Create as a list:
			return new MediaQueryList(this,mqry);
			
		}
		
	}
	
}

namespace PowerUI{
	
	public partial class Window{
		
		/// <summary>Matches the given CSS media string.</summary>
		public MediaQueryList matchMedia(string mediaQuery){
			
			// Match now!
			return document.matchMedia(mediaQuery);
			
		}
		
	}
	
}