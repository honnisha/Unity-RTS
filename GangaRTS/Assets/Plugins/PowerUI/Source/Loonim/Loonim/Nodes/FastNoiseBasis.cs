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
	
	public class FastNoiseBasis : TextureNode{
		
		public TextureNode Frequency{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode OctaveCount{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public virtual TextureNode Lacunarity{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode Persistence{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		private int[] RandomPermutations = new int[512];
		private int[] SelectedPermutations = new int[512];
		private float[] GradientTable = new float[512];

		private int mSeed;
		public NoiseQuality NoiseQuality=NoiseQuality.Standard;
		
		
		public FastNoiseBasis(int src):base(src){}
		
		public FastNoiseBasis(){}
		
		public double GradientCoherentNoiseWrap(double x, double y,int wrap, int seed, NoiseQuality noiseQuality)
		{
			int ix0 = (x > 0.0 ? (int)x : (int)x - 1);
			int iy0 = (y > 0.0 ? (int)y : (int)y - 1);

			double fx0 = 0, fy0 = 0;
			switch (noiseQuality)
			{
				case NoiseQuality.Low:
					fx0 = (x - ix0);
					fy0 = (y - iy0);
					break;
				case NoiseQuality.Standard:
					fx0 = Loonim.Math.SCurve3(x - ix0);
					fy0 = Loonim.Math.SCurve3(y - iy0);
					break;
				case NoiseQuality.High:
					fx0 = Loonim.Math.SCurve5(x - ix0);
					fy0 = Loonim.Math.SCurve5(y - iy0);
					break;
			}
			
			int ix1=((ix0 + 1) % wrap ) & 255;
			ix0 = (ix0 % wrap ) & 255;
			
			int A = ((SelectedPermutations[ix0] + iy0) % wrap ) & 255;
			int AA = SelectedPermutations[A];
			int AB;
			
			if(A==wrap-1){
				AB=SelectedPermutations[0];
			}else{
				AB=SelectedPermutations[A + 1];
			}
			
			int B = ((SelectedPermutations[ix1] + iy0) % wrap ) & 255;
			int BA = SelectedPermutations[B];
			int BB;
			
			if(B==wrap-1){
				BB=SelectedPermutations[0];
			}else{
				BB=SelectedPermutations[B + 1];
			}
			
			double a = Loonim.Math.LinearInterpolate(GradientTable[AA], GradientTable[BA], fx0);
			double b = Loonim.Math.LinearInterpolate(GradientTable[AB], GradientTable[BB], fx0);
			return Loonim.Math.LinearInterpolate(a, b, fy0);
		}

		public double GradientCoherentNoise(double x, double y, double z, int seed, NoiseQuality noiseQuality)
		{
			int x0 = (x > 0.0 ? (int)x : (int)x - 1);
			int y0 = (y > 0.0 ? (int)y : (int)y - 1);
			int z0 = (z > 0.0 ? (int)z : (int)z - 1);

			int X = x0 & 255;
			int Y = y0 & 255;
			int Z = z0 & 255;

			double x1 = 0, y1 = 0, z1 = 0;
			switch (noiseQuality)
			{
				case NoiseQuality.Low:
					x1 = (x - x0);
					y1 = (y - y0);
					z1 = (z - z0);
					break;
				case NoiseQuality.Standard:
					x1 = Loonim.Math.SCurve3(x - x0);
					y1 = Loonim.Math.SCurve3(y - y0);
					z1 = Loonim.Math.SCurve3(z - z0);
					break;
				case NoiseQuality.High:
					x1 = Loonim.Math.SCurve5(x - x0);
					y1 = Loonim.Math.SCurve5(y - y0);
					z1 = Loonim.Math.SCurve5(z - z0);
					break;
			}
			
			int A = SelectedPermutations[X] + Y;
			int AA = SelectedPermutations[A] + Z;
			int AB = SelectedPermutations[A + 1] + Z;
			int B = SelectedPermutations[X + 1] + Y;
			int BA = SelectedPermutations[B] + Z;
			int BB = SelectedPermutations[B + 1] + Z;
			
			double a = Loonim.Math.LinearInterpolate(GradientTable[AA], GradientTable[BA], x1);
			double b = Loonim.Math.LinearInterpolate(GradientTable[AB], GradientTable[BB], x1);
			double c = Loonim.Math.LinearInterpolate(a, b, y1);
			double d = Loonim.Math.LinearInterpolate(GradientTable[AA + 1], GradientTable[BA + 1], x1);
			double e = Loonim.Math.LinearInterpolate(GradientTable[AB + 1], GradientTable[BB + 1], x1);
			double f = Loonim.Math.LinearInterpolate(d, e, y1);
			return Loonim.Math.LinearInterpolate(c, f, z1);
		}
		
		public double GradientCoherentNoise(double x, double y, int seed, NoiseQuality noiseQuality)
		{
			int x0 = (x > 0.0 ? (int)x : (int)x - 1);
			int y0 = (y > 0.0 ? (int)y : (int)y - 1);

			int X = x0 & 255;
			int Y = y0 & 255;

			double u = 0, v = 0;
			switch (noiseQuality)
			{
				case NoiseQuality.Low:
					u = (x - x0);
					v = (y - y0);
					break;
				case NoiseQuality.Standard:
					u = Loonim.Math.SCurve3(x - x0);
					v = Loonim.Math.SCurve3(y - y0);
					break;
				case NoiseQuality.High:
					u = Loonim.Math.SCurve5(x - x0);
					v = Loonim.Math.SCurve5(y - y0);
					break;
			}

			int A = SelectedPermutations[X] + Y;
			int AA = SelectedPermutations[A];
			int AB = SelectedPermutations[A + 1];
			int B = SelectedPermutations[X + 1] + Y;
			int BA = SelectedPermutations[B];
			int BB = SelectedPermutations[B + 1];

			double a = Loonim.Math.LinearInterpolate(GradientTable[AA], GradientTable[BA], u);
			double b = Loonim.Math.LinearInterpolate(GradientTable[AB], GradientTable[BB], u);
			return Loonim.Math.LinearInterpolate(a, b, v);
		}

		public int Seed
		{
			get { return mSeed; }
			set
			{
				mSeed = value;

				// Generate new random permutations with this seed.
				System.Random random = new System.Random(mSeed);
				for (int i = 0; i < 512; i++)
					RandomPermutations[i] = random.Next(255);
				for (int i = 0; i < 256; i++)
					SelectedPermutations[256 + i] = SelectedPermutations[i] = RandomPermutations[i];

				// Generate a new gradient table
				float[] kkf = new float[256];
				for (int i = 0; i < 256; i++)
					kkf[i] = -1.0f + 2.0f * ((float)i / 255.0f);

				for (int i = 0; i < 256; i++)
					GradientTable[i] = kkf[SelectedPermutations[i]];
				for (int i = 256; i < 512; i++)
					GradientTable[i] = GradientTable[i & 255];
			}
		}
		
	}
}
