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
using System.Reflection;


namespace Dom{

	/// <summary>
	/// A Html5 capable lexer.
	/// </summary>
	
	public class HtmlLexer : StringReader{
		
		/// <summary>Cached reference for the XHTML namespace.</summary>
		private static MLNamespace _XHTMLNamespace;
		
		/// <summary>The XML namespace for XHTML.</summary>
		public static MLNamespace XHTMLNamespace{
			get{
				if(_XHTMLNamespace==null){
					
					// Get the namespace (Doesn't request the URL; see XML namespaces for more info):
					_XHTMLNamespace=Dom.MLNamespaces.Get("http://www.w3.org/1999/xhtml","xhtml","text/html");
					
				}
				
				return _XHTMLNamespace;
			}
		}
		
		/// <summary>
		/// Gets or sets the current parse mode.
		/// </summary>
		public HtmlParseMode State;
		/// <summary>Current namespace. Defaults to XHTML (for all our HTML tags).</summary>
		public MLNamespace Namespace;
		/// <summary>Document we're adding to.</summary>
		public Document Document;
        public readonly List<Element> OpenElements;
        public readonly Stack<int> TemplateModes;
		public readonly List<Element> FormattingElements;
		/// <summary>The current tree mode.</summary>
		public int PreviousMode = HtmlTreeMode.Initial;
		/// <summary>The current tree mode.</summary>
		public int CurrentMode = HtmlTreeMode.Initial;
		/// <summary>The length of the current text buffer.</summary>
		public int TextBlockLength;
		/// <summary>A string builder used for constructing tokens.</summary>
		public System.Text.StringBuilder Builder=new System.Text.StringBuilder();
		/// <summary>The head pointer.</summary>
		public Element head;
		/// <summary>The form pointer.</summary>
		public Element form;
		/// <summary>The pending table chars 'list' (we only ever add one to it).</summary>
		public TextNode PendingTableCharacters;
		/// <summary>The last created start tag name (lowercase).</summary>
		public string LastStartTag;
		/// <summary>Frameset-ok flag</summary>
		public bool FramesetOk=true;
		/// <summary>Table foster parenting. Occurs when tables are mis-nested and affects how elements are added.</summary>
		public bool _foster=false;
		/// <summary>The latest added text node. Gets cleared whenever Process is called.</summary>
		private TextNode text_;
		
		
		public HtmlLexer(string str,Node context):base(str){
			
			// First, check for the UTF8 BOM (rather awkwardly encoded in UTF16 here):
			if(str!=null && str.Length>=1 && (int)str[0]==0xFEFF){
				
				// We're UTF-8. Skip the BOM:
				Position++;
				
			}
			
			// Create sets:
            OpenElements = new List<Element>();
            TemplateModes = new Stack<int>();
			FormattingElements = new List<Element>();
			
			Document=context as Document;
			
			if(Document==null){
				
				// We're in some other context. Note that we'll always append into this context.
				Document=context.document;
				
				// Push it to the open element set, if it's an element:
				Element el=context as Element;
				
				if(el!=null){
					OpenElements.Add(el);
					Reset();
				}
				
			}
			
			// Setup namespace:
			Namespace=Document.Namespace;
			
		}
		
		/// <summary>Checks if all elements on the stack are ok to be open in the AfterBody mode.</summary>
		public void CheckAfterBodyStack(){
			
			for(int i=OpenElements.Count-1;i>=0;i--){
				
				// Can it still be open?
				if(!OpenElements[i].OkToBeOpenAfterBody){
					
					// Fatal error - mangled DOM.
					throw new DOMException(DOMException.SYNTAX_ERR,(ushort)HtmlParseError.BodyClosedWrong);
					
				}
				
			}
			
		}
		
		/// <summary>Resets the insertion mode.
		/// http://www.w3.org/html/wg/drafts/html/master/syntax.html#the-insertion-mode
		/// </summary>
		public void Reset(){
		
			for (int i = OpenElements.Count - 1; i >= 0; i--){
				
				Element element = OpenElements[i];
				bool last = (i == 0);
				
				int mode = element.SetLexerMode(last, this);
				
				if(mode!=HtmlTreeMode.Current){
					CurrentMode=mode;
					break;
				}
				
			}
			
		}
		
		/// <summary>Parses the whole string.</summary>
		public void Parse(){
			
			char peek=Peek();
			
			// While we have more..
			while(peek!=StringReader.NULL){
				
				switch (State){
					case HtmlParseMode.PCData:
						
						if(peek=='<'){
							
							// Tag!
							Read();
							OpenPCTag();
							
						}else{
							
							// Handle text nodes:
							HandleText(true,true);
							
						}
						
					break;
					case HtmlParseMode.RCData:
						
						if(peek=='<'){
							
							// Tag!
							Read();
							OpenRCTag();
							
						}else{
							
							// Handle text nodes:
							HandleText(true,true);
							
						}
						
					break;
					case HtmlParseMode.Rawtext:
					case HtmlParseMode.Script:
						
						// Keep going until we reach </lastTagName>
						// Note that these two cases are only identical here because we hand over 
						// the special parsing of comments in the script mode to the JS parser.
						
						int start=Position;
						bool close;
						int end=GetAppropriateEnd(out close);
						
						// Add substring as a text node:
						string script=Input.Substring(start,end-start);
						
						// Create the text element:
						TextNode el=Namespace.CreateTextNode(Document);
						el.textContent=script;
						
						// Add it now.
						Process(el,null,CurrentMode);
						
						if(close){
							// Add the close tag:
							Process(null,LastStartTag);
						}
						
					break;	
					case HtmlParseMode.Plaintext:
						
						// Plaintext to end - optimized case here:
						
						int textStart=Position-TextBlockLength;
						int textEnd=Input.Length;
						
						// From Position to Input.Length:
						Position=textEnd;
						TextBlockLength=textEnd-textStart;
						
						// Close text:
						FlushTextNode();
						
						// Just in case:
						Finish();
						return;
						
				}
				
				// Next peek char:
				peek=Peek();
				
			}
			
			// Ok:
			Finish();
			
		}
		
		/// <summary>Keeps reading until </lastStartTag> is seen.</summary>
		private int GetAppropriateEnd(out bool closing){
			
			int end=-1;
			char peek=Peek();
			
			while (peek!='\0'){
				
				if(peek=='<'){
					
					end=Position;
					Position++;
					peek = Peek();
					
					if (peek == '/'){
						
						Position++;
						
						// Keep peeking until we hit a non-letter:
						int x=0;
						
						while(IsAsciiLetter(Peek(x))){
							x++;
						}
						
						// x is the length of the tag name.
						// It needs to be equal to lastStartTag (case insensitive)
						// in order for it to successfully close.
						string tag=ReadString(x);
						
						if(tag.ToLower()==LastStartTag){
							
							// We're closing the tag.
							
							// Clear until we hit >
							char c=Read();
							
							while(c!='>' && c!='\0'){c=Read();}
							
							closing=true;
							return end;
							
						}
						
						// Not actually closing it if we fall down here.
						
					}
				
				}
				
				Position++;
				peek=Peek();
				
			}
			
			if(end==-1){
				// Terminated early.
				end=Position;
			}
			
			closing=false;
			return end;
		}
		
