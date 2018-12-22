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
using Dom;


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the initial keyword.
	/// </summary>
	
	public class Initial:CssKeyword{
		
		/// <summary>The property that this is the value for.</summary>
		public CssProperty Property;
		
		
		/// <summary>Used by the parser (via reflection during load). Must clone these.</summary>
		public Initial(){}
		
		/// <summary>Used by the cloner. Optionally set a specifity.</summary>
		public Initial(CssProperty prop,int specifity){
			Property=prop;
			Specifity=specifity;
		}
		
		/// <summary>Used by the cloner.</summary>
		public Initial(CssProperty prop){
			Property=prop;
		}
		
		protected override Value Clone(){
			return new Initial(Property);
		}
		
		public override string Name{
			get{
				return "initial";
			}
		}
		
		/// <summary>Resolves through e.g. inherit and initial.</summary>
		public override Css.Value Computed{
			get{
				return Property.InitialValue;
			}
		}
		
		/// <summary>Checks if this is the 'auto' keyword</summary>
		/// <returns>True if this value is 'auto'.</returns>
		public override bool IsAuto{
			get{
				return Property.InitialValue.IsAuto;
			}
		}
		
		/// <summary>Checks if this is a particular type. Note that this is always false for inherit/ initial
		/// (as they pass the type through them).</summary>
		/// <returns>True if this value is of the given type.</returns>
		public override bool IsType(Type type){
			return Property.InitialValue.GetType()==type;
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return Property.InitialValue.GetDecimal(context,property);
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			return Property.InitialValue.GetText(context,property);
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			return Property.InitialValue.GetBoolean(context,property);
		}
		
		public override IEnumerator<Value> GetEnumerator(){
			
			foreach(Value value in Property.InitialValue){
				
				yield return value;
				
			}
			
		}
		
		private void Readonly(){
			throw new Exception("Initial is readonly. Clone the object before trying to write to it.");
		}
		
		public override int Count{
			get{
				return Property.InitialValue.Count;
			}
			set{
				Readonly();
			}
		}
		
		public override Value this[int index]{
			get{
				return Property.InitialValue[index];
			}
			set{
				Readonly();
			}
		}
		
	}
	
}



