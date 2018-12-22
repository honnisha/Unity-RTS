//--------------------------------------
//   Kulestar Standard Binary Helpers
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Text;
using System.IO;


namespace BinaryIO{
	
	/// <summary>
	/// This class is a wrapper for writing to a block of bytes.
	/// </summary>
	
	public partial class Writer:System.IO.BinaryWriter{
		
		/// <summary>The verification byte. ASCII 'E'. Also in NetworkClient.Message</summary>
		internal const byte NetVerify=69;
		
		/// <summary>A block of bytes that represents 0 as a short.</summary>
		public static byte[] EmptyInt16=new byte[2];
		/// <summary>A block of bytes that represents 0 as an int.</summary>
		public static byte[] EmptyInt32=new byte[4];
		
		
		/// <summary>Creates a new writer. This can be as long as you want it to be.</summary>
		public Writer():base(new MemoryStream()){}
		
		/// <summary>Creates a new writer to the given stream.</summary>
		/// <param name="stream">The stream to write to.</param>
		public Writer(Stream stream):base(stream){}
		
		/// <summary>Creates a fixed length writer. It can't write more data than the given number of bytes.</summary>
		/// <param name="size">The maximum size of the writer in bytes.</param>
		public Writer(int size):base(new MemoryStream(size)){}
		
		/// <summary>Gets the current position the writer is at in the stream.</summary>
		/// <returns>The position in the stream.</returns>
		public long Position(){
			return BaseStream.Position;
		}
		
		/// <summary>Seeks to the given position in the stream relative to the start.</summary>
		/// <param name="point">The location to seek to.</param>
		public void Seek(long point){
			Seek(point,SeekOrigin.Begin);
		}
		
		/// <summary>Seeks to the given position in the stream.</summary>
		/// <param name="to">The location to seek to.</param>
		/// <param name="about">The position that the given location is relative to.</param>
		public void Seek(long to,SeekOrigin about){
			BaseStream.Seek(to,about);
		}
		
		/// <summary>Writes the given byte to the stream.</summary>
		/// <param name="b">The byte to write.</param>
		public void WriteByte(byte b){
			Write(b);
		}
		
		#if !NETFX_CORE
		/// <summary>Debugs this writer. Returns bytes as text.</summary>
		public string Debug(){
			
			// Raw byte block:
			byte[] res=(BaseStream as MemoryStream).GetBuffer();
			
			StringBuilder str=new StringBuilder();
			
			for(int i=0;i<res.Length;i++){
				
				str.Append(res[i].ToString());
				
				if(res[i]>10){
					str.Append(" "+((char)res[i]));
				}
				
				str.Append("\n");
				
			}
			
			return str.ToString();
			
		}
		#endif
		
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
		
		/// <summary>Writes a number compressed into the stream, taking a minimum of a single byte.</summary>
		/// <param name="l">The number to write.</param>
		public void WriteCompressed(ulong l){
			if (l< 251){
				Write((byte)l);
			}else if (l <= ushort.MaxValue){
				Write((byte)251);
				Write((ushort)l);
			}else if (l < 16777216L){
				Write((byte)252);
				WriteUInt24((uint)l);
			}else if(l <= uint.MaxValue){
				Write((byte)253);
				Write((uint)l);
			}else{
				Write((byte)254);
				Write(l);
			}
		}
		
		/// <summary>Writes an unsigned number compressed into the stream, taking a minimum of a single byte.</summary>
		/// <param name="value">The number to write.</param>
		public void WriteCompressedSigned(long value){
			
			if(value<0){
				ulong val=(ulong)-value;
				WriteCompressed((val<<1)|1);
			}else{
				WriteCompressed(((ulong)value<<1));
			}
			
		}
		
