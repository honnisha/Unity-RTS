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
using PowerUI;
using UnityEngine;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;


namespace Css{
	
	/// <summary>
	/// Used to map colour name to colour value.
	/// Note that this can be deleted - make sure you add a compile flag of "NoColourMap" if you choose to remove it to minimise size.
	/// </summary>
	
	public static class ColourMap{
		
		#if !NoColourMap
		
		/// <summary>A map from colour name to hex value.</summary>
		private static Dictionary<string,Color> Map;
		
		#endif
		
		/// <summary>Converts any random string into a colour. This is a strange part of the specs
		/// that rarely gets used now, but it makes for some interesting colours ("grass" is green!).
		/// For example, body bgcolor uses this.</summary>
		public static Color ToSpecialColour(string randomString){
			
			// Trim and lowercase:
			randomString=randomString.Trim().ToLower();
			
			if(randomString.StartsWith("#")){
				randomString=randomString.Substring(1);
			}
			
			// Recognised colour?
			bool recognised;
			Color col=GetColourByName(randomString,out recognised);
			
			if(recognised){
				return col;
			}
			
			// Time to go a bit odd! http://randomstringtocsscolor.com/ 
			// has a nice explanation of this algorithm.
			// (Although we do it in a slightly different order for better performance)
			
			int length=randomString.Length;
			
			if(length==0){
				return Color.black;
			}
			
			// Round up to nearest 3:
			int charsPerChannel=((length+2)/3);
			int twoCharsPerChannel=charsPerChannel*2;
			int startOffset=0;
			
			if(charsPerChannel>2){
				// Check if all 3 start with a zero:
				
				for(int i=0;i<charsPerChannel;i++){
					
					if(randomString[i]=='0' && randomString[i+charsPerChannel]=='0' && randomString[i+twoCharsPerChannel]=='0'){
						
						// Removing the leading 0's.
						startOffset++;
						
					}
					
				}
				
			}
			
			// Load the RGB values now:
			int r=LoadHexFromPart(randomString,startOffset,charsPerChannel);
			int g=LoadHexFromPart(randomString,startOffset+charsPerChannel,twoCharsPerChannel);
			int b=LoadHexFromPart(randomString,startOffset+twoCharsPerChannel,length);
			
			return new Color((float)r/255f,(float)g/255f,(float)b/255f,1f);
		}
		
		/// <summary>Loads a hex value from the given 'any' string. Non-hex chars are treated as 0.
		/// (Normally, non-hex chars invalidate the whole thing).</summary>
		private static int LoadHexFromPart(string randomString,int startOffset,int length){
			
			bool firstChar=true;
			int result=0;
			
			// Hex chars are in the range:
			// 48->57 and 97->102
			
			// For each char in our substring..
			for(int i=startOffset;i<length;i++){
				
				int cc=( (int)randomString[i] );
				
				if(cc>48 && cc<=57){
					
					// In hex range (0-9)
					if(firstChar){
						firstChar=false;
						result+=(cc-48) * 16;
					}else{
						result+=(cc-48);
						
						// Truncate the rest:
						break;
					}
					
				}else if(cc>=97 && cc<=102){
					
					// In hex range (A-F)
					
					if(firstChar){
						firstChar=false;
						result+=(cc-87) * 16;
					}else{
						result+=(cc-87);
						
						// Truncate the rest:
						break;
					}
					
				}else{
					
					// A zero.
					if(firstChar){
						firstChar=false;
					}else{
						// Truncate the rest:
						break;
					}
					
				}
				
			}
			
			if((length-startOffset)==1){
				// Duplicate:
				result+=(result>>4);
			}
			
			return result;
			
		}
		
		/// <summary>Maps colour name, e.g. aqua, to colour. Must be lowercase.</summary>
		/// <returns>An uppercase hex string or null if not found.</returns>
		public static Color GetColourByName(string name,out bool success){
		
			#if NoColourMap
			
			success=false;
			return Color.black;
			
			#else
			
			if(Map==null){
				
				// Note: This occurs when the global ColourUnit object is created.
				
				Map=new Dictionary<string,Color>();
				
				int colourCount=ColourMap.Colours.Length;
				
				for(int i=0;i<colourCount;i+=2){
					
					// Get name and colour:
					string n=ColourMap.Colours[i];
					string c=ColourMap.Colours[i+1];
					
					// Add to map:
					Map[n.ToLower()]=GetHexColour(c);
					
				}
				
			}
			
			Color colour;
			success=Map.TryGetValue(name,out colour);
			return colour;
			
			#endif
			
		}
		
