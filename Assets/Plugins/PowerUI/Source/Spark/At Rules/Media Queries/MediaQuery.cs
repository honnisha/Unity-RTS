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


namespace Css{
	
	/// <summary>
	/// Represents an instance of a media query. They resolve to either true or false.
	/// </summary>
	
	public class MediaQuery{
		
		/// <summary>Loads the given segment of the given value.</summary>
		private static MediaQuery LoadSection(Value value,ref int i,int endInclusive){
			
			Value currentValue=value[i];
			
			if(currentValue==null || currentValue==Css.Value.Empty){
				return null;
			}
			
			// Is it an expression?
			ValueSet expr=currentValue as ValueSet;
			
			if(expr==null){
				
				// (only|not)? MEDIA_TYPE
				// Optionally followed by 'and'
				// Always textual though:
				string token=currentValue.Text;
				
				if(token=="all"){
					
					// An empty query instance represents all:
					return new MediaQuery();
					
				}else if(token=="not"){
					
					i++;
					return new MediaQueryNot(LoadSection(value,ref i,endInclusive));
					
				}else if(token=="only"){
					
					// Totally ignore this.
					i++;
					return LoadSection(value,ref i,endInclusive);
					
				}
				
				// Current media test:
				MediaQuery current=new MediaQueryCurrentMedia(token);
				
				// Check for 'and'
				if((i+1)<=endInclusive){
					
					currentValue=value[i+1];
					
					if(currentValue!=null &&currentValue!=Css.Value.Empty && currentValue.Text=="and"){
						
						// Got the 'and' keyword!
						i+=2;
						
						return new MediaQueryAnd(current,LoadSection(value,ref i,endInclusive));
						
					}
					
				}
				
				return current;
				
			}
			
			// Got e.g. (min-width:400px) or (color) etc.
			// Essentially it's a feature in brackets.
			return LoadFeatureExpression(expr);
			
		}
		
		/// <summary>Loads a feature ref from the given brackets. It's e.g. (color) or (min-width:xpx) etc.</summary>
		private static MediaQuery LoadFeatureExpression(ValueSet set){
			
			// Get feature name:
			string featureName=set[0].Text;
			
			if(set.Count==1){
				
				// Single feature check:
				return new MediaQueryHasFeature(featureName);
				
			}
			
			// Get the value:
			Css.Value value=set[2];
			
			// Check if it contains 'min-' or 'max-':
			if(featureName.StartsWith("min-")){
				
				// Min test.
				return new MediaQueryMinFeature(featureName,value);
				
			}else if(featureName.StartsWith("max-")){
				
				// Max test.
				return new MediaQueryMaxFeature(featureName,value);
				
			}
			
			// Equality otherwise.
			return new MediaQueryEqualsFeature(featureName,value);
			
		}
		
		/// <summary>Loads this media query from the given CSS value between the given set indices.</summary>
		public static MediaQuery Load(Value value,int start,int endInclusive){
			
			// Got anything?
			if(start>endInclusive){
				
				// Act like 'all' was declared by clearing the things to match:
				return new MediaQuery();
				
			}
			
			// From start to end (inclusive), read the keywords
			// (property:value) are ValueSet instances
			MediaQuery result=null;
			List<MediaQuery> results=null;
			
			for(int i=start;i<=endInclusive;i++){
				
				// Put the value into context:
				MediaQuery section=LoadSection(value,ref i,endInclusive);
				
				if(section==null){
					continue;
				}
				
				if(result==null){
					result=section;
				}else{
					
					if(results==null){
						// Create set:
						results=new List<MediaQuery>();
						results.Add(result);
					}
					
					results.Add(section);
					
				}
				
			}
			
			if(results!=null){
				
				// Multiple results.
				return new MediaQueryList(results.ToArray());
				
			}
			
			return result;
			
			
		}
		
		public MediaQuery(){
		}
		
		/// <summary>Evaluates this media query now.</summary>
		public virtual bool IsTrue(ReflowDocument screen){
			
			// 'All' is the default:
			return true;
			
		}
		
		public override string ToString(){
			// 'All' is the default:
			return "all";
		}
		
	}
	
	public class MediaQueryList : MediaQuery{
		
