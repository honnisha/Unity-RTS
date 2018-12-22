using System;
using System.Collections;
using Dom;
using Css;


namespace Svg{
	
	/// <summary>
	/// An SVG image. Note that the ordinary HTML node system loads the actual nodes of the SVG.
	/// </summary>
	
	public partial class SVG{
		
		/// <summary>The internal document for this SVG. You can use all the ordinary DOM commands on this.</summary>
		public ReflowDocument document;
		
		
		/// <summary>Creates a new blank SVG.</summary>
		public SVG(ReflowDocument hostDocument){
			document=hostDocument;
		}
		
		/// <summary>Loads an SVG from the given XML string.</summary>
		public SVG(string xml){
			
			// Create an SVGDocument in this instance:
			document=new SVGDocument();
			
			if(xml!=null){
				innerSVG=xml;
			}
			
		}
		
		/// <summary>Gets an element with the given ID.</summary>
		public SVGElement getElementById(string id){
			return document.getElementById(id) as SVGElement;
		}
		
		/// <summary>The SVG's preferred width.</summary>
		public int Width{
			get{
				return svgElement.GetInt(Css.Properties.Width.GlobalProperty);
			}
		}
		
		/// <summary>The SVG's preferred height.</summary>
		public int Height{
			get{
				return svgElement.GetInt(Css.Properties.Height.GlobalProperty);
			}
		}
		
		/// <summary>Renders this SVG at the given size using the shared render context.</summary>
		public UnityEngine.Texture Render(int width,int height){
			
			// Create the context and draws the first time:
			RenderContext rc=svgElement.Context;
			
			// Update the size (Which triggers a draw):
			rc.SetSize(width,height);
			
			// Return the image:
			return rc.Texture;
			
		}
		
		/// <summary>Renders this SVG at the given size.
		/// Get the Texture property on the object returned.
		/// This also caches nothing - it's a one-shot texture.</summary>
		public UnityEngine.Texture RenderOnce(int width,int height){
			
			// Create the context and draws the first time:
			RenderContext rc=new RenderContext(svgElement);
			
			// Update the size (Which triggers a draw):
			rc.SetSize(width,height);
			
			// Return the image:
			return rc.Texture;
			
		}
		
		/// <summary>The cached svg element.</summary>
		private SVGSVGElement _SVG;
		
		/// <summary>The &gt;svg&lt; element.</summary>
		public SVGSVGElement svgElement{
			get{
				if(_SVG==null){
					_SVG=document.getElementByTagName("svg") as SVGSVGElement;
				}
				
				return _SVG;
			}
		}
		
		/// <summary>The SVG tag handler for the svgElement.</summary>
		public SVGSVGElement svgHandler{
			get{
				return svgElement;
			}
		}
		
		/// <summary>The title of this SVG.</summary>
		public string title{
			get{
				return document.title;
			}
			set{
				document.title=value;
			}
		}
		
		/// <summary>The description of this SVG.</summary>
		public string description{
			get{
				SVGDescElement desc=svgElement.getElementByTagName("desc") as SVGDescElement;
				
				if(desc==null){
					return null;
				}
				
				return desc.Description;
			}
		}
		
		/// <summary>The innerSVG of this SVG.</summary>
		public string innerSVG{
			get{
				
				return document.innerML;
				
			}
			set{
				
				// Clear cached values:
				_SVG=null;
				
				document.innerML=value;
				
			}
		}
		
	}

}