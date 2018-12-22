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
	/// The MathML maction tag.
	/// </summary>
	
	[Dom.TagName("maction")]
	public class MathActionElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="actiontype"){
				
				return true;
				
			}else if(property=="selection"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}