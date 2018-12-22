//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//   Kulestar would like to thank the following:
//    PDF.js, Microsoft, Adobe and opentype.js
//    For providing implementation details and
// specifications for the TTF and OTF file formats.
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;


namespace InfiniText{

	public static class NameTables{
		
		private static string[] PropertyMap = new string[]{
			"copyright", // 0
			"fontFamily", // 1
			"fontSubfamily", // 2
			"uniqueID", // 3
			"fullName", // 4
			"version", // 5
			"postScriptName", // 6
			"trademark", // 7
			"manufacturer", // 8
			"designer", // 9
			"description", // 10
			"manufacturerURL", // 11
			"designerURL", // 12
			"licence", // 13
			"licenceURL", // 14
			"reserved", // 15
			"preferredFamily", // 16
			"preferredSubfamily", // 17
			"compatibleFullName", // 18
			"sampleText", // 19
			"postScriptFindFontName", // 20
			"wwsFamily", // 21
			"wwsSubfamily" // 22
		};
		
		public static Dictionary<string,string> Load(FontParser parser,int start,FontFace font){
			
			// Create the map:
			Dictionary<string,string> map=new Dictionary<string,string>();
			
			// Go there now:
			parser.Position=start;
			
			// Format:
			int format=parser.ReadUInt16();
			
			// Number of names:
			int count = parser.ReadUInt16();
			
			// String offset:
			int stringOffset = start + parser.ReadUInt16();
			
			int unknownCount = 0;
			
			for(int i=0;i<count;i++){
				
				// Platform ID:
				ushort platformID = parser.ReadUInt16();
				ushort encodingID = parser.ReadUInt16();
				ushort languageID = parser.ReadUInt16();
				ushort nameID = parser.ReadUInt16();
				string property = null;
				
				if(nameID<PropertyMap.Length){
					property=PropertyMap[nameID];
				}
				
				ushort byteLength = parser.ReadUInt16();
				ushort offset = parser.ReadUInt16();
				
				// platformID - encodingID - languageID standard combinations :
				// 1 - 0 - 0 : Macintosh, Roman, English
				// 3 - 1 - 0x409 : Windows, Unicode BMP (UCS-2), en-US
				
				if(platformID == 3 && encodingID == 1 && languageID == 0x409){
					
					int length = byteLength/2;
					char[] chars=new char[length];
				
					int index=stringOffset+offset;
					
					for(int j = 0; j < length; j++){
						
						chars[j] = (char)parser.ReadInt16(ref index);
						
					}
					
					string str = new string(chars);
					
					if(property!=null){
						map[property] = str;
					}else{
						unknownCount++;
						map["unknown"+unknownCount] = str;
					}
					
				}
				
			}
			
			// Next grab the name and family. Done last as we must know name before family.
			string name;
			map.TryGetValue("fullName",out name);
			
			string family;
			map.TryGetValue("fontFamily",out family);
			
			if(name==null){
				name="[Untitled font face]";
			}
			
			// Apply name first:
			font.Name=name;
			
			if(family==null){
				family="[Untitled font family]";
			}
			
			// Apply family name (which in turn creates the family instance):
			font.FamilyName=family;
			
			if(format==1){
				
				// Language tag count:
				parser.ReadUInt16();
				
			}
			
			return map;
			
		}
		
	}

}