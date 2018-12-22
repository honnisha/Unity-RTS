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
	
	/// <summary>
	/// Handles a variable tag within a language file.
	/// Variable tags define what a &variable; should be replaced with and essentially seperate
	/// layout or structure from the language.
	/// </summary>
	
	[Dom.TagName("var,v")]
	public class LangVarElement:LangElement{
		
		/// <summary>True if this element has special parsing rules.</summary>
		public override bool IsSpecial{
			get{
				return true;
			}
		}
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			// Close an open var tag if there's one in scope:
			if(lexer.CurrentTag=="var"){
				lexer.CloseCurrentNode();
			}
			
			// Add as text:
			lexer.RawTextOrRcDataAlgorithm(this,HtmlParseMode.Rawtext);
			
			return true;
			
		}
		
		/// <summary>Called when this tag is all loaded and ready to go.</summary>
		public override void OnChildrenLoaded(){
			
			string name=getAttribute("name");
			
			if(name==null){
				return;
			}
			
			// Get the group:
			LanguageGroup g=group;
			
			if(g==null){
				// Got a var inside all.xml - ignore it.
				return;
			}
			
			// Get the gender:
			string gender=getAttribute("gender");
			
			if(gender!=null){
				gender=gender.Trim().ToLower();
			}
			
			if(gender=="boy"||gender=="male"){
				name+=" gender:b";
			}else if(gender=="girl"||gender=="female"){
				name+=" gender:g";
			}
			
			// Set the value now:
			g.SetValue(name,firstChild.textContent);
			
		}
		
	}
	
}