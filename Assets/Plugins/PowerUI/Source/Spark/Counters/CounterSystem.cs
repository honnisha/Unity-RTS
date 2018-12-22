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
using Css.AtRules;


namespace Css.Counters{
	
	/// <summary>
	/// Represents a counter system.
	/// </summary>
	
	public class CounterSystem{
		
		public CounterSystem Fallback;
		/// <summary>A suffix to add to all numbers.</summary>
		public string Suffix=". ";
		/// <summary>A prefix to add to all numbers.</summary>
		public string Prefix="";
		/// <summary>A prefix for negative numbers.</summary>
		public string NegativePrefix="-";
		/// <summary>A suffix for negative numbers (used in financial contexts).</summary>
		public string NegativeSuffix="";
		/// <summary>Overall range min.</summary>
		public int Min=int.MinValue;
		/// <summary>Overall range max.</summary>
		public int Max=int.MaxValue;
		/// <summary>Min length for padding.</summary>
		public int PadMin;
		/// <summary>The padding symbol.</summary>
		public string PadSymbol="";
		/// <summary>Additional ranges. These are always between Min and Max.</summary>
		public CounterRange[] AdditionalRanges;
		/// <summary>A shared string builder.</summary>
		protected System.Text.StringBuilder Builder = new System.Text.StringBuilder();
		
		
		public CounterSystem(){
			if(this is DecimalSystem){
				return;
			}
			
			Fallback=CounterSystems.Decimal;
		}
		
		/// <summary>Builds a string symbol set from the given 'symbols' value.</summary>
		public string[] SymbolSet(Css.Style style){
			
			// Symbols:
			Css.Value value=style[Css.Properties.Symbols.GlobalProperty];
			
			if(value==null){
				return null;
			}
			
			string[] set=new string[value.Count];
			
			for(int i=0;i<value.Count;i++){
				set[i]=value[i].Text;
			}
			
			return set;
		}
		
		/// <summary>Loads this counter systems symbols.</summary>
		protected virtual void LoadSymbols(Css.Style style){
			
		}
		
		/// <summary>Loads this systems values from the given style.</summary>
		public void Load(Css.Style style,Css.ReflowDocument document){
			
			// Get the range:
			Css.Value range=style[Css.Properties.RangeProperty.GlobalProperty];
			
			if(range!=null && !range.IsAuto){
				
				Css.ValueSet rangeSet=range as Css.ValueSet;
				
				int rangeSize=(rangeSet!=null && rangeSet.Spacer==",")?range.Count : 1;
				
				if(rangeSize>1){
					AdditionalRanges=new CounterRange[rangeSize];
					
					int overallMin=0;
					int overallMax=0;
					
					// For each one..
					for(int i=0;i<rangeSize;i++){
						
						int min=range[i][0].GetInteger(null,null);
						int max=range[i][1].GetInteger(null,null);
						
						if(i==0){
							overallMin=min;
							overallMax=max;
						}else{
							
							if(min<overallMin){
								overallMin=min;
							}
							
							if(max>overallMax){
								overallMax=max;
							}
							
						}
						
						AdditionalRanges[i]=new CounterRange(min,max);
						
					}
					
					Min=overallMin;
					Max=overallMax;
					
				}else{
					AdditionalRanges=null;
					
					// Set min/max:
					Min=range[0].GetInteger(null,null);
					Max=range[1].GetInteger(null,null);
				}
				
			}
			
			// Get the pad:
			Css.Value pad=style[Css.Properties.Pad.GlobalProperty];
			
			if(pad!=null){
				
				PadMin=pad[0].GetInteger(null,null);
				
				if(pad is Css.ValueSet){
					PadSymbol=pad[1].Text;
				}else{
					PadSymbol="";
				}
				
			}
			
			// Prefix:
			Css.Value prefix=style[Css.Properties.Prefix.GlobalProperty];
			
			if(prefix!=null){
				Prefix=prefix.Text;
			}
			
			// Suffix:
			Css.Value suffix=style[Css.Properties.Suffix.GlobalProperty];
			
			if(suffix!=null){
				Suffix=suffix.Text;
			}
			
			// Negative:
			Css.Value negative=style[Css.Properties.Negative.GlobalProperty];
			
			if(negative!=null){
				
				NegativePrefix=negative[0].Text;
				
				if(negative is Css.ValueSet){
					NegativeSuffix=negative[1].Text;
				}else{
					NegativeSuffix="";
				}
				
			}
			
			// Symbols:
			LoadSymbols(style);
			
			// Fallback:
			Css.Value fallback=style[Css.Properties.Fallback.GlobalProperty];
			
			if(fallback!=null){
				
				string fbName=fallback[0].Text;
				
				if(fbName=="none" || fbName=="decimal"){
					fbName=null;
				}	
				
				if(fbName!=null){
					
					// Get by built in name:
					Fallback=Counters.CounterSystems.Get(fbName);
					
					if(Fallback==null){
						
						// Get by doc name:
						CounterStyleRule rule;
						if( document.CssCounters.TryGetValue(fbName,out rule) ){
							
							// Get the system:
							Fallback=rule.System;
							
						}
						
						
					}
					
				}
				
				if(Fallback==null){
					// Default to decimal system:
					Fallback=CounterSystems.Decimal;
				}
				
			}
			
			
		}
		
