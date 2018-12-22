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
	/// Represents the ornaments function.
	/// </summary>
	
	public class Ornaments:FontVariant{
		
		public Ornaments(){
			
			Name="ornaments";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"ornaments"};
		}
		
		protected override Css.Value Clone(){
			Ornaments result=new Ornaments();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



