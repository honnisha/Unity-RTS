using System;
using UnityEngine;


namespace Blaze{
	
	public partial class VectorPath{
		
		public void Transform(Matrix4x4 mat){
			
			VectorPoint current=FirstPathNode;
			
			while(current!=null){
				
				current.Transform(mat);
				current=current.Next;
				
			}
			
		}
		
	}

}