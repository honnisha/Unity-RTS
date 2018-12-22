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
using System.Collections;
using System.Collections.Generic;


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the counter-style rule. Syntax support only at the moment.
	/// </summary>
	
	public class CounterStyleRule:CssAtRule, Rule{
		
		/// <summary>The style to apply.</summary>
		public Style style;
		/// <summary>Counters name.</summary>
		public string CounterName;
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		/// <summary>The constructed counter system.</summary>
		public Counters.CounterSystem System;
		
		
		public override string[] GetNames(){
			return new string[]{"counter-style"};
		}
		
		public override CssAtRule Copy(){
			CounterStyleRule at=new CounterStyleRule();
			at.style=style;
			at.CounterName=CounterName;
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
				return 11;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
			// Add to available counters:
			if(document.CssCounters==null){
				document.CssCounters=new Dictionary<string,CounterStyleRule>();
			}
			
			if(System==null){
				
				// Build the system now!
				
				// What kind of system is it?
				Css.Value sys=style[Css.Properties.System.GlobalProperty];
				
				Counters.CounterSystem system=null;
				
				// If it's a set with 'extends'..
				if(sys is Css.ValueSet && sys[0].Text=="extends"){
					
					// Get the system and clone it:
					string name=sys[1].Text;
					
					CounterStyleRule rule;
					if( document.CssCounters.TryGetValue(name,out rule) ){
						
						// Get the system:
						system=rule.System;
						
					}else{
						
						// Get by name:
						system=Counters.CounterSystems.Get(name);
						
					}
					
				}else if(sys!=null){
					
					// The name is..
					string name=sys.Text;
					
					// Get by name:
					system=Counters.CounterSystems.Get(name);
					
				}
				
				if(system==null){
					// Decimal is the default:
					system=Counters.CounterSystems.Decimal;
				}
				
				// Clone it:
				System=system.Clone();
				
				// Apply additional rules now!
				System.Load(style,document);
				
			}
			
			// Add it:
			document.CssCounters[CounterName]=this;
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
			CounterStyleRule cs;
			if(document.CssCounters.TryGetValue(CounterName,out cs)){
				
				if(cs==this){
					document.CssCounters.Remove(CounterName);
				}
				
			}
			
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet sheet,Css.Value value){
			
			// Grab the sheet:
			ParentSheet=sheet;
			RawValue=value;
			
			// Read a value:
			Css.Value val=value[1];
			
			// Get the value as constant text:
			CounterName=val.Text;
			
			// Read a block like normal:
			Css.Value blk=value[value.Count-1];
			
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

namespace Css{
	
	public partial class ReflowDocument{
		
		/// <summary>The available counter-style rules.</summary>
		public Dictionary<string,Css.AtRules.CounterStyleRule> CssCounters;
		
		
		/// <summary>Gets the ordinal for a given index.</summary>
		public string GetOrdinal(int index,string name,bool prefixed){
			
			// Try a core counter system:
			Counters.CounterSystem sys=Counters.CounterSystems.Get(name);
			
			if(sys!=null){
				return sys.Get(index,prefixed);
			}
			
			// Get the counter system:
			AtRules.CounterStyleRule rule;
			if( CssCounters!=null && CssCounters.TryGetValue(name,out rule) ){
				
				// Get the system and return the ordinal:
				return rule.System.Get(index,prefixed);
				
			}
			
			// Not found:
			return null;
		}
		
		/// <summary>Gets a named counter system. Null if not found.</summary>
		public Counters.CounterSystem GetCounter(string name){
			
			// Try a core counter system:
			Counters.CounterSystem sys=Counters.CounterSystems.Get(name);
			
			if(sys!=null){
				return sys;
			}
			
			// Get the counter system:
			AtRules.CounterStyleRule rule;
			if( CssCounters!=null && CssCounters.TryGetValue(name,out rule) ){
				
				// Get the system:
				return rule.System;
				
			}
			
			// Not found:
			return null;
		}
		
	}
	
}



