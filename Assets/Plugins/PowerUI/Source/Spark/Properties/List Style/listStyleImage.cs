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
using Css.AtRules;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the list-style-image: css property.
	/// </summary>
	
	public class ListStyleImage:CssProperty{
		
		public ListStyleImage(){
			Inherits=true;
		}
		
		public override string[] GetProperties(){
			return new string[]{"list-style-image"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Value could be a function (symbols):
			Css.Functions.SymbolsFunction symbols=(value as Css.Functions.SymbolsFunction);
			
			if(symbols!=null){
				
				// Use symbols.GetAt(..)
				
			}else{
				
				// E.g. the name of a counter-style:
				Dictionary<string,CounterStyleRule> rules=style.RenderData.Document.CssCounters;
				
				if(rules!=null){
					
					CounterStyleRule cs;
					if(rules.TryGetValue(value.Text,out cs)){
						
						// Got the rule set.
						
					}
					
				}
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			uri | none | inherit
			*/
			
			return new Spec.OneOf(
				new Spec.FunctionCall("url"),
				new Spec.Literal("none")
			);
			
		}
		
	}
	
}



