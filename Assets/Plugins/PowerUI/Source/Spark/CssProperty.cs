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


namespace Css{
	
	/// <summary>
	/// A CSS property. You can create custom ones by deriving from this class.
	/// Note that they are instanced globally.
	/// </summary>
	
	[Values.Preserve]
	public class CssProperty{
		
		/// <summary>If this property is a known set, the size of the set. This is automatically computed from aliases.</summary>
		public int SetSize;
		/// <summary>The main property name. Some properties use multiple names (e.g. content and inner-text).</summary>
		public string Name;
		/// <summary>True if this is an alias.</summary>
		public bool IsAlias;
		/// <summary>Does this css property apply to text? E.g. font-size, color etc.</summary>
		public bool IsTextual;
		/// <summary>Set if this property comes from some other namespace (e.g. "svg").</summary>
		public string NamespaceName;
		/// <summary>True if this property occurs along the x axis. E.g. width, left, right.</summary>
		public ValueAxis Axis=ValueAxis.None;
		/// <summary>What is this property relative to? Most of the time, they're relative to dimensions (e.g. 100% of width).</summary>
		public ValueRelativity RelativeTo=ValueRelativity.Dimensions;
		/// <summary>True if this is width or height only.</summary>
		public bool IsWidthOrHeight;
		/// <summary>The properties that are aliased to this one. For example, color-r is aliased to color.</summary>
		public List<CssProperty> AliasedProperties;
		/// <summary>The specification for this properties value. Use Specification instead.</summary>
		private Css.Spec.Value Specification_;
		
		
		
		/// <summary>The set of all properties that this one will handle. Usually just one.
		/// e.g. "v-align", "vertical-align".
		public virtual string[] GetProperties(){
			return null;
		}
		
		/// <summary>The value of the 'normal' keyword.</summary>
		public virtual float GetNormalValue(RenderableData context){
			return 0f;
		}
		
		/// <summary>True if this property has aliases.</summary>
		public bool HasAliases{
			get{
				return AliasedProperties!=null;
			}
		}
		
		/// <summary>Gets the aliased property for the given index. E.g. color-r is index 0 of color.</summary>
		public CssProperty GetAliased(int index){
			
			return AliasedProperties[index];
			
		}
		
		/// <summary>The specification for this properties value.</summary>
		public Css.Spec.Value Specification{
			get{
				
				if(Specification_==null){
					Specification_=GetSpecification();
				}
				
				return Specification_;
				
			}
		}
		
		/// <summary>Loads the values spec.</summary>
		protected virtual Css.Spec.Value GetSpecification(){
			return null;
		}
		
		/// <summary>Gets the aliased property for the given index. E.g. color-r is index 0 of color.</summary>
		/// <param name='orThis'>If true, returns 'this' if index is not a valid alias.</param>
		public CssProperty GetAliased(int index,bool orThis){
			
			if(AliasedProperties==null || index>=AliasedProperties.Count){
				return orThis?this:null;
			}
			
			return AliasedProperties[index];
			
		}
		
		/// <summary>The set of aliases for this property. For example, the border-radius property 
		/// can declare an alias of border-top-left which maps to e.g. "1", i.e. the first inner index.</summary>
		public virtual void Aliases(){
		}
		
		/// <summary>Loads a set of colour aliases for a given property name. E.g. name-r,name-g,name-b and name-a.
		/// The alias may also be mapped to a sub-index. It must be an empty string otherwise.</summary>
		protected void ColourAliases(string property,ValueAxis axis,int index){
			if(index==-1){
				Alias(property+"-r",axis,0);
				Alias(property+"-g",axis,1);
				Alias(property+"-b",axis,2);
				Alias(property+"-a",axis,3);
			}else{
				Alias(property+"-r",axis,index,0);
				Alias(property+"-g",axis,index,1);
				Alias(property+"-b",axis,index,2);
				Alias(property+"-a",axis,index,3);
			}
		}
		
		/// <summary>Loads a set of square aliases. E.g. name-top or name-bottom.</summary>
		protected void SquareAliases(string property,int index){
			if(index==-1){
				Alias(property+"-top",ValueAxis.Y,0);
				Alias(property+"-right",ValueAxis.X,1);
				Alias(property+"-bottom",ValueAxis.Y,2);
				Alias(property+"-left",ValueAxis.X,3);
			}else{
				Alias(property+"-top",ValueAxis.Y,index,0);
				Alias(property+"-right",ValueAxis.X,index,1);
				Alias(property+"-bottom",ValueAxis.Y,index,2);
				Alias(property+"-left",ValueAxis.X,index,3);
			}
			
			// Logical aliases:
			if(index==-1){
				LogicalAlias(property+"-block-start",ValueAxis.Y,0); 
				LogicalAlias(property+"-inline-end",ValueAxis.X,1);
				LogicalAlias(property+"-block-end",ValueAxis.Y,2);
				LogicalAlias(property+"-inline-start",ValueAxis.X,3);
			}else{
				LogicalAlias(property+"-block-start",ValueAxis.Y,index,0);
				LogicalAlias(property+"-inline-end",ValueAxis.X,index,1);
				LogicalAlias(property+"-block-end",ValueAxis.Y,index,2);
				LogicalAlias(property+"-inline-start",ValueAxis.X,index,3);
			}
			
		}
		
