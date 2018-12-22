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
using System.IO;
using System.Collections;
using System.Collections.Generic;
using PowerUI; // Ideally this is inside the if below, but it would fail in a new install

#if PowerUI
using Spa;
#endif



namespace InfiniText{
	
	/// <summary>
	/// A font face for a particular font family.
	/// Note that you must consider if this font face has been loaded (see RequiresLoad) yet if you use deferred loading.
	/// The metrics will always be available however - just the glyphs will not be if RequiresLoad is true.
	/// </summary>
	
	public class FontFace{
		
		/// <summary>The font weight. 1-9.</summary>
		public int Weight;
		/// <summary>The font style. 0-2.</summary>
		public int Style;
		/// <summary>The font stretch. 1-9.</summary>
		public int Stretch;
		/// <summary>The full name of this font face.</summary>
		public string Name;
		/// <summary>The winding order of glyphs of this font.</summary>
		public bool Clockness;
		/// <summary>Use this to disable the SDF extrusion of a font face which may cause the letters to go streaky in rare cases.</summary>
		public bool DisableExtrude;
		/// <summary>Is the clockness of this face known yet? If so, the first loaded glyph will figure it out.</summary>
		public bool WindingUnknown=true;
		/// <summary>Is this font face loaded at all? If it requires a load, call Load(). Note that metrics are available if it's not loaded.</summary>
		public bool RequiresLoad=false;
		/// <summary>The spacing between the text, divided by two. It's divided because it gets shared.</summary>
		public float HalfLineGap=0.1f;
		/// <summary>The gap between lines.
		/// It gets placed below the line when it's necessary.
		/// Note that this is *not* the line height (or leading).</summary>
		public float LineGap;
		
		/// <summary>Baseline to baseline height. Same as 'line-height' in CSS.</summary>
		public float BaselineToBaseline{
			get{
				// In order of their vertical position:
				return Ascender+Descender+LineGap;
			}
		}
		
		/// <summary>The computed amount of units to "push" (sheer) the top of a glyph over by. Std default is 12 degrees; tan(12d) units.</summary>
		public float ItalicAngle=0.213f;
		/// <summary>The size of the ascender. Relative to em size.</summary>
		public float Ascender;
		/// <summary>The size of the descender. Relative to em size and is used to define where the baseline is.</summary>
		public float Descender;
		/// <summary>The family that this font is in. Created by the first font in the family.</summary>
		public FontFamily Family;
		/// <summary>The number of characters in this font face.</summary>
		public int CharacterCount;
		/// <summary>The maximum advance width.</summary>
		public float MaxAdvanceWidth;
		/// <summary>The minimum left side bearing.</summary>
		public float MinLeftSideBearing;
		/// <summary>The minimum right side bearing.</summary>
		public float MinRightSideBearing;
		/// <summary>The maximum x extent.</summary>
		public float MaxXExtent;
		/// <summary>The cached height of a lowercase 'x'. Relative.</summary>
		private float ExHeightRaw;
		/// <summary>The cached width of an 'M'. Relative.</summary>
		private float MWidthRaw;
		/// <summary>The cached height of a '0'. Relative.</summary>
		private float ChHeightRaw;
		/// <summary>The angle of the caret when this font is used.</summary>
		public float CaretAngle;
		/// <summary>The horizontal caret offset.</summary>
		public float CaretOffset;
		/// <summary>The number of glyphs in this font face.</summary>
		public int NumberOfHMetrics;
		/// <summary>The glyph units per em value.</summary>
		public int UnitsPerEm=1000;
		/// <summary>The .notdef glyph.</summary>
		public Glyph NotDefined;
		/// <summary>The glyph units per em value, as a float.</summary>
		public float UnitsPerEmF=1000f;
		/// <summary>The thickness of a strikethrough line.</summary>
		public float StrikeSize=0.1f;
		/// <summary>The offset to a strikethrough line.</summary>
		public float StrikeOffset=0.25f;
		/// <summary>The font that this is a derivative of.</summary>
		public FontFace SyntheticDerivative;
		/// <summary>The flags for this font face. A style bitmask.</summary>
		public int StyleFlags=FontFaceFlags.None;
		/// <summary>All available typographic features. Ligatures, smallcaps etc.</summary>
		public Dictionary<string,FontFeature> Features=new Dictionary<string,FontFeature>();
		/// <summary>All raw glyphs in this font face. Indexed by charcode. Note that if you have Font.Preload false, they may not be loaded.
		/// Check glyph.RequiresLoad if you access this directly.</summary>
		public Dictionary<int,Glyph> Glyphs=new Dictionary<int,Glyph>();
		/// <summary>The CFF parser, if this is a CFF format font. Used to load glyphs on demand.</summary>
		public CffGlyphParser CffParser;
		/// <summary>The raw parser. Used to load glyphs on demand.</summary>
		public FontParser Parser;
		/// <summary>Don't use this! Use Glyphs instead. The raw glyph set used by the parser. May be null.</summary>
		internal Glyph[] ParserGlyphs;
		/// <summary>The number of not yet loaded glyphs in this font.</summary>
		public int UnloadedGlyphs;
		
