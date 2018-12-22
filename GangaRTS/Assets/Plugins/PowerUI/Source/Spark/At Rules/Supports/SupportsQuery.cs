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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Css.Units;
using Dom;


namespace Css{
	
	/// <summary>
	/// Represents an instance of a supports query. They resolve to either true or false.
	/// </summary>
	
	public class SupportsQuery{
		
		/// <summary>Loads the given segment of the given value.</summary>
		private static SupportsQuery LoadSection(Value value,ref int i,int endInclusive){
			
			Value currentValue=value[i];
			
			if(currentValue==null || currentValue==Css.Value.Empty){
				return null;
			}
			
			// Is it an expression?
			ValueSet expr=currentValue as ValueSet;
			
			SupportsQuery current;
			
			if(expr==null){
				
				// Always textual here:
				string token=currentValue.Text;
				
				if(token=="not"){
					
					i++;
					current=new SupportsQueryNot(LoadSection(value,ref i,endInclusive));
					
				}else{
					
					current=null;
					
				}
				
			}else{
				
				if(expr.Count==2 && expr[1] is ValueSet){
					
					// Nested functions:
					int nestedI=0;
					current=LoadSection(expr,ref nestedI,1);
					
				}else{
					
					// Got e.g. (min-width:400px) etc.
					// Essentially it's a feature in brackets.
					current=LoadPropertyExpression(expr);
					
				}
				
			}
		
			// Check for 'and'/'or'
			if((i+1)<=endInclusive){
				
				currentValue=value[i+1];
				
				if(currentValue!=null &&currentValue!=Css.Value.Empty){
					if(currentValue.Text=="and"){
						
						// Got the 'and' keyword!
						i+=2;
						
						return new SupportsQueryAnd(current,LoadSection(value,ref i,endInclusive));
						
					}else if(currentValue.Text=="or"){
						
						// Got the 'or' keyword!
						i+=2;
						
						return new SupportsQueryList(new SupportsQuery[]{current,LoadSection(value,ref i,endInclusive)});
						
					}
					
				}
				
			}
			
			return current;
			
		}
		
		/// <summary>Loads a prop ref from the given brackets. It's e.g. (min-width:xpx) etc.</summary>
		private static SupportsQuery LoadPropertyExpression(ValueSet set){
			
			// Get prop name:
			string featureName=set[0].Text;
			
			if(set.Count==1){
				
				// Got this property?:
				return new SupportsQueryHasProperty(featureName);
				
			}
			
			// Get the value:
			Css.Value value=set[2];
			
			// Can this property be set?
			return new SupportsQueryCanSetProperty(featureName,value);
			
		}
		
		/// <summary>Loads this supports query from the given CSS value between the given set indices.</summary>
		public static SupportsQuery Load(Value value,int start,int endInclusive){
			
			// Got anything?
			if(start==endInclusive){
				
				// Act like 'all' was declared by clearing the things to match:
				return new SupportsQuery();
				
			}
			
			// From start to end (inclusive), read the keywords
			// (property:value) are ValueSet instances
			SupportsQuery result=null;
			List<SupportsQuery> results=null;
			
			for(int i=start;i<=endInclusive;i++){
				
				// Put the value into context:
				SupportsQuery section=LoadSection(value,ref i,endInclusive);
				
				if(section==null){
					continue;
				}
				
				if(result==null){
					result=section;
				}else{
					
					if(results==null){
						// Create set:
						results=new List<SupportsQuery>();
						results.Add(result);
					}
					
					results.Add(section);
					
				}
				
			}
			
			if(results!=null){
				
				// Multiple results.
				return new SupportsQueryList(results.ToArray());
				
			}
			
			return result;
			
			
		}
		
		public SupportsQuery(){
		}
		
		/// <summary>Evaluates this query now.</summary>
		public virtual bool IsTrue(ReflowDocument screen){
			
			// 'All' is the default:
			return true;
			
		}
		
		public override string ToString(){
			// 'All' is the default:
			return "all";
		}
		
	}
	
	public class SupportsQueryList : SupportsQuery{
		
		/// <summary>The underlying set of queries.</summary>
		public SupportsQuery[] Queries;
		/// <summary>The host document (if any).</summary>
		public ReflowDocument Document;
		
		
		/// <summary>Used when listing,supports,queries.</summary>
		public SupportsQueryList(SupportsQuery[] queries){
			Queries=queries;
		}
		
		/// <summary>Used by JS.</summary>
		public SupportsQueryList(ReflowDocument doc,SupportsQuery q){
			
			// Pull the query set from the supports query, if it is a set:
			if( !(q is SupportsQueryList) ){
				
				// Just the one query.
				Queries=new SupportsQuery[]{q};
				return;
				
			}
			
			// Pull the set out:
			SupportsQueryList list=q as SupportsQueryList;
			Queries=list.Queries;
			
		}
		
		/// <summary>Do they all match?</summary>
		public bool matches{
			get{
				return IsTrue(Document);
			}
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			// For each one..
			for(int i=0;i<Queries.Length;i++){
				
				// Is it true?
				if(Queries[i].IsTrue(document)){
					return true;
				}
				
			}
			
			// None were true
			return false;
			
		}
		
		public override string ToString(){
			
			string res="";
			
			// For each one..
			for(int i=0;i<Queries.Length;i++){
				
				if(i!=0){
					res+=", ";
				}
				
				res+=Queries[i].ToString();
				
			}
			
			return res;
			
		}
		
	}
	
	public class SupportsQueryAnd : SupportsQuery{
		
		public SupportsQuery Input1;
		public SupportsQuery Input2;
		
		
		public SupportsQueryAnd(SupportsQuery in1,SupportsQuery in2){
			Input1=in1;
			Input2=in2;
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			// Must both be true:
			return Input1.IsTrue(document) && Input2.IsTrue(document);
			
		}
		
		public override string ToString(){
			
			return Input1.ToString()+" and "+Input2.ToString();
			
		}
		
	}
	
	public class SupportsQueryNot : SupportsQuery{
		
		/// <summary>The query being inverted.</summary>
		public SupportsQuery Input1;
		
		
		public SupportsQueryNot(SupportsQuery input){
			Input1=input;
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			return (!Input1.IsTrue(document));
			
		}
		
		public override string ToString(){
			
			return "not "+Input1.ToString();
			
		}
		
	}
	
	public class SupportsQueryHasProperty : SupportsQuery{
		
		/// <summary>The name of the property, e.g. "color".</summary>
		public string Property;
		
		
		public SupportsQueryHasProperty(string name){
			Property=name.Trim().ToLower();
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			return (document.Media.HasFeature(Property));
			
		}
		
		public override string ToString(){
			
			return "("+Property+")";
			
		}
		
	}
	
	/// <summary>Checks if a property can be set to the given value.</summary>
	public class SupportsQueryCanSetProperty : SupportsQuery{
		
		/// <summary>The name of the property, e.g. "orientation".</summary>
		public string Property;
		/// <summary>The value to check for.</summary>
		public Css.Value Value;
		
		
		public SupportsQueryCanSetProperty(string name,Css.Value eqValue){
			Property=name.Trim().ToLower();
			Value=eqValue;
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			// Get the value:
			Css.Value value=document.Media[Property];
			
			// Match?
			return Value.Equals(value);
			
		}
		
		public override string ToString(){
			
			return "("+Property+":"+Value+")";
			
		}
		
	}
	
}