		/// <summary>The underlying set of queries.</summary>
		public MediaQuery[] Queries;
		/// <summary>The host document (if any).</summary>
		public ReflowDocument Document;
		
		
		/// <summary>Used when listing,media,queries.</summary>
		public MediaQueryList(MediaQuery[] queries){
			Queries=queries;
		}
		
		/// <summary>Used by document.matchMedia.</summary>
		public MediaQueryList(ReflowDocument doc,MediaQuery q){
			
			// Pull the query set from the media query, if it is a set:
			if( !(q is MediaQueryList) ){
				
				// Just the one query.
				Queries=new MediaQuery[]{q};
				return;
				
			}
			
			// Pull the set out:
			MediaQueryList list=q as MediaQueryList;
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
	
	public class MediaQueryAnd : MediaQuery{
		
		public MediaQuery Input1;
		public MediaQuery Input2;
		
		
		public MediaQueryAnd(MediaQuery in1,MediaQuery in2){
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
	
	public class MediaQueryNot : MediaQuery{
		
		/// <summary>The query being inverted.</summary>
		public MediaQuery Input1;
		
		
		public MediaQueryNot(MediaQuery input){
			Input1=input;
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			return (!Input1.IsTrue(document));
			
		}
		
		public override string ToString(){
			
			return "not "+Input1.ToString();
			
		}
		
	}
	
	public class MediaQueryCurrentMedia : MediaQuery{
		
		/// <summary>The name of the media, e.g. "screen".</summary>
		public string MediaName;
		
		
		public MediaQueryCurrentMedia(string name){
			MediaName=name.Trim().ToLower();
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			return (document.Media.Is(MediaName));
			
		}
		
		public override string ToString(){
			
			return MediaName;
			
		}
		
	}
	
	public class MediaQueryHasFeature : MediaQuery{
		
		/// <summary>The name of the feature, e.g. "color".</summary>
		public string Feature;
		
		
		public MediaQueryHasFeature(string name){
			Feature=name.Trim().ToLower();
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			return (document.Media.HasFeature(Feature));
			
		}
		
		public override string ToString(){
			
			return "("+Feature+")";
			
		}
		
	}
	
	/// <summary>Checks if the named feature is at least x.</summary>
	public class MediaQueryMinFeature : MediaQuery{
		
		/// <summary>The name of the feature, e.g. "color".</summary>
		public string Feature;
		/// <summary>The minimum value.</summary>
		public float MinValue;
		
		
		public MediaQueryMinFeature(string name,Css.Value minValue){
			Feature=name.Trim().ToLower();
			MinValue=minValue.GetRawDecimal();
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			Css.Value v=document.Media[Feature];
			
			if(v==null){
				return (0f >= MinValue);
			}
			
			return (v.GetRawDecimal() >= MinValue);
			
		}
		
		public override string ToString(){
			
			return "("+Feature+":"+MinValue+")";
			
		}
		
	}
	
	/// <summary>Checks if the named feature is at most x.</summary>
	public class MediaQueryMaxFeature : MediaQuery{
		
		/// <summary>The name of the feature, e.g. "color".</summary>
		public string Feature;
		/// <summary>The maximum value.</summary>
		public float MaxValue;
		
		
		public MediaQueryMaxFeature(string name,Css.Value maxValue){
			Feature=name.Trim().ToLower();
			MaxValue=maxValue.GetRawDecimal();
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			Css.Value v=document.Media[Feature];
			
			if(v==null){
				return (0f <= MaxValue);
			}
			
			return (v.GetRawDecimal() <= MaxValue);
			
		}
		
		public override string ToString(){
			
			return "("+Feature+":"+MaxValue+")";
			
		}
		
	}
	
	/// <summary>Checks if the named feature is equal to x.</summary>
	public class MediaQueryEqualsFeature : MediaQuery{
		
		/// <summary>The name of the feature, e.g. "orientation".</summary>
		public string Feature;
		/// <summary>The value to check for.</summary>
		public Css.Value Value;
		
		
		public MediaQueryEqualsFeature(string name,Css.Value eqValue){
			Feature=name.Trim().ToLower();
			Value=eqValue;
		}
		
		public override bool IsTrue(ReflowDocument document){
			
			// Get the value:
			Css.Value value=document.Media[Feature];
			
			// Match?
			return Value.Equals(value);
			
		}
		
		public override string ToString(){
			
			return "("+Feature+":"+Value+")";
			
		}
		
	}
	
}



