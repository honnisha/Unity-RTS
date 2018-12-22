using System;


namespace Dom{
	
	/// <summary>
	/// Possible insertation mode values.
	/// </summary>
	public static class HtmlTreeMode{
		
		/// <summary>
		/// Initial insertation mode.
		/// </summary>
		public const int Initial=1;
		/// <summary>
		/// Before the html tag insertation mode.
		/// </summary>
		public const int BeforeHtml=1<<1;
		/// <summary>
		/// Before the head tag insertation mode.
		/// </summary>
		public const int BeforeHead=1<<2;
		/// <summary>
		/// Within the head tag insertation mode.
		/// </summary>
		public const int InHead=1<<3;
		/// <summary>
		/// Within the head tag in a noscript section.
		/// </summary>
		public const int InHeadNoScript=1<<4;
		/// <summary>
		/// After the head tag insertation mode.
		/// </summary>
		public const int AfterHead=1<<5;
		/// <summary>
		/// Within the body tag insertation mode.
		/// </summary>
		public const int InBody=1<<6;
		/// <summary>
		/// Within some text area insertation mode.
		/// </summary>
		public const int Text=1<<7;
		/// <summary>
		/// Within a table tag insertation mode.
		/// </summary>
		public const int InTable=1<<8;
		/// <summary>
		/// Within the table caption tag.
		/// </summary>
		public const int InCaption=1<<9;
		/// <summary>
		/// Within the column group tag.
		/// </summary>
		public const int InColumnGroup=1<<10;
		/// <summary>
		/// Within the table body tag.
		/// </summary>
		public const int InTableBody=1<<11;
		/// <summary>
		/// Within a table row tag.
		/// </summary>
		public const int InRow=1<<12;
		/// <summary>
		/// Within a table division tag.
		/// </summary>
		public const int InCell=1<<13;
		/// <summary>
		/// Within a select tag insertation mode.
		/// </summary>
		public const int InSelect=1<<14;
		/// <summary>
		/// Within a select tag in a table.
		/// </summary>
		public const int InSelectInTable=1<<15;
		/// <summary>
		/// Within the template tag.
		/// </summary>
		public const int InTemplate=1<<16;
		/// <summary>
		/// After the body tag.
		/// </summary>
		public const int AfterBody=1<<17;
		/// <summary>
		/// Within the frameset tag.
		/// </summary>
		public const int InFrameset=1<<18;
		/// <summary>
		/// After the frameset tag.
		/// </summary>
		public const int AfterFrameset=1<<19;
		/// <summary>
		/// After the after the body tag.
		/// </summary>
		public const int AfterAfterBody=1<<20;
		/// <summary>
		/// Once we are far behind the frameset tag.
		/// </summary>
		public const int AfterAfterFrameset=1<<21;
		/// <summary>
		/// Unchanged state.
		/// </summary>
		public const int Current=0;
		
		/// <summary>
		/// All the states that default open tags to the "InBody" state.
		/// </summary>
		public const int AllInBodyStates=InBody | InCaption | InCell | AfterAfterBody;
		
	}
	
}
