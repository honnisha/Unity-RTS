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


namespace Dom{
	
	public class CharacterData:Node{
		
		/// <summary>The raw character data.</summary>
		internal string characterData_="";
		
		public virtual string data{
			get{
				return characterData_;
			}
			set{
				characterData_=value;
			}
		}
		
		public int length{
			get{
				return characterData_.Length;
			}
		}
		
		/// <summary>Gets a substring from this text at the given position.</summary>
		public string substringData(int offset,int count){
			// Basic substring:
			return characterData_.Substring(offset,count);
		}
		
		/// <summary>Replaces the given block of data.</summary>
		public void replaceData(int offset,int count,string with){
			// Get text:
			string text=characterData_;
			
			// Get before/after:
			string before=text.Substring(0,offset-1);
			string after=text.Substring(offset+count);
			
			// Apply:
			data=before+with+after;
			
		}
		
		/// <summary>Deletes the given block of data.</summary>
		public void deleteData(int offset,int count){
			replaceData(offset,count,"");
		}
		
		/// <summary>Clones this node.</summary>
		public override Node cloneNode(bool deep){
			
			Node nd=base.cloneNode(deep);
			
			TextNode ele=(nd as TextNode);
			
			ele.data=characterData_;
			
			return ele;
			
		}
		
		/// <summary>True if this is all 'whitespace' characters as defined by the HTML spec (includes newlines).</summary>
		internal bool IsSpaces{
			get{
				
				if(characterData_==null){
					return true;
				}
				
				for(int i=characterData_.Length-1;i>=0;i--){
					
					if(!HtmlLexer.IsSpaceCharacter(characterData_[i])){
						return false;
					}
					
				}
				
				return true;
				
			}
		}
		
		/// <summary>Appends the given data to this text element. Must not contain HTML.</summary>
		public void appendData(string newData){
			data+=newData;
		}
		
		/// <summary>True if the content is only a whitespace.</summary>
		public bool isElementContentWhitespace{
			get{
				return data==" ";
			}
		}
		
		/// <summary>The text of all Text nodes logically adjacent to this node</summary>
		public string wholeText{
			get{
				
				string text=data;
				
				// Get sibling:
				TextNode sibling=nextSibling as TextNode;
				
				while(sibling!=null){
					
					// Append:
					text+=sibling.data;
					
					// Next sibling:
					sibling=sibling.nextSibling as TextNode;
					
				}
				
				return text;
				
			}
		}
		
		/// <summary>Replaces whole text data.</summary>
		public void replaceWholeText(string with){
			data=with;
		}
		
		/// <summary>The value of this node.</summary>
		public override string nodeValue{
			get{
				return characterData_;
			}
			set{}
		}
		
		/// <summary>Gets or sets the text content of this element (i.e. the content without any html.).
		/// Setting this is good for preventing any html injection as it will be taken literally.</summary>
		public override string textContent{
			get{
				return characterData_;
			}
			set{
				data=value;
			}
		}
		
		public override void ToString(System.Text.StringBuilder builder){
			builder.Append(characterData_);
		}
		
	}
	
	public class TextNode:CharacterData{
		
		// localName is null
		
		/// <summary>Splits the text into two nodes at the given offset.</summary>
		public void splitText(int offset){
			
			string text=characterData_;
			data=text.Substring(0,offset);
			
			// Get the rest:
			text=text.Substring(offset);
			
			// Create a new text node:
			TextNode ele=Namespace.CreateTextNode(document);
			
			// Write text to it:
			ele.textContent=text;
			
			// Insert immediately after "me":
			parentNode.insertBefore(ele,nextSibling);
			
		}
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public override bool isEqualNode(Node other){
			if(other==this){
				return true;
			}
			
			return other is TextNode && other.textContent==textContent;
		}
		
		/// <summary>The name for this type of node.</summary>
		public override string nodeName{
			get{
				return "#text";
			}
		}
		
		/// <summary>The type of element that this is.</summary>
		public override ushort nodeType{
			get{
				return 3;
			}
		}
		
		public override string ToString(){
			return "[object Text]";
		}
		
		/// <summary>The tree modes where a text node will get appended to CurrentNode.</summary>
		private const int AppendAdd=HtmlTreeMode.InSelect
		| HtmlTreeMode.InSelectInTable
		| HtmlTreeMode.Text
		| HtmlTreeMode.InTemplate;
		
		/// <summary>The tree modes where a text node will get appended to CurrentNode, but only if IsSpaces is true.</summary>
		private const int AppendSpacesAdd=HtmlTreeMode.InColumnGroup
		| HtmlTreeMode.AfterFrameset
		| HtmlTreeMode.AfterBody
		| HtmlTreeMode.AfterHead
		| HtmlTreeMode.InHead
		| HtmlTreeMode.InHeadNoScript
		| HtmlTreeMode.AfterAfterBody
		| HtmlTreeMode.AfterAfterFrameset
		| HtmlTreeMode.InFrameset;
		
		/// <summary>The tree modes where a text node will get appended to CurrentNode, but only if IsSpaces is true.</summary>
		private const int IgnoreSpaces=HtmlTreeMode.BeforeHead
		| HtmlTreeMode.BeforeHtml
		| HtmlTreeMode.Initial;
		
		/// <summary>Tree modes which act like 'in body'.</summary>
		private const int BodyMode=HtmlTreeMode.InBody
		| HtmlTreeMode.InCaption
		| HtmlTreeMode.InCell;
		
		/// <summary>Tree modes which act like 'in table'.</summary>
		private const int TableMode=HtmlTreeMode.InTable
		| HtmlTreeMode.InTableBody
		| HtmlTreeMode.InRow;
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if((mode & BodyMode)!=0){
				
				lexer.ReconstructFormatting();
				
				// Append it:
				lexer.CurrentNode.appendChild(this);
				
				if(!IsSpaces){
					// set frames to not ok:
					lexer.FramesetOk=false;
				}
			
			}else if((mode & AppendAdd)!=0){
				
				// Append it:
				lexer.CurrentNode.appendChild(this);
				
			}else if((mode & TableMode)!=0 && lexer.CurrentElement.IsTableStructure){
				
				// Check if any of the chars are spaces:
				if(IsSpaces){
					
					// Insert:
					lexer.CurrentElement.appendChild(this);
					
				}else{
					
					// Reprocess as in table else:
					lexer.InTableElse(this,null);
					
				}
				
			}else if(((mode & AppendSpacesAdd)!=0) && IsSpaces){
				
				// Append it:
				lexer.CurrentNode.appendChild(this);
				
			}else if(((mode & IgnoreSpaces)!=0) && IsSpaces){
				
				// Ignore spaces.
				
			}else{
				return false;
			}
			
			return true;
			
		}
		
	}
	
}