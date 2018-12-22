//--------------------------------------
//			   PowerUI
//
//		For documentation or 
//	if you have any issues, visit
//		powerUI.kulestar.com
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using Dom;
using Blaze;
using UnityEngine;
using Loonim;
using Css;


namespace Svg{
	
	/// <summary>
	/// The base of all fe* elements (such as feBlend).
	/// </summary>
	
	public class SVGFilterPrimitiveStandardAttributes:SVGElement{
		
		/// <summary>The computed in="" target element.</summary>
		private SVGFilterPrimitiveStandardAttributes In1_;
		/// <summary>The computed in2="" target element.</summary>
		private SVGFilterPrimitiveStandardAttributes In2_;
		
		/// <summary>The in="" element.</summary>
		public SVGFilterPrimitiveStandardAttributes In1{
			get{
				
				if(In1_==null){
					In1_=GetRef("in");
				}
				
				return In1_;
			}
		}
		
		/// <summary>The in2="" element.</summary>
		public SVGFilterPrimitiveStandardAttributes In2{
			get{
				
				if(In2_==null){
					In2_=GetRef("in2");
				}
				
				return In2_;
			}
		}
		
		/// <summary>Resolves the named CSS property into a decimal value held in the computed style.</summary>
		protected float ResolveDecimal(Css.CssProperty prop){
			
			// Reslove now:
			Css.Value val=ComputedStyle.Resolve(prop);
			
			if(val==null){
				// Use initial value to avoid this one.
				return 0f;
			}
			
			return val.GetDecimal(RenderData,prop);
			
		}
		
		/// <summary>The cached Loonim node.</summary>
		private TextureNode LoonimNode_;
		
		/// <summary>Gets this node as a loonim node. Potentially pulls a cached node.</summary>
		public TextureNode GetLoonim(SurfaceTexture tex){
			
			if(LoonimNode_==null){
				
				LoonimNode_=ToLoonimNode(tex);
				
			}
			
			return LoonimNode_;
			
		}
		
		/// <summary>Converts this SVG FX node to a Loonim node.</summary>
		protected virtual TextureNode ToLoonimNode(SurfaceTexture tex){
			return new Property(0f);
		}
		
		/// <summary>Gets the named CSS property as a colour. If it's not found, the given default is used.</summary>
		protected Color GetFilterColour(CssProperty prop,Color deflt){
			
			// Resolve the property (it must know its own default):
			Css.Value value=ComputedStyle.Resolve(prop);
			
			if(value==null){
				return deflt;
			}
			
			// Get as a colour:
			return value.GetColour(RenderData,prop);
			
		}
		
		/// <summary>Gets the element ref'd by ID from the named attribute. It's usually "in" or "in2".</summary>
		private SVGFilterPrimitiveStandardAttributes GetRef(string attr){
			
			string id=getAttribute(attr);
			
			if(id==null){
				return null;
			}
			
			// Get and return
			return document.getElementById(id) as SVGFilterPrimitiveStandardAttributes;
			
		}
		
		public override bool OnAttributeChange(string property){
			
			if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
}
