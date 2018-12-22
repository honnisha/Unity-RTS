//--------------------------------------
//             InfiniText
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


namespace InfiniText{
	
	/// <summary>A font family is a collection of font faces (Font objects). 
	/// A font family is e.g. Arial, and it's faces are e.g. bold and italic variants.</summary>
	
	public class FontFamily{
		
		/// <summary>The name of this family.</summary>
		public string Name;
		/// <summary>The regular "default" font. Always set (first font to join by default, overridden if a better face occurs).</summary>
		public FontFace Regular;
		/// <summary>See InvertNormals.</summary>
		public bool InvertedNormals=false;
		/// <summary>All font faces in this family</summary>
		public List<FontFace> FontFaces=new List<FontFace>();
		
		
		public FontFamily(string name){
			Name=name;
			InvertedNormals=Fonts.InvertedNormals;
		}
		
		/// <summary>Adds the given font face to this family.</summary>
		public void Add(FontFace font){
			
			FontFaces.Add(font);
			
			if(Regular==null || font.Regular){
				Regular=font;
			}
			
			if(FontFaces.Count==20){
				Dom.Log.Add("Warning: InfiniText is reporting an unusually large font family. This may impact performance. This is assumed to be rare so if you see this, please let us know!");
			}
			
		}
		
		/// <summary>Gets the .notdef glyph from this family. Tries to get it in the given style; uses regular if not found.</summary>
		public Glyph GetNotDefined(int styleCode){
			
			// Get the face:
			FontFace face=GetFace(styleCode,FontSynthesisFlags.None);
			
			Glyph result=null;
			
			if(face!=null){
				
				// Great! Use that font face:
				result=face.NotDefined;
				
				if(result!=null){
					return result;
				}
				
			}
			
			if(Regular!=null){
				return Regular.NotDefined;
			}
			
			return null;
			
		}
		
		/// <summary>Gets or synthesises a glyph for the given style settings which include weight and italic. Style code 400 means regular.</summary>
		public Glyph GetGlyph(int charcode,int styleCode){
			
			bool firstTime;
			return GetGlyph(charcode,styleCode,FontSynthesisFlags.All,out firstTime);
			
		}
		
		/// <summary>Gets or synthesises a glyph for the given style settings which include weight and italic. Style code 400 means regular.</summary>
		/// <param name='firstTime'>True if this glyph was seen for the first time.</param>
		public Glyph GetGlyph(int charcode,int styleCode,out bool firstTime){
			
			// Get or synth the face:
			FontFace face=GetFace(styleCode,FontSynthesisFlags.All);
			
			return face.GetGlyph(charcode,out firstTime);
			
		}
		
		/// <summary>Gets or synthesises a glyph for the given style settings which include weight and italic. Style code 400 means regular.</summary>
		public Glyph GetGlyph(int charcode,int styleCode,int synthFlags){
			
			bool firstTime;
			return GetGlyph(charcode,styleCode,synthFlags,out firstTime);
			
		}
		
		/// <summary>Gets or creates the most suitable font face for the given style.</summary>
		public FontFace GetFace(int styleCode,int synthFlags){
			
			// Style (italic, oblique etc):
			int style=(styleCode&FontFaceFlags.StyleMask);
			
			// Get the font weight:
			int weight=(styleCode&FontFaceFlags.WeightMask)>>3;
			
			if(weight==0){
				// Regular:
				weight=4;
			}
			
			// And the stretch mode:
			int stretch=(styleCode&FontFaceFlags.StretchMask)>>7;
			
			if(stretch==0){
				// Medium stretch
				stretch=5;
			}
			
			// Try getting the face:
			FontFace bestFace=Regular;
			
			// A linear search is actually optimal for most cases.
			// This is because the set is typically very small.
			
			// [Condensed][Weight][Italic]
			int bestMatch=0;
			
			
			for(int i=0;i<FontFaces.Count;i++){
				
				// Current face:
				FontFace face=FontFaces[i];
				
				// We'll now figure out a match weighting.
				int matchWeight=0;
				
				// First, we match by condensed:
				if(face.Stretch==stretch){
					// Direct match. 8 points awarded.
					matchWeight=8;
				}else{
					
					// How close is it?
					int stretchDiff=face.Stretch-stretch;
					
					if(stretchDiff<0){
						stretchDiff=-stretchDiff;
					}
					
					// StretchDiff maxes out at 8. Award at most 8 points.
					matchWeight=8-stretchDiff;
					
				}
				
				// Weight next.
				if(face.Weight==weight){
					// Direct match. 8 points awarded.
					matchWeight+=8;
				}else{
					
					// How close is it?
					int weightDiff=face.Weight-weight;
					
					if(weightDiff<0){
						weightDiff=-weightDiff;
					}
					
					// weightDiff maxes out at 8. Award at most 8 points.
					matchWeight+=8-weightDiff;
					
				}
				
				// Style.
				if(face.Style==style){
					// Direct match! 2 points awarded.
					matchWeight+=2;
				}else{
					
					// If it's the other of oblique/italic, award 1 point.
					if(style!=0 && face.Style!=0){
						matchWeight+=1;
					}
					
				}
				
				if(matchWeight>bestMatch){
					
					// Got a new best!
					bestMatch=matchWeight;
					bestFace=face;
					
				}
				
				if(matchWeight==18){
					// Perfect match!
					break;
				}
				
			}
			
			if(bestMatch!=18 && synthFlags!=0){
				
				// Synthesize a face, deriving it from our best match.
				bestFace=bestFace.CreateSynthetic(style,weight,stretch);
				
				// Add it:
				Add(bestFace);
				
			}
			
			return bestFace;
			
		}
		
		/// <summary>Gets or synthesises a glyph for the given style settings which include weight and italic. Style code 400 means regular.</summary>
		/// <param name='firstTime'>True if this glyph was seen for the first time.</param>
		public Glyph GetGlyph(int charcode,int styleCode,int synthFlags,out bool firstTime){
			
			// Get or synth the face:
			FontFace face=GetFace(styleCode,synthFlags);
			
			return face.GetGlyph(charcode,out firstTime);
			
		}
		
	}

}