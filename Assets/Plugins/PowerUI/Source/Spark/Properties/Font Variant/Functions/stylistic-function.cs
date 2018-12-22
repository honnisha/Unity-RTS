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
	/// Represents the stylistic function.
	/// </summary>
	
	public class Stylistic:FontVariant{
		
		public Stylistic(){
			
			Name="stylistic";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"stylistic"};
		}
		
		protected override Css.Value Clone(){
			Stylistic result=new Stylistic();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



