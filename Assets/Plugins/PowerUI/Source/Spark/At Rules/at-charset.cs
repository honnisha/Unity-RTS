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


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the charset rule.
	/// </summary>
	
	public class Charset:CssAtRule{
		
		/// <summary>The charset to use.</summary>
		public string CharsetName;
		
		
		public override string[] GetNames(){
			return new string[]{"charset"};
		}
		
		public override CssAtRule Copy(){
			Charset at=new Charset();
			at.CharsetName=CharsetName;
			return at;
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			
			// Read a value:
			Css.Value val=value[1];
			
			// Get the value as constant text:
			CharsetName=val.Text;
			
			// Note: Charset rule is obsolete.
			return null;
			
		}
		
	}
	
}



