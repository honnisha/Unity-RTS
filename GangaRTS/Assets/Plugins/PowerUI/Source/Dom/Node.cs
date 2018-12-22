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
	/// The base for all DOM nodes.
	/// <summary>
	
	[Values.Preserve]
	public partial class Node : EventTarget, IParentNode{
		
		/// <summary>The document that this node belongs to.</summary>
		internal Document document_;
		/// <summary>The parent node.</summary>
		internal Node parentNode_;
		/// <summary>The kids of this node. This can be null.</summary>
		internal NodeList childNodes_;
		/// <summary>The namespace this node belongs to.</summary>
		public MLNamespace Namespace;
		/// <summary>The set of attributes on this tag. An attribute is e.g. style="display:none;".</summary>
		public Dictionary<string,string> Properties=new Dictionary<string,string>();
		
		
		#if !ACCEPTED_DOM_NOTICE && !LEGACY_DOM
			#warning If you're upgrading to this release, see the important upgrade notice about attributes at Window > PowerUI > Upgrading. Hide this warning from that window too.
		#endif
		
		#if LEGACY_DOM && !LEGACY_DOM_UPGRADE
		/// <summary>Gets or sets the named attribute of this tag. An attribute is e.g. style="display:none;".</summary>
		/// <param name="property">The name of the attribute to get/set.</param>
		[Obsolete("Use the standard getAttribute/ setAttribute methods instead.")]
		public string this[string property]{
			get{
				string result=null;
				Properties.TryGetValue(property,out result);
				return result;
			}
			
			set{
				
				// Did it change?
				string before=this[property];
				
				Properties[property]=value;
				
				if(before!=value){
					// Sure did!
					OnAttributeChange(property);
				}
				
			}
		}
		#endif
		
		/// <summary>The document that this target belongs to.</summary>
		internal override Document eventTargetDocument{
			get{
				return document_;
			}
		}
		
		/// <summary>True if this node should clear the background state of the renderer.
		/// The background state essentially declares if this nodes background-color or background-image
		/// represents the viewports background. In HTML, both the body tag and root html tag can
		/// represent the background, so the root HTML node doesn't clear the state to allow body to
		/// set it if needed. So, only the root html element overrides this (and this is better than checking for .Tag=="html").</summary>
		internal virtual bool ClearBackground{
			get{
				return true;
			}
		}
		
		/// <summary>The parent node as used by EventTarget during capture. Can be null.</summary>
		internal override EventTarget eventTargetParentNode{
			get{
				return parentNode_;
			}
		}
		
		/// <summary>Attempts to get a title='' attribute when a mouse enters this element.</summary>
		public override string getTitle(){
			string title;
			Properties.TryGetValue("title",out title);
			return title;
		}
		
		/// <summary>The document that this node belongs to.</summary>
		public Document document{
			get{
				return document_;
			}
		}
		
		/// <summary>The name for this type of node.</summary>
		public virtual string nodeName{
			get{
				return null;
			}
		}
		
		/// <summary>The value of this node.</summary>
		public virtual string nodeValue{
			get{
				return null;
			}
			set{}
		}
		
		/// <summary>The type of element that this is.</summary>
		public virtual ushort nodeType{
			get{
				return 0;
			}
		}
		
		/// <summary>The first child of this element.</summary>
		public Node firstChild{
			get{
				if(childNodes_==null || childNodes_.length==0){
					return null;
				}
				return childNodes_[0];
			}
		}
		
		/// <summary>The last child of this element.</summary>
		public Node lastChild{
			get{
				if(childNodes_==null || childNodes_.length==0){
					return null;
				}
				return childNodes_[childNodes_.length-1];
			}
		}
		
		/// <summary>The sibling before this one under this elements parent. Null if this is the first child.</summary>
		public Node previousSibling{
			get{
				int index=childIndex;
				// No parent or it's the first one.
				if(index<=0){
					return null;
				}
				return parentNode_.childNodes[index-1];
			}
		}
		
		/// <summary>The sibling following this one under this elements parent. Null if this is the last child.</summary>
		public Node nextSibling{
			get{
				int index=childIndex;
				// No parent or it's the last one.
				if(index==-1 || index==parentNode_.childNodes.length-1){
					return null;
				}
				return parentNode_.childNodes[index+1];
			}
		}
		
		/// <summary>The next non-text node sibling.</summary>
		public Element previousElementSibling{
			get{
				Node e = previousSibling;
				
				while(e!=null && e.nodeType!=1){
					e = e.previousSibling;
				}
				
				return e as Element;
			}
		}
		
		/// <summary>The next non-text node sibling.</summary>
		public Element nextElementSibling{
			get{
				Node e = nextSibling;
				
				while(e!=null && e.nodeType!=1){
					e = e.nextSibling;
				}
				
				return e as Element;
			}
		}
		
		// attributes
		
		/// <summary>True if this element is before the other one in the DOM.</summary>
		public bool isBefore(Node other){
			
			// Starting from other, we'll repeatedly go into its parent node
			// and check all the kids before it:
			Node child=other;
			Node currentParent=other.parentElement;
			
			while(currentParent!=null){
				
				NodeList kids=currentParent.childNodes;
				
				if(kids==null){
					// Broken DOM.
					throw new DOMException(DOMException.INVALID_STATE_ERR);
				}
				
				for(int i=0;i<kids.length;i++){
					
					Node e=kids[i];
					
					if(e==child){
						// Time to go to the next parent.
						break;
					}
					
					Element el=e as Element;
					
					// Must check if any of e's kids are 'this' which is simply:
					if(el.contains(this)){
						// Boom!
						return true;
					}
					
				}
				
				// Next!
				currentParent=currentParent.parentElement;
			}
			
			// No matches.
			return false;
		}
		
		/// <summary>The owner document.</summary>
		public virtual Document ownerDocument{
			get{
				return document_;
			}
		}
		
		/// <summary>The local name of this node.</summary>
		public virtual string localName{
			get{
				return null;
			}
		}
		
		/// <summary>The base URI.</summary>
		public virtual string baseURI{
			get{
				return document.baseURI;
			}
		}
		
		/// <summary>The namespace this node is in.</summary>
		public string namespaceURI{
			get{
				if(Namespace==null){
					return null;
				}
				
				return Namespace.Name;
			}
		}
		
		/// <summary>Gets or sets the text content of this element (i.e. the content without any html.).
		/// Setting this is good for preventing any html injection as it will be taken literally.</summary>
		public string innerText{
			get{
				return textContent;
			}
			set{
				textContent=value;
			}
		}
		
		/// <summary>Clears the child node set such that they no longer have a parent.</summary>
		internal void empty(){
			
			if(childNodes_==null){
				return;
			}
			
			int count=childNodes.length;
			
			// For each child node..
			for(int i=0;i<count;i++){
				
				// Get the child:
				Node child=childNodes[i];
				
				// Clear its parent node:
				child.parentNode_=null;
				
				// Tell it that it's no longer in the DOM:
				child.RemovedFromDOM();
				
			}
			
			// Clear:
			childNodes_=null;
			ChangedDOM();
			
		}
		
		/// <summary>True if this element is in any document and is rooted.</summary>
		public bool isRooted{
			get{
				
				if(ownerDocument==null){
					
					// Nope!
					return false;
					
				}
				
				// Grab the parent:
				Node current=parentNode_;
				
				// While the current parent has a parent..
				while(current!=null){
					
					if(current==ownerDocument){
						return true;
					}
					
					// Go to the next one - we know for sure it's not null.
					current=current.parentNode_;
					
				}
				
				// Nope!
				return false;
				
			}
		}
		
		/// <summary>Gets or sets the text content of this element (i.e. the content without any html.).
		/// Setting this is good for preventing any html injection as it will be taken literally.</summary>
		public string outerText{
			get{
				return textContent;
			}
			set{
				textContent=value;
			}
		}
		
		/// <summary>Gets or sets the text content of this element (i.e. the content without any html.).
		/// Setting this is good for preventing any html injection as it will be taken literally.</summary>
		public virtual string textContent{
			get{
				return null;
			}
			set{
			}
		}
		
		/// <summary>Gets the parent html element of this element.</summary>
		public Element parentElement{
			get{
				return parentNode_ as Element;
			}
		}
		
		/// <summary>The prefix (namespace).</summary>
		public string prefix{
			get{
				return Namespace.Prefix;
			}
		}
		
		/// <summary>The root node.</summary>
		public Node rootNode{
			get{
				if(parentNode_==null){
					return this;
				}
				
				return parentNode_.rootNode;
			}
		}
		
		/// <summary>The set of children of this element.</summary>
		public NodeList childNodes{
			get{
				if(childNodes_==null){
					childNodes_=new NodeList();
				}
				
				return childNodes_;
			}
		}
		
		/// <summary>The set of children elements.</summary>
		public HTMLCollection children{
			get{
				HTMLCollection kids=new HTMLCollection();
				
				if(childNodes_!=null){
					
					foreach(Node node in childNodes_){
						// Just push it - HTMLCollection will ignore non-elements:
						kids.push(node);
					}
					
				}
				
				return kids;
			}
		}
		
		/// <summary>Gets the index of this element in it's parents childNodes 
		/// in terms of child elements that have the same name (tag) as this one.</summary>
		public int sameNameIndex{
			get{
				if(parentNode_==null){
					return -1;
				}
				
				NodeList kids=parentNode_.childNodes;
				int result=0;
				string tag=(this as Element).Tag;
				
				for(int i=0;i<kids.length;i++){
					
					Element kid=kids[i] as Element;
					
					if(kid==this){
						return result;
					}
					
					if(kid.Tag==tag){
						result++;
					}
					
				}
				
				return -1;
			}
		}
		
		/// <summary>Gets the index of this node in its parents childNodes. Ignores all textNodes.</summary>
		public int childElementIndex{
			get{
				if(parentNode_==null){
					return -1;
				}
				
				NodeList kids=parentNode_.childNodes;
				int index=0;
				
				for(int i=0;i<kids.length;i++){
					Node kid=kids[i];
					
					if(kid==this){
						return index;
					}
					
					if(kid is Element){
						index++;
					}
				}
				
				return -1;
			}
		}
		
		/// <summary>Gets the index of this node in its parents childNodes.</summary>
		public int childIndex{
			get{
				if(parentNode_==null){
					return -1;
				}
				
				NodeList kids=parentNode_.childNodes;
				
				for(int i=0;i<kids.length;i++){
					if(kids[i]==this){
						return i;
					}
				}
				
				return -1;
			}
		}
		
		/// <summary>Inserts the given element after the given one.</summary>
		public void insertAfter(Node toInsert,Node after){
			
			// Create child nodes:
			if(childNodes_==null){
                childNodes_=new NodeList();
            }
			
			if(after==null){
				// Prepend:
				prependChild(toInsert);
				return;
			}
			
			// Get child index:
			int index=after.childIndex;
			
			if(index==-1){
				return;
			}
			
			index++;
			
			if(toInsert.parentNode_==this){
				
				// Moving the node from wherever its at to index.
				moveChild(toInsert,index);
				
				// DOM has changed:
				ChangedDOM();
				
			}else{
				
				if(toInsert.parentNode!=null){
					toInsert.parentNode.removeChild(toInsert);
				}
				
				// Adding it:
				toInsert.parentNode_=this;
				
				// Insert:
				childNodes_.insert(index,toInsert);
				
				// Added:
				toInsert.AddedToDOM();
				
			}
			
		}
		
		/// <summary>Prepends the given element as a child.</summary>
		public Node prependChild(Node child){
			return insertBefore(child,firstChild);
		}
		
		/// <summary>Inserts the given element before the given one.</summary>
		public Node insertBefore(Node toInsert,Node before){
			
			// Create child nodes:
			if(childNodes_==null){
                childNodes_=new NodeList();
            }
			
			if(before==null){
				// Straight add:
				appendChild(toInsert);
				return toInsert;
			}
			
			// Get child index:
			int index=before.childIndex;
			
			if(index==-1){
				return null;
			}
			
			if(toInsert.parentNode_==this){
				
				// Moving the node from wherever its at to index.
				moveChild(toInsert,index);
				
				// DOM has changed:
				ChangedDOM();
				
			}else{
				
				if(toInsert.parentNode!=null){
					toInsert.parentNode.removeChild(toInsert);
				}
				
				// Adding it:
				toInsert.parentNode_=this;
				
				// Insert:
				childNodes_.insert(index,toInsert);
				
				// Added:
				toInsert.AddedToDOM();
				
			}
			
			return toInsert;
			
		}
		
		/// <summary>Replaces the given child with another.</summary>
		public Node replaceChild(Node with,Node element){
			
			if(childNodes_==null){
				return null;
			}
			
			// Get child index:
			int index=element.childIndex;
			
			if(index==-1){
				return null;
			}
			
			// Update element:
			element.parentNode_=null;
			element.RemovedFromDOM();
			
			with.parentNode_=this;
			childNodes_[index]=with;
			
			// Let with know it's now in the DOM:
			with.AddedToDOM();
			
			return element;
			
		}
		
		/// <summary>Adds the given element to the children of this element.</summary>
		/// <param name="element">The child element to add.</param>
		public Node appendChild(Node element){
			
			if(element.parentNode_!=null){
				element.parentNode_.removeChild(element);
			}
			
			// Update parent etc:
			element.parentNode_=this;
			
			// Append (creates if needed):
			childNodes.push(element);
			
			// Call append method:
			element.AddedToDOM();
			
			return element;
			
		}
		
		/// <summary>Moves the given node to the given new index.</summary>
		public Node moveChild(Node toMove,int index){
			int currentIndex=toMove.childIndex;
			int targetIndex=0;
			int count=childNodes_.length;
			
			if(index<currentIndex){
				// Go through the list backwards
				
				targetIndex=count-1;
				
				for(int i=targetIndex;i>=0;i--){
					
					if(i==currentIndex){
						// Chop this one out (skip it without decreasing targetIndex).
						continue;
					}
					
					if(targetIndex==index){
						// Leave a gap here (decrease targetIndex but revisit i).
						i++;
						targetIndex--;
						continue;
					}
					
					childNodes_[targetIndex]=childNodes_[i];
					
					targetIndex--;
					
				}
				
			}else{
				
				for(int i=0;i<count;i++){
					
					if(i==currentIndex){
						// Chop this one out (skip it without increasing targetIndex).
						continue;
					}
					
					if(targetIndex==index){
						// Leave a gap here (bump up targetIndex but revisit i).
						i--;
						targetIndex++;
						continue;
					}
					
					childNodes_[targetIndex]=childNodes_[i];
					
					targetIndex++;
					
				}
				
			}
			
			childNodes_[index]=toMove;
			return toMove;
			
		}
		
		/// <summary>Removes the child at the given index from this element.</summary>
		/// <param name="element">The child element to remove.</param>
		public Node removeChildAt(int index){
			
			if(childNodes_==null){
				return null;
			}
			
			Node child=childNodes_[index];
			childNodes_.removeAt(index);
			
			child.parentNode_=null;
			child.RemovedFromDOM();
			return child;
		}
		
		/// <summary>Removes the given child from this element.</summary>
		/// <param name="element">The child element to remove.</param>
		public Node removeChild(Node element){
			
			if(element.parentNode_!=this){
				// Wrong parent!
				return null;
			}
			
			if(childNodes_!=null){
				childNodes_.remove(element);
			}
			
			element.parentNode_=null;
			element.RemovedFromDOM();
			return element;
		}
		
		/// <summary>True if this element has any child nodes.</summary>
		public bool hasChildNodes(){
			return (childNodes_!=null && childNodes_.length>0);
		}
		
		/// <summary>True if two args sets are equal.</summary>
		public static bool PropertiesEqual(Dictionary<string,string> a,Dictionary<string,string> b){
			
			if((a==null && b==null) || a==b){
				return true;
			}
			
			if(a.Count!=b.Count){
				return false;
			}
			
			foreach(KeyValuePair<string,string> kvp in a){
				
				string v;
				if(!b.TryGetValue(kvp.Key,out v)){
					return false;
				}
			
				if(v!=kvp.Value){
					return false;
				}
				
			}
			
			return true;
			
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public virtual bool OnLexerAddNode(HtmlLexer lexer,int mode){
			return false;
		} 
		
		/// <summary>Clones this node.</summary>
		public virtual Node cloneNode(bool deep){
			
			// Create the copy:
			Node copy=Activator.CreateInstance(GetType()) as Node;
			copy.document_=document_;
			copy.Namespace=Namespace;
			copy.parentNode_=parentNode_;
			
			// Apply attributes:
			foreach(KeyValuePair<string,string> kvp in Properties){
				
				// kvp.Key = the attribute name (e.g. 'style' or 'id' are common ones)
				// kvp.Value = the attribute value on this element
				
				// Get the value from this element and apply it to the copy:
				copy.setAttribute(kvp.Key, kvp.Value);
				
			}
			
			if(deep && childNodes_!=null){
				
				// Clone the kids too!
				foreach(Node child in childNodes_){
					
					// Copy and append:
					copy.appendChild(child.cloneNode(true));
					
				}
				
			}
			
			// All done:
			return copy;
		}
		
		/// <summary>Normalises this node.</summary>
		public void normalize(){
			
			if(childNodes==null){
				return;
			}
			
			for(int i = 0; i < childNodes.length; i++){
				
				TextNode text = childNodes[i] as TextNode;
				
				if (text != null){
					
					if(text.childNodes==null || text.childNodes.length==0){
						removeChild(text);
						i--;
					}else{
						
						System.Text.StringBuilder sb = new System.Text.StringBuilder();
						TextNode sibling = text;
						int end = i;

						while ((sibling = sibling.nextSibling as TextNode) != null){
							sb.Append(sibling.data);
							end++;
						}
						
						text.appendData(sb.ToString());
						
						for (int j = end; j > i; j--){
							removeChild(childNodes[j]);
						}
						
					}
					
				}else if (childNodes[i].hasChildNodes()){
					childNodes[i].normalize();
				}
				
			}
			
		}
		
		/// <summary>Is the given feature supported?</summary>
		public bool isSupported(string feature,string version){
			
			return document.implementation.hasFeature(feature,version);
			
		}
		
		/// <summary>Does this node have any attributes?</summary>
		public bool hasAttributes(){
			return Properties!=null && Properties.Count>0;
		}
		
		/// <summary>Compare document position (two elements only here).</summary>
		public ushort compareDocumentPosition(Node other){
			
			if(other==this){
				return 0;
			}
			
			// This is node2, other is node1
			if(other==null || ownerDocument != other.ownerDocument){
				
				return Node.DOCUMENT_POSITION_DISCONNECTED; 
				
			}
			
			Element thisEl=this as Element;
			
			// If other is one of my kids..
			if(thisEl!=null && thisEl.contains(other)){
				// Yes:
				return Node.DOCUMENT_POSITION_CONTAINED_BY | Node.DOCUMENT_POSITION_FOLLOWING;
			}
			
			// If I am one of others kids..
			Element otherEl=(other as Element);
			
			if(otherEl!=null && otherEl.contains(this)){
				// Yes:
				return Node.DOCUMENT_POSITION_CONTAINS | Node.DOCUMENT_POSITION_PRECEDING;
			}
			
			// Finally, we must check if other is before me.
			if(otherEl!=null && otherEl.isBefore(this)){
				return Node.DOCUMENT_POSITION_PRECEDING;
			}
			
			if(parentNode==null || other.parentNode==null){
				// Disconnected.
				return Node.DOCUMENT_POSITION_DISCONNECTED; 
			}
			
			// Otherwise, return following only:
			return Node.DOCUMENT_POSITION_FOLLOWING;
			
		}
		
		/// <summary>True if this is a parent of the given node.</summary>
		public bool isParentOf(EventTarget node){
			
			EventTarget current=node.eventTargetParentNode;
			
			while(current!=null){
				
				if(current==this){
					return true;
				}
				
				current=current.eventTargetParentNode;
				
			}
			
			// Nope!
			return false;
			
		}
		
		/// <summary>True if the given node is a descendant of this or not.</summary>
		public bool contains(Node node){
			
			if(node==this){
				// Inclusive
				return true;
			}
			
			if(childNodes_==null){
				return false;
			}
			
			// For each child, do any contain the given node?
			for(int i=0;i<childNodes_.length;i++){
				
				Element el=childNodes_[i] as Element;
				
				if(el==null){
					continue;
				}
				
				if(el.contains(node)){
					return true;
				}
				
			}
			
			// Nope!
			return false;
			
		}
		
		/// <summary>Looks up a namespace URI, returning the prefix.</summary>
		public string lookupNamespaceURI(string nsUri){
			MLNamespace ns=MLNamespaces.Get(nsUri);
			
			if(ns==null){
				return null;
			}
			
			return ns.Prefix;
		}
		
		/// <summary>Gets all elements with the given class name(s), seperated by spaces.
		/// May include this element or any of it's kids.</summary>
		/// <param name="className">The name of the classes to find. E.g. "red box".</param>
		/// <returns>A list of all matches.</returns>
		public HTMLCollection getElementsByClassName(string className){
			HTMLCollection results=new HTMLCollection();
			getElementsByClassName(className.Split(' '),results);
			return results;
		}
		
		/// <summary>Gets the first child element with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The first child with the tag.</returns>
		public Element getElementByTagNameNS(string namespaceURI,string tag){
			
			// Get the namespace:
			MLNamespace ns=MLNamespaces.Get(namespaceURI);
			
			HTMLCollection results=getElementsByTagNameNS(ns,tag,true);
			
			if(results.length>0){
				return results[0] as Element;
			}
			
			return null;
			
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection getElementsByTagNameNS(string namespaceURI,string tag){
			
			// Get the namespace:
			MLNamespace ns=MLNamespaces.Get(namespaceURI);
			
			return getElementsByTagNameNS(ns,tag,false);
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <param name="stopWithOne">True if the search should stop when one is found.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection getElementsByTagNameNS(MLNamespace ns,string tag,bool stopWithOne){
			HTMLCollection results=new HTMLCollection();
			getElementsByTagNameNS(ns,tag,stopWithOne,results);
			return results;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public bool getElementsByTagNameNS(MLNamespace ns,string tag,bool stopWithOne,INodeList results){
			if(childNodes==null || ns==null){
				return false;
			}
			
			for(int i=0;i<childNodes.length;i++){
				
				Element child=childNodes[i] as Element;
				
				if(child==null){
					continue;
				}
				
				if(child.Namespace==ns && (child.Tag==tag || tag=="*")){
					// Yep, this has it.
					results.push(child);
					if(stopWithOne){
						return true;
					}
				}
				
				if(child.getElementsByTagNameNS(ns,tag,stopWithOne,results)){
					// Hit the breaks - stop right here.
					return true;
				}
				
			}
			
			return false;
		}
		
		/// <summary>Gets the first child element with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The first child with the tag.</returns>
		public Element getElementByTagName(string tag){
			
			HTMLCollection results=getElementsByTagName(tag,true);
			
			if(results.length>0){
				return results[0] as Element;
			}
			
			return null;
			
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection getElementsByTagName(string tag){
			return getElementsByTagName(tag,false);
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <param name="stopWithOne">True if the search should stop when one is found.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection getElementsByTagName(string tag,bool stopWithOne){
			HTMLCollection results=new HTMLCollection();
			getElementsByTagName(tag,stopWithOne,results);
			return results;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="tag">The html tag to look for.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public bool getElementsByTagName(string tag,bool stopWithOne,INodeList results){
			if(childNodes==null){
				return false;
			}
			
			for(int i=0;i<childNodes.length;i++){
				
				Element child=childNodes[i] as Element;
				
				if(child==null){
					continue;
				}
				
				if(child.Tag==tag || tag=="*"){
					// Yep, this has it.
					results.push(child);
					if(stopWithOne){
						return true;
					}
				}
				
				if(child.getElementsByTagName(tag,stopWithOne,results)){
					// Hit the breaks - stop right here.
					return true;
				}
				
			}
			
			return false;
		}
		
		/// <summary>Gets the first child element with the given name.</summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>The first child with the name.</returns>
		public Element getElementByName(string name){
			
			HTMLCollection results=getElementsByName(name,true);
			
			if(results.length>0){
				return results[0] as Element;
			}
			
			return null;
			
		}
		
		/// <summary>Gets all child elements with the given name.</summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>The set of all tags with this name.</returns>
		public HTMLCollection getElementsByName(string name){
			return getElementsByName(name,false);
		}
		
		/// <summary>Gets all child elements with the given name.</summary>
		/// <param name="name">The name to look for.</param>
		/// <param name="stopWithOne">True if the search should stop when one is found.</param>
		/// <returns>The set of all tags with this name.</returns>
		public HTMLCollection getElementsByName(string name,bool stopWithOne){
			HTMLCollection results=new HTMLCollection();
			getElementsByName(name,stopWithOne,results);
			return results;
		}
		
		/// <summary>Gets all child elements with the given name.</summary>
		/// <param name="name">The name to look for.</param>
		/// <returns>The set of all tags with this name.</returns>
		public bool getElementsByName(string name,bool stopWithOne,INodeList results){
			if(childNodes==null){
				return false;
			}
			
			for(int i=0;i<childNodes.length;i++){
				
				Element child=childNodes[i] as Element;
				
				if(child==null){
					continue;
				}
				
				if(child.getAttribute("name")==name){
					// Yep, this has it.
					results.push(child);
					if(stopWithOne){
						return true;
					}
				}
				
				if(child.getElementsByName(name,stopWithOne,results)){
					// Hit the breaks - stop right here.
					return true;
				}
				
			}
			
			return false;
		}
		
		
		
		
		/// <summary>Gets a child element by ID.</summary>
		public Element getElementById(string value){
			return getElementByAttribute("id",value);
		}
		
		/// <summary>Gets all elements with the given attribute. May include this element or any of it's kids.</summary>
		/// <param name="property">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>A list of all matches.</returns>
		public NodeList getElementsByAttribute(string property,string value){
			NodeList results=new NodeList();
			getElementsByAttribute(property,value,results);
			return results;
		}
		
		/// <summary>Gets all elements with the given attribute. May include this element or any of it's kids.</summary>
		/// <param name="attribute">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>A list of all matches.</returns>
		public NodeList getElementsWithProperty(string property,string value){
			NodeList results=new NodeList();
			getElementsByAttribute(property,value,results);
			return results;
		}
		
		/// <summary>Gets all elements with the given property. May include this element or any of it's kids.</summary>
		/// <param name="property">The name of the property to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the property should be; null for any value.</param>
		/// <param name="results">The set of elements to add results to.</param>
		public void getElementsByAttribute(string property,string value,INodeList results){
			
			if(value==null){
				// It just needs to exist.
				if(Properties.ContainsKey(property)){
					results.push(this);
				}
			}else if(getAttribute(property)==value){
				results.push(this);
			}
			// Any kids got it?
			if(childNodes==null){
				return;
			}
			
			for(int i=0;i<childNodes.length;i++){
				
				Element child=(childNodes[i] as Element);
				
				if(child==null){
					continue;
				}
				
				child.getElementsByAttribute(property,value,results);
				
			}
			
		}
		
		/// <summary>Gets all elements with the given class name(s).
		/// May include this element or any of it's kids.</summary>
		/// <param name="classes">The name of the classes to find. No duplicates allowed.</param>
		/// <param name="results">The set into which the results are placed.</param>
		public void getElementsByClassName(string[] classes,INodeList results){
			
			// Grab this elements class names:
			string thisClassName=getAttribute("class");
			
			// Can it be split up?
			if(thisClassName!=null && thisClassName.Contains(" ")){
				// Yep - split them up:
				string[] thisClassNames=thisClassName.Split(' ');
				
				// Are we only looking for one? If so, skip a double loop.
				if(classes.Length==1){
					// Grab the one and only we're looking for:
					string classToFind=classes[0];
					
					for(int t=0;t<thisClassNames.Length;t++){
						if(thisClassNames[t]==classToFind){
							results.push(this);
							break;
						}
					}
					
				}else if(classes.Length<=thisClassNames.Length){
					// Otherwise we're looking for more than we actually have.
					
					bool add=true;
					
					// For each one we're looking for..
					for(int i=0;i<classes.Length;i++){
						// Is it in this elements set?
						bool inSet=false;
						
						// For each of this elements class names..
						for(int t=0;t<thisClassNames.Length;t++){
							if(thisClassNames[t]==classes[i]){
								// Yep, it's in there!
								inSet=true;
								break;
							}
						}
						
						if(!inSet){
							add=false;
							break;
						}
					}
					
					if(add){
						// Add it in:
						results.push(this);
					}
				}
				
			}else if(classes.Length==1){
				// Single one - special case here (for speed purposes):
				// This is because this element only has one class value, 
				// thus if we're looking for 2 it can't possibly match.
				if(classes[0]==thisClassName){
					// Add it in:
					results.push(this);
				}
			}
			
			// Any kids got it?
			if(childNodes==null){
				return;
			}
			
			for(int i=0;i<childNodes.length;i++){
				Element el=childNodes[i] as Element;
				
				if(el==null){
					continue;
				}
				
				el.getElementsByClassName(classes,results);
			}
		}
		
		/// <summary>Gets an element with the given attribute. May be this element or any of it's kids.</summary>
		/// <param name="property">The name of the attribute to find. E.g. "id".</param>
		/// <param name="value">Optional. The value that the attribute should be; null for any value.</param>
		/// <returns>The first element found that matches.</returns>
		public Element getElementByAttribute(string property,string value){
			
			if(value==null){
				// It just needs to exist.
				if(Properties.ContainsKey(property)){
					return this as Element;
				}
			}else{
				string compare;
				Properties.TryGetValue(property, out compare);
				if(compare==value){
					return this as Element;
				}
			}
			
			// Any kids got it?
			if(childNodes!=null){
				
				for(int i=0;i<childNodes.length;i++){
					
					Element child=childNodes[i] as Element;
					
					if(child==null){
						continue;
					}
					
					child=child.getElementByAttribute(property,value);
					
					if(child!=null){
						return child;
					}
					
				}
				
			}
			
			return null;
		}
		
		/// <summary>Tests whether two nodes are the same, that is if they reference the same object</summary>
		public bool isSameNode(Node other){
			return other==this;
		}
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public virtual bool isEqualNode(Node other){
			if(other==this){
				return true;
			}
			
			return other.ToString()==ToString();
		}
		
		/// <summary>Tests if this elements default namespace is the same as the given one.</summary>
		public bool isDefaultNamespace(string nsUri){
			return (namespaceURI==nsUri);
		}
		
		/// <summary>Looks up a namespace prefix, returning the namespace URI.</summary>
		public string lookupPrefix(string prefix){
			MLNamespace ns=MLNamespaces.GetPrefix(prefix);
			
			if(ns==null){
				return null;
			}
			
			return ns.Name;
		}
		
		/// <summary>The number of child elements of this element.</summary>
		public int childElementCount{
			get{
				if(childNodes_==null){
					return 0;
				}
				
				int c=0;
				
				for(int i=0;i<childNodes_.length;i++){
					
					if(childNodes_[i].nodeType==1){
						c++;
					}
					
				}
				
				return c;
			}
		}
		
		/// <summary>The number of children of this element.</summary>
		public int childCount{
			get{
				if(childNodes_==null){
					return 0;
				}
				
				return childNodes_.length;
			}
		}
		
		/// <summary>Removes this node from the tree it belongs to.</summary>
		public Node remove(){
			Element parent=parentElement;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Remove from the parent:
			parent.removeChild(this);
			return this;
		}
		
		public virtual void ToString(System.Text.StringBuilder result){
			
			// For each child node, append it:
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				Node e=childNodes_[i];
				e.ToString(result);
			}
			
		}
		
		public override string ToString(){
			return "[object Node]";
		}
		
		/// <summary>The first non-text child of this element.</summary>
		public Element firstElementChild{
			get{
				if(childNodes_==null || childNodes_.length==0){
					return null;
				}
				
				for(int i=0;i<childNodes_.length;i++){
					Node e=childNodes_[i];
					
					if(e.nodeType!=1){
						continue;
					}
					
					return e as Element;
				}
				
				return null;
			}
		}
		
		/// <summary>The last non-text child of this element.</summary>
		public Element lastElementChild{
			get{
				if(childNodes_==null || childNodes_.length==0){
					return null;
				}
				
				for(int i=childNodes_.length-1;i>=0;i--){
					Node e=childNodes_[i];
					
					if(e.nodeType!=1){
						continue;
					}
					
					return e as Element;
				}
				
				return null;
			}
		}
		
		/// <summary>Inserts a child into this element at the given index. Pushes any elements at the given index over.</summary>
		public Node insertChild(int index,Node child){
			
			if(child==null){
				return null;
			}
			
			// Cache child nodes:
			NodeList children=childNodes_;
			
			int childCount=children==null ? 0 : children.length;
			
			if(index>=childCount){
				// Append:
				return appendChild(child);
			}
			
			// Create new nodes set:
			childNodes_=new NodeList();
			
			// Transfer up to but not including index:
			if(children!=null){
				
				for(int i=0;i<index;i++){
					
					childNodes_.push(children[i]);
					
				}
				
			}
			
			// Update the child settings:
			child.parentNode_=this;
			
			// Added:
			child.AddedToDOM();
			
			// Add it now:
			childNodes_.push(child);
			
			if(children==null){
				return child;
			}
			
			// Append the remaining nodes:
			for(int i=index;i<childCount;i++){
				childNodes_.push(children[i]);
			}
			
			return child;
			
		}
		
		/// <summary>Replaces this element with the given element.</summary>
		public Node replaceWith(Node element){
			
			// Replace this element with the given one:
			Element parent=parentElement;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Replace:
			parent.replaceChild(element,this);
			return this;
			
		}
		
		/// <summary>The ID of this element. Won't ever be null.</summary>
		public string id{
			get{
				string value=getAttribute("id");
				
				if(value==null){
					return "";
				}
				
				return value;
			}
			set{
				setAttribute("id", value);
			}
		}
		
		/// <summary>The css class attribute of this element. Won't ever be null.
		/// Note that it can potentially hold multiple names, e.g. "red button". Use classList for those.</summary>
		public string className{
			get{
				string value=getAttribute("class");
				
				if(value==null){
					return "";
				}
				
				return value;
			}
			set{
				setAttribute("class", value);
			}
		}
		
		/// <summary>This nodes parent node.</summary>
		public Node parentNode{
			get{
				return parentNode_;
			}
		}
		
		/// <summary>The set of class names.</summary>
		public DOMTokenList classList{
			get{
				return new DOMTokenList(this,"class");
			}
		}
		
		// getFeature(feature,version)
		
		// setUserData(key,data,handler)
		
		// getUserData(key)
		
		#region Internal methods
		
		/// <summary>Reloads the content of variables if it's name matches the given one.</summary>
		/// <param name="name">The name of the variable to reset.</param>
		internal virtual void ResetVariable(string name){
			
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				childNodes_[i].ResetVariable(name);
			}
		}
		
		/// <summary>Re-resolves all variable tags. This is used when the language is changed.</summary>
		internal virtual void ResetAllVariables(){
			
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				childNodes_[i].ResetAllVariables();
			}
			
		}
		
		/// <summary>Called on an instance of this handler when an attribute on the element it's attached to changes.
		/// It's also called when the tag is being loaded.</summary>
		/// <param name="attribute">The attribute that changed.</param>
		public virtual bool OnAttributeChange(string attribute){
			return false;
		}
		
		/// <summary>Internal. Called when this element has been removed from the DOM.</summary>
		internal virtual void RemovedFromDOM(){
			
			if(childNodes!=null){
				
				for(int i=0;i<childNodes.length;i++){
					childNodes[i].RemovedFromDOM();
				}
				
			}
			
		}
		
		/// <summary>Internal. Called when this element has been added to the DOM.</summary>
		internal virtual void AddedToDOM(){}
		
		/// <summary>Called when this elements child nodes change.
		/// Doesn't occur when AddedToDOM or RemovedFromDOM trigger.</summary>
		internal virtual void ChangedDOM(){}
		
		#endregion
		
		#region Node constants
		
		// Node type
		public const ushort ELEMENT_NODE=1;
		public const ushort ATTRIBUTE_NODE=2;
		public const ushort TEXT_NODE=3;
		public const ushort CDATA_SECTION_NODE=4;
		public const ushort ENTITY_REFERENCE_NODE=5;
		public const ushort ENTITY_NODE=6;
		public const ushort PROCESSING_INSTRUCTION_NODE=7;
		public const ushort COMMENT_NODE=8;
		public const ushort DOCUMENT_NODE=9;
		public const ushort DOCUMENT_TYPE_NODE=10;
		public const ushort DOCUMENT_FRAGMENT_NODE=11;
		public const ushort DOCUMENT_NOTATION_NODE=12;
		
		// Document position
		public const ushort DOCUMENT_POSITION_DISCONNECTED=1;
		public const ushort DOCUMENT_POSITION_PRECEDING=2;
		public const ushort DOCUMENT_POSITION_FOLLOWING=4;
		public const ushort DOCUMENT_POSITION_CONTAINS=8;
		public const ushort DOCUMENT_POSITION_CONTAINED_BY=16;
		public const ushort DOCUMENT_POSITION_IMPLEMENTATION_SPECIFIC=32;
		
		#endregion
		
	}
	
	/// <summary>
	/// The interface for all parent nodes.
	/// <summary>

	public interface IParentNode{
		
		/// <summary>The kids of this node.</summary>
		NodeList childNodes{ get; }
		
		/// <summary>The parent node.</summary>
		Node parentNode{ get; }
		
		/// <summary>The parent element.</summary>
		Element parentElement{ get; }
		
		/// <summary>The number of kids of this node.</summary>
		int childCount{ get; }
		
		/// <summary>The number of child elements of this parent.</summary>
		int childElementCount{ get; }
		
		/// <summary>The kids of this node.</summary>
		HTMLCollection children{ get; }
		
		/// <summary>The first element child of this node.</summary>
		Element firstElementChild{ get; }
		
		/// <summary>The last element child of this node.</summary>
		Element lastElementChild{ get; }
		
	}
	
}