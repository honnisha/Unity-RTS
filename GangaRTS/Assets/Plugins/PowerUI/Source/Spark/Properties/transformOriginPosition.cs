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
	/// Represents the transform-origin-position: css property.
	/// </summary>
	
	public class TransformOriginPosition:CssProperty{
		
		/// <summary>Converts a CSS value into an origin position.</summary>
		public static int GetPosition(Css.Value value){
			
			int pos=PositionMode.Relative;
			
			if(value!=null){
				
				switch(value.Text){
				
					case "fixed":
						
						pos=PositionMode.Fixed;
					
					break;
					case "absolute":
					
						pos=PositionMode.Absolute;
					
					break;
					
				}
				
			}
			
			return pos;
			
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public static TransformOriginPosition GlobalProperty;
		
		public TransformOriginPosition(){
			GlobalProperty=this;
		}
		
		public override string[] GetProperties(){
			return new string[]{"transform-origin-position"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			// E.g. relative, fixed.
			
			// Got a transform?
			Transformation transform=style.TransformX;
			
			if(transform!=null){
				
				// Update origin position:
				transform.OriginPosition=GetPosition(value);
				
				// Paint away!
				style.RequestPaint();
				
			}
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



