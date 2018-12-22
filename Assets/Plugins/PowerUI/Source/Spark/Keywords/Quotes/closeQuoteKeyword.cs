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
using Dom;


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the close-quote keyword.
	/// </summary>
	
	public class CloseQuote:CssKeyword{
		
		protected override Value Clone(){
			return new CloseQuote();
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			
			// Get the quotes text:
			Css.Value quotes=context.computedStyle[Css.Properties.Quotes.GlobalProperty];
			
			// Go up a parent (past the text element) and check for a 'lang' attribute:
			Element host=context.Node.parentElement;
			
			// -> We also need to know if we're nesting a q inside another q.
			bool innerQuote=(host!=null && host.Tag=="q");
			
			if(innerQuote){
				
				// Its parent must also be q:
				Element hostParent=host.parentElement;
				
				innerQuote=(hostParent!=null && hostParent.Tag=="q");
				
			}
			
			// Note quotes can be inherit too.
			if(quotes==null || quotes.IsAuto || (quotes.IsType(typeof(Css.Keywords.None)))){
				
				string langCode=null;
				
				if(host!=null){
					langCode=host.getAttribute("lang");
				}
				
				if(langCode==null){
					// Current UI language:
					langCode=Dom.Text.Language;
				}
				
				// No quotes explicitly defined. Time to look them up for the current language!
				Css.QuoteSet set=Css.Quotes.Get(langCode);
				
				return set[innerQuote?3:1];
			}
			
			return quotes[innerQuote?3:1].GetText(context,property);
			
		}
		
		public override string Name{
			get{
				return "close-quote";
			}
		}
		
	}
	
}



