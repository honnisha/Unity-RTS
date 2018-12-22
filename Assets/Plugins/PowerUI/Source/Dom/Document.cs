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


namespace Dom{
	
	/// <summary>
	/// The interface for all XML-style documents.
	/// </summary>
	
	public partial class Document : Node{
		
		private static uint UniqueID=0;
		/// <summary>Turn this off to block scripts.</summary>
		public static bool AllowScripts=true;
		
		
		public Document(){
			// Setup node document property:
			document_=this;
			
			if(UniqueID==uint.MaxValue){
				UniqueID=0;
			}
			
			uniqueID=UniqueID++;
			
		}
		
		public DOMImplementation implementation{
			get{
				return new DOMImplementation(this);
			}
		}
		
		/// <summary>The document type of this document.</summary>
		public DocumentType doctype;
		
		/// <summary>A unique ID for this document.</summary>
		public uint uniqueID;
		/// <summary>The current ready state.</summary>
		internal int readyState_;
		/// <summary>The number of resources still loading. When this reaches 0, onload is triggered.</summary>
		internal int resourcesLoading;
		/// <summary>The current location of this document.
		/// Originates from the src attribute of iframes or is 'resources://' by default.</summary>
		internal Location location_;
		
		
		/// <summary>The current document ready state.</summary>
		public string readyState{
			get{
				switch(readyState_){
					
					default:
					case 0:
						return "loading";
					case 1:
						return "interactive";
					case 2:
						return "complete";
				}
			}
		}
		
		/// <summary>The current location of this document.
		/// Originates from the src attribute of iframes or is 'resources://' by default.</summary>
		public Location location{
			get{
				return location_;
			}
			set{
				SetLocation(value,true);
			}
		}
		