		#if PowerUI
		/// <summary>Bitmap font data held inside an SPA, if this is a bitmap font.</summary>
		public SPA BitmapFontData; 
		
		
		/// <summary>Sets this fontface up as a bitmap font.</summary>
		public void SetupBitmapFont(SPA spa){
			
			// Apply BFD:
			BitmapFontData=spa;
			RequiresLoad=false;
			
		}
		
		#endif
		
		
		/// <summary>Called when all glyphs in this font face have been loaded up.
		/// Note that this may occur very late or, more likely never, when glyphs are loaded on demand.</summary>
		public void AllGlyphsLoaded(){
			
			CffParser=null;
			Parser=null;
			UnloadedGlyphs=0;
			ParserGlyphs=null;
			
		}
		
		/// <summary>The width of an 'M'. Relative.</summary>
		public float MWidth{
			get{
				if(MWidthRaw!=0f){
					return MWidthRaw;
				}
				
				// Get it:
				Glyph m=GetGlyph((int)'M');
				
				if(m==null){
					// Assume 1em:
					MWidthRaw=1f;
				}else{
					
					if(m.Width==0f){
						m.RecalculateBounds();
					}
					
					MWidthRaw=m.Width;
				}
				
				return MWidthRaw;
			}
		}
		
		/// <summary>The height of a lowercase 'x'. Relative.</summary>
		public float ExHeight{
			get{
				if(ExHeightRaw!=0f){
					return ExHeightRaw;
				}
				
				// Get it:
				Glyph ex=GetGlyph((int)'x');
				
				if(ex==null){
					// Assume 0.5em:
					ExHeightRaw=0.5f;
				}else{
					
					if(ex.Width==0f){
						ex.RecalculateBounds();
					}
					
					ExHeightRaw=ex.Height;
				}
				
				return ExHeightRaw;
			}
		}
		
		/// <summary>The height of a '0'. Relative.</summary>
		public float ChHeight{
			get{
				if(ChHeightRaw!=0f){
					return ChHeightRaw;
				}
				
				// Get it:
				Glyph ch=GetGlyph((int)'0');
				
				if(ch==null){
					// Assume 1em:
					ChHeightRaw=1f;
				}else{
					ChHeightRaw=ch.Height;
				}
				
				return ChHeightRaw;
			}
		}
		
		/// <summary>Figures out the winding order of this font face from the given glyph.</summary>
		internal void FindWinding(Glyph glyph){
			
			// No longer unknown:
			WindingUnknown=false;
			
			// Get the signed area:
			float firstArea=glyph.GetSignedArea();
			
			Clockness=(firstArea<=0f);
			
			if(Family.InvertedNormals){
				Clockness=!Clockness;
			}
			
		}
		
		/// <summary>Loads the glyph info for this font face now. See RequiresLoad.</summary>
		public void Load(){
			if(!RequiresLoad){
				return;
			}
			
			RequiresLoad=false;
			FontLoader.ReadTables(Parser,this);
		}
		
		/// <summary>Derives this font face from the given one, with the given settings.</summary>
		public void Derive(FontFace from,int styleCode,int weight,int stretch){
			
			// Flags and weight:
			SetFlags(styleCode,weight,stretch);
			
			SyntheticDerivative=from;
			
			Clockness=from.Clockness;
			WindingUnknown=from.WindingUnknown;
			
			Family=from.Family;
			
			Name=FamilyName+" (InfiniText Synthetic)";
			
		}
		
		/// <summary>Applies the angle, in degrees, used for synthesizing italic chars.</summary>
		public void SetItalicAngle(float angle){
			
			ItalicAngle=(float)Math.Tan(angle * Math.PI/180f);
			
		}
		
		/// <summary>Sets the weight of this face and if it's italic.</summary>
		public void SetFlags(int style,int weight,int stretch){
			
			if(weight>100){
				// 1-9:
				weight/=100;
			}
			
			if(stretch==0){
				// Medium.
				stretch=5;
			}
			
			// Build the flags:
			int flags=style | ( weight<<3 ) | (stretch<<7);
			
			// Apply the flags:
			StyleFlags=flags;
			
			// Apply flags:
			Weight=weight;
			Stretch=stretch;
			Style=style;
			
		}
		
		/// <summary>Is this a synthetic font?</summary>
		public bool Synthetic{
			get{
				return (SyntheticDerivative!=null);
			}
		}
		
		/// <summary>Is this the regular font for this family?</summary>
		public bool Regular{
			get{
				return (Weight==4) && Style==0;
			}
		}
		
