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
using Dom;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the user-select: css property.
	/// </summary>
	
	public class UserSelect:CssProperty{
		
		public static UserSelect GlobalProperty;
		
		
		/// <summary>Called when a selection starts using the given input pointer (typically a mouse).</summary>
		public static void BeginSelect(PowerUI.InputPointer pointer,Css.Value mode){
			
			// Selection starting at the position the pointer started dragging
			
			// Firstly, resolve our doc coords to a selection range start.
			
			Element pressed = pointer.ActivePressedTarget as Element;
			
			if(pressed == null){
				return;
			}
			
			// Get as first containing child node (should be text or null):
			NodeList kids=pressed.childNodes_;
			
			if(kids!=null){
				
				for(int i=0;i<kids.length;i++){
					
					IRenderableNode irn=(kids[i] as IRenderableNode);
					
					if(irn!=null){
						
						// Contains it?
						LayoutBox box=irn.RenderData.BoxAt(pointer.DownDocumentX, pointer.DownDocumentY);
						
						if(box==null){
							// Use the last one:
							box=irn.RenderData.LastBox;
						}
						
						if(box!=null){
							
							// Great! Try it as a text node (should always be one):
							PowerUI.RenderableTextNode htn=(kids[i] as PowerUI.RenderableTextNode);
							
							if(htn!=null){
								
								// Awesome - get the letter indices:
								int startIndex=htn.LetterIndex(pointer.DownDocumentX,box);
								int endIndex=htn.LetterIndex(pointer.DocumentX,pointer.DocumentY);
								
								// Create a range:
								Range range=new Range();
								range.startOffset=startIndex;
								range.endOffset=endIndex;
								range.startContainer=htn;
								range.endContainer=htn;
								
								// Get the current selection:
								Selection s=(kids[i].document as HtmlDocument).getSelection();
								
								// Clear all:
								s.removeAllRanges();
								
								// Add range:
								s.addRange(range);
								
							}
							
							break;
							
						}
						
					}
					
				}
				
			}
			
		}
		
		public UserSelect(){
			
			GlobalProperty=this;
			Inherits=true;
			InitialValue=AUTO;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"user-select","-moz-user-select"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



