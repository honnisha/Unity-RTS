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
using System.Text;
using Dom;


namespace Css{
	
	public enum StyleCopyMode{
		AddAlways,
		AddIfDoesntExist,
		Specifity
	}
	
	/// <summary>
	/// Holds a set of css style properties.
	/// </summary>

	public class Style{
		
		/// <summary>Creates a style from the given property block with the given parent.</summary>
		public static Style Create(string properties,Node element){
			Style style=new Style(element);
			style.cssText=properties;
			return style;
		}
		
		/// <summary>The element that this style belongs to, if any.</summary>
		public Node Element;
		
		/// <summary>The document containing this style.</summary>
		public ReflowDocument document{
			get{
				if(Element==null){
					return null;
				}
				
				return Element.document as ReflowDocument;
			}
		}
		
		/// <summary>The mapping of css property (e.g. display) to value ("none" as a <see cref="Css.Value"/>).
		/// Do not set values directly into this - use style[property]=value; instead to correctly handle aliases.</summary>
		public Dictionary<CssProperty,Value> Properties=new Dictionary<CssProperty,Value>();
		
		
		/// <summary>Creates a new style for the given element.</summary>
		/// <param name="element">The element this style is for.</param>
		public Style(Node element){
			Element=element;
		}
		
		/// <summary>Creates a new style with the given css text string seperated by semicolons.</summary>
		/// <param name="text">A css text string to apply to this style.</param>
		public Style(string text,Node element){
			Element=element;
			cssText=text;
		}
		
		/// <summary>Sets the css text of this style as a css string seperated by semicolons (;).</summary>
		public string cssText{
			set{
				Properties.Clear();
				
				if(!string.IsNullOrEmpty(value)){
					
					CssLexer lexer=new CssLexer(value,Element);
					
					LoadProperties(lexer,null);
					
				}
			}
			get{
				return ToString();
			}
		}
		
		/// <summary>Clone this style object.</summary>
		public Style Clone(){
			
			Style style=new Style(Element);
			CopyTo(style,StyleCopyMode.AddAlways);
			return style;
			
		}
		
		/// <summary>JS API for setting property values.</summary>
		public void setProperty(string property, string value){
			if(property.StartsWith("--")){
				var cssValue = Css.Value.Load(value);
				(Element.document as ReflowDocument).CssVariables[property] = cssValue;
				// TODO: Nudge the style engine to redraw all uses of the above variable.
				UnityEngine.Debug.Log("Setting a CSS variable is partially supported; this won't update all elements which use the variable. Ask if you'd like it to!");
				return;
			}
			Set(property, value);
		}
		
		/// <summary>JS API for getting property values. In PowerUI you can just use this[property] instead.</summary>
		public string getPropertyValue(string property){
			if(property.StartsWith("--")){
				Css.Value result;
				(Element.document as ReflowDocument).CssVariables.TryGetValue(property.Substring(2), out result);
				return result.ToString();
			}
			return this[property].ToString();
		}
		
		/// <summary>Copies this objects properties to the other given style, overwriting existing properties if told to do so.</summary>
		/// <param name="otherStyle">The style to copy this objects properties into.</param>
		/// <param name="overwrite">True if existing properties should be overwriten.</param>
		public void CopyTo(Style otherStyle,StyleCopyMode mode){
			if(otherStyle==null){
				return;
			}
			
			foreach(KeyValuePair<CssProperty,Value> kvp in Properties){
				
				Css.Value val=kvp.Value;
				
				if(val==null){
					continue;
				}
				
				if(mode==StyleCopyMode.AddAlways){
					
				}else if(mode==StyleCopyMode.AddIfDoesntExist){
					
					if(otherStyle.Properties.ContainsKey(kvp.Key)){
						continue;
					}
					
				}else{
					
					// Does it exist?
					Css.Value current=otherStyle[kvp.Key];
					
					if(current!=null){
						// Precedence check.
						if(val.Specifity < current.Specifity && !current.IsInherit){
							// Reject - it's less specific, unless it's inherit.
							continue;
						}
					}
					
				}
				
				otherStyle.Properties[kvp.Key]=val.Copy();
				
			}
			
		}
		