		/// <summary>Loads a set of colour aliases. E.g. name-r,name-g,name-b and name-a.</summary>
		protected void ColourAliases(){
			Alias(Name+"-r",ValueAxis.None,0);
			Alias(Name+"-g",ValueAxis.None,1);
			Alias(Name+"-b",ValueAxis.None,2);
			Alias(Name+"-a",ValueAxis.None,3);
		}
		
		/// <summary>Loads a set of point aliases. E.g. name-x, name-y and name-z.</summary>
		protected void PointAliases2D(){
			Alias(Name+"-x",ValueAxis.X,0);
			Alias(Name+"-y",ValueAxis.Y,1);
		}
		
		/// <summary>Loads a set of point aliases. E.g. name-x, name-y and name-z.</summary>
		protected void PointAliases3D(){
			Alias(Name+"-x",ValueAxis.X,0);
			Alias(Name+"-y",ValueAxis.Y,1);
			Alias(Name+"-z",ValueAxis.None,2);
		}
		
		/// <summary>Loads a set of square aliases. E.g. name-top or name-bottom.</summary>
		protected void SquareAliases(){
			Alias(Name+"-top",ValueAxis.Y,0);
			Alias(Name+"-right",ValueAxis.X,1);
			Alias(Name+"-bottom",ValueAxis.Y,2);
			Alias(Name+"-left",ValueAxis.X,3);
			
			// Logical aliases:
			LogicalAlias(Name+"-block-start",ValueAxis.Y,0); 
			LogicalAlias(Name+"-inline-end",ValueAxis.X,1);
			LogicalAlias(Name+"-block-end",ValueAxis.Y,2);
			LogicalAlias(Name+"-inline-start",ValueAxis.X,3);
			
		}
		
		/// <summary>Adds a logical alias for the given CSS property to the given target.
		/// A numeric target, e.g. 1, means that inner index of this property, which is a set.</summary>
		protected void LogicalAlias(string name,ValueAxis axis,params int[] target){
			
			DirectionAwareProperty alias=new DirectionAwareProperty(this,target);
			alias.Axis=axis;
			AddAlias(name,alias);
		}
		
		/// <summary>Adds an alias for the given CSS property to the given target.
		/// A numeric target, e.g. 1, means that inner index of this property, which is a set.</summary>
		protected void Alias(string name,ValueAxis axis,params int[] target){
			
			CssPropertyAlias alias=new CssPropertyAlias(this,target);
			alias.Axis=axis;
			AddAlias(name,alias);
		}
		
