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
	/// The MathML mfenced tag.
	/// </summary>
	
	[Dom.TagName("mfenced")]
	public class MathFencedElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="open"){
				
				return true;
				
			}else if(property=="separators"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}