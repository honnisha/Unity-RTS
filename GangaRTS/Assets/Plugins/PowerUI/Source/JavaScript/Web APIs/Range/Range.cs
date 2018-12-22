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


namespace PowerUI{
	
	public partial class Range{
		
		/// <suummary>Where in startContainer the range starts.</summary>
		public int startOffset;
		/// <suummary>Where in endContainer the range ends.</summary>
		public int endOffset;
		/// <summary>The node within which the range starts.</summary>
		public Node startContainer;
		/// <summary>The node within which the range ends.</summary>
		public Node endContainer;
		
		
		public Range(){}
		
		public Range(int s,int e,Node start,Node end){
			
			startOffset=s;
			endOffset=e;
			startContainer=start;
			endContainer=end;
			
		}
		
		public Range(HtmlDocument doc){
			startContainer=endContainer=doc;
		}
		
		/// <summary>Returns a Boolean indicating whether the selection's start and end points are at the same position.</summary>
		public bool isCollapsed{
            get {
				return startContainer==endContainer && startOffset==endOffset;
			}
        }
		
		public Node rootNode{
            get { return startContainer.rootNode; }
        }
		
		public void setStart(Node start,int offset){
			startContainer=start;
			startOffset=offset;
		}
		
		public void setEnd(Node end,int offset){
			endContainer=end;
			endOffset=offset;
		}
		
		public void setStartBefore(Node refNode)
		{
			if (refNode == null)
				throw new ArgumentNullException("refNode");

			Node parent = refNode.parentNode;

			if (parent == null)
				throw new Exception("Invalid node type");
			
			startContainer=parent;
			startOffset=refNode.childIndex;
		}

		public void setEndBefore(Node refNode)
		{
			if (refNode == null)
				throw new ArgumentNullException("refNode");

			Node parent = refNode.parentNode;

			if (parent == null)
				throw new Exception("Invalid node type");
			
			endContainer=parent;
			endOffset=refNode.childIndex;
		}

		public void setStartAfter(Node refNode)
		{
			if (refNode == null)
				throw new ArgumentNullException("refNode");

			Node parent = refNode.parentNode;

			if (parent == null)
				throw new Exception("Invalid node type");
			
			startContainer=parent;
			startOffset=refNode.childIndex+1;
		}

		public void setEndAfter(Node refNode)
		{
			if (refNode == null)
				throw new ArgumentNullException("refNode");

			Node parent = refNode.parentNode;

			if (parent == null)
				throw new Exception("Invalid node type");
			
			endContainer=parent;
			endOffset=refNode.childIndex+1;
		}

		public void collapse(bool toStart)
		{
			if (toStart)
			{
				endContainer = startContainer;
				endOffset = startOffset;
			}
			else
			{
				startContainer = endContainer;
				startOffset = endOffset;
			}
		}

		public void select(Node refNode)
		{
			if (refNode == null)
				throw new ArgumentNullException("refNode");

			Node parent = refNode.parentNode;

			if (parent == null)
				throw new Exception("Invalid node type");
			
			int index = refNode.childIndex;
			
			startContainer=parent;
			startOffset=index;
			endContainer=parent;
			endOffset=index+1;
		}
		
		/// <summary>A common ancestor for both start and end.</summary>
		public Node commonAncestorContainer{
			get{
				
				if(startContainer==endContainer){
					return startContainer;
				}
				
				Node node=endContainer;
				
				while(node!=null){
					
					Node nodeB=startContainer;
					
					while(nodeB!=null){
						
						if(node==nodeB){
							return node;
						}
						
						nodeB=nodeB.parentNode;
					}
					
					node=node.parentNode;
				}
				
				// They don't share a root at all.
				return null;
			}
		}
		
		/*
		public DocumentFragment extractContents(){
			
		}
		
		public void insertNode(Node newNode){
			
		}
		
		public void surroundContents(Node newNode){
			
			newNode.appendChild(extractContents());
			insertNode(newNode);
			
		}
		*/
		
		private IEnumerable<Node> containedNodes{
			get{
				throw new NotImplementedException("Can't currently collect nodes from a Range.");
			}
		}
		
		/// <summary>All the nodes within the range.</summary>
		public NodeList childNodes{
			get{
				NodeList list=new NodeList();
				
				foreach(Node node in containedNodes){
					list.push(node);
				}
				
				return list;
			}
		}
		
		/// <summary>Deletes all nodes in the range.</summary>
		public void deleteContents(){
			
			foreach(Node node in containedNodes){
				HtmlElement e=node as HtmlElement ;
				
				if(e==null){
					continue;
				}
				
				// Remove it:
				e.parentNode.removeChild(e);
			}
			
		}
		
		/// <summary>Gets selected text as a string.</summary>
		public override string ToString(){
			
			if(startContainer is TextNode){
				
				// We've got a text node - just a substring:
				TextNode tn=(startContainer as TextNode);
				
				int end=endOffset;
				int start=startOffset;
				
				if(end<start){
					int t=start;
					start=end;
					end=t;
				}
				
				// Get:
				return tn.data.Substring(start,end-start);
				
			}
			
			StringBuilder sb=new StringBuilder();
			
			foreach(Node node in containedNodes){
				// Remove it:
				sb.Append(node.ToString());
			}
			
			return sb.ToString();
			
		}
		
		/// <summary>True if the given node is within this range.</summary>
		public bool contains(Node n){
			
			foreach(Node node in containedNodes){
				
				if(n==node){
					return true;
				}
				
			}
			
			return false;
			
		}
		
        public bool intersects(Node node){
			
            if (node == null)
                throw new ArgumentNullException("node");
			
            if (rootNode == node.rootNode)
            {
                var parent = node.parentNode;
				
                if(parent != null){
					
					HtmlElement startElement=startContainer as HtmlElement ;
					HtmlElement endElement=endContainer as HtmlElement ;
					
					// Get as element:
					HtmlElement node1=node as HtmlElement ;
					
					if(endElement==null && startElement==null){
						// Document - must intersect.
						return true;
					}
					
					// End must be after node1 and start must be before node1.
					
					// Node 1 must be after start:
					if(startElement!=null && (startElement.compareDocumentPosition(node1) & Node.DOCUMENT_POSITION_FOLLOWING)!=Node.DOCUMENT_POSITION_FOLLOWING){
						return false;
					}
					
					// Node 1 must be before end:
					if(endElement!=null && (endElement.compareDocumentPosition(node1) & Node.DOCUMENT_POSITION_PRECEDING)!=Node.DOCUMENT_POSITION_PRECEDING){
						return false;
					}
					
                }

                return true;
            }

            return false;
        }

		public Range cloneRange(){
			
			return new Range(startOffset,endOffset,startContainer,endContainer);
			
		}
		
	}
	
	public partial class HtmlDocument{
		
		/// <summary>Creates a range.</summary>
		public Range createRange(){
			return new Range(this);
		}
		
	}
	
}