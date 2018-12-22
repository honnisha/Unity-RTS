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
using System.Reflection;
using System.Collections.Generic;


namespace Dom{
	
	/// <summary>
	/// A global lookup of tag text to handler.
	/// TagHandlers are instanced globally and mapped to the tags they accept.
	/// When a tag is found, it is then instanced. One instance of a tag is created per element.
	/// </summary>
	
	public static class TagHandlers{
		
		/// <summary>Gets all available tag handlers.
		/// Note that interally lookups go through MLNamespace.Tags instead.</summary>
		public static Dictionary<string,Type> GetAll(){
			
			// Create:
			Dictionary<string,Type> set=new Dictionary<string,Type>();
			
			// For each namespace..
			foreach(KeyValuePair<string,MLNamespace> kvp in MLNamespaces.All){
				
				MLNamespace ns=kvp.Value;
				
				// For each type..
				foreach(KeyValuePair<string,SupportedTagMeta> t in ns.Tags){
					
					set[ns.Prefix+":"+t.Key]=t.Value.TagType;
					
				}
				
			}
			
			return set;
			
		}
		
		/// <summary>Clears all tag handlers.</summary>
		public static void Clear(){
			
			Dom.Start.Reset();
			
		}
		
		/// <summary>Adds an element type to the set.
		/// This is generally done automatically, but you can also add it manually if you wish.</summary>
		/// <param name="elementType">The type to add.</param>
		/// <returns>True if adding it was successful.</returns>
		public static bool Add(Type elementType){
			
			if( elementType == typeof(Element) ){
				return false;
			}
			
			// Get the name attribute from it (don't inherit):
			#if NETFX_CORE
			TagName tagName=elementType.GetTypeInfo().GetCustomAttribute(typeof(TagName),false) as TagName;
			#else
			TagName tagName=Attribute.GetCustomAttribute(elementType,typeof(TagName),false) as TagName;
			#endif
			
			if(tagName==null){
				// Nope!
				return false;
			}
			
			string tags=tagName.Tags;
			
			if(string.IsNullOrEmpty(tags)){
				return false;
			}
			
			// get the namespace name:
			#if NETFX_CORE
			XmlNamespace xmlns=elementType.GetTypeInfo().GetCustomAttribute(typeof(XmlNamespace),true) as XmlNamespace;
			#else
			XmlNamespace xmlns=Attribute.GetCustomAttribute(elementType,typeof(XmlNamespace),true) as XmlNamespace;
			#endif
			
			if(xmlns==null){
				return false;
			}
			
			// Add it:
			xmlns.Namespace.AddTag(tags,elementType);
			
			return true;
		}
		
		/// <summary>
		/// Gets the metadata for the given tag.
		/// </summary>
		public static SupportedTagMeta Get(MLNamespace ns,string tag){
			
			SupportedTagMeta globalHandler;
			
			if(!ns.Tags.TryGetValue(tag,out globalHandler)){
			
				// Tag wasn't found in the given namespace.
				
				// Check if the namespace can help out by providing some other namespace:
				MLNamespace newNamespace=ns.GetNamespace(tag);
				
				if(newNamespace==null){
					
					// Use default:
					globalHandler=ns.Default;
					
				}else{
					
					ns=newNamespace;
					
					// Try to get the handler:
					if(!ns.Tags.TryGetValue(tag,out globalHandler)){
						
						// Use default:
						globalHandler=ns.Default;
						
					}
					
				}
				
			}
			
			return globalHandler;
		}
		
		/// <summary>Attempts to find the tag with the given name.
		/// If it's not found, a default tag which is known to exist can be returned instead.
		/// The handler for the found tag is then instanced and the instance is returned.
		/// For example, tag "h1" with a default of "span".</summary>
		/// <param name="ns">The namespace the tag is in.</param>
		/// <param name="tag">The tag to look for.</param>
		/// <param name="defaultTag">If the given tag is not found, this is used instead.</param>
		/// <returns>An instance of the tag handler for the tag. Throws an error if tag or defaultTag are not found.</returns>
		public static Element Create(MLNamespace ns,string tag){
			
			SupportedTagMeta globalHandler;
			
			if(!ns.Tags.TryGetValue(tag,out globalHandler)){
			
				// Tag wasn't found in the given namespace.
				
				// Check if the namespace can help out by providing some other namespace:
				MLNamespace newNamespace=ns.GetNamespace(tag);
				
				if(newNamespace==null){
					
					// Use default:
					globalHandler=ns.Default;
					
				}else{
					
					ns=newNamespace;
					
					// Try to get the handler:
					if(!ns.Tags.TryGetValue(tag,out globalHandler)){
						
						// Use default:
						globalHandler=ns.Default;
						
					}
					
				}
				
			}
			
			// Instance it now:
			Element result=Activator.CreateInstance(globalHandler.TagType) as Element;
			result.Namespace=ns;
			result.Tag=tag;
			
			// Ok!
			return result;
		}
		
	}
	
}