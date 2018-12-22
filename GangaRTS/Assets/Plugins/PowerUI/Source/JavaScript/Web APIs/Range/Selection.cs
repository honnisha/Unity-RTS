//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Dom;
using Css;


namespace PowerUI{
	
	/// <summary>
	/// The rather unusual text selection API.
	/// </summary>
	
	public partial class Selection{
		
		/// <suummary>All ranges (oddly, it's required to be exactly 1, but addRange exists)</summary>
		public List<Range> ranges=new List<Range>();
		
		/// <summary>Returns the Node in which the selection ends.</summary>
		public Node focusNode{
			get{
				if(ranges.Count==0){
					return null;
				}
				
				return ranges[0].endContainer;
			}
		}
		
		/// <summary>Returns the number of ranges in the selection</summary>
		public int rangeCount{
			get{
				return ranges.Count;
			}
		}
		
		/// <summary>Returns the Node in which the selection starts.</summary>
		public Node anchorNode{
			get{
				if(ranges.Count==0){
					return null;
				}
				
				return ranges[0].startContainer;
			}
		}
		
		/// <summary>Returns a number representing the offset of the selection's anchor within the anchorNode.
		/// If anchorNode is a text node, this is the number of characters within anchorNode preceding the anchor.
		/// If anchorNode is an element, this is the number of child nodes of the anchorNode preceding the anchor.</summary>
		public int anchorOffset{
			get{
				if(ranges.Count==0){
					return 0;
				}
				
				return ranges[0].startOffset;
			}
		}
		
		/// <summary>Returns a number representing the offset of the selection's anchor within the focusNode.
		/// If focusNode is a text node, this is the number of characters within focusNode preceding the focus.
		/// If focusNode is an element, this is the number of child nodes of the focusNode preceding the focus.</summary>
		public int focusOffset{
			get{
				if(ranges.Count==0){
					return 0;
				}
				
				return ranges[0].endOffset;
			}
		}
		
		/// <summary>Returns a Boolean indicating whether the selection's start and end points are at the same position.</summary>
		public bool isCollapsed{
			get{
				if(ranges.Count==0){
					return true;
				}
				
				return ranges[0].isCollapsed;
			}
		}
		
		/// <summary>Removes a range from the selection.</summary>
		public void removeRange(Range r){
			
			if(ranges.Remove(r)){
				UpdateSelection(false,r);
			}
			
		}
		
		/// <summary>Adds a range which selects all the kids of the given node.</summary>
		public void selectAllChildren(Node node){
			
			// Create range and add it:
			Range r=new Range();
			r.endContainer=node;
			r.startContainer=node;
			r.endOffset=node.childCount;
			
			addRange(r);
			
		}
		
		/// <summary>Selects/ Deselects a whole range.</summary>
		internal void UpdateSelection(bool select,Range r){
			
			if(r.startContainer==null){
				return;
			}
			
			// Get as text node:
			RenderableTextNode htn=(r.startContainer as RenderableTextNode);
			
			if(htn==null){
				// Can't select this at the moment.
				Dom.Log.Add("Note: Attempted to select something that isn't text. If you want this to work, let us know!");
				return;
			}
			
			// Text selection from r.startOffset -> r.endOffset
			
			// Get the selection renderer:
			SelectionRenderingProperty srp=htn.RenderData.GetProperty(
				typeof(SelectionRenderingProperty)
			) as SelectionRenderingProperty;
			
			if(select){
				
				// Create if it doesn't exist:
				if(srp==null){
					
					// Create:
					srp=new SelectionRenderingProperty(htn.RenderData);
					srp.Text=htn.RenderData.Text;
					
					// Add it:
					htn.RenderData.AddOrReplaceProperty(srp,typeof(SelectionRenderingProperty));
					
				}
				
				// Update range:
				if(r.endOffset<r.startOffset){
					
					// Dragged upwards:
					srp.StartIndex=r.endOffset;
					srp.EndIndex=r.startOffset;
					
				}else{
					
					srp.StartIndex=r.startOffset;
					srp.EndIndex=r.endOffset;
					
				}
				
				// Layout:
				srp.RequestLayout();
				
			}else if(srp!=null){
				
				// Remove:
				htn.RenderData.AddOrReplaceProperty(null,typeof(SelectionRenderingProperty));
				
				// Layout:
				srp.RequestLayout();
				
			}
			
		}
		
		/// <summary>Removes all ranges from the selection.</summary>
		public void removeAllRanges(){
			
			for(int i=0;i<ranges.Count;i++){
				UpdateSelection(false,ranges[i]);
			}
			
			ranges.Clear();
		}
		
		/// <summary>Adds a range to the selection.</summary>
		public void addRange(Range r){
			ranges.Add(r);
			UpdateSelection(true,r);
		}
		
		/// <summary>A range object representing one of the ranges currently selected.</summary>
		public Range getRangeAt(int index){
			
			if(index<0 || index>=ranges.Count){
				return null;
			}
			
			return ranges[index];
		}
		
		/// <summary>The currently selected text.</summary>
		public override string ToString(){
			
			if(ranges.Count==0){
				return "";
			}
			
			return ranges[0].ToString();
		}
		
		/// <summary>Collapses the selection to the start of the range.</summary>
		public void collapseToStart(){
			collapse(true);
		}
		
		/// <summary>Collapses the selection to the end of the range.</summary>
		public void collapseToEnd(){
			collapse(false);
		}
		
		/// <summary>Collapses the selection to the start or end of the range.</summary>
		public void collapse(bool toStart){
			
			for(int i=0;i<ranges.Count;i++){
				Range r=ranges[i];
				UpdateSelection(false,r);
				r.collapse(toStart);
				UpdateSelection(true,r);
			}
			
		}
		
		/// <summary>Deletes the selection's content from the document.</summary>
		public void deleteFromDocument(){
			
			for(int i=0;i<ranges.Count;i++){
				Range r=ranges[i];
				UpdateSelection(false,r);
				r.deleteContents();
			}
			
			ranges.Clear();
			
		}
		
		/// <summary>Indicates if a certain node is part of the selection.</summary>
		public bool containsNode(Node n){
			
			for(int i=0;i<ranges.Count;i++){
				
				if( ranges[i].contains(n) ){
					return true;
				}
				
			}
			
			return false;
			
		}
		
	}
	
	public partial class Window{
		
		/// <summary>The current selection. Use the standard getSelection() method instead.</summary>
		internal Selection currentSelection;
		
		/// <summary>Creates a range.</summary>
		public Selection getSelection(){
			
			if(currentSelection==null){
				currentSelection=new Selection();
			}
			
			return currentSelection;
		}
		
	}
	
	public partial class HtmlDocument{
		
		/// <summary>Clears any selection.</summary>
		internal void clearSelection(){
			
			Selection s=window.currentSelection;
			
			if(s!=null && s.ranges.Count>0){
				s.removeAllRanges();
			}
			
		}
		
		/// <summary>The current selection.</summary>
		public Selection getSelection(){
			return window.getSelection();
		}
		
	}
	
}