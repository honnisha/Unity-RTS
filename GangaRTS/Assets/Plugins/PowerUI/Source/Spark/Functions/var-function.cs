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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the var() css function.
	/// </summary>
	
	public class VarFunction:CssFunction{
		
		/// <summary>The link to the value.</summary>
		public Css.Value Value;
		
		
		public VarFunction(){
			
			Name="var";
			Type=ValueType.Text;
			
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// Get the var name:
			string name=base[0].Text;
			
			if(name.StartsWith("--")){
				name = name.Substring(2);
			}
			
			if(name!=null){
				Value=lexer.GetVariable(name.ToLower());
			}
			
			if(Value == null){
				Value = new Units.DecimalUnit(0);
			}
			
		}
		
		public override string[] GetNames(){
			return new string[]{"var"};
		}
		
		protected override Css.Value Clone(){
			VarFunction result=new VarFunction();
			result.Values=CopyInnerValues();
			result.Value=Value;
			return result;
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			if(Value==null){
				return null;
			}
			
			return Value.GetText(context,property);
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			if(Value==null){
				return 0f;
			}
			
			return Value.GetDecimal(context,property);
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			if(Value==null){
				return false;
			}
			
			return Value.GetBoolean(context,property);
		}
		
		
		/// <summary>Resolves through e.g. inherit and initial.</summary>
		public override Css.Value Computed{
			get{
				return Value;
			}
		}
		
		/// <summary>Checks if this is the 'auto' keyword</summary>
		/// <returns>True if this value is 'auto'.</returns>
		public override bool IsAuto{
			get{
				return Value.IsAuto;
			}
		}
		
		/// <summary>Converts this value into a hex string that is 2 characters long.</summary>
		public override string HexString{
			get{
				return Value.HexString;
			}
		}
		
		/// <summary>Checks if this is a particular type. Note that this is always false for inherit/ initial
		/// (as they pass the type through them).</summary>
		/// <returns>True if this value is of the given type.</returns>
		public override bool IsType(Type type){
			return Value.GetType()==type;
		}
		
		/// <summary>Gets the value as an image, if it is one.</summary>
		public override PowerUI.ImageFormat GetImage(RenderableData context,CssProperty property){
			return Value.GetImage(context,property);
		}
		
		public override IEnumerator<Value> GetEnumerator(){

			foreach(Value value in Value){
				
				yield return value;
				
			}
			
		}
		
		public override int Count{
			get{
				return Value.Count;
			}
			set{
				base.Count=value;
			}
		}
		
		public override Value this[int index]{
			get{
				return Value[index];
			}
			set{
				base[index]=value;
			}
		}
		
	}
	
}



