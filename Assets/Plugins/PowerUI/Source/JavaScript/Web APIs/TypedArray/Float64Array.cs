using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PowerUI
{
	/// <summary>
	/// A double32 array.
	/// </summary>
	public class Float64Array : TypedArray
	{
		
		/// <summary>An internal buffer.</summary>
		private double[] rawBuffer;
		
		// new Float64Array(length);
		// new Float64Array(typedArray);
		// new Float64Array(object);
		// new Float64Array(buffer[, byteOffset[, length]]);
		
		public Float64Array(int length):base(TypedArrayStyle.Float64Array, length){
			
			// Create the fast buffer:
			rawBuffer=new double[Length];
			
		}
		
		public Float64Array(double length):base(TypedArrayStyle.Float64Array, (int)length){
			
			// Create the fast buffer:
			rawBuffer=new double[Length];
			
		}
		
		public Float64Array(TypedArray array):base(TypedArrayStyle.Float64Array, array)
		{
			
			// Create the quick buffer:
			rawBuffer=new double[Length];
			
			// Now add the array:
			Add(array);
			
		}
		
		public Float64Array(object iterableObj):base(TypedArrayStyle.Float64Array, iterableObj)
		{
			
			// Create the quick buffer:
			rawBuffer=new double[Length];
			
			// Now add it:
			Add(iterableObj);
			
		}
		
		public Float64Array(ArrayBuffer buff):this(buff,0,0){}
		
		public Float64Array(ArrayBuffer buff,int byteOffset,int length):base(TypedArrayStyle.Float64Array, length==0?buff.ByteLength:length)
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
				byte[] bytes=BitConverter.GetBytes(value);
				System.Array.Copy(bytes,0,_Buffer.buffer,byteOffset,8);
				byteOffset+=8;
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
				return BitConverter.ToDouble(_Buffer.buffer,(index * 8) + ByteOffset);
			}
			return rawBuffer[index];
		}
		
		/// <summary>
		/// Puts an unknown object into this array.
		/// Note that the value is always expected to be a value type.
		/// </summary>
		protected override void Set(int index,object value)
		{
			double sValue = (double)value;
			if(rawBuffer==null)
			{
				// Use the _Buffer instead (this is unfortunate!):
				byte[] bytes=BitConverter.GetBytes(sValue);
				System.Array.Copy(bytes,0,_Buffer.buffer,(index * 8) + ByteOffset,8);
				return;
			}
			// Get it as an int and put it in:
			rawBuffer[index]=sValue;
		}
		
		/// <summary>
		/// Gets or sets the given entry in the array.
		/// </summary>
		public double this[int index]{
			get{
				if(rawBuffer==null)
				{
					// Use the _Buffer instead:
					return BitConverter.ToDouble(_Buffer.buffer,(index * 8) + ByteOffset);
				}
				return rawBuffer[index];
			}
			set{
				if(rawBuffer==null)
				{
					// Use the _Buffer instead (this is unfortunate!):
					byte[] bytes=BitConverter.GetBytes(value);
					System.Array.Copy(bytes,0,_Buffer.buffer,(index * 8) + ByteOffset,8);
					return;
				}
				rawBuffer[index]=value;
			}
		}
		
		/// <summary>Creates a Float64Array from the given iterable object.</summary>
		public static Float64Array From(object value)
		{
			return new Float64Array(value);
		}
		
	}
	
}