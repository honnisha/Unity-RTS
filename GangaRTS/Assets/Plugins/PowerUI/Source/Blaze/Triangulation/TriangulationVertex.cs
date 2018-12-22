//--------------------------------------
//          Blaze Rasteriser
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;


namespace Blaze{
	
	/// <summary>
	/// An intermediate vertex which is used for rapid triangulation.
	/// </summary>
	
	public class TriangulationVertex{
		
		public float X;
		public float Y;
		public int Index;
		public TriangulationVertex Next;
		public TriangulationVertex Previous;
		
		
		public TriangulationVertex(Vector3 vertex,int index,bool z){
			X=vertex.x;
			
			if(z){
				Y=vertex.z;
			}else{
				Y=vertex.y;
			}
			Index=index;
		}
		
		/// <summary>Removes this vertex.</summary>
		public void Remove(){
			Previous.Next=Next;
			Next.Previous=Previous;
		}
		
	}

}