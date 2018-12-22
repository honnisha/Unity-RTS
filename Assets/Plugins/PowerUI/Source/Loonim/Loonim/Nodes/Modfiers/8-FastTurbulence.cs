// 
// Copyright (c) 2013 Jason Bell
// 
// Permission is hereby granted, free of charge, to any person obtaining a 
// copy of this software and associated documentation files (the "Software"), 
// to deal in the Software without restriction, including without limitation 
// the rights to use, copy, modify, merge, publish, distribute, sublicense, 
// and/or sell copies of the Software, and to permit persons to whom the 
// Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included 
// in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS 
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
// DEALINGS IN THE SOFTWARE.
// 

using System;
using UnityEngine;

namespace Loonim{
	
	public class FastTurbulence: TextureNode{
		
		public TextureNode Power{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode Frequency{
			get { return Sources[2]; }
			set
			{
				Sources[2] = value;
				XDistort.Frequency = value;
				YDistort.Frequency = value;
				ZDistort.Frequency = value;
				WDistort.Frequency = value;
			}
		}
		
		public TextureNode Roughness{
			get { return Sources[3]; }
			set
			{
				Sources[3] = value;
				XDistort.OctaveCount = value;
				YDistort.OctaveCount = value;
				ZDistort.OctaveCount = value;
				WDistort.OctaveCount = value;
			}
		}

		private FastPerlin XDistort;
		private FastPerlin YDistort;
		private FastPerlin ZDistort;
		private FastPerlin WDistort;

		public FastTurbulence() : base(4)
		{
			
			XDistort = new FastPerlin();
			YDistort = new FastPerlin();
			ZDistort = new FastPerlin();
			WDistort = new FastPerlin();
			
			//Frequency = 1.0;
			//Power = 1.0;
			//Roughness = 3;
			Seed = 0;
		}
		
		internal override int OutputDimensions{
			get{
				// Same as source.
				return SourceModule.OutputDimensions;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double x0, y0;
			double x1, y1;
			x0 = x + (12414.0 / 65536.0);
			y0 = y + (65124.0 / 65536.0);
			x1 = x + (26519.0 / 65536.0);
			y1 = y + (18128.0 / 65536.0);
			
			double power=Power.GetWrapped(x,y,wrap);
			
			double xDistort = x + (XDistort.GetValue(x0, y0)
			  * power);
			double yDistort = y + (YDistort.GetValue(x1, y1)
			  * power);

			// Retrieve the output value at the offsetted input value instead of the
			// original input value.
			return SourceModule.GetWrapped(xDistort, yDistort,wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			double x0, y0, z0;
			double x1, y1, z1;
			double x2, y2, z2;
			x0 = x + (12414.0 / 65536.0);
			y0 = y + (65124.0 / 65536.0);
			z0 = z + (21124.0 / 65536.0);
			
			x1 = x + (26519.0 / 65536.0);
			y1 = y + (18128.0 / 65536.0);
			z1 = z + (34128.0 / 65536.0);
			
			x2 = x + (26519.0 / 65536.0);
			y2 = y + (42128.0 / 65536.0);
			z2 = z + (12128.0 / 65536.0);
			
			double power=Power.GetValue(x,y,z);
			
			double xDistort = x + (XDistort.GetValue(x0, y0, z0)* power);
			double yDistort = y + (YDistort.GetValue(x1, y1, z1)* power);
			double zDistort = z + (ZDistort.GetValue(x2, y2, z2)* power);
			
			return SourceModule.GetValue(xDistort, yDistort, zDistort);
		}
		
		public override double GetValue(double x, double y){
			// Get the values from the three noise::module::Perlin noise modules and
			// add each value to each coordinate of the input value.  There are also
			// some offsets added to the coordinates of the input values.  This prevents
			// the distortion modules from returning zero if the (x, y, z) coordinates,
			// when multiplied by the frequency, are near an integer boundary.  This is
			// due to a property of gradient coherent noise, which returns zero at
			// integer boundaries.
			double x0, y0;
			double x1, y1;
			x0 = x + (12414.0 / 65536.0);
			y0 = y + (65124.0 / 65536.0);
			x1 = x + (26519.0 / 65536.0);
			y1 = y + (18128.0 / 65536.0);
			
			double power=Power.GetValue(x,y);
			
			double xDistort = x + (XDistort.GetValue(x0, y0)
			  * power);
			double yDistort = y + (YDistort.GetValue(x1, y1)
			  * power);

			// Retrieve the output value at the offsetted input value instead of the
			// original input value.
			return SourceModule.GetValue(xDistort, yDistort);
		}
		
		public int Seed
		{
			get { return XDistort.Seed; }
			set
			{
				XDistort.Seed = value;
				YDistort.Seed = value + 1;
				ZDistort.Seed = value + 2;
				WDistort.Seed = value + 3;
			}
		}
		
		public override int TypeID{
			get{
				return 8;
			}
		}
		
	}
}