		/// <summary>Gets the counter text for the given value using this system.</summary>
		/// <param name='prefixed'>True if the prefix/ suffix should be added.</param>
		public string Get(int value,bool prefixed){
			
			// If the value is out of range, fallback:
			if(value<Min || value>Max){
				
				// Try fallback:
				if(Fallback==null){
					return "";
				}
				
				return Fallback.Get(value,prefixed);
				
			}else if(AdditionalRanges!=null){
				
				// Must fall in one of them (there won't be many):
				bool inRange=false;
				
				for(int i=0;i<AdditionalRanges.Length;i++){
					
					CounterRange range=AdditionalRanges[i];
					
					if(value>=range.Min && value<=range.Max){
						inRange=true;
						break;
					}
					
				}
				
				if(!inRange){
					
					// Try fallback:
					if(Fallback==null){
						return "";
					}
					
					return Fallback.Get(value,prefixed);
					
				}
				
			}
			
			bool negative=false;
			
			if(value<0){
				// Ignore the negative:
				value=-value;
				negative=true;
			}
			
			// Get it:
			string result=GetPositive(value);
			
			if(result==null){
				// Try fallback:
				if(Fallback==null){
					return "";
				}
				
				return Fallback.Get(value,prefixed);
			}
			
			if(result.Length<PadMin){
				
				// Pad it:
				for(int i=result.Length;i<PadMin;i++){
					
					// Prepend:
					result=PadSymbol+result;
					
				}
				
			}
			
			if(negative){
				// Add the negative pref/suf:
				result=NegativePrefix + result + NegativeSuffix;
			}
			
			if(!prefixed){
				// Don't add prefix/ suffix.
				return result;
			}
			
			return Prefix+result+Suffix;
		}
		
		/// <summary>Gets the given number as handled by this counter system.</summary>
		protected virtual string GetPositive(int value){
			return "";
		}
		
		protected string Build(){
			string result=Builder.ToString();
			Builder.Length=0;
			return result;
		}
		
		public virtual CounterSystem Clone(){
			// Create the instance:
			CounterSystem cs=Activator.CreateInstance(GetType()) as CounterSystem;
			
			// Copy the core values:
			cs.Fallback=Fallback;
			cs.Suffix=Suffix;
			cs.Prefix=Prefix;
			cs.NegativePrefix=NegativePrefix;
			cs.NegativeSuffix=NegativeSuffix;
			cs.Min=Min;
			cs.Max=Max;
			cs.PadMin=PadMin;
			cs.PadSymbol=PadSymbol;
			
			if(AdditionalRanges!=null){
				cs.AdditionalRanges=AdditionalRanges.Clone() as CounterRange[];
			}
			
			return cs;
		}
		
	}
	
	/// <summary>The cyclic counter system.</summary>
	public class CyclicSystem : CounterSystem{
		
