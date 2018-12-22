//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using Blaze;


namespace InfiniText{
	
	/// <summary>
	/// A node which stores unloaded glyph information.
	/// These are used to help improve glyph load time without causing additional memory usage.
	/// This is because they act like a point - the only point in the path.
	/// </summary>
	
	public class LoadMetaPoint:VectorPoint{
		
		public int Start;
		public int Length;
		
		
		/// <summary>Creates a new meta point.</summary>
		public LoadMetaPoint(int start,int length):base(0f,0f){
			Start=start;
			Length=length;
		}
		
		public override bool Unloaded{
			get{
				return true;
			}
		}
		
		public override string ToString(){
			return "[Path not loaded]";
		}
		
	}
	
}