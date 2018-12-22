using System;
using System.Collections;
using System.Collections.Generic;
using Jint.Native;

namespace Dom{
	
	/// <summary>An object which can hold extra custom properties. Virtually every DOM object inherits from this.</summary>
	public class ExpandableObject{
		
		/// <summary>The set of properties on this object.</summary>
		private Dictionary<string, object> expansionProperties;
		
		#if !LEGACY_DOM && !LEGACY_DOM_UPGRADE
		/// <summary>Get/ set the named property to the given value.</summary>
		public object this[string name]{
			get{
				object value;
				if(expansionProperties != null && expansionProperties.TryGetValue(name, out value)){
					return value;
				}
				return Undefined.Instance;
			}
			set{
				if((value as JsValue) == Undefined.Instance){
					// Remove
					if(expansionProperties != null){
						expansionProperties.Remove(name);
					}
					return;
				}
				if(expansionProperties == null){
					expansionProperties = new Dictionary<string, object>();
				}
				expansionProperties[name]=value;
			}
		}
		#endif
		
	}
	
}
