//--------------------------------------
//	   Loonim Image Generator
//	Partly derived from LibNoise
//	See License.txt for more info
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using Blaze;
using System.Collections;
using System.Collections.Generic;


namespace Loonim{
	
	/// <summary>
	/// Fast stacking of images with their default blending.
	/// </summary>
	
	public class Stack: TextureNode{
		
		public Stack(int size):base(size){}
		
		/// <summary>A stack which auto grows.</summary>
		public Stack():base(0){}
		
		/// <summary>Adds to the stack.</summary>
		public void Add(TextureNode node){
			AddSource(node);
		}
		
		internal override int OutputDimensions{
			get{
				// 2D image.
				return 2;
			}
		}
		
		#if !NO_BLADE_RUNTIME
		
		public override DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			// Stack required.
			
			// Allocate a target stack now:
			int targetStack=stackID;
			DrawStack stack=tex.GetStack(targetStack,info);
			stackID++;
			
			// Allocate sources all to use the exact same stack for their output:
			
			for(int i=0;i<Sources.Length;i++){
				
				// Get the input node:
				TextureNode input=Sources[i];
				
				// Allocate it now, always allocating the same one as our target:
				int inputStacks=targetStack;
				DrawStackNode dsn=input.Allocate(info,tex,ref inputStacks);
				
				// If it's cleared, set cleared to false for anything but i=0:
				if(i!=0){
					dsn.Clear=false;
				}
				
			}
			
			// Create our node:
			StackerStackNode matNode=DrawStore as StackerStackNode;
			
			if(matNode==null){
				matNode=new StackerStackNode();
				DrawStore=matNode;
			}
			
			matNode.Stack=stack;
			
			return matNode;
			
		}
		
		#endif
		
		public override int TypeID{
			get{
				return 115;
			}
		}
		
	}
}
