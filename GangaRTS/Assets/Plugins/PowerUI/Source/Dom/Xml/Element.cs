//--------------------------------------
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;

namespace Dom{
	
	/// <summary>
	/// This object represents any Markup Language (ML) tag such as html, sml, xml etc.
	/// </summary>
	
	public partial class Element : Node{
		
		/// <summary>The raw tag as a string. e.g. "div","span" etc in html.</summary>
		public string Tag;
		/// <summary>True if this tag closes itself and doesn't need an end ("/div" for example) tag.</summary>
		internal bool SelfClosing;
		/// <summary>This is true if the childNodes are being rebuilt. True for a tiny amount of time, but prevents collisions with the renderer thread.</summary>
		public bool IsRebuildingChildren;
		
		
		/// <summary>Called when this elements children are fully loaded.
		public virtual void OnChildrenLoaded(){}
		
		/// <summary>Called when the tag is instanced and the element plus its attributes and kids have been fully parsed.</summary>
		public virtual void OnTagLoaded(){}
		
		/// <summary>Gets a parent element by tag name.</summary>
		public Element GetParentByTagName(string tag){
			
			Element e=parentElement;
			
			while(e!=null){
				
				if(e.Tag==tag){
					return e;
				}
				
				// Go up:
				e=e.parentElement;
			}
			
			// Nope!
			return null;
			
		}
		
		/// <summary>True if this element is 'self closing' - i.e. the end tag can be omitted.</summary>
		public virtual bool IsSelfClosing{
			get{
				return false;
			}
		}
		
		/// <summary>True if this property is non-standard.</summary>
		public virtual bool NonStandard{
			get{
				return false;
			}
		}
		
		/// <summary>The minimum distance this element must be dragged in order to trigger a drag start event.</summary>
		/// <returns>If this returns 0, it means use the default value (Input.MinimumDragStartDistance).</returns>
		public virtual float DragStartDistance{
			get{
				// Use the default:
				return 0f;
			}
		}
		
		/// <summary>Called repeatedly when this element is being dragged.</summary>
		public virtual bool OnDrag(PowerUI.DragEvent e){
			return true;
		}
		
		/// <summary>When the given lexer resets, this is called.</summary>
		public virtual int SetLexerMode(bool last,HtmlLexer lexer){
			
			if(last){
				return HtmlTreeMode.InBody;
			}
			
			// Unchanged:
			return HtmlTreeMode.Current;
			
		}
		
		/// <summary>Clones this node.</summary>
		public override Node cloneNode(bool deep){
			
			Node nd=base.cloneNode(deep);
			
			Element ele=(nd as Element);
			
			ele.Tag=Tag;
			
			return ele;
			
		}
		
		/// <summary>Called when a close tag of this element has been created and is being added to the given lexer.
		/// Note that this method must not use 'this'. It is most often called as if it was a static method.
		/// It's not a static method so it can use inheritence and, as a result, be easily used on the element objects
		/// on the current open element stack.</summary>
		/// <returns>True if this element handled itself.</returns>
		public virtual bool OnLexerCloseNode(HtmlLexer lexer,int mode){
			return false;
		}
		
		/// <summary>True if this element is ok to be open when /body shows up. html is one example.</summary>
		public virtual bool OkToBeOpenAfterBody{
			get{
				return false;
			}
		}
		