		/// <summary>Duplicates the given nibble (4 bit number) and places the result alongside in the same byte.
		/// E.g. c in hex becomes cc.</summary>
		/// <param name="nibble">The nibble to duplicate.</param>
		private static int DoubleNibble(int nibble){
			return ((nibble<<4) | nibble);
		}
		
		/// <summary>Gets a colour from either hex or a named colour.</summary>
		public static Color GetColour(string valueText){
			
			// Try to get the colour by name:
			bool recognised;
			Color colour=GetColourByName(valueText,out recognised);
			
			if(recognised){
				return colour;
			}
			
			// Get as hex:
			return GetHexColour(valueText);
			
		}
		
		/// <summary>Gets a colour from a hex HTML string.</summary>
		public static Color GetHexColour(string valueText){
			
			if(valueText[0]=='#'){
				valueText=valueText.Substring(1);
			}
			
			float r;
			float g;
			float b;
			float a;
			GetHexColour(valueText,out r,out g,out b,out a);
			
			return new Color(r,g,b,a);
			
		}
		
		/// <summary>Gets a colour from a hex HTML string.</summary>
		public static void GetHexColour(string valueText,out float r,out float g,out float b,out float a){
			
			int rI;
			int gI;
			int bI;
			int aI;
			GetHexColour(valueText,out rI,out gI,out bI,out aI);
			
			r=(float)rI / 255f;
			g=(float)gI / 255f;
			b=(float)bI / 255f;
			a=(float)aI / 255f;
		}
		
		/// <summary>Gets a colour from a hex HTML string.</summary>
		public static void GetHexColour(string valueText,out int r,out int g,out int b,out int a){
			
			int temp;
			
			int length=valueText.Length;
			
			if(length==3){
				// Shorthand hex colour, e.g. #f0f. Each character is essentially duplicated.
				
				// R:
				int.TryParse(valueText.Substring(0,1),NumberStyles.HexNumber,null,out temp);
				r=DoubleNibble(temp);
				// G:
				int.TryParse(valueText.Substring(1,1),NumberStyles.HexNumber,null,out temp);
				g=DoubleNibble(temp);
				// B:
				int.TryParse(valueText.Substring(2,1),NumberStyles.HexNumber,null,out temp);
				b=DoubleNibble(temp);
				
				a=255;
				return;
				
			}
			// Full hex colour, possibly also including alpha.
			
			if(length>=2){
				int.TryParse(valueText.Substring(0,2),NumberStyles.HexNumber,null,out r);
			}else{
				r=0;
			}
			
			if(length>=4){
				int.TryParse(valueText.Substring(2,2),NumberStyles.HexNumber,null,out g);
			}else{
				g=0;
			}
			
			if(length>=6){
				int.TryParse(valueText.Substring(4,2),NumberStyles.HexNumber,null,out b);
			}else{
				b=0;
			}
			
			if(length>=8){
				int.TryParse(valueText.Substring(6,2),NumberStyles.HexNumber,null,out a);
			}else{
				a=255;
			}
			
		}
		
		#if !NoColourMap
		
