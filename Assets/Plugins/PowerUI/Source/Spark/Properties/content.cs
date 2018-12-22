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
using System.Collections;
using System.Collections.Generic;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the content: css property.
	/// Note that this one is *not* textual - this is actually correct.
	/// It doesn't apply to text - rather it 'is' text.
	/// </summary>
	
	public class Content:CssProperty{
		
		public static Content GlobalProperty;
		
		
		public Content(){
			// Note that it is not IsTextual - this is correct!
			GlobalProperty=this;
			InitialValue=new Css.Units.TextUnit("");
		}
		
		public override string[] GetProperties(){
			return new string[]{"content"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// *Must* be applied to a (virtual) span (Otherwise CSS style rules could potentially go recursive).
			PowerUI.HtmlSpanElement e=(style.Element as PowerUI.HtmlSpanElement);
			
			if(e!=null){
				
				// Write as innerHTML. Importantly we use the host (non-virtual) element as the context:
				RenderableData host=(e.parentNode as IRenderableNode).RenderData;
				
				// Get the text and write out now!
				e.innerHTML=value.GetText(host,this);
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



