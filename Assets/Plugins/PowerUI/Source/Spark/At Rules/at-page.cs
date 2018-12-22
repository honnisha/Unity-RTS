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
using Css.Units;
using Dom;


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the at page rule.
	/// </summary>
	
	public class PageRule:CssAtRule, Rule{
		
		/// <summary>The declared style.</summary>
		public Style style;
		/// <summary>The document containing this font-face rule.</summary>
		public Document Document{
			get{
				return ParentSheet.document;
			}
		}
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		
		public override string[] GetNames(){
			return new string[]{"page"};
		}
		
		public override CssAtRule Copy(){
			PageRule at=new PageRule();
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
		/// <summary>The CSS text of this rule.</summary>
		public string cssText{
			get{
				return RawValue.ToString();
			}
			set{
				throw new NotImplementedException("cssText is read-only on rules. Set it for a whole sheet instead.");
			}
		}
		
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet parentStyleSheet{
			get{
				return ParentSheet;
			}
		}
		
		/// <summary>Rule type.</summary>
		public int type{
			get{
				return 6;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet sheet,Css.Value value){
			
			// Grab the sheet:
			ParentSheet=sheet;
			RawValue=value;
			
			// Simply read a block like normal:
			Css.Value blk=value[value.Count-1];
			
			// Grab it as a block unit:
			SelectorBlockUnit block=blk as SelectorBlockUnit;
			
			if(block==null){
				// Broken page :(
				return null;
			}
			
			// Grab the style:
			this.style=block.Style;
			
			
			
			return this;
			
		}
		
	}
	
}



