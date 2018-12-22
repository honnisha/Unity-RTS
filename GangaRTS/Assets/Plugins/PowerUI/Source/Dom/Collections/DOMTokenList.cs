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
	/// A live collection of DOM Tokens (always strings).
	/// </summary>
	public partial class DOMTokenList : ExpandableObject, IEnumerable<string>{
		
		/// <summary>The node this comes from.</summary>
		private Node Host_;
		/// <summary>The attribute this originates from.</summary>
		private string Attribute_;
		/// <summary>The raw values.</summary>
		List<string> values=new List<string>();
		
		
		public DOMTokenList(Node host,string attrib){
			Host_=host;
			Attribute_=attrib;
			if(host!=null){
				load(host.getAttribute(attrib));
			}
		}
		
		public void load(string text){
			values.Clear();
			if(text == null){
				return;
			}
			text = text.Trim();
			foreach(string str in text.Split(' ')){
				values.Add(str);
			}
		}
		
		public int length{
			get{
				return values.Count;
			}
		}
		
		/// <summary>Removes the given token.</summary>
		public void remove(string token){
			values.Remove(token);
			Flush();
		}
		
		/*
		/// <summary>True if a given token is supported by the original attribute.</summary>
		public bool supports(string token){
			return false;
		}
		*/
		
		/// <summary>Updates the original attribute.</summary>
		private void Flush(){
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			for(int i=0;i<values.Count;i++){
				if(i!=0){
					sb.Append(' ');
				}
				sb.Append(values[i]);
			}
			
			Host_.setAttribute(Attribute_, sb.ToString());
		}
		
		/// <summary>Adds the given token if it's not in there yet; removes it if it is.</summary>
		public void toggle(string token){
			int index=values.IndexOf(token);
			if(index==-1){
				values.Add(token);
			}else{
				values.RemoveAt(index);
			}
			Flush();
		}
		
		/// <summary>Replaces old with the given token.</summary>
		public void replace(string old,string with){
			int index=values.IndexOf(old);
			
			if(index==-1){
				return;
			}
			
			values[index]=with;
			Flush();
		}
		
		/// <summary>Insert at the given index.</summary>
		public void insert(int index,string token){
			values.Insert(index,token);
			Flush();
		}
		
		/// <summary>Adds a token to the set.</summary>
		public void add(string token){
			values.Add(token);
			Flush();
		}
		
		/// <summary>True if the given token is in the set.</summary>
		public bool contains(string token){
			return values.Contains(token);
		}
		
		/// <summary>Gets a token at a particular index.</summary>
		public string item(int index){
			
			if(index>=values.Count || index<0){
				return null;
			}
			
			return values[index];
			
		}
		
		/// <summary>Gets an element at the specified index.</summary>
		public string this[int index]{
			get{
				return values[index];
			}
			set{
				values[index]=value;
				Flush();
			}
		}
		
		public IEnumerator<string> GetEnumerator(){
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
}