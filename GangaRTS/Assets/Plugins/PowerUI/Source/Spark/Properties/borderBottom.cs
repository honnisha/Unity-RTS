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

using System;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the border-bottom: composite css property.
	/// </summary>
	
	public class BorderBottomProperty:CssCompositeProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-bottom"};
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			// Map the values into the underlying properties.
			
			// Border width:
			styleBlock.SetComposite("border-bottom-width",value.GetByTypes(ValueType.Number,ValueType.RelativeNumber),value);
			
			// Border style:
			styleBlock.SetComposite("border-bottom-style",value.GetByType(ValueType.Text),value);
			
			// Border colour:
			styleBlock.SetComposite("border-bottom-color",value.GetByType(ValueType.Set),value);
			
		}
		
	}
	
}



