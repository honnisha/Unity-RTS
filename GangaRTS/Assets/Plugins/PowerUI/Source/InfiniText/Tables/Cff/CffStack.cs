//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//   Kulestar would like to thank the following:
//    PDF.js, Microsoft, Adobe and opentype.js
//    For providing implementation details and
// specifications for the TTF and OTF file formats.
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.IO;


namespace InfiniText{

	public class CffStack{
		
		public int Length;
		public CffStackEntry Last;
		public CffStackEntry First;
		
		
		public void Push(int value){
			
			Push((float)value);
			
		}
		
		public bool IsOdd{
			get{
				return ((Length&1)==1);
			}
		}
		
		public bool Empty{
			get{
				return (Length==0);
			}
		}
		
		public void Push(float value){
			
			// Create the entry:
			CffStackEntry entry=new CffStackEntry(value);
			
			Length++;
			
			if(First==null){
				
				First=Last=entry;
				
			}else{
				
				entry.Previous=Last;
				Last=Last.Next=entry;
				
			}
			
		}
		
		public void Clear(){
			
			First=null;
			Last=null;
			Length=0;
			
		}
		
		public float Shift(){
			
			// Remove first one:
			CffStackEntry toRemove=First;
			
			// Update first:
			First=toRemove.Next;
			
			if(First==null){
				
				Last=null;
				Length=0;
				
			}else{
				First.Previous=null;
			
				Length--;
			}
			
			return toRemove.Value;
			
		}
		
		public float Pop(){
			
			// Remove last one:
			CffStackEntry toPop=Last;
			
			// Update last:
			Last=toPop.Previous;
			
			if(Last==null){
				
				First=null;
				Length=0;
				
			}else{
				Last.Next=null;
				
				Length--;
			}
			
			return toPop.Value;
			
		}
		
	}
	
}