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
	/// Represents the background: composite css property.
	/// </summary>
	
	public class BackgroundCompProperty:CssCompositeProperty{
		
		public override string[] GetProperties(){
			return new string[]{"background"};
		}
		
		public override void OnReadValue(Style styleBlock,Css.Value value){
			
			Css.Value colour=null;
			Css.Value image=null;
			Css.Value repeat=null;
			Css.Value clip=null;
			Css.Value attach=null;
			Css.Value pos=null;
			
			// Map the values into the underlying properties.
			if(value==null || value.IsType(typeof(Css.Keywords.None)) || value.IsColour){
				
				// It's just a colour on its own.
				// This is a special case because a colour is a set too.
				colour=value;
				
			}else{
				
				ReadBackground(value,true,ref colour,ref repeat,ref image,ref attach,ref pos,ref clip);
				
			}
			
			// Any that are null act like InitialValue with value's specifity.
			styleBlock.SetComposite("background-color",colour,value);
			styleBlock.SetComposite("background-repeat",repeat,value);
			styleBlock.SetComposite("background-image",image,value);
			styleBlock.SetComposite("background-attachment",attach,value);
			styleBlock.SetComposite("background-position",pos,value);
			styleBlock.SetComposite("background-clip",clip,value);
			
		}
		
		private void ReadBackground(Css.Value value,bool bottom,ref Css.Value colour,ref Css.Value repeat,ref Css.Value image,ref Css.Value attach,ref Css.Value pos,ref Css.Value clip){
			
			if(value.Type==ValueType.Image){
				image=value;
				return;
			}
			
			int count=value.Count;
			int spec=value.Specifity;
			bool xPosition=true;
			
			for(int i=0;i<count;i++){
				
				// Url?
				Css.Value innerValue=value[i];
				
				if(innerValue.Type==ValueType.Text){
					
					// Get text:
					string text=innerValue.GetText(null,this);
					
					if(text=="repeat" || text=="repeat-x" || text=="repeat-y" || text=="no-repeat"){
						
						// Background-repeat:
						repeat=innerValue;
						
					}else if(text=="content-box" || text=="padding-box" || text=="border-box"){
						
						// Background-clip:
						clip=innerValue;
						
					}else if(text=="scroll" || text=="fixed" || text=="local"){
						
						// Background-scroll:
						attach=innerValue;
						
					}else if(text=="top" || text=="bottom" || text=="left" || text=="right" || text=="center"){
						
						// background-pos:
						pos=innerValue;
						
					}else if(text=="none"){
						
						// The image:
						image=null;
						colour=null;
						
					}else if(image==null){
						
						// Url:
						image=innerValue;
						
					}
					
				}else if(innerValue.Type==ValueType.Number || innerValue.Type==ValueType.RelativeNumber){
					
					// Background-position.
					if(xPosition){
						xPosition=false;
						pos=innerValue;
					}else{
						// innerValue is Y
						ValueSet set=new ValueSet();
						set.Specifity=spec;
						set.Count=2;
						set[0]=pos;
						set[1]=innerValue;
						pos=set;
					}
					
				}else if(innerValue.Type==ValueType.Image){
					
					// Shapes etc:
					image=innerValue;
					
				}else if(innerValue.Type==ValueType.Set){
					
					if(!bottom || innerValue.IsColour){
					
						// Colour:
						colour=innerValue;
						
					}else{
						
						// A second (or third) background.
						// Note that colour is only declared once, so we share that one.
						Css.Value rep2=null;
						Css.Value image2=null;
						Css.Value attach2=null;
						Css.Value pos2=null;
						Css.Value clip2=null;
						
						ReadBackground(innerValue,false,ref colour,ref rep2,ref image2,ref attach2,ref pos2,ref clip2);
						
						Dom.Log.Add("Multiple images detected - These are only partially supported. If you want them, let us know!");
						// The first step would be writing a ValueSet to background-image, background-repeat etc.
						// You'd then need to store more than 1 background in ComputedStyle (Specifically the "Image" property would have to be a set, or ideally, you'd have a secondary set)
						// background-image etc would then need to map their set of values to the ComputedStyle set.
						// Then you'd need to render them! This would be done in ComputedStyle - wherever Image is used (e.g. Image.Layout() calls), use the ones in your set too.
						
					}
					
				}
				
			}
			
		}
		
	}
	
}



