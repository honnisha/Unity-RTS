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

namespace Css.Counters{
	
	/// <summary>
	/// Represents a counter system.
	/// </summary>
	
	public static class CounterSystems{
		
		/// <summary>A decimal counter system (the default fallback).</summary>
		private static DecimalSystem Decimal_;
		/// <summary>A decimal counter system (the default fallback).</summary>
		public static DecimalSystem Decimal{
			get{
				if(Decimal_==null){
					Decimal_=new DecimalSystem();
				}
				
				return Decimal_;
			}
		}
		
		/// <summary>A cyclic counter system.</summary>
		private static CyclicSystem Cyclic_;
		/// <summary>A cyclic counter system.</summary>
		public static CyclicSystem Cyclic{
			get{
				if(Cyclic_==null){
					Cyclic_=new CyclicSystem();
				}
				
				return Cyclic_;
			}
		}
		
		/// <summary>A fixed counter system.</summary>
		private static FixedSystem Fixed_;
		/// <summary>A fixed counter system.</summary>
		public static FixedSystem Fixed{
			get{
				if(Fixed_==null){
					Fixed_=new FixedSystem();
				}
				
				return Fixed_;
			}
		}
		
		/// <summary>A symbolic counter system.</summary>
		private static SymbolicSystem Symbolic_;
		/// <summary>A symbolic counter system.</summary>
		public static SymbolicSystem Symbolic{
			get{
				if(Symbolic_==null){
					Symbolic_=new SymbolicSystem();
				}
				
				return Symbolic_;
			}
		}
		
		/// <summary>An alphabetic counter system.</summary>
		private static AlphabeticSystem Alphabetic_;
		/// <summary>An alphabetic counter system.</summary>
		public static AlphabeticSystem Alphabetic{
			get{
				if(Alphabetic_==null){
					Alphabetic_=new AlphabeticSystem();
				}
				
				return Alphabetic_;
			}
		}
		
		/// <summary>A numeric counter system.</summary>
		private static NumericSystem Numeric_;
		/// <summary>A numeric counter system.</summary>
		public static NumericSystem Numeric{
			get{
				if(Numeric_==null){
					Numeric_=new NumericSystem();
				}
				
				return Numeric_;
			}
		}
		
		/// <summary>An additive counter system.</summary>
		private static AdditiveSystem Additive_;
		/// <summary>An additive counter system.</summary>
		public static AdditiveSystem Additive{
			get{
				if(Additive_==null){
					Additive_=new AdditiveSystem();
				}
				
				return Additive_;
			}
		}
		
		/// <summary>Gets a global counter system by the name as used by the 'system' descriptor (css property).</summary>
		public static CounterSystem Get(string name){
			
			switch(name){
				case "cyclic":
					return Cyclic;
				case "numeric":
					return Numeric;
				case "alphabetic":
					return Alphabetic;
				case "symbolic":
					return Symbolic;
				case "additive":
					return Additive;
				case "fixed":
					return Fixed;
				case "decimal":
					return Decimal;
			}
			
			return null;
		}
		
	}
	
}