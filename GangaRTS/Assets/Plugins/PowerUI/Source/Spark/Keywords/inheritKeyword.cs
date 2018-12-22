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
using Dom;


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the inherit keyword.
	/// </summary>
	
	public class Inherit:CssKeyword{
		
		/// <summary>The object being inherited from. Note that this is never null.</summary>
		private Css.Value From_;
		/// <summary>The context it's being inherited from.</summary>
		private RenderableData Context;
		/// <summary>The property that is inheriting.</summary>
		private CssProperty Property;
		
		
		/// <summary>The object being inherited from. Note that this is never null.</summary>
		public Css.Value From{
			get{
				return From_;
			}
		}
		
		/// <summary>Note that you'll need to ensure ReEvaluate is called if you use this.
		/// Only the parser uses this one (via reflection during load).</summary>
		public Inherit(){}
		
		/// <summary>Optionally set a specifity.</summary>
		public Inherit(CssProperty property,int specifity){
			Property=property;
			Specifity=specifity;
		}
		
		public Inherit(CssProperty property){
			Property=property;
		}
		
		public Inherit(Node context,CssProperty property){
			ReEvaluate(context,property);
		}
		
		/// <summary>
		/// Updates the property which is used as a fallback if there is nothing to inherit from.
		/// </summary>
		public void SetProperty(CssProperty prop){
			if(prop==null){
				return;
			}
			
			Property=prop;
		}
		
		/// <summary>Updates the value that is being inherited.
		/// If you don't have the value available then use ReEvaluate.</summary>
		public void SetFrom(RenderableData context,Css.Value value){
			Inherit inher=value as Inherit;
			
			if(inher==null){
				From_=value;
				Context=context;
			}else{
				// Collapse the cascade - Pull the inner value instead:
				From_=inher.From;
				Context=inher.Context;
			}
			
			if(From_!=null){
				Type=From_.Type;
			}
			
		}
		
		/// <summary>Resolves through e.g. inherit and initial.</summary>
		public override Css.Value Computed{
			get{
				if(From_==null){
					return Property.InitialValue;
				}
				return From_;
			}
		}
		
		/// <summary>Checks if this is the 'auto' keyword</summary>
		/// <returns>True if this value is 'auto'.</returns>
		public override bool IsAuto{
			get{
				if(From_==null){
					return Property.InitialValue.IsAuto;
				}
				return From_.IsAuto;
			}
		}
		
		/// <summary>Converts this value into a hex string that is 2 characters long.</summary>
		public override string HexString{
			get{
				if(From_==null){
					return Property.InitialValue.HexString;
				}
				return From_.HexString;
			}
		}
		
		/// <summary>Checks if this is a particular type. Note that this is always false for inherit/ initial
		/// (as they pass the type through them).</summary>
		/// <returns>True if this value is of the given type.</returns>
		public override bool IsType(Type type){
			if(From_==null){
				return Property.InitialValue.GetType()==type;
			}
			return From_.GetType()==type;
		}
		
		/// <summary>Copies the inherited value. If from is a set, this returns a set of inherit values.</summary>
		public Css.Value FromCopy(){
			
			if(From_==null){
				return Property.InitialValue.Copy();
			}
			
			if(!(From_ is Css.ValueSet) || Property.AliasedProperties==null){
				return From_.Copy();
			}
			
			// Deep set copy:
			return SetCopy();
			
		}
		
		/// <summary>A special variant of clone which checks if the inherited value is a set 
		/// (and if it is a set, this returns a set of inherits).</summary>
		public Css.Value SetCopy(){
			
			// Either we're inheriting nothing, it's not a set, or the property is atomic:
			if(From_==null || !(From_ is Css.ValueSet) || Property.AliasedProperties==null){
				
				// Ordinary copy:
				return Copy();
				
			}
			
			// Create a set of inherits:
			int count=Count;
			Css.ValueSet vs=Activator.CreateInstance(From_.GetType()) as Css.ValueSet;
			vs.Count=count;
			
			for(int i=0;i<count;i++){
				
				// Get the aliased property (e.g. color-r):
				CssProperty aliasedProperty=Property.GetAliased(i,false);
				
				if(aliasedProperty==null){
					
					// Use the original value:
					vs[i]=From_[i];
					
				}else{
					
					// Create:
					Inherit inh=new Inherit(aliasedProperty);
					
					// Get the actual value:
					inh.From_=From_[i];
					
					// Add to set:
					vs[i]=inh;
					
				}
				
			}
			
			return vs;
			
		}
		
		public void ReEvaluate(Node context,CssProperty property){
			
			Property=property;
			IRenderableNode rdr=(context.parentNode_ as IRenderableNode);
			
			if(rdr!=null){
				
				// Inherit right now:
				Context=rdr.RenderData;
				From_=rdr.ComputedStyle[property];
				
				Inherit inher=From_ as Inherit;
				
				if(inher!=null){
					
					// Collapse the cascade - Pull the inner value instead:
					From_=inher.From;
					Context=inher.Context;
					
				}
				
			}
			
			if(From_==null){
				From_=property.InitialValue;
			}
			
			if(Context==null){
				Context=(context as IRenderableNode).RenderData;
			}
			
			Type=From_.Type;
			
		}
		
		protected override Value Clone(){
			Inherit inherit=new Inherit(Property);
			inherit.From_=From_;
			inherit.Context=Context;
			return inherit;
		}
		
		public override bool IsInherit{
			get{
				return true;
			}
		}
		
		public override string Name{
			get{
				return "inherit";
			}
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			if(From_==null){
				// Initial:
				return Property.InitialValue.GetDecimal(context,property);
			}
			
			// ValueScale is actually context.ValueScale;
			float valueScale=0f;
			
			if(Context!=null){
				valueScale=Context.ValueScale;
				Context.ValueScale=context.ValueScale;
			}
			
			// Get now:
			float result=From_.GetDecimal(Context,property);
			
			if(Context!=null){
				// Restore:
				Context.ValueScale=valueScale;
			}
			
			return result;
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			
			if(From_==null){
				// Initial:
				return Property.InitialValue.GetText(context,property);
			}
			
			return From_.GetText(Context,property);
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			
			if(From_==null){
				// Initial:
				return Property.InitialValue.GetBoolean(context,property);
			}
			
			return From_.GetBoolean(Context,property);
		}
		
		/// <summary>Gets the value as an image, if it is one.</summary>
		public override PowerUI.ImageFormat GetImage(RenderableData context,CssProperty property){
			
			if(From_==null){
				// Initial:
				return Property.InitialValue.GetImage(context,property);
			}
			
			return From_.GetImage(Context,property);
		}
		
		public override IEnumerator<Value> GetEnumerator(){
			
			if(From_==null){
				
				// Initial:
				foreach(Value value in Property.InitialValue){
					
					yield return value;
					
				}
				
			}else{
				
				foreach(Value value in From_){
					
					yield return value;
					
				}
				
			}
			
		}
		
		private void Readonly(){
			throw new Exception("Inherit is readonly. Clone the object before trying to write to it.");
		}
		
		public override int Count{
			get{
				if(From_==null){
					return Property.InitialValue.Count;
				}
				
				return From_.Count;
			}
			set{
				Readonly();
			}
		}
		
		public override Value this[int index]{
			get{
				if(From_==null){
					return Property.InitialValue[index];
				}
				
				return From_[index];
			}
			set{
				Readonly();
			}
		}
		
	}
	
}



