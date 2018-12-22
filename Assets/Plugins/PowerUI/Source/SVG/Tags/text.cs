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

using Dom;

namespace Svg{
	
	/// <summary>
	/// An SVG text tag.
	/// </summary>
	
	[Dom.TagName("text")]
	public class SVGTextElement:SVGTextContentElement{
		
		public SVGTextElement(){}
		
        /// <summary>
        /// Initializes a new instance of the <see cref="SvgText"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        public SVGTextElement(string text):this(){
            this.Text = text;
        }
		
    }
	
}