using System;
using UnityEngine;

namespace Loonim{
	
	public class SelectFalloff : TextureNode{
		
		public TextureNode ControlModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The lower clamping bound.</summary>
		public TextureNode LowerBound{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		/// <summary>The upper clamping bound.</summary>
		public TextureNode UpperBound{
			get{
				return Sources[4];
			}
			set{
				Sources[4]=value;
			}
		}
		
		/// <summary>The edge falloff.</summary>
		public TextureNode EdgeFalloff{
			get{
				return Sources[5];
			}
			set{
				Sources[5]=value;
			}
		}
		
		public SelectFalloff():base(6){}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			double controlValue = ControlModule.GetValue(x, y);
			float alpha;

			double lowerBound=LowerBound.GetValue(x,y);
			double upperBound=UpperBound.GetValue(x,y);
			double edgeFalloff=EdgeFalloff.GetValue(x,y);
			
			if (edgeFalloff > 0.0)
			{
				if (controlValue < (lowerBound - edgeFalloff))
				{
					// The output value from the control module is below the selector
					// threshold; return the output value from the first source module.
					return SourceModule1.GetColour(x, y);
				}
				else if (controlValue < (lowerBound + edgeFalloff))
				{
					// The output value from the control module is near the lower end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (lowerBound - edgeFalloff);
					double upperCurve = (lowerBound + edgeFalloff);
					alpha = (float)Loonim.Math.SCurve3((controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule1.GetColour(x, y),
						SourceModule2.GetColour(x, y), alpha);
				}
				else if (controlValue < (upperBound - edgeFalloff))
				{
					// The output value from the control module is within the selector
					// threshold; return the output value from the second source module.
					return SourceModule2.GetColour(x, y);
				}
				else if (controlValue < (upperBound + edgeFalloff))
				{
					// The output value from the control module is near the upper end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (upperBound - edgeFalloff);
					double upperCurve = (upperBound + edgeFalloff);
					alpha = (float)Loonim.Math.SCurve3(
					  (controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule2.GetColour(x, y),
					  SourceModule1.GetColour(x, y),
					  alpha);
				}
				else
				{
					// Output value from the control module is above the selector threshold;
					// return the output value from the first source module.
					return SourceModule1.GetColour(x, y);
				}
			}
			else
			{
				if (controlValue < lowerBound || controlValue > upperBound)
				{
					return SourceModule1.GetColour(x, y);
				}
				else
				{
					return SourceModule2.GetColour(x, y);
				}
			}
			
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double controlValue = ControlModule.GetWrapped(x,y,wrap);
			double alpha;

			double lowerBound=LowerBound.GetWrapped(x,y,wrap);
			double upperBound=UpperBound.GetWrapped(x,y,wrap);
			double edgeFalloff=EdgeFalloff.GetWrapped(x,y,wrap);
			
			if (edgeFalloff > 0.0)
			{
				if (controlValue < (lowerBound - edgeFalloff))
				{
					// The output value from the control module is below the selector
					// threshold; return the output value from the first source module.
					return SourceModule1.GetWrapped(x,y,wrap);
				}
				else if (controlValue < (lowerBound + edgeFalloff))
				{
					// The output value from the control module is near the lower end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (lowerBound - edgeFalloff);
					double upperCurve = (lowerBound + edgeFalloff);
					alpha = Loonim.Math.SCurve3((controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule1.GetWrapped(x,y,wrap),
						SourceModule2.GetWrapped(x,y,wrap), alpha);
				}
				else if (controlValue < (upperBound - edgeFalloff))
				{
					// The output value from the control module is within the selector
					// threshold; return the output value from the second source module.
					return SourceModule2.GetWrapped(x,y,wrap);
				}
				else if (controlValue < (upperBound + edgeFalloff))
				{
					// The output value from the control module is near the upper end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (upperBound - edgeFalloff);
					double upperCurve = (upperBound + edgeFalloff);
					alpha = Loonim.Math.SCurve3(
					  (controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule2.GetWrapped(x,y,wrap),
					  SourceModule1.GetWrapped(x,y,wrap),
					  alpha);
				}
				else
				{
					// Output value from the control module is above the selector threshold;
					// return the output value from the first source module.
					return SourceModule1.GetWrapped(x,y,wrap);
				}
			}
			else
			{
				if (controlValue < lowerBound || controlValue > upperBound)
				{
					return SourceModule1.GetWrapped(x,y,wrap);
				}
				else
				{
					return SourceModule2.GetWrapped(x,y,wrap);
				}
			}
		}
		
		public override double GetValue(double x, double y, double z){
			double controlValue = ControlModule.GetValue(x, y,z);
			double alpha;

			double lowerBound=LowerBound.GetValue(x,y,z);
			double upperBound=UpperBound.GetValue(x,y,z);
			double edgeFalloff=EdgeFalloff.GetValue(x,y,z);
			
			if (edgeFalloff > 0.0)
			{
				if (controlValue < (lowerBound - edgeFalloff))
				{
					// The output value from the control module is below the selector
					// threshold; return the output value from the first source module.
					return SourceModule1.GetValue(x, y,z);
				}
				else if (controlValue < (lowerBound + edgeFalloff))
				{
					// The output value from the control module is near the lower end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (lowerBound - edgeFalloff);
					double upperCurve = (lowerBound + edgeFalloff);
					alpha = Loonim.Math.SCurve3((controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule1.GetValue(x, y,z),
						SourceModule2.GetValue(x, y,z), alpha);
				}
				else if (controlValue < (upperBound - edgeFalloff))
				{
					// The output value from the control module is within the selector
					// threshold; return the output value from the second source module.
					return SourceModule2.GetValue(x, y,z);
				}
				else if (controlValue < (upperBound + edgeFalloff))
				{
					// The output value from the control module is near the upper end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (upperBound - edgeFalloff);
					double upperCurve = (upperBound + edgeFalloff);
					alpha = Loonim.Math.SCurve3(
					  (controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule2.GetValue(x, y,z),
					  SourceModule1.GetValue(x, y,z),
					  alpha);
				}
				else
				{
					// Output value from the control module is above the selector threshold;
					// return the output value from the first source module.
					return SourceModule1.GetValue(x, y,z);
				}
			}
			else
			{
				if (controlValue < lowerBound || controlValue > upperBound)
				{
					return SourceModule1.GetValue(x, y,z);
				}
				else
				{
					return SourceModule2.GetValue(x, y,z);
				}
			}
		}
		
		public override double GetValue(double x, double y){
			double controlValue = ControlModule.GetValue(x, y);
			double alpha;
			
			double lowerBound=LowerBound.GetValue(x,y);
			double upperBound=UpperBound.GetValue(x,y);
			double edgeFalloff=EdgeFalloff.GetValue(x,y);
			
			if (edgeFalloff > 0.0)
			{
				if (controlValue < (lowerBound - edgeFalloff))
				{
					// The output value from the control module is below the selector
					// threshold; return the output value from the first source module.
					return SourceModule1.GetValue(x, y);
				}
				else if (controlValue < (lowerBound + edgeFalloff))
				{
					// The output value from the control module is near the lower end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (lowerBound - edgeFalloff);
					double upperCurve = (lowerBound + edgeFalloff);
					alpha = Loonim.Math.SCurve3((controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule1.GetValue(x, y),
						SourceModule2.GetValue(x, y), alpha);
				}
				else if (controlValue < (upperBound - edgeFalloff))
				{
					// The output value from the control module is within the selector
					// threshold; return the output value from the second source module.
					return SourceModule2.GetValue(x, y);
				}
				else if (controlValue < (upperBound + edgeFalloff))
				{
					// The output value from the control module is near the upper end of the
					// selector threshold and within the smooth curve. Interpolate between
					// the output values from the first and second source modules.
					double lowerCurve = (upperBound - edgeFalloff);
					double upperCurve = (upperBound + edgeFalloff);
					alpha = Loonim.Math.SCurve3(
					  (controlValue - lowerCurve) / (upperCurve - lowerCurve));
					return Loonim.Math.LinearInterpolate(SourceModule2.GetValue(x, y),
					  SourceModule1.GetValue(x, y),
					  alpha);
				}
				else
				{
					// Output value from the control module is above the selector threshold;
					// return the output value from the first source module.
					return SourceModule1.GetValue(x, y);
				}
			}
			else
			{
				if (controlValue < lowerBound || controlValue > upperBound)
				{
					return SourceModule1.GetValue(x, y);
				}
				else
				{
					return SourceModule2.GetValue(x, y);
				}
			}
		}
		
		public override int TypeID{
			get{
				return 32;
			}
		}
		
	}
}
