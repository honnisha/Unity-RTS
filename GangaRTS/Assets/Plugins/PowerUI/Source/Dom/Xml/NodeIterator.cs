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

using Dom;
using System.Collections;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// Allows easy iteration through all elements in a node.
	/// </summary>
	
	public class NodeIterator:ExpandableObject, IEnumerable<Node>{
		
		/// <summary>A filter if there is one. JS objects get cooerced to one of these.</summary>
		public NodeFilter filter;
		/// <summary>The source containing the elements to enumerate. Returns this too.</summary>
		public Node Source;
		/// <summary>False if the iteration should skip the following children of the current element.</summary>
		public bool Deep=true;
		/// <summary>Set this to true during iteration to avoid going into a particular elements child nodes.</summary>
		public bool SkipChildren;
		/// <summary>The current enumerator for use with nextNode.</summary>
		private IEnumerator<Node> curEnumerator;
		
		/// <summary>The source containing the elements to enumerate. Returns this too.</summary>
		public Node root{
			get{
				return Source;
			}
		}
		
		/// <summary>The next node.</summary>
		public Node nextNode{
			get{
				
				if(curEnumerator==null){
					curEnumerator=GetEnumerator();
					
					// Skip root:
					curEnumerator.MoveNext();
				}
				
				curEnumerator.MoveNext();
				return curEnumerator.Current;
				
			}
		}
		
		/// <summary>Iterates through the entire DOM tree from the given node.</summary>
		public NodeIterator(Node source){
			Source=source;
		}
		
		public NodeIterator(Node source,bool deep){
			Source=source;
			Deep=deep;
		}
		
		/// <summary>Iterates through the given element.</summary>
		/// <param name="element">The element to iterate through.</param>
		public IEnumerable<Node> IterateThrough(Node src){
			
			NodeList children=src.childNodes_;
			
			if(children!=null){
				
				// Grab how many their are:
				int childCount=children.length;
				
				for(int i=0;i<childCount;i++){
					
					// Return the child:
					yield return children[i];
					
					if(SkipChildren){
						// Set it to false and continue:
						SkipChildren=false;
						continue;
					}
					
					// Iterate through each of its kids, returning the result:
					foreach(Node child in IterateThrough(children[i])){
						yield return child;
					}
					
				}
				
			}
			
		}
		
		public IEnumerator<Node> GetEnumerator(){
			
			NodeList children=Source.childNodes_;
			
			if(children!=null){
				
				// Grab how many their are:
				int childCount=children.length;
				
				for(int i=0;i<childCount;i++){
					
					// Return the child:
					yield return children[i];
					
					if(Deep){
						
						// Iterate through each of its kids, returning the result:
						foreach(Node child in IterateThrough(children[i])){
							yield return child;
						}
						
					}
					
				}
			
			}
			
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
	}
	
}