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
	/// The MathML semantics annotation tag.
	/// </summary>
	
	[Dom.TagName("annotation")]
	public class MathAnnotationElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="definitionurl"){
			
				return true;
				
			}else if(property=="encoding"){
				
				return true;
				
			}else if(property=="cd"){
				
				return true;
				
			}else if(property=="name"){
				
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