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
using Dom;


namespace Dom{
	
	/// <summary>
	/// The type of a HTML document.
	/// </summary>

	
	public class DocumentType:Node{
		
		/// <summary>
		/// The raw name.
		/// </summary>
		internal string name_="";
		
        /// <summary>
        /// Gets or sets the public ID of the document type.
        /// </summary>
        public string publicId="";
		
        /// <summary>
        /// Gets or sets the system ID of the document type.
        /// </summary>
        public string systemId="";
		
		/// <summary>
		/// True if this is a forced quirksmode doctype.
		/// </summary>
		public bool quirksMode;
		
		
		
		public DocumentType(string name){
			name_=name;
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// This is only ever valid in the Initial state:
			if(mode==HtmlTreeMode.Initial){
				
				// Append it:
				lexer.Document.appendChild(this);
				
				// Apply it as doctype:
				lexer.Document.doctype=this;
				
				// Switch to before html:
				lexer.CurrentMode=HtmlTreeMode.BeforeHtml;
				
			}
			
			return true;
		} 
		
		/// <summary>The name without the prefix.</summary>
		public override string localName{
			get{
				return name;
			}
		}
		
		/// <summary>The nodes full name, accounting for namespace.</summary>
		public string name{
			get{
				return name_;
			}
		}
		
		/// <summary>The nodes full name, accounting for namespace.</summary>
		public override string nodeName{
			get{
				return name_;
			}
		}
		
		/// <summary>The node type.</summary>
		public override ushort nodeType{
			get{
				return 10;
			}
		}
		
		/// <summary>The value.</summary>
		public override string nodeValue{
			get{
				return null;
			}
			set{}
		}
		
		/// <summary>The value.</summary>
		public override string textContent{
			get{
				return null;
			}
			set{}
		}
		
		/// <summary>
		/// Gets if the given doctype token represents a limited quirks mode state.
		/// </summary>
		public bool IsLimitedQuirks{
			get{
				if(publicId==null)
					return false;
				if (publicId.StartsWith("-//W3C//DTD XHTML 1.0 Frameset//", StringComparison.OrdinalIgnoreCase))
					return true;
				else if (publicId.StartsWith("-//W3C//DTD XHTML 1.0 Transitional//", StringComparison.OrdinalIgnoreCase))
					return true;
				if(systemId==null)
					return false;
				else if (systemId.StartsWith("-//W3C//DTD HTML 4.01 Frameset//", StringComparison.OrdinalIgnoreCase))
					return true;
				else if (systemId.StartsWith("-//W3C//DTD HTML 4.01 Transitional//", StringComparison.OrdinalIgnoreCase))
					return true;

				return false;
			}
		}

		/// <summary>
		/// Gets if the given doctype token represents a full quirks mode state.
		/// </summary>
		public bool IsFullQuirks{
			get{
				if (quirksMode)
					return true;
				else if(name_!="html")
					return true;
				else if(string.IsNullOrEmpty(publicId))
					return false;
				else if (publicId.StartsWith("-//W3C//DTD HTML 4.01 Transitional//EN", StringComparison.OrdinalIgnoreCase))
					return false;
				else if (publicId.StartsWith("-//W3C//DTD HTML 4.01//EN", StringComparison.OrdinalIgnoreCase))
					return false;
				
				return true;
			}
		}
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public override bool isEqualNode(Node other){
			if(other==this){
				return true;
			}
			
			DocumentType t=other as DocumentType;
			
			return t!=null && t.publicId==publicId && t.systemId==systemId && t.name_==name_;
		}
		
		public override string ToString(){
			return name;
		}
		
		/// <summary>reads a "quoted" or 'quoted' string.</summary>
		private string ReadQuotedString(HtmlLexer lexer){
			PropertyTextReader.ReadString(lexer,lexer.Builder);
			
			string str=lexer.Builder.ToString();
			lexer.Builder.Length=0;
			return str;
		}
		
