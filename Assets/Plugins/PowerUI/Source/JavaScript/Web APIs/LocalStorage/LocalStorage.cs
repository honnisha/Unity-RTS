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
using Dom;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// The LocalStorage Web API.
	/// </summary>
	public class LocalStorage : Storage{
		
		/// <summary>True if it's been loaded.</summary>
		private bool loaded;
		
		
		public LocalStorage(HtmlDocument d):base(d){
			
		}
		
		private void Load(){
			loaded=true;
			
			string keySet=PlayerPrefs.GetString(origin+"-internal-keyset");
			
			if(keySet!=null){
				
				// Load it up:
				loadKeySet(keySet);
				
			}
			
		}
		
		/// <summary>Gets the item with the given key.</summary>
		public override string getItem(string key){
			
			if(!loaded){
				Load();
			}
			
			return PlayerPrefs.GetString(origin+key);
		}
		
		/// <summary>Sets the item with the given key.</summary>
		public override void setItem(string key,string data){
			
			if(!loaded){
				Load();
			}
			
			if(!hasItem(key)){
				
				// Add key:
				AddKey(key);
				
				// Write keys out:
				PlayerPrefs.SetString(origin+"-internal-keyset",saveKeySet());
				
			}
			
			PlayerPrefs.SetString(origin+key,data);
			
			// Save both the key change and the string:
			PlayerPrefs.Save();
			
		}
		
		/// <summary>Removes the item with the given key.</summary>
		public override void removeItem(string key){
			
			if(!loaded){
				Load();
			}
			
			// Remove the key:
			RemoveKey(key);
			
			// Remove:
			PlayerPrefs.DeleteKey(origin+key);
			
		}
		
		/// <summary>Removes all the items.</summary>
		public override void clear(){
			
			if(!loaded){
				Load();
			}
			
			if(KeyList==null){
				return;
			}
			
			for(int i=0;i<KeyList.Count;i++){
				
				// Get the key:
				string key=KeyList[i];
				
				// Remove:
				PlayerPrefs.DeleteKey(origin+key);
				
			}
			
			base.clear();
		}
		
	}
	
	/// <summary>
	/// The SessionStorage Web API.
	/// </summary>
	public class SessionStorage : Storage{
		
		/// <summary>The key/value store.</summary>
		private Dictionary<string,string> Store=new Dictionary<string,string>();
		
		
		public SessionStorage(HtmlDocument d):base(d){}
		
		/// <summary>Gets the item with the given key.</summary>
		public override string getItem(string key){
			string r;
			Store.TryGetValue(key,out r);
			return r;
		}
		
		/// <summary>Sets the item with the given key.</summary>
		public override void setItem(string key,string data){
			
			if(!hasItem(key)){
				AddKey(key);
			}
			
			Store[key]=data;
			
		}
		
		/// <summary>Removes the item with the given key.</summary>
		public override void removeItem(string key){
			
			// Remove the key:
			RemoveKey(key);
			
			// Remove:
			Store.Remove(key);
			
		}
		
		/// <summary>Removes all the items.</summary>
		public override void clear(){
			base.clear();
			Store.Clear();
		}
		
	}
	
	/// <summary>
	/// The storage web API. Used by Local/SessionStorage.
	/// </summary>
	public class Storage{
		
		/// <summary>The available keys for this domain.</summary>
		public List<string> KeyList;
		/// <summary>The available keys for this domain.</summary>
		public Dictionary<string,bool> KeyLookup;
		/// <summary>The storage origin.</summary>
		internal string origin;
		
		
		public Storage(HtmlDocument document){
			origin=document.location.hostname;
			origin="ui."+origin+"-";
		}
		
		/// <summary>The key at index i.</summary>
		public string key(int i){
			
			if(KeyList==null || i<0 || i>=KeyList.Count){
				return null;
			}
			
			return KeyList[i];
			
		}
		
		/// <summary>Builds a key set as a string.</summary>
		public string saveKeySet(){
			
			if(KeyList==null){
				return null;
			}
			
			System.Text.StringBuilder sb=new System.Text.StringBuilder();
			
			for(int i=0;i<KeyList.Count;i++){
				
				// Get the key:
				string key=KeyList[i];
				
				if(i!=0){
					sb.Append('\n');
				}
				
				sb.Append(key);
				
			}
			
			return sb.ToString();
			
		}
		
		/// <summary>Loads a key set.</summary>
		internal void loadKeySet(string keys){
			
			string[] keyList=keys.Split('\n');
			
			KeyList=new List<string>();
			KeyLookup=new Dictionary<string,bool>();
			
			for(int i=0;i<keyList.Length;i++){
				AddKey(keyList[i]);
			}
			
		}
		
		/// <summary>True if the given key is set.</summary>
		internal bool hasItem(string key){
			
			return KeyLookup!=null && KeyLookup.ContainsKey(key);
			
		}
		
		/// <summary>Adds a key to the lookups.</summary>
		internal void AddKey(string key){
			
			if(KeyList==null){
				KeyList=new List<string>();
				KeyLookup=new Dictionary<string,bool>();
			}
			
			// Add the key:
			KeyList.Add(key);
			KeyLookup[key]=true;
			
		}
		
		/// <summary>Removes a key from the lookups.</summary>
		internal void RemoveKey(string key){
			
			if(KeyList==null){
				return;
			}
			
			if(KeyLookup.Remove(key)){
				
				// Remove from the list.
				for(int i=0;i<KeyList.Count;i++){
					if(KeyList[i]==key){
						// Remove at i:
						KeyList.RemoveAt(i);
						break;
					}
				}
				
			}
			
		}
		
		/// <summary>Gets the item with the given key.</summary>
		public virtual string getItem(string key){
			throw new NotImplementedException();
		}
		
		/// <summary>Sets the item with the given key.</summary>
		public virtual void setItem(string key,string data){
			throw new NotImplementedException();
		}
		
		/// <summary>Removes the item with the given key.</summary>
		public virtual void removeItem(string key){
			throw new NotImplementedException();
		}
		
		/// <summary>Removes all the items.</summary>
		public virtual void clear(){
			KeyList=null;
			KeyLookup=null;
		}
		
	}
	
}