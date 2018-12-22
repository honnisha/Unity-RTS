//--------------------------------------
//   Kulestar Standard Binary Helpers
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;
using System.Text;


namespace BinaryIO{

	/// <summary>
	/// This class is a wrapper for any block of bytes. It allows values to be read from the stream
	/// one at a time. Works best with streams created by BinaryIO.Writer.
	/// </summary>

	public partial class Reader:System.IO.BinaryReader{
		
		/// <summary>The text encoding in use.</summary>
		public static Encoding TextEncoding=Encoding.UTF8;
		
		
		/// <summary>Returns the maximum number that x bits can hold.</summary>
		/// <param name="bitCount">The number of bits.</param>
		/// <returns>The maximum unsigned value that many bits can hold.</returns>
		public static int ValueMax(int bitCount){
			return (1<<bitCount)-1;
		}
		
		
		/// <summary>The length of the stream.</summary>
		public long Length;
		
		
		/// <summary>Creates a new reader for the given stream.</summary>
		/// <param name="stream">A stream to read from.</param>
		public Reader(Stream stream):base(stream){}
		
		/// <summary>Creates a new reader for the given block of bytes.</summary>
		/// <param name="src">The source block of bytes to read from.</param>
		public Reader(byte[] src):base(new MemoryStream(src)){
			Length=BaseStream.Length;
		}
		
		/// <summary>Reads the bytes of a texture from the stream.</summary>
		public byte[] ReadImageBytes(){
			return ReadBytes((int)ReadUInt32());
		}
		
		public void Skip(int count){
			for(int i=0;i<count;i++){
				ReadByte();
			}
		}
		
		/// <summary>Gets the value of the next byte in the stream.</summary>
		/// <returns>The value of the byte; -1 if the reader is past the end of the stream.</returns>
		public int PeekByte(){
			if(More()){
				int b=(int)ReadByte();
				BaseStream.Position--;
				return b;
			}else{
				return -1;
			}
		}
		
		/// <summary>The current starting location.</summary>
		public virtual long MarkStart(){
			return Position;
		}
		
		/// <summary>The current data index.</summary>
		public virtual int DataIndex{
			get{
				return (int)Position;
			}
		}
		
		/// <summary>Reads a packed integer from the stream. Not the opposite of </summary>
		/// <returns>The packed integer.</returns>
		public long PackedInt(){
			byte c=ReadByte();
			switch (c){
				case 251: return -1;
				case 252: return ReadUInt16();
				case 253: return ReadInt24();
				case 254: return ReadInt64();
				default: return c;
			}
		}
		
		
		/// <summary>Reads a compressed floating point value from the stream.
		/// Compressed floats are stored as a ushort. Their value represents a percentage of a range
		/// (i.e. it gets divided by the max ushort value 64k), where 64k = 100% (the top of the range, low + width).</summary>
		/// <param name="low">The bottom of the float range this was compressed into.</param>
		/// <param name="width">This value is high-low; the width of the range.</param>
		/// <returns>The decompressed float.</returns>
		public float ReadInt16F(float low,float width){
			return low+(ReadUInt16()*width/ushort.MaxValue);
		}
		
		public ulong ReadUInt48(){
			ulong value=0;
			int shift=0;
			
			for(int i=0;i<6;i++){
				value|=((ulong)ReadByte()<<shift);
				shift+=8;
			}
			
			return value;
		}
		
		
		public ulong ReadUInt40(){
			ulong value=0;
			int shift=0;
			for(int i=0;i<5;i++){
				value|=((ulong)ReadByte()<<shift);
				shift+=8;
			}
			return value;
		}
		