		/// <summary>Reads the contents of an open/close tag.</summary>
		private string ReadRawTag(bool open,bool withName){
			
			bool first=withName;
			char peek=Peek();
			bool selfClosing=false;
			
			if(peek=='<'){
				Read();
				peek=Peek();
			}
			
			string tagName=null;
			Element result=null;
			
			while(peek!=StringReader.NULL && peek!='>' && peek!='<'){
				
				string property;
				string value;
				PropertyTextReader.Read(this,Builder,first||selfClosing,out property,out value);
				property=property.ToLower();
				
				// xmlns?
				if(property=="xmlns"){
					
					// Obtain the namespace by name:
					MLNamespace targetNS=MLNamespaces.Get(value);
					
					if(targetNS!=null){
						Namespace=targetNS;
					}
					
				}else if(property.StartsWith("xmlns:")){
					
					// Declaring a namespace
					
					string[] pieces=property.Split(':');
					
					// Create it if needed:
					MLNamespaces.Get(value,pieces[1],"");
					
				}
				
				if(first){
					first=false;
					tagName=property;
				}else if(property=="/"){
					selfClosing=true;
				}else if(open){
					
					if(tagName!=null){
						
						// Create the tag now:
						result=CreateTag(tagName,false);
						
						// Clear:
						tagName=null;
						
					}
					
					// Apply the property:
					result.Properties[property]=value;
					
					// Inform the change:
					result.OnAttributeChange(property);
					
				}
				
				peek=Peek();
			}
			
			if(peek=='>'){
				Read();
			}
			
			if(!open){
				
				return tagName;
				
			}
			
			if(tagName!=null){
				
				// Set the tag now:
				result=CreateTag(tagName,true);
				
			}else{
				
				// Call tag loaded:
				result.OnTagLoaded();
				
			}
			
			result.SelfClosing=selfClosing;
			
			
			// Add it:
			Process(result,null,CurrentMode);
			
			return null;
			
		}
		
        /// <summary>
        /// Determines if the given character is an upper/lowercase character.
        /// </summary>
        /// <param name="c">The character to examine.</param>
        public static bool IsAsciiLetter(char c){
			return (c >= 0x41 && c <= 0x5a) || (c >= 0x61 && c <= 0x7a);
		}
		
		/// <summary>True if the given char is any of the HTML5 space characters (includes newlines etc).</summary>
		public static bool IsSpaceCharacter(char c){
            return c == ' ' || c == '\t' || c == '\r' || c == '\n' || c == '\f';
        }
		
		private void EndTag(){
			char c=Peek();
			
			if (IsAsciiLetter(c)){
				string tagName=ReadRawTag(false,true);
				
				// Closed tagName
				Process(null,tagName,CurrentMode);
				
			}else if (c == '>'){
				Read();
				State = HtmlParseMode.PCData;
				HandleText(true,true);
			}else if (c == '\0'){
				
				TextBlockLength+=2;
				
				FlushTextNode();
				
			}else{
				BogusComment();
			}
			
		}
		
		private void OpenPCTag(){
			
			char c=Peek();
			
			if(c == '/'){
				Read();
				EndTag();
				return;
			}
			
			if (IsAsciiLetter(c)){
				ReadRawTag(true,true);
			}else if(c == '!'){
				Position++;
				
				if (Peek(0)=='-' && Peek(1)=='-'){
					
					Position+=2;
					TextBlockLength=0;
					c=Read();
					
					if(c=='-'){
						
						// 8.2.4.47 Comment start dash state
						
						c=Read();
						
						if(c=='-'){
							if(CommentEnd()){
								return;
							}
						}else if(c=='>'){
							State = HtmlParseMode.PCData;
						}else if(c=='\0'){
							
							// Empty comment:
							FlushCommentNode(0);
							
						}else{
							TextBlockLength++;
							LoadComment();
						}
						
					}else if(c=='>'){
						State = HtmlParseMode.PCData;
						return;
					}else if(c=='\0'){
						// Do nothing.
						return;
					}else{
						TextBlockLength++;
						LoadComment();
					}
					
				}else if(PeekLower("doctype")){
					
					Position+=7;
					
					// Skip spaces:
					PropertyTextReader.SkipSpaces(this);
					
					c=Read();
					
					DocumentType docType=new DocumentType("");
					
					if (c == '>'){
						docType.quirksMode=true;
						State = HtmlParseMode.PCData;
					}else if (c == '\0'){
						docType.quirksMode=true;
					}else{
						// Started reading the name.
						int nameIndex=Position-1;
						int length=1;
						
						while (true){
							c = Read();
							
							if(IsSpaceCharacter(c)){
								docType.name_ = Input.Substring(nameIndex,length);
								
								PropertyTextReader.SkipSpaces(this);
								c=Read();
								
								if (c == '>'){
									State = HtmlParseMode.PCData;
								}else if(c == '\0'){
									docType.quirksMode = true;
								}else if((c=='p' || c=='P') && PeekLower("ublic")){
									Position+=5;
									
									// Public doctype.
									
									docType.ParsePublic(this);
									
								}else if((c=='s' || c=='S') && PeekLower("ystem")){
									Position+=5;
									
									// System doctype.
									
									docType.ParseSystem(this);
									
								}else{
									docType.quirksMode = true;
									
									// Broken doctype.
									
									docType.ParseBroken(this);
									
								}
								
								break;
								
							}else if (c == '>'){
								State = HtmlParseMode.PCData;
								docType.name_ = Input.Substring(nameIndex,length);
								break;
							}else if (c == '\0'){
								docType.quirksMode = true;
								docType.name_ = Input.Substring(nameIndex,length);
								break;
							}else{
								length++;
							}
							
						}
						
					}
					
					Process(docType,null);
					return;
					
				}else if(Peek("[CDATA[")){
					
					Position+=7;
					TextBlockLength=0;
					c=Peek();
					
					while(c!='\0'){
						
						if(c == ']' && Peek("]]>")){
							
							// Close comment if there is any:
							FlushCommentNode(0);
							
							Position+=3;
							
							return;
							
						}else{
							Position++;
							TextBlockLength++;
							c = Peek();
						}
						
					}
					
					// Close comment if there is any:
					FlushCommentNode(0);
					
				}else{
					BogusComment();
				}
				
			}else if(c != '?'){
				State = HtmlParseMode.PCData;
				HandleText(true,true);
			}else{
				BogusComment();
			}
			
		}
		
