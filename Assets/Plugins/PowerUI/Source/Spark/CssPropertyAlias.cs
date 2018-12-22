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
using Dom;


namespace Css{
	
	/// <summary>
	/// A CSS property alias. For example, border-top-left-radius is an alias for border-radius[0].
	/// When an alias property is parsed, it maps the value through to the actual target property.
	/// </summary>
	
	public class CssPropertyAlias:CssProperty{
		
		/// <summary>Inner indices used to map the alias property, e.g. index 0 of border-radius.</summary>
		public int[] Index;
		/// <summary>The property that this alias maps to.</summary>
		public CssProperty Target;
		
		
		public CssPropertyAlias(){
			IsAlias=true;
		}
		
		public CssPropertyAlias(CssProperty target,int[] index){
			
			IsAlias=true;
			
			if(index!=null){
				if(index.Length==0){
					index=null;
				}else{
					// Update target set size:
					int maxSize=index[0]+1;
					
					if(maxSize>target.SetSize){
						target.SetSize=maxSize;
					}
				}
			}
			
			Target=target;
			Index=index;
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				if(Target==null){
					return false;
				}
				
				return Target.NonStandard;
			}
		}
		
		/// <summary>Used to map a logical index to a physical one.</summary>
		public virtual int RemapIndex(Css.Style style){
			return 0;
		}
		
		public override Css.Value GetValue(Style styleBlock){
			
			// Get target:
			Css.Value result=styleBlock[Target];
			
			if(result==null){
				// Doesn't exist at all.
				return null;
			}
			
			// Any inner indices?
			if(Index==null){
				return result;
			}
			
			// For each inner index..
			for(int i=0;i<Index.Length;i++){
				
				// Try pulling it:
				int index=Index[i];
				
				if(index==-1){
					// Logical remap:
					index=RemapIndex(styleBlock);
				}
				
				result=result[index];
				
				// Got a value?
				if(result==null){
					return null;
				}
				
			}
			
			return result;
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			if(Index==null || !(styleBlock is ComputedStyle)){
				
				// Just call the base:
				base.OnReadValue(styleBlock,value);
				
				return;
				
			}
			
			// Get/ Create the target value:
			Value targetValue=styleBlock.GetBaseValue(Target);
			
			// Update targetValue's Specifity - it'll be the biggest one of its components:
			if(value!=null && value.Specifity > targetValue.Specifity){
				
				targetValue.Specifity=value.Specifity;
				
			}
			
			int max=Index.Length-1;
			
			for(int i=0;i<max;i++){
				
				// Create and add a set:
				ValueSet set=new ValueSet();
				targetValue[Index[i]]=set;
				targetValue=set;
				
			}
			
			// Apply value:
			int index=Index[max];
			
			if(index==-1){
				// Logical remap:
				index=RemapIndex(styleBlock);
			}
			
			targetValue[index]=value;
			
			// Call the change:
			styleBlock.CallChange(Target,targetValue);
			
		}
		
		public override Css.Value GetOrCreateValue(Node context,Style styleBlock,bool allowInherit,out Css.Value hostValue){
			
			if(Index==null || !(styleBlock is ComputedStyle) ){
				
				// Just call the base:
				return base.GetOrCreateValue(context,styleBlock,allowInherit,out hostValue);
				
			}
			
			// Get/ Create the target value (always exists; doesn't inherit):
			hostValue=styleBlock.GetBaseValue(Target);
			
			int max=Index.Length-1;
			
			int index=Index[max];
			
			if(index==-1){
				// Logical remap:
				index=RemapIndex(styleBlock);
			}
			
			// Read the value from the host:
			Css.Value value=hostValue[index];
			
			if(allowInherit){
				// Just return as-is:
				return value;
			}
			
			if(value is Css.Keywords.Inherit){
				
				// Clone (Special inherit clone here):
				value=(value as Css.Keywords.Inherit).FromCopy();
				
				// Set back:
				hostValue[index]=value;
				
			}else if(value is Css.Keywords.Initial){
				
				// Clone!
				value=Target.InitialValue[index].Copy();
				
				// Set back:
				hostValue[index]=value;
				
			}
			
			return value;
			
		}
		
	}
	
}