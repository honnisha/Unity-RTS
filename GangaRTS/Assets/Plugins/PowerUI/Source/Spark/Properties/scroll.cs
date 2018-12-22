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
	/// An animatable scroll-top and scroll-left CSS property.
	/// </summary>
	
	public class Scroll:CssProperty{
		
		public static Scroll GlobalProperty;
		public static CssProperty Top;
		public static CssProperty Left;
		
		
		public Scroll(){
			GlobalProperty=this;
			InitialValueText="0 0";
		}
		
		public override string[] GetProperties(){
			return new string[]{"scroll"};
		}
		
		public override void Aliases(){
			Alias("scroll-top",ValueAxis.Y,0);
			Alias("scroll-left",ValueAxis.X,1);
			
			Top=GetAliased(0);
			Left=GetAliased(1);
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Request a shortform redraw:
			style.RequestFastLayout();
			
			// Grab the virtual elements:
			VirtualElements virts=style.RenderData.Virtuals;
			
			// Update them:
			if(virts!=null){
				
				// H:
				HtmlScrollbarElement scroll=virts.Get(ComputedStyle.HorizontalScrollPriority) as HtmlScrollbarElement;
				
				if(scroll!=null){
					scroll.ElementScrolled();
				}
				
				// V:
				scroll=virts.Get(ComputedStyle.VerticalScrollPriority) as HtmlScrollbarElement;
				
				if(scroll!=null){
					scroll.ElementScrolled();
				}
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>Computes the box for the given context now.</summary>
		public BoxStyle Compute(Css.Value value,RenderableData context){
			
			BoxStyle box=new BoxStyle();
			
			if(value!=null){
				
				Css.Value a=value[0];
				
				if(a!=null){
					box.Top=a.GetDecimal(context,Top);
				}
				
				a=value[1];
				
				if(a!=null){
					box.Left=a.GetDecimal(context,Left);
				}
				
			}
			
			return box;
			
		}
		
	}
	
}