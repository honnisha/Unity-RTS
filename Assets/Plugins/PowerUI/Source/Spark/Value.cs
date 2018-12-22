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
using System.Globalization;
using System.Collections.Generic;
using PowerUI;


namespace Css{

	/// <summary>
	/// This represents a parsed value of a css property.
	/// </summary>

	public partial class Value:IEnumerable<Value>{
		
		/// <summary>A vertical CSS property. Used when needing to specify that a % is on Y.</summary>
		private static CssProperty VerticalProperty;
		
		/// <summary>A horizontal CSS property. Used when needing to specify that a % is on X.</summary>
		private static CssProperty HorizontalProperty;
		
		/// <summary>A depth CSS property. Used when needing to specify that a % is on Z.</summary>
		private static CssProperty DepthProperty;
		
		/// <summary>Represents an empty value.</summary>
		internal static readonly Value Empty=new Units.DecimalUnit(0);
		
		
		public static Css.Value Load(string text){
			
			CssLexer lexer=new CssLexer(text,null);
			Css.Value v=lexer.ReadValue();
			
			if(v==null){
				v=Empty;
			}
			
			return v;
			
		}
		
		/// <summary>True if this value is important.</summary>
		public bool Important;
		/// <summary>How specific this value is. A low specifity value should never overwrite a high specifity one.
		/// By default it's a very high specifity (but not so high that it overflows if it's "important"!)</summary>
		public int Specifity=1<<28;
		/// <summary>The type of value this is.</summary>
		protected ValueType Type_=ValueType.Null;
		
		public ValueType Type{
			get{
				return Type_;
			}
			protected set{
				Type_=value;
			}
		}
		
		
		/// <summary>Creates a new empty value.</summary>
		public Value(){}
		
