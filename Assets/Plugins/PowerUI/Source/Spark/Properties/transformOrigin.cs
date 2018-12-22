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

using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the transform-origin: css property.
	/// </summary>
	
	public class TransformOrigin:CssProperty{
		
		public static TransformOrigin GlobalProperty;
		
		public TransformOrigin(){
			GlobalProperty=this;
			RelativeTo=ValueRelativity.Self;
			InitialValueText="50% 50% 0";
		}
		
		public override string[] GetProperties(){
			return new string[]{"transform-origin"};
		}
		
		public override void Aliases(){
			PointAliases3D();
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			// E.g. 50% 10% 100%, 0px 0px 0px.
			
			// Got a transform?
			Transformation transform=style.TransformX;
			
			if(transform!=null){
				
				// Set new origin:
				transform.SetOriginOffset(value);
				
				// Update it:
				style.RequestPaint();
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



