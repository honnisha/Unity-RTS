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
		/// Sets up the Dom module.
		/// </summary>
		public static void Start_Dom(StartInfo info){
			
			// Add to the scanner:
			Dom.Start.AddToScanner(info.Scanner);
			
			// (That's so it'll "scan" any added assemblies for <elements> etc).
			
		}
		
	}

}

namespace Dom{
	
	/// <summary>
	/// Used to setup custom tags.
	/// </summary>
	public static class Start{
		
		/// <summary>
		/// True if the DOM module has been started for at least one module.
		/// </summary>
		public static bool Started{
			get{
				return MLNamespaces.All!=null && MLNamespaces.All.Count!=0;
			}
		}
		
		/// <summary>
		/// Sets up the DOM engine by scanning "this" assembly for
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
			
			// Clear all namespaces:
			MLNamespaces.All.Clear();
			
		}
		
		/// <summary>
		/// Adds all the type checkers to the given scanner.
		/// They basically automatically find elements etc in assemblies.
		/// </summary>
		public static void AddToScanner(Modular.AssemblyScanner scanner){
			
			// Element:
			scanner.FindAllSubTypes(typeof(Dom.Element),delegate(Type type){
				
				// Add it:
				TagHandlers.Add(type);
				
			});
			
			// TextNode:
			scanner.FindAllSubTypes(typeof(Dom.TextNode),delegate(Type type){
				
				// Add it:
				TagHandlers.Add(type);
				
			});
			
			// ILangNode:
			scanner.FindAllSubTypes(typeof(Dom.ILangNode),delegate(Type type){
				
				// Add it:
				TagHandlers.Add(type);
				
			});
			
		}
		
	}
	
}