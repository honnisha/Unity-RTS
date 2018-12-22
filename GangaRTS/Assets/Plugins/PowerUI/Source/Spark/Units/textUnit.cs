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
using System.Text;


namespace Css.Units{
	
	/// <summary>
	/// Represents an instance of an textual value.
	/// </summary>
	
	public class TextUnit:CssUnit{
		
		
		public string RawValue;
		
		
		public TextUnit(){
			
			// It's a text value by default:
			Type=ValueType.Text;
			
		}
		
		public TextUnit(string text):this(){
			RawValue=text;
		}
		
		public override bool Equals(Value value){
			
			if(value==null){
				return false;
			}
			
			TextUnit tu=value as TextUnit;
			
			if(tu==null){
				return false;
			}
			
			return tu.RawValue==RawValue;
			
		}
		
		public override Value ReadStartValue(CssLexer lexer){
			
			// Skip quote:
			char quote=lexer.Read();
			char current=lexer.Read();
			bool delimMode=false;
			StringBuilder result=lexer.Builder;
			result.Length=0;
			
			while((delimMode || current!=quote) && current!='\0'){
				
				if(delimMode){
					
					if(current==quote){
						result.Append(current);
						delimMode=false;
					}else if(current=='\n' || current=='\f'){
						// Ignore it - act like it's not there.
						delimMode=false;
					}else if(current=='\r'){
						// Must we ignore an \n too?
						current=lexer.Peek();
						
						if(current=='\n'){
							// Yep - Ignore it too:
							lexer.Read();
						}
						
						delimMode=false;
					}else{
						
						// Unicode character? [0-9a-f]{1,6}
						delimMode=false;
						int charIndex=(int)current;
						bool unicode=false;
						int charcode=0;
						
						for(int i=0;i<6;i++){
							
							if(	charIndex>=(int)'0' && charIndex<=(int)'9' ){
								
								// Apply to charcode:
								charcode=(charcode<<4) | (charIndex-(int)'0');
								
								// Move on:
								current=lexer.Read();
								charIndex=(int)current;
								unicode=true;
								
							}else if( charIndex>=(int)'a' && charIndex<=(int)'f' ){
								
								// Apply to charcode:
								charcode=(charcode<<4) | (charIndex+10-(int)'a');
								
								// Move on:
								current=lexer.Read();
								charIndex=(int)current;
								unicode=true;
								
							}else{
								// No longer valid unicode.
								break;
							}
							
						}
						
						if(unicode){
							result.Append(char.ConvertFromUtf32(charcode));
							
							// Don't read another character, unless the current one should be ignored.
							if(current=='\r'){
								
								current=lexer.Peek();
								
								if(current=='\n'){
									lexer.Read();
								}
								
							}else if(current==' ' || current=='\n' || current=='\t' || current=='\f'){
								
								// Ignore this char.
								
							}else{
								// Append it:
								continue;
							}
							
						}else{
							// It's any other character:
							result.Append(current);
						}
						
					}
					
				}else if(current=='\\'){
					
					delimMode=true;
					
				}else{
					
					result.Append(current);
					
				}
				
				current=lexer.Read();
			}
			
			return new TextUnit(result.ToString());
			
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			// try it as a float:
			float result;
			float.TryParse(RawValue,out result);
			return result;
		}
		
		public override string[] PreText{
			get{
				return new string[]{"\"","'"};
			}
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			return RawValue;
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			return (!string.IsNullOrEmpty(RawValue) && RawValue!="0" && RawValue!="false" && RawValue!="no" && RawValue!="off");
		}
		
		public override string ToString(){
			return RawValue;
		}
		
		protected override Value Clone(){
			TextUnit result=new TextUnit();
			result.RawValue=RawValue;
			return result;
		}
		
	}
	
}