		public string[] Symbols;
		
		
		public CyclicSystem(){
		}
		
		/// <summary>Loads this counter systems symbols.</summary>
		protected override void LoadSymbols(Css.Style style){
			
			string[] symbols=SymbolSet(style);
			
			if(Symbols!=null && symbols==null){
				return;
			}
			
			Symbols=symbols;
			
		}
		
		/// <summary>Gets the given number as handled by this counter system.</summary>
		protected override string GetPositive(int value){
			return Symbols[value % Symbols.Length];
		}
		
		public override CounterSystem Clone(){
			CyclicSystem cs=base.Clone() as CyclicSystem;
			
			if(Symbols!=null){
				cs.Symbols=Symbols.Clone() as string[];
			}
			
			return cs;
		}
		
	}
	
	/// <summary>The fixed counter system.</summary>
	public class FixedSystem : CounterSystem{
		
		public string[] Symbols;
		
		
		/// <summary>Loads this counter systems symbols.</summary>
		protected override void LoadSymbols(Css.Style style){
			
			string[] symbols=SymbolSet(style);
			
			if(Symbols!=null && symbols==null){
				return;
			}
			
			Symbols=symbols;
			
		}
		
		/// <summary>Gets the given number as handled by this counter system.</summary>
		protected override string GetPositive(int value){
			
			if(value>=Symbols.Length){
				// Fallback counter system:
				return null;
			}
			
			return Symbols[value];
		}
		
		public override CounterSystem Clone(){
			FixedSystem cs=base.Clone() as FixedSystem;
			
			if(Symbols!=null){
				cs.Symbols=Symbols.Clone() as string[];
			}
			
			return cs;
		}
		
	}
	
	/// <summary>The symbolic counter system.</summary>
	public class SymbolicSystem : CounterSystem{
		
		public string[] Symbols;
		
		
		public SymbolicSystem(){
			Min=1;
		}
		
		/// <summary>Loads this counter systems symbols.</summary>
		protected override void LoadSymbols(Css.Style style){
			
			string[] symbols=SymbolSet(style);
			
			if(Symbols!=null && symbols==null){
				return;
			}
			
			Symbols=symbols;
			
		}
		
		/// <summary>Gets the given number as handled by this counter system.</summary>
		protected override string GetPositive(int value){
			
			// How many times to repeat the character?
			int repetitionCount=(value/Symbols.Length);
			
			// Offset index:
			value-=repetitionCount * Symbols.Length;
			
			// Repeat it repCount+1 times:
			for(int i=0;i<=repetitionCount;i++){
				Builder.Append(Symbols[value]);
			}
			
			return Build();
		}
		
		public override CounterSystem Clone(){
			SymbolicSystem cs=base.Clone() as SymbolicSystem;
			
			if(Symbols!=null){
				cs.Symbols=Symbols.Clone() as string[];
			}
			
			return cs;
		}
		
	}
	
	/// <summary>The alphabetic counter system.</summary>
	public class AlphabeticSystem : CounterSystem{
		
		public string[] Symbols;
		
		
		public AlphabeticSystem(){
			Min=1;
		}
		
		/// <summary>Loads this counter systems symbols.</summary>
		protected override void LoadSymbols(Css.Style style){
			
			string[] symbols=SymbolSet(style);
			
			if(Symbols!=null && symbols==null){
				return;
			}
			
			Symbols=symbols;
			
		}
		
		/// <summary>Gets the given number as handled by this counter system.</summary>
		protected override string GetPositive(int value){
			
			while(value > 0){
				value--;
				int remainder = value % Symbols.Length;
				Builder.Insert(0,Symbols[remainder]);
				value = value / Symbols.Length;
			}
			
			return Build();
		}
		
		public override CounterSystem Clone(){
			AlphabeticSystem cs=base.Clone() as AlphabeticSystem;
			
			if(Symbols!=null){
				cs.Symbols=Symbols.Clone() as string[];
			}
			
			return cs;
		}
		
	}
	
	/// <summary>The numeric counter system.</summary>
	public class NumericSystem : CounterSystem{
		
