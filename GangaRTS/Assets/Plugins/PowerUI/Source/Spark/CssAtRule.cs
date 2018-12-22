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
	/// A CSS at rule. You can create custom ones by deriving from this class.
	/// Note that they are instanced globally.
	/// </summary>
	
	[Values.Preserve]
	public class CssAtRule{
		
		/// <summary>The main name of this function. Originates from the first result returned by GetNames.</summary>
		public string Name{
			get{
				return GetNames()[0];
			}
		}
		
		/// <summary>The set of all names that this at rule can use. Usually just one. E.g. font-face etc.</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		/// <summary>This e.g. sets AtRuleMode. It's true if this @ rule uses nested selectors. Media and keyframes are two examples.</summary>
		public virtual void SetupParsing(CssLexer lexer){
		}
		
		/// <summary>Copies this at rule.</summary>
		public virtual CssAtRule Copy(){
			return null;
		}
		
		/// <summary>Called on this instance object to load it's values from the given value object.</summary>
		public virtual Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			return null;
		}
		
	}
	
}