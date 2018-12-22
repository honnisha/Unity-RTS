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
using Css.Counters;


namespace Css.Functions{
	
	/// <summary>
	/// Represents the counter() css function.
	/// </summary>
	
	public class CounterFunction:CssFunction{
		
		/// <summary>The counter name.</summary>
		private string Counter;
		/// <summary>The cached counter system.</summary>
		private CounterSystem System_;
		/// <summary>If defined, the list style.</summary>
		public Css.Value Style{
			get{
				if(Count==0){
					return null;
				}
				return this[1];
			}
		}
		
		
		public CounterFunction(){
			
			Name="counter";
			Type=ValueType.Text;
			
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			
			if(context==null){
				return "";
			}
			
			// Get the document:
			ReflowDocument doc=context.computedStyle.reflowDocument;
			
			if(System_==null){
				
				// Get the system by name:
				Css.Value style=Style;
				
				if(style!=null){
					System_=doc.GetCounter(style.Text);
				}else{
					System_=null;
				}
				
				if(System_==null){
					// Default to decimal:
					System_=CounterSystems.Decimal;
				}
				
			}
			
			return System_.Get(doc.Renderer.GetCounter(Counter),false);
			
		}
		
		public override string[] GetNames(){
			return new string[]{"counter"};
		}
		
		protected override Css.Value Clone(){
			CounterFunction result=new CounterFunction();
			result.Values=CopyInnerValues();
			result.Counter=Counter;
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			if(Count==0){
				return;
			}
			
			Counter=this[0].Text;
			
		}
		
	}
	
}