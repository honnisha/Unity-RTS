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
	/// The name(s) of the tag.
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class,Inherited=false)]
	public class TagName : Attribute{
		
		/// <summary>One or more names separated by commas. Always lowercase with no spaces.</summary>
		public string Tags;
		
		public TagName(string value){
			Tags=value;
		}
		
	}
	
	/// <summary>
	/// Used to indicate which namespace a tag belongs to.
	/// Typically placed on some base class of all tags.
	/// </summary>
	[AttributeUsageAttribute(AttributeTargets.Class,Inherited=true)]
	public class XmlNamespace : Attribute{
		
		/// <summary>The namespace. One global object per namespace.</summary>
		public MLNamespace Namespace;
		
		public XmlNamespace(string name,string prefix,string mime,Type docType){
			Namespace=MLNamespaces.Get(name,prefix,mime);
			
			if(docType!=null){
				Namespace.DocumentType=docType;
			}
			
		}
		
		public XmlNamespace(string name,string prefix,string mime,Type docType,string foreign){
			Namespace=MLNamespaces.Get(name,prefix,mime);
			
			if(docType!=null){
				Namespace.DocumentType=docType;
			}
			
			if(foreign!=null){
				// Foreign elements (such as mathml's 'math' in html) are available.
				
				// Set:
				Namespace.ForeignNames=foreign;
				
			}
			
		}
		
	}
	
}