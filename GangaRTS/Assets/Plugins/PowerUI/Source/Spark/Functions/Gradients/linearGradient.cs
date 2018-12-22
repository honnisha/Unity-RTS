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
using System.Collections;
using System.Collections.Generic;
using Blaze;
using UnityEngine;
using Css.Units;
using PowerUI;


namespace Css.Functions{
	
	/// <summary>
	/// Represents the linear-gradient css function.
	/// </summary>
	
	public class LinearGradientFunction:CssFunction{
		
		/// <summary>Attempts to load an angle from the given CSS value.</summary>
		/// <returns>float.MaxValue if it wasn't recognised as an angle.</returns>
		public static float LoadAngle(Css.Value value){
			
			float angle; // radians
			
			if(value is Css.Units.AngleUnit){
				
				// Just get the raw decimal:
				angle=value.GetRawDecimal();
				
			}else if(value is Css.ValueSet && value[0].Text=="to"){
				
				int horizontal=0;
				int vertical=0;
				
				// to ..
				for(int i=1;i<value.Count;i++){
					
					// Get the side name:
					string text=value[i].Text;
					
					switch(text){
						case "right":
							horizontal=2;
						break;
						case "left":
							horizontal=1;
						break;
						case "top":
							vertical=2;
						break;
						case "bottom":
							vertical=1;
						break;
						default:
							angle=(float)Math.PI;
							goto LoopBreaker;
					}
					
				}
				
				LoopBreaker:
				
				// Combine into a single easy to check number:
				int segment=(vertical*3) + horizontal;
				
				// yx
				// 20 == top 		    0deg => seg 6
				// 22 == top right	   45deg => seg 8
				// 02 == right 		   90deg => seg 2
				// 12 == bottom right 135deg => seg 5
				// 10 == bottom 	  180deg => seg 3
				// 11 == bottom left  225deg => seg 4
				// 01 == left 		  270deg => seg 1
				// 21 == top left 	  315deg => seg 7
				
				switch(segment){
					case 1:
						angle=Fourty5Deg * 6f; // 270
					break;
					case 2:
						angle=Fourty5Deg * 2f; // 90
					break;
					case 3:
						angle=Fourty5Deg * 4f; // 180
					break;
					case 4:
						angle=Fourty5Deg * 5f; // 225
					break;
					case 5:
						angle=Fourty5Deg * 3f; // 135
					break;
					case 6:
						angle=0f; // 0
					break;
					case 7:
						angle=Fourty5Deg * 7f; // 315
					break;
					default:
					case 8:
						angle=Fourty5Deg;
					break;
				}
				
			}else{
				angle=float.MaxValue;
			}
			
			return angle;
		}
		
		/// <summary>Loads a gradient from a set of stops in the given parameters.</summary>
		public static Gradient2D LoadGradient(int firstColour,Css.ValueSet parameters,bool autoRepeat){
			
			// Next load the stops.
			int paramCount=parameters.Count;
			int stopCount=paramCount-firstColour;
			int lastColour=paramCount-1;
			
			// Create the gradient:
			Gradient2D grad=new Gradient2D(stopCount);
			
			int index=0;
			int undefinedPoints=0;
			
			for(int i=firstColour;i<paramCount;i++){
				
				// Get the stop value:
				Css.Value stop=parameters[i];
				
				// It's either a set (colour and stop) or just a colour.
				Color colour;
				float stopValue;
				
				if(stop is Css.Units.ColourUnit){
					
					// Pull the colour:
					colour=stop.GetColour();
					
					if(i==lastColour){
						stopValue=1f;
					}else if(i==firstColour){
						stopValue=0f;
					}else{
						stopValue=float.MaxValue;
					}
					
				}else{
					
					// Pull the colour:
					colour=stop[0].GetColour();
					
					// And the stop %:
					stopValue=stop[1].GetRawDecimal();
					
				}
				
				// Drop in the colour:
				grad.Colours[index]=colour;
				
				if(stopValue==float.MaxValue){
					
					// This point doesn't have a stop value defined.
					// We'll have to load the following ones first and come back to it.
					undefinedPoints++;
					
				}else{
					
					// Define any undefined points now:
					if(undefinedPoints!=0){
						
						// First undefined point is..
						int firstUndef=index-undefinedPoints;
						
						// Get the previously defined position:
						float lastDefined=grad.Positions[firstUndef-1];
						
						// Delta is therefore..
						float delta=(stopValue-lastDefined)/ (float)(undefinedPoints+1);
						
						// For each undefined point..
						for(int undef=0;undef<undefinedPoints;undef++){
							
							// Bump up the defined amount:
							lastDefined+=delta;
							
							// Set the position:
							grad.Positions[firstUndef+undef]=lastDefined;
							
						}
						
						// Clear:
						undefinedPoints=0;
						
					}
					
					// Set the position:
					grad.Positions[index]=stopValue;
					
				}
				
				index++;
				
			}
			
			return grad;
			
		}
		
		/// <summary>45 degrees in rad.</summary>
		private const float Fourty5Deg=(float)Math.PI / 4f;
		
		/// <summary>Standard resolution of gradients, in pixels.</summary>
		public const int Resolution=50;
		
		/// <summary>The computed gradient image.</summary>
		public SparkSpecialImageFormat Image;
		/// <summary>The angle.</summary>
		public float Angle;
		
		
		public LinearGradientFunction(){
			
			Name="linear-gradient";
			Type=ValueType.Image;
			
		}
		
		/// <summary>Gets the image.</summary>
		public override ImageFormat GetImage(Css.RenderableData context,CssProperty property){
			
			if(Image==null){
				
				// Get first value (either a colour stop, angle or 'to x'):
				int firstColour=1;
				Angle=LoadAngle(this[0]);
				
				if(Angle==float.MaxValue){
					// It's the first colour. Angle is 180deg:
					Angle=(float)Math.PI;
					firstColour=0;
				}
				
				// Load the gradient:
				Gradient2D grad=LoadGradient(firstColour,this,false);
				
				// Render the gradient:
				Color32[] rendered=grad.Render32(Resolution,true).Pixels;
				Color32[] pixels=null;
				
				// Create the image now.
				Texture2D image;
				
				// If angle is perfectly vertical, or perfectly horizontal..
				if(Angle==0f || Angle==(float)Math.PI){
					
					// Vertical:
					image=new Texture2D(1,Resolution);
					
					if(Angle==(float)Math.PI){
						
						// Set the pixels as-is:
						pixels=rendered;
						
					}else{
						
						// Pixels is a new set:
						pixels=new Color32[Resolution];
						
						// Backwards:
						for(int i=0;i<Resolution;i++){
							pixels[i]=rendered[Resolution-1-i];
						}
						
					}
					
				}else if(Angle==(Fourty5Deg * 6f) || Angle==(Fourty5Deg * 2f)){
					
					// Horizontal:
					image=new Texture2D(Resolution,1);
					
					if(Angle==(Fourty5Deg * 6f)){
						
						// Set the pixels as-is:
						pixels=rendered;
						
					}else{
						
						// Pixels is a new set:
						pixels=new Color32[Resolution];
						
						// Backwards:
						for(int i=0;i<Resolution;i++){
							pixels[i]=rendered[Resolution-1-i];
						}
						
					}
					
				}else{
					
					// Any other angle:
					image=new Texture2D(Resolution,Resolution);
					
					pixels=new Color32[Resolution * Resolution];
					
					float cos = (float)System.Math.Cos(-Angle);
					float sin = (float)System.Math.Sin(-Angle);
					
					// Other angles ignored for now:
					int index=0;
					float max=(float)Resolution-1f;
					float delta=1f/(max-1f);
					float yPoint=0.5f - delta;
					
					for(int y=Resolution-1;y>=0;y--){
						
						float xPoint=-0.5f;
						
						for(int x=0;x<Resolution;x++){
							
							// int rotatedX=(int)( (xPoint * cos) - (yPoint * sin) );
							int rotatedY=(int)(( (xPoint * sin) + (yPoint * cos)) * max);
							
							if(rotatedY<0){
								rotatedY=0;
							}else if(rotatedY>=Resolution){
								rotatedY=Resolution-1;
							}
							
							pixels[index]=rendered[rotatedY];
							xPoint+=delta;
							index++;
							
						}
						
						yPoint-=delta;
					}
					
				}
				
				// Set:
				image.SetPixels32(pixels);
				
				// Flush:
				image.Apply();
				
				// Apply the image:
				Image=new SparkSpecialImageFormat();
				Image.Image=image;
				
			}
			
			return Image;
			
		}
		
		protected override Css.Value Clone(){
			LinearGradientFunction result=new LinearGradientFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override string[] GetNames(){
			return new string[]{"linear-gradient"};
		}
		
	}
	
}



