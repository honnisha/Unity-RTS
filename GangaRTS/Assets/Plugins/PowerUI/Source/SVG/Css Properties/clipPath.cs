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
	/// Represents the clip and clip-path: css property.
	/// </summary>
	
	public class ClipPath:CssProperty{
		
		public ClipPath(){
			NamespaceName="svg";
		}
		
		public override string[] GetProperties(){
			return new string[]{"clip","clip-path"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



