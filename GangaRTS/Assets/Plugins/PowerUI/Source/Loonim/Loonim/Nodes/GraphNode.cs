//--------------------------------------
//       Loonim Image Generator
//    Partly derived from LibNoise
//    See License.txt for more info
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;


namespace Loonim{
	
    /// <summary>
    /// Represents a node for graph data.
    /// </summary>
    public class GraphNode:TextureNode{
		
		public GraphNode(int size):base(size){}
		
		public GraphNode():base(0){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			float value=(float)GetValue(x);
			
			return new UnityEngine.Color(value,value,value,1f);
			
		}
		
		public override double GetWrapped(double x,double y,int wrap){
			
			return GetValue(x);
			
		}
		
		public override double GetValue(double x,double y){
			
			return GetValue(x);
			
		}
		
		public override double GetValue(double x,double y,double z){
			
			return GetValue(x);
			
		}
		
		public override double GetValue(double t){
			
			return 0;
			
		}
		
	}
	
}