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
	/// The MathML mmultiscripts tag.
	/// </summary>
	
	[Dom.TagName("mmultiscripts")]
	public class MathMultiScriptsElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="subscriptshift"){
				
				return true;
				
			}else if(property=="superscriptshift"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}