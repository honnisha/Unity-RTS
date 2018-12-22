using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PowerUI
{
	/// <summary>
	/// An int8 array.
	/// Note that this could use an sbyte[] array at the cost of some potentially hefty duplication.
	/// </summary>
	public class Int8Array : TypedArray
	{
		
		/// <summary>A reference to the internal buffer.</summary>
		private byte[] rawBuffer;
		
		// new Int8Array(length);
		// new Int8Array(typedArray);
		// new Int8Array(object);
		// new Int8Array(buffer[, byteOffset[, length]]);
		
		public Int8Array(int length):base(TypedArrayStyle.Int8Array, length){
			
			// Always uses a buffer as they are a byte[] anyway:
			_Buffer=new ArrayBuffer(ByteLength);
			
			// Quick ref to the buffers buffer:
			rawBuffer=_Buffer.buffer;
			
		}
		
		public Int8Array(double length):base(TypedArrayStyle.Int8Array, (int)length){
			
			// Always uses a buffer as they are a byte[] anyway:
			_Buffer=new ArrayBuffer(ByteLength);
			
			// Quick ref to the buffers buffer:
			rawBuffer=_Buffer.buffer;
			
		}
		
		public Int8Array(TypedArray array):base(TypedArrayStyle.Int8Array, array)
		{
			
			// Create a new buffer:
			_Buffer=new ArrayBuffer(ByteLength);
			
			// Get a quick ref to the buffers buffer:
			rawBuffer=_Buffer.buffer;
			
			// Now add the array:
			Add(array);
			
		}
		
		public Int8Array(object iterableObj):base(TypedArrayStyle.Int8Array, iterableObj)
		{
			
			// Create a new buffer:
			_Buffer=new ArrayBuffer(ByteLength);
			
			// Get a quick ref to the buffers buffer:
			rawBuffer=_Buffer.buffer;
			
			// Now add it:
			Add(iterableObj);
			
		}
		
		public Int8Array(ArrayBuffer buff):this(buff,0,0){}
		
		public Int8Array(ArrayBuffer buff,int byteOffset,int length):base(TypedArrayStyle.Int8Array, length==0?buff.ByteLength:length)
		{
			ByteOffset=byteOffset;
			_Buffer=buff;
			rawBuffer=_Buffer.buffer;
		}
		
		/// <summary>Gets the value at the given index.</summary>
		protected override object Get(int index)
		{
			return (rawBuffer[index + ByteOffset]-128);
		}
		
		/// <summary>
		/// Puts an unknown object into this array.
		/// Note that the value is always expected to be a value type.
		/// </summary>
		protected override void Set(int index,object value)
		{
			// Get it as a byte and put it in:
			rawBuffer[index + ByteOffset] = (value is double) ? (byte)(((sbyte)((double)value))+128) : (byte)(((sbyte)value)+128);
		}
		
		/// <summary>
		/// Gets or sets the given entry in the array.
		/// </summary>
		public sbyte this[int index]{
			get{
				return (sbyte)(rawBuffer[index + ByteOffset]-128);
			}
			set{
				rawBuffer[index + ByteOffset]=(byte)(value+128);
			}
		}
		
		/// <summary>Creates a Int8Array from the given iterable object.</summary>
		public static Int8Array From(object value)
		{
			return new Int8Array(value);
		}
		
	}
	
}