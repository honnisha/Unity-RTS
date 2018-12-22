using System;
using UnityEngine;


namespace Blaze{

	/// <summary>
	/// Used to create and represent 2D gradients.
	/// </summary>
	
	public class Gradient2D{

		public int Length=0;
		public Color[] Colours;
		public float[] Positions;
		
		
		public Gradient2D(){}
		
		public Gradient2D(int count){
			Colours=new Color[count];
			Positions=new float[count];
		}
		
		public void Add(float at,Color colour){
			Add(colour,at);
		}
		
		public void Add(float at,int r,int g,int b){
			
			Color col=new Color((float)r/255f,(float)g/255f,(float)b/255f,1f);
			
			Add(col,at);
			
		}
		
		public void Add(Color colour,float at){
			
			if(Colours==null){
				Colours=new Color[]{colour};
				Positions=new float[]{at};
			}else{
				Color[] newColours=new Color[Colours.Length+1];
				float[] newPositions=new float[Colours.Length+1];
				
				int index=0;
				bool inserted=false;
				for(int i=0;i<Colours.Length;i++){
					//Copy everything across.
					if(!inserted&&Positions[i]>at){
						inserted=true;
						newColours[index]=colour;
						newPositions[index++]=at;
					}
					
					newColours[index]=Colours[i];
					newPositions[index++]=Positions[i];
				}
				
				if(!inserted){
					newColours[index]=colour;
					newPositions[index]=at;
				}
				
				Colours=newColours;
				Positions=newPositions;
			}
			
		}
		
		
		public int IndexOf(float at){
			int index=-1;
			for(int i=0;i<Positions.Length;i++){
				if(Positions[i]>at){
					break;
				}
				index++;
			}
			return index;
		}
		
		/// <summary>Draws the gradient into a buffer of a fixed size.</summary>
		public RenderedGradient32 Render32(int pixelCount){
			return Render32(pixelCount,false);
		}
		
		/// <summary>Draws the gradient into a buffer of a fixed size.</summary>
		public RenderedGradient32 Render32(int pixelCount,bool reversed){
			if(Colours==null){
				return null;
			}
			
			Color32[] colours=new Color32[pixelCount];
			
			if(Colours.Length==1){
				for(int i=0;i<pixelCount;i++){
					colours[i]=Colours[0];
				}
			}else{
				float width=(Positions[Positions.Length-1]-Positions[0])/(pixelCount-1);
				float at=0f;
				if(reversed){
					for(int i=pixelCount-1;i>=0;i--){
						colours[i]=Render(at);
						at+=width;
					}
				}else{
					for(int i=0;i<pixelCount;i++){
						colours[i]=Render(at);
						at+=width;
					}
				}
			}
			
			return new RenderedGradient32(colours);
		}
		
		/// <summary>Draws the gradient into a buffer of a fixed size.</summary>
		public RenderedGradient Render(int pixelCount){
			return Render(pixelCount,false);
		}
		
		/// <summary>Draws the gradient into a buffer of a fixed size.</summary>
		public RenderedGradient Render(int pixelCount,bool reversed){
			if(Colours==null){
				return null;
			}
			Color[] colours=new Color[pixelCount];
			
			if(Colours.Length==1){
				for(int i=0;i<pixelCount;i++){
					colours[i]=Colours[0];
				}
			}else{
				float width=(Positions[Positions.Length-1]-Positions[0])/(pixelCount-1);
				float at=0f;
				if(reversed){
					for(int i=pixelCount-1;i>=0;i--){
						colours[i]=Render(at);
						at+=width;
					}
				}else{
					for(int i=0;i<pixelCount;i++){
						colours[i]=Render(at);
						at+=width;
					}
				}
			}
			
			return new RenderedGradient(colours);
		}
		
		public Color Render(float at){ //At varies from 0->YourMaxPosition.
			int index=IndexOf(at);
			
			if(index==Colours.Length-1||index<0){
				if(index<0){
					index=0;
				}
				return Colours[index];
			}else{
				Color a=Colours[index];
				Color b=Colours[index+1];
				float delta=((at-Positions[index])/(Positions[index+1]-Positions[index]));
				
				return new Color(a.r+(b.r-a.r)*delta,a.g+(b.g-a.g)*delta,a.b+(b.b-a.b)*delta,a.a+(b.a-a.a)*delta);
			}
		}
		
	}

}