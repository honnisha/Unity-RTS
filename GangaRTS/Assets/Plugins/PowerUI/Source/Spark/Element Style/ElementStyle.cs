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
using PowerUI;


namespace Css{
	
	/// <summary>
	/// The .style property of a html element.
	/// </summary>
	
	public partial class ElementStyle:Style{
		
		/// <summary>The computed style of this element.</summary>
		public ComputedStyle Computed;	
		
		
		/// <summary>Creates a new element style for the given element.</summary>
		/// <param name="element">The element that this will be the style for.</param>
		public ElementStyle(Dom.Element element):base(element){
			Computed=new ComputedStyle(element);
		}
		
		public override void OnChanged(CssProperty property,Value newValue){
			// Update the computed object:
			Computed.ChangeProperty(property,newValue);
		}
		
		/// <summary>JS API function for getting the computed style.</summary>
		public ComputedStyle getComputedStyle(){
			return Computed;
		}
		
		public override ComputedStyle GetComputed(){
			return Computed;
		}
		
		public override string GetString(string property){
			return Computed.GetString(property);
		}
		
		/// <summary>Sets the top left border radius.</summary>
		public string borderTopLeftRadius{
			set{
				Set("border-top-left-radius",value);
			}
			get{
				return GetString("border-top-left-radius");
			}
		}
		
		/// <summary>Sets the top right border radius.</summary>
		public string borderTopRightRadius{
			set{
				Set("border-top-right-radius",value);
			}
			get{
				return GetString("border-top-right-radius");
			}
		}
		
		/// <summary>Sets the bottom right border radius.</summary>
		public string borderBottomRightRadius{
			set{
				Set("border-bottom-right-radius",value);
			}
			get{
				return GetString("border-bottom-right-radius");
			}
		}
		
		/// <summary>Sets the bottom left border radius.</summary>
		public string borderBottomLeftRadius{
			set{
				Set("border-bottom-left-radius",value);
			}
			get{
				return GetString("border-bottom-left-radius");
			}
		}
		
		/// <summary>Sets the border radius of all corners.</summary>
		public string borderRadius{
			set{
				Set("border-radius",value);
			}
			get{
				return GetString("border-radius");
			}
		}
		
		/// <summary>CSS animation.</summary>
		public string animation{
			set{
				Set("animation",value);
			}
			get{
				return GetString("animation");
			}
		}
		
		/// <summary>Sets the left scroll offset.</summary>
		public string scrollLeft{
			set{
				Set("scroll-left",value);
			}
			get{
				return GetString("scroll-left");
			}
		}
		
		/// <summary>Sets the top scroll offset.</summary>
		public string scrollTop{
			set{
				Set("scroll-top",value);
			}
			get{
				return GetString("scroll-top");
			}
		}
		
		/// <summary>The minimum height for this element. Default is 0px.</summary>
		public string minHeight{
			set{
				Set("min-height",value);
			}
			get{
				return GetString("min-height");
			}
		}
		
		/// <summary>The minimum width for this element. Default is 0px.</summary>
		public string minWidth{
			set{
				Set("min-width",value);
			}
			get{
				return GetString("min-width");
			}
		}
		
		/// <summary>The maximum height for this element.</summary>
		public string maxHeight{
			set{
				Set("max-height",value);
			}
			get{
				return GetString("max-height");
			}
		}
		
		/// <summary>The maximum width for this element.</summary>
		public string maxWidth{
			set{
				Set("max-width",value);
			}
			get{
				return GetString("max-width");
			}
		}
		
		/// <summary>How scrolling text gets clipped. By default this is "fast" and makes text look like it is being squished. "fast" or "clip".</summary>
		public string textClip{
			set{
				Set("text-clip",value);
			}
			get{
				return GetString("text-clip");
			}
		}
		
		/// <summary>A shortcut for applying an image, solid colour and the repeat setting all at once.
		/// E.g. "url(myImage.png) repeat-x #000000".</summary>
		public string background{
			set{
				Set("background",value);
			}
			get{
				return GetString("background");
			}
		}
		
		/// <summary>background-attachment.</summary>
		public string backgroundAttachment{
			set{
				Set("background-attachment",value);
			}
			get{
				return GetString("background-attachment");
			}
		}
		