		public void WriteInt24(int number){
			number+=(int.MaxValue>>8);
			for(int x=0;x<3;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public void WriteInt40(long number){
			number+=(long.MaxValue>>24);
			for(int x=0;x<5;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public void WriteInt48(long number){
			number+=(long.MaxValue>>16);
			for(int x=0;x<6;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public void WriteInt56(long number){
			number+=(long.MaxValue>>8);
			for(int x=0;x<7;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public void WriteUInt24(uint number){
			for(int x=0;x<3;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public void WriteUInt40(ulong number){
			for(int x=0;x<5;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public void WriteUInt48(ulong number){
			for(int x=0;x<6;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public void WriteUInt56(ulong number){
			for(int x=0;x<7;x++){
				Write((byte)(number&0xff));
				number>>=8;
			}
		}
		
		public override void Write(string str){
			WriteString(str);
		}
		
		/// <summary>Writes a string to the stream of any length.</summary>
		/// <param name="str">The string to write.</param>
		public void WriteString(string str){
			if(str==null){
				WriteCompressed(0);
				return;
			}
			byte[] bytes=Reader.TextEncoding.GetBytes(str);
			WriteCompressed((uint)bytes.Length);
			Write(bytes);
		}
		
		/// <summary>Writes a long string to the stream. Maximum length of 4GB.</summary>
		/// <param name="str">The string to write.</param>
		public void WriteLongString(string str){
			if(str==null){
				Write((uint)0);
				return;
			}
			Write((uint)str.Length);
			Write(Reader.TextEncoding.GetBytes(str));
		}
		
		/// <summary>Gets the content written to the stream as a block of bytes.</summary>
		/// <returns>The block of bytes.</returns>
		public virtual byte[] GetResult(){
			byte[] r=((MemoryStream)BaseStream).ToArray();
			#if !NETFX_CORE
			BaseStream.Close();
			#endif
			BaseStream.Dispose();
			return r;
		}
		
		public void WriteMessage(Writer writer){
			
			// Get the message:
			byte[] msg=writer.GetResult();
			
			// Write it's header:
			Write(msg[0]);
			Write(msg[1]);
			
			// Write the size:
			int size=(msg.Length-2);
			Write((int)size);
			
			// Write the rest:
			Write(msg,2,size);
			
		}
		
		public void Write(Writer writer){
			
			Write(writer.GetResult());
			
		}
		
		/// <summary>Writes the given floating point value to the given portion of a block of bits.
		/// The floating point value is mapped into a range first - between min and max.
		/// Note that this does not write the bits to the stream.</summary>
		/// <param name="data">The bits that the value will be written to.</param>
		/// <param name="value">The floating point value that will be written.</param>
		/// <param name="start">The starting index in the bit array for this value.</param>
		/// <param name="count">The number of bits this value can take up. More results in higher accuracy.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		public void WriteRange(bool[] data,float value,int start,int count,float min,float max){
			WriteToBits(data,WriteRange(value,count,min,max),start,count);
		}
		
		/// <summary>Maps the given floating point value into a range - between min and max.</summary>
		/// <param name="value">The floating point value that will be mapped.</param>
		/// <param name="count">The number of bits the mapped value can use. More results in higher accuracy.</param>
		/// <param name="min">The minimum value of the range.</param>
		/// <param name="max">The maximum value of the range.</param>
		/// <returns>The mapped float value.</returns>
		public int WriteRange(float value,int count,float min,float max){
			return (int)(((value-min)*Reader.ValueMax(count))/(max-min));
		}
		
		/// <summary>Writes the given input number to the given portion of a block of bits.</summary>
		/// <param name="bits">The bits to write the number to.</param>
		/// <param name="input">The number to write to the bits.</param>
		/// <param name="start">The start location in the bit array to write the number to.</param>
		/// <param name="length">The number of bits to use to write the number to.</param>
		public void WriteToBits(bool[] bits,int input,int start,int length){
			for(int i=length+start-1;i>=start;i--){
				bits[i]=((input&1)==1);
				input=input>>1;
			}
		}
		
		/// <summary>Writes the given block of bits to the stream. Must be a multiple of 8.</summary>
		/// <param name="bits">The bits to write. 8 bits are grouped together and written as a byte.</param>
		public void WriteBits(bool[] bits){
			int length=bits.Length;
			for(int i=0;i<length;i+=8){
				Write((byte)ReadFromBits(bits,i,8));
			}
		}
		
		/// <summary>Writes the given block of bits to the stream. It can be any length.</summary>
		/// <param name="bits">The bits to write. 8 bits are grouped together and written as a byte.</param>
		public void WriteRoundBits(bool[] bits){
			int length=bits.Length;
			for(int i=0;i<length;i+=8){
				int bitsToWrite=length-i;
				if(bitsToWrite>8){
					bitsToWrite=8;
				}
				
				Write((byte)ReadFromBits(bits,i,bitsToWrite));
			}
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

		/// <summary>Seeks to the end of the stream.</summary>
		public void SeekToEnd(){
			Seek(0,SeekOrigin.End);
		}
		
		/// <summary>Writes a network header to this writer, returning it's size.</summary>
		/// <param name="type">The message type.</param>
		/// <param name="header">The header block.</param>
		/// <param name="verify">True if a verification 'E' should be added.</param>
		/// <returns>The size of the header.</returns>
		public int WriteNetHeader(byte type){
			Write(type);
			Write(NetVerify);
			return 2;
		}
		
		/// <summary>Starts off a section that has a number representing its size in bytes before it.
		/// This size has a maximum of 64kb as it's stored as a ushort.</summary>
		/// <returns>A number you should pass to EndPackageUInt16 when your done.</returns>
		public long StartPackageUInt16(){
			Write(EmptyInt16);
			return Length();
		}
		
		/// <summary>Ends a section that has a number representing the sections size in bytes before it.</summary>
		/// <param name="location">The value you received from StartPackageUInt16.</param>
		public void EndPackageUInt16(long location){
			// Location is the point after the number.
			// So the length of the data we wrote is just:
			long length=Length()-location;
			Seek(location-2,SeekOrigin.Begin);
			Write((ushort)length);
			// And seek back to the end:
			Seek(0,SeekOrigin.End);
		}
		
		/// <summary>Starts off a section that has a number representing its size in bytes before it.
		/// This size has a maximum of 2GB as it's stored as an int.</summary>
		/// <returns>A number you should pass to EndPackageInt32 when your done.</returns>
		public long StartPackageInt32(){
			Write(EmptyInt32);
			return Length();
		}
		
		/// <summary>Ends a section that has a number representing the sections size in bytes before it.</summary>
		/// <param name="location">The value you received from StartPackageInt32.</param>
		public void EndPackageInt32(long location){
			// Location is the point after the number.
			// So the length of the data we wrote is just:
			long length=Length()-location;
			Seek(location-4,SeekOrigin.Begin);
			Write((int)length);
			// And seek back to the end:
			Seek(0,SeekOrigin.End);
		}
		
		/// <summary>Ends a block noted with the given value from StartBlock.
		/// When you call this, you must then write your length value and then call EndBlockLength.</summary>
		/// <param name="location">The value from StartBlock which defines where the block is.</param>
		public void EndBlock(long location){
			Seek(location,SeekOrigin.Begin);
		}
		
		/// <summary>Call when your done writing the length of a block. See StartBlock.</summary>
		public void EndBlockLength(){
			SeekToEnd();
		}
		
		/// <summary>Starts a block. That's a section in the stream of unknown length which must
		/// have the length written at the start of it.</summary>
		/// <param name="byteCount">The number of bytes to leave for the length.</param>
		/// <returns>A value you must pass to EndBlock.</returns>
		public long StartBlock(int byteCount){
			long location=BaseStream.Position;
			for(int i=0;i<byteCount;i++){
				Write((byte)0);
			}
			return location;
		}
		
		/// <summary>Gets the current length of the stream.</summary>
		/// <returns>The length ot the stream.</returns>
		public long Length(){
			return BaseStream.Length;
		}
		
		/// <summary>Maps a rotation into a single byte then writes it to the stream.</summary>
		/// <param name="rotation">The rotation to write (in degrees).</param>
		public void MapRotation(float rotation){
			if(rotation<-180f){
				rotation=360f+rotation;
			}else if(rotation>180f){
				rotation-=360f;
			}
			Write((byte)(((rotation+180f)/360f)*255));
		}
		
	}
	
}