		private static string[] Colours=new string[]{
"AliceBlue","F0F8FF",
"AntiqueWhite","FAEBD7",
"Aqua","00FFFF",
"Aquamarine","7FFFD4",
"Azure","F0FFFF",
"Beige","F5F5DC",
"Bisque","FFE4C4",
"Black","000000",
"BlanchedAlmond","FFEBCD",
"Blue","0000FF",
"BlueViolet","8A2BE2",
"Brown","A52A2A",
"BurlyWood","DEB887",
"CadetBlue","5F9EA0",
"Chartreuse","7FFF00",
"Chocolate","D2691E",
"Coral","FF7F50",
"CornflowerBlue","6495ED",
"Cornsilk","FFF8DC",
"Crimson","DC143C",
"Cyan","00FFFF",
"DarkBlue","00008B",
"DarkCyan","008B8B",
"DarkGoldenRod","B8860B",
"DarkGray","A9A9A9",
"DarkGrey","A9A9A9",
"DarkGreen","006400",
"DarkKhaki","BDB76B",
"DarkMagenta","8B008B",
"DarkOliveGreen","556B2F",
"DarkOrange","FF8C00",
"DarkOrchid","9932CC",
"DarkRed","8B0000",
"DarkSalmon","E9967A",
"DarkSeaGreen","8FBC8F",
"DarkSlateBlue","483D8B",
"DarkSlateGray","2F4F4F",
"DarkSlateGrey","2F4F4F",
"DarkTurquoise","00CED1",
"DarkViolet","9400D3",
"DeepPink","FF1493",
"DeepSkyBlue","00BFFF",
"DimGray","696969",
"DimGrey","696969",
"DodgerBlue","1E90FF",
"FireBrick","B22222",
"FloralWhite","FFFAF0",
"ForestGreen","228B22",
"Fuchsia","FF00FF",
"Gainsboro","DCDCDC",
"GhostWhite","F8F8FF",
"Gold","FFD700",
"GoldenRod","DAA520",
"Gray","808080",
"Green","008000",
"GreenYellow","ADFF2F",
"Grey","808080",
"HoneyDew","F0FFF0",
"HotPink","FF69B4",
"IndianRed","CD5C5C",
"Indigo","4B0082",
"Ivory","FFFFF0",
"Khaki","F0E68C",
"Lavender","E6E6FA",
"LavenderBlush","FFF0F5",
"LawnGreen","7CFC00",
"LemonChiffon","FFFACD",
"LightBlue","ADD8E6",
"LightCoral","F08080",
"LightCyan","E0FFFF",
"LightGoldenRodYellow","FAFAD2",
"LightGray","D3D3D3",
"LightGreen","90EE90",
"LightGrey","D3D3D3",
"LightPink","FFB6C1",
"LightSalmon","FFA07A",
"LightSeaGreen","20B2AA",
"LightSkyBlue","87CEFA",
"LightSlateGray","778899",
"LightSlateGrey","778899",
"LightSteelBlue","B0C4DE",
"LightYellow","FFFFE0",
"Lime","00FF00",
"LimeGreen","32CD32",
"Linen","FAF0E6",
"Magenta","FF00FF",
"Maroon","800000",
"MediumAquaMarine","66CDAA",
"MediumBlue","0000CD",
"MediumOrchid","BA55D3",
"MediumPurple","9370DB",
"MediumSeaGreen","3CB371",
"MediumSlateBlue","7B68EE",
"MediumSpringGreen","00FA9A",
"MediumTurquoise","48D1CC",
"MediumVioletRed","C71585",
"MidnightBlue","191970",
"MintCream","F5FFFA",
"MistyRose","FFE4E1",
"Moccasin","FFE4B5",
"NavajoWhite","FFDEAD",
"Navy","000080",
"OldLace","FDF5E6",
"Olive","808000",
"OliveDrab","6B8E23",
"Orange","FFA500",
"OrangeRed","FF4500",
"Orchid","DA70D6",
"PaleGoldenRod","EEE8AA",
"PaleGreen","98FB98",
"PaleTurquoise","AFEEEE",
"PaleVioletRed","DB7093",
"PapayaWhip","FFEFD5",
"PeachPuff","FFDAB9",
"Peru","CD853F",
"Pink","FFC0CB",
"Plum","DDA0DD",
"PowderBlue","B0E0E6",
"Purple","800080",
"RebeccaPurple","663399",
"Red","FF0000",
"RosyBrown","BC8F8F",
"RoyalBlue","4169E1",
"SaddleBrown","8B4513",
"Salmon","FA8072",
"SandyBrown","F4A460",
"SeaGreen","2E8B57",
"SeaShell","FFF5EE",
"Sienna","A0522D",
"Silver","C0C0C0",
"SkyBlue","87CEEB",
"SlateBlue","6A5ACD",
"SlateGray","708090",
"SlateGrey","708090",
"Snow","FFFAFA",
"SpringGreen","00FF7F",
"SteelBlue","4682B4",
"Tan","D2B48C",
"Teal","008080",
"Thistle","D8BFD8",
"Tomato","FF6347",
"Transparent","00000000",
"Turquoise","40E0D0",
"Violet","EE82EE",
"Wheat","F5DEB3",
"White","FFFFFF",
"WhiteSmoke","F5F5F5",
"Yellow","FFFF00",
"YellowGreen","9ACD32",

// CSS2 System Colours
"Activeborder","FFFFFF",
"Activecaption","CCCCCC",
"Appworkspace","FFFFFF",
"Background","6363CE",
"Buttonface","DDDDDD",
"Buttonhighlight","DDDDDD",
"Buttonshadow","888888",
"Buttontext","000000",
"Captiontext","000000",
"Graytext","808080",
"Highlight","B5D5FF",
"Highlighttext","000000",
"Inactiveborder","FFFFFF",
"Inactivecaption","FFFFFF",
"Inactivecaptiontext","7F7F7F",
"Infobackground","FBFCC5",
"Infotext","000000",
"Menu","F7F7F7",
"Menutext","000000",
"Scrollbar","FFFFFF",
"Threeddarkshadow","666666",
"Threedface","FFFFFF",
"Threedhighlight","DDDDDD",
"Threedlightshadow","C0C0C0",
"Threedshadow","888888",
"Window","FFFFFF",
"Windowframe","CCCCCC",
"Windowtext","000000"

};

#endif

	}
}
