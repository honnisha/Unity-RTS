using System;
using UnityEngine;


namespace Blaze{
	
	public partial class CurveLinePoint{
		
		public override void Transform(Matrix4x4 by){
			
			// Transform x/y:
			Vector3 transformed=by*new Vector3(X,Y,0f);
			
			X=transformed.x;
			Y=transformed.y;
			
			// Control 1:
			transformed=by*new Vector3(Control1X,Control1Y,0f);
			
			Control1X=transformed.x;
			Control1Y=transformed.y;
			
			// Control 2:
			transformed=by*new Vector3(Control2X,Control2Y,0f);
			
			Control2X=transformed.x;
			Control2Y=transformed.y;
			
		}
		
	}
	
}