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

using Dom;
using System.Text;
using UnityEngine;
using Blaze;
using Svg;


namespace Css{
	
	public partial class Value{
		
		/// <summary>Gets the shape that this CSS value represents.</summary>
		public virtual ShapeProvider ToShape(SVGElement context,RenderContext renderer,out Css.Value value){
			
			// Unchanged value:
			value=this;
			
			// Isn't one!
			return null;
			
		}
		
	}
	
}

namespace Css{
	
	/// <summary>
	/// Stores a TextureNode in the computed style.
	/// </summary>
	
	public class TextureNodeValue : Css.Value{
		
		/// <summary>The computed texture node.</summary>
		public Loonim.TextureNode TextureNode;
		/// <summary>The underlying value.</summary>
		public Css.Value RawValue;
		
		
		public TextureNodeValue(Css.Value val){
			
			// Value could be a set but not a colour. If that's the case, use the last one.
			// E.g. "red device-cmyk(0.1,....)"
			RawValue=val;
			
			Color result;
			
			if(val.IsColour){
				result=val.GetColour(null,null);
			}else{
				result=val[val.Count-1].GetColour(null,null);
			}
			
			// Assume solid col for now:
			TextureNode=new Loonim.Property(result);
			
		}
		
		public override string ToString(){
			return RawValue.ToString();
		}
		
	}
	
}

namespace Css.Functions{
	
	public partial class RectFunction{
		
		public override ShapeProvider ToShape(SVGElement context,RenderContext renderer,out Css.Value value){
			
			// Convert into an SVG rect function so it can store a cached square:
			SVGRectFunction rect=new SVGRectFunction();
			rect.Values=Values;
			value=rect;
			
			Css.Value junk;
			return value.ToShape(context,renderer,out junk);
			
		}
		
	}
	
	/// <summary>
	/// An SVG specific variant of the css rect() function.
	/// Standard rect() is auto-converted into one of these when it's used by the SVG system.
	/// </summary>
	
	public class SVGRectFunction : RectFunction{
		
		private ShapeProvider _cachedShape;
		
		
		public override ShapeProvider ToShape(SVGElement context,RenderContext renderer,out Css.Value value){
			
			value=this;
			
			if(_cachedShape==null){
				
				// Create a rectangle:
				RectangleProvider rect=new RectangleProvider();
				
				// Apply width/height/position to it:
				rect.X=this[0];
				rect.Y=this[1];
				rect.Width=this[2];
				rect.Height=this[3];
				
				// Cached:
				_cachedShape=rect;
				
			}
			
			return _cachedShape;
			
		}
		
		protected override Css.Value Clone(){
			SVGRectFunction result=new SVGRectFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
	public partial class UrlFunction{
		
		public override ShapeProvider ToShape(SVGElement context,RenderContext renderer,out Css.Value value){
			
			// Convert into an SVG url function so it can store a cached square:
			SVGUrlFunction url=new SVGUrlFunction();
			url.Values=Values;
			value=url;
			
			Css.Value junk;
			return value.ToShape(context,renderer,out junk);
			
		}
		
	}
	
	/// <summary>
	/// An SVG specific variant of the css url() function.
	/// Standard url() is auto-converted into one of these when it's used by the SVG system.
	/// </summary>
	
	public class SVGUrlFunction : UrlFunction{
		
		private ShapeProvider _cachedTarget;
		
		
		public override ShapeProvider ToShape(SVGElement context,RenderContext renderer,out Css.Value value){
			
			value=this;
			
			if(_cachedTarget==null){
				
				string id=this[0].Hash;
				
				// Get the target now:
				SVGElement target=context.document.getElementById(id) as SVGElement;
				
				if(target==null){
					return null;
				}
				
				// Get the path from the element (note that it could be a clip path):
				VectorPath path=target.GetPath(context,renderer);
				
				// Create a shape provider for it:
				_cachedTarget=new ShapeProvider();
				_cachedTarget.SetPath(path);
				
			}
			
			return _cachedTarget;
			
		}
		
		protected override Css.Value Clone(){
			SVGUrlFunction result=new SVGUrlFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}