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

namespace MathML{
	
	/// <summary>
	/// The MathML semantics annotation-xml tag.
	/// </summary>
	
	[Dom.TagName("annotation-xml")]
	public class MathAnnotationXmlElement:MathElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>True if this element indicates being 'in scope'. http://w3c.github.io/html/syntax.html#in-scope</summary>
		public override bool IsParserScope{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="definitionurl"){
			
				return true;
				
			}else if(property=="encoding"){
				
				return true;
				
			}else if(property=="cd"){
				
				return true;
				
			}else if(property=="name"){
				
				return true;
				
			}else if(property=="src"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}