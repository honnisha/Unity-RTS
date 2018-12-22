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
	/// Represents the annotation function.
	/// </summary>
	
	public class Annotation:FontVariant{
		
		public Annotation(){
			
			Name="annotation";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"annotation"};
		}
		
		protected override Css.Value Clone(){
			Annotation result=new Annotation();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



