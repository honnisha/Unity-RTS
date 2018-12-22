using System;
using Jint.Runtime;
using Jint.Native;


namespace PowerUI
{
	/// <summary>
	/// The DataView view provides a low-level interface for reading and writing multiple number
	/// types in an ArrayBuffer irrespective of the platform's endianness.
	/// </summary>
	public partial class DataView
	{
		private ArrayBuffer buffer;
		private int byteOffset;
		private int byteLength;


		//	 INITIALIZATION
		//_________________________________________________________________________________________
		
		/// <summary>
		/// Creates a new DataView instance.
		/// </summary>
		/// <param name="prototype"> The next object in the prototype chain. </param>
		/// <param name="buffer"> An existing ArrayBuffer to use as the storage for the new
		/// DataView object. </param>
		/// <param name="byteOffset"> The offset, in bytes, to the first byte in the specified
		/// buffer for the new view to reference. If not specified, the view of the buffer will
		/// start with the first byte. </param>
		/// <param name="byteLength"> The number of elements in the byte array. If unspecified,
		/// length of the view will match the buffer's length. </param>
		internal DataView(ArrayBuffer buffer, int byteOffset, int byteLength)
		{
			this.buffer = buffer;
			this.byteOffset = byteOffset;
			this.byteLength = byteLength;
		}
		
		//	 JAVASCRIPT PROPERTIES
		//_________________________________________________________________________________________

		/// <summary>
		/// The ArrayBuffer referenced by the DataView at construction time.
		/// </summary>
		public ArrayBuffer Buffer
		{
			get { return this.buffer; }
		}

		/// <summary>
		/// The offset (in bytes) of this view from the start of its ArrayBuffer.
		/// </summary>
		public int ByteOffset
		{
			get { return this.byteOffset; }
		}

		/// <summary>
		/// The length (in bytes) of this view from the start of its ArrayBuffer.
		/// </summary>
		public int ByteLength
		{
			get { return this.byteLength; }
		}
		
		//	 JAVASCRIPT FUNCTIONS
		//_________________________________________________________________________________________

		/// <summary>
		/// Gets a 32-bit floating point number at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <param name="littleEndian"> Indicates whether the number is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is read. </param>
		/// <returns> The 32-bit floating point number at the specified byte offset from the start
		/// of the DataView. </returns>
		public double GetFloat32(int byteOffset, bool littleEndian)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 4)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			
			byteOffset+=this.byteOffset;
			