		/// <summary>Some CSS values are sets of entries.
		/// This attempts to select the first entry of the given type.</summary>
		public Value GetByType(ValueType type){
			
			// For each value in this..
			foreach(Value innerValue in this){
				
				if(innerValue==null){
					continue;
				}
				
				if(innerValue.Type==type){
					return innerValue;
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>When a unit declares that it has a start identifier, such as #, this reads the rest of it's value.</summary>
		public virtual Value ReadStartValue(CssLexer lexer){
			return null;
		}
		
		/// <summary>Some CSS values are sets of entries.
		/// This attempts to select the first entry of the given type.</summary>
		public Value GetByTypes(params ValueType[] types){
			
			int typeCount=types.Length;
			
			// For each value in this..
			foreach(Value innerValue in this){
				
				if(innerValue==null){
					continue;
				}
				
				for(int i=0;i<typeCount;i++){
					if(innerValue.Type==types[i]){
						return innerValue;
					}
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Some CSS values are sets of entries.
		/// This attempts to select the entry without the named attribute.</summary>
		public Value GetEntryWithoutAttribute(string funcName){
			
			Value attrib=null;
			
			if(IsCommaArray){
				
				// For each value in this..
				foreach(Value innerValue in this){
					
					// Find the one which is "tagged" with the given values.
					attrib=innerValue[funcName];
					
					if(attrib==null){
						
						// Got it!
						return innerValue;
						
					}
					
				}
				
			}else{
				
				// If not, it actually is the attribute to check.
				attrib=this[funcName];
				
				if(attrib!=null){
					// Nope!
					return null;
				}
				
				// It's actually the parent we're returning:
				return this;
				
			}
			
			return null;
			
		}
		
		/// <summary>Some CSS values are sets of entries.
		/// This attempts to select the entry which has an attribute which matches one of the given values.</summary>
		public Value GetEntryWithAttribute(string funcName,params string[] values){
			
			// How many values?
			int valueCount=values.Length;
			
			if(IsCommaArray){
				
				// For each value in this..
				foreach(Value innerValue in this){
					
					// Is this innerValue a set?
					Value attrib=null;
					
					// Find the one which is "tagged" with the given values.
					attrib=innerValue[funcName];
					
					if(attrib==null){
						continue;
					}
					
					// Get it as a text value:
					string text=attrib.Text;
					
					// Got a match?
					for(int i=0;i<valueCount;i++){
						
						if(text==values[i]){
							
							// Got it!
							return innerValue;
							
						}
						
					}
					
				}
				
			}else{
				
				// Find the one which is "tagged" with the given values.
				Value attrib=this[funcName];
				
				if(attrib==null){
					return null;
				}
				
				// Get it as a text value:
				string text=attrib.Text;
				
				// Got a match?
				for(int i=0;i<valueCount;i++){
					
					if(text==values[i]){
						
						// It's actually the parent we're returning:
						return this;
						
					}
					
				}
				
			}
			
			return null;
			
		}
		
		/// <summary>Checks if this value is a,comma,array.</summary>
		public bool IsCommaArray{
			get{
				ValueSet set=this as ValueSet;
				
				if(set==null){
					return false;
				}
				
				return (set.Spacer==",");
				
			}
		}
		
		/// <summary>Sets the specifity for this and all child values.</summary>
		public void SetSpecifity(int val){
			
			if(val==-1){
				// Use default/ unchanged:
				return;
			}
			
			if(Important){
				Specifity=val|(1<<29);
			}else{
				Specifity=val;
			}
			
			if(this is Css.Keywords.Inherit || this is Css.Keywords.Initial){
				return;
			}
			
			foreach(Value value in this){
				if(value==this || value==null){
					continue;
				}
				value.SetSpecifity(val);
			}
			
		}
		
		/// <summary>Sets this and all child values as being important.</summary>
		public void SetImportant(bool important){
			
			if(Important!=important){
				
				if(important){
					Specifity+=150000;
				}else{
					Specifity-=150000;
				}
				
			}
			
			Important=important;
			
			if(this is Css.Keywords.Inherit || this is Css.Keywords.Initial){
				// Don't set to internal values.
				return;
			}
			
			foreach(Value value in this){
				if(value==this){
					continue;
				}
				value.SetImportant(important);
			}
			
		}
		
		/// <summary>Checks if this is a function.</summary>
		/// <returns>True if this value is a function.</returns>
		public virtual bool IsFunction{
			get{
				return false;
			}
		}
		
		/// <summary>Checks if this is the 'auto' keyword</summary>
		/// <returns>True if this value is 'auto'.</returns>
		public virtual bool IsAuto{
			get{
				return false;
			}
		}
		
		/// <summary>Checks if this is a particular type. Note that this is always false for inherit/ initial
		/// (as they pass the type through them).</summary>
		/// <returns>True if this value is of the given type.</returns>
		public virtual bool IsType(Type type){
			return (GetType()==type);
		}
		
		/// <summary>Checks if this is a suitable colour.</summary>
		public virtual bool IsColour{
			get{
				return false;
			}
		}
		
		/// <summary>True if this is a 'cached' value. See CachedIntegerUnit.</summary>
		public virtual bool IsCached{
			get{
				return false;
			}
		}
		
		/// <summary>The original value of a cached object.</summary>
		public virtual Css.Value CachedOrigin{
			get{
				return null;
			}
		}
		
		/// <summary>Checks if this is an absolute value and is not a percentage/em/ rectangle containing percents.</summary>
		/// <returns>True if this value is absolute; false if it is relative.</returns>
		public virtual bool IsAbsolute{
			get{
				
				if(Type==ValueType.RelativeNumber || IsInherit || IsAuto){
					return false;
				}
				
				return true;
				
			}
		}
		
		/// <summary>Called after a value has been loaded from the stream. Functions etc get it too.
		/// This is used to, for example, map a value to a faster internal representation.</summary>
		public virtual void OnValueReady(CssLexer lexer){
		}
		
		/// <summary>Copies this value.</summary>
		public Value Copy(){
			Value v=Clone();
			
			if(v!=null){
				v.Specifity=Specifity;
				v.Important=Important;
			}
			
			return v;
		}
		
		/// <summary>Duplicates this value.</summary>
		/// <returns>A duplicated copy of this value. Note that if this value has inner values, they are copied too.</returns>
		protected virtual Value Clone(){
			return null;
		}
		
		/// <summary>Used for locating e.g. a function contained within a set.</summary>
		public virtual string Identifier{
			get{
				return null;
			}
		}
		
		/// <summary>Obtains the underlying decimal value, if there is one.</summary>
		public virtual float GetRawDecimal(){
			return 0f;
		}
		
		/// <summary>Sets a raw decimal value to this object. Used by the animation system.</summary>
		public virtual void SetRawDecimal(float value){
		}
		
		/// <summary>If this is a decimal, the raw decimal value. This is generally the main output.</summary>
		public virtual float GetDecimal(RenderableData context,CssProperty property){
			return 0f;
		}
		
		/// <summary>Creates vertical and horizontal CSS properties which are used when needing to specify which
		/// axis a % value is relative to.</summary>
		private static void CreateAxisProperties(){
			
			// Create the vertical property:
			VerticalProperty=new CssProperty();
			VerticalProperty.Axis=ValueAxis.Y;
			
			// Create the horizontal property:
			HorizontalProperty=new CssProperty();
			HorizontalProperty.Axis=ValueAxis.X;
			
			// Z property:
			DepthProperty=new CssProperty();
			DepthProperty.Axis=ValueAxis.Z;
			
		}
		
		/// <summary>If this is a decimal, the raw decimal value. This is generally the main output.
		/// This overload explicitly tells it to use the vertical axis or not (e.g. when resolving %).</summary>
		public float GetDecimal(RenderableData context,ValueAxis axis){
			
			// Create the width or height property:
			if(VerticalProperty==null){
				CreateAxisProperties();
			}
			
			CssProperty property=(axis==ValueAxis.Y) ? VerticalProperty : (axis==ValueAxis.X) ? HorizontalProperty : DepthProperty;
			property.RelativeTo=ValueRelativity.SelfDimensions;
			
			// Get decimal using them:
			return GetDecimal(context,property);
		}
		
		/// <summary>If this is a decimal, the raw decimal value. This is generally the main output.
		/// This overload explicitly tells it to use the vertical axis or not (e.g. when resolving %).</summary>
		public float GetDecimal(RenderableData context,ValueAxis axis,ValueRelativity rel){
			
			// Create the width or height property:
			if(VerticalProperty==null){
				CreateAxisProperties();
			}
			
			CssProperty property=(axis==ValueAxis.Y) ? VerticalProperty : (axis==ValueAxis.X) ? HorizontalProperty : DepthProperty;
			property.RelativeTo=rel;
			
			// Get decimal using them:
			return GetDecimal(context,property);
			
		}
		
		/// <summary>Text is e.g. "afile.svg#something". This is either 'something' or null.</summary>
		public string Hash{
			get{
				
				// Get text:
				string t=Text;
				
				// Split at the hash:
				string[] pieces=t.Split('#');
				
				if(pieces.Length>1){
					return pieces[1];
				}
				
				return null;
				
			}
		}
		
		/// <summary>Gets context-free text such as font family names.</summary>
		public string Text{
			get{
				return GetText(null,null);
			}
		}
		
		/// <summary>If this is a text value, e.g. "auto", the raw text value.</summary>
		public virtual string GetText(RenderableData context,CssProperty property){
			return "";
		}

		/// <summary>If this is a boolean, the raw bool value.</summary>
		public virtual bool GetBoolean(RenderableData context,CssProperty property){
			return false;
		}
		
		/// <summary>If this is a pixel integer, the raw pixel value.</summary>
		public int GetInteger(RenderableData context,CssProperty property){
			return (int)GetDecimal(context,property);
		}

		/// <summary>The number of internal values.</summary>
		public virtual int Count{
			get{
				// 1 means just this.
				return 1;
			}
			set{}
		}
		
		public virtual Value this[int index]{
			get{
				
				// By returning this actual object, we can auto wrap.
				return this;
			}
			set{
			}
		}

		
		public virtual Value this[string index]{
			get{
				
				if(index=="" || index=="value"){
					return this;
				}
				
				// Find the entry with an identifier of index:
				foreach(Value value in this){
					
					if(value.Identifier==index){
						return value;
					}
					
				}
				
				return null;
			}
			set{
			}
		}
		
		/// <summary>Is this an inheriting value?</summary>
		public virtual bool IsInherit{
			get{
				return false;
			}
		}
		
		/// <summary>Checks if two values are equal.</summary>
		/// <param name="value">The value to check for equality with this. Always returns false if null.</param>
		/// <returns>True if this and the given value are equal; false otherwise.</returns>
		public virtual bool Equals(Value value){
			if(value==null || value.GetType()!=GetType()){
				return false;
			}
			
			return true;
		}
		
		/// <summary>Make this value a set. If it is already a set, the value is returned unchanged.
		/// Otherwise, a set of 1 is created.</summary>
		public ValueSet ToSet(){
			
			if(this is ValueSet){
				
				return this as ValueSet;
				
			}
			
			// Set of 1:
			return new ValueSet(new Css.Value[]{this});
			
		}
		
		/// <summary>Converts this value to a Unity Vector3.</summary>
		public Vector3 GetVector(RenderableData context,CssProperty property){
			return new Vector3(
				this[0].GetDecimal(context,property.GetAliased(0)),
				this[1].GetDecimal(context,property.GetAliased(1)),
				this[2].GetDecimal(context,property.GetAliased(2))
			);
		}
		
		/// <summary>Converts this value to a Unity Quaternion.</summary>
		public Quaternion GetQuaternion(RenderableData context,CssProperty property){		
			return Quaternion.Euler(
				this[0].GetDecimal(context,property.GetAliased(0)) * Mathf.Rad2Deg,
				this[1].GetDecimal(context,property.GetAliased(1)) * Mathf.Rad2Deg,
				this[2].GetDecimal(context,property.GetAliased(2)) * Mathf.Rad2Deg
			);
		}
		
		/// <summary>Gets the value as an image, if it is one.</summary>
		public virtual ImageFormat GetImage(RenderableData context,CssProperty property){
			return null;
		}
		
		/// <summary>Converts this value into a suitable selector matcher.
		/// Typically either a local matcher or a structural one.</summary>
		public virtual SelectorMatcher GetSelectorMatcher(){
			return null;
		}
		
		/// <summary>Gets this value as a vector path (or null if it isn't one).</summary>
		public virtual Blaze.VectorPath GetPath(RenderableData context,CssProperty property){
			
			return null;
			
		}
		
		/// <summary>Converts this value to a context-free Unity colour.</summary>
		/// <returns>The unity colour represented by this value.</returns>
		public Color GetColour(){
			return GetColour(null,null);
		}
		
		/// <summary>Converts this value to a Unity colour.</summary>
		/// <returns>The unity colour represented by this value.</returns>
		public Color GetColour(RenderableData context,CssProperty property){
			
			float r;
			float g;
			float b;
			float a;
			
			// If we actually hold less than 4 values, the value read is always 255.
			// We do this because alpha must not get wrapped, and because defaulting white is more useful.
			// (e.g. if you only declare rgb, the ValueSet handler will wrap r as alpha).
			int count=Count;
			
			if(count<1){
				r=1f;
			}else{
				r=this[0].GetDecimal(context,property);
			}
			
			if(count<2){
				g=1f;
			}else{
				g=this[1].GetDecimal(context,property);
			}
			
			if(count<3){
				b=1f;
			}else{
				b=this[2].GetDecimal(context,property);
			}
			
			if(count<4){
				a=1f;
			}else{
				a=this[3].GetDecimal(context,property);
			}
			
			return new Color(r,g,b,a);
		}
		
		/// <summary>Resolves through e.g. inherit and initial.</summary>
		public virtual Css.Value Computed{
			get{
				return this;
			}
		}
		
		/// <summary>Checks if this is the same set of one or more functions as the other value.
		/// Note that this only checks if they are the same functions in the same order.
		/// It's not a complete equiv check (and its used by the animation system to interpolate CSS transform).</summary>
		public bool FunctionalEquals(Css.Value other){
			
			if(this is Css.CssFunction){
				
				// other must also be the same one.
				return (other.GetType()==GetType());
				
			}
			
			if(other.Count!=Count){
				return false;
			}
			
			// Count can be just 1 to represent 'this':
			for(int i=0;i<Count;i++){
				
				Css.Value a=this[i];
				Css.Value b=other[i];
				
				// Must both exist:
				if(a==null || b==null){
					return false;
				}
				
				// Same type of function:
				if(a.GetType()!=b.GetType() || !(a is Css.CssFunction)){
					return false;
				}
				
			}
			
			return true;
			
		}
		
		/// <summary>Converts this value to a css useable string.</summary>
		/// <returns>A css formatted string.</returns>
		public override string ToString(){
			return "";
		}
		
		/// <summary>Are all entries in this rectangle the same?</summary>
		public bool AllSameValues(){
			
			Value first=null;
			
			foreach(Value value in this){
				
				if(value==this){
					// It's actually empty.
					return true;
				}
				
				if(first==null){
					first=value;
				}
				
				if(value==null){
					
					if(first!=null){
						return false;
					}
					
					continue;
				}
				
				if(!value.Equals(first)){
					return false;
				}
				
			}
			
			return true;
			
		}
		
		/// <summary>Converts this value into a hex string that is 2 characters long.</summary>
		public virtual string HexString{
			get{
				return "00";
			}
		}
		
		IEnumerator IEnumerable.GetEnumerator(){
			return GetEnumerator();
		}
		
		public virtual IEnumerator<Value> GetEnumerator(){
			
			// Just return this:
			yield return this;
			
		}
		
	}
	
}