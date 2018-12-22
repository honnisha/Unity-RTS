//--------------------------------------
//       Loonim Image Generator
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

// FFT Ported from Java by Kulestar.
// Original Java source copyright 2005 Huxtable.com. 
// All rights reserved.

using UnityEngine;
using System.Collections;

namespace Loonim{

	public class Kernel{
		
		public int Rows;
		public int Columns;
		public float[] Matrix;
		
		
		public Kernel(float[] matrix){
			Matrix=matrix;
			Rows=3;
			Columns=3;
		}
		
		public Kernel(int r,int c,float[] matrix){
			Matrix=matrix;
			Rows=r;
			Columns=c;
		}
		
	}
	
}
