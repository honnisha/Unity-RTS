//--------------------------------------
//                Blaze
//
//        For documentation or 
//    if you have any issues, visit
//        blaze.kulestar.com
//
//    Copyright © 2014 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;


namespace Blaze{
	
	/// <summary>
	/// Holds a global set of Vector3[] int[] etc arrays. They're always a constant length.
	/// </summary>
	
	public static class MeshDataBufferPool{
		
		/// <summary>Blocks per buffer.</summary>
		public const int BlockCount=64;
		/// <summary>Standard size of the triangle buffers. Must be divisible by 6, otherwise optimisations will fail.</summary>
		public const int TriangleBufferSize=BlockCount * 6;
		/// <summary>Standard size of the vert buffers.</summary>
		public const int VertexBufferSize=BlockCount * 4;
		
		/// <summary>Pool of Vector3[],Vector2[] etc buffers.</summary>
		private static BlockBuffer FirstBuffer;
		
		
		/// <summary>Gets a vert buffer from the pool.</summary>
		public static BlockBuffer GetBuffer(){
			
			BlockBuffer result;
			
			if(FirstBuffer==null){
				
				result=new BlockBuffer();
				
				return result;
			}
			
			result=FirstBuffer;
			FirstBuffer=result.Next;
			result.Next=null;
			return result;
			
		}
		
		/// <summary>Returns one or more buffers as a linked list.</summary>
		public static void Return(BlockBuffer bufferStart,BlockBuffer bufferEnd){
			
			if(bufferStart==null){
				return;
			}
			
			// Push into pool:
			bufferEnd.Next=FirstBuffer;
			FirstBuffer=bufferStart;
			
		}
		
		/// <summary>Clears out both buffer pools, freeing some memory.</summary>
		public static void Clear(){
			
			// Empty both buffer pools:
			FirstBuffer=null;
			
		}
		
	}

}