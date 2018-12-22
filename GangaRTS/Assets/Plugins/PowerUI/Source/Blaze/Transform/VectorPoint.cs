using System;
using UnityEngine;


namespace Blaze{
	
	public partial class VectorPoint{
		
		public virtual void Transform(Matrix4x4 by){
			
			// Transform x/y:
			Vector3 transformed=by*new Vector3(X,Y,0f);
			
			X=transformed.x;
			Y=transformed.y;
			
		}
		
	}
	
}