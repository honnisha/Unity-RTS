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
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace Svg{
	
	/// <summary>
	/// The base interface for SVG lists.
	/// </summary>
	
	public class SVGListInterface<T>:SVGSerializable where T:ISVGListable{
		
		/// <summary>The raw values.</summary>
		private List<T> Values=new List<T>();
		
		
		public SVGListInterface(){}
		
		public SVGListInterface(bool readOnly){
			IsReadOnly=readOnly;
		}
		
		public SVGListInterface(bool readOnly,Css.Value init,Node host,string attr){
			IsReadOnly=readOnly;
			HostNode=host;
			AttributeName=attr;
			
			// Load each one now:
			if(init==null){
				return;
			}
			
			for(int i=0;i<init.Count;i++){
				
				// Create it:
				T v=Create(init[i]);
				
				// Attach and add:
				v.Attach(this);
				Values.Add(v);
				
			}
		}
		
		/// <summary>Creates an instance of the listed type from the given information in a CSS value.</summary>
		protected virtual T Create(Css.Value from){
			return default(T);
		}
		
		/// <summary>The length of the list.</summary>
		public int length{
			get{
				return Values.Count;
			}
		}
		
		/// <summary>The number of items.</summary>
		public int numberOfItems{
			get{
				return Values.Count;
			}
		}
		
		/// <summary>If the list reflects an attrib, this reserialises it.</summary>
		public override void Reserialize(){
			
			if(HostNode==null){
				return;
			}
			
			// Create builder:
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			int count=Values.Count;
			
			for(int i=0;i<count;i++){
				
				// Separated by spaces:
				if(i!=0){
					sb.Append(' ');
				}
				
				// Serialize it:
				Values[i].Serialize(sb);
				
			}
			
			// Output it:
			HostNode.setAttribute(AttributeName, sb.ToString());
			
		}
		
		/// <summary>Clears out the list.</summary>
		public void clear(){
			
			if(IsReadOnly){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			foreach(T value in Values){
				
				// Detach it:
				value.Detach();
				
			}
			
			// Clear:
			Values.Clear();
			
			// If attrib, reserialize:
			Reserialize();
			
		}
		
		/// <summary>Sets up the list with the given object in it.</summary>
		public T initialize(T newItem){
			
			if(IsReadOnly){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			foreach(T value in Values){
				
				// Detach it:
				value.Detach();
				
			}
			
			// Clear:
			Values.Clear();
			
			// Append:
			newItem.Attach(this);
			Values.Add(newItem);
			
			// If attrib, reserialize:
			Reserialize();
			
			return newItem;
		}
		
		public T getItem(int index){
			if(index<0 || index>=Values.Count){
				throw new DOMException(DOMException.INDEX_SIZE_ERR);
			}
			
			return Values[index];
		}
		
		public T insertItemBefore(T newItem,int index){
			if(IsReadOnly){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			if(index>Values.Count){
				Values.Add(newItem);
			}else{
				Values.Insert(index,newItem);
			}
			
			newItem.Attach(this);
			Reserialize();
			return newItem;
			
		}
		
		/// <summary>Replaces item at the given index with the given item.</summary>
		public T replaceItem(T newItem,int index){
			if(IsReadOnly){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			if(index<0 || index>=Values.Count){
				throw new DOMException(DOMException.INDEX_SIZE_ERR);
			}
			
			Values[index].Detach();
			Values[index]=newItem;
			newItem.Attach(this);
			Reserialize();
			return newItem;
		}
		
		/// <summary>Removes the item at the given index.</summary>
		public T removeItem(int index){
			
			if(IsReadOnly){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			if(index<0 || index>=Values.Count){
				throw new DOMException(DOMException.INDEX_SIZE_ERR);
			}
			
			T item=Values[index];
			item.Detach();
			Values.RemoveAt(index);
			Reserialize();
			return item;
			
		}
		
		/// <summary>Adds the item to the list.</summary>
		public T appendItem(T newItem){
			
			if(IsReadOnly){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			Values.Add(newItem);
			newItem.Attach(this);
			Reserialize();
			return newItem;
		}
		
		public T this[int index]{
			get{
				return getItem(index);
			}
			set{
				// Same as replaceItem:
				replaceItem(value,index);
			}
		}
		
	}
	
	public class SVGSerializable{
		
		/// <summary>Is this list readonly?</summary>
		protected bool IsReadOnly;
		/// <summary>The node being reflected (if any).</summary>
		public Dom.Node HostNode;
		/// <summary>The attribute being reflected (if any) in the host node.</summary>
		public string AttributeName;
		
		
		public virtual void Reserialize(){}
		
	}
	
	/// <summary>A node which can be present in an SVG list.</summary>
	public interface ISVGListable{
		
		void Detach();
		void Attach(SVGSerializable list);
		void Serialize(System.Text.StringBuilder sb);
		
	}
	
}