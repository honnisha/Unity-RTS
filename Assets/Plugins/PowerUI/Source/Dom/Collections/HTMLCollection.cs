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
using System.Globalization;
using System.Collections;
using System.Collections.Generic;


namespace Dom{

	/// <summary>
	/// A collection of HTML elements.
	/// </summary>
	public partial class HTMLCollection : ExpandableObject, INodeList,IEnumerable<Element>{
		
		List<Element> values=new List<Element>();
		
		
		public int length{
			get{
				return values.Count;
			}
		}
		
		/// <summary>The index of the given node.</summary>
		public int indexOf(Node node){
			return values.IndexOf(node as Element);
		}
		
		/// <summary>Removes the given node.</summary>
		public void remove(Node node){
			
			Element el=node as Element;
			
			if(el==null){
				return;
			}
			
			values.Remove(el);
		}
		
		/// <summary>Insert at the given index.</summary>
		public void insert(int index,Node node){
			
			Element el=node as Element;
			
			if(el==null){
				return;
			}
			
			values.Insert(index,el);
		}
		
		public void push(Node node){
			
			Element el=node as Element;
			
			if(el==null){
				return;
			}
			
			values.Add(el);
			
		}
		
		public virtual Element namedItem(string name){
			
			for(int i=0;i<values.Count;i++){
				
				if(values[i].id==name){
					return values[i];
				}
				
			}
			
			// Last resort - try name this time:
			
			for(int i=0;i<values.Count;i++){
				
				if(values[i].getAttribute("name")==name){
					return values[i];
				}
				
			}
			
			// Or null:
			return null;
			
		}
		
		/// <summary>Gets a node at a particular index.</summary>
		public Element item(int index){
			
			if(index>=values.Count || index<0){
				return null;
			}
			
			return values[index];
			
		}
		
		/// <summary>Gets an element at the specified index.</summary>
		public Element this[int index]{
			get{
				return values[index];
			}
			internal set{
				values[index]=value;
			}
		}
		
		public IEnumerator<Element> GetEnumerator(){
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
}