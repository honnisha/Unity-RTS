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
	/// Namespaces allow multiple XML elements using the same tag name to be present in a single document.
	/// https://www.w3.org/TR/2006/REC-xml-names-20060816/
	/// </summary>
	
	public partial class MLNamespace{
		
		/// <summary>The unique name of the namespace. Typically these are URI's.</summary>
		public string Name;
		/// <summary>The prefix applied e.g. "html" resulting in "&gt;html:a ..&lt;".</summary>
		public string Prefix;
		/// <summary>The mimetype for this namespace. E.g. "text/html".</summary>
		public string MimeType;
		/// <summary>The type of documents created in this namespace.</summary>
		public Type DocumentType;
		/// <summary>The default node type.</summary>
		public SupportedTagMeta Default;
		/// <summary>The text node type.</summary>
		public Type TextNode=typeof(TextNode);
		/// <summary>The comment node type.</summary>
		public Type CommentNode=typeof(Comment);
		/// <summary>The lang node type.</summary>
		public Type LangNode;
		/// <summary>Of the form e.g. 'svg:svg,mml:math'. Allows common foreign tags from another namespace to
		/// be included without needing to declare their xmlns.</summary>
		public string ForeignNames;
		/// <summary>Loaded version of ForeignNames.</summary>
		public Dictionary<string,MLNamespace> Foreigners;
		/// <summary>The tag lookup. Matches tag text (e.g. "div") to the Type that should be instanced.</summary>
		public Dictionary<string,SupportedTagMeta> Tags=new Dictionary<string,SupportedTagMeta>();
		
		
		public MLNamespace(string name,string prefix,string mime){
			Name=name;
			Prefix=prefix;
			MimeType=mime;
		}
		
		/// <summary>Loads up the foreigners set.</summary>
		private void LoadForeigners(){
			
			Foreigners=new Dictionary<string,MLNamespace>();
			
			if(ForeignNames==null){
				return;
			}
			
			string[] tags=ForeignNames.Split(',');
			
			for(int i=0;i<tags.Length;i++){
				
				// FQ with it's xmlns, e.g. 'svg:svg' or 'mml:math'
				string fullyQualifiedTag=tags[i];
				
				string[] tagParts=fullyQualifiedTag.Split(':');
				
				// Get the namespace from a (global only) prefix:
				MLNamespace ns=MLNamespaces.GetPrefix(tagParts[0]);
				
				// Add to set:
				Foreigners[tagParts[1]]=ns;
				
			}
			
		}
		
		/// <summary>Adds a tag to this namespace. E.g. declaring 'div' in 'xhtml'.</summary>
		internal void AddTag(string tags,Type elementType){
			
			int commaIndex=tags.IndexOf(',');
			
			// Create the tag meta:
			SupportedTagMeta st=new SupportedTagMeta(elementType);
			
			if(commaIndex==-1){
				
				// Special tag?
				if(tags=="TextNode"){
					
					TextNode=elementType;
					
				}else if(tags=="CommentNode"){
					
					CommentNode=elementType;
					
				}else if(tags=="LangNode"){
					
					LangNode=elementType;
					
				}else if(tags=="Default"){
					
					Default=st;
					
				}
				
				// Always add - use namespaces to override a tag:
				Tags[tags]=st;
				
			}else{
				
				// Get all the tags:
				string[] tagSet=tags.Split(',');
				
				for(int i=0;i<tagSet.Length;i++){
					
					// Grab the tag name:
					string tag=tagSet[i];
					
					// Always add - use namespaces to override a tag:
					Tags[tag]=st;
					
				}
				
			}
			
		}
		
		/// <summary>Creates a document from this namespace.</summary>
		public Document CreateDocument(){
			
			if(DocumentType==null){
				return null;
			}
			
			// Create it:
			Document doc=Activator.CreateInstance(DocumentType) as Document;
			doc.Namespace=this;
			
			return doc;
		}
		
		/// <summary>Creates a new text node relative to this namespace.</summary>
		public TextNode CreateTextNode(Document document){
			TextNode node=Activator.CreateInstance(TextNode) as TextNode;
			node.document_=document;
			node.Namespace=this;
			return node;
		}
		
		/// <summary>Creates a new comment node relative to this namespace.</summary>
		public Comment CreateCommentNode(Document document){
			Comment	node=Activator.CreateInstance(CommentNode) as Comment;
			node.document_=document;
			node.Namespace=this;
			return node;
		}
		
		/// <summary>Creates a new lang node relative to this namespace.</summary>
		public Node CreateLangNode(Document document){
			
			if(LangNode==null){
				// Namespace doesn't support these.
				return null;
			}
			
			Node node=Activator.CreateInstance(LangNode) as Node;
			node.document_=document;
			node.Namespace=this;
			return node;
		}
		
		/// <summary>Gets a namespace for the given tag. This is used when the tag isn't found in "this" namespace.</summary>
		public MLNamespace GetNamespace(string tag){
			
			if(Foreigners==null){
				LoadForeigners();
			}
			
			MLNamespace ns;
			Foreigners.TryGetValue(tag,out ns);
			
			return ns;
			
		}
		
	}
	
}