		/// <summary>Called when a resource for this document is loading.</summary>
		internal virtual bool ResourceStatus(EventTarget package,int status){
			
			if(status==1){
				
				// Loading:
				resourcesLoading++;
				
			}else if(status==4){
				
				// Done:
				resourcesLoading--;
				
				if(resourcesLoading<=0 && readyState_!=2){
					
					// Fire onload now!
					ReadyStateChange(2);
					
					// Fire event:
					PowerUI.UIEvent de=new PowerUI.UIEvent("load");
					de.SetTrusted(true);
					dispatchEvent(de);
					return true;
					
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>Sets the current location, optionally adding a history entry or not.
		/// Generally you should use the window.history API or document.location instead.</summary>
		internal virtual void SetLocation(Location value,bool addHistory){
			location_=value;
		}
		
		/// <summary>The path that this document is relative to (if any). Set to null to use document.location.</summary>
		public Location basepath{
			get{
				if(basepath_==null){
					return location;
				}
				
				return basepath_;
			}
			set{
				basepath_=value;
			}
		}
		
		/// <summary>The path that this document is relative to (if any). See basepath and baseURI.</summary>
		internal Location basepath_;
		
		/// <summary>The path that this document is relative to (if any).</summary>
		public override string baseURI{
			get{
				return basepath.absolute;
			}
		}
		
		/// <summary>Loads a variable value from the language set. Note that this occurs after checking 'diverted' variables
		/// and after custom ones too - it only looks for languages. This is essentially handled by the document.languages API.</summary>
		public virtual void LoadLanguageVariable(string groupName,string variableName,LanguageTextEvent onResolved){
			onResolved(null);
		}
		
		/// <summary>A simple loader which uses a HTML5 parser to load the given string.</summary>
		/// <param name='mode'>The initial HtmlTreeMode to use.</param>
		public void LoadHtml(string html,int mode){
			
			if(string.IsNullOrEmpty(html)){
				return;
			}
			
			HtmlLexer lexer=new HtmlLexer(html,this);
			lexer.CurrentMode=mode;
			lexer.Parse();
		}
		
		/// <summary>A simple loader which uses a HTML5 parser to load the given string.</summary>
		public void LoadHtml(string html){
			
			if(string.IsNullOrEmpty(html)){
				return;
			}
			
			HtmlLexer lexer=new HtmlLexer(html,this);
			lexer.Parse();
		}
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public override bool isEqualNode(Node other){
			return other==this;
		}
		
		/// <summary>The root document element. It must also subscribe to IRenderableNode.</summary>
		public virtual Element documentElement{
			get{
				return null;
			}
		}
		
		/// <summary>The name for this type of node.</summary>
		public override string nodeName{
			get{
				return "#document";
			}
		}
		
		/// <summary>An iteratable set of all nodes from this document.
		/// Whilst iterating you can actively skip nodes, so it's often useful to cache this first.</summary>
		public NodeIterator allNodes{
			get{
				return new NodeIterator(this);
			}
		}
		
		/// <summary>Document title.</summary>
		public virtual string title{
			get{
				return "";
			}
			set{}
		}
		
		/// <summary>The global scripting scope.</summary>
		public virtual object GlobalScope{
			get{
				return null;
			}
		}
		
		/// <summary>The type of element that this is.</summary>
		public override ushort nodeType{
			get{
				return 9;
			}
		}
		
		public override string ToString(){
			return "[object Document]";
		}
		
		/// <summary>The documents current target. It's the same as location.hash.</summary>
		public virtual string Target{
			get{
				return null;
			}
		}
		
		/// <summary>The owning document.</summary>
		public override Document ownerDocument{
			get{
				return null;
			}
		}
		
		/// <summary>Gets or sets the inner markup of this document.</summary>
		public virtual string innerML{
			get{
				return "";
			}
			set{}
		}
		
		/// <summary>A window event target if there is one.</summary>
		internal virtual EventTarget windowTarget{
			get{
				return null;
			}
		}
		
		/// <summary>Changes the ready state of this document.</summrry>
		protected void ReadyStateChange(int state){
			
			readyState_=state;
			
			if(state==0){
				return;
			}
			
			// Fire event:
			Dom.Event de=new Dom.Event("readystatechange");
			de.SetTrusted(true);
			dispatchEvent(de);
			
		}
		
		/// <summary>Dispatches the DOMContentLoaded event.</summary>
		protected void ContentLoadedEvent(){
			
			// ReadyState is now 1 (interactive):
			ReadyStateChange(1);
			
			// Dom content loaded:
			Dom.Event de=new Dom.Event("DOMContentLoaded");
			de.SetTrusted(true);
			dispatchEvent(de);
			
		}
		
		/// <summary>Creates a text node.</summary>
		public TextNode createTextNode(){
			return Namespace.CreateTextNode(this);
		}
		
		/// <summary>Creates a comment node.</summary>
		public Comment createComment(){
			return Namespace.CreateCommentNode(this);
		}
		
		/// <summary>Creates a new element in this document with the given namespace. 
		/// You'll need to parent it to something.</summary>
		public Element createElementNS(string namespaceName,string tag){
			
			// Get the namespace by its URL:
			MLNamespace ns=MLNamespaces.Get(namespaceName);
			
			if(ns==null){
				ns=Namespace;
			}
			
			// Create the element and call its startup methods:
			Element result=TagHandlers.Create(ns,tag) as Element;
			result.document_=this;
			result.OnTagLoaded();
			result.OnChildrenLoaded();
			
			return result;
		}
		
		/// <summary>Creates a new element in this document. You'll need to parent it to something.
		/// E.g. with thisDocument.body.appendChild(...). Alternative to innerHTML and appendInnerHTML.</summary>
		/// <param name='tag'>The tag, e.g. <div id='myNewElement' .. ></param>
		public Element createElement(string tag){
			
			// Note that we ignore a namespace declaration here.
			
			// Create the element and call its startup methods:
			Element result=TagHandlers.Create(Namespace,tag) as Element;
			result.document_=this;
			result.OnTagLoaded();
			result.OnChildrenLoaded();
			
			return result;
		}
		
		/// <summary>Creates a new document fragment in this document.</summary>
		public DocumentFragment createDocumentFragment(){
			
			// Create the element and call its startup methods:
			DocumentFragment result=new DocumentFragment();
			result.document_=this;
			return result;
			
		}
		
	}
	
}