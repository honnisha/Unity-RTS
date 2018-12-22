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
	/// Represents the swash function.
	/// </summary>
	
	public class Swash:FontVariant{
		
		public Swash(){
			
			Name="swash";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"swash"};
		}
		
		protected override Css.Value Clone(){
			Swash result=new Swash();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



