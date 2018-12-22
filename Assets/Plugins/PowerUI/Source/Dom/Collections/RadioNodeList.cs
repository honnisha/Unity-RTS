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
	/// A list of radio nodes. Note that this is an element so it can be stored in a HTMLFormControlsCollection.
	/// </summary>
	public partial class RadioNodeList : Element,INodeList,IEnumerable<Node>{
		
		List<Node> values=new List<Node>();
		
		public RadioNodeList(HTMLCollection hc,string name){
			
			// Find all nodes with the given name/id in the collection:
			foreach(Element e in hc){
				
				if(e.getAttribute("type")!="radio"){
					continue;
				}
				
				if(e.getAttribute("id")==name || e.getAttribute("name")==name){
					values.Add(e);
				}
				
			}
			
		}
		
		/// <summary>The actively checked element, if any.</summary>
		public Element ActiveElement{
			get{
				// Get the node in the checked state:
				foreach(Node n in values){
					Element e=(n as Element);
					
					if(e!=null && e.GetBoolAttribute("checked")){
						return e;
					}
				}
				
				return null;
			}
		}
		
		/// <summary>The value of the radios.</summary>
		public string value{
			get{
				Element e=ActiveElement;
				
				if(e==null){
					return "";
				}
				
				return e.getAttribute("value");
			}
		}
		
		/// <summary>Removes the given index.</summary>
		public void removeAt(int index){
			values.RemoveAt(index);
		}
		
		/// <summary>Removes the given node.</summary>
		public void remove(Node node){
			values.Remove(node);
		}
		
		/// <summary>Insert at the given index.</summary>
		public void insert(int index,Node node){
			values.Insert(index,node);
		}
		
		/// <summary>Adds the given node to the list.</summary>
		public void push(Node node){
			values.Add(node);
		}
		
		/// <summary>The number of nodes in the list.</summary>
		public int length{
			get{
				return values.Count;
			}
		}
		
		/// <summary>Gets a node at a particular index.</summary>
		public Node item(int index){
			return values[index];
		}
		
		/// <summary>Gets an element at the specified index.</summary>
		public Node this[int index]{
			get{
				return values[index];
			}
			internal set{
				values[index]=value;
			}
		}
		
		public IEnumerator<Node> GetEnumerator(){
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
}