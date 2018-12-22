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

namespace Dom{
	
	/// <summary>An event called when any of the modifiers (such as gender) changes.</summary>
	public delegate void TextModifierChange();
	
	/// <summary>An event called when the language is changed.</summary>
	/// <param name="code">The new language code.</param>
	public delegate void LanguageChange(string code);
	
	/// <summary>
	/// This class simply represents a global Language service.
	/// Things such as the UI/Speech sign on to the language change event and
	/// update themselves accordingly when it's fired.
	/// </summary>
	
	public static class Text{
		
		/// <summary>The current language. See <see cref="Dom.Text.Language"/>.</summary>
		private static string _Language="en";
		/// <summary>A string appended onto the end of variable names for handling gender.</summary>
		public static string VariableModifiers="";
		/// <summary>The current gender. See <see cref="Dom.Text.Gender"/>.</summary>
		private static Gender _Gender=Gender.Either;
		/// <summary>An event fired when the language is changed.</summary>
		public static event LanguageChange OnLanguageChanged;
		/// <summary>An event fired when the player gender changes.</summary>
		public static event TextModifierChange OnGenderChanged;
		/// <summary>The global whitespace mode. By default sequences of spaces will be collapsed into one.</summary>
		public static WhitespaceMode Whitespace=WhitespaceMode.Normal;
		
		
		/// <summary>Called at startup. Sets up the default language.</summary>
		/// <param name="defaultLanguage">The code for the default language, e.g. "en".</param>
		public static void Setup(string defaultLanguage){
			Language=defaultLanguage;
		}
		
		/// <summary>Gets or sets the active gender. Internally fires the OnGenderChanged event.</summary>
		public static Gender Gender{
			get{
				return _Gender;
			}
			set{
				if(value==_Gender){
					return;
				}
				_Gender=value;
				if(value==Gender.Either){
					VariableModifiers="";
				}else if(value==Gender.Boy){
					VariableModifiers=" gender:b";
				}else{
					VariableModifiers=" gender:g";
				}
				// Tell everybody that the gender has changed
				if(OnGenderChanged!=null){
					OnGenderChanged();
				}
			}
		}
		
		/// <summary>Gets or sets the code of the active language (e.g. "en"). Internally fires the OnLanguageChanged event.</summary>
		public static string Language{
			get{
				return _Language;
			}
			set{
				value=value.ToLower();
				if(string.IsNullOrEmpty(value)){
					value="en";
				}
				if(value==_Language){
					return;
				}
				_Language=value;
				LanguageChanged();
			}
		}
		
		/// <summary>Tells all the OnLanguageChanged subscribers the language changed.
		/// Calling this directly is an alternative way of refreshing all &variable; elements.</summary>
		public static void LanguageChanged(){
			//Tell everybody it's changed globally:
			if(OnLanguageChanged!=null){
				OnLanguageChanged(_Language);
			}
		}
		
		/// <summary>Swaps e.g. < with &gt; so the text won't get treated like xml/html.</summary>
		/// <param name="xml">The xml to escape.</param>
		/// <returns>The escaped string.</returns>
		public static string Escape(string xml){
			StringReader reader=new StringReader(xml);
			System.Text.StringBuilder result=new System.Text.StringBuilder();
			
			while(reader.More()){
				char character=reader.Read();
				if(character=='<'){
					result.Append("&lt;");
				}else if(character=='>'){
					result.Append("&gt;");
				}else if(character=='&'){
					result.Append("&amp;");
				}else{
					result.Append(character);
				}
			}
			return result.ToString();
		}
		
	}
	
}