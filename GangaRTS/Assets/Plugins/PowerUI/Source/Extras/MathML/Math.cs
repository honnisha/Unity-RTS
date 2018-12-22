using System;
using System.Collections;
using Dom;
using Css;


namespace MathML{
	
	/// <summary>
	/// MathML. Note that the ordinary HTML node system loads the actual nodes of the MathML.
	/// </summary>
	
	public partial class Math{
		
		/// <summary>The internal document for this MathML. You can use all the ordinary DOM commands on this.</summary>
		public ReflowDocument document;
		
		
		/// <summary>Creates a new blank MathML.</summary>
		public Math(ReflowDocument hostDocument){
			document=hostDocument;
		}
		
		/// <summary>Loads MathML from the given XML string.</summary>
		public Math(string xml){
			
			// Create an MathDocument in this instance:
			document=new MathDocument();
			
			if(xml!=null){
				innerML=xml;
			}
			
		}
		
		/// <summary>Gets an element with the given ID.</summary>
		public MathElement getElementById(string id){
			return document.getElementById(id) as MathElement;
		}
		
		/// <summary>The title of this MathML document.</summary>
		public string title{
			get{
				return document.title;
			}
			set{
				document.title=value;
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
		
		/// <summary>The innerML of this MathML.</summary>
		public string innerML{
			get{
				
				return document.innerML;
				
			}
			set{
				
				document.innerML=value;
				
			}
		}
		
	}

}