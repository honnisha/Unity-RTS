using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PowerUI
{
	/// <summary>
	/// An int32 array.
	/// </summary>
	public class Int32Array : TypedArray
	{
		
		/// <summary>An internal buffer.</summary>
		private int[] rawBuffer;
		
		// new Int32Array(length);
		// new Int32Array(typedArray);
		// new Int32Array(object);
		// new Int32Array(buffer[, byteOffset[, length]]);
		
		public Int32Array(int length):base(TypedArrayStyle.Int32Array, length){
			
			// Create the fast buffer:
			rawBuffer=new int[Length];
			
		}
		
		public Int32Array(double length):base(TypedArrayStyle.Int32Array, (int)length){
			
			// Create the fast buffer:
			rawBuffer=new int[Length];
			
		}
		
		public Int32Array(TypedArray array):base(TypedArrayStyle.Int32Array, array)
		{
			
			// Create the quick buffer:
			rawBuffer=new int[Length];
			
			// Now add the array:
			Add(array);
			
		}
		
		public Int32Array(object iterableObj):base(TypedArrayStyle.Int32Array, iterableObj)
		{
			
			// Create the quick buffer:
			rawBuffer=new int[Length];
			
			// Now add it:
			Add(iterableObj);
			
		}
		
		public Int32Array(ArrayBuffer buff):this(buff,0,0){}
		
		public Int32Array(ArrayBuffer buff,int byteOffset,int length):base(TypedArrayStyle.Int32Array, length==0?buff.ByteLength:length)
		{
			ByteOffset=byteOffset;
			_Buffer=buff;
		}
		
		protected override void FillBuffer()
		{
			int length=Length;
			int byteOffset=ByteOffset;
			
			for(int i=0;i<length;i++)
			{
				var value=rawBuffer[i];
				LittleConverter.GetBytes(value,_Buffer.buffer,byteOffset);
				byteOffset+=4;
			}
			
			// Remove the fast buffer:
			rawBuffer=null;
		}
		
		/// <summary>Gets the value at the given index.</summary>
		protected override object Get(int index)
		{
			if(rawBuffer==null)
			{
				// Use the _Buffer instead:
				return LittleConverter.ToInt32(_Buffer.buffer,(index * 4) + ByteOffset);
			}
			return rawBuffer[index];
		}
		
		/// <summary>
		/// Puts an unknown object into this array.
		/// Note that the value is always expected to be a value type.
		/// </summary>
		protected override void Set(int index,object value)
		{
			int sValue = (value is double) ? (int)((double)value) : (int)value;
			
			if(rawBuffer==null)
			{
				// Use the _Buffer instead:
				LittleConverter.GetBytes(sValue,_Buffer.buffer,(index * 4) + ByteOffset);
				return;
			}
			// Get it as an int and put it in:
			rawBuffer[index]=sValue;
		}
		
		/// <summary>
		/// Gets or sets the given entry in the array.
		/// </summary>
		public int this[int index]{
			get{
				if(rawBuffer==null)
				{
					// Use the _Buffer instead:
					return LittleConverter.ToInt32(_Buffer.buffer,(index * 4) + ByteOffset);
				}
				return rawBuffer[index];
			}
			set{
				
				if(rawBuffer==null)
				{
					// Use the _Buffer instead:
					LittleConverter.GetBytes(value,_Buffer.buffer,(index * 4) + ByteOffset);
					return;
				}
				
				rawBuffer[index]=value;
			}
		}
		
		/*
		public Int32Array Subarray(){
			return Subarray(0,int.MaxValue);
		}
		
		public Int32Array Subarray(int begin){
			return Subarray(begin,int.MaxValue);
		}
		*/
		
        /// <summary>
        /// Returns a new typed array on the same ArrayBuffer store and with the same element types
        /// as for this TypedArray object. The begin offset is inclusive and the end offset is
        /// exclusive.
        /// </summary>
        /// <param name="begin"> Element to begin at. The offset is inclusive. </param>
        /// <param name="end"> Element to end at. The offset is exclusive. If not specified, all
        /// elements from the one specified by begin to the end of the array are included in the
        /// new view. </param>
        /// <returns> A new typed array that shares the same ArrayBuffer store. </returns>
		public Int32Array Subarray(int begin, int end){
			
			// Constrain the input parameters to valid values.
			begin = begin < 0 ? Math.Max(this.Length + begin, 0) : Math.Min(begin, this.Length);
			end = end < 0 ? Math.Max(this.Length + end, 0) : Math.Min(end, this.Length);
			int newLength = Math.Max(end - begin, 0);
			
			// Create a new typed array that uses the same ArrayBuffer.
			return new Int32Array(this.buffer, this.ByteOffset + begin * BYTES_PER_ELEMENT, newLength);
		}
		
		/// <summary>Creates a Int32Array from the given iterable object.</summary>
		public static Int32Array From(object value)
		{
			return new Int32Array(value);
		}
		
	}
	
}