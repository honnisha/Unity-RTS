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
	/// Represents the border-top: composite css property.
	/// </summary>
	
	public class BorderTopProperty:CssCompositeProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-top"};
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			// Map the values into the underlying properties.
			
			// Border width:
			styleBlock.SetComposite("border-top-width",value.GetByTypes(ValueType.Number,ValueType.RelativeNumber),value);
			
			// Border style:
			styleBlock.SetComposite("border-top-style",value.GetByType(ValueType.Text),value);
			
			// Border colour:
			styleBlock.SetComposite("border-top-color",value.GetByType(ValueType.Set),value);
			
		}
		
	}
	
}



