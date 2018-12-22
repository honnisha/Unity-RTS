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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the -spark-specifity: css property.
	/// </summary>
	
	public class SparkSpecifity:CssProperty{
		
		public static SparkSpecifity GlobalProperty;
		
		
		public SparkSpecifity(){
			
			GlobalProperty=this;
			
		}
		
		/// <summary>Internal property.</summary>
		public override bool Internal{
			get{
				return true;
			}
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-specifity"};
		}
		
	}
	
}