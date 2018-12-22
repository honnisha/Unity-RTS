using System;
using System.Reflection;
using System.Runtime.InteropServices;


namespace WebAssembly{
	
	/// <summary>
	/// A WebAssembly Memory object.
	/// </summary>
	
	public class Memory{
		
		/// <summary>The WebAssembly page size.</summary>
		public const int PageSize=64 * 1024; // 64KiB
		
		/// <summary>The raw memory block.</summary>
		private byte[] Buffer_;
		/// <summary>True if it's been pinned.</summary>
		private bool Pinned_;
		/// <summary>The pinned pointer.</summary>
		private IntPtr Pointer_;
		/// <summary>The pin.</summary>
		private GCHandle Handle_;
		/// <summary>Current number of pages.</summary>
		private int Pages_;
		
		
		/// <summary>Convenience access to the memory block.</summary>
		public byte this[int index]{
			get{
				return Buffer_[index];
			}
			set{
				Buffer_[index]=value;
			}
		}
		
		public Memory(int size){
			// Create it:
			Buffer_=new byte[size];
			Pages_ = size / PageSize;
		}
		
		/// <summary>The current number of pages.</summary>
		public int Pages{
			get{
				return Pages_;
			}
		}
		
		/// <summary>Expands the available memory.</summary>
		public int Grow(int pageCount){
			// int prevPages = Pages;
			
			throw new NotImplementedException();
			
			// return prevPages;
		}
		
		/// <summary>Gets a pointer to the buffer (as a safe IntPtr).</summary>
		public IntPtr ToPointer(){
			
			if(Pinned_){
				return Pointer_;
			}
			
			Pinned_=true;
			
			// Pin the memory block:
			Handle_ = GCHandle.Alloc(Buffer_, GCHandleType.Pinned);
			Pointer_ = Handle_.AddrOfPinnedObject();
			
			return Pointer_;
		}
		
		~Memory(){
			if(Pinned_){
				Pinned_=false;
				Handle_.Free();
			}
		}
		
	}
	
}