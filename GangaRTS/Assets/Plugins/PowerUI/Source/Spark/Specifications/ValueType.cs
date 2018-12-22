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
using System.Reflection;


namespace Css.Spec{
	
	/// <summary>
	/// Represents e.g. <number> in the specification.
	/// </summary>
	
	public class ValueType : Spec.Value{
		
		/// <summary>The value type. The value object inherits from this.</summary>
		public Type RawType;
		/// <summary>General CSS value type.</summary>
		public Css.ValueType Type;
		
		
		public ValueType(Type type){
			RawType=type;
		}
		
		public ValueType(Css.ValueType type){
			Type=type;
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			if(value!=null){
				
				// Get a quick ref to the actual value to check:
				value=value[start];
				
				#if NETFX_CORE
				if(
					(RawType!=null && RawType.GetTypeInfo().IsAssignableFrom(value.GetType().GetTypeInfo())) ||
					value.Type==Type
				){
					size=1;
					return true;
				}
				#else
				if(
					(RawType!=null && RawType.IsAssignableFrom(value.GetType())) ||
					value.Type==Type
				){
					size=1;
					return true;
				}
				#endif
				
			}
			
			size=0;
			return false;
			
		}
		
	}
	
}