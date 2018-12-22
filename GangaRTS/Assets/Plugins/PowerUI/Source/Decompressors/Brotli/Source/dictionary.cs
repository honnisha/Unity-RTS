//--------------------------------------
//         Brotli Decompressor
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2016 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------


using System;
using System.IO;
using PowerUI;


namespace Brotli{
	
	/// <summary>An event that triggers when the dictionary is ready.</summary>
	public delegate void DictionaryReadyEvent();
	
	/// <summary>
	/// Brotli static dictionary.
	/// </summary>
	
	public static class Dictionary{
		
		/// <summary>Location of the dictionary.</summary>
		public static string Location="resources://brotli-static";
		/// <summary>The dictionary data.</summary>
		internal static byte[] Data;
		
		internal readonly static uint[] OffsetsByLength = new uint[]{
			0,     0,     0,     0,     0,  4096,  9216, 21504, 35840, 44032,
			53248, 63488, 74752, 87040, 93696, 100864, 104704, 106752, 108928, 113536,
			115968, 118528, 119872, 121280, 122016,
		};

		internal readonly static byte[] SizeBitsByLength = new byte[]{
		  0,  0,  0,  0, 10, 10, 11, 11, 10, 10,
		 10, 10, 10,  9,  9,  8,  7,  7,  8,  7,
		  7,  6,  6,  5,  5,
		};
		
		internal const int MinDictionaryWordLength = 4;
		internal const int MaxDictionaryWordLength = 24;
		
		/// <summary>Loads the dictionary.</summary>
		public static void Load(DictionaryReadyEvent onLoaded){
			
			// Get it:
			DataPackage package=new DataPackage(Location);
			
			package.onload=delegate(UIEvent e){
				
				Data=package.responseBytes;
				onLoaded();
				
			};
			
			// Go!
			package.send();
			
		}
		
	}
	
}