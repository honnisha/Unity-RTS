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
using UnityEngine;
using Dom;


namespace PowerUI{
	
	public class FileList : IEnumerable<WebFile>{
		
		internal List<WebFile> values=new List<WebFile>();
		
		/// <summary>Adds the given node to the list.</summary>
		public void push(WebFile node){
			values.Add(node);
		}
		
		/// <summary>The number of nodes in the list.</summary>
		public int length{
			get{
				return values.Count;
			}
		}
		
		/// <summary>Gets a node at a particular index.</summary>
		public WebFile item(int index){
			return values[index];
		}
		
		/// <summary>Gets an element at the specified index.</summary>
		public WebFile this[int index]{
			get{
				return values[index];
			}
			internal set{
				values[index]=value;
			}
		}
		
		public IEnumerator<WebFile> GetEnumerator(){
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
	public class WebFile : DataTransferItem{
		
		public override string kind{
			get{
				return "file";
			}
		}
		
		public override string type{
			get{
				return "Files";
			}
		}
		
	}
	
}