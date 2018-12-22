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
	/// The MathML mstyle tag.
	/// </summary>
	
	[Dom.TagName("mstyle")]
	public class MathStyleElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="decimalpoint"){
				
				return true;
				
			}else if(property=="displaystyle"){
				
				return true;
				
			}else if(property=="infixlinebreakstyle"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}