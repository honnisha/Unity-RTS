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


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the viewport rule. Syntax support only at the moment.
	/// </summary>
	
	public class ViewportRule:CssAtRule,Rule{
		
		/// <summary>The style to apply.</summary>
		public Style style;
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		public override string[] GetNames(){
			return new string[]{"viewport"};
		}
		
		public override CssAtRule Copy(){
			ViewportRule at=new ViewportRule();
			at.style=style;
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
				return 15;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			
			// Read a block like normal:
			Css.Value blk=value[1];
			
			// Grab it as a block unit:
			SelectorBlockUnit block=blk as SelectorBlockUnit;
			
			if(block==null){
				// Broken :(
				return null;
			}
			
			// Grab the style:
			this.style=block.Style;
			
			return this;
			
		}
		
	}
	
}



