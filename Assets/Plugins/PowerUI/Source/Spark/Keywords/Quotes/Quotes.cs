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
using System.Collections;
using System.Collections.Generic;


namespace Css{
	
	/// <summary>
	/// Maps 2 character language code (e.g. "en") to the quotes to use.
	/// </summary>
	
	public static class Quotes{
		
		/// <summary>The default quote set.</summary>
		private static QuoteSet DefaultSet;
		/// <summary>The lookup.</summary>
		private static Dictionary<string,QuoteSet> LanguageLookup;
		
		
		/// <summary>Gets the quote set for the given language.</summary>
		public static QuoteSet Get(string langCode){
			
			if(LanguageLookup==null){
				
				// Load now:
				Setup();
				
			}
			
			QuoteSet set;
			if(!LanguageLookup.TryGetValue(langCode,out set)){
				
				set=DefaultSet;
				
			}
			
			return set;
			
		}
		
		/// <summary>Adds the given lang to the lookup.</summary>
		private static QuoteSet Add(string langCode,string quotes){
			QuoteSet set=new QuoteSet(quotes);
			langCode=langCode.Trim().ToLower();
			
			LanguageLookup[langCode]=set;
			return set;
		}
		
		/// <summary>Sets up the lookup.</summary>
		private static void Setup(){
			
			LanguageLookup=new Dictionary<string,QuoteSet>();
			DefaultSet=Add("en","\u201C \u201D \u2018 \u2019");
			
			Add("eo", "\u201C \u201D \u201C \u201D");
			Add("es", "\u00AB \u00BB \u0022 \u0022");
			Add("fa", "\u00AB \u00BB \u00AB \u00BB");
			Add("fi", "\u201D \u201D \u2019 \u2019");
			Add("fr", "\u00AB\u00A0 \u00A0\u00BB \u2039\u00A0 \u00A0\u203A");
			Add("hr", "\u00BB \u00AB \u203A \u2039");
			Add("hu", "\u201E \u201D \u201E \u201D");
			Add("hy", "\u00AB \u00BB \u201C \u201D");
			Add("id", "\u201C \u201D \u2018 \u2019");
			Add("it", "\u00BB \u00AB \u00BB \u00AB");
			Add("ja", "\u300C \u300D \u300E \u300F");
			Add("ko", "\u201C \u201D \u2018 \u2019");
			Add("lt", "\u201E \u201C \u201E \u201C");
			Add("nl", "\u201E \u201D \u201A \u2019");
			Add("nb", "\u00AB \u00BB \u2018 \u2019");
			Add("no", "\u00AB \u00BB \u2018 \u2019");
			Add("pl", "\u201E \u201D \u00AB \u00BB");
			Add("pt", "\u201C \u201D \u2018 \u2019");
			Add("ro", "\u201E \u201D \u00AB \u00BB");
			Add("ru", "\u00AB \u00BB \u201E \u201C");
			Add("sk", "\u201E \u201C \u201A \u2018");
			Add("sl", "\u201E \u201C \u201A \u2018");
			Add("sv", "\u201D \u201D \u2019 \u2019");
			Add("tr", "\u00AB \u00BB \u2039 \u203A");
			Add("uk", "\u00AB \u00BB \u201E \u201C");
			Add("zh-cn", "\u201C \u201D \u2018 \u2019");
			Add("zh-hk", "\u300C \u300D \u300E \u300F");
			Add("zh-tw", "\u300C \u300D \u300E \u300F");
			Add("vi", "\u00AB \u00BB \u2039 \u203A");
		}
		
	}
	
}