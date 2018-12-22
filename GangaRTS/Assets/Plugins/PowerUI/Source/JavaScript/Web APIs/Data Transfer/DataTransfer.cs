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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace PowerUI{
	
	public class DataTransfer{
		
		public string dropEffect;
		public string effectAllowed;
		/// <summary>The data transfer store.</summary>
		private DataTransferList items_=new DataTransferList();
		
		/// <summary>All items (not live).</summary>
		public DataTransferList items{
			get{
				items_.Reset();
				return items_;
				
			}
		}
		
		/// <summary>Files only (live).</summary>
		public FileList files{
			get{
				if(items_.files_==null){
					items_.files_=new FileList();
				}
				
				return items_.files_;
			}
		}
		
		/// <summary>Meta types. Includes 'Files' if there's any files in here. Read only.</summary>
		public List<string> types{
			get{
				return items_.types;
			}
		}
		
		public void clearData(){
			clearData(null);
		}
		
		public void clearData(string format){
			items.clearText(format);
		}
		
		public void setData(string format,string data){
			items.setText(format,data);
		}
		
		public string getData(string format){
			return items_.getText(format);
		}
		
	}
	
	/// <summary>Used by the copy/paste API.</summary>
	internal class TextTransferItem:DataTransferItem{
		
		public string text;
		public string metaType="text/plain";
		
		public TextTransferItem(string text){
			this.text=text;
		}
		
		public override string type{
			get{
				return metaType;
			}
		}
		
		public override string getAsString(){
			return text;
		}
		
	}
	
	/// <summary>A data transfer item.</summary>
	public class DataTransferItem{
		
		public virtual string kind{
			get{
				return "string";
			}
		}
		
		public virtual string type{
			get{
				return "text/plain";
			}
		}
		
		public virtual string getAsString(){
			return "";
		}
		
		public virtual WebFile getAsFile(){
			return null;
		}
		
	}
	
	/// <summary>A list of items being transferred.</summary>
	public class DataTransferList : IEnumerable<DataTransferItem>{
		
		internal FileList files_;
		internal Dictionary<string,TextTransferItem> text_;
		/// <summary>Combined list of values. Computed on demand.</summary>
		internal List<DataTransferItem> values;
		
		/// <summary>Rebuilds the values array.</summary>
		internal void Reset(){
			
			if(values==null){
				values=new List<DataTransferItem>();
			}else{
				values.Clear();
			}
			
			if(text_!=null){
				
				foreach(KeyValuePair<string,TextTransferItem> kvp in text_){
					values.Add(kvp.Value);
				}
				
			}
			
			if(files_!=null){
				
				foreach(WebFile file in files_){
					values.Add(file);
				}
				
			}
			
		}
		
		public List<string> types{
			get{
				List<string> list=new List<string>();
				
				if(text_!=null){
					foreach(KeyValuePair<string,TextTransferItem> kvp in text_){
						list.Add(kvp.Key);
					}
				}
				
				if(files_!=null && files_.length>0){
					list.Add("Files");
				}
				
				return list;
			}
		}
		
		public void add(WebFile file){
			
			if(files_==null){
				files_=new FileList();
			}
			
			files_.push(file);
		}
		
		public void add(string format,string data){
			setText(format,data);
		}
		
		/// <summary>Adds the given node to the list.</summary>
		public void push(DataTransferItem node){
			values.Add(node);
		}
		
		/// <summary>The number of nodes in the list.</summary>
		public int length{
			get{
				return values.Count;
			}
		}
		
		/// <summary>Gets a node at a particular index.</summary>
		public DataTransferItem item(int index){
			return values[index];
		}
		
		public void clearText(string format){
			
			format=MapFormat(format);
			
			if(format==null){
				text_=null;
			}else if(text_!=null){
				text_.Remove(format);
			}
			
		}
		
		public void setText(string format,string data){
			
			if(format==null){
				format="";
			}
			
			format=MapFormat(format);
			
			TextTransferItem tti=new TextTransferItem(data);
			tti.metaType=format;
			
			if(text_==null){
				text_=new Dictionary<string,TextTransferItem>();
			}
			
			text_[format]=tti;
			
		}
		
		public string getText(string format){
			
			if(text_==null || format==null){
				return "";
			}
			
			format=MapFormat(format);
			
			TextTransferItem dti;
			
			if(!text_.TryGetValue(format,out dti)){
				return "";
			}
			
			if(dti.kind!="string"){
				return "";
			}
			
			return dti.text;
		}
		
		/// <summary>Tidies up the given format, and e.g. converts 'text' to 'text/plain'.</summary>
		private string MapFormat(string format){
			
			format=format.Trim().ToLower();
			
			if(format=="text"){
				format="text/plain";
			}else if(format=="url"){
				format="text/uri-list";
			}
			
			return format;
			
		}
		
		/// <summary>Gets an element at the specified index.</summary>
		public DataTransferItem this[int index]{
			get{
				return values[index];
			}
			internal set{
				values[index]=value;
			}
		}
		
		public IEnumerator<DataTransferItem> GetEnumerator(){
			return values.GetEnumerator();
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
}