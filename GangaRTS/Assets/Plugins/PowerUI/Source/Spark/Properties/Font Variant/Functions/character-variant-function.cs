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
	/// Represents the character-variant function.
	/// </summary>
	
	public class CharacterVariant:FontVariant{
		
		public CharacterVariant(){
			
			Name="character-variant";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"character-variant"};
		}
		
		protected override Css.Value Clone(){
			CharacterVariant result=new CharacterVariant();
			result.Values=CopyInnerValues();
			return result;
		}
		
	}
	
}



