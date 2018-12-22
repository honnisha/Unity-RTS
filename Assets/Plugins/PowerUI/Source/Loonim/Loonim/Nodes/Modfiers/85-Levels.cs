using System;
using UnityEngine;

namespace Loonim{

    /// <summary>
    /// Changes the range of RGB values to the specified white/black range. Clipped to the min/max, then stretched out.
    /// </summary>
    public class Levels: TextureNode{
		
        /// <summary>
        /// The black min value. Clipped to this.
        /// </summary>
        public TextureNode BlackModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>
        /// The white max value. Clipped to this.
        /// </summary>
        public TextureNode WhiteModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public Levels():base(3){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Read min of range:
			UnityEngine.Color min=BlackModule.GetColour(x,y);
            
			// Read max of range:
			UnityEngine.Color max=WhiteModule.GetColour(x,y);
			
			// Divide by range and offset by min:
			col1.r=( (col1.r) / (max.r - min.r) ) - min.r;
			col1.g=( (col1.g) / (max.g - min.g) ) - min.g;
			col1.b=( (col1.b) / (max.b - min.b) ) - min.b;
			
			// Offset:
			return col1;
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
			// Read value:
			double value=SourceModule.GetWrapped(x,y,wrap);
			
			// Read min of range:
			double min=BlackModule.GetWrapped(x,y,wrap);
            
			// Divide by range:
			value/=( WhiteModule.GetWrapped(x,y,wrap) - min );
			
			// Offset:
			return value + min;
			
		}
		
        public override double GetValue(double x, double y, double z){
			
			// Read value:
			double value=SourceModule.GetValue(x,y,z);
			
			// Read min of range:
			double min=BlackModule.GetValue(x,y,z);
            
			// Divide by range:
			value/=( WhiteModule.GetValue(x,y,z) - min );
			
			// Offset:
			return value + min;
			
		}
		
        /// <summary>
        /// Returns the output of the two source modules added together.
        /// </summary>
        public override double GetValue(double x, double y){
			
			// Read value:
			double value=SourceModule.GetValue(x, y);
			
			// Read min of range:
			double min=BlackModule.GetValue(x, y);
            
			// Divide by range:
			value/=( WhiteModule.GetValue(x,y) - min );
			
			// Offset:
			return value + min;
			
        }
		
		public override int TypeID{
			get{
				return 85;
			}
		}
		
    }
	
}
