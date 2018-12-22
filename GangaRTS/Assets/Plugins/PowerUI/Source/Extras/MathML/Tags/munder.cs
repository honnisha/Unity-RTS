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
	/// The MathML munder tag.
	/// </summary>
	
	[Dom.TagName("munder")]
	public class MathUnderElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="accentunder"){
				
				return true;
				
			}else if(property=="align"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}