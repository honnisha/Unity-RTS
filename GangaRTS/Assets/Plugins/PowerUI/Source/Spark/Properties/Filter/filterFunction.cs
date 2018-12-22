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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the various css filter functions like blur, sepia etc.
	/// </summary>
	
	public class FilterFunction : CssFunction{
		
		public FilterFunction(){
		}
		
	}
	
}

namespace Css{
	
	public partial class Value{
		
		/// <summary>Converts this to a Loonim node.</summary>
		public virtual Loonim.TextureNode ToLoonimNode(RenderableData context){
			
			return null;
			
		}
		
	}
	
}