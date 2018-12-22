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
	/// Treats a string of characters as a stream. This allows it to be read one character at a time.
	/// Attempting to read after the end of the stream will generate a <see cref="Dom.StringReader.NULL"/> character.
	/// </summary>
	
	public class StringReader{
		
		/// <summary>The null character. This is returned when operations are working beyond the end of the stream.</summary>
		public static char NULL='\0';
		
		/// <summary>The original string.</summary>
		public string Input;
		/// <summary>The current position this reader is at in the string.</summary>
		public int Position;
		/// <summary>The length of the input string.</summary>
		public int InputLength;
		
		
		/// <summary>Creates a new reader for the raw single-byte encoded string.
		/// Useful if you're talking to e.g. a webserver with a binary protocol.</summary>
		/// <param name="str">The set of characters that is encoded as one byte per character.</param>
		public StringReader(byte[] str){
			if(str!=null){
				
				// Get the length:
				InputLength=str.Length;
				
				// Create the char set:
				char[] chars=new char[InputLength];
				
				// Transfer each one..
				for(int i=InputLength-1;i>=0;i--){
					chars[i]=(char)str[i];
				}
				
				// Create the input string:
				Input=new string(chars);
			}
		}
		
		/// <summary>Creates a new reader for the given string.</summary>
		/// <param name="str">The string to treat as a stream of characters.</param>
		public StringReader(string str){
			if(str!=null){
				Input=str;
				InputLength=str.Length;
			}
		}
		
		/// <summary>Checks if there is anything left to read.</summary>
		/// <returns>True if there is more content to read.</returns>
		public bool More(){
			return (Position<InputLength);
		}
		
		/// <summary>Checks if the given string is next.</summary>
		public bool Peek(string str){
			
			for(int i=0;i<str.Length;i++){
				
				int index=Position+i;
				
				if(index>=InputLength || Input[index]!=str[i]){
					return false;
				}
				
			}
			
			return true;
			
		}
		
		/// <summary>Checks if the given string is next; it checks by lowercasing the target character.</summary>
		public bool PeekLower(string str){
			
			for(int i=0;i<str.Length;i++){
				
				int index=Position+i;
				
				if(index>=InputLength || char.ToLower(Input[index])!=str[i]){
					return false;
				}
				
			}
			
			return true;
			
		}
		
		/// <summary>Takes a peek at the next character in the stream without reading it.</summary>
		public char Peek(){
			return Peek(0);
		}
		
		/// <summary>Takes a peek at the character that is a number of characters away from the next one
		/// without actually reading it. Peek(0) is the next character, Peek(1) is the one after that etc.</summary>
		public char Peek(int delta){
			if(Position+delta>=InputLength){
				return NULL;
			}
			
			return Input[Position+delta];
		}
		
		/// <summary>Steps back one place in the stream.</summary>
		public void StepBack(){
			if(Position<=0){
				return;
			}
			
			Position--;
		}
		
		/// <summary>Steps forward one place in the stream.</summary>
		public void Advance(){
			if(Position<InputLength){
				Position++;
			}
		}
		
		/// <summary>Steps forward the given number of places in the stream.</summary>
		public void Advance(int places){
			Position+=places;
			
			if(Position>InputLength){
				// Clip it - can only be equal to the length.
				Position=InputLength;
			}
		}
		
		/// <summary>The length of the string.</summary>
		public int Length(){
			return InputLength;
		}
		
		/// <summary>Reads a substring of the given length.
		/// Note that this does not do bounds checking.</summary>
		public string ReadString(int length){
			string str=Input.Substring(Position,length);
			Position+=length;
			return str;
		}
		
		/// <summary>Reads a character from the stream and advances the stream one place.</summary>
		public virtual char Read(){
			if(Position>=InputLength){
				return NULL;
			}
			
			return Input[Position++];
		}
		
		/// <summary>Keeps reading the given character from the stream until it's no longer next.
		/// Used for e.g. stripping an unknown length block of whitespaces in the stream.</summary>
		/// <param name="character">The character to read off from this stream.</param>
		public void ReadUntil(char character){
			while(Peek()!=character){
				Advance();
			}
			Advance();
		}
		
		/// <summary>Keeps reading from the stream until no characters in the given set are next.
		/// Used for e.g. stripping an unknown number of newlines (\n or \r) from this stream.</summary>
		/// <param name="chars">The set of characters to read off.</param>
		public void ReadOff(char[] chars){
			int count=0;
			ReadOff(chars,out count);
		}
		
		/// <summary>Keeps reading from the stream until no characters in the given set are next.
		/// Used for e.g. stripping an unknown number of newlines (\n or \r) from this stream.</summary>
		/// <param name="chars">The set of characters to read off.</param>
		/// <param name="count">The number of characters that were read from the stream.</param>
		public void ReadOff(char[] chars,out int count){
			count=0;
			bool read=true;
			while(read){
				char peek=Peek();
				read=false;
				
				for(int i=0;i<chars.Length;i++){
					if(peek==chars[i]){
						count++;
						Advance();
						read=true;
						break;
					}
				}
			}
		}
		
		/// <summary>Gets the next index of the given character.
		/// The length is returned if it wasn't found at all.</summary>
		public int NextIndexOf(char character){
			
			for(int index=Position;index<InputLength;index++){
				
				if(Input[index]==character){
					return index;
				}
				
			}
			
			return InputLength;
			
		}
		
		/// <summary>Gets the next index of the given character, up to limit.
		/// Limit is returned if it wasn't found at all.</summary>
		public int NextIndexOf(char character,int limit){
			
			for(int index=Position;index<limit;index++){
				
				if(Input[index]==character){
					return index;
				}
				
			}
			
			return limit;
			
		}
		
		/// <summary>Gets the next index of the given character, up to limit.
		/// Limit is returned if it wasn't found at all.</summary>
		public static int NextIndexOf(int position,string input,char character,int limit){
			
			for(int index=position;index<limit;index++){
				
				if(input[index]==character){
					return index;
				}
				
			}
			
			return limit;
			
		}
		
		/// <summary>Gets the next index of the given character.
		/// The length is returned if it wasn't found at all.</summary>
		public static int NextIndexOf(int position,string input,char character){
			
			int l=input.Length;
			
			for(int index=position;index<l;index++){
				
				if(input[index]==character){
					return index;
				}
				
			}
			
			return l;
			
		}
		
		/// <summary>Gets the line number that the pointer is currently at.</summary>
		/// <returns>The current line number, starting from 1.</returns>
		public virtual int GetLineNumber(){
			int junk;
			return GetLineNumber(out junk);
		}
		
		/// <summary>Gets the line number and character number that the pointer is currently at.</summary>
		/// <param name="charOnLine">The column/ character number the reader is at on this line.</param>
		/// <returns>The current line number, starting from 1.</returns>
		public int GetLineNumber(out int charOnLine){
			int result=1;
			charOnLine=1;
			
			int max=Position+1;
			
			if(max>InputLength){
				max=InputLength;
			}
			
			for(int i=0;i<max;i++){
				if(Input[i]=='\n'){
					result++;
					charOnLine=1;
				}else{
					charOnLine++;
				}
			}
			
			return result;
		}
		
		/// <summary>Reads the numbered line from this stream.</summary>
		/// <param name="lineNumber">The number of the line to read.</param>
		/// <returns>The numbered line as a string.</returns>
		public string ReadLine(int lineNumber){
			int line=1;
			string result="";
			
			for(int i=0;i<InputLength;i++){
				
				if(line<lineNumber){
					if(Input[i]=='\n'){
						line++;
					}
				}else{
					
					if(Input[i]=='\n'){
						return result;
					}
					
					result+=Input[i];
				}
				
			}
			
			return result;
		}
		
	}
	
}