		private Comment FlushCommentNode(int positionDelta){
			
			if(TextBlockLength==0){
				return null;
			}
			
			// Get and clear the text:
			string text=Input.Substring(Position+positionDelta-TextBlockLength,TextBlockLength);
			TextBlockLength=0;
			
			// Create the node:
			Comment el=Namespace.CreateCommentNode(Document);
			el.textContent=text;
			
			// Add it now.
			Process(el,null,CurrentMode);
			
			return el;
			
		}
		
		/// <summary>
		/// Reads a comments body.
		/// </summary>
		private void LoadComment(){
			
			while (true){
				
				char c=Read();
				
				if(c=='-'){
					if(CommentDashEnd()){
						return;
					}
				}else if(c=='\0'){
					FlushCommentNode(0);
					return;
				}else{
					TextBlockLength++;
				}
				
			}
			
		}
		
		/// <summary>
		/// See 8.2.4.49 Comment end dash state
		/// </summary>
		private bool CommentDashEnd(){
			
			char c=Read();
			
			if(c=='-'){
				return CommentEnd();
			}else if(c=='\0'){
				FlushCommentNode(0);
				return true;
			}else{
				TextBlockLength+=2;
			}
			
			// Didn't actually terminate.
			return false;
			
		}
		
		/// <summary>Checks if the comment has ended.</summary>
		private bool CommentEnd(){
			
			while (true){
				
				char c=Read();
				
				if(c=='>'){
					
					State = HtmlParseMode.PCData;
					FlushCommentNode(-3);
					return true;
					
				}else if(c=='!'){
					// 8.2.4.51 Comment end bang state
					
					c=Read();
					
					if(c=='-'){
						TextBlockLength+=3;
						if(CommentDashEnd()){
							return true;
						}
					}else if(c=='>'){
						State = HtmlParseMode.PCData;
						return true;
					}else if(c=='\0'){
						return true;
					}else{
						TextBlockLength+=4;
					}
					
					break;
				}else if(c=='-'){
					// (Syntax error)
					TextBlockLength++;
					break;
				}else if(c=='\0'){
					FlushCommentNode(0);
					return true;
				}else{
					TextBlockLength+=3;
					break;
				}
				
			}
			
			return false;
		}
		
		private void OpenRCTag(){
			
			char peek=Peek();
			
			if(peek == '/'){
				
				Position++;
				
				// Keep peeking until we hit a non-letter:
				int x=0;
				
				while(IsAsciiLetter(Peek(x))){
					x++;
				}
				
				// x is the length of the tag name.
				// It needs to be equal to lastStartTag (case insensitive)
				// in order for it to successfully close.
				string tag=ReadString(x);
				
				if(tag.ToLower()==LastStartTag){
					
					// Clear until we hit >
					char c=Read();
					
					while(c!='>' && c!='\0'){c=Read();}
					
					// Process it:
					Process(null,LastStartTag);
					
					return;
					
				}
				
				// Include the </ as text:
				TextBlockLength+=2;
				
			}else{
				
				// Include the < as text:
				TextBlockLength++;
				
			}
			
			HandleText(true,true);
			
		}
		
		/// <summary>Creates a close tag if one is appropriate.</summary>
		private bool CreateIfAppropriate(char c){
			
			if(Builder.Length!=LastStartTag.Length){
				return false;
			}
			
			if((c=='>' || c=='/' || IsSpaceCharacter(c)) && Builder.ToString()==LastStartTag){
				
				// Read it off:
				Read();
				
				// Clear builder:
				Builder.Length=0;
				
				// Close tag:
				if(c!='/' && c!='>'){
					
					// Read attribs only (and just dump them):
					ReadRawTag(false,false);
					
				}
				
				Process(null,LastStartTag,CurrentMode);
				
				return true;
			}
			
			return false;
			
		}
		
		/// <summary>
		/// See 8.2.4.44 Bogus comment state
		/// </summary>
		/// <param name="c">The current character.</param>
		private void BogusComment(){
			
			char c=Peek(0);
			TextBlockLength=0;
			
			while (c!='\0' && c!='>'){
				
				TextBlockLength++;
				c=Peek(TextBlockLength);
				
			}
			
			State = HtmlParseMode.PCData;
			FlushComment();
			
		}

		/// <summary>Creates a text content block.</summary>
		private void HandleText(bool stopAtTag,bool allowVars){
			
			// While we don't have a <, read text.
			// Look out for &, EOF and <.
			char peek=Peek();
			
			while(peek!=StringReader.NULL){
				
				if(stopAtTag && peek=='<'){
					break;
				}
				
				if(allowVars && peek=='&'){
					
					// Variable.
					AddVariable();
					
				}else{
					
					// Advance:
					TextBlockLength++;
					Position++;
					
				}
				
				// Next peek char:
				peek=Peek();
				
			}
			
			// Close text if there is any:
			FlushTextNode();
			
		}
		
		/// <summary>The current tag on the top of the stack.</summary>
		public string CurrentTag{
			get{
				Element current=CurrentElement;
				
				if(current==null){
					return null;
				}
				
				return current.Tag;
			}
		}
		
		/// <summary>The current open node.</summary>
		public Node CurrentNode{
			get{
				int count=OpenElements.Count;
				
				if(count==0){
					return Document;
				}
				
				return OpenElements[count-1];
			}
		}
		
		/// <summary>The current open element.</summary>
		public Element CurrentElement{
			get{
				int count=OpenElements.Count;
				
				if(count==0){
					return null;
				}
				
				return OpenElements[count-1];
			}
		}
		
		/// <summary>Pushes a new open element.</summary>
		public void Push(Element el,bool stack){
			
			if(_foster && CurrentElement.IsTableStructure){
				AddElementWithFoster(el);
			}else{
				
				Node current=CurrentElement;
				
				if(current==null){
					current=Document;
				}
				
				current.appendChild(el);
			}
			
			if(stack){
				OpenElements.Add(el);
			}else{
				// Kids loaded already:
				el.OnChildrenLoaded();
			}
			
		}
		
		private void AddElementWithFoster(Element element){
			
			bool table = false;
			int index = OpenElements.Count;

			while (--index != 0){
				
				if (OpenElements[index].Tag=="template"){
					OpenElements[index].appendChild(element);
					return;
				}else if (OpenElements[index].Tag=="table"){
					table = true;
					break;
				}
				
			}
			
			Node foster = OpenElements[index].parentNode_;
			
			if(foster==null){
				foster=OpenElements[index + 1];
			}
			
			if(table && OpenElements[index].parentNode_ != null){
				
				for(int i=0;i<foster.childCount;i++){
					
					if (foster.childNodes_[i] == OpenElements[index]){
						foster.insertChild(i,element);
						break;
					}
					
				}
				
			}else{
				foster.appendChild(element);
			}
			
		}
		
