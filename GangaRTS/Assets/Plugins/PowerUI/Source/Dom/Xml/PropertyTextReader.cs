//--------------------------------------
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;

namespace Dom{

	/// <summary>
	/// Reads properties/ attributes from a lexer.
	/// These are of the form propertyName="propertyValue", propertyName=value or singleProperty (i.e. no value).
	/// </summary>

	public static class PropertyTextReader{
		
		/// <summary>Reads a property and its value from the given lexer.</summary>
		/// <param name="lexer">The lexer to read the property from.</param>
		/// <param name="selfClosing">True if the tag being read is a self closing one.</param>
		/// <param name="property">The name of the property that was read.</param>
		/// <param name="value">The value of the property, if there was one. Null otherwise.</param>
		public static void Read(StringReader lexer,System.Text.StringBuilder builder,bool selfClosing,out string property,out string value){
			
			SkipSpaces(lexer);
			char peek=lexer.Peek();
			int x=0;
			
			while(peek!=StringReader.NULL && peek!=' ' && peek!='=' && peek!='>' &&(peek!='/' || x==0)){
				x++;
				peek=lexer.Peek(x);
			}
			
			// Get the property:
			property=lexer.ReadString(x).ToLower();
			
			SkipSpaces(lexer);
			peek=lexer.Peek();
			
			if(peek=='='){
				// Here comes the value!
				lexer.Read();
				SkipSpaces(lexer);
				peek=lexer.Peek();
				
				if(peek==StringReader.NULL){
					
					// Read the value:
					value=builder.ToString();
					
					// Clear it:
					builder.Length=0;
					
					return;
				}else if(peek=='"'||peek=='\''){
					ReadString(lexer,builder);
				}else{
					char character=lexer.Peek();
					
					// Read until we hit a junk character or >
					while(!HtmlLexer.IsSpaceCharacter(character) && character!='\0' && character!='>' && character!='/'){
						
						if(character=='/' && lexer.Peek(1)=='>'){
							// End only if this is a self-closing tag.
							
							if(selfClosing){
								
								// Yep! Halt:
								value=builder.ToString();
								
								// Clear it:
								builder.Length=0;
								
								return;
							}
						}
						
						lexer.Read();
						builder.Append(character);
						character=lexer.Peek();
						
					}
				}
			}
			
			SkipSpaces(lexer);
			value=builder.ToString();
			
			// Clear it:
			builder.Length=0;
			
		}
		
		/// <summary>Reads a "string" or a 'string' from the lexer. Delimiting can be done with a backslash.</summary>
		/// <param name="lexer">The lexer the string will be read from.</param>
		/// <param name="builder">The builder to read it into.</summary>
		public static void ReadString(StringReader lexer,System.Text.StringBuilder builder){
			
			char quote=lexer.Read();
			char character=lexer.Read();
			bool delimited=false;
			
			while(delimited || character!=quote && character!='\0'){
				
				if(character=='\\'&&!delimited){
					delimited=true;
				}else{
					delimited=false;
					builder.Append(character);
				}
				
				character=lexer.Read();
				
			}
			
		}
		
		/// <summary>Skips any whitespaces that occur next in the given lexer.</summary>
		/// <param name="lexer">The lexer to skip spaces within.</param>
		public static void SkipSpaces(StringReader lexer){
			char peek=lexer.Peek();
			
			while(HtmlLexer.IsSpaceCharacter(peek)){
				lexer.Read();
				peek=lexer.Peek();
			}
			
		}
		
	}
	
}