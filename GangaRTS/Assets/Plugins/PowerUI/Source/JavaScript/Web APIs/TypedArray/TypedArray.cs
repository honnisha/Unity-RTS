using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace PowerUI
{
	/// <summary>
	/// 
	/// </summary>
	public class TypedArray
	{
		
		/// <summary>
		/// The number of bytes per element in the array. Read-only.
		/// </summary>
		public int BYTES_PER_ELEMENT;
		
		/// <summary>
		/// The length of the array. Read-only.
		/// </summary>
		public int Length;
		
		/// <summary>
		/// The number of bytes that form this array. Read-only.
		/// </summary>
		public int ByteLength;
		
		/// <summary>
		/// The offset to the start of buffer.
		/// </summary>
		public int ByteOffset;
		
		/// <summary>
		/// The underlying buffer.
		/// Note that this may be created on demand.
		/// </summary>
		protected ArrayBuffer _Buffer;
		
		/// <summary>
		/// The underlying buffer.
		/// Note that this may be created on demand.
		/// </summary>
		public ArrayBuffer buffer{
			get{
				if(_Buffer==null)
				{
					// Create the buffer now:
					_Buffer=new ArrayBuffer(ByteLength);
				}
				
				return _Buffer;
			}
		}
		
		/// <summary>
		/// Transfers the contents of this array instance into the given buffer.
		/// This occurs when a 'native' ushort[] etc are in use and a byte[] is required.
		/// </summary>
		protected virtual void FillBuffer()
		{
		}
		
		/// <summary>
		/// The type of typed array that this is.
		/// </summary>
		internal TypedArrayStyle Style;

		//	 INITIALIZATION
		//_________________________________________________________________________________________
		
		internal TypedArray(TypedArrayStyle style,object iterableObj):this(style,GetLength(iterableObj))
		{
		}
		
		internal TypedArray(TypedArrayStyle style,TypedArray array):this(style,(int)array.Length)
		{
		}
		
		private static int GetLength(object iterableObj)
		{
			var arr = iterableObj as System.Array;
			if(arr != null){
				return arr.Length;
			}
			
			var jsValue = iterableObj as Jint.Native.JsValue;
			
			if(jsValue != null && jsValue.IsArray()){
				return (int)jsValue.AsArray().GetLength();
			}
			
			return 0;
		}
		
		/// <summary>Adds all values in the given array into this array.</summary>
		protected void Add(object iterableObj)
		{
			System.Array arr = iterableObj as System.Array;
			int index=0;
			foreach(var value in arr)
			{
				var jsValue = value as Jint.Native.JsValue;
				if(jsValue != null){
					if(jsValue.IsNumber()){
						Set(index, jsValue.AsNumber());
					}
					index++;
					continue;
				}
				Set(index,value);
				index++;
			}
		}
		
		/// <summary>Adds all values in the given array into this array.</summary>
		protected void Add(TypedArray array)
		{
			for(int index=0;index<array.Length;index++)
			{
				Set(index,Get(index));
			}
		}
		
		/// <summary>Gets the value at the given index.</summary>
		protected virtual object Get(int index)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>Puts the given value into this array at the given index.</summary>
		protected virtual void Set(int index,object value)
		{
			throw new NotImplementedException();
		}
		
		/// <summary>
		/// Creates an empty typed array instance for use as a prototype.
		/// </summary>
		/// <param name="constructor"> A reference to the constructor that owns the prototype. </param>
		internal TypedArray(TypedArrayStyle style,int length)
		{
			Style=style;
			int bytesPerElement;
			
			switch (style)
			{
				case TypedArrayStyle.Int8Array:
				case TypedArrayStyle.Uint8Array:
				case TypedArrayStyle.Uint8ClampedArray:
					bytesPerElement = 1;
					break;
				case TypedArrayStyle.Int16Array:
				case TypedArrayStyle.Uint16Array:
					bytesPerElement = 2;
					break;
				case TypedArrayStyle.Int32Array:
				case TypedArrayStyle.Uint32Array:
				case TypedArrayStyle.Float32Array:
					bytesPerElement = 4;
					break;
				case TypedArrayStyle.Float64Array:
					bytesPerElement = 8;
					break;
				default:
					throw new NotSupportedException("Unsupported TypedArray style '"+style+"'.");
			}
			
			BYTES_PER_ELEMENT=bytesPerElement;
			Length=length;
			ByteLength=length * BYTES_PER_ELEMENT;
		
		}
		
		//	 JAVASCRIPT FUNCTIONS
		//_________________________________________________________________________________________

		/// <summary>
		/// Fills all the elements of a typed array from a start index to an end index with a
		/// static value.
		/// </summary>
		/// <param name="value"> The value to fill the typed array with. </param>
		/// <param name="start"> Optional. Start index. Defaults to 0. </param>
		/// <param name="end"> Optional. End index (exclusive). Defaults to the length of the array. </param>
		/// <returns></returns>
		
		public string Fill(int value, int start, int end)
		{
			
			if(end==0)
			{
				end=Length;
			}
			
			throw new NotImplementedException();
		}
		
	}
}
