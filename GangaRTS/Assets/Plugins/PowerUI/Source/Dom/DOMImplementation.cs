using System;
using System.Collections;
using System.Collections.Generic;
using Dom;


namespace Dom{

	/// <summary>
	/// Provides a number of methods for performing operations that are
	/// independent of any particular instance of the DOM.
	/// </summary>
	public partial class DOMImplementation{
		
		private static Dictionary<string, string[]> features;
		
		/// <summary>Creates the feature set.</summary>
		private static void SetupFeatures(){
			
			features = new Dictionary<string, string[]>();
			
			features["XML"]=new string[]{"1.0","2.0"};
			features["HTML"]=new string[]{"1.0","2.0"};
			features["Core"]=new string[]{"2.0"};
			features["Views"]=new string[]{"2.0"};
			features["StyleSheets"]=new string[]{"2.0"};
			features["CSS"]=new string[]{"2.0"};
			features["CSS2"]=new string[]{"2.0"};
			features["Traversal"]=new string[]{"2.0"};
			features["Events"]=new string[]{"2.0"};
			features["UIEvents"]=new string[]{"2.0"};
			features["HTMLEvents"]=new string[]{"2.0"};
			features["Range"]=new string[]{"2.0"};
			features["MutationEvents"]=new string[]{"2.0"};
			
		}
		
		readonly Document _owner;
		
		public DOMImplementation(Document owner){
			_owner = owner;
			
		}
		
		public DocumentType createDocumentType(string qualifiedName, string publicId, string systemId){
			
			if (qualifiedName == null)
				throw new ArgumentNullException("qualifiedName");
			
			DocumentType type=new DocumentType(qualifiedName);
			type.publicId=publicId;
			type.systemId=systemId;
			
			return type;
		}
		
		/// <summary>Creates a new document and sets it to the given namespace.</summary>
		public Document createDocument(string namespaceUri){
			return createDocument(namespaceUri,null,null);
		}
		
		/// <summary>Creates a new document and sets it to the given namespace.</summary>
		public Document createDocument(string namespaceUri, string qualifiedName){
			return createDocument(namespaceUri,qualifiedName,null);
		}
		
		/// <summary>Creates a new document and sets it to the given namespace.</summary>
		public Document createDocument(string namespaceUri, string qualifiedName, DocumentType doctype){
			
			MLNamespace ns=null;
			
			// Get the namespace if there is one:
			if(!string.IsNullOrEmpty(namespaceUri)){
				
				ns=MLNamespaces.Get(namespaceUri);
				
			}
			
			// Create the document:
			Document document=(ns==null)? new Document() : ns.CreateDocument();
			
			// Add the doctype:
			if (doctype!=null){
				document.appendChild(doctype);
			}
			
			if(!string.IsNullOrEmpty(qualifiedName)){
				
				// Create the element:
				Element element = document.createElement(qualifiedName);
				
				if (element != null){
					document.appendChild(element);
				}
				
			}

			// Apply the base URI:
			document.basepath = _owner.basepath;
			
			return document;
		}
		
		/// <summary>Is the named feature supported?</summary>
		public bool hasFeature(string feature){
			return hasFeature(feature,null);
		}
		
		/// <summary>Is the named feature supported?</summary>
		public bool hasFeature(string feature, string version){
			
			if (feature == null)
				throw new ArgumentNullException("feature");
			
			if(feature.StartsWith("+")){
				// Ignore the prepended plus:
				feature=feature.Substring(1);
			}
			
			if(features==null){
				SetupFeatures();
			}
			
			string[] versions;
			if(features.TryGetValue(feature, out versions)){
				
				if(version==null){
					return true;
				}
				
				for(int i=0;i<versions.Length;i++){
					if(versions[i]==version){
						return true;
					}
				}
				
			}

			return false;
		}
		
	}
	
}
