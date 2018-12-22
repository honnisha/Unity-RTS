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
	/// Represents the background-attachment: css property.
	/// </summary>
	
	public class BackgroundAttachment:CssProperty{
		
		public BackgroundAttachment(){
			InitialValueText="scroll";
		}
		
		public override string[] GetProperties(){
			return new string[]{"background-attachment"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



