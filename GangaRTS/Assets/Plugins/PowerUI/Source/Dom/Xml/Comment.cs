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
	
	public class Comment:CharacterData{
		
		/// <summary>Tests whether two nodes are the same by attribute comparison.</summary>
		public override bool isEqualNode(Node other){
			if(other==this){
				return true;
			}
			
			return other is Comment && other.textContent==textContent;
		}
		
		/// <summary>All the HtmlTreeModes that result in this being appended to the document.</summary>
		private const int AppendToDocumentModes=HtmlTreeMode.Initial
			| HtmlTreeMode.BeforeHtml
			| HtmlTreeMode.AfterAfterFrameset
			| HtmlTreeMode.AfterAfterBody;
		
		/// <summary>Called when this node has been created and is being added to the given lexer.
		/// Closely related to Element.OnLexerCloseNode.</summary>
		/// <returns>True if this element handled itself.</returns>
		public override bool OnLexerAddNode(HtmlLexer lexer,int mode){
			
			if(mode==HtmlTreeMode.AfterBody){
				
				// Special case here:
				lexer.OpenElements[0].appendChild(this);
				
			}else{
				
				lexer.CurrentNode.appendChild(this);
				
			}
			
			return true;
		} 
		
		public override void ToString(System.Text.StringBuilder result){
			
			result.Append("<!--"+characterData_+"-->");
			
		}
		
		/// <summary>The name for this type of node.</summary>
		public override string nodeName{
			get{
				return "#comment";
			}
		}
		
		/// <summary>The type of element that this is.</summary>
		public override ushort nodeType{
			get{
				return 8;
			}
		}
		
		public override string ToString(){
			return "[object Comment]";
		}
		
	}
	
}