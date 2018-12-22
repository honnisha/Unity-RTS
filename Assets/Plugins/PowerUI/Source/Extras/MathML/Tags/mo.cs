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
	/// The MathML mo tag.
	/// </summary>
	
	[Dom.TagName("mo")]
	public class MathOElement:MathElement{
		
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
			
			if(property=="accent"){
				
				return true;
				
			}else if(property=="fence"){
				
				return true;
				
			}else if(property=="form"){
				
				return true;
				
			}else if(property=="largeop"){
				
				return true;
				
			}else if(property=="lspace"){
				
				return true;
				
			}else if(property=="maxsize"){
				
				return true;
				
			}else if(property=="minsize"){
				
				return true;
				
			}else if(property=="moveablelimits"){
				
				return true;
				
			}else if(property=="rspace"){
				
				return true;
				
			}else if(property=="separator"){
				
				return true;
				
			}else if(property=="stretchy"){
				
				return true;
				
			}else if(property=="symmetric"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}