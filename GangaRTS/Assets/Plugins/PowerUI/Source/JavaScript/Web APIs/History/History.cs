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
using System.Collections;
using System.Collections.Generic;


namespace PowerUI{
	
	/// <summary>
	/// The window.history API.
	/// </summary>
	public class History{
		
		private Window window;
		/// <summary>The current history state.</summary>
		private HistoryState current;
		/// <summary>Pages before the current one.</summary>
		private List<HistoryState> back_;
		/// <summary>Pages after the current one.</summary>
		private List<HistoryState> forward_;
		
		
		public History(Window window){
			this.window=window;
		}
		
		internal void DocumentNavigated(){
			
			if(current!=null){
				Push(true,current);
			}
			
			// Create current:
			current=CreateState(null,null,null);
			
		}
		
		internal void Ready(){
			
			// This is where the first popstate event comes from:
			Dispatch();
			
		}
		
		private HistoryState CreateState(object state,string title,string url){
			
			if(window.document!=null){
				if(url==null){
					url=window.document.baseURI;
				}
				
				if(title==null){
					title=window.document.title;
				}
			}
			
			HistoryState hs=new HistoryState();
			hs.state=state;
			hs.title=title;
			hs.url=url;
			
			return hs;
			
		}
		
		/// <summary>The current history state object.</summary>
		public object state{
			get{
				if(current==null){
					return null;
				}
				
				return current.state;
			}
		}
		
		/// <summary>Loads the given state now.</summary>
		private void LoadNow(HistoryState state){
			
			if(state==current){
				
				// Reload:
				window.document.location.reload();
				
			}else{
				
				// Set as current:
				current=state;
				Dispatch();
				
				// Go there now (handles relative urls for us):
				window.document.location.assign(state.url,false);
				
			}
			
		}
		
		private void Dispatch(){
			
			// Pop state event:
			PopStateEvent pse=new PopStateEvent();
			pse.state=state;
			pse.SetTrusted();
			window.dispatchEvent(pse);
			
		}
		
		private void Push(bool back,HistoryState state){
			
			List<HistoryState> stack=back?back_ : forward_;
			
			if(stack==null){
				stack=new List<HistoryState>();
				
				if(back){
					back_=stack;
				}else{
					forward_=stack;
				}
				
			}
			
			stack.Add(state);
			
		}
		
		public bool canGoBack{
			get{
				return (back_!=null);
			}
		}
		
		public bool canGoForward{
			get{
				return (forward_!=null);
			}
		}
		
		public void back(){
			go(-1);
		}
		
		public void forward(){
			go(1);
		}
		
		public void pushState(object state,string title){
			pushState(state,title,window.document.baseURI);
		}
		
		public void pushState(object state,string title,string url){
			
			HistoryState hs=CreateState(state,title,url);
			
			// Add to back:
			back_.Add(current);
			current=hs; // Don't dispatch.
			
		}
		
		public void replaceState(object state,string title){
			replaceState(state,title,window.document.baseURI);
		}
		
		public void replaceState(object state,string title,string url){
			
			// Don't dispatch.
			current=CreateState(state,title,url);
			
		}
		
		public void go(int delta){
			
			if(delta==0){
				LoadNow(current);
				return;
			}
			
			HistoryState state=null;
			bool back=(delta<0);
			
			List<HistoryState> set;
			
			if(back){
				// Going back
				delta=-delta;
				set=back_;
			}else{
				set=forward_;
			}
			
			// Range check - got enough scope for delta?
			if(set==null || set.Count<delta){
				// Nope - silently do nothing.
				return;
			}
			
			// Pop and push (always in range):
			for(int i=0;i<delta;i++){
				
				// Pop:
				state=set[set.Count-1];
				set.RemoveAt(set.Count-1);
				
				// Push current:
				Push(!back,current);
				
				// Update current (don't dispatch just yet):
				current=state;
				
			}
			
			if(set.Count==0){
				
				// Clear it:
				if(back){
					back_=null;
				}else{
					forward_=null;
				}
				
			}
			
			// Dispatch:
			LoadNow(state);
			
		}
		
		public int length{
			get{
				int count=1; // Always at least 1 for current.
				
				if(back_!=null){
					count+=back_.Count;
				}
				
				if(forward_!=null){
					count+=forward_.Count;
				}
				
				return count;
				
			}
		}
		
	}
	
	public class HistoryState{
		
		public string url;
		public string title;
		public object state;
		
	}
	
	public partial class Window{
		
		/// <summary>Instance of the history API for this window. Note that this is never null.</summary>
		private History history_;
		
		/// <summary>The window.history API. Read only.</summary>
		public History history{
			get{
				return history_;
			}
		}
		
	}
	
}