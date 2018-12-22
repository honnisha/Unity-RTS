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

	public class CffSubPosition{
		
		public int Length;
		public int Position;
		
		
		public CffSubPosition(int pos,int length){
			Position=pos;
			Length=length;
		}
		
	}
	
}