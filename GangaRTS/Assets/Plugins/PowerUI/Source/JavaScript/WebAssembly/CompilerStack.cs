using System;
using System.Collections;
using System.Collections.Generic;


namespace WebAssembly{
	
	/// <summary>
	/// A stack of T objects.
	/// </summary>
	public class CompilerStack<T>{
		
		/// <summary>The current stack count.</summary>
		public int Count;
		/// <summary>The type set.</summary>
		private T[] Entries = new T[20];
		
		
		/// <summary>Peeks the top of the stack.</summary>
		public T Peek(){
			return Entries[Count-1];
		}
		
		/// <summary>Peeks at the given index relative to the top of the stack. 0 is the top.</summary>
		public T Peek(int index){
			return Entries[Count-1-index];
		}
		
		/// <summary>Pops multiple entries.</summary>
		public void Pop(int count){
			if(Count<count){
				throw new InvalidOperationException("WebAssembly failed - the stack was mangled.");
			}
			
			Count-=count;
		}
		
		/// <summary>Raw access to the entries. Index starts at the bottom of the stack.</summary>
		public T this[int index]{
			get{
				return Entries[index];
			}
		}
		
		/// <summary>Pops the given number of entries and then pushes the new top.</summary>
		public void Replace(int count,T newTop){
			if(Count<count){
				throw new InvalidOperationException("WebAssembly failed - the stack was mangled.");
			}
			Count-=count;
			Entries[Count++]=newTop;
		}
		
		/// <summary>Pops one and then pushes the new top.</summary>
		public void Replace(T newTop){
			Entries[Count-1]=newTop;
		}
		
		/// <summary>Grows the type stack.</summary>
		public void Grow(){
			
			// Create the new set:
			T[] newTypes = new T[Entries.Length * 2];
			Array.Copy(Entries,0,newTypes,0,Entries.Length);
			Entries = newTypes;
			
		}
		
		/// <summary>Pushes a type to the stack.</summary>
		public void Push(T type){
			
			// Grow if necessary:
			if(Count==Entries.Length){
				Grow();
			}
			
			// Push:
			Entries[Count++]=type;
		}
		
		/// <summary>Pops a type from the stack.</summary>
		public T Pop(){
			if(Count == 0){
				throw new InvalidOperationException("WebAssembly failed - the stack was mangled.");
			}
			
			return Entries[--Count];
		}
		
	}
	
}