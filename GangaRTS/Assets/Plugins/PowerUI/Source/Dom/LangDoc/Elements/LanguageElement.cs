//--------------------------------------
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;

namespace Dom{

	/// <summary>
	/// This represents the language tag seen at the top of a language file.
	/// </summary>
	
	[XmlNamespace("http://translate.kulestar.com/namespace/","lang","text/lang",typeof(LangDocument))]
	[TagName("Default")]
	public class LangElement:Element{
		
		/// <summary>The group being loaded. Can be null if we're not in a group context.</summary>
		public LanguageGroup group{
			get{
				return (document_ as LangDocument).Group;
			}
		}
		
	}
	
}