		public string[] Symbols;
		
		
		public NumericSystem(){
		}
		
		/// <summary>Loads this counter systems symbols.</summary>
		protected override void LoadSymbols(Css.Style style){
			
			string[] symbols=SymbolSet(style);
			
			if(Symbols!=null && symbols==null){
				return;
			}
			
			Symbols=symbols;
			
		}
		
		/// <summary>Gets the given number as handled by this counter system.</summary>
		protected override string GetPositive(int value){
			
			if(value==0){
				return Symbols[0];
			}
			
			while(value > 0){
				int remainder = value % Symbols.Length;
				Builder.Insert(0,Symbols[remainder]);
				value = value / Symbols.Length;
			}
			
			return Build();
		}
		
		public override CounterSystem Clone(){
			NumericSystem cs=base.Clone() as NumericSystem;
			
			if(Symbols!=null){
				cs.Symbols=Symbols.Clone() as string[];
			}
			
			return cs;
		}
		
	}
	
	/// <summary>An additive tuple.</summary>
	public struct AdditiveTuple{
		public string Symbol;
		public int Weight;
		
		public AdditiveTuple(string symbol,int weight){
			Symbol=symbol;
			Weight=weight;
		}
	}
	
	/// <summary>A range for counter values.</summary>
	public struct CounterRange{
		public int Min;
		public int Max;
		
		public CounterRange(int min,int max){
			Min=min;
			Max=max;
		}
	}
	
	/// <summary>The additive counter system.</summary>
	public class AdditiveSystem : CounterSystem{
		
		/// <summary>Symbols sorted in descending order of weight.</summary>
		public AdditiveTuple[] Symbols;
		
		
		public AdditiveSystem(){
			Min=0;
		}
		
		/// <summary>Loads this counter systems symbols.</summary>
		protected override void LoadSymbols(Css.Style style){
			
			// Get a set:
			Css.Value set=style[Css.Properties.AdditiveSymbols.GlobalProperty];
			
			if(set==null){
				return;
			}
			
			Symbols=new AdditiveTuple[set.Count];
			
			for(int i=0;i<Symbols.Length;i++){
				
				// Get both values:
				Css.Value a=set[i][0];
				Css.Value b=set[i][1];
				
				// Allow them to be defined either way around:
				int weight;
				string symbol;
				
				if(b is Css.Units.DecimalUnit){
					weight=b.GetInteger(null,null);
					symbol=a.Text;
				}else{
					weight=a.GetInteger(null,null);
					symbol=b.Text;
				}
				
				Symbols[i]=new AdditiveTuple(symbol,weight);
				
			}
			
		}
		
		/// <summary>Gets the given number as handled by this counter system.</summary>
		protected override string GetPositive(int value){
			
			if(value==0){
				
				// Got a tuple of weight 0? It'll always be the one at the end:
				AdditiveTuple lastTuple=Symbols[Symbols.Length-1];
				
				if(lastTuple.Weight==0){
					return lastTuple.Symbol;
				}
				
				return "";
			}
			
			// Index of the current tuple:
			int currentTuple=-1;
			int max=Symbols.Length-1;
			
			while(value>0 && currentTuple<max){
				
				// Pop a symbol from the list:
				currentTuple++;
				
				// Append the current symbol this many times:
				int repeatCount=value/Symbols[currentTuple].Weight;
				
				for(int i=0;i<repeatCount;i++){
					// Append:
					Builder.Append(Symbols[currentTuple].Symbol);
				}
				
				// Decrement value:
				value-=repeatCount * Symbols[currentTuple].Weight;
				
			}
			
			if(value!=0){
				// Can't actually be represented by this counter style.
				Builder.Length=0;
				
				// Fallback:
				return null;
			}
			
			return Build();
		}
		
		public override CounterSystem Clone(){
			AdditiveSystem cs=base.Clone() as AdditiveSystem;
			
			if(Symbols!=null){
				cs.Symbols=Symbols.Clone() as AdditiveTuple[];
			}
			
			return cs;
		}
		
	}
	
}