		/// <summary>The colour of a solid background. E.g. "#ffffff".</summary>
		public string backgroundColor{
			set{
				Set("background-color",value);
			}
			get{
				return GetString("background-color");
			}
		}
		
		/// <summary>The location of a background image. E.g. "url(imgInResources.png)", "url(cache://cachedImage)".
		/// See <see cref="PowerUI.FileProtocol"/> for more information on the default protocols://.</summary>
		public string backgroundImage{
			set{
				Set("background-image",value);
			}
			get{
				return GetString("background-image");
			}
		}
		
		/// <summary>The offset of the background. E.g. "10px 5px" (x,y).</summary>
		public string backgroundPosition{
			set{
				Set("background-position",value);
			}
			get{
				return GetString("background-position");
			}
		}
		
		/// <summary>How the background should be repeated if at all. E.g. "repeat-x", "repeat-y", "none".</summary>
		public string backgroundRepeat{
			set{
				Set("background-repeat",value);
			}
			get{
				return GetString("background-repeat");
			}
		}
		
		/// <summary>How the background should be scaled if at all. E.g. "100% 100%" or "auto" (default).</summary>
		public string backgroundSize{
			set{
				Set("background-size",value);
			}
			get{
				return GetString("background-size");
			}
		}
		
		/// <summary>How the width of the background should be scaled if at all. E.g. "100%" or "auto" (default).</summary>
		public string backgroundSizeX{
			set{
				Set("background-size-x",value);
			}
			get{
				return GetString("background-size-x");
			}
		}
		
		/// <summary>How the height of the background should be scaled if at all. E.g. "100%" or "auto" (default).</summary>
		public string backgroundSizeY{
			set{
				Set("background-size-y",value);
			}
			get{
				return GetString("background-size-y");
			}
		}
		
		/// <summary>background-clip.</summary>
		public string backgroundClip{
			set{
				Set("background-clip",value);
			}
			get{
				return GetString("background-clip");
			}
		}
		
		/// <summary>How far from the bottom this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string bottom{
			set{
				Set("bottom",value);
			}
			get{
				return GetString("bottom");
			}
		}
		
		/// <summary>The style of the border around the element. Can only be "solid" for now.</summary>
		public string borderStyle{
			set{
				Set("border-style",value);
			}
			get{
				return GetString("border-style");
			}
		}
		
		/// <summary>The width of the border around the element. E.g. "2px" (all sides) or "4px 5px 4px 5px" (top,right,bottom,left).</summary>
		public string borderWidth{
			set{
				Set("border-width",value);
			}
			get{
				return GetString("border-width");
			}
		}
		
		/// <summary>The colour of the border around the element. E.g. "#ffffff". Also supports alpha (e.g. #ffffff77).</summary>
		public string borderColor{
			set{
				Set("border-color",value);
			}
			get{
				return GetString("border-color");
			}
		}
		
		/// <summary>A shortcut for defining width, style and colour of all sides in one go. E.g. "2px solid #ffffff".</summary>
		public string border{
			set{
				Set("border",value);
			}
			get{
				return GetString("border");
			}
		}
		
		/// <summary>The width of the left border. E.g. "2px".</summary>
		public string borderLeft{
			set{
				Set("border-left",value);
			}
			get{
				return GetString("border-left");
			}
		}
		
		/// <summary>The width of the right border. E.g. "2px".</summary>
		public string borderRight{
			set{
				Set("border-right",value);
			}
			get{
				return GetString("border-right");
			}
		}
		
		/// <summary>The width of the top border. E.g. "2px".</summary>
		public string borderTop{
			set{
				Set("border-top",value);
			}
			get{
				return GetString("border-top");
			}
		}
		
		/// <summary>The width of the bottom border. E.g. "2px".</summary>
		public string borderBottom{
			set{
				Set("border-bottom",value);
			}
			get{
				return GetString("border-bottom");
			}
		}
		
		/// <summary>The style of the left border. E.g. "solid".</summary>
		public string borderLeftStyle{
			set{
				Set("border-left-style",value);
			}
			get{
				return GetString("border-left-style");
			}
		}
		
		/// <summary>The style of the right border. E.g. "solid".</summary>
		public string borderRightStyle{
			set{
				Set("border-right-style",value);
			}
			get{
				return GetString("border-right-style");
			}
		}
		
