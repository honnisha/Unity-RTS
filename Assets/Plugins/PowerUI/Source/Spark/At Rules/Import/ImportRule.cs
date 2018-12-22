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
using PowerUI;


namespace Css{
	
	/// <summary>
	/// An @import rule.
	/// </summary>
	
	public class ImportRule : Rule{
		
		/// <summary>Imported href.</summary>
		public string href;
		/// <summary>The raw CSS value.</summary>
		public Css.Value RawValue;
		/// <summary>A media query.</summary>
		public MediaQuery Query;
		/// <summary>The imported sheet.</summary>
		public StyleSheet ImportedSheet;
		/// <summary>The parent stylesheet.</summary>
		public StyleSheet ParentSheet;
		
		
		public ImportRule(StyleSheet sheet,Css.Value rawValue,MediaQuery query,string href){
			
			ParentSheet=sheet;
			Query=query;
			RawValue=rawValue;
			this.href=href;
			
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
		
		/// <summary>The imported stylesheet.</summary>
		public StyleSheet styleSheet{
			get{
				return ImportedSheet;
			}
		}
		
		/// <summary>Rule type.</summary>
		public int type{
			get{
				return 3;
			}
		}
		
		/// <summary>Media query.</summary>
		public MediaQuery media{
			get{
				return Query;
			}
		}
		
		private void DownloadNow(){
			
			// Import the file now, and ensure StyleSheet.ownerNode is set.
			
			string url=href;
			
			if(ParentSheet!=null && ParentSheet.Location!=null){
				
				// Make relative now:
				url=(new Dom.Location(url,ParentSheet.Location)).absoluteNoHash;
				
			}
			
			// Create a style sheet:
			ImportedSheet=new Css.StyleSheet(ParentSheet.ownerNode);
			ImportedSheet.ownerRule=this;
			
			// Priority is at least that of its parent:
			ImportedSheet.Priority=ParentSheet==null? 1 : ParentSheet.Priority;
			
			// Load the file now:
			DataPackage package=new DataPackage(url,ParentSheet.document.basepath);
			
			package.onload=delegate(UIEvent e){
				
				// Load it now:
				ImportedSheet.ParseCss(package.responseText);
				
				// Redraw:
				ParentSheet.document.RequestLayout();
				
			};
			
			// Get now:
			package.send();
			
		}
		
		public void AddToDocument(ReflowDocument document){
			
			if(Query==null || Query.IsTrue(document)){
				
				if(ImportedSheet==null){
					DownloadNow();
					
					// Add it:
					ParentSheet.document.AddStyle(ImportedSheet,null);
					
				}else{
					
					ImportedSheet.ReAddSheet(document);
				
				}
				
			}
			
		}
		
		public void RemoveFromDocument(ReflowDocument document){
			
			if(ImportedSheet!=null){
				
				ImportedSheet.RemoveSheet(document);
				
			}
			
		}
		
	}
	
}