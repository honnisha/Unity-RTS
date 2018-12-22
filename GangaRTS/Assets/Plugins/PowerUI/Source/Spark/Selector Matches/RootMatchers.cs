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


namespace Css{
	
	/// <summary>
	/// A selector is made up of a series of one or more 'roots'.
	/// This is a (global) instance of one of those roots.
	/// There's three types of root: tag, id, class.
	/// </summary>
	
	public class RootMatcher : SelectorMatcher{
		
		/// <summary>The ID/Class/Tag.</summary>
		public string Text;
		/// <summary>True if this root is the target.</summary>
		public bool IsTarget;
		/// <summary>Each root can have an optional collection of 'local' matchers.
		/// These will typically originate from pseudo-classes as well as the attribute selector.
		/// Things like :hover or :checked; they're local matchers.</summary>
		public LocalMatcher[] LocalMatchers;
		/// <summary>The structure matcher between this and the following root, if any.</summary>
		public StructureMatcher NextMatcher;
		/// <summary>The structure matcher between this and the previous root, if any.</summary>
		public StructureMatcher PreviousMatcher;
		
		
		/// <summary>Sets the local matchers.</summary>
		public void SetLocals(List<LocalMatcher> locals){
			
			if(locals!=null && locals.Count>0){
				
				// Apply:
				LocalMatchers=locals.ToArray();
				
			}
			
		}
		
		/// <summary>True if the given root matchers are equal.</summary>
		public bool Equals(RootMatcher rm){
			
			// Reference match:
			if(rm==this){
				return true;
			}
			
			// Type and text match:
			if(GetType()!=rm.GetType() || Text!=rm.Text){
				return false;
			}
			
			// Local matchers next.
			if(LocalMatchers!=null){
				
				if(rm.LocalMatchers==null || rm.LocalMatchers.Length!=LocalMatchers.Length){
					return false;
				}
				
				// Compare each local matcher:
				for(int i=0;i<LocalMatchers.Length;i++){
					
					// Match?
					if( LocalMatchers[i].Equals(LocalMatchers[i]) ){
						
						return false;
						
					}
					
				}
				
			}else if(rm.LocalMatchers!=null){
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>Gets a string of all the local matchers.</summary>
		public override string ToString(){
			if(LocalMatchers==null){
				return StyleText;
			}
			
			string str="";
			
			for(int i=0;i<LocalMatchers.Length;i++){
				
				// Close enough for now!
				str+=":"+LocalMatchers[i].GetType().ToString().Replace("Matcher","").ToLower();
				
			}
			
			return StyleText+str;
		}
		
		/// <summary>The text as it's seen in a stylesheet. E.g. #ID.</summary>
		public virtual string StyleText{
			get{
				return Text;
			}
		}
		
		/// <summary>The specifity. Add it to other roots to get the selectors specifity.</summary>
		public int Specifity{
			
			get{
				// RootSpecifity + any local specifity.
				int specifity=RootSpecifity;
				
				if(LocalMatchers!=null){
					
					// Each of these goes into the second byte:
					specifity += (LocalMatchers.Length * (1<<8));
					
				}
				
				return specifity;
			}
			
		}
		
		/// <summary>This matchers specifity.</summary>
		protected virtual int RootSpecifity{
			get{
				return 0;
			}
		}
		
	}
	
	/// <summary>
	/// Handles *.
	/// </summary>
	
	sealed class RootUniversalMatcher : RootMatcher{
		
		// Specifity is 0 - it's ignored.
		
		public override string StyleText{
			get{
				return "*";
			}
		}
		
		public override bool TryMatch(Dom.Node context){
			
			// Everything matches!
			return true;
			
		}
		
	}
	
	/// <summary>
	/// Handles #id.
	/// </summary>
	
	sealed class RootIDMatcher : RootMatcher{
		
		public override string StyleText{
			get{
				return "#"+Text;
			}
		}
		
		protected override int RootSpecifity{
			get{
				// 2nd byte:
				return 1<<16;
			}
		}
		
		/// <summary>The case sensitive ID.</summary>
		public string ID{
			get{
				return Text;
			}
			set{
				Text=value;
			}
		}
		
		public override bool TryMatch(Dom.Node context){
			
			// Match with ID?
			string value;
			if(context.Properties.TryGetValue("id",out value)){
				return value==Text;
			}
			
			return false;
			
		}
		
	}
	
	/// <summary>
	/// Handles tags.
	/// </summary>
	
	sealed class RootTagMatcher : RootMatcher{
		
		protected override int RootSpecifity{
			get{
				// 0th byte:
				return 1;
			}
		}
		
		/// <summary>The case insensitive tag.</summary>
		public string Tag{
			get{
				return Text;
			}
			set{
				Text=value;
			}
		}
		
		public override bool TryMatch(Dom.Node context){
			
			// Tag match?
			Dom.Element el=context as Dom.Element;
			
			if(el==null){
				return false;
			}
			
			return el.Tag==Text || (el.Tag==null && Text=="span");
			
		}
		
	}
	
	/// <summary>
	/// Handles .class.
	/// </summary>
	
	sealed class RootClassMatcher : RootMatcher{
		
		public override string StyleText{
			get{
				return "."+Text;
			}
		}
		
		protected override int RootSpecifity{
			get{
				// 1st byte:
				return 1<<8;
			}
		}
		
		/// <summary>The case sensitive class.</summary>
		public string Class{
			get{
				return Text;
			}
			set{
				Text=value;
			}
		}
		
		public override bool TryMatch(Dom.Node context){
			
			string cName=context.className;
			
			if(string.IsNullOrEmpty(cName)){
				return false;
			}
			
			int nextSpace=cName.IndexOf(' ');
			
			if(nextSpace!=-1){
				
				// Multiple classes to check.
				
				int currentStart=0;
				int currentEnd=nextSpace;
				
				// While there's still more text..
				while(currentStart<cName.Length){
					
					if(Text==cName.Substring(currentStart,currentEnd-currentStart)){
						return true;
					}
					
					// Move start:
					currentStart=currentEnd+1;
					
					if(currentStart>=cName.Length){
						// All done.
						break;
					}
					
					// Next space is at..
					nextSpace=cName.IndexOf(' ',currentStart);
					
					if(nextSpace==-1){
						currentEnd=cName.Length;
					}else{
						currentEnd=nextSpace;
					}
					
				}
				
			}else if(Text==cName){
				
				// Just a single class to check and it passed!
				return true;
				
			}
			
			// Nope!
			return false;
			
		}
		
	}
	
}