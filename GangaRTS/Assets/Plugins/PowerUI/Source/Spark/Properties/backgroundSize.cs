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

using System;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the background-size: css property.
	/// </summary>
	
	public class BackgroundSize:CssProperty{
		
		public static CssProperty GlobalProperty;
		public static CssProperty GlobalPropertyX;
		public static CssProperty GlobalPropertyY;
		
		
		public BackgroundSize(){
			
			RelativeTo=ValueRelativity.SelfDimensions;
			GlobalProperty=this;
			InitialValue=AUTO;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"background-size"};
		}
		
		public override void Aliases(){
			// A set of point aliases, e.g. background-size-x:
			PointAliases2D();
			
			GlobalPropertyX=GetAliased(0);
			GlobalPropertyY=GetAliased(1);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the background image:
			Css.BackgroundImage image=GetBackground(style);
			
			if(value==null || value.IsType(typeof(Css.Keywords.Auto))){
				image.SizeX=null;
				image.SizeY=null;
			}else if(value.Type==Css.ValueType.Text){
				
				if(value.Text=="initial"){
					// Same as the default:
					image.SizeX=null;
					image.SizeY=null;
					
				}else if(value.Text=="cover"){
					// Same as 100% on both axis:
					image.SizeX=Css.Value.Load("100%");
					image.SizeY=Css.Value.Load("100%");
					
				}
				
			}else{
				// It's a vector:
				image.SizeX=value[0];
				image.SizeY=value[1];
			}
			
			// Request a layout:
			image.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}



