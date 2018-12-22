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
using Css;


namespace Css.Spec{
	
	/// <summary>
	/// Represents a string literal in the specification.
	/// </summary>
	
	public class Literal : Spec.Value{
		
		/// <summary>The value itself.</summary>
		public string RawLiteral;
		
		
		public Literal(string lit){
			RawLiteral=lit;
		}
		
		public override bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			
			if(value[start].Text==RawLiteral){
				size=1;
				return true;
			}
			
			size=0;
			return false;
			
		}
		
	}
	
}