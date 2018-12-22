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
	/// The MathML msub tag.
	/// </summary>
	
	[Dom.TagName("msub")]
	public class MathSubElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="superscriptshift"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}