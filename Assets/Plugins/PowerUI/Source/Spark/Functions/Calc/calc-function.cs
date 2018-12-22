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
using Css.Units;


namespace Css.Functions{
	
	/// <summary>
	/// Represents the calc() css function.
	/// </summary>
	
	public class Calc:CssFunction{
		
		/// <summary>The operator precedence list.</summary>
		private static char[] Precedence=new char[]{'-','+','*','/'};
		/// <summary>The global operators set.</summary>
		public static Dictionary<string,int> Operators;
		
		
		/// <summary>Require the operators lookup to be created.</summary>
		public static void RequireOperators(){
			
			// Create:
			Operators=new Dictionary<string,int>();
			
			for(int i=0;i<Precedence.Length;i++){
				
				// Push:
				Operators[Precedence[i].ToString()]=i;
				
			}
			
		}
		
		/// <summary>The internal "compiled" operator.</summary>
		public Css.Value Operator;
		
		
		public Calc(){
			
			Name="calc";
			Type=ValueType.RelativeNumber;
			
		}
		
		protected override Css.Value Clone(){
			Calc result=new Calc();
			result.Operator=Operator;
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// The parameter set is now treated as an interpretable set of commands.
			// Compact them into a single "operator":
			
			if(Operators==null){
				RequireOperators();
			}
			
			Operator=BuildOperator(this);
			
			if(Operator==this){
				Operator=null;
			}
			
		}
		
		private Css.Value BuildOperator(Css.Value value){
			
			int size=value.Count;
			
			if(size==1){
				return value;
			}
			
			return BuildOperator(value,0,size);
			
		}
		
		private Css.Value BuildOperator(Css.Value value,int start,int max){
			
			// First, find the most important operator.
			
			int diff=max-start;
			
			if(diff==1){
				return value[start];
			}
			
			if(diff<=0){
				// This occurs where an operator is declared but with no operands.
				return new DecimalUnit(0f);
			}
			
			char bestOperator='\0';
			int bestPrecedence=-1;
			int bestIndex=-1;
			
			for(int i=start;i<max;i++){
				
				// Grab the value:
				Css.Value innerValue=value[i];
				
				if(innerValue.Type==ValueType.Set){
					
					// Got a set (e.g. brackets) - build it:
					value[i]=BuildOperator(innerValue);
					
					continue;
					
				}
					
				// Is this an operator?
				if(innerValue.Type==ValueType.Text){
					
					// Highly likely to be!
					string text=innerValue.GetText(null,null);
					
					int precedence;
					if(!Operators.TryGetValue(text,out precedence)){
						continue;
					}
					
					// Got an operator - is this currently the favourite (most important) one?
					if(precedence>bestPrecedence){
						
						// Yep!
						bestPrecedence=precedence;
						bestIndex=i;
						bestOperator=text[0];
						
					}
					
				}
				
			}
			
			if(bestIndex==-1){
				// No operators! Return it as-is:
				return value;
			}
			
			// Which operator is it?
			CalcOperator op=null;
			
			switch(bestOperator){
				
				case '*':
					op=new CalcMultiply();
				break;
				case '/':
					op=new CalcDivide();
				break;
				case '+':
					op=new CalcAdd();
				break;
				case '-':
					op=new CalcSubtract();
				break;
			}
			
			// Hook up inputs:
			op.Input0=BuildOperator(value,start,bestIndex);
			op.Input1=BuildOperator(value,bestIndex+1,max);
			
			return op;
		}
		
		public override string[] GetNames(){
			return new string[]{"calc"};
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			if(Operator==null){
				return 0f;
			}
			
			// Run the calc here!
			return Operator.GetDecimal(context,property);
			
		}
		
	}
	
}



