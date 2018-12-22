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

#if UNITY_2_6 || UNITY_3_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
	#define PRE_UNITY3_5
#endif

#if PRE_UNITY3_5 || UNITY_3_5 || UNITY_4_0 || UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_4 || UNITY_4_5 || UNITY_4_6 || UNITY_4_7
	#define PRE_UNITY5
#endif

#if PRE_UNITY5 || UNITY_5 || UNITY_5_3_OR_NEWER
	#define UNITY
#endif

using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Used to map entity names to their character.
	/// E.g. 'nbsp' to ' '.
	/// </summary>
	
	public static class CharacterEntities{
		
		/// <summary>A map from entity name to character.</summary>
		private static Dictionary<string,string> Map;
		
		
		/// <summary>Sets up the map.</summary>
		public static void Setup(){
			
			Map=new Dictionary<string,string>();
			
			#if UNITY
			
			// Get the entity file:
			UnityEngine.TextAsset ncData=(UnityEngine.Resources.Load("NamedCharacters") as UnityEngine.TextAsset);
			
			if(ncData==null){
				return;
			}
			
			byte[] file=ncData.bytes;
			
			// Create a reader:
			BinaryIO.Reader reader=new BinaryIO.Reader(file);
			
			while(reader.More()){
				
				// Read the line:
				string rowName=reader.ReadString();
				string chars=reader.ReadString();
				
				// Add to map:
				Map[rowName]=chars;
				
			}
			
			#else
				
				// Load entities from a standalone file.
				#warning Named characters won't be available
			
			#endif
			
		}
		
		/// <summary>Maps entity name, e.g. nbsp, to the character.</summary>
		public static string GetEntityByName(string name){
			
			string result;
			Map.TryGetValue(name,out result);
			return result;
			
		}
		
		/// <summary>Includes e.g. &#xaa; as well as nbsp.</summary>
		public static string GetByValueOrName(string value){
			
			if(value.StartsWith("#x")){
				
				// Direct hex character.
				int charcode;
				if(int.TryParse(value.Substring(2).Trim(),NumberStyles.HexNumber,CultureInfo.InvariantCulture,out charcode)){
					return char.ConvertFromUtf32(charcode);
				}
				
			}else if(value.Length>1 && value[0]=='#'){
				
				// Direct decimal character.
				int charcode;
				if(int.TryParse(value.Substring(1).Trim(),out charcode)){
					return char.ConvertFromUtf32(charcode);
				}
				
			}
			
			// Try pulling it from our standard set:
			return GetEntityByName(value);
			
		}
		
	}
	
}
