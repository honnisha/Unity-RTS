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
	public class SVGLength:ISVGListable{
		
		/// <summary>Gets the SVG length type for a particular CSS value.</summary>
		public static ushort GetLengthType(Css.Value value){
			
			if(value==null || !(value is Css.Units.DecimalUnit)){
				return SVG_LENGTHTYPE_UNKNOWN;
			}
			
			// Get the type:
			Type t=value.GetType();
			
			if(t==typeof(Css.Units.PercentUnit)){
				return SVG_LENGTHTYPE_PERCENTAGE;
			}else if(t==typeof(Css.Units.EmUnit)){
				return SVG_LENGTHTYPE_EMS;
			}else if(t==typeof(Css.Units.ExUnit)){
				return SVG_LENGTHTYPE_EXS;
			}else if(t==typeof(Css.Units.PxUnit)){
				return SVG_LENGTHTYPE_PX;
			}else if(t==typeof(Css.Units.CmUnit)){
				return SVG_LENGTHTYPE_CM;
			}else if(t==typeof(Css.Units.MmUnit)){
				return SVG_LENGTHTYPE_MM;
			}else if(t==typeof(Css.Units.InchUnit)){
				return SVG_LENGTHTYPE_IN;
			}else if(t==typeof(Css.Units.PointUnit)){
				return SVG_LENGTHTYPE_PT;
			}else if(t==typeof(Css.Units.PcUnit)){
				return SVG_LENGTHTYPE_PC;
			}
			
			// Otherwise it's just a number:
			return SVG_LENGTHTYPE_NUMBER;
		}
		
		/// <summary>Length type constants.</summary>
		public const ushort SVG_LENGTHTYPE_UNKNOWN=0;
		public const ushort SVG_LENGTHTYPE_NUMBER=1;
		public const ushort SVG_LENGTHTYPE_PERCENTAGE=2;
		public const ushort SVG_LENGTHTYPE_EMS=3;
		public const ushort SVG_LENGTHTYPE_EXS=4;
		public const ushort SVG_LENGTHTYPE_PX=5;
		public const ushort SVG_LENGTHTYPE_CM=6;
		public const ushort SVG_LENGTHTYPE_MM=7;
		public const ushort SVG_LENGTHTYPE_IN=8;
		public const ushort SVG_LENGTHTYPE_PT=9;
		public const ushort SVG_LENGTHTYPE_PC=10;
		
		private bool IsReadOnly;
		private SVGSerializable List;
		private Css.Value rawValue;
		
		
		public SVGLength(bool readOnly,SVGSerializable list,Css.Value originalValue){
			IsReadOnly=readOnly;
			List=list;
			rawValue=originalValue;
		}
		
		/// <summary>The resolved value (in CSS pixels).</summary>
		public float value{
			get{
				// Get it as a renderable node so we can resolve the value:
				Css.IRenderableNode irn=(List.HostNode as Css.IRenderableNode);
				
				if(irn==null){
					return 0f;
				}
				
				// Resolve it now, using a direction aware property:
				return rawValue.GetDecimal(irn.RenderData,null);
			}
			set{
				if(IsReadOnly){
					throw new Dom.DOMException(Dom.DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				// Set to just a "number":
				rawValue=new Css.Units.DecimalUnit(value);
				
				// Reserialize:
				if(List!=null){
					List.Reserialize();
				}
			}
		}
		
		/// <summary>The resolved value (in CSS pixels).</summary>
		public float valueInSpecifiedUnits{
			get{
				Css.Units.DecimalUnit du=rawValue as Css.Units.DecimalUnit;
				return (du==null)? 0f : du.DisplayNumber;
			}
			set{
				if(IsReadOnly){
					throw new Dom.DOMException(Dom.DOMException.NO_MODIFICATION_ALLOWED_ERR);
				}
				
				Css.Units.DecimalUnit du=rawValue as Css.Units.DecimalUnit;
				
				if(du==null){
					// Set to just a "number":
					rawValue=new Css.Units.DecimalUnit(value);
				}else{
					// Update its display value (which happens in OnValueReady):
					du.SetRawDecimal(value);
					du.OnValueReady(null);
				}
				
				// Reserialize:
				if(List!=null){
					List.Reserialize();
				}
			}
		}
		
		/// <summary>The value as a string.</summary>
		public string valueAsString{
			get{
				return rawValue.ToString();
			}
			set{
				// Load it:
				rawValue=Css.Value.Load(value);
			}
		}
		
		/// <summary>The type of units of this length.</summary>
		public ushort unitType{
			get{
				return GetLengthType(rawValue);
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