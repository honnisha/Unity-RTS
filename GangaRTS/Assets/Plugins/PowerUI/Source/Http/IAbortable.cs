//--------------------------------------
//          Kulestar Unity HTTP
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;


namespace PowerUI.Http{
	
	/// <summary>
	/// Used to abort a request.
	/// </summary>
	
	public interface IAbortable{
		
		/// <summary>The timeout if there is one.</summary>
		float timeout{get;set;}
		
		/// <summary>Aborts the request.</summary>
		void abort();
		
	}
	
}