		/// <summary>The style of the top border. E.g. "solid".</summary>
		public string borderTopStyle{
			set{
				Set("border-top-style",value);
			}
			get{
				return GetString("border-top-style");
			}
		}
		
		/// <summary>The style of the bottom border. E.g. "solid".</summary>
		public string borderBottomStyle{
			set{
				Set("border-bottom-style",value);
			}
			get{
				return GetString("border-bottom-style");
			}
		}
		
		/// <summary>The color of the left border.</summary>
		public string borderLeftColor{
			set{
				Set("border-left-color",value);
			}
			get{
				return GetString("border-left-color");
			}
		}
		
		/// <summary>The color of the right border.</summary>
		public string borderRightColor{
			set{
				Set("border-right-color",value);
			}
			get{
				return GetString("border-right-color");
			}
		}
		
		/// <summary>The color of the top border.</summary>
		public string borderTopColor{
			set{
				Set("border-top-color",value);
			}
			get{
				return GetString("border-top-color");
			}
		}
		
		/// <summary>The color of the bottom border.</summary>
		public string borderBottomColor{
			set{
				Set("border-bottom-color",value);
			}
			get{
				return GetString("border-bottom-color");
			}
		}
		
		/// <summary>The width of the left border.</summary>
		public string borderLeftWidth{
			set{
				Set("border-left-width",value);
			}
			get{
				return GetString("border-left-width");
			}
		}
		
		/// <summary>The width of the right border.</summary>
		public string borderRightWidth{
			set{
				Set("border-right-width",value);
			}
			get{
				return GetString("border-right-width");
			}
		}
		
		/// <summary>The width of the top border.</summary>
		public string borderTopWidth{
			set{
				Set("border-top-width",value);
			}
			get{
				return GetString("border-top-width");
			}
		}
		
		/// <summary>The width of the bottom border.</summary>
		public string borderBottomWidth{
			set{
				Set("border-bottom-width",value);
			}
			get{
				return GetString("border-bottom-width");
			}
		}
		
		/// <summary>The CSS clear property.</summary>
		public string clear{
			set{
				Set("clear",value);
			}
			get{
				return GetString("clear");
			}
		}
		
		/// <summary>The CSS float property.</summary>
		public string cssFloat{
			set{
				Set("float",value);
			}
			get{
				return GetString("float");
			}
		}
		
		/// <summary>The clip property.</summary>
		public string clip{
			set{
				Set("clip",value);
			}
			get{
				return GetString("clip");
			}
		}
		
		/// <summary>The font colour. E.g. "#ffffff".</summary>
		public string color{
			set{
				Set("color",value);
			}
			get{
				return GetString("color");
			}
		}
		
		/// <summary>Sets the text content of this element.</summary>
		public string content{
			set{
				Set("content","\""+value.Replace("\"","\\\"")+"\"");
			}
			get{
				return GetString("content");
			}
		}
		