			return BitConverter.ToSingle(buffer.Buffer,byteOffset);
			
		}

		/// <summary>
		/// Gets a 64-bit floating point number at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <param name="littleEndian"> Indicates whether the number is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is read. </param>
		/// <returns> The 64-bit floating point number at the specified byte offset from the start
		/// of the DataView. </returns>
		public double GetFloat64(int byteOffset, bool littleEndian)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 8)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			
			byteOffset+=this.byteOffset;
			
			return BitConverter.ToDouble(buffer.Buffer,byteOffset);
		}

		/// <summary>
		/// Gets a signed 16-bit integer at the specified byte offset from the start of the DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <param name="littleEndian"> Indicates whether the number is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is read. </param>
		/// <returns> The signed 16-bit integer at the specified byte offset from the start of the
		/// DataView. </returns>
		public int GetInt16(int byteOffset, bool littleEndian)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 2)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			
			byteOffset+=this.byteOffset;
			
			if (littleEndian)
			{
				return (short)(buffer.Buffer[byteOffset] | (buffer.Buffer[byteOffset+1] << 8));
			}
			else
			{
				return (short)((buffer.Buffer[byteOffset] << 8) | (buffer.Buffer[byteOffset+1]));
			}
		}

		/// <summary>
		/// Gets a signed 32-bit integer at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <param name="littleEndian"> Indicates whether the number is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is read. </param>
		/// <returns> The signed 32-bit integer at the specified byte offset from the start
		/// of the DataView. </returns>
		public int GetInt32(int byteOffset, bool littleEndian)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 4)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			
			byteOffset+=this.byteOffset;
			
			if (littleEndian)
			{
				return (int)(buffer.Buffer[byteOffset] | (buffer.Buffer[byteOffset+1] << 8) | 
					(buffer.Buffer[byteOffset+2] << 16) | (buffer.Buffer[byteOffset+3] << 24));
			}
			else
			{
				return (int)((buffer.Buffer[byteOffset] << 24) | (buffer.Buffer[byteOffset+1] << 16) |
					(buffer.Buffer[byteOffset+2] << 8) | (buffer.Buffer[byteOffset+3]));
			}
			
		}

		/// <summary>
		/// Gets a signed 64-bit integer at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <param name="littleEndian"> Indicates whether the number is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is read. </param>
		/// <returns> The signed 64-bit integer at the specified byte offset from the start
		/// of the DataView. </returns>
		public long GetInt64(int byteOffset, bool littleEndian)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 8)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
				
				byteOffset+=this.byteOffset;
			
			if (littleEndian)
			{
				return (long)((long)buffer.Buffer[byteOffset] | ((long)buffer.Buffer[byteOffset+1] << 8) | 
					((long)buffer.Buffer[byteOffset+2] << 16) | ((long)buffer.Buffer[byteOffset+3] << 24) |
					((long)buffer.Buffer[byteOffset+4] << 32) | ((long)buffer.Buffer[byteOffset+5] << 40) |
					((long)buffer.Buffer[byteOffset+6] << 48) | ((long)buffer.Buffer[byteOffset+7] << 56)
					);
			}
			else
			{
				return (long)((long)buffer.Buffer[byteOffset+7] | ((long)buffer.Buffer[byteOffset+6] << 8) | 
					((long)buffer.Buffer[byteOffset+5] << 16) | ((long)buffer.Buffer[byteOffset+4] << 24) |
					((long)buffer.Buffer[byteOffset+3] << 32) | ((long)buffer.Buffer[byteOffset+2] << 40) |
					((long)buffer.Buffer[byteOffset+1] << 48) | ((long)buffer.Buffer[byteOffset] << 56)
					);
				
			}
			
		}

		/// <summary>
		/// Gets a signed 8-bit integer (byte) at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <returns> The signed 8-bit integer (byte) at the specified byte offset from the start
		/// of the DataView. </returns>
		public int GetInt8(int byteOffset)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 1)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			return (sbyte)buffer.Buffer[this.byteOffset + byteOffset];
		}

		/// <summary>
		/// Gets an unsigned 8-bit integer (byte) at the specified byte offset from the start of
		/// the DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <param name="littleEndian"> Indicates whether the number is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is read. </param>
		/// <returns> The unsigned 8-bit integer (byte) at the specified byte offset from the start
		/// of the DataView. </returns>
		public int GetUint16(int byteOffset, bool littleEndian)
		{
			return (ushort)GetInt16(byteOffset, littleEndian);
		}

		/// <summary>
		/// Gets an unsigned 32-bit integer at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <param name="littleEndian"> Indicates whether the number is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is read. </param>
		/// <returns> The unsigned 32-bit integer at the specified byte offset from the start
		/// of the DataView. </returns>
		public uint GetUint32(int byteOffset, bool littleEndian)
		{
			return (uint)GetInt32(byteOffset, littleEndian);
		}

		/// <summary>
		/// Gets an unsigned 8-bit integer (byte) at the specified byte offset from the start of
		/// the DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// read the data. </param>
		/// <returns> The unsigned 8-bit integer (byte) at the specified byte offset from the start
		/// of the DataView. </returns>
		public int GetUint8(int byteOffset)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 1)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			return buffer.Buffer[this.byteOffset + byteOffset];
		}

		/// <summary>
		/// Stores a signed 32-bit float value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		/// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is written. </param>
		public void SetFloat32(int byteOffset, double value, bool littleEndian)
		{
			SetCore(byteOffset, BitConverter.GetBytes((float)value), littleEndian);
		}

		/// <summary>
		/// Stores a signed 64-bit float value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		/// <param name="littleEndian"> Indicates whether the 64-bit float is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is written. </param>
		public void SetFloat64(int byteOffset, double value, bool littleEndian)
		{
			SetCore(byteOffset, BitConverter.GetBytes(value), littleEndian);
		}

		/// <summary>
		/// Stores a signed 16-bit integer value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		/// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is written. </param>
		public void SetInt16(int byteOffset, JsValue value, bool littleEndian)
		{
			SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToInt16(value)), littleEndian);
		}

		/// <summary>
		/// Stores a signed 32-bit integer value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		/// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is written. </param>
		public void SetInt32(int byteOffset, JsValue value, bool littleEndian)
		{
			SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToInt32(value)), littleEndian);
		}

		/// <summary>
		/// Stores a signed 8-bit integer value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		public void SetInt8(int byteOffset, JsValue value)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 1)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			buffer.Buffer[this.byteOffset + byteOffset] = (byte)TypeConverter.ToInt8(value);
		}

		/// <summary>
		/// Stores an unsigned 16-bit integer value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		/// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is written. </param>
		public void SetUint16(int byteOffset, JsValue value, bool littleEndian)
		{
			SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToUint16(value)), littleEndian);
		}

		/// <summary>
		/// Stores an unsigned 32-bit integer value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		/// <param name="littleEndian"> Indicates whether the 32-bit float is stored in little- or
		/// big-endian format. If false or undefined, a big-endian value is written. </param>
		public void SetUint32(int byteOffset, JsValue value, bool littleEndian)
		{
			SetCore(byteOffset, BitConverter.GetBytes(TypeConverter.ToUint32(value)), littleEndian);
		}

		/// <summary>
		/// Stores an unsigned 8-bit integer value at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="value"> The value to set. </param>
		public void SetUint8(int byteOffset, JsValue value)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - 1)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			buffer.Buffer[this.byteOffset + byteOffset] = TypeConverter.ToUint8(value);
		}



		//	 .NET HELPER FUNCTIONS
		//_________________________________________________________________________________________

		/// <summary>
		/// Stores a series of bytes at the specified byte offset from the start of the
		/// DataView.
		/// </summary>
		/// <param name="byteOffset"> The offset, in bytes, from the start of the view where to
		/// store the data. </param>
		/// <param name="bytes"> The bytes to store. </param>
		/// <param name="littleEndian"> Indicates whether the bytes are stored in little- or
		/// big-endian format. If false, a big-endian value is written. </param>
		private void SetCore(int byteOffset, byte[] bytes, bool littleEndian)
		{
			if (byteOffset < 0 || byteOffset > this.byteLength - bytes.Length)
				throw new JavaScriptException("RangeError", "Offset is outside the bounds of the DataView.");
			if (littleEndian)
			{
				for (int i = 0; i < bytes.Length; i++)
					buffer.Buffer[this.byteOffset + byteOffset + i] = bytes[i];
			}
			else
			{
				for (int i = 0; i < bytes.Length; i++)
					buffer.Buffer[this.byteOffset + byteOffset + bytes.Length - 1 - i] = bytes[i];
			}
		}

	}
}