		public void Process(Node node,string close){
			Process(node,close,CurrentMode);
		}
		
		public void Process(Node node,string close,int mode){
			
			text_=null;
			Element el=node as Element;
			string open=null;
			
			if(el!=null){
				open=el.Tag;
			}
			
			// If it's a close or open tag:
			if(el!=null){
				
				// Change state back to PCData:
				State = HtmlParseMode.PCData;
				
				// Note open tag:
				LastStartTag=open;
				
			}else if(close!=null){
				
				// Change state back to PCData:
				State = HtmlParseMode.PCData;
				
			}
			
			// OnLexerAddNode checks CurrentMode and handles itself accordingly.
			// The most common case should be checked first (depends on node implementations).
			// Essentially the more broken your HTML is, the slower it goes!
			if(node!=null && node.OnLexerAddNode(this,mode)){
				// It handled itself.
				return;
			}else if(close!=null && CallCloseMethod(close,mode)){
				// It handled itself.
				return;
			}
			
			// Default state for the current mode:
			
			switch(mode){
				
				case HtmlTreeMode.Initial:
					
					// Reprocess it as BeforeHtml:
					CurrentMode = HtmlTreeMode.BeforeHtml;
					Process(node,close,CurrentMode);
					
				break;
				
				case HtmlTreeMode.BeforeHtml:
					
					if(close==null){
						
						// Otherwise it's an end tag which didn't match any of the acceptable ones; Ignore it.
						BeforeHtmlElse(node,close);
						
					}
					
				break;
				
				case HtmlTreeMode.BeforeHead:
					
					// Create a head node:
					el=CreateTag("head",true);
					Push(el,true);
					head=el;
					
					// Switch to in head and reprocess:
					CurrentMode=HtmlTreeMode.InHead;
					Process(node,close,CurrentMode);
					
				break;
				
				case HtmlTreeMode.InHead:
					
					if(close==null){ // Ignore other close tags.
						
						InHeadElse(node,null);
						
					}
					
				break;
				
				case HtmlTreeMode.InHeadNoScript:
					
					// Ignore any other close tags:
					if(close==null){
						
						// Reprocess as in head:
						
						// Switch mode:
                        CurrentMode = HtmlTreeMode.InHead;
						Process(node,null,CurrentMode);
						
					}
					
				break;
				
				case HtmlTreeMode.AfterHead:
					
					if(close==null){ // Ignore other close tags.
						
						AfterHeadElse(node,close);
						
					}
					
				break;
				
				case HtmlTreeMode.InCaption:
				case HtmlTreeMode.InCell:
					
					// Reprocess as 'in body'
					Process(node,close,HtmlTreeMode.InBody);
					
				break;
				case HtmlTreeMode.InBody:
					
					// In body mode.
					
					if(el!=null){
						
						// Any other open tag.
						
						ReconstructFormatting();
						
						// Push it, accounting for its self closing state:
						Push(el,!(el.SelfClosing || el.IsSelfClosing));
						
					}else if(close!=null){
						
						// Any other close tag.
						
						InBodyEndTagElse(close);
						
					}
					
				break;
				
				case HtmlTreeMode.Text:
					
					// Text mode:
					CloseCurrentNode();
					CurrentMode = PreviousMode;
					
				break;
				
				case HtmlTreeMode.InRow:
				case HtmlTreeMode.InTableBody:
				case HtmlTreeMode.InTable:
					
					InTableElse(node,close);
					
				break;
				case HtmlTreeMode.InTemplate:
					
					if(open!=null){
						
						TemplateStep(node,close,HtmlTreeMode.InBody);
						
					}
					
				break;
				
				case HtmlTreeMode.InColumnGroup:
					
					if(CurrentElement.Tag=="colgroup"){
						// Ignore otherwise
						
						CloseCurrentNode();
						
						// Change mode:
						CurrentMode=HtmlTreeMode.InTable;
						Process(node,close,CurrentMode);
						
					}
					
				break;
				
				case HtmlTreeMode.AfterBody:
					
					// Switch:
					CurrentMode=HtmlTreeMode.InBody;
					Process(node,close,CurrentMode);
					
				break;
				
				case HtmlTreeMode.AfterAfterBody:
					
					// Switch and reproc:
					CurrentMode=HtmlTreeMode.InBody;
					Process(node,close);
					
				break;
				
			}
			
		}
		
		/// <summary>used by e.g. pre; skips a newline if there is one.</summary>
		public void SkipNewline(){
			// Read off the first newline:
			char peek=Peek();
			
			if(peek=='\n'){
				Read();
			}else if(peek=='\r'){
				Read();
				if(Peek(1)=='\n'){
					Read();
				}
			}
			
		}
		
		public void CloseParagraph(){
			
			// Close a 'p' element.
			// http://w3c.github.io/html/syntax.html#close-a-p-element
			
			GenerateImpliedEndTagsExceptFor("p");
			
			// Close inclusive:
			CloseInclusive("p");
			
		}
		
		/// <summary>Closes a paragraph in button scope then pushes the given element.</summary>
		public void CloseParagraphThenAdd(Element el){
			
			if(IsInButtonScope("p")){
				CloseParagraph();
			}
			
			Push(el,true);
			
		}
		
		public void CloseParagraphButtonScope(){
			
			if(IsInButtonScope("p")){
				CloseParagraph();
			}
			
		}
		
		/// <summary>The all other nodes route when in the 'in table' mode.</summary>
		public void InTableElse(Node node,string close){
			
			_foster = true;
			Process(node,close,HtmlTreeMode.InBody);
			_foster = false;
			
		}
		
		/// <summary>Closes the cell if the given close tag is in scope, then reprocesses it.</summary>
		public void CloseTableZoneInCell(string close){
			
			if(IsInTableScope(close)){
				
				CloseCell();
				
				// Reproc:
				Process(null,close,CurrentMode);
				return;
				
			}
			
		}
		
		/// <summary>Handles head-favouring tags when in the 'after head' mode.
		/// Base, link, meta etc are examples of favouring tags; they prefer to be in the head.</summary>
		public void AfterHeadHeadTag(Node node){
			
			// Push head onto the stack only:
			OpenElements.Add(head);
			
			// process in head:
			Process(node,null,HtmlTreeMode.InHead);
			
			// Pop:
			CloseCurrentNode();
			
		}
		
