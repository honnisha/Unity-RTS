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


namespace Css{
	
	/// <summary>
	/// A set of quotes for a particular language.
	/// </summary>
	
	public class QuoteSet{
		
		public string[] Quotes;
		
		
		public QuoteSet(string quotes){
			Quotes=quotes.Split(' ');
		}
		
		public string this[int index]{
			get{
				return Quotes[index];
			}
			set{
				Quotes[index]=value;
			}
		}
		
	}
	
}