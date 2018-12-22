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
using Css;


namespace Css.Spec{
	
	/// <summary>
	/// Represents <a-css-property> in the specification.
	/// </summary>
	
	public class Property : Spec.Value{
		
		/// <summary>The property itself.</summary>
		public CssProperty RawProperty;
		/// <summary>An object used to indicate that this property has been set.</summary>
		private CssPropertySetInfo SetInfo;
		
		
		public Property(CssProperty prop,string name){
			RawProperty=CssProperties.Get(name);
			
			if(RawProperty==null){
				throw new Exception("CSS Specification failure: Property '"+name+"' is required.");
			}
			
			CssCompositeProperty compProp=prop as CssCompositeProperty;
			
			if(compProp!=null){
				SetInfo=compProp.GetPropertySetInfo(RawProperty);
			}
			
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			// Check if it's inherit or initial:
			if(value is Css.Keywords.Initial || value is Css.Keywords.Inherit){
				
				// Apply it!
				size=1;
				
				if(SetInfo!=null){
					// Indicate that we've been set:
					SetInfo.Set=true;
				}
				
				styleBlock[RawProperty]=value;
				
				return true;
			}
			
			Css.Spec.Value spec=RawProperty.Specification;
			
			if(spec.OnReadValue(styleBlock,value,start,out size)){
				
				// Apply it!
				if(SetInfo!=null){
					// Indicate that we've been set:
					SetInfo.Set=true;
				}
				
				if(size==1){
					styleBlock[RawProperty]=value[start];
				}else{
					
					// Chop out a segment:
					Css.ValueSet set=new Css.ValueSet(new Css.Value[size]);
					
					for(int i=0;i<size;i++){
						set[i]=value[start+i];
					}
					
					// Apply:
					styleBlock[RawProperty]=set;
					
				}
				
				return true;
				
			}
			
			size=0;
			return false;
			
		}
		
	}
	
}