		/// <summary>Closes to table body context if tbody, head or foot are in scope.</summary>
		public void CloseToTableBodyIfBody(Node node,string close){
			
			if(IsInTableScope("tbody") || IsInTableScope("thead") || IsInTableScope("tfoot")){
				// Ignore otherwise.
				
				CloseToTableBodyContext();
				
				CloseCurrentNode();
				
				// Reprocess:
				CurrentMode=HtmlTreeMode.InTable;
				Process(node,close,CurrentMode);
				
			}
			
		}
		
		/// <summary>Closes a caption (if it's in scope) and reprocesses the node in table mode.</summary>
		public void CloseCaption(Node node,string close){
			
			if(IsInTableScope("caption")){
				// Ignore the token otherwise.
				
				// Generate implied:
				GenerateImpliedEndTags();	
				
				// Close upto and incl. caption:
				CloseInclusive("caption");
				
				// Clear to last marker:
				ClearFormatting();
				
				// Table mode:
				CurrentMode=HtmlTreeMode.InTable;
				Process(node,close,CurrentMode);
				
			}
			
		}
		
		/// <summary>Triggers CloseCell if th or td are in scope.</summary>
		public void CloseIfThOrTr(Node node,string close){
			
			if(IsInTableScope("th") || IsInTableScope("td")){
				// Ignore otherwise.
				
				CloseCell();
				
				// Reprocess:
				Process(node,close,CurrentMode);
				
			}
			
		}
		
		/// <summary>Closes to a table context and switches to table body if a tr is in scope.</summary>
		public void TableBodyIfTrInScope(Node node,string close){
			
			if(IsInTableScope("tr")){
				// Ignore otherwise.
				
				CloseToTableBodyContext();
				
				CloseCurrentNode();
				CurrentMode=HtmlTreeMode.InTableBody;
				
				// Reproc:
				Process(node,close,CurrentMode);
				
			}
			
		}
		
		public void CloseSelect(bool skipScopeCheck,Node node,string close){
			
			if(skipScopeCheck || IsInSelectScope(close)){
				
				CloseInclusive("select");
				
				Reset();
				
				// Reproc:
				Process(node,close,CurrentMode);
				
			}
			
		}
		
		/// <summary>Closes a table cell.</summary>
		public void CloseCell(){
			
			// Generate implied:
			GenerateImpliedEndTags();
			
			// Pop up to and incl 'td' or 'th':
			Element el=CurrentElement;
			
			while(el.Tag!="td" && el.Tag!="th"){
				CloseCurrentNode();
				el=CurrentElement;
			}
			
			if(el.Tag=="td" || el.Tag=="th"){
				CloseCurrentNode();
			}
			
			ClearFormatting();
			
			CurrentMode=HtmlTreeMode.InRow;
			
		}
		
		public void BeforeHtmlElse(Node node,string close){
			
			// Create a html node:
			Element el=CreateTag("html",true);
			Push(el,true);
			
			// Switch to before head and reprocess:
			CurrentMode=HtmlTreeMode.BeforeHead;
			Process(node,close,CurrentMode);
		}
		
		/// <summary>Any other end tag has been found in the InBody state.</summary>
		/// <param name="tag">The actual tag found.</param>
		private void InBodyEndTagElse(string close){
			int index = OpenElements.Count - 1;
			Element node = CurrentElement;

			while (node != null){
				if (node.Tag==close){
					GenerateImpliedEndTagsExceptFor(close);
					CloseNodesFrom(index);
					break;
				}else if(node.IsSpecial){
					break;
				}

				node = OpenElements[--index];
			}
			
		}

		/// <summary>This attempts to recover mis-nested tags. For example <b><i>Hi!</b></i> is relatively common.
		/// This is aka the Heisenburg algorithm, but it's named 'adoption agency' in HTML5.</summary>
		/// <param name="tag">The actual tag given.</param>
		public void AdoptionAgencyAlgorithm(string tag){
			
			int outer = 0;
			int inner = 0;
			int bookmark = 0;
			int index = 0;
			
			while (outer < 8){
				
				Element formattingElement = null;
				Element furthestBlock = null;

				outer++;
				index = 0;
				inner = 0;
				
				for (int j = FormattingElements.Count - 1; j >= 0 && FormattingElements[j] != null; j--){
					if (FormattingElements[j].Tag==tag){
						index = j;
						formattingElement = FormattingElements[j];
						break;
					}
				}

				if (formattingElement == null){
					// Note that tag is always a close node.
					InBodyEndTagElse(tag);
					break;
				}
				
				int openIndex = OpenElements.IndexOf(formattingElement);

				if (openIndex == -1){
					// HtmlParseError.FormattingElementNotFound
					FormattingElements.Remove(formattingElement);
					break;
				}
				
				if (!IsInScope(formattingElement.Tag)){
					// HtmlParseError.ElementNotInScope
					break;
				}

				if (openIndex != OpenElements.Count - 1){
					// HtmlParseError.TagClosedWrong
				}
				
				bookmark = index;

				for (int j = openIndex + 1; j < OpenElements.Count; j++){
					
					if (OpenElements[j].IsSpecial){
						index = j;
						furthestBlock = OpenElements[j];
						break;
					}
					
				}
				
				if (furthestBlock == null){
					do{
						furthestBlock = CurrentElement;
						CloseCurrentNode();
					}while (furthestBlock != formattingElement);
					
					FormattingElements.Remove(formattingElement);
					break;
				}
				
				Element commonAncestor = OpenElements[openIndex - 1];
				Element node = furthestBlock;
				Element lastNode = furthestBlock;

				while (true){
					
					inner++;
					node = OpenElements[--index];

					if (node == formattingElement){
						break;
					}

					if (inner > 3 && FormattingElements.Contains(node)){
						FormattingElements.Remove(node);
					}

					if (!FormattingElements.Contains(node)){
						CloseNode(node);
						continue;
					}
					
					Element newElement = node.cloneNode(false) as Element;
					commonAncestor.appendChild(newElement);
					OpenElements[index] = newElement;
					
					for (int l = 0; l != FormattingElements.Count; l++){
						if (FormattingElements[l] == node){
							FormattingElements[l] = newElement;
							break;
						}
					}

					node = newElement;

					if (lastNode == furthestBlock){
						bookmark++;
					}

					if(lastNode.parentNode_!=null){
						lastNode.parentNode_.removeChild(lastNode);
					}
					
					node.appendChild(lastNode);
					lastNode = node;
				}
				
				if(lastNode.parentNode_!=null){
					lastNode.parentNode_.removeChild(lastNode);
				}
				
				if (!commonAncestor.IsTableStructure){
					commonAncestor.appendChild(lastNode);
				}else{
					AddElementWithFoster(lastNode);
				}
				
				Element element = formattingElement.cloneNode(false) as Element;
				
				while (furthestBlock.childNodes_.length > 0){
					Node childNode = furthestBlock.childNodes_[0];
					furthestBlock.removeChildAt(0);
					element.appendChild(childNode);
				}
				
				furthestBlock.appendChild(element);
				FormattingElements.Remove(formattingElement);
				FormattingElements.Insert(bookmark, element);
				CloseNode(formattingElement);
				OpenElements.Insert(OpenElements.IndexOf(furthestBlock) + 1, element);
			}
			
		}
		
