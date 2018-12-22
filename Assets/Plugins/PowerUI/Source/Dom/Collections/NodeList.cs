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


namespace Dom{
	
	/// <summary>
	/// The NodeList interface.
	/// </summary>
	
	public interface INodeList{
		
		/// <summary>The number of nodes in the list.</summary>
		int length{get;}
		
		/// <summary>Adds a node.</summary>
		void push(Node node);
		
		#region Internal methods
		
		/// <summary>Removes a node.</summary>
		void remove(Node node);
		
		/// <summary>Inserts a node at the given index.</summary>
		void insert(int index, Node node);
		
		#endregion
		
	}
	
	public partial class NodeList : ExpandableObject,INodeList,IEnumerable<Node>{
		
		List<Node> values=new List<Node>();
		
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