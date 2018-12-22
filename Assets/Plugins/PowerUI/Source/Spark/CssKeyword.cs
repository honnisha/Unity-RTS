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


namespace Css{
	
	/// <summary>
	/// A CSS keyword. You can create custom ones by deriving from this class.
	/// Note that they are instanced globally and locally.
	/// </summary>
	
	[Values.Preserve]
	public class CssKeyword:Value{
		
		public CssKeyword(){
			Type=ValueType.Text;
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			return Name;
		}
		
		protected override Value Clone(){
			return Activator.CreateInstance(GetType()) as Value;
		}
		
		/// <summary>The keyword itself. E.g. auto etc.</summary>
		public virtual string Name{
			get{
				return null;
			}
		}
		
		public override string ToString(){
			return Name;
		}
		
	}
	
}