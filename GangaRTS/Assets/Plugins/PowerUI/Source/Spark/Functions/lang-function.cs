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
	/// Represents the lang() css function.
	/// </summary>
	
	public class LangFunction:CssFunction{
		
		/// <summary>The language code.</summary>
		public string LanguageCode;
		
		
		public LangFunction(){
			
			Name="lang";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"lang"};
		}
		
		protected override Css.Value Clone(){
			LangFunction result=new LangFunction();
			result.Values=CopyInnerValues();
			result.LanguageCode=LanguageCode;
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			string code=this[0].Text;
			
			LanguageCode=code.ToLower();
			
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			
			// Create a local lang selector:
			return new LangMatcher(LanguageCode);
			
		}
		
	}
	
	/// <summary>
	/// Handles the matching process for dir().
	/// </summary>
	sealed class LangMatcher : LocalMatcher{
		
		public string LanguageCode;
		
		
		public LangMatcher(string lc){
			
			LanguageCode=lc;
			
		}
		
		public override bool TryMatch(Dom.Node node){
			
			return (LanguageCode==Dom.Text.Language);
			
		}
		
	}
	
}