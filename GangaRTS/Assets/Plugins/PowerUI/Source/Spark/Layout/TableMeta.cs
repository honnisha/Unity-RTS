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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Css;
using Blaze;
using Dom;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Stores information when in display:table/ table-cell mode.
	/// </summary>
	public class TableMeta{
		
		/// <summary>True when cols have been recalced.</summary>
		public bool ColumnsReady;
		/// <summary>The number of columns.</summary>
		public int ColumnCount;
		/// <summary>Index of the first column (used by table-cell).</summary>
		public int StartColumn;
		/// <summary>The host table.</summary>
		public TableMeta Table;
		/// <summary>Column metadata.</summary>
		internal List<TableColumnMeta> Columns;
		
		
		private void TryHandleRowC1(NodeList rowKids,ref bool twoPassesRequired){
			
			List<TableColumnMeta> col=Columns;
			int colIndex=0;
			
			for(int c=0;c<rowKids.length;c++){
				
				// Get the cell:
				Node cellNode=rowKids[c];
				
				// Get its RenderData:
				RenderableData rd=(cellNode as IRenderableNode).RenderData;
				
				LayoutBox tlb=rd.FirstBox;
				
				if(tlb==null){
					continue;
				}
				
				TableMeta cell=tlb.TableMeta;
				
				if(cell==null){
					// Not a table-cell.
					return;
				}
				
				if(colIndex>=ColumnCount){
					
					// Update column count:
					ColumnCount=colIndex+1;
					
				}
				
				if(ColumnCount>col.Count){
					
					// Add the column(s):
					for(int i=col.Count;i<ColumnCount;i++){
						col.Add(new TableColumnMeta());
					}
					
				}
				
				cell.StartColumn=colIndex;
				cell.ColumnCount=1;
				cell.Table=this;
				colIndex++;
				
				if(cell.ColumnCount != 1){
					twoPassesRequired=true;
					continue;
				}
				
				TableColumnMeta column = col[cell.StartColumn];
				
				if (tlb.PositionMode != PositionMode.Absolute &&
					tlb.PositionMode != PositionMode.Fixed){
					
					column.Positioned=false;
					
				}
				
				// Get the cells width:
				Css.Value widthValue=rd.computedStyle.WidthX;
				
				// Is the width value set?
				bool isSet=(widthValue!=null && !widthValue.IsAuto);
				
				// Fixed width takes priority over any other width type:
				if (column.Type != TableColumnMeta.COLUMN_WIDTH_FIXED &&
						isSet && widthValue.Type != Css.ValueType.RelativeNumber){
					
					// Update the type:
					column.Type = TableColumnMeta.COLUMN_WIDTH_FIXED;
					
					// Get the initial width:
					float width = widthValue.GetDecimal(rd,Css.Properties.Width.GlobalProperty);
					
					if(width<0f){
						width=0f;
					}
					
					column.Width=width;
					
					continue;
					
				}
				
				if (column.Type != TableColumnMeta.COLUMN_WIDTH_UNKNOWN)
					continue;
				
				// Percent next:
				if (isSet && widthValue.Type == Css.ValueType.RelativeNumber){
					
					// It's a %:
					column.Type = TableColumnMeta.COLUMN_WIDTH_PERCENT;
					
					// Get the initial width:
					float width = widthValue.GetRawDecimal();
					
					if(width<0f){
						width=0f;
					}
					
					column.Width=width;
					
				}else if(!isSet){
					column.Type = TableColumnMeta.COLUMN_WIDTH_AUTO;
				}
				
			}
		}
		
		/// <summary>Attempts to handle all colspan 2+ cells.</summary>
		private void TryHandleRowC2(NodeList rowKids){
			
			List<TableColumnMeta> col=Columns;
			
			for(int c=0;c<rowKids.length;c++){
				
				// Get the cell:
				Node cellNode=rowKids[c];
				
				// Get its RenderData:
				RenderableData rd=(cellNode as IRenderableNode).RenderData;
				
				LayoutBox tlb=rd.FirstBox;
				
				if(tlb==null){
					continue;
				}
				
				TableMeta cell=tlb.TableMeta;
				
				if(cell==null){
					// Not a cell!
					return;
				}
				
				if(cell.ColumnCount==1){
					continue;
				}
				
				int fixed_columns = 0;
				int percent_columns = 0;
				int auto_columns = 0;
				int unknown_columns = 0;
				float fixed_width = 0f;
				float percent_width = 0f;
				
				int i = cell.StartColumn;
				
				for(int j=i;j<i+cell.ColumnCount;j++){
					col[j].Positioned=false;
				}
				
				// Count column types in spanned cells:
				for(int j=0;j<cell.ColumnCount;j++){
					
					TableColumnMeta column = col[i + j];
			
					if(column.Type == TableColumnMeta.COLUMN_WIDTH_FIXED){
						fixed_width += column.Width;
						fixed_columns++;
					}else if(column.Type == TableColumnMeta.COLUMN_WIDTH_PERCENT){
						percent_width += column.Width;
						percent_columns++;
					}else if(column.Type == TableColumnMeta.COLUMN_WIDTH_AUTO){
						auto_columns++;
					}else{
						unknown_columns++;
					}
					
				}
				
				if(unknown_columns==0){
					continue;
				}
				
				// Get the cells width:
				Css.Value widthValue=rd.computedStyle.WidthX;
				
				// Is the width value set?
				bool isSet=(widthValue!=null && !widthValue.IsAuto);
				
				// If cell is fixed width, and all spanned columns are fixed
				// or unknown width, split extra width among unknown columns
				if(isSet && widthValue.Type != Css.ValueType.RelativeNumber &&
						fixed_columns + unknown_columns == cell.ColumnCount){
					
					// Get the initial width:
					float width = (
						widthValue.GetDecimal(rd,Css.Properties.Width.GlobalProperty) - fixed_width
					) / unknown_columns;
					
					if(width<0f){
						width=0f;
					}
					
					for(int j=0;j<cell.ColumnCount;j++){
						
						TableColumnMeta column = col[i + j];
						
						if(column.Type == TableColumnMeta.COLUMN_WIDTH_UNKNOWN){
							column.Type = TableColumnMeta.COLUMN_WIDTH_FIXED;
							column.Width = width;
						}
						
					}
					
				}
				
				// As above for percentage width
				if(isSet && widthValue.Type == Css.ValueType.RelativeNumber &&
						percent_columns + unknown_columns == cell.ColumnCount){
					
					// Get the initial width:
					float width = (widthValue.GetRawDecimal() - percent_width) / unknown_columns;
					
					if(width<0f){
						width=0f;
					}
					
					for(int j=0;j<cell.ColumnCount;j++){
						
						TableColumnMeta column = col[i + j];
						
						if(column.Type==TableColumnMeta.COLUMN_WIDTH_UNKNOWN){
							column.Type=TableColumnMeta.COLUMN_WIDTH_PERCENT;
							column.Width=width;
						}
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Determines the columns for a table.</summary>
		public void CalculateColumns(RenderableData host){
			
			if(ColumnsReady){
				// Columns already constructed; e.g. frameset table
				return;
			}
			
			ColumnsReady=true;
			
			if(Columns==null){
				
				// Create set:
				Columns = new List<TableColumnMeta>();
				
			}else{
				
				// Reset:
				for(int i=0;i<Columns.Count;i++){
					
					// Set defaults:
					TableColumnMeta col=Columns[i];
					col.Width=0f;
					col.Positioned=false;
					col.Type=TableColumnMeta.COLUMN_WIDTH_UNKNOWN;
					
				}
				
				
			}
			
			// Get tables kids:
			NodeList kids=host.Node.childNodes_;
			
			if(kids==null){
				return;
			}
			
			bool twoPassesRequired=false;
			
			// 1st pass: cells with colspan of 1 only:
			for(int g=0;g<kids.length;g++){
				
				// Get the group (a tbody, for example):
				Node rowGroup=kids[g];
				
				NodeList groupKids=rowGroup.childNodes_;
				
				if(groupKids==null){
					continue;
				}
				
				// Check if it's visible:
				
				// Get its RenderData:
				RenderableData rd=(rowGroup as IRenderableNode).RenderData;
				
				LayoutBox tlb=rd.FirstBox;
				
				if(tlb==null){
					// Hidden.
					continue;
				}
				
				// Check if it's actually a group:
				TableMeta cell=tlb.TableMeta;
				
				if(cell!=null){
					
					// It's actually a set of cells:
					TryHandleRowC1(kids,ref twoPassesRequired);
					break;
					
				}
				
				for(int r=0;r<groupKids.length;r++){
					
					// Get the row:
					Node row=groupKids[r];
					
					NodeList rowKids=row.childNodes_;
					
					if(rowKids==null){
						continue;
					}
					
					// Get its RenderData:
					rd=(row as IRenderableNode).RenderData;
					
					tlb=rd.FirstBox;
					
					if(tlb==null){
						// Hidden.
						continue;
					}
					
					cell=tlb.TableMeta;
					
					if(cell!=null){
						
						// It's actually a set of cells:
						TryHandleRowC1(groupKids,ref twoPassesRequired);
						break;
						
					}
					
					TryHandleRowC1(rowKids,ref twoPassesRequired);
					
				}
			
			}
			
			if(twoPassesRequired){
				
				// 2nd pass: Cells which span multiple columns
				for(int g=0;g<kids.length;g++){
					
					// Get the group (a tbody, for example):
					Node rowGroup=kids[g];
					
					NodeList groupKids=rowGroup.childNodes_;
					
					if(groupKids==null){
						continue;
					}
					
					// Check if it's visible:
					
					// Get its RenderData:
					RenderableData rd=(rowGroup as IRenderableNode).RenderData;
					
					LayoutBox tlb=rd.FirstBox;
					
					if(tlb==null){
						// Hidden.
						continue;
					}
					
					// Check if it's actually a group:
					TableMeta cell=tlb.TableMeta;
					
					if(cell!=null){
						
						// It's actually a set of cells:
						TryHandleRowC2(kids);
						break;
						
					}
					
					for(int r=0;r<groupKids.length;r++){
						
						// Get the row:
						Node row=groupKids[r];
						
						NodeList rowKids=row.childNodes_;
						
						if(rowKids==null){
							continue;
						}
						
						// Get its RenderData:
						rd=(row as IRenderableNode).RenderData;
						
						tlb=rd.FirstBox;
						
						if(tlb==null){
							// Hidden.
							continue;
						}
						
						cell=tlb.TableMeta;
						
						if(cell!=null){
							
							// It's actually a set of cells:
							TryHandleRowC2(groupKids);
							break;
							
						}
						
						TryHandleRowC2(rowKids);
						
					}
					
				}
				
			}
			
			// Use AUTO if no width type was specified.
			// (Use COLUMN_WIDTH_UNK_OR_AUTO)
			
			// If the table has a defined width then we base it on that.
			// If not, then we have to find the shrink-to-fit width of each cell and use that.
			// Shrink to fit is slow and unreliable, so we'll assume that a width is always defined here.
			float width=host.FirstBox.InnerWidth;
			float totalUsedWidth=0f;
			int auto=0;
			
			//    I.e. take the width and divide it by x auto columns.
			for(int i=0;i<Columns.Count;i++){
				
				TableColumnMeta tcm=Columns[i];
				
				// Resolve % now:
				if(tcm.Type==TableColumnMeta.COLUMN_WIDTH_PERCENT){
					tcm.Width*=width;
					tcm.Type=TableColumnMeta.COLUMN_WIDTH_FIXED;
					totalUsedWidth+=tcm.Width;
				
				// Track px fields:
				}else if(tcm.Type==TableColumnMeta.COLUMN_WIDTH_FIXED){
					totalUsedWidth+=tcm.Width;
				
				// Anything else is auto:
				}else{
					// Auto present!
					auto++;
				}
				
			}
			
			if(auto!=0){
				
				// Divide non-allocated space by the number of auto columns:
				width=(width-totalUsedWidth) / (float)auto;
				
				// Apply to each one:
				for(int i=0;i<Columns.Count;i++){
					
					TableColumnMeta tcm=Columns[i];
					
					if((tcm.Type & TableColumnMeta.COLUMN_WIDTH_UNK_OR_AUTO)!=0){
						
						// Apply width now:
						tcm.Width=width;
						tcm.Type=TableColumnMeta.COLUMN_WIDTH_FIXED;
						
					}
					
				}
				
			}
			
		}

		
	}
	
	/// <summary>Stores information about table columns for display:table.</summary>
	public class TableColumnMeta{
		
		/// <summary>Unknown or auto.</summary>
		public const int COLUMN_WIDTH_UNK_OR_AUTO=COLUMN_WIDTH_UNKNOWN | COLUMN_WIDTH_AUTO;
		
		/// <summary>Table column types.</summary>
		public const int COLUMN_WIDTH_UNKNOWN=1;
		public const int COLUMN_WIDTH_AUTO=2;
		public const int COLUMN_WIDTH_PERCENT=4;
		public const int COLUMN_WIDTH_FIXED=8;
		
		/// <summary>The width of this column.</summary>
		public float Width=0f;
		/// <summary>Is this column positioned?</summary>
		public bool Positioned=true;
		/// <summary>The type of this column.</summary>
		public int Type=COLUMN_WIDTH_UNKNOWN; 
		
	}
	
}