		/// <summary>True if this is an italic or oblique face.</summary>
		public bool Stylized{
			get{
				return (Style!=0);
			}
		}
		
		/// <summary>True if this is a bold face.</summary>
		public bool Bold{
			get{
				// Not regular:
				return (Weight!=4);
			}
		}
		
		/// <summary>Creates a synthetic derivative of this font. Used when a font does not have e.g. an italic face.</summary>
		public FontFace CreateSynthetic(int styleCode,int weight,int stretch){
			
			FontFace font=new FontFace();
			
			font.Derive(this,styleCode,weight,stretch);
			
			return font;
			
		}
		
		/// <summary>Synthesizes a glyph of the given charcode.</summary>
		public Glyph Synthesize(int charCode){
			
			if(SyntheticDerivative.RequiresLoad){
				// Load it now:
				SyntheticDerivative.Load();
			}
			
			Glyph baseGlyph;
			
			if(!SyntheticDerivative.Glyphs.TryGetValue(charCode,out baseGlyph)){
				return null;
			}
			
			if(baseGlyph.RequiresLoad){
				baseGlyph.LoadNow();
			}
			
			// Make a copy of the glyph:
			Glyph synthetic=baseGlyph.Copy();
			
			// Push it in:
			Glyphs[charCode]=synthetic;
			
			/*
			if(Weight!=SyntheticDerivative.Weight){
				
				// Go bold or thinner. "Pull" it out along the normals.
				int delta=Weight-SyntheticDerivative.Weight;
				
				float extrudeBy=0.01f*delta;
				
				// We need the meta:
				synthetic.RecalculateMeta();
				
				// Expand the draw distances:
				extrudeBy+=1f;
				
				synthetic.AdvanceWidth*=extrudeBy;
				synthetic.LeftSideBearing*=extrudeBy;
				
			}
			*/
			
			if(Stylized && !SyntheticDerivative.Stylized){
				
				// Time to go italic! Sheer based on our fonts italic angle.
				synthetic.Sheer(ItalicAngle);
				
			}
			
			// Make sure it knows to recompute the meta:
			synthetic.Width=0f;
			
			// All done!
			return synthetic;
			
		}
		
		/// <summary>Gets or sets the given charcode glyph.</summary>
		public Glyph this[int charCode]{
			get{
				Glyph result;
				Glyphs.TryGetValue(charCode,out result);
				return result;
			}
			set{
				Glyphs[charCode]=value;
			}
		}
		
		/// <summary>Gets a glyph which can be optionally overriden by a 'character provider'.</summary>
		public Glyph GetGlyphOrEmoji(int charcode){
			
			// Is it e.g. emoji?
			Glyph result=CharacterProviders.Find(charcode);
			
			if(result==null){
				
				bool firstTime;
				return GetGlyph(charcode,out firstTime);
				
			}
			
			return result;
			
		}
		
		/// <summary>Gets a glyph for a particular charcode without loading anything.</summary>
		/// <returns>A glyph if it's available. Null otherwise.</returns>
		public Glyph GetGlyphDirect(int charCode){
			Glyph result;
			Glyphs.TryGetValue(charCode,out result);
			return result;
		}
		
		/// <summary>Gets a glyph for a particular charcode. Note that no glyphs are ever empty.</summary>
		public Glyph GetGlyph(int charCode){
			bool firstTime;
			return GetGlyph(charCode,out firstTime);
		}
		
		/// <summary>Gets a glyph for a particular charcode. Note that no glyphs are ever empty.</summary>
		/// <param name='firstTime'>True if this glyph was seen for the very first time.</param>
		public Glyph GetGlyph(int charCode,out bool firstTime){
			
			firstTime=false;
			
			if(RequiresLoad){
				// Load it now:
				Load();
			}
			
			Glyph result;
			if(!Glyphs.TryGetValue(charCode,out result)){
				
				if(SyntheticDerivative!=null){
					// Synthesize now.
					result=Synthesize(charCode);
				}
				
				#if PowerUI
				
				if(BitmapFontData!=null){
					
					// Get from bitmap font:
					Spa.SPACharacter character=BitmapFontData.GetCharacter(charCode);
					
					if(character!=null){
						
						// Create it as a glyph:
						// Note! internally adds to this fontface. 
						result=character.ToGlyph(this);
						
						
					}
					
				}
				
				#endif
				
			}else if(result.RequiresLoad){
				firstTime=true;
				result.LoadNow();
			}
			
			return result;
			
		}
		
		/// <summary>The name of the family this font is in. Set is for internal use only.</summary>
		public string FamilyName{
			get{
				if(Family==null){
					return null;
				}
				
				return Family.Name;
			}
			set{
				
				// Get/ create the family:
				Family=Fonts.GetOrCreate(value);
				
				// Add to family:
				Family.Add(this);
				
			}
		}
		
	}

}