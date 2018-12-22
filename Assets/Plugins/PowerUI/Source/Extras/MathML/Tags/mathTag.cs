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
	/// The parent Math tag.
	/// </summary>
	
	[Dom.TagName("math")]
	public class MathMathElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="display"){
				
				Style.Computed.ChangeTagProperty(
					"display",
					getAttribute(property)
				);
				
				return true;
			
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}