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
	/// The MathML mfrac tag.
	/// </summary>
	
	[Dom.TagName("mfrac")]
	public class MathFractionElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="bevelled"){
				
				return true;
				
			}else if(property=="denomalign"){
				
				return true;
				
			}else if(property=="linethickness"){
				
				return true;
				
			}else if(property=="numalign"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
	
}