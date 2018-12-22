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


namespace Svg{
	
	/// <summary>A number optionally associated to a particular element.</summary>
	public class SVGNumber:ISVGListable{
		
		private bool IsReadOnly;
		private SVGSerializable List;
		private float value_;
		
		
		public SVGNumber(bool readOnly,float v){
			IsReadOnly=readOnly;
			value_=v;
		}
		
		/// <summary>The internal value.</summary>
		public float value{
			get{
				return value_;
			}
			set{
				if(IsReadOnly){
					throw new Dom.DOMException(Dom.DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				value_=value;
				if(List!=null){
					List.Reserialize();
				}
			}
		}
		
		/// <summary>Detaches this value from a serializable list.</summary>
		public void Detach(){
			List=null;
		}
		
		/// <summary>Attaches this value to a serializable list.</summary>
		public void Attach(SVGSerializable list){
			List=list;
		}
		
		/// <summary>Used when serializing this value.</summary>
		public void Serialize(System.Text.StringBuilder sb){
			sb.Append(value);
		}
		
	}

}