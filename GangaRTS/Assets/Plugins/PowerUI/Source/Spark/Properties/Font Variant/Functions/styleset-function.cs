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
using UnityEngine;


namespace Css.Functions{
	
	/// <summary>
	/// Represents the styleset function.
	/// </summary>
	
	public class StyleSet:FontVariant{
		
		public StyleSet(){
			
			Name="styleset";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"styleset"};
		}
		
		protected override Css.Value Clone(){
			StyleSet result=new StyleSet();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



