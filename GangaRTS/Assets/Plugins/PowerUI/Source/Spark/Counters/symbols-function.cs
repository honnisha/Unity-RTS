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
	/// Represents the symbols() css function.
	/// </summary>
	
	public class SymbolsFunction:CssFunction{
		
		
		public SymbolsFunction(){
			
			Name="symbols";
			
		}
		
		public override string[] GetNames(){
			return new string[]{"symbols"};
		}
		
		protected override Css.Value Clone(){
			SymbolsFunction result=new SymbolsFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		/// <summary>The symbol at the given index.</summary>
		public Css.Value GetAt(int index){
			return this[index];
		}
		
		public override void OnValueReady(CssLexer lexer){
			
		}
		
	}
	
}