		/// <summary>Checks if the named tag is currently open on the formatting stack.</summary>
		public Element FormattingCurrentlyOpen(string tagName){
			
			// Start from the top of the stack because that's most likely where we'll find it.
			for(int i = FormattingElements.Count-1; i >=0; i--){
				
				Element fmt=FormattingElements[i];
				
				if(fmt==null){
					// Marker - stop there.
					break;
				}
				
				if(fmt.Tag==tagName){
					return fmt;
				}
				
			}
			
			return null;
		}
		
		/// <summary>Adds a formatting element.</summary>
		public void AddFormatting(Element element){
			int count = 0;

			for (int i = FormattingElements.Count - 1; i >= 0; i--){
				
				Element format = FormattingElements[i];

				if (format == null){
					break;
				}
				
				if (format.Tag==element.Tag && 
				format.Namespace==element.Namespace && 
				Node.PropertiesEqual(format.Properties,element.Properties) && ++count == 3)
				{
					FormattingElements.RemoveAt(i);
					break;
				}
			}
			
			FormattingElements.Add(element);
		}

		
		/// <summary>
		/// Clears formatting info to the last marker.
		/// </summary>
		public void ClearFormatting(){
			
			while (FormattingElements.Count != 0){
				
				int index = FormattingElements.Count - 1;
				Element entry = FormattingElements[index];
				FormattingElements.RemoveAt(index);
				
				if (entry == null){
					break;
				}
				
			}
			
		}
		
		/// <summary>
		/// Adds a formatting scope marker.
		/// </summary>
		public void AddScopeMarker(){
			FormattingElements.Add(null);
		}
		
		/// <summary>
		/// Reconstruct the list of active formatting elements, if any.
		/// </summary>
		public void ReconstructFormatting(){
			
			if (FormattingElements.Count == 0){
				return;
			}
			
			int index = FormattingElements.Count - 1;
			Element entry = FormattingElements[index];

			if (entry == null || OpenElements.Contains(entry)){
				return;
			}
			
			while (index > 0){
				entry = FormattingElements[--index];

				if (entry == null || OpenElements.Contains(entry)){
					index++;
					break;
				}
			}

			for (; index < FormattingElements.Count; index++)
			{
				
				// Clone it:
				Element element = FormattingElements[index].cloneNode(false) as Element;
				
				// Add:
				Push(element,true);
				
				FormattingElements[index] = element;
			}
			
		}
		
		/// <summary>Closes a marked formatting element like object or applet.</summary>
		public void CloseMarkedFormattingElement(string close){
			
			if(IsInScope(close)){
				
				GenerateImpliedEndTags();
				
				CloseInclusive(close);
				
				// Clear to marker:
				ClearFormatting();
				
			}
			
		}
		
		
		/// <summary>Adds a marked formatting element like object or applet.</summary>
		public void AddMarkedFormattingElement(Element el){
			
			ReconstructFormatting();
			Push(el,true);
			AddScopeMarker();
			FramesetOk=false;
			
		}
		
		public void AddFormattingElement(Element el){
			
			// 'in body', A start tag whose tag name is one of: "b", "big", "code", "em", ..
			
			ReconstructFormatting();
			Push(el,true);
			AddFormatting(el);
			
		}
		