		public long ReadInt40(){
			long value=0;
			int shift=0;
			for(int i=0;i<5;i++){
				value|=((long)ReadByte()<<shift);
				shift+=8;
			}
			return value;
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
			
			long value=(long)ReadCompressed();
			
			// Negative?
			bool negative=((value & 1)==1);
			
			// Shift:
			value=value>>1;
			
			if(negative){
				value=-value;
			}
			
			return value;
			
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
		
		/// <summary>Reads a string that is terminated with the null character. No length limit but slower to read.</summary>
		/// <returns>The string read from the stream.</returns>
		public string ReadZeroString(){
			if(!More()){
				return "";
			}
			
			StringBuilder str=new StringBuilder();
			
			char current=ReadChar();
			while(current!='\0'){
				str.Append(current);
				if(More()){
					current=ReadChar();
				}else{
					break;
				}
			}
			
			return str.ToString();
		}
		
		/// <summary>Reads a long string from the stream. Maximum length of 2GB.</summary>
		/// <returns>A string read from the stream.</returns>
		public string ReadLongString(){
			return ReadString((int)ReadUInt32());
		}
		
		/// <summary>Reverses Writer.WriteRange. Converts a compressed float stored in a block of
		/// bits back into a floating point.</summary>
		/// <param name="data">The bits the value is held in.</param>
		/// <param name="start">The starting bit index.</param>
		/// <param name="count">The number of bits the value is stored in.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <returns>The uncompressed floating point value.</returns>
		public float ReadRange(bool[] data,int start,int count,float min,float max){
			int value=ReadFromBits(data,start,count);
			return ReadRange(value,ValueMax(count),min,max);
		}
		
		/// <summary>Reverses Writer.WriteRange. Converts a compressed float back into a floating point.</summary>
		/// <param name="value">The compressed float.</param>
		/// <param name="valueMax">The maximum number that value can be.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <returns>The uncompressed floating point value.</returns>
		public float ReadRange(int value,int valueMax,float min,float max){
			return (((max-min)*value)/valueMax)+min;
		}
		
		/// <summary>Unmaps a single byte rotation.</summary>
		/// <returns>The unmapped rotation in degrees.</returns>
		public float UnmapRotation(){
			return (float)((ReadByte()/255f)*360f)-180f;
		}
		
		/// <summary>Gets how many bytes are left in this stream, if it can be known.</summary>
		/// <returns>The bytes left.</returns>
		public long BytesLeft(){
			return (Length-BaseStream.Position);
		}
		
		/// <summary>Checks if there are at least the given number of bytes more in this stream.</summary>
		/// <returns>True if there is more bytes; false otherwise.</returns>
		public bool More(int manyMore){
			return (BaseStream.Position+manyMore)<=Length;
		}
		
		/// <summary>Checks if there are any more bytes in this stream.</summary>
		/// <returns>True if there is more bytes; false otherwise.</returns>
		public bool More(){
			return BaseStream.Position<Length;
		}
		
		/// <summary>Checks if there are any more bytes in this stream.</summary>
		/// <returns>True if there is more bytes; false otherwise.</returns>
		public bool IsMore{
			get{
				return BaseStream.Position<Length;
			}
		}
		
		/// <summary>Gets the current position in the stream this reader is at.</summary>
		/// <returns>The current stream position.</returns>
		public long GetPosition(){
			return BaseStream.Position;
		}
		
		public long Position{
			get{
				return BaseStream.Position;
			}
			set{
				BaseStream.Position=value;
			}
		}
		
		/// <summary>Reads the bits from the given byte as a block of booleans.</summary>
		/// <param name="byteValue">The byte to read the bits from.</param>
		/// <returns>A block of 8 booleans. Lowest bit is at the highest index.</returns>
		public bool[] ReadBitsFrom(byte byteValue){
			bool[] result=new bool[8];
			int value=(int)byteValue;
			for(int i=7;i>=0;i--){
				result[i]=((value&1)==1);
				value=value>>1;
			}
			return result;
		}
		
		/// <summary>Converts the given block of bits into an integer.</summary>
		/// <param name="bits">The bits to read from. The highest index is the lowest bit.</param>
		/// <returns>The bits converted into an integer.</returns>
		public int ReadFromBits(bool[] bits){
			return ReadFromBits(bits,0,bits.Length);
		}
		
		/// <summary>Converts the given portion of a block of bits into an integer.</summary>
		/// <param name="bits">The bits to read from. The highest index is the lowest bit.</param>
		/// <param name="start">The index of where to start. This is the location of the highest bit of the value.</param>
		/// <param name="length">How many bits represent this value.</param>
		/// <returns>The bits converted into an integer.</returns>
		public int ReadFromBits(bool[] bits,int start,int length){
			int o=0;
			for(int i=0;i<length;i++){
				o=o<<1;
				if(bits[i+start]){
					o=(o|1);
				}
			}
			return o;
		}
		
		/// <summary>Reads the given number of bytes from the stream into a set of bits.</summary>
		/// <param name="bytes">The number of bytes to read from the stream.</param>
		/// <returns>The bytes as a block of bits.</returns>
		public bool[] ReadBits(int bytes){
			int t=bytes*8;
			bool[] outp=new bool[t];
			int up2=0;
			for(int e=0;e<t;e+=8){
				bool[] temp=ReadBitsFrom(ReadByte());
				for(int r=0;r<8;r++){
					outp[up2]=temp[r];
					up2++;
				}
			}
			return outp;
		}
		
		public void Seek(long to){
			BaseStream.Seek(to,SeekOrigin.Begin);
		}
		
		/// <summary>Seek the stream to the given location.</summary>
		/// <param name="to">The location to seek to.</param>
		/// <param name="origin">Where your location is relative to.</param>
		public void Seek(int to,SeekOrigin origin){
			BaseStream.Seek((long)to,origin);
		}
		
	}
	
}