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
using PowerUI;
using InfiniText;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the font-family: css property.
	/// </summary>
	
	public class FontFamily:CssProperty{
		
		public static FontFamily GlobalProperty;
		
		public FontFamily(){
			IsTextual=true;
			GlobalProperty=this;
			Inherits=true;
			// Note that 'none', our initial value, results in the default font being 
			// the only entry in our cached 'FontFamilyUnit' object.
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"font-family"};
		}
		
		protected override Spec.Value GetSpecification(){
			
			/*
			[ <family-name> | <generic-family> ]#
			*/
			
			return new Spec.Repeated(
				new Spec.OneOf(
					new Spec.ValueType(Css.ValueType.Text)
				)
			,1,true);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the document:
			ReflowDocument doc=style.document;
			
			// Watch out for none:
			if(!(value is Css.Units.FontFamilyUnit) && !(value.IsType(typeof(Css.Keywords.None)))){
				
				// Cache our fonts:
				style[this]=new Css.Units.FontFamilyUnit(doc,value);
				
				// Require a reload:
				return ApplyState.ReloadValue;
				
			}
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}

namespace Css.Units{
	
	/// <summary>
	/// Used by the font-family property to cache the underlying font objects in a fast array.
	/// The array is each font plus the default font.
	/// </summary>
	internal class FontFamilyUnit:Css.Value{
		
		/// <summary>The default font packaged into a font-family unit. See DefaultUnit.</summary>
		private static FontFamilyUnit DefaultUnit_;
		
		/// <summary>The default font packaged into a font-family unit.</summary>
		internal static FontFamilyUnit DefaultUnit{
			get{
				if(DefaultUnit_==null){
					DefaultUnit_=new FontFamilyUnit(null,new Css.Keywords.None());
				}
				
				return DefaultUnit_;
			}
		}
		
		/// <summary>The fallback fonts. Never any null entries by they can be "unloaded" (i.e. loading @font-face fonts).</sumary>
		public DynamicFont[] Fonts;
		
		
		public FontFamilyUnit(ReflowDocument doc,Css.Value names){
			
			// Names is a list of strings.
			int length=(names==null || names.IsType(typeof(Css.Keywords.None))) ? 0 : names.Count;
			
			Fonts=new DynamicFont[length+1];
			
			// For each font name..
			for(int i=0;i<length;i++){
				
				// Get as text:
				string fontName=names[i].Text;
				
				// Trim the name:
				fontName=fontName.Trim();
				
				// Get the font from the doc:
				DynamicFont font=doc.GetOrCreateFont(fontName);
				
				// Add to list:
				Fonts[i]=font;
				
			}
			
			// Push the default:
			Fonts[length]=DynamicFont.GetDefaultFamily();
			
		}
		
		/// <summary>Gets the best font face.</summary>
		public FontFace GetFace(int style,int synth){
			
			FontFace result=null;
			
			for(int i=0;i<Fonts.Length;i++){
				
				// Loaded?
				DynamicFont df=Fonts[i];
				
				if(df.Family!=null){
					
					result=df.Family.GetFace(style,synth);
					
					if(result!=null){
						return result;
					}
					
				}
				
			}
			
			// No suitable face found.
			return null;
			
		}
		
		/// <summary>Gets the first available "normal" face.</summary>
		/// <returns>The font face.</returns>
		public FontFace GetFirstFace(){
			
			for(int i=0;i<Fonts.Length;i++){
				
				// Loaded?
				DynamicFont df=Fonts[i];
				
				if(df.Family!=null && df.Family.Regular!=null){
					
					return df.Family.Regular;
					
				}
				
			}
			
			// This will only ever happen if the default font was removed.
			return null;
		}
		
		/// <summary>True if this family set contains the given font.</summary>
		public bool Contains(DynamicFont font){
			
			for(int i=0;i<Fonts.Length;i++){
				
				DynamicFont df=Fonts[i];
				
				if(df==font){
					return true;
				}
				
			}
			
			return false;
			
		}
		
		/// <summary>The best font to use for metrics at the moment.</summary>
		public FontFamily MetricsFont{
			get{
				
				for(int i=0;i<Fonts.Length;i++){
					
					DynamicFont df=Fonts[i];
					
					if(df.Family!=null){
						return df.Family;
					}
					
				}
				
				return null;
				
			}
		}
		
		public override string ToString(){
			
			string str="";
			
			for(int i=0;i<Fonts.Length-1;i++){
				
				if(i!=0){
					str+=",";
				}
				
				str+="\""+Fonts[i].Name+"\"";
				
			}
			
			return str;
			
		}
		
	}
	
}



