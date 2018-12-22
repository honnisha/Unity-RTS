using System;
using UnityEngine;


namespace Loonim{
	
	/// <summary>
	/// Some nodes in CPU mode have to cache their complete input in order to apply their effect.
	/// An example is blur. These are 'bitmap' nodes.
	/// </summary>
	
	public class BitmapNode : TextureNode{
		
		/// <summary>Maps 0 to 1 x into 0 to (Width-1).</summary>
		public double MapX;
		/// <summary>Maps 0 to 1 y into 0 to (Height-1).</summary>
		public double MapY;
		/// <summary>Raster width.</summary>
		public int Width;
		/// <summary>Raster height.</summary>
		public int Height;
		
		/// <summary>The raw buffer.</summary>
		public Color[] Buffer;
		/// <summary>Linked list of modules to clear.</summary>
		public BitmapNode NextToClear;
		
		
		public BitmapNode(){}
		
		public BitmapNode(int src):base(src){}
		
		public void Clear(DrawInfo info){
			
			Buffer=null;
			
		}
		
		/// <summary>Sets up the buffer and mapX/mapY values (used in CPU mode).</summary>
		protected void Setup(DrawInfo info){
			
			int size=info.PixelCount;
			
			if(Buffer==null || Buffer.Length!=size){
				
				// Create or resize:
				Buffer=new UnityEngine.Color[size];
				
			}
			
			MapX=info.DeltaX;
			MapY=info.DeltaY;
			
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Sample from Buffer.
			x*=MapX;
			y*=MapY;
			
			int xIndex=(int)x;
			int yIndex=(int)y;
			
			if(xIndex<0){
				xIndex=0;
			}else if(xIndex>=Width){
				xIndex=Width-1;
			}
			
			if(yIndex<0){
				yIndex=0;
			}else if(yIndex>=Height){
				yIndex=Height-1;
			}
			
			return Buffer[(yIndex*Width) + xIndex];
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
			// Col intensity:
			UnityEngine.Color col1=GetColour(x,y);
            return col1.r + col1.g + col1.b / 3.0;
			
		}
		
        public override double GetValue(double x, double y, double z){
			
			// Col intensity:
			UnityEngine.Color col1=GetColour(x,y);
            return col1.r + col1.g + col1.b / 3.0;
			
		}
		
        public override double GetValue(double x, double y){
			
			// Col intensity:
			UnityEngine.Color col1=GetColour(x,y);
            return col1.r + col1.g + col1.b / 3.0;
			
        }
		
	}
	
}