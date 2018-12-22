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


namespace Css.AtRules{
	
	/// <summary>
	/// Represents the namespace rule.
	/// </summary>
	
	public class NamespaceRule:CssAtRule, Rule{
		
		/// <summary>The raw value.</summary>
		public Css.Value RawValue;
		/// <summary>The prefix, if any, to use.</summary>
		public string prefix;
		/// <summary>The namespace path/name.</summary>
		public CssNamespace Namespace;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		public override string[] GetNames(){
			return new string[]{"namespace"};
		}
		
		public override CssAtRule Copy(){
			NamespaceRule at=new NamespaceRule();
			at.Namespace=Namespace;
			at.prefix=prefix;
			at.RawValue=RawValue;
			at.ParentSheet=ParentSheet;
			return at;
		}
		
		public override Rule LoadRule(Css.Rule parent,StyleSheet style,Css.Value value){
			
			RawValue=value;
			ParentSheet=style;
			
			// Get the count:
			int count=value.Count;
			
			if(count==1){
				// Invalid
				return null;
			}
			
			// Read a value:
			Css.Value val=value[1];
			
			// Got a prefix?
			if(count>=3){
				prefix=val.Text.ToLower();
				val=value[2];
			}
			
			// Get the namespace:
			Namespace=new CssNamespace(val.Text);
			
			if(!string.IsNullOrEmpty(prefix)){
				
				if(ParentSheet.Namespaces==null){
					// Create prefix lookup:
					ParentSheet.Namespaces=new Dictionary<string,CssNamespace>();
				}
				
				ParentSheet.Namespaces[prefix]=Namespace;
				
			}else{
				// Assign NS:
				ParentSheet.Namespace=Namespace;
			}
			
			return this;
		}
		
		public string namespaceURI{
			get{
				return Namespace.Name;
			}
		}
		
		/// <summary>The CSS text of this rule.</summary>
		public string cssText{
			get{
				return RawValue.ToString();
			}
			set{
				throw new NotImplementedException("cssText is read-only on rules. Set it for a whole sheet instead.");
			}
		}
		
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet parentStyleSheet{
			get{
				return ParentSheet;
			}
		}
		
		/// <summary>Rule type.</summary>
		public int type{
			get{
				return 10;
			}
		}
		
		public void AddToDocument(ReflowDocument document){
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
		}
		
	}
	
}



