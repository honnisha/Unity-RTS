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
	/// Represents the counters() css function.
	/// </summary>
	
	public class CountersFunction:CssFunction{
		
		/// <summary>The counter name.</summary>
		private string Counter;
		/// <summary>The cached counter system.</summary>
		private CounterSystem System_;
		/// <summary>If defined, the list style.</summary>
		public Css.Value Style{
			get{
				if(Count<3){
					return null;
				}
				return this[2];
			}
		}
		
		/// <summary>If defined, the separator.</summary>
		private string Separator="";
		
		
		public CountersFunction(){
			
			Name="counters";
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
			
			// Get all counters from the system and join them:
			List<CssCounter> counters=doc.Renderer.Counters;
			
			if(counters==null){
				return "";
			}
			
			// Go forward here:
			string result="";
			
			for(int i=0;i<counters.Count;i++){
				
				if(counters[i].Name==Counter){
					
					// Got one! Add it on:
					if(result!=""){
						result+=Separator;
					}
					
					result+=System_.Get(counters[i].Count,false);
					
				}
				
			}
			
			return result;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"counters"};
		}
		
		protected override Css.Value Clone(){
			CountersFunction result=new CountersFunction();
			result.Values=CopyInnerValues();
			result.Counter=Counter;
			result.Separator=Separator;
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			if(Count==0){
				return;
			}
			
			Counter=this[0].Text;
			
			if(Count==1){
				return;
			}
			
			// Separator:
			Separator=this[1].Text;
			
		}
		
	}
	
}