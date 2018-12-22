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
using Css.Spec;
using System.Collections;
using System.Collections.Generic;


namespace Css{
	
	/// <summary>
	/// A CSS composite property. These are properties like background; one which is simply
	/// a shortform way of setting more than one property at the same time.
	/// Importantly, they are considered aliases. 
	/// They do not actually enter the CSS style set because of this.
	/// </summary>
	
	public class CssCompositeProperty:CssPropertyAlias{
		
		/// <summary>The lookup of all properties which get set by this one. Computed when the spec is obtained.</summary>
		public Dictionary<CssProperty,CssPropertySetInfo> AllProperties=new Dictionary<CssProperty,CssPropertySetInfo>();
		
		
		public CssCompositeProperty():base(){
		}
		
		/// <summary>In order to know when certain properties have been set and which have not, each property that a composite property updates
		/// has a 'set info' object. When reading a composite value, all the set info values are checked to see which ones were
		/// not set. It then proceeds to update those to their initial value.</summary>
		public CssPropertySetInfo GetPropertySetInfo(CssProperty prop){
			
			CssPropertySetInfo result;
			if(!AllProperties.TryGetValue(prop,out result)){
				
				result=new CssPropertySetInfo();
				AllProperties[prop]=result;
				
			}
			
			return result;
			
		}
		
		public override Css.Value GetValue(Style styleBlock){
			
			// Re-compose the value. Optional.
			return null;
			
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			// Special case for initial and inherit - we use inherit or initial for all it's internal properties:
			if(value is Css.Keywords.Inherit){
				
				// Foreach property..
				foreach(KeyValuePair<CssProperty,CssPropertySetInfo> kvp in AllProperties){
					
					// Set it to its initial/inherited value:
					styleBlock[kvp.Key]=new Css.Keywords.Inherit(kvp.Key,value.Specifity);
					
				}
				
				return;
				
			}else if(value is Css.Keywords.Initial || value==null){
				
				// Foreach property..
				foreach(KeyValuePair<CssProperty,CssPropertySetInfo> kvp in AllProperties){
					
					// Set it to its initial/inherited value:
					styleBlock[kvp.Key]=value == null ? new Css.Keywords.Initial(kvp.Key) : new Css.Keywords.Initial(kvp.Key,value.Specifity);
					
				}
				
				return;
				
			}
			
			// Get the specification:
			Spec.Value spec=Specification;
			
			if(spec==null){
				return;
			}
			
			// Load the value now:
			int size;
			spec.OnReadValue(styleBlock,value,0,out size);
			
			// Foreach property..
			foreach(KeyValuePair<CssProperty,CssPropertySetInfo> kvp in AllProperties){
				
				CssPropertySetInfo info=kvp.Value;
				
				if(info.Set){
					// Clear:
					info.Set=false;
				}else{
					
					// Set it to its initial value:
					styleBlock[kvp.Key]=new Css.Keywords.Initial(kvp.Key,value.Specifity);
					
				}
				
			}
			
		}
		
	}
	
}