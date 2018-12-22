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
	/// Represents the border-left: composite css property.
	/// </summary>
	
	public class BorderLeftProperty:CssCompositeProperty{
		
		public override string[] GetProperties(){
			return new string[]{"border-left"};
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			// Map the values into the underlying properties.
			
			// Border width:
			styleBlock.SetComposite("border-left-width",value.GetByTypes(ValueType.Number,ValueType.RelativeNumber),value);
			
			// Border style:
			styleBlock.SetComposite("border-left-style",value.GetByType(ValueType.Text),value);
			
			// Border colour:
			styleBlock.SetComposite("border-left-color",value.GetByType(ValueType.Set),value);
			
		}
		
	}
	
}