		/// <summary>True if this element has special parsing rules. http://w3c.github.io/html/syntax.html#special</summary>
		public virtual bool IsSpecial{
			get{
				return false;
			}
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public virtual bool IsParserScope{
			get{
				return false;
			}
		}
		
		/// <summary>True if this element is a table row context.</summary>
		public virtual bool IsTableRowContext{
			get{
				return false;
			}
		}
		
		/// <summary>True if this element is a table body context.</summary>
		public virtual bool IsTableBodyContext{
			get{
				return false;
			}
		}
		
		/// <summary>True if this element is a table context.</summary>
		public virtual bool IsTableContext{
			get{
				return false;
			}
		}
		
		/// <summary>True if this element is part of table structure, except for td.</summary>
		public virtual bool IsTableStructure{
			get{
				return false;
			}
		}
		
		/// <summary>The mode used for an implicit end (see http://w3c.github.io/html/syntax.html#generate-implied-end-tags).</summary>
		public virtual ImplicitEndMode ImplicitEndAllowed{
			get{
				return ImplicitEndMode.None;
			}
		}
		
		public override string textContent{
			get{
				string result="";
				
				if(childNodes_!=null){
					
					for(int i=0;i<childNodes_.length;i++){
						
						Node child=childNodes_[i];
						
						// Ignore comments and doctype:
						if(child is Comment || child is DocumentType){
							continue;
						}
						
						result+=child.textContent;
						
					}
					
				}
				
				return result;
			}
			set{
				
				TextNode node;
				
				// If we've got a single textNode, write straight to it.
				if(childCount==1 && firstChild is TextNode){
					
					// Check if the new value is blank:
					if(string.IsNullOrEmpty(value)){
						// No kids at all.
						empty();
					}else{
						// Write to that one and only text node.
						node=firstChild as TextNode;
						
						// Update data:
						node.data=value;
					}
					
					return;
				}
				
				// Remove all kids (results in a null childNodes set):
				empty();
				
				// Set a single child (a TextNode) next.
				
				if(string.IsNullOrEmpty(value)){
					// No kids at all.
					return;
				}
				
				// Create the node:
				node=Namespace.CreateTextNode(document);
				
				// Set the value:
				node.data=value;
				
				// Set a single child (a TextNode):
				appendChild(node);
				
			}
		}
		
		/// <summary>Inserts a node before this element.</summary>
		public void before(Node node){
			
			Element parent=parentElement;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Insert:
			parent.insertChild(childIndex,node);
			
		}
		
		/// <summary>Inserts an element after this element.</summary>
		public void after(Node node){
			
			Element parent=parentElement;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Insert:
			parent.insertChild(childIndex+1,node);
			
		}
		
		/// <summary>Inserts html before this element.</summary>
		public void before(string html){
			
			Element parent=parentElement;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Insert:
			parent.insertInnerHTML(childIndex,html);
			
		}
		
		/// <summary>Inserts html after this element.</summary>
		public void after(string html){
			
			Element parent=parentElement;
			
			if(parent==null){
				throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
			}
			
			// Insert:
			parent.insertInnerHTML(childIndex+1,html);
			
		}
		
		/// <summary>The html of this element including the element itself.</summary>
		public string outerHTML{
			get{
				System.Text.StringBuilder result=new System.Text.StringBuilder();
				ToString(result);
				return result.ToString();
			}
			set{
				
				Element parent=parentElement;
				
				if(parent==null){
					throw new DOMException(DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				// Get child index:
				int index=childIndex;
				
				// Remove this element:
				parent.removeChild(this);
				
				// Insert at index:
				parent.insertInnerHTML(index,value);
				
			}
		}
		
		/// <summary>Gets or sets the innerHTML of this element.</summary>
		public virtual string innerHTML{
			get{
				System.Text.StringBuilder result=new System.Text.StringBuilder();
				
				if(childNodes_!=null){
					
					for(int i=0;i<childNodes_.length;i++){
						childNodes_[i].ToString(result);
					}
					
				}
				
				return result.ToString();
			}
			set{
				
				IsRebuildingChildren=true;
				
				if(childNodes_!=null){
					empty();
				}
				
				if(!string.IsNullOrEmpty(value)){
					appendInnerHTML(value);
				}
				
				IsRebuildingChildren=false;
				
			}
		}
		
		public override string ToString(){
			return "[object "+GetType().Name+"]";
		}
		
		/// <summary>Inserts HTML at the given position name.</summary>
		public void insertAdjacentHTML(string positionName,string html){
			
			positionName=positionName.ToLower().Trim();
			
			switch(positionName){
				
				case "beforebegin":
					// Before this element
					before(html);
				break;
				case "afterbegin":
					// New first child
					prependInnerHTML(html);
				break;
				case "beforeend":
					// New last child
					appendInnerHTML(html);
				break;
				case "afterend":
					// Immediately after
					after(html);
				break;
			}
			
		}
		
		/// <summary>Appends the given literal text to the content of this element.
		/// This is good for preventing html injection as the text will be taken literally.</summary>
		/// <param name="text">The literal text to append.</param>
		public void appendTextContent(string text){
			
			if(string.IsNullOrEmpty(text)){
				return;
			}
			
			// Parse now:
			HtmlLexer lexer=new HtmlLexer(text,this);
			
			// Plaintext state:
			lexer.State=HtmlParseMode.Plaintext;
			
			// Ok!
			lexer.Parse();
			
		}
		
		/// <summary>Appends the given html text to the content of this element.</summary>
		/// <param name="text">The html text to append.</param>
		public void append(string html){
			if(string.IsNullOrEmpty(html)){
				return;
			}
			
			// Parse now:
			HtmlLexer lexer=new HtmlLexer(html,this);
			
			// PCData is fine here.
			
			// Ok!
			lexer.Parse();
			
		}
		
		/// <summary>Appends the given child element to the content of this element.</summary>
		/// <param name="child">The child to append.</param>
		public void append(Element child){
			if(child==null){
				return;
			}
			appendChild(child);
		}
		
		/// <summary>Appends the given html text to the content of this element.</summary>
		/// <param name="text">The html text to append.</param>
		public void appendInnerHTML(string text){
			append(text);
		}
		
		/// <summary>Inserts HTML into this element at the given index. Pushes any elements at the given index over.</summary>
		public void insertInnerHTML(int index,string text){
			if(string.IsNullOrEmpty(text)){
				return;
			}
			
			// Cache child nodes:
			NodeList children=childNodes_;
			
			int childCount=children==null ? 0 : children.length;
			
			if(index>=childCount){
				// Append:
				appendInnerHTML(text);
				return;
			}
			
			// Create new nodes set:
			childNodes_=new NodeList();
			
			// Transfer up to but not including index:
			if(children!=null){
				
				for(int i=0;i<index;i++){
					
					childNodes_.push(children[i]);
					
				}
				
			}
			
			// Add to end:
			appendInnerHTML(text);
			
			if(children==null){
				return;
			}
			
			// Append the remaining nodes:
			for(int i=index;i<childCount;i++){
				childNodes_.push(children[i]);
			}
			
		}
		
		/// <summary>Prepends the given child element to the content of this element, adding it as the first child.</summary>
		/// <param name="child">The child to prepend.</param>
		public void prepend(Element child){
			// Insert at #0:
			insertChild(0,child);
		}
		
		/// <summary>Prepends the given html text to the content of this element, adding it as the first child.</summary>
		/// <param name="text">The html text to prepend.</param>
		public void prepend(string text){
			// Insert at #0:
			insertInnerHTML(0,text);
		}
		
		/// <summary>Prepends the given html text to the content of this element, adding it as the first child.</summary>
		/// <param name="text">The html text to prepend.</param>
		public void prependInnerHTML(string text){
			// Insert at #0:
			insertInnerHTML(0,text);
		}
		
		/// <summary>True if this tag is for internal use only.</summary>
		public virtual bool Internal{
			get{
				return false;
			}
		}
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public override bool isEqualNode(Node other){
			if(other==this){
				return true;
			}
			
			Element el=other as Element;
			
			return el!=null && el.ToString()==ToString();
		}
		
		/// <summary>A string built from the properties only.</summary>
		public string PropertyString{
			get{
				System.Text.StringBuilder builder=new System.Text.StringBuilder();
				GetPropertyString(builder);
				return builder.ToString();
			}
		}
		
		/// <summary>A string built from the properties only.</summary>
		public void GetPropertyString(System.Text.StringBuilder result){
			
			if(Properties.Count==0){
				return;
			}
			
			foreach(KeyValuePair<string,string> property in Properties){
				
				if(result.Length!=0){
					
					result.Append(" ");
					
				}
				
				result.Append(property.Key);
				
				string value=property.Value;
				
				if(value!=""){
					
					result.Append("=\"");
					result.Append(value);
					result.Append("\"");
					
				}
				
			}
			
		}
		
		/// <summary>Gets a string representation of this element.</summary>
		public override void ToString(System.Text.StringBuilder result){
			
			result.Append("<");
			result.Append(Tag);
			
			GetPropertyString(result);
			
			// Is it supposed to be self closing?
			bool selfClosing=IsSelfClosing;
			
			if(selfClosing){
				result.Append("/");
			}
			
			result.Append(">");
			
			if(!selfClosing){
				
				// Append child nodes:
				base.ToString(result);
				
				// Close tag too:
				result.Append("</");
				result.Append(Tag);
				result.Append(">");
				
			}
			
		}
		
		/// <summary>Checks if the given element is a child of this element.</summary>
		/// <param name="childElement">The element to check if it's a child of this or not.</param>
		/// <returns>True if the given element is actually a child of this.</returns>
		public bool isChild(Node childElement){
			if(childNodes==null){
				return false;
			}
			
			for(int i=0;i<childNodes.length;i++){
				if(childNodes[i]==childElement){
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>The name for this type of node.</summary>
		public override string nodeName{
			get{
				if(Tag==null){
					return null;
				}
				
				return Tag.ToUpper();
			}
		}
		
		/// <summary>The lowercase tag name.</summary>
		public string tagName{
			get{
				return Tag;
			}
		}
		
		/// <summary>The local name of this node.</summary>
		public override string localName{
			get{
				return tagName;
			}
		}
		
		/// <summary>The type of element that this is.</summary>
		public override ushort nodeType{
			get{
				return 1;
			}
		}
		
	}
	
}