		/// <summary>The red component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayR{
			set{
				Set("color-overlay-r",value);
			}
			get{
				return GetString("color-overlay-r");
			}
		}
		
		/// <summary>The green component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayG{
			set{
				Set("color-overlay-g",value);
			}
			get{
				return GetString("color-overlay-g");
			}
		}
		
		/// <summary>The blue component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayB{
			set{
				Set("color-overlay-b",value);
			}
			get{
				return GetString("color-overlay-b");
			}
		}
		
		/// <summary>The alpha component of the colour overlay as a value from 0->1. e.g. "0.5".</summary>
		public string colorOverlayA{
			set{
				Set("color-overlay-a",value);
			}
			get{
				return GetString("color-overlay-a");
			}
		}
		
		/// <summary>A colour to apply over the top of this element. E.g. "#ff0000" will make it get a red tint.</summary>
		public string colorOverlay{
			set{
				Set("color-overlay",value);
			}
			get{
				return GetString("color-overlay");
			}
		}
		
		/// <summary>The cursor property.</summary>
		public string cursor{
			set{
				Set("cursor",value);
			}
			get{
				return GetString("cursor");
			}
		}
		
		/// <summary>The text direction.</summary>
		public string direction{
			set{
				Set("direction",value);
			}
			get{
				return GetString("direction");
			}
		}
		
		/// <summary>How this element should sit around other elements. "inline", "inline-block", "block", "none" (not visible).</summary>
		public string display{
			set{
				Set("display",value);
			}
			get{
				return GetString("display");
			}
		}
		
		/// <summary>Filter CSS property.</summary>
		public string filter{
			set{
				Set("filter",value);
			}
			get{
				return GetString("filter");
			}
		}
		
		/// <summary>How images should be filtered. "point", "bilinear","trilinear".</summary>
		public string filterMode{
			set{
				Set("filter-mode",value);
			}
			get{
				return GetString("filter-mode");
			}
		}
		
		/// <summary>The font CSS property.</summary>
		public string font{
			set{
				Set("font",value);
			}
			get{
				return GetString("font");
			}
		}
		
		/// <summary>This property has strict usage. It must refer to the name of a font in resources only; e.g. "PowerUI/Arial".
		/// Currently, there can only be one font in use on screen. There is certainly scope for adding more in the future.</summary>
		public string fontFamily{
			set{
				Set("font-family",value);
			}
			get{
				return GetString("font-family");
			}
		}
		
		/// <summary>The size of the font. E.g. "1.5em", "10px".</summary>
		public string fontSize{
			set{
				Set("font-size",value);
			}
			get{
				return GetString("font-size");
			}
		}
		
		/// <summary>The font-variant property.</summary>
		public string fontVariant{
			set{
				Set("font-variant",value);
			}
			get{
				return GetString("font-variant");
			}
		}
		
		/// <summary>Sets the weight (thickness) of the font. "bold", "normal".</summary>
		public string fontWeight{
			set{
				Set("font-weight",value);
			}
			get{
				return GetString("font-weight");
			}
		}
		
		/// <summary>Sets the styling of the font. "italic", "oblique", "none".</summary>
		public string fontStyle{
			set{
				Set("font-style",value);
			}
			get{
				return GetString("font-style");
			}
		}
		
		/// <summary>Sets the height of this element. E.g. "50%", "120px".</summary>
		public string height{
			set{
				Set("height",value);
			}
			get{
				return GetString("height");
			}
		}
		
		/// <summary>How far from the left this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string left{
			set{
				Set("left",value);
			}
			get{
				return GetString("left");
			}
		}
		
		/// <summary>Sets the gap between letters for custom kerning. You may also use a percentage, 
		/// e.g. 100% is no spacing and 200% is a gap that is the size of the font size between letters.</summary>
		public string letterSpacing{
			set{
				Set("letter-spacing",value);
			}
			get{
				return GetString("letter-spacing");
			}
		}
		
		/// <summary>Sets the line height for this element, e.g. "200%".</summary>
		public string lineHeight{
			set{
				Set("line-height",value);
			}
			get{
				return GetString("line-height");
			}
		}
		
		/// <summary>Sets the list-style property.</summary>
		public string listStyle{
			set{
				Set("list-style",value);
			}
			get{
				return GetString("list-style");
			}
		}
		
		/// <summary>Sets the list-style-image property.</summary>
		public string listStyleImage{
			set{
				Set("list-style-image",value);
			}
			get{
				return GetString("list-style-image");
			}
		}
		
		/// <summary>Sets the list-style-position property.</summary>
		public string listStylePosition{
			set{
				Set("list-style-position",value);
			}
			get{
				return GetString("list-style-position");
			}
		}
		
		/// <summary>Sets the list-style-type property.</summary>
		public string listStyleType{
			set{
				Set("list-style-type",value);
			}
			get{
				return GetString("list-style-type");
			}
		}
		
		/// <summary>The margin around the outside of the element. E.g. "6px" (all sides) or "4px 5px 4px 5px" (top,right,bottom,left).</summary>
		public string margin{
			set{
				Set("margin",value);
			}
			get{
				return GetString("margin");
			}
		}
		
		/// <summary>The size of the left margin around the outside of the element. E.g. "5px".</summary>
		public string marginLeft{
			set{
				Set("margin-left",value);
			}
			get{
				return GetString("margin-left");
			}
		}
		
		/// <summary>The size of the right margin around the outside of the element. E.g. "5px".</summary>
		public string marginRight{
			set{
				Set("margin-right",value);
			}
			get{
				return GetString("margin-right");
			}
		}
		
		/// <summary>The size of the top margin around the outside of the element. E.g. "5px".</summary>
		public string marginTop{
			set{
				Set("margin-top",value);
			}
			get{
				return GetString("margin-top");
			}
		}
		
		/// <summary>The size of the bottom margin around the outside of the element. E.g. "5px".</summary>
		public string marginBottom{
			set{
				Set("margin-bottom",value);
			}
			get{
				return GetString("margin-bottom");
			}
		}
		
		/// <summary>The opacity of the element as a value from 0->1. e.g. "0.5".</summary>
		public string opacity{
			set{
				Set("opacity",value);
			}
			get{
				return GetString("opacity");
			}
		}
		
		/// <summary>Sets what should happen if the content of an element overflows its boundaries. "scroll scroll" (x,y).</summary>
		public string overflow{
			set{
				Set("overflow",value);
			}
			get{
				return GetString("overflow");
			}
		}
		
		/// <summary>What happens if the content of an element overflows the x boundary. "scroll", "hidden", "auto", "visible".</summary>
		public string overflowX{
			set{
				Set("overflow-x",value);
			}
			get{
				return GetString("overflow-x");
			}
		}
		
		/// <summary>What happens if the content of an element overflows the y boundary. "scroll", "hidden", "auto", "visible".</summary>
		public string overflowY{
			set{
				Set("overflow-y",value);
			}
			get{
				return GetString("overflow-y");
			}
		}
		
		/// <summary>Should an image be on the atlas? May be globally overridden with UI.RenderMode. "true" (default) or "false".</summary>
		public string onAtlas{
			set{
				Set("on-atlas",value);
			}
			get{
				return GetString("on-atlas");
			}
		}
		
		/// <summary>The size of the padding inside the element. E.g. "20px" (all sides) or "4px 5px 4px 5px" (top,right,bottom,left).</summary>
		public string padding{
			set{
				Set("padding",value);
			}
			get{
				return GetString("padding");
			}
		}
		
		/// <summary>The size of the left padding inside the element. E.g. "5px".</summary>
		public string paddingLeft{
			set{
				Set("padding-left",value);
			}
			get{
				return GetString("padding-left");
			}
		}
		
		/// <summary>The size of the right padding inside the element. E.g. "5px".</summary>
		public string paddingRight{
			set{
				Set("padding-right",value);
			}
			get{
				return GetString("padding-right");
			}
		}
		
		/// <summary>The size of the top padding inside the element. E.g. "5px".</summary>
		public string paddingTop{
			set{
				Set("padding-top",value);
			}
			get{
				return GetString("padding-top");
			}
		}
		
		/// <summary>The size of the bottom padding inside the element. E.g. "5px".</summary>
		public string paddingBottom{
			set{
				Set("padding-bottom",value);
			}
			get{
				return GetString("padding-bottom");
			}
		}
		
		/// <summary>The page-break-after property.</summary>
		public string pageBreakAfter{
			set{
				Set("page-break-after",value);
			}
			get{
				return GetString("page-break-after");
			}
		}
		
		/// <summary>The page-break-before property.</summary>
		public string pageBreakBefore{
			set{
				Set("page-break-before",value);
			}
			get{
				return GetString("page-break-before");
			}
		}
		
		/// <summary>The position of this element. "fixed","relative","absolute".</summary>
		public string position{
			set{
				Set("position",value);
			}
			get{
				return GetString("position");
			}
		}
		
		/// <summary>How far from the right this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string right{
			set{
				Set("right",value);
			}
			get{
				return GetString("right");
			}
		}
		
		/// <summary>How far from the top this element is. E.g. "10px", "10%".
		/// What it's relative to depends on the position value.</summary>
		public string top{
			set{
				Set("top",value);
			}
			get{
				return GetString("top");
			}
		}
		
		/// <summary>The horizontal alignment of text and other elements. "left", "right", "center", "justify".</summary>
		public string textAlign{
			set{
				Set("text-align",value);
			}
			get{
				return GetString("text-align");
			}
		}
		
		/// <summary>The transform to apply.</summary>
		public string transform{
			set{
				Set("transform",value);
			}
			get{
				return GetString("transform");
			}
		}
		
		/// <summary>The location of the transform origin in 2D screen space. E.g. "10px 10px", "50% 50%".</summary>
		public string transformOrigin{
			set{
				Set("transform-origin",value);
			}
			get{
				return GetString("transform-origin");
			}
		}
		
		/// <summary>The x component of the location of the transform origin in 2D screen space.</summary>
		public string transformOriginX{
			set{
				Set("transform-origin-x",value);
			}
			get{
				return GetString("transform-origin-x");
			}
		}
		
		/// <summary>The y component of the location of the transform origin in 2D screen space.</summary>
		public string transformOriginY{
			set{
				Set("transform-origin-y",value);
			}
			get{
				return GetString("transform-origin-y");
			}
		}
		
		/// <summary>How the origin is positioned. "relative" (to the top left corner of the element)
		/// or "fixed" (fixed location on the screen).</summary>
		public string transformOriginPosition{
			set{
				Set("transform-origin-position",value);
			}
			get{
				return GetString("transform-origin-position");
			}
		}
		
		/// <summary>Can be used to apply a line to text. E.g. "underline", "line-through", "overline", "none".</summary>
		public string textDecoration{
			set{
				Set("text-decoration",value);
			}
			get{
				return GetString("text-decoration");
			}
		}
		
		/// <summary>text-decoration:blink.</summary>
		public bool textDecorationBlink{
			set{
				Set("text-decoration",value? "blink" : "none");
			}
			get{
				return GetString("text-decoration")=="blink";
			}
		}
		
		/// <summary>text-decoration:line-through.</summary>
		public bool textDecorationLineThrough{
			set{
				Set("text-decoration",value? "line-through" : "none");
			}
			get{
				return GetString("text-decoration")=="line-through";
			}
		}
		
		/// <summary>text-decoration:none.</summary>
		public bool textDecorationNone{
			set{
				Set("text-decoration","none");
			}
			get{
				return GetString("text-decoration")=="none";
			}
		}
		
		/// <summary>text-decoration:overline.</summary>
		public bool textDecorationOverline{
			set{
				Set("text-decoration",value? "overline" : "none");
			}
			get{
				return GetString("text-decoration")=="overline";
			}
		}
		
		/// <summary>text-decoration:underline.</summary>
		public bool textDecorationUnderline{
			set{
				Set("text-decoration",value? "underline" : "none");
			}
			get{
				return GetString("text-decoration")=="underline";
			}
		}
		
		/// <summary>The vertical alignment of child elements. "top","middle","bottom".</summary>
		public string verticalAlign{
			set{
				Set("vertical-align",value);
			}
			get{
				return GetString("vertical-align");
			}
		}
		
		/// <summary>Is this element visible? If not, it still takes up space. See display to make it act like it's not there at all.</summary>
		public string visibility{
			set{
				Set("visibility",value);
			}
			get{
				return GetString("visibility");
			}
		}
		
		/// <summary>Sets the width of this element. E.g. "50%", "120px".</summary>
		public string width{
			set{
				Set("width",value);
			}
			get{
				return GetString("width");
			}
		}
		
		/// <summary>Sets the gap between words for custom kerning.</summary>
		public string wordSpacing{
			set{
				Set("word-spacing",value);
			}
			get{
				return GetString("word-spacing");
			}
		}
		
		/// <summary>Defines how text should wrap onto new lines. Either "nowrap" or "normal".</summary>
		public string whiteSpace{
			set{
				Set("white-space",value);
			}
			get{
				return GetString("white-space");
			}
		}
		
		/// <summary>Sets the depth of this element. The higher the value, the higher up the element.</summary>
		public string zIndex{
			set{
				Set("z-index",value);
			}
			get{
				return GetString("z-index");
			}
		}
		
	}

}

namespace Dom{
	
	public partial class Element{
		
		/// <summary>True if this element matches the given selector.</summary>
		public bool matches(string selectorText){
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selectorText,this);
			
			// Read a value:
			Css.Value value=lexer.ReadValue();
			
			// Read the selectors from the value:
			List<Css.Selector> selectors=new List<Css.Selector>();
			Css.CssLexer.ReadSelectors(null,value,selectors);
			
			if(selectors.Count==0){
				return false;
			}
			
			// Get renderable node:
			Css.IRenderableNode irn=(this as Css.IRenderableNode);
			
			if(irn==null){
				// Can't select this element.
				return false;
			}
			
			// Get the computed style:
			Css.ComputedStyle cs=irn.ComputedStyle;
			
			// Try and match it!
			return selectors[0].StructureMatch(cs,new Css.CssEvent());
			
		}
		
		/// <summary>This elements style.</summary>
		public virtual Css.ElementStyle style{
			get{
				return null;
			}
		}
		
	}
	
}