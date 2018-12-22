using System;
using UnityEngine;

namespace Loonim{

    /// <summary>
    /// Does Source(LookupX(x,y),LookupY(x,y)); or Source(LookupX(x,y),0) if Y is null.
    /// </summary>
    public class Lookup : TextureNode{
		
        /// <summary>The module to read the x coord from.</summary>
        public TextureNode XModule{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
        /// <summary>The module to read the y coord from.</summary>
        public TextureNode YModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		
        public Lookup():base(3){}
		
        public Lookup(TextureNode gradient,TextureNode x,TextureNode y):base(3){
			
			SourceModule=gradient;
			XModule=x;
			YModule=y;
			
		}
		
        public Lookup(TextureNode gradient,TextureNode x):base(3){
			
			SourceModule=gradient;
			XModule=x;
			YModule=new Property(0f);
			
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			// Read coords:
			double lookX=(XModule==null)?0:XModule.GetValue(x,y);
			double lookY=(YModule==null)?0:YModule.GetValue(x,y);
			
			// Read colour:
			return SourceModule.GetColour(lookX,lookY);
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			
			// Read coords:
			double lookX=(XModule==null)?0:XModule.GetWrapped(x,y,wrap);
			double lookY=(YModule==null)?0:YModule.GetWrapped(x,y,wrap);
			
			// Read:
			return SourceModule.GetWrapped(lookX,lookY,wrap);
			
		}
		
        public override double GetValue(double x, double y, double z){
			
			// Read coords:
			double lookX=(XModule==null)?0:XModule.GetValue(x,y,z);
			double lookY=(YModule==null)?0:YModule.GetValue(x,y,z);
			
			// Read:
			return SourceModule.GetValue(lookX,lookY,z);
			
		}
		
        /// <summary>
        /// Returns the output of the two source modules added together.
        /// </summary>
        public override double GetValue(double x, double y){
            
			// Read coords:
			double lookX=(XModule==null)?0:XModule.GetValue(x,y);
			double lookY=(YModule==null)?0:YModule.GetValue(x,y);
			
			// Read:
			return SourceModule.GetValue(lookX,lookY);
			
        }
		
		public override double GetValue(double t){
			
			// Read the target t value from the lookup graph:
			t=XModule.GetValue(t);
			
			// Read at that point:
			return SourceModule.GetValue(t);
			
		}
		
		public override int TypeID{
			get{
				return 92;
			}
		}
		
    }
	
}
