using System;
using System.Collections;
using Dom;
using Css;

namespace Svg{
	
	/// <summary>
	/// An SVG document is used when an SVG is displayed standalone (i.e. not inline inside a web page)
	/// </summary>
	
	public class SVGDocument : ReflowDocument{
		
		/// <summary>Cached reference for the SVG namespace.</summary>
		private static MLNamespace _SVGNamespace;
		
		/// <summary>The XML namespace for SVG.</summary>
		public static MLNamespace SVGNamespace{
			get{
				if(_SVGNamespace==null){
					
					// Setup the namespace (Doesn't request the URL; see XML namespaces for more info):
					_SVGNamespace=Dom.MLNamespaces.Get("http://www.w3.org/2000/svg","svg","image/svg+xml");
					
				}
				
				return _SVGNamespace;
			}
		}
		
		private bool IsOpen=true;
		private string CurrentTitle;
		
		
		public override string title{
			get{
				return CurrentTitle;
			}
			set{
				CurrentTitle=value;
			}
		}
		/// <summary>The root SVG element.</summary>
		public SVGSVGElement svg;
		
		
		public SVGDocument():base(null){
			
			// Apply the namespace:
			Namespace=SVGNamespace;
			Renderer=new Renderman(this);
			
			// Clear style; this loads in the default stylesheet:
			ClearStyle();
			
		}
		
		/// <summary>The root style node.</summary>
		public override Dom.Element documentElement{
			get{
				return svg;
			}
		}
		
		/// <summary>Gets or sets the innerSVG of this document.</summary>
		public override string innerML{
			get{
				return innerSVG;
			}
			set{
				innerSVG=value;
			}
		}
		
		/// <summary>Gets or sets the innerSVG of this document.</summary>
		public string innerSVG{
			get{
				System.Text.StringBuilder builder=new System.Text.StringBuilder();
				ToString(builder);
				return builder.ToString();
			}
			set{
				// Open parse and close:
				IsOpen=false;
				open();
				
				// Parse now:
				HtmlLexer lexer=new HtmlLexer(value,this);
				lexer.Parse();
				
				// Dom loaded:
				ContentLoadedEvent();
				
				close();
			}
		}
		
		/// <summary>Closes the document.</summary>
		public void close(){
			
			if(!IsOpen){
				// Already closed.
				return;
			}
			
			// Mark as closed:
			IsOpen=false;
			
			// Force a render request as required:
			RequestLayout();
			
		}
		
		/// <summary>Opens the document for writing.</summary>
		public void open(){
			
			if(IsOpen){
				// Already open
				return;
			}
			
			// Mark as open:
			IsOpen=true;
			
			// Clear it:
			clear();
			
		}
		
		/// <summary>Clears the document of all it's content, including scripts and styles.</summary>
		public override void clear(){
			
			svg=null;
			
			// Gracefully clear the innerHTML:
			base.clear();
			
		}
		
	}
	
}