		/// <summary>
		/// See 8.2.4.56 After DOCTYPE public keyword state
		/// </summary>
		internal void ParsePublic(HtmlLexer lexer){
			
			PropertyTextReader.SkipSpaces(lexer);
			
			char c = lexer.Peek();
			
			if(c == '"' || c == '\''){
				
				publicId=ReadQuotedString(lexer);
				PublicIdentifierAfter(lexer);
				
			}else if (c == '>'){
				lexer.Position++;
				lexer.State = HtmlParseMode.PCData;
				quirksMode = true;
			}else if (c == '\0'){
				quirksMode = true;
			}else{
				lexer.Position++;
				quirksMode = true;
				ParseBroken(lexer);
			}
			
		}
		
		/// <summary>
		/// See 8.2.4.60 After DOCTYPE public identifier state
		/// </summary>
		private void PublicIdentifierAfter(HtmlLexer lexer){
			
			char c = lexer.Peek();

			if (HtmlLexer.IsSpaceCharacter(c)){
				DoctypeBetween(lexer);
			}else if(c == '>'){
				lexer.Position++;
				lexer.State = HtmlParseMode.PCData;
			}else if(c == '"' || c=='\''){
				
				systemId=ReadQuotedString(lexer);
				SystemIdentifierAfter(lexer);
				
			}else if(c == '\0'){
				quirksMode = true;
			}else{
				quirksMode = true;
				ParseBroken(lexer);
			}
			
		}

		/// <summary>
		/// See 8.2.4.61 Between DOCTYPE public and system identifiers state
		/// </summary>
		private void DoctypeBetween(HtmlLexer lexer){
			
			PropertyTextReader.SkipSpaces(lexer);
			
			char c=lexer.Peek();
			
			if (c == '>'){
				lexer.Position++;
				lexer.State = HtmlParseMode.PCData;
			}else if(c == '"' || c=='\''){
				
				systemId=ReadQuotedString(lexer);
				SystemIdentifierAfter(lexer);
				
			}else if (c == '\0'){
				quirksMode = true;
			}else{
				quirksMode = true;
				ParseBroken(lexer);
			}
			
		}

		/// <summary>
		/// See 8.2.4.62 After DOCTYPE system keyword state
		/// </summary>
		public void ParseSystem(HtmlLexer lexer){
			
			PropertyTextReader.SkipSpaces(lexer);
			
			char c = lexer.Peek();
			
			if(c == '"' || c == '\''){
				
				systemId=ReadQuotedString(lexer);
				SystemIdentifierAfter(lexer);
				
			}else if (c == '>'){
				lexer.Read();
				lexer.State = HtmlParseMode.PCData;
				quirksMode = true;
			}else if (c == '\0'){
				quirksMode = true;
			}else{
				lexer.Read();
				quirksMode = true;
				ParseBroken(lexer);
			}
			
		}
		
		/// <summary>
		/// See 8.2.4.66 After DOCTYPE system identifier state
		/// </summary>
		private void SystemIdentifierAfter(HtmlLexer lexer){
			
			PropertyTextReader.SkipSpaces(lexer);
			
			char c = lexer.Peek();
			
			if(c=='>'){
				lexer.Position++;
				lexer.State = HtmlParseMode.PCData;
			}else if(c=='\0'){
				quirksMode=true;
			}else{
				
				ParseBroken(lexer);
				
			}
			
		}
		
		public override void ToString(System.Text.StringBuilder result){
			
			result.Append("<!doctype");
			
			if(name!=null){
				result.Append(' ');
				result.Append(name);
			}
			
			bool pub=!string.IsNullOrEmpty(publicId);
			bool sys=!string.IsNullOrEmpty(systemId);
			
			if(pub){
				result.Append(" PUBLIC ");
				result.Append("\""+publicId+"\"");
				
				if(sys){
					result.Append(" \""+systemId+"\"");
				}
				
			}else if(sys){
				result.Append(" SYSTEM ");
				result.Append(" \""+systemId+"\"");
			}
			
			result.Append(">");
		}
		
		/// <summary>
		/// See 8.2.4.67 Bogus DOCTYPE state
		/// </summary>
		public void ParseBroken(HtmlLexer lexer){
			
			while (true){
				
				char c=lexer.Read();
				
				if(c=='>'){
					lexer.State = HtmlParseMode.PCData;
					return;
				}else if(c=='\0'){
					return;
				}
				
			}
			
		}
		
	}
	
}
	