		/// <summary>True if this property is for internal use only.</summary>
		public virtual bool Internal{
			get{
				return false;
			}
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public virtual bool NonStandard{
			get{
				return false;
			}
		}
		
		private void AddAlias(string name,CssPropertyAlias alias){
			
			alias.RelativeTo=RelativeTo;
			
			alias.Name=name;
			CssProperties.All[name]=alias;
			
			// Add to property set:
			int index=alias.Index[0];
			
			if(index==-1){
				// Logical - don't add.
				return;
			}
			
			if(AliasedProperties==null){
				AliasedProperties=new List<CssProperty>();
			}
			
			if(AliasedProperties.Count==index){
				// Just add it - most of the time it'll fall in here.
				AliasedProperties.Add(alias);
				return;
			}
			
			// Grow the aliased properties set if it's not currently big enough:
			for(int x=AliasedProperties.Count;x<=index;x++){
				AliasedProperties.Add(null);
			}
			
			AliasedProperties[index]=alias;
			
		}
		
		/// <summary>Apply this CSS style to the given computed style.
		/// Note that you can grab the element from the computed style if you need that.</summary>
		/// <param name="style">The computed style to apply the property to.</param>
		/// <param name="value">The new value being applied.</param>
		public virtual ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>Apply this CSS style to the given computed style.
		/// Note that you can grab the element from the computed style if you need that.</summary>
		/// <param name="style">The computed style to apply the property to.</param>
		/// <param name="value">The new value being applied.</param>
		public virtual void ApplyText(TextRenderingProperty text,RenderableData data,ComputedStyle style,Value value){
			
		}
		
		/// <summary>Called when a value is read from the CSS stream.</summary>
		public virtual void OnReadValue(Style styleBlock,Css.Value value){
			
			if(value==null){
				
				// Remove the property:
				styleBlock.Properties.Remove(this);
				
			}else{
				
				// If it's inherit or initial then apply property:
				if(value is Css.Keywords.Inherit){
				
					// Apply property:
					(value as Css.Keywords.Inherit).SetProperty(this);
					
				}else if(value is Css.Keywords.Initial){
					
					// Apply property:
					(value as Css.Keywords.Initial).Property=this;
					
				}
				
				// Simply push the value into the block:
				styleBlock.Properties[this]=value;
				
			}
			
			// Call the change method:
			styleBlock.CallChange(this,value);
			
		}
		
		/// <summary>Called to get a this properties value from the given style.
		/// This exists as it may be an alias property.</summary>
		public virtual Css.Value GetValue(Style styleBlock){
			
			// Simply pull the value from the block:
			Css.Value result;
			styleBlock.Properties.TryGetValue(this,out result);
			return result;
			
		}
		
		/// <summary>Called to get a this properties value from the given style.
		/// This overload should not be used with alias properties - it always returns the 'host' value.</summary>
		public Css.Value GetOrCreateValue(Node context,Style styleBlock,bool allowInherit){
			
			Css.Value result;
			GetOrCreateValue(context,styleBlock,allowInherit,out result);
			return result;
			
		}
		
		/// <summary>Called to get a this properties value from the given style.
		/// This exists as it may be an alias property.</summary>
		/// <param name="hostValue">This can return aliased values. The 'host' value is the actual property.
		/// Think the value of 'margin' (host) when calling GetValue on 'margin-left' (returned).</param>
		public virtual Css.Value GetOrCreateValue(Node context,Style styleBlock,bool allowInherit,out Css.Value hostValue){
			
			// Pull the value from the block:
			Css.Value result;
			styleBlock.Properties.TryGetValue(this,out result);
			
			if(result==null){
				
				// Does it inherit?
				if(Inherits){
					
					// Create the keyword:
					Css.Keywords.Inherit inherit=new Css.Keywords.Inherit(context,this);
					
					if(allowInherit){
						
						// Yep - inherit is our result:
						result=inherit;
						
					}else{
						
						// Clone it instead (using special inherit copy):
						result=inherit.SetCopy();
						
					}
					
				}else{
					
					// Apply the initial value (must copy because it's likely to be changed):
					result=InitialValue.Copy();
					
				}
				
				// Add it:
				OnReadValue(styleBlock,result);
				
			}
			
			// Host is the same as result:
			hostValue=result;
			
			return result;
			
		}
		
		/// <summary>Shared default values.</summary>
		internal static Css.Value NONE=new Css.Keywords.None();
		internal static Css.Value ZERO=new Css.Units.DecimalUnit(0);
		internal static Css.Value AUTO=new Css.Keywords.Auto();
		
		/// <summary>True if this property inherits. Defaults to false.</summary>
		public bool Inherits;
		/// <summary>The initial value for this property. Defaults to a decimal 0 (equiv of 'none' or just '0').</summary>
		public Css.Value InitialValue=NONE;
		
		/// <summary>A convenience property for setting up initial values.</summary>
		public string InitialValueText{
			set{
				InitialValue=Css.Value.Load(value);
			}
		}
		
		/// <summary>Causes the named property to apply its value.</summary>
		public void Reapply(ComputedStyle computed,string propertyName){
			
			// Get the property:
			CssProperty property=CssProperties.Get(propertyName);
			
			if(property==null){
				return;
			}
			
			// Got a value?
			Value value=computed[property];
			
			if(value==null){
				return;
			}
			
			// Apply:
			property.Apply(computed,value);
			
		}
		
		/// <summary>Sets the named css property from the given style if the property exists in the style.</summary>
		/// <param name="property">The css property, e.g. color.</param>
		/// <param name="style">The style to load value of the property from. This should be the computed style for the parent element.</param>
		public void Reapply(ComputedStyle style,CssProperty property){
			// Get the current value:
			Value value=style[property];
			
			if(value!=null){
				// Apply it:
				property.Apply(style,value);
			}
		}
		
		/// <summary>Call this if the current property requires a background image object.</summary>
		public BackgroundImage GetBackground(ComputedStyle style){
			
			RenderableData rd=style.RenderData;
			BackgroundImage image=rd.BGImage;
			
			if(image==null){
				rd.BGImage=image=new BackgroundImage(rd);
			}
			
			return image;
		}
		
		/// <summary>Call this if the current property requies a border object.</summary>
		public BorderProperty GetBorder(ComputedStyle style){
			
			RenderableData rd=style.RenderData;
			BorderProperty border=rd.Border;
			
			if(border==null){
				rd.Border=border=new BorderProperty(rd);
			}
			
			return border;
		}
		
		/// <summary>Call this if the current property requires a text object. NOTE: This one may be null.</summary>
		public TextRenderingProperty GetText(ComputedStyle style){
			
			// Grab it:
			return style.RenderData.Text;
			
		}
		
		/// <summary>Call this if the current property requires a text object. NOTE: This one may be null.</summary>
		public TextRenderingProperty3D GetText3D(ComputedStyle style){
			
			// Grab it:
			return style.RenderData.Text3D;
			
		}
		
	}
	
}