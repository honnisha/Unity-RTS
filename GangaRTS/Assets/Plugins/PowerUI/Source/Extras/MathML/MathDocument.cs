using System;
using System.Collections;
using Dom;
using Css;

namespace MathML{
	
	/// <summary>
	/// A MathML document is used when MathML is displayed standalone (i.e. not inline inside a web page)
	/// </summary>
	
	public class MathDocument : ReflowDocument{
		
		/// <summary>Cached reference for the MathML namespace.</summary>
		private static MLNamespace _MathMLNamespace;
		
		/// <summary>The XML namespace for MathML.</summary>
		public static MLNamespace MathMLNamespace{
			get{
				if(_MathMLNamespace==null){
					
					// Setup the namespace (Doesn't request the URL; see XML namespaces for more info):
					_MathMLNamespace=Dom.MLNamespaces.Get("http://www.w3.org/1998/Math/MathML","mml","application/mathml+xml");
					
				}
				
				return _MathMLNamespace;
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
		/// <summary>The root math element.</summary>
		public MathMathElement math;
		
		
		public MathDocument():base(null){
			
			// Apply namespace:
			Namespace=MathMLNamespace;
			
		}
		
		/// <summary>The root style node.</summary>
		public override Element documentElement{
			get{
				return math;
			}
		}
		
		/// <summary>Gets or sets the innerMML of this document.</summary>
		public string innerMML{
			get{
				return innerML;
			}
			set{
				innerML=value;
			}
		}
		
		/// <summary>Gets or sets the innerML of this document.</summary>
		public override string innerML{
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
			
			math=null;
			
			// Gracefully clear the innerHTML:
			base.clear();
			
		}
		
	}
	
}