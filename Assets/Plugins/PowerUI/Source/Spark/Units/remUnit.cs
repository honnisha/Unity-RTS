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


namespace Css.Units{
	
	/// <summary>
	/// Represents an instance of rem units.
	/// </summary>
	
	public class RemUnit:FontUnit{
		
		public override string ToString(){
			return RawValue+"rem";
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			return RawValue * GetFontSize((context.Document.documentElement as IRenderableNode).RenderData,property) * context.ValueScaleRelative;
			
		}
		
		protected override Value Clone(){
			RemUnit result=new RemUnit();
			result.RawValue=RawValue;
			return result;
		}
		
		public override string[] PostText{
			get{
				return new string[]{"rem"};
			}
		}
		
	}
	
}



