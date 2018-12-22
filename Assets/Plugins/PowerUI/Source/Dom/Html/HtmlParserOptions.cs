using System;


namespace Dom{
	
	/// <summary>
	/// Contains a number of options for the HTML parser.
	/// </summary>
	public struct HtmlParserOptions{
		/// <summary>
		/// Gets or sets if the document is embedded.
		/// </summary>
		public bool IsEmbedded;

		/// <summary>
		/// Gets or sets if scripting is allowed.
		/// </summary>
		public bool IsScripting;

		/// <summary>
		/// Gets or sets if errors should be treated as exceptions.
		/// </summary>
		public bool IsStrictMode;
		
	}
	
}
