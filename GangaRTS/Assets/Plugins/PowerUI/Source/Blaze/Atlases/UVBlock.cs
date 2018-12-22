//--------------------------------------
//                Blaze
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;


namespace Blaze{
	
	/// <summary>
	/// Represents a block of four UV coordinates. Commonly shared globally.
	/// </summary>
	
	public partial class UVBlock{
	
		/// <summary>The min UV x coordinate.</summary>
		public float MinX;
		/// <summary>The min UV y coordinate.</summary>
		public float MinY;
		/// <summary>The max UV x coordinate.</summary>
		public float MaxX;
		/// <summary>The max UV y coordinate.</summary>
		public float MaxY;
		
		
		public UVBlock(){}
		
		public UVBlock(UVBlock copy){
			
			MinX=copy.MinX;
			MinY=copy.MinY;
			MaxX=copy.MaxX;
			MaxY=copy.MaxY;
			
		}
		
		public UVBlock(float minX,float maxX,float minY,float maxY){
			
			MinX=minX;
			MinY=minY;
			MaxX=maxX;
			MaxY=maxY;
			
		}
		
		/// <summary>Writes out this block of UV's to the given buffer.</summary>
		public void Write(Vector2[] buffer,int index){
			
			// Top Left:
			buffer[index]=new Vector2(MinX,MaxY);
			
			// Top Right:
			buffer[index+1]=new Vector2(MaxX,MaxY);
			
			// Bottom Left:
			buffer[index+2]=new Vector2(MinX,MinY);
			
			// Bottom Right:
			buffer[index+3]=new Vector2(MaxX,MinY);
			
		}
		
		/// <summary>True if this UV block is a globally shared one.</summary>
		public virtual bool Shared{
			get{
				return false;
			}
		}
		
		public override string ToString(){
			
			return "(MinX: "+MinX+", MinY: "+MinY+", MaxX:"+MaxX+", MaxY:"+MaxY+")";
			
		}
		
	}
	
}