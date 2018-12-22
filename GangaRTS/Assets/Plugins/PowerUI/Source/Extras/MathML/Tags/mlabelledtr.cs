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
	/// The MathML mlabelledtr tag.
	/// </summary>
	
	[Dom.TagName("mlabelledtr")]
	public class MathLabelledTrElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="columnalign"){
				
				return true;
				
			}else if(property=="groupalign"){
				
				return true;
				
			}else if(property=="rowalign"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}