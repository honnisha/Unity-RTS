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


namespace Svg{
	
	/// <summary>
	/// a 2D or 3D DOM point, used primarily by SVG.
	/// </summary>
	public class DOMPoint : ISVGListable{
		
		private double x_;
		private double y_;
		private double z_;
		private double w_;
		private bool IsReadOnly;
		private SVGSerializable List;
		
		
		public double x{
			get{
				return x_;
			}
			set{
				if(IsReadOnly){
					throw new Dom.DOMException(Dom.DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				x_=value;
				
				if(List!=null){
					List.Reserialize();
				}
			}
		}
		
		public double y{
			get{
				return y_;
			}
			set{
				if(IsReadOnly){
					throw new Dom.DOMException(Dom.DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				y_=value;
				
				if(List!=null){
					List.Reserialize();
				}
			}
		}
		
		public double z{
			get{
				return z_;
			}
			set{
				if(IsReadOnly){
					throw new Dom.DOMException(Dom.DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				z_=value;
				
				if(List!=null){
					List.Reserialize();
				}
			}
		}
		
		public double w{
			get{
				return w_;
			}
			set{
				if(IsReadOnly){
					throw new Dom.DOMException(Dom.DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				w_=value;
				
				if(List!=null){
					List.Reserialize();
				}
			}
		}
		
		
		public DOMPoint(bool readOnly,double x,double y){
			IsReadOnly=readOnly;
			x_=x;
			y_=y;
			w_=1; // w defaults to 1
		}
		
		public DOMPoint(double x,double y,double z,double w){
			x_=x;
			y_=y;
			z_=z;
			w_=w;
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
			sb.Append(x);
			sb.Append(',');
			sb.Append(y);
		}
		
	}
	
}