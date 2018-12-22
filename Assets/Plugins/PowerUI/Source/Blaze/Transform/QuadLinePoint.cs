using System;
using UnityEngine;


namespace Blaze{
	
	public partial class QuadLinePoint{
		
		public override void Transform(Matrix4x4 by){
			
			// Transform x/y:
			Vector3 transformed=by*new Vector3(X,Y,0f);
			
			X=transformed.x;
			Y=transformed.y;
			
			// Control 1:
			transformed=by*new Vector3(Control1X,Control1Y,0f);
			
			Control1X=transformed.x;
			Control1Y=transformed.y;
			
		}
		
	}

}