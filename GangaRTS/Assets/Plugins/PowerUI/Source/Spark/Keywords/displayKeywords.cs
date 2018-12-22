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


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the inline keyword.
	/// This is defined because it is the initial value; unlike all the other values, initial values don't get
	/// applied. display's apply caches an integer version of each keyword.
	/// Essentially we need this to be able to respond with the integer display mode for 'inline'.
	/// </summary>
	
	public class Inline:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.Inline;
		}
		
		public override string Name{
			get{
				return "inline";
			}
		}
		
	}
	
	public class Block:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.Block;
		}
		
		public override string Name{
			get{
				return "block";
			}
		}
		
	}
	
	public class RunIn:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.RunIn;
		}
		
		public override string Name{
			get{
				return "run-in";
			}
		}
		
	}
	
	public class InlineBlock:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.InlineBlock;
		}
		
		public override string Name{
			get{
				return "inline-block";
			}
		}
		
	}
	
	public class Flex:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.Flex;
		}
		
		public override string Name{
			get{
				return "flex";
			}
		}
		
	}
	
	public class InlineFlex:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.InlineFlex;
		}
		
		public override string Name{
			get{
				return "inline-flex";
			}
		}
		
	}
	
	public class InlineTable:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.InlineTable;
		}
		
		public override string Name{
			get{
				return "inline-table";
			}
		}
		
	}
	
	public class InlineListItem:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.InlineListItem;
		}
		
		public override string Name{
			get{
				return "inline-list-item";
			}
		}
		
	}
	
	public class ListItem:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.ListItem;
		}
		
		public override string Name{
			get{
				return "list-item";
			}
		}
		
	}
	
	public class Table:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.Table;
		}
		
		public override string Name{
			get{
				return "table";
			}
		}
		
	}
	
	public class TableRowGroup:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableRowGroup;
		}
		
		public override string Name{
			get{
				return "table-row-group";
			}
		}
		
	}
	
	public class TableHeaderGroup:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableHeaderGroup;
		}
		
		public override string Name{
			get{
				return "table-header-group";
			}
		}
		
	}
	
	public class TableFooterGroup:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableFooterGroup;
		}
		
		public override string Name{
			get{
				return "table-footer-group";
			}
		}
		
	}
	
	public class TableRow:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableRow;
		}
		
		public override string Name{
			get{
				return "table-row";
			}
		}
		
	}
	
	public class TableCell:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableCell;
		}
		
		public override string Name{
			get{
				return "table-cell";
			}
		}
		
	}
	
	public class TableColumnGroup:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableColumnGroup;
		}
		
		public override string Name{
			get{
				return "table-column-group";
			}
		}
		
	}
	
	public class TableColumn:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableColumn;
		}
		
		public override string Name{
			get{
				return "table-column";
			}
		}
		
	}
	
	public class TableCaption:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.TableCaption;
		}
		
		public override string Name{
			get{
				return "table-caption";
			}
		}
		
	}
	
	public class Grid:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.Grid;
		}
		
		public override string Name{
			get{
				return "grid";
			}
		}
		
	}
	
	public class Contents:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.Contents;
		}
		
		public override string Name{
			get{
				return "contents";
			}
		}
		
	}
	
	public class Ruby:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.Ruby;
		}
		
		public override string Name{
			get{
				return "ruby";
			}
		}
		
	}
	
	public class RubyBase:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.RubyBase;
		}
		
		public override string Name{
			get{
				return "ruby-base";
			}
		}
		
	}
	
	public class RubyText:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.RubyText;
		}
		
		public override string Name{
			get{
				return "ruby-text";
			}
		}
		
	}
	
	public class RubyBaseContainer:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.RubyBaseContainer;
		}
		
		public override string Name{
			get{
				return "ruby-base-container";
			}
		}
		
	}
	
	public class RubyTextContainer:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.RubyTextContainer;
		}
		
		public override string Name{
			get{
				return "ruby-text-container";
			}
		}
		
	}
	
	public class FlowRoot:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.InsideFlowRoot;
		}
		
		public override string Name{
			get{
				return "flow-root";
			}
		}
		
	}
	
	public class Flow:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return DisplayMode.InsideFlow;
		}
		
		public override string Name{
			get{
				return "flow";
			}
		}
		
	}
	
}