		/// <summary>True if the given tag is in list item scope.</summary>
		public bool IsInListItemScope(string tagName){
			
			for (int i = OpenElements.Count - 1; i >= 0; i--){
				
				Element node = OpenElements[i];
				string tag=node.Tag;
				
				if(tag==tagName){
					return true;
				}else if(node.IsParserScope || tag=="ol" || tag=="ul"){
					return false;
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>True if the given tag is in element scope.</summary>
		public bool IsInScope(string tagName){
			
			for (int i = OpenElements.Count - 1; i >= 0; i--){
				
				Element node = OpenElements[i];
				
				if(node.Tag==tagName){
					return true;
				}else if(node.IsParserScope){
					return false;
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>True if the given tag is in button scope.</summary>
		public bool IsInButtonScope(string tagName){
			
			for (int i = OpenElements.Count - 1; i >= 0; i--){
				
				Element node = OpenElements[i];
				
				if(node.Tag==tagName){
					return true;
				}else if(node.Tag=="button" || node.IsParserScope){
					return false;
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>True if the given tag is in table scope.</summary>
		public bool IsInTableScope(string tagName){
			
			for (int i = OpenElements.Count - 1; i >= 0; i--){
				
				Element node = OpenElements[i];
				
				if(node.Tag==tagName){
					return true;
				}else if(node.IsTableContext){
					return false;
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>True if the given tag is in select scope.</summary>
		public bool IsInSelectScope(string tagName){
			
			for (int i = OpenElements.Count - 1; i >= 0; i--){
				
				Element node = OpenElements[i];
				
				if(node.Tag==tagName){
					return true;
				}else if(node.Tag!="option" && node.Tag!="optgroup"){
					return false;
				}
				
			}
			
			return false;
			
		}
		
		public void CloseInclusive(string tag){
			
			// Pop up to and incl:
			while(CurrentElement.Tag!=tag){
				CloseCurrentNode();
			}
			
			if(CurrentElement.Tag==tag){
				CloseCurrentNode();
			}
			
		}
		
		/// <summary>Closes all nodes from the given open element stack index. Inclusive.</summary>
		public void CloseNodesFrom(int index){
			
			for (int i = OpenElements.Count - 1; i >= index; i--){
				CloseCurrentNode();
			}
			
		}
		
		/// <summary>Close to a table body context. thead, tfoot, tbody, html and template.</summary>
		public void CloseToTableRowContext(){
			
			while(!CurrentElement.IsTableRowContext){
				
				CloseCurrentNode();
				
			}
			
		}
		
		/// <summary>Close to a table body context. thead, tfoot, tbody, html and template.</summary>
		public void CloseToTableBodyContext(){
			
			while(!CurrentElement.IsTableBodyContext){
				
				CloseCurrentNode();
				
			}
			
		}
		
		/// <summary>Close to a table context.</summary>
		public void CloseToTableContext(){
			
			while(!CurrentElement.IsTableContext){
				
				CloseCurrentNode();
				
			}
			
		}
		
		/// <summary>Input or textarea in select mode.</summary>
		public void InputOrTextareaInSelect(Element el){
			
			if(IsInSelectScope("select")){
				
				CloseInclusive("select");
				
				Reset();
				
				// Reproc:
				Process(el,null);
				
			}
			
		}
		
		/// <summary>'Generic raw text element parsing algorithm'. Adds the current node then switches to the given state, whilst also changing the mode to Text.</summary>
		public void RawTextOrRcDataAlgorithm(Element el,HtmlParseMode stateAfter){
			
			// Append it:
			Push(el,true);
			
			// Switch to RawText:
			PreviousMode = CurrentMode;
			CurrentMode = HtmlTreeMode.Text;
			State = stateAfter;
			
		}
		
		/// <summary>Anything else in the 'after head' mode.</summary>
		public void AfterHeadElse(Node node,string close){
			
			// Create a body element:
			Element el=CreateTag("body",true);
			Push(el,true);
			
			// Switch:
			CurrentMode=HtmlTreeMode.InBody;
			Process(node,close,CurrentMode);
			
		}
		
		/// <summary>Anything else in the 'in head' mode.</summary>
		public void InHeadElse(Node node,string close){
			
			// Anything else.
			
			// Close the head tag.
			CloseCurrentNode();
			
			// Switch mode:
			CurrentMode = HtmlTreeMode.AfterHead;
			Process(node,close,CurrentMode);
			
		}
		
		/// <summary>Combines the attribs of the given element into target.
		/// Adds the attributes to target if they don't exist (doesn't overwrite).</summary>
		public void CombineInto(Element el,Element target){
			
			foreach(KeyValuePair<string,string> kvp in el.Properties){
				
				// Add the attrib if it doesn't exist:
				if(!target.Properties.ContainsKey(kvp.Key)){
					target.setAttribute(kvp.Key, kvp.Value);
				}
				
			}
			
		}
		
		/// <summary>Attempts to close a block element.</summary>
		public void BlockClose(string close){
			
			if(IsInScope(close)){
				
				GenerateImpliedEndTags();
				
				CloseInclusive(close);
				
			}
			
		}
		
		/// <summary>Checks if the named tag is currently open.</summary>
		public bool TagCurrentlyOpen(string tagName){
			
			// Start from the top of the stack because that's most likely where we'll find it.
			for(int i = OpenElements.Count-1; i >=0; i--){
				
				if(OpenElements[i].Tag==tagName){
					return true;
				}
				
			}
			
			return false;
		}
		
		/// <summary>
		/// Inserting something in the template.
		/// </summary>
		/// <param name="token">The token to insert.</param>
		/// <param name="mode">The mode to push.</param>
		public void TemplateStep(Node node,string close,int mode){
			TemplateModes.Pop();
			TemplateModes.Push(mode);
			CurrentMode = mode;
			Process(node,close,mode);
		}
		
		/// <summary>
		/// Closes the template element.
		/// </summary>
		public void CloseTemplate(){
			
			Element node = CurrentElement;
			
			while (node!=null && node.ImplicitEndAllowed==ImplicitEndMode.Normal && node.Tag!="template"){
				CloseCurrentNode();
				node = CurrentElement;
			}
			
			ClearFormatting();
			TemplateModes.Pop();
			Reset();
			
		}
		
		/// <summary>Generate implicit end tags.</summary>
		public void Finish(){
			
			/*
			// The only other side effects of this involve tidying the lexer up.
			// Not necessary because we're terminating anyway.
			
			if(TemplateModes.Count>0){
				
				// Close the template first:
				if (TagCurrentlyOpen("template")){
					
					CloseTemplate();
					
				}
				
			}
			*/
			
			Element node = CurrentElement;
			
			while(node!=null){
				CloseCurrentNode();
				node = CurrentElement;
			}
			
		}
		
		/// <summary>Generate implicit end tags.</summary>
		public void GenerateImpliedEndTags(){
			
			Element node = CurrentElement;
			
			// Normal only:
			while(node!=null && node.ImplicitEndAllowed==ImplicitEndMode.Normal){
				CloseCurrentNode();
				node = CurrentElement;
			}
			
		}
		
		/// <summary>Generate implicit end tags.</summary>
		public void GenerateImpliedEndTagsThorough(){
			
			Element node = CurrentElement;
			
			// Normal and thorough:
			while(node!=null && ( (node.ImplicitEndAllowed & ImplicitEndMode.Thorough)!=0 )){
				CloseCurrentNode();
				node = CurrentElement;
			}
			
		}
		
		/// <summary>Generate implicit end tags.</summary>
		public void GenerateImpliedEndTagsExceptFor(string tagName){
			
			Element node = CurrentElement;
			
			// Applies to normal only:
			while (node!=null && node.ImplicitEndAllowed==ImplicitEndMode.Normal && node.Tag!=tagName){
				CloseCurrentNode();
				node = CurrentElement;
			}
			
		}
		
		public void CloseNode(Element el){
			
			// Kids are now ready:
			el.OnChildrenLoaded();
			
			// Remove it:
			OpenElements.Remove(el);
			
			// Update namespace:
			int index=OpenElements.Count-1;
			
			if(index!=0){
				Namespace=OpenElements[index-1].Namespace;
			}
			
		}
		
		/// <summary>
		/// Pops the last node from the stack of open nodes.
		/// </summary>
		public void CloseCurrentNode(){
			
			int index=OpenElements.Count-1;
			
			if(index<0){
				return;
			}
			
			// Kids are now ready:
			OpenElements[index].OnChildrenLoaded();
			
			// Pop:
			OpenElements.RemoveAt(index);
			
			// Update namespace:
			if(index!=0){
				Namespace=OpenElements[index-1].Namespace;
			}
			
		}
		
		/// <summary>Writes out any pending text as a comment node.</summary>
		private void FlushComment(){
			
			if(TextBlockLength==0){
				return;
			}
			
			// Get and clear the text:
			string text=Input.Substring(Position-TextBlockLength,TextBlockLength);
			TextBlockLength=0;
			
			// Create the text element:
			Comment el=Namespace.CreateCommentNode(Document);
			el.textContent=text;
			
			// Add comment node
			Process(el,null,CurrentMode);
			
		}
		
		/// <summary>Writes out any pending text to a text element.</summary>
		private TextNode FlushTextNode(){
			
			if(TextBlockLength==0){
				return null;
			}
			
			// Get and clear the text:
			string text=Input.Substring(Position-TextBlockLength,TextBlockLength);
			TextBlockLength=0;
			
			TextNode el=text_;
			
			if(el==null){
				
				// Create the text element:
				el=Namespace.CreateTextNode(Document);
				el.textContent=text;
				
				// Add it now.
				Process(el,null,CurrentMode);
				
				// Set as latest text node:
				text_=el;
				
			}else{
				
				// Great - append to an existing node. This happens after &these; are seen.
				el.appendData(text);
				
			}
			
			return el;
			
		}
		
		/// <summary>Appends text to the given node or creates a new node if it's null.</summary>
		private TextNode AppendText(TextNode node,string text){
			
			if(node==null){
				
				// Create the text element:
				node=Namespace.CreateTextNode(Document);
				node.textContent=text;
				
				// Add it now.
				Process(node,null,CurrentMode);
				
			}else{
				
				// Append:
				node.appendData(text);
				
			}
			
			return node;
			
		}
		
		/// <summary>Reads out a &variable; (as used by PowerUI for localization purposes).</summary>
		private void AddVariable(){
			
			// Flush textElement if we're working on one (likely):
			TextNode textNode=FlushTextNode();
			
			// Read off the &:
			Read();
			int nameLength=0;
			
			char peek=Peek();
			string variableString="";
			List<string> variableArguments=null;
			
			while(peek!='\0' && peek!='>'){
				
				if(peek==';'){
					
					if(nameLength!=0){
						// The name is in the builder:
						variableString=Input.Substring(Position-nameLength,nameLength);
					}
					
					// Read off the ;
					Read();
					
					// Check if this string (e.g. &WelcomeMessage;) is provided by the standard variable set:
					string stdEntity=CharacterEntities.GetByValueOrName(variableString);
					
					if(stdEntity==null){
						
						// Not a standard entity; we have e.g. &Welcome;
						
						// Generate a new variable element:
						Node varElement=Namespace.CreateLangNode(Document);
						
						if(varElement==null){
							
							// This namespace doesn't support this kind of variable.
							// Add it as a literal string instead.
							
							// Append:
							textNode=AppendText(textNode,"&"+variableString+";");
							
						}else{
							
							// Can no longer append to the previous text node:
							textNode=null;
							text_=null;
							
							ILangNode langInterface=varElement as ILangNode;
							
							langInterface.SetVariableName(variableString);
							
							if(variableArguments!=null){
								langInterface.SetArguments(variableArguments.ToArray());
								variableArguments=null;
							}
							
							langInterface.LoadNow();
							
							// Append it:
							CurrentNode.appendChild(varElement);
							
						}
						
					}else{
						
						// Got a standard entity! Append stdEntity to text node, or create a new one if it needs it.
						textNode=AppendText(textNode,stdEntity);
						
					}
					
					return;
					
				}else if(peek=='('){
					// Read runtime argument set. &WelcomeMessage('heya!');
					variableString=Input.Substring(Position-nameLength,nameLength);
					nameLength=0;
					
					// Read off the bracket:
					Read();
					
					peek=Peek();
					variableArguments=new List<string>();
					
					while(peek!=StringReader.NULL){
						
						if(peek==')'){
							// Add it:
							variableArguments.Add(Input.Substring(Position-nameLength,nameLength));
							nameLength=0;
							
							// Read it off:
							Read();
							break;
						}else if(peek==','){
							
							// Done one parameter - onto the next.
							variableArguments.Add(Input.Substring(Position-nameLength,nameLength));
							nameLength=0;
							
						}else if(peek=='"' || peek=='\''){
							
							// One of our args is a "string".
							// Use the string reader of the PropertyTextReader to read it.
							PropertyTextReader.ReadString(this,Builder);
							
							// We don't want to read a char off, so continue here.
							// Peek the next one:
							peek=Peek();
							
							continue;
							
						}else if(peek!=' '){
							
							// General numeric args will fall down here.
							// Disallowing spaces means the set can be spaced out like so: (14, 21)
							nameLength++;
							
						}
						
						// Read off the char:
						Read();
						
						// Peek the next one:
						peek=Peek();
						
					}
					
				}else if(IsSpaceCharacter(peek) || peek=='<'){
					
					// Halt! Add it as a string literal.
					variableString=Input.Substring(Position-nameLength,nameLength);
					
					AppendText(textNode,"&"+variableString);
					return;
					
				}else{
					// Read off the char:
					Read();
					nameLength++;
				}
				
				peek=Peek();
				
			}
			
		}
		
		/// <summary>Calls Element.OnLexerCloseNode. Note that it's an instance method
		/// but it can be called without an instance when the DOM isn't balanced.
		/// For example, a balanced DOM will have a 'div' on the open element stack, and we want to handle its /div
		/// tag when it shows up. This would directly invoke close on that open element.
		/// If we're not balanced, it obtains SupportedTagMeta.CloseMethod and invokes it with a null instance.
		/// See SupportedTagMeta.CloseMethod for more.</summary>
		public bool CallCloseMethod(string tag,int mode){
			
			// Balanced?
			for(int i=OpenElements.Count-1;i>=0;i--){
				
				// Get it:
				Element stacked=OpenElements[i];
				
				if(stacked.Tag==tag){
					
					// Great - call it on that instance:
					return stacked.OnLexerCloseNode(this,mode);
					
				}
				
			}
			
			// Unexpected close tag.
			// Ideally this is rare 
			// but who knows what kind of crazy syntax is out there on the interwebs..
			
			MLNamespace ns=Namespace;
			
			if(tag!=null && tag.IndexOf(':')!=-1){
				
				// We have a namespace declaration in the tag.
				// e.g. svg:svg
				string[] pieces=tag.Split(':');
				
				// Get the namespace from a (global only) prefix:
				ns=MLNamespaces.GetPrefix(pieces[0]);
				
				// Update the tag:
				tag=pieces[1];
				
			}
			
			// Get the tag handler:
			SupportedTagMeta globalHandler=TagHandlers.Get(ns,tag);
			
			MethodInfo mInfo=globalHandler.CloseMethod;
			
			if(mInfo==null){
				return false;
			}
			
			// We'll need to instance a tag:
			object tagInstance=Activator.CreateInstance(globalHandler.TagType);
			
			// Run it now:
			return (bool)mInfo.Invoke(tagInstance,new object[]{this,mode});
			
		}
		
		/// <summary>Creates an element from the given namespace/ tag name.</summary>
		public Element CreateTag(string tag,bool callLoad){
			
			MLNamespace ns=Namespace;
			
			if(tag!=null && tag.IndexOf(':')!=-1){
				
				// We have a namespace declaration in the tag.
				// e.g. svg:svg
				string[] pieces=tag.Split(':');
				
				// Get the namespace from a (global only) prefix:
				var tagNamespace=MLNamespaces.GetPrefix(pieces[0]);
				
				if(tagNamespace != null){
					ns = tagNamespace;
				}
				
				// Update the tag:
				tag=pieces[1];
				
			}
			
			// Ok:
			Element el=TagHandlers.Create(ns,tag);
			el.document_=Document;
			
			// Change namespace if needed:
			Namespace=el.Namespace;
			
			if(callLoad){
				// Call the loaded method:
				el.OnTagLoaded();
			}
			
			return el;
			
		}
		
	}
	
}