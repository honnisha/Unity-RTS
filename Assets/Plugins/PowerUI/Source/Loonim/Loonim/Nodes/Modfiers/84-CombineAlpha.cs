using System;
using UnityEngine;

namespace Loonim{

    /// <summary>
    /// Module that returns the output of two source modules added together.
    /// </summary>
    public class CombineAlpha : Std2InputNode{
		
        /// <summary>
        /// The second module from which to retrieve noise.
        /// </summary>
        public TextureNode AlphaModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read colour:
			UnityEngine.Color col1=SourceModule.GetColour(x,y);
			
			// Read and apply alpha:
			col1.a=(float)AlphaModule.GetValue(x,y);
			
			return col1;
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			return SourceModule.GetWrapped(x,y,wrap) + AlphaModule.GetWrapped(x,y,wrap);
		}
		
        public override double GetValue(double x, double y, double z){
			return SourceModule.GetValue(x, y,z) + AlphaModule.GetValue(x, y,z);
		}
		
        /// <summary>
        /// Returns the output of the two source modules added together.
        /// </summary>
        public override double GetValue(double x, double y){
            return SourceModule.GetValue(x, y) + AlphaModule.GetValue(x, y);
        }
		
		public override int TypeID{
			get{
				return 84;
			}
		}
		
    }
	
}
