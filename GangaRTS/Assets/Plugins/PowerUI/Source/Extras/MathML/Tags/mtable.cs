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
	/// The MathML mtable tag.
	/// </summary>
	
	[Dom.TagName("mtable")]
	public class MathTableElement:MathElement{
		
		public override bool OnAttributeChange(string property){
			
			if(property=="align"){
				
				return true;
				
			}else if(property=="alignmentscope"){
				
				return true;
				
			}else if(property=="columnalign"){
				
				return true;
				
			}else if(property=="columnlines"){
				
				return true;
				
			}else if(property=="columnspacing"){
				
				return true;
				
			}else if(property=="columnwidth"){
				
				return true;
				
			}else if(property=="displaystyle"){
				
				return true;
				
			}else if(property=="equalcolumns"){
				
				return true;
				
			}else if(property=="equalrows"){
				
				return true;
				
			}else if(property=="frame"){
				
				return true;
				
			}else if(property=="framespacing"){
				
				return true;
				
			}else if(property=="groupalign"){
				
				return true;
				
			}else if(property=="minlabelspacing"){
				
				return true;
				
			}else if(property=="rowalign"){
				
				return true;
				
			}else if(property=="rowlines"){
				
				return true;
				
			}else if(property=="rowspacing"){
				
				return true;
				
			}else if(property=="side"){
				
				return true;
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
	}
	
}