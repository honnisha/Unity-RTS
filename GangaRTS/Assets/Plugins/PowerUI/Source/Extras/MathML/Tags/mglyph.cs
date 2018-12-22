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
	/// The MathML mglyph tag.
	/// </summary>
	
	[Dom.TagName("mglyph")]
	public class MathGlyphElement:MathElement{
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="alt"){
				
				return true;
				
			}else if(property=="valign"){
				
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