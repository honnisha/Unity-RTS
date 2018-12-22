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
	/// Represents the import rule.
	/// </summary>
	
	public class Import:CssAtRule{
		
		/// <summary>The file to import.</summary>
		public string Href;
		/// <summary>The media query.</summary>
		public MediaQuery Query;
		
		
		public override string[] GetNames(){
			return new string[]{"import"};
		}
		
		public override CssAtRule Copy(){
			Import at=new Import();
			at.Href=Href;
			at.Query=Query;
			return at;
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			
			// Read a value:
			Css.Value val=value[1];
			
			// Load queries:
			Query=MediaQuery.Load(value,2,value.Count-1);
			
			// Get the value as constant text:
			Href=val.Text;
			
			return new ImportRule(style,value,Query,Href);
		}
		
	}
	
}

namespace Css{
	
	public partial class StyleSheet{
		
		/// <summary>The @import rule that imported this stylesheet.</summary>
		public Rule ownerRule;
		
	}
	
}



