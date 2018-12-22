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
	/// Represents an instance of the inherit keyword.
	/// </summary>
	
	public class CssUnitHandlers{
		
		/// <summary>The character that this set "handles".</summary>
		public char Character;
		/// <summary>The CSS value at this "level".</summary>
		public Css.Value Value;
		/// <summary>Essentially a graph of sub-handlers. Linear scans is faster than a hashtable.</summary>
		public List<CssUnitHandlers> Handlers;
		
		
		/// <summary>Attempts to handle the given lexer with this units set.</summary>
		public Css.Value Handle(CssLexer lexer,bool removeAll){
			
			// By peeking, get as far as we can until a delim of any kind is reached, or we can't go any further.
			// e.g. 41px*50% - the first will terminate successfully at the x.
			
			// Note that we know the current peek matches character.
			CssUnitHandlers handler=this;
			
			int currentIndex=1;
			
			while(true){
				
				// Get the char:
				char letter=lexer.Peek(currentIndex);
				currentIndex++;
				
				// Got a handler for it?
				if(handler.Handlers==null){
					// Nope! handler.Value, if there is one, is the furthest we can go.
					
					break;
				}
				
				int handlerSetCount=handler.Handlers.Count;
				
				// Linear scan is faster than a hashtable here.
				for(int i=0;i<handlerSetCount;i++){
					
					if(handler.Handlers[i].Character==letter){
						
						// Got it! Keep going.
						handler=handler.Handlers[i];
						goto NextLetter;
						
					}
					
				}
				
				// No letter matches. Stop there.
				break;
				
				NextLetter:
				continue;
				
			}
			
			if(handler.Value!=null){
				// Read off the amount we read now.
				
				int start=2;
				
				if(removeAll){
					start=1;
				}
				
				for(int i=start;i<currentIndex;i++){
					lexer.Read();
				}
				
			}
			
			return handler.Value;
			
		}
		
		public void Add(string text,Css.Value value){
			
			// Get the letters of the text, e.g. px:
			char[] pieces=text.ToCharArray();
			
			// Run through the "tree" of handlers, creating them. This is to maximise parse speed.
			CssUnitHandlers handlerSet=this;
			
			for(int i=1;i<pieces.Length;i++){
				
				char current=pieces[i];
				
				handlerSet=handlerSet.RequireSet(current);
				
			}
			
			// Apply the value:
			handlerSet.Value=value;
			
		}
		
		public CssUnitHandlers RequireSet(char character){
			
			if(Handlers==null){
				// Create the map:
				Handlers=new List<CssUnitHandlers>(1);
			}
			
			// Already got a set for this character? Linear scan is faster than a hashtable here.
			int count=Handlers.Count;
			
			for(int i=0;i<count;i++){
				if(Handlers[i].Character==character){
					return Handlers[i];
				}
			}
			
			// Create the set:
			CssUnitHandlers set=new CssUnitHandlers();
			set.Character=character;
			Handlers.Add(set);
			
			return set;
			
		}
		
	}
	
}



