using System;
using UnityEngine;

namespace Loonim{

    /// <summary>
    /// Remaps the incoming data according to a given tone curve.
    /// </summary>
    public class ToneMap : Std2InputNode {
		
        /// <summary>The tone mapping graph.</summary>
        public TextureNode ToneGraph{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>A baked mapping buffer. Always 256 long.</summary>
		public float[] MappingBuffer;
		
		public ToneMap(){}
		
		public ToneMap(TextureNode src,TextureNode graph){
			SourceModule=src;
			ToneGraph=graph;
		}
		
		public override void Prepare(DrawInfo info){
			
			if(MappingBuffer==null){
				
				// Presample the graph:
				MappingBuffer=ToneGraph.Bake(256);
				
			}
			
			// Prepare sources:
			base.Prepare(info);
			
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Tone map RGB - Red:
			int index=(int)(col1.r * 255f);
			
			// Clip:
			if(index<0){
				index=0;
			}else if(index>255){
				index=255;
			}
			
			// Read:
			col1.r=MappingBuffer[index];
			
			// Green:
			
			index=(int)(col1.g * 255f);
			
			// Clip:
			if(index<0){
				index=0;
			}else if(index>255){
				index=255;
			}
			
			// Read:
			col1.g=MappingBuffer[index];
			
			// Blue:
			
			index=(int)(col1.b * 255f);
			
			// Clip:
			if(index<0){
				index=0;
			}else if(index>255){
				index=255;
			}
			
			// Read:
			col1.b=MappingBuffer[index];
			
			// Offset:
			return col1;
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
			// Read value:
			double value=SourceModule.GetWrapped(x,y,wrap);
			
			// Get index:
			int index=(int)(value * 255f);
			
			// Clip:
			if(index<0){
				index=0;
			}else if(index>255){
				index=255;
			}
			
			// Read:
			return MappingBuffer[index];
			
		}
		
        public override double GetValue(double x, double y, double z){
			
			// Read value:
			double value=SourceModule.GetValue(x,y,z);
			
			// Get index:
			int index=(int)(value * 255f);
			
			// Clip:
			if(index<0){
				index=0;
			}else if(index>255){
				index=255;
			}
			
			// Read:
			return MappingBuffer[index];
			
		}
		
        /// <summary>
        /// Returns the output of the two source modules added together.
        /// </summary>
        public override double GetValue(double x, double y){
			
			// Read value:
			double value=SourceModule.GetValue(x, y);
			
			// Get index:
			int index=(int)(value * 255f);
			
			// Clip:
			if(index<0){
				index=0;
			}else if(index>255){
				index=255;
			}
			
			// Read:
			return MappingBuffer[index];
			
        }
		
		public override int TypeID{
			get{
				return 88;
			}
		}
		
    }
	
}