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
using System.IO;
using System.Text;
using PowerUI;


namespace Spa{
	
	/// <summary>
	/// Manages loading binary data from an SPA file.
	/// </summary>
	
	public class SPAReader:BinaryReader{
		
		/// <summary>The text encoding in use.</summary>
		public static readonly Encoding TextEncoding=Encoding.UTF8;
		
		/// <summary>The previously read map X coordinate.
		/// Used for compression purposes.</summary>
		public int PreviousX;
		/// <summary>The previously read map Y coordinate.
		/// Used for compression purposes.</summary>
		public int PreviousY;
		
		/// <summary>Creates a new reader for the given stream.</summary>
		/// <param name="stream">A stream to read from.</param>
		public SPAReader(Stream stream):base(stream){}
		
		/// <summary>Creates a new reader for the given block of bytes.</summary>
		/// <param name="src">The source block of bytes to read from.</param>
		public SPAReader(byte[] src):base(new MemoryStream(src)){
		}
		
		/// <summary>Resets the map coordinates. Used only when an SPA actually has a map.</summary>
		public void ResetCoordinates(){
			
			PreviousX=0;
			PreviousY=0;
			
		}
		
		/// <summary>Reads a string from the stream; it has no length limit.</summary>
		/// <returns>The string that was read.</returns>
		public override string ReadString(){
			return ReadString((int)ReadCompressed());
		}
		
		/// <summary>Reads a string with the given length from the stream.</summary>
		/// <param name="length">The length of the string.</param>
		/// <returns>The string read from the stream.</returns>
		public string ReadString(int length){
			return TextEncoding.GetString(ReadBytes(length));
		}
		
		/// <summary>Reads a 3 byte unsigned integer from the stream.</summary>
		/// <returns>A 3 byte unsigned integer.</returns>
		public uint ReadUInt24(){
			uint value=0;
			int shift=0;
			for(int i=0;i<3;i++){
				value|=(uint)(ReadByte()<<shift);
				shift+=8;
			}
			return value;
		}
		
		/// <summary>Reads a 3 byte integer from the stream.</summary>
		/// <returns>A 3 byte integer.</returns>
		public int ReadInt24(){
			int value=0;
			int shift=0;
			for(int i=0;i<3;i++){
				value|=(int)(ReadByte()<<shift);
				shift+=8;
			}
			return value;
		}
		
		/// <summary>How many bytes the given value would create if written out.</summary>
		public static int CompressedSize(ulong l){
			if (l< 251){
				return 1;
			}else if (l <= ushort.MaxValue){
				return 3;
			}else if (l < 16777216L){
				return 4;
			}else if(l <= uint.MaxValue){
				return 5;
			}
			
			return 9;
		}
		
		/// <summary>Reads a compressed integer - the opposite of Writer.WriteCompressed.</summary>
		public ulong ReadCompressed(){
			byte c=ReadByte();
			switch (c){
				case 251: return (ulong)ReadUInt16();
				case 252: return (ulong)ReadUInt24();
				case 253: return (ulong)ReadUInt32();
				case 254: return ReadUInt64();
				default: return c;
			}
		}
		
		/// <summary>Reads a signed compressed number.</summary>
		public long ReadCompressedSigned(){
			
			ulong compr=ReadCompressed();
			
			// Odd?
			if((compr&1)==1){
				
				// This means it's negative.
				// Chop off the negative byte and apply -:
				return -(long)(compr>>1);
				
			}
			
			// It's +ve
			return (long)(compr>>1);
			
		}
		
	}
	
}