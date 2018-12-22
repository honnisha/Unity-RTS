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


namespace Modular{
	
	public static partial class Main{
		
		/// <summary>
		/// Sets up the spark module.
		/// </summary>
		public static void Start_Spark(StartInfo info){
			
			// Add to the scanner:
			Css.Start.AddToScanner(info.Scanner);
			
			// (That's so it'll "scan" any added assemblies for properties etc).
			
		}
		
	}

}

namespace Css{
	
	/// <summary>
	/// Used to setup CSS properties, units etc.
	/// </summary>
	public static class Start{
		
		/// <summary>
		/// True if Spark has been started for at least one module.
		/// </summary>
		public static bool Started{
			get{
				return CssProperties.All!=null;
			}
		}
		
		/// <summary>
		/// Sets up the spark engine by scanning "this" assembly for
		/// units, properties etc.
		/// </summary>
		public static void Now(){
			
			// Refuse to start if it's already started:
			if(Started){
				return;
			}
			
			// Scan right now (scans the assembly containing the given method):
			Modular.AssemblyScanner.ScanThisNow(AddToScanner);
			
		}
		
		/// <summary>
		/// Clears all the buffers so you can safely call any of the start methods again.
		/// </summary>
		public static void Reset(){
			
			// Clear properties:
			CssFunctions.All=null;
			CssAtRules.All=null;
			CssUnits.AllStart=null;
			CssUnits.AllEnd=null;
			CssKeywords.All=null;
			CssProperties.All=null;
			
		}
		
		/// <summary>
		/// Adds all the type checkers to the given scanner.
		/// They basically automatically find CSS properties, units etc in assemblies.
		/// </summary>
		public static void AddToScanner(Modular.AssemblyScanner scanner){
			
			// CSS functions:
			scanner.FindAllSubTypes(typeof(CssFunction),delegate(Type type){
				
				// Add it:
				CssFunctions.Add(type);
				
			});
			
			// CSS at rules:
			scanner.FindAllSubTypes(typeof(CssAtRule),delegate(Type type){
				
				// Add it:
				CssAtRules.Add(type);
				
			});
			
			// CSS units:
			scanner.FindAllSubTypes(typeof(CssUnit),delegate(Type type){
				
				// Add it:
				CssUnits.Add(type);
				
			});
			
			// CSS keywords:
			scanner.FindAllSubTypes(typeof(CssKeyword),delegate(Type type){
				
				// Add it:
				CssKeywords.Add(type);
				
			});
			
			// CSS properties (secondary pass; requires default values which can be any of the above):
			scanner.FindAllSubTypes(1,typeof(CssProperty),delegate(Type type){
				
				// Add it:
				CssProperties.Add(type);
				
			});
			
		}
		
	}
	
}