		/// <summary>Reads the properties for this style block from the lexer.
		/// Essentially treats it like a set of properties only. Terminated by }, null or &gt;.</summary>
		public void LoadProperties(CssLexer lexer,OnReadProperty onPropertyRead){
			
			// Read the properties inside a selectors block.
			lexer.SkipJunk();
			
			char current=lexer.Peek();
			
			bool readPropertyName=true;
			bool isVariable=false;
			int dashCount=0;
			
			StringBuilder propertyName=new StringBuilder();
			
			while(current!='\0' && current!='<' && current!='}'){
				
				// Read the property name:
				if(readPropertyName){
					
					// Hash, open quote or {? If so, the value does not start with a colon.
					CssUnitHandlers set;
					if(CssUnits.AllStart.TryGetValue(current,out set)){
						
						// Match on the pretext:
						Value value=set.Handle(lexer,false);
						
						if(value!=null){
							
							// Read the value (using the global instance):
							value=value.ReadStartValue(lexer);
							
							// Call ready:
							value.OnValueReady(lexer);
							
							// Get the property name:
							string pName=propertyName.ToString().ToLower().Trim();
							propertyName.Length=0;
							
							// Trigger OPR (note that it's triggered all the time):
							int status=0;
							
							if(onPropertyRead!=null){
								status=onPropertyRead(this,pName,value);
							}
							
							if(status==0){
								
								// Map property to a function:
								CssProperty property=CssProperties.Get(pName);
								
								if(property!=null){
									// Apply, taking aliases into account:
									property.OnReadValue(this,value);
								}
								
							}
							
							// Read off any junk:
							lexer.SkipJunk();
							
							// Update current:
							current=lexer.Peek();
							
							continue;
							
						}
						
					}
					
					if(current==':'){
						// Done reading the property:
						readPropertyName=false;
						dashCount=0;
						
						// Note that we don't need to skip junk as ReadValue will internally anyway.
					}else if(current=='-' && propertyName.Length==dashCount){
						dashCount++;
						
						if(dashCount==2){
							isVariable=true;
							propertyName.Length=0;
						}else{
							propertyName.Append(current);
						}
						
					}else{
						propertyName.Append(current);
					}
					
					// Read it off:
					lexer.Read();
					
					// And take a look at what's next:
					current=lexer.Peek();
					
				}else{
					
					// Read the value:
					Value value=lexer.ReadValue();
					
					string pName=propertyName.ToString().ToLower().Trim();
					propertyName.Length=0;
					
					if(isVariable){
						
						// CSS variable.
						isVariable=false;
						
						// Add to lexer set:
						lexer.AddVariable(pName,value);
						
					}else{
						
						// Trigger OPR (note that it's triggered all the time):
						int status=0;
						
						if(onPropertyRead!=null){
							status=onPropertyRead(this,pName,value);
						}
						
						if(status==0){
							
							// Map property to a function:
							CssProperty property=CssProperties.Get(pName);
							
							if(property!=null){
								
								// Apply, taking aliases into account:
								property.OnReadValue(this,value);
								
							}
							
						}
						
					}
					
					// ReadValue might actually read off the } in malformed CSS.
					// Make sure it didn't just do that:
					if(lexer.Input[lexer.Position-1]=='}'){
						lexer.Position--;
					}
					
					// Read off any junk:
					lexer.SkipJunk();
					
					// Update current:
					current=lexer.Peek();
					
					// Go into property reading mode:
					readPropertyName=true;
					
				}
				
			}
			
		}
		
		/// <summary>Gets the value of the given property, if any.</summary>
		/// <param name="cssProperty">The property to get the value of, e.g. "display".</param>
		/// <returns>The value of the property if found. Null otherwise.</returns>
		public Value Get(string cssProperty){
			return this[cssProperty];
		}
		
		/// <summary>Lets the sheet know that a value changed. Non-alias values here only.</summary>
		public void CallChange(CssProperty property,Value value){
			// Let the sheet know the value changed:
			OnChanged(property,value);
		}
		
		/// <summary>Gets the computed form of this style.</summary>
		/// <returns>The computed style.</returns>
		public virtual ComputedStyle GetComputed(){
			return null;
		}
		
		/// <summary>Sets the named property on this style to the given value.</summary>
		/// <param name="cssProperty">The property to set or overwrite. e.g. "display".</param>
		/// <param name="value">The value to set the property to, e.g. "none".</param>
		public Css.Value Set(string cssProperty,string valueText){
			
			cssProperty=cssProperty.Trim().ToLower();
			
			// Get the property:
			CssProperty property=CssProperties.Get(cssProperty);
			
			if(property==null){
				// Property not found:
				return null;
			}
			
			// The underlying value:
			Css.Value value;
			
			if(string.IsNullOrEmpty(valueText)){
				// No value - actually a clear:
				value=null;
			}else{
				// Create a lexer for the value:
				CssLexer lexer=new CssLexer(valueText,Element);
				
				// Read the underlying value:
				value=lexer.ReadValue();
			}
			
			// Apply, taking aliases into account:
			this[property]=value;
			
			return value;
			
		}
		
		/// <summary>Gets or creates the base value for the given property.
		/// The base value is essentially the value held directly in this style sheet.
		/// E.g. if the value you're setting is the R channel of color-overlay, this sets up the color-overlay value for you.</summary>
		/// <returns>The raw value (which may have just been created). Never an 'inherit' or 'initial' keyword.</returns>
		internal Css.Value GetBaseValue(CssProperty property){
			
			Css.Value propertyValue;
			
			// Does it exist already?
			if(!Properties.TryGetValue(property,out propertyValue)){
				
				// Nope! Create it now. Does the computed style hold a value instead?
				ComputedStyle computed=GetComputed();
				
				if(computed!=null && computed.Properties.TryGetValue(property,out propertyValue) && propertyValue!=null){
					
					// Derive from the computed value.
					if(propertyValue is Css.Keywords.Inherit){
						
						// Must clone the inherited value (using special inherit copy):
						propertyValue=(propertyValue as Css.Keywords.Inherit).SetCopy();
						
					}else if(propertyValue is Css.Keywords.Initial){
						
						// Clone the initial value:
						propertyValue=property.InitialValue.Copy();
						
					}else{
						
						// Must copy it:
						propertyValue=propertyValue.Copy();
						
					}
					
					// Make sure it has low specif:
					propertyValue.Specifity=-1;
					
				}else{
					
					// Needs to be created. Must also copy it.
					// Copy is used because it'll probably change some internal value.
					propertyValue=property.InitialValue.Copy();
					
				}
				
				Properties[property]=propertyValue;
				
			}else if(propertyValue is Css.Keywords.Inherit){
				
				// Must clone the inherited value (using special inherit copy):
				propertyValue=(propertyValue as Css.Keywords.Inherit).SetCopy();
				
				// Make sure it has low specif:
				propertyValue.Specifity=-1;
				
				Properties[property]=propertyValue;
				
			}else if(propertyValue is Css.Keywords.Initial){
				
				// Clone the initial value:
				propertyValue=property.InitialValue.Copy();
				
				// Make sure it has low specif:
				propertyValue.Specifity=-1;
				
				Properties[property]=propertyValue;
				
			}
			
			// If it's not currently a set, we need it as one.
			int size=property.SetSize;
			
			if(propertyValue is Css.ValueSet){
				
				if(propertyValue.Count<size){
					
					// Resize it:
					propertyValue.Count=size;
					
				}
				
			}else{
				
				// Create the set:
				Css.ValueSet set=new Css.ValueSet();
				set.Count=size;
				
				// Make sure it has low specif:
				set.Specifity=-1;
				
				for(int i=0;i<size;i++){
					
					// Must copy each value (e.g. if they get animated):
					set[i]=propertyValue.Copy();
					
				}
				
				Properties[property]=set;
				
				propertyValue=set;
			}
			
			return propertyValue;
			
		}
		
		/// <summary>Sets a property from a composite set. Any new values that are null are set to the initial value
		/// and inherit the specifity from the composite value.</summary>
		public void SetComposite(string cssProperty,Css.Value newValue,Css.Value composite){
			
			// Get the property:
			CssProperty property=CssProperties.Get(cssProperty);
			
			if(property==null){
				return;
			}
			
			if(newValue==null && composite!=null){
				
				// Use the specifity in the comps value:
				newValue=new Css.Keywords.Initial(property,composite.Specifity);
				
			}
			
			// Write it now:
			this[property]=newValue;
			
		}
		
		/// <summary>called when the named property changes.</summary>
		/// <param name="property">The property that changed.</param>
		/// <param name="newValue">It's new fully parsed value. May be null.</param>
		public virtual void OnChanged(CssProperty property,Value newValue){
		}
		
		/// <summary>Gets or sets the parsed value of this style by property name.</summary>
		/// <param name="property">The property to get the value for.</param>
		public Value this[string cssProperty]{
			
			get{
				
				// Get the property:
				CssProperty property=CssProperties.Get(cssProperty);
				
				if(property==null){
					return null;
				}
				
				return this[property];
			}
			
			set{
				// Get the CSS property:
				CssProperty property=CssProperties.Get(cssProperty);
				
				if(property==null){
					return;
				}
				
				this[property]=value;
				
			}
		}
		
		/// <summary>Gets or sets the parsed value of this style by property name.</summary>
		/// <param name="property">The property to get the value for.</param>
		public virtual Value this[CssProperty property]{
			get{
				return property.GetValue(this);
			}
			set{
				property.OnReadValue(this,value);
			}
		}
		
		/// <summary>Gets the given property as a css string. May optionally read the given inner index of it as a css string.</summary>
		/// <param name="property">The property to get as a string.</param>
		/// <param name="innerIndex">The inner value to get from the property. -1 for the whole property.</param>
		/// <returns>The property as a css string, e.g. color-overlay may return "#ffffff".</returns>
		public virtual string GetString(string cssProperty){
			
			Css.Value val=this[cssProperty];
			
			if(val==null){
				return "";
			}
			
			return val.ToString();
			
		}
		
		public override string ToString(){
			
			string result="";
		
			bool first=true;
			
			foreach(KeyValuePair<CssProperty,Css.Value> kvp in Properties){
			
				if(first){
					first=false;
				}else{
					result+=";\r\n";
				}
				
				result+=kvp.Key.Name+":"+kvp.Value.ToString();
				
			}
			
			return result;
		}
		
	}
	
}