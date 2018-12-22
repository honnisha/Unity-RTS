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
using Css;


namespace Css.Spec{
	
	/// <summary>
	/// The base of a CSS specification value. These are primarily used by composite properties such as background: or font:.
	///
	/// Spec value cheat sheet:
	/// Spec.AnyOf			||
	/// Spec.All			a b
	/// Spec.Property		<a>
	/// Spec.Optional		a?
	/// Spec.OneOf			a|b
	/// Spec.Literal		/
	/// Spec.AllAnyOrder	a && b
	/// Spec.Repeated		{1,4}, * => {0,infinity}, + => {1,infinity}
	/// Spec.ValueType      <number>, <percent> etc.
	/// </summary>
	
	public class Value{
		
		public virtual bool OnReadValue(Style styleBlock,Css.Value value,int start,out int size){
			size=0;
			return false;
		}
		
	}
	
}