using System;
using UnityEngine;


namespace Blaze{
	
	/// <summary>
	/// Used to create and represent 3D gradients.
	/// </summary>
	
	public class Gradient3D{
		private int PixelCount;
		public float[] Positions;
		public Gradient2D[] Gradients;
		public RenderedGradient[] RenderedGradients;

		
		public Gradient3D(int pixelCount){
			PixelCount=pixelCount; //The height the gradients should be rendered.
		}
		
		public void Add(Gradient2D gradient,float at){
			RenderedGradient rendition=gradient.Render(PixelCount);
			
			if(Gradients==null){
				
				Gradients=new Gradient2D[]{gradient};
				RenderedGradients=new RenderedGradient[1]{rendition};
				Positions=new float[]{at};
					
			}else{
				
				Gradient2D[] newGradients=new Gradient2D[Gradients.Length+1];
				RenderedGradient[] newRenders=new RenderedGradient[Gradients.Length+1];
				float[] newPositions=new float[Gradients.Length+1];
				
				int index=0;
				bool inserted=false;
				
				for(int i=0;i<Gradients.Length;i++){
					//Copy everything across.
					if(!inserted&&Positions[i]>at){
						inserted=true;
						newGradients[index]=gradient;
						newRenders[index]=rendition;
						newPositions[index++]=at;
					}
					
					newGradients[index]=Gradients[i];
					newRenders[index]=RenderedGradients[i];
					newPositions[index++]=Positions[i];
				}
				
				if(!inserted){
					newGradients[index]=gradient;
					newRenders[index]=rendition;
					newPositions[index]=at;
				}
				
				Gradients=newGradients;
				Positions=newPositions;
				RenderedGradients=newRenders;
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
		
		/// <summary>Renders a slice of this gradient at the given point.</summary>
		/// <param name="at">The percentage along this gradient to render. Variest from 0->100.</param>
		public RenderedGradient Render(float at){
			int index=IndexOf(at);
			
			if(index==Gradients.Length-1||index<0){
				
				if(index<0){
					index=0;
				}
				
				return RenderedGradients[index];
				
			}else{
				
				RenderedGradient gradientA=RenderedGradients[index];
				RenderedGradient gradientB=RenderedGradients[index+1];
				
				float delta=((at-Positions[index])/(Positions[index+1]-Positions[index]));
				
				Color[] newPixels=new Color[PixelCount];
				
				for(int i=0;i<PixelCount;i++){
					Color a=gradientA[i];
					Color b=gradientB[i];
					
					newPixels[i]=new Color(a.r+(b.r-a.r)*delta,a.g+(b.g-a.g)*delta,a.b+(b.b-a.b)*delta,a.a+(b.a-a.a)*delta);
				}
				
				return new RenderedGradient(newPixels);
				
			}
			
		}
		
	}

}