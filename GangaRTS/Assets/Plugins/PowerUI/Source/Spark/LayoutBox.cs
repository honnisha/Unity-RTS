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
using System.Text;
using UnityEngine;


namespace Css{
	
	/// <summary>
	/// Used by the core layout system. They store full box information such as padding and margin, 
	/// then they get converted into basic box regions once the render process is complete.
	/// </summary>
	
	public class LayoutBox : BoxRegion{
		
		/// <summary>Elements are added in lines.
		/// We've got a linked list of line starts as well as a linked list of elements on each line.</summary>
		public LayoutBox NextLineStart;
		/// <summary>When being rendered, a linked list of computed styles on this line is built up. This is the next one on this line.</summary>
		public LayoutBox NextOnLine;
		/// <summary>The linked list of layout boxes that belong to a particular node.</summary>
		public LayoutBox NextInElement;
		/// <summary>If this box has child boxes, the first one. Note that it'll be a line start too.</summary>
		public LayoutBox FirstChild;
		/// <summary>The parent layout box, if there is one. Note that this is not the same as the parent element (as we can be e.g. inside
		/// a secondary line of a span).</summary>
		public LayoutBox Parent;
		/// <summary>The active font size.</summary>
		public float FontSize;
		/// <summary>The font face in use.</summary>
		public InfiniText.FontFace FontFace;
		public float ParentOffsetTop;
		public float ParentOffsetLeft;
		/// <summary>The size of a space at the end of the text.</summary>
		public float EndSpaceSize;
		/// <summary>The start index in the TextRenderingProperty glyph array (inclusive).</summary>
		public int TextStart;
		/// <summary>The end index in the TextRenderingProperty glyph array (*exclusive* - allows it to 'select' 0 characters).</summary>
		public int TextEnd;
		/// <summary>Table metadata. Used by display table-cell/ table.</summary>
		public TableMeta TableMeta;
		/// <summary>False for 'replaced' inline elements.</summary>
		public bool OrdinaryInline=true;
		/// <summary>The bidirectional draw mode. Typically it'll be leftwards or neutral.</summary>
		public int UnicodeBidi;
		/// <summary>True if this box contains text and it doesn't end in a space.</summary>
		public bool NoEndingSpace{
			get{
				return (EndSpaceSize==0f);
			}
		}
		
		/// <summary>Display mode.</summary>
		public int DisplayMode;
		/// <summary>Float: value.</summary>
		public int FloatMode;
		/// <summary>Position: value.</summary>
		public int PositionMode;
		
		/// <summary>The width of interior of this element. Used by the scrolling system.</summary>
		public float InnerWidth;
		/// <summary>The height of interior of this element. Used by the scrolling system.</summary>
		public float InnerHeight;
		/// <summary>The width of content in this element. Used by the scrolling system.</summary>
		public float ContentWidth;
		/// <summary>The height of the content in this element. Used by the scrolling system.</summary>
		public float ContentHeight;
		/// <summary>The distance between the bottom of this box and the baseline.</summary>
		public float Baseline;
		/// <summary>Defines the visibility of this element.</summary>
		public int Visibility=VisibilityMode.Visible;
		/// <summary>Defines how horizontally overflowing content is treated. Visible by default.
		/// Only applicable to block/inline-block elements.</summary>
		public int OverflowX=VisibilityMode.Visible;
		/// <summary>Defines how vertically overflowing content is treated. Visible by default.
		/// Only applicable to block/inline-block elements.</summary>
		public int OverflowY=VisibilityMode.Visible;
		
		/// <summary>Position.</summary>
		public BoxStyle Position;
		/// <summary>Padding.</summary>
		public BoxStyle Padding;
		/// <summary>Margin.</summary>
		public BoxStyle Margin;
		/// <summary>Border width.</summary>
		public BoxStyle Border;
		/// <summary>Accumulated scrolling state (the total of this and all parents).</summary>
		public BoxStyle Scroll;
		/// <summary>True if this was made with border-box.</summary>
		public bool BorderBox;
		
		
		/// <summary>How much of this elements horizontal content is currently visible? Used by scrolling.</summary>
		/// <returns>A value from 0-1 of how much of the horizontal content is visible. 1 is all of it.</returns>
		public float VisiblePercentageX(){
			return InnerWidth/ContentWidth;
		}
		
		/// <summary>How much of this elements vertical content is currently visible? Used by scrolling.</summary>
		/// <returns>A value from 0-1 of how much of the vertical content is visible. 1 is all of it.</returns>
		public float VisiblePercentageY(){
			return InnerHeight/ContentHeight;
		}
		
		/// <summary>Checks if this element is offset or positioned.</summary>
		/// <returns>True if this element is offset/positioned in any way.</returns>
		public bool IsOffset(){
			return (Position.Left!=float.MaxValue || Position.Top!=float.MaxValue || Position.Right!=float.MaxValue || Position.Bottom!=float.MaxValue);
		}
		
		/// <summary>Figures out the pixel width/pixel height using padding, border and inner dimensions.</summary>
		public void SetDimensions(bool widthUndefined,bool heightUndefined){
			
			if(widthUndefined){
				// Inline elements. We come back to these later.
				Width=0;
			}else{
				Width=BorderedWidth;
			}
			
			if(heightUndefined){
				// Most elements. We come back to the height later.
				Height=0;
			}else{
				Height=BorderedHeight;
			}
			
		}
		
		/// <summary>Width plus margin.</summary>
		public float TotalWidth{
			get{
				return Margin.Left+Width+Margin.Right;
			}
		}
		
		/// <summary>Height plus margin.</summary>
		public float TotalHeight{
			get{
				return Height+Margin.Top+Margin.Bottom;
			}
		}
		
		/// <summary>InnerStartX+InnerWidth</summary>
		public float InnerEndX{
			get{
				return InnerStartX+InnerWidth;
			}
		}
		
		/// <summary>InnerStartY+InnerHeight</summary>
		public float InnerEndY{
			get{
				return InnerStartY+InnerHeight;
			}
		}
		
		/// <summary>X+StyleOffsetLeft</summary>
		public float InnerStartX{
			get{
				return X+Padding.Left+Border.Left;
			}
		}
		
		/// <summary>Y+StyleOffsetTop</summary>
		public float InnerStartY{
			get{
				return Y+Padding.Top+Border.Top;
			}
		}
		
		/// <summary>Padding plus inner width plus border. Typically the same as PixelWidth (but directly computes it).</summary>
		public float BorderedWidth{
			get{
				return Border.Left+Padding.Left+InnerWidth+Padding.Right+Border.Right;
			}
		}
		
		/// <summary>Padding plus inner height. Typically the same as PixelHeight (but directly computes it).</summary>
		public float BorderedHeight{
			get{
				return Border.Top+Padding.Top+InnerHeight+Padding.Bottom+Border.Bottom;
			}
		}
		
		/// <summary>Padding plus inner width.</summary>
		public float PaddedWidth{
			get{
				return Padding.Left+InnerWidth+Padding.Right;
			}
		}
		
		/// <summary>Padding plus inner height.</summary>
		public float PaddedHeight{
			get{
				return Padding.Top+InnerHeight+Padding.Bottom;
			}
		}
		
		/// <summary>The offset of style from the left of the screen without scroll.</summary>
		public float FixedStyleOffsetLeft{
			get{
				return Padding.Left+Border.Left;
			}
		}
		
		/// <summary>The offset of style from the top of the screen without scroll.</summary>
		public float FixedStyleOffsetTop{
			get{
				return Padding.Top+Border.Top;
			}
		}
		
		/// <summary>The offset of style from the top of the screen.</summary>
		public float StyleOffsetTop{
			get{
				return Padding.Top+Border.Top;
			}
		}
		
		/// <summary>The size of style at the bottom.</summary>
		public float StyleOffsetBottom{
			get{
				return Padding.Bottom+Border.Bottom;
			}
		}
		
		/// <summary>The offset of style from the left of the screen.</summary>
		public float StyleOffsetLeft{
			get{
				return Padding.Left+Border.Left;
			}
		}
		
		/// <summary>The offset of style from the left of the screen as used by inline elements.</summary>
		public float InlineStyleOffsetLeft{
			get{
				return Padding.Left+Margin.Left+Border.Left;
			}
		}
		
		/// <summary>The offset of style from the top of the screen as used by inline elements.</summary>
		public float InlineStyleOffsetTop{
			get{
				return Padding.Top+Margin.Top+Border.Top;
			}
		}
		
		public override string ToString(){
			
			return "LayoutBox(x: "+X+", y: "+Y+", width: "+Width+", height: "+Height+", offset x: "+ParentOffsetLeft+", offset y: "+ParentOffsetTop+")";
			
		}
		
	}
	
}