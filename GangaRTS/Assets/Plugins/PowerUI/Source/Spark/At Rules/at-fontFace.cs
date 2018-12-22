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
using InfiniText;
using Css.Units;
using PowerUI;


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the font-face rule.
	/// </summary>
	
	public class FontFaceRule:CssAtRule, Rule{
		
		/// <summary>The style for this font face.</summary>
		public Style style;
		/// <summary>The document containing this font-face rule.</summary>
		public ReflowDocument Document{
			get{
				return ParentSheet.document;
			}
		}
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		public override string[] GetNames(){
			return new string[]{"font-face"};
		}
		
		public override CssAtRule Copy(){
			FontFaceRule at=new FontFaceRule();
			at.style=style;
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet sheet,Css.Value value){
			
			// Grab the sheet:
			ParentSheet=sheet;
			RawValue=value;
			
			// Simply read a block like normal:
			Css.Value blk=value[1];
			
			// Grab it as a block unit:
			SelectorBlockUnit block=blk as SelectorBlockUnit;
			
			if(block==null){
				// Broken font-face :(
				return null;
			}
			
			// Grab the style:
			this.style=block.Style;
			
			// Grab the src:
			Value src=this.style["src"];
			
			// Is it set?
			if(src==null){
				return null;
			}
			
			// Get the one with a format of otf or ttf:
			Value specificFormat=src.GetEntryWithAttribute(
				"format",
				"otf",
				"ttf",
				"truetype",
				"opentype"
			);
			
			if(specificFormat==null){
				
				// Just a url()?
				if(src.IsCommaArray){
					
					//  url(), url(),.. (may contain formats)
					
					// Get the first entry with no format attribute:
					specificFormat=src.GetEntryWithoutAttribute("format");
					
				}else{
					
					// Either just url() or url() format(unsupported)
					
					// Check if it's the first form:
					if(src["format"]==null){
						
						// Yep, just a url. We'll try using it.
						specificFormat=src;
						
					}
					
				}
				
			}
			
			if(specificFormat!=null){
				LoadFont(specificFormat);
			}
			
			return this;
			
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
				return 5;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
		}
		
		private bool LoadFont(Value value){
			
			Value path=value["url"];
			string pathValue;
			
			if(path!=null){
				// Get the text value:
				pathValue=path.Text;
			}else{
				// Read the raw value:
				pathValue=value.Text;
			}
			
			DataPackage package=new DataPackage(pathValue,Document.basepath);
			
			package.onload=delegate(UIEvent e){
				
				// Load the face - it will by default add to it's "native" family:
				FontFace loaded=FontLoader.Load(package.responseBytes);
				
				if(loaded==null || loaded.Family==null){
					Dom.Log.Add("@font-face error: invalid font file at "+package.location.absolute);
					return;
				}
				
				// Got any weight, stretch or style overrides?
				Css.Value styleValue=style["font-style"];
				Css.Value weightValue=style["font-weight"];
				Css.Value stretchValue=style["font-stretch"];
				
				if(styleValue!=null || weightValue!=null || stretchValue!=null){
					
					// Yep!
					
					// New style value:
					int styleV=(styleValue==null) ? loaded.Style : styleValue.GetInteger(null,Css.Properties.FontStyle.GlobalProperty);
					
					// New weight value:
					int weight=(weightValue==null) ? loaded.Weight : weightValue.GetInteger(null,Css.Properties.FontWeight.GlobalProperty);
					
					// New stretch value:
					int stretch=(stretchValue==null) ? loaded.Stretch : stretchValue.GetInteger(null,Css.Properties.FontStretch.GlobalProperty);
					
					// Update the flags:
					loaded.SetFlags(styleV,weight,stretch);
					
				}
				
				Value family=style["font-family"];
				
				// Grab the family name:
				string familyName;
				
				if(family==null || family.Text==null){
				
					familyName=loaded.Family.Name;
				
				}else{
					
					familyName=family.Text.Trim();
					
				}
				
				// Add as an active font:
				Dictionary<string,DynamicFont> fonts=Document.ActiveFonts;
				
				DynamicFont dFont;
				if(!fonts.TryGetValue(familyName,out dFont)){
					
					// Create the font object:
					dFont=new DynamicFont(familyName);
					dFont.Family=loaded.Family;
					
					fonts[familyName]=dFont;
					
				}else{
					
					// Hook up the "real" family:
					dFont.Family=loaded.Family;
					
					// Tell all instances using this dFont that the font is ready:
					Document.FontLoaded(dFont);
					
				}
				
			};
			
			// Send now:
			package.send();
			
			return true;
			
		}
		
	}
	
}