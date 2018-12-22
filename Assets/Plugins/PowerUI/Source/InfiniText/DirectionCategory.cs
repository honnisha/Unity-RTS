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
using System.Collections;


namespace InfiniText{
	
	/// <summary>
	/// Obtains the bidirectional catagory of a character.
	/// </summary>
	
	public static class DirectionCategory{
		
		/// <summary>The available blocks. Sorted by start.</summary>
		private static BidiBlock[] Blocks;
		
		
		/// <summary>Sets up the bidirectional data.</summary>
		public static void Setup(){
			
			if(Blocks!=null){
				return;
			}
			
			// Get the bidi data now:
			
			#if UNITY
			
			// Get the entity file:
			UnityEngine.TextAsset bidiData=(UnityEngine.Resources.Load("BidirectionalData") as UnityEngine.TextAsset);
			
			if(bidiData==null){
				return;
			}
			
			byte[] file=bidiData.bytes;
			
			// Create a reader:
			BinaryIO.Reader reader=new BinaryIO.Reader(file);
			
			int count=(int)reader.ReadCompressed();
			Blocks=new BidiBlock[count];
			int index=0;
			
			// Setup all blocks:
			while(reader.More()){
				
				// Read the block:
				Blocks[index++]=new BidiBlock(
					(int)reader.ReadCompressed(),
					(int)reader.ReadCompressed(),
					reader.ReadByte()
				);
				
			}
			
			#else
				
				#warning Bidirectional data not available
			
			#endif
			
			
		}
		
		/// <summary>Finds the nearest block for the given charcode and returns its mode.
		/// The mode is one of the BidiBlock options.</summary>
		public static int Get(int charCode){
			
			if(Blocks==null){
				// Always LTR:
				return BidiBlock.LeftToRight;
			}
			
			int min=0;
			int max=Blocks.Length-1;
			
			while(min<=max){
				
				// Midpoint:
				int mid = (min + max) / 2;
				
				// Get that block:
				BidiBlock nearest=Blocks[mid];
				
				// Check:
				if(charCode>=nearest.Start && charCode<=nearest.End){
					return nearest.Mode;
				}else if(charCode < nearest.Start){
					max = mid - 1;
				}else{
					min = mid + 1;
				}
			
			}
			
			// LTR otherwise:
			return BidiBlock.LeftToRight;
			
		}
		
	}
	
	/// <summary>
	/// A block of characters with the same bidi mode.
	/// </summary>
	public struct BidiBlock{
		
		public const int LeftToRight = 1;
		public const int RightToLeft = 2;
		/// <summary>It goes left to right, but doesn't affect neutrals around it.</summary>
		public const int WeakLeftToRight = 4;
		public const int Neutral = 8;
		public const int Rightwards=LeftToRight | WeakLeftToRight;
		
		
		/// <summary>Start codepoint.</summary>
		public int Start;
		/// <summary>End codepoint.</summary>
		public int End;
		/// <summary>The bidi mode for this block.</summary>
		public int Mode;
		
		
		public BidiBlock(int s,int e,int mode){
			Start=s;
			End=e;
			Mode=mode;
		}
		
	}
	
}