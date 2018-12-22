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
using Blaze;
using System.Collections;
using System.Collections.Generic;


namespace InfiniText{
	
	/// <summary>A delegate used when InfiniText logs messages.</summary>
	public delegate void OnLogEvent(string text);
	
	/// <summary>The main class for handling global font settings.</summary>
	public static class Fonts{
		
		/// <summary>Should InfiniText always use the OS/2 'typo' metrics when an OS/2 table is present?
		/// This is the W3C specification but it seems browsers vary in unclear ways.</summary>
		public static bool AlwaysUseTypo=true;
		/// <summary>A globally unique glyph ID. Used when placing glyphs on atlases.</summary>
		public static int GlyphID=1;
		/// <summary>Should fonts be rendered using the GPU? Strongly advised they are!</summary>
		public static bool UseGPU=true;
		/// <summary>A tuned set of points which hint the auto-alias system.</summary>
		public readonly static float[] AutoAliasHints=new float[]{
			// Font size, bottom alias, top alias
			6f,		0f,		0.1f,
			12f,	0f,		0.7f,
			50f,	0.12f,	0.5f,
			100f,	0.24f,	0.34f,
			200f,	0.28f,	0.32f
			
		};
		/// <summary>The current text aliasing value.</summary>
		public static float Aliasing=0.3f;
		/// <summary>The main rasteriser used for drawing fonts. Sets itself up as an SDF rasteriser by default.</summary>
		public static Scanner Rasteriser;
		/// <summary>Should font glyphs be loaded when the font is? If yes, font loading takes slightly longer (especially if you use large fonts) but you'll instead have the entire font loaded which can be useful.</summary>
		public static bool Preload=true;
		/// <summary>Should font faces be entirely deferred? If yes, font faces are loaded up only when they are first used (at which point preloading may also occur).</summary>
		public static bool DeferLoad=false;
		/// <summary>See InvertNormals.</summary>
		public static bool InvertedNormals=false;
		/// <summary>Change PixelHeight instead. The SDF draw height.</summary>
		public static float SdfPixelHeight=40f;
		/// <summary>The SDF offset. SDFSize / SdfPixelHeight.</summary>
		public static float SdfOffset=0.05f;
		/// <summary>Change SDFSpread instead. The raw size of the default SDF spread. The bigger this is, the more anti-aliasing and bigger glows you can use.</summary>
		public static int SdfSize=2;
		/// <summary>Change SDFSpread instead. Simply 2 times SdfSize.</summary>
		public static int DoubleSdfSize=SdfSize*2;
		/// <summary>The SDF outline location. 0-1.</summary>
		public static float OutlineLocation=0.35f;
		/// <summary>All available font families.</summary>
		public static Dictionary<string,FontFamily> All;
		/// <summary>The delegate used when InfiniText logs a message.</summary>
		public static OnLogEvent OnLog;
		
		
		/// <summary>Logs a message.</summary>
		public static void OnLogMessage(string message){
			
			if(OnLog!=null){
				OnLog(message);
			}
			
		}
		
		/// <summary>Wrapper function for loading a font from the given data. Note that loaded fonts are cached once loaded.</summary>
		public static FontFace Load(byte[] data){
			
			return FontLoader.Load(data);
			
		}
		
		/// <summary>The size of the default SDF spread around glyphs.</summary>
		public static int SDFSpread{
			get{
				return SdfSize;
			}
			set{
				SdfSize=value;
				DoubleSdfSize=value*2;
				
				// Update offset:
				SdfOffset=(float)SdfSize / SdfPixelHeight;
				
			}
		}
		
		/// <summary>The SDF draw height. Default is 40.</summary>
		public static float PixelHeight{
			get{
				return SdfPixelHeight;
			}
			set{
				SdfPixelHeight=value;
				TextureCameras.Accuracy=1f/value;
				if(Rasteriser!=null){
					Rasteriser.DrawHeight=(int)value;
				}
				
				// Update offset:
				SdfOffset=(float)SdfSize / SdfPixelHeight;
				
			}
		}
		
		/// <summary>Clears all fonts.</summary>
		public static void Clear(){
			All=null;
			GlyphID=1;
		}
		
		/// <summary>Sets up InfiniText. Called automatically when the first font is loaded.</summary>
		internal static void Start(){
			
			// Update offset:
			SdfOffset=(float)SdfSize / SdfPixelHeight;
			
			if(!UseGPU || Rasteriser!=null){
				return;
			}
			
			// Setup and start the rasteriser:
			Rasteriser=new Scanner();
			Rasteriser.SDFSize=SdfSize;
			Rasteriser.DrawHeight=(int)SdfPixelHeight;
			Rasteriser.Start();
			
		}
		
		/// <summary>Gets or creates a font family by name.</summary>
		public static FontFamily GetOrCreate(string name){
			
			FontFamily result=Get(name);
			
			if(result==null){
				
				if(Rasteriser==null && !UseGPU){
					// Start now:
					Start();
				}
				
				result=new FontFamily(name);
				
				if(All==null){
					All=new Dictionary<string,FontFamily>();
				}
				
				All[name.ToLower()]=result;
				
			}
			
			return result;
		}
		
		/// <summary>Tries to get a font family by name.</summary>
		/// <returns>Null if the family isn't loaded.</returns>
		public static FontFamily Get(string name){
			
			if(All==null){
				return null;
			}
			
			// Lowercase the name:
			name=name.ToLower();
			
			FontFamily result;
			All.TryGetValue(name,out result);
			
			return result;
		}
		
	}

}