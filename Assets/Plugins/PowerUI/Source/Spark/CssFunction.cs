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
using PowerUI;


namespace Css{
	
	/// <summary>
	/// A CSS function. You can create custom ones by deriving from this class.
	/// Note that they are instanced globally as well as locally.
	/// </summary>
	
	[Values.Preserve]
	public class CssFunction:ValueSet{
		
		/// <summary>True if this functions value should be read literally.</summary>
		public bool LiteralValue;
		/// <summary>The main name of this function. Originates from the first result returned by GetNames.</summary>
		public string Name;
		
		
		/// <summary>The set of all function names that this one will handle. Usually just one. Lowercase.
		/// e.g. "rgb", "rgba".</summary>
		public virtual string[] GetNames(){
			return null;
		}
		
		public override bool IsFunction{
			get{
				return true;
			}
		}
		
		public override string Identifier{
			get{
				return Name;
			}
		}
		
		public override string ToString(){
			
			return Name+"("+base.ToString()+")";
			
		}
		
	}
	
}