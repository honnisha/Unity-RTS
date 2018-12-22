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
	
	public class FastBillow : FastNoiseBasis{
		
		private const int MaxOctaves = 30;

		public FastBillow() : base(4){}
		
		internal override int OutputDimensions{
			get{
				// 2D image.
				return 2;
			}
		}
		
		public override int MaterialID{
			get{
				// Use the billow shader.
				return 89;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			long seed;

			double lacunarity=Lacunarity.GetWrapped(x,y,wrap);
			double persist=Persistence.GetWrapped(x,y,wrap);
			double frequency=Frequency.GetWrapped(x,y,wrap);
			x *= frequency;
			y *= frequency;

			int mOctaveCount=(int)OctaveCount.GetWrapped(x,y,wrap);
			
			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
				seed = (Seed + currentOctave) & 0xffffffff;
				signal = GradientCoherentNoiseWrap(x, y,wrap, (int)seed, NoiseQuality);
				signal = 2.0 * System.Math.Abs(signal) - 1.0;
				value += signal * curPersistence;

				x *= lacunarity;
				y *= lacunarity;
				curPersistence *= persist;
			}

			value += 0.5;

			return value;
		}
		
		public override double GetValue(double x, double y, double z){
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			long seed;

			double lacunarity=Lacunarity.GetValue(x,y,z);
			double persist=Persistence.GetValue(x,y,z);
			double frequency=Frequency.GetValue(x,y,z);
			x *= frequency;
			y *= frequency;
			z *= frequency;

			int mOctaveCount=(int)OctaveCount.GetValue(x,y,z);
			
			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{

				seed = (Seed + currentOctave) & 0xffffffff;
				signal = GradientCoherentNoise(x, y,z, (int)seed, NoiseQuality);
				signal = 2.0 * System.Math.Abs(signal) - 1.0;
				value += signal * curPersistence;

				x *= lacunarity;
				y *= lacunarity;
				z *= lacunarity;
				curPersistence *= persist;
			}

			value += 0.5;

			return value;
		}
		
		public override double GetValue(double x, double y)
		{
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			long seed;

			double lacunarity=Lacunarity.GetValue(x,y);
			double persist=Persistence.GetValue(x,y);
			double frequency=Frequency.GetValue(x,y);
			x *= frequency;
			y *= frequency;

			int mOctaveCount=(int)OctaveCount.GetValue(x,y);
			
			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{

				seed = (Seed + currentOctave) & 0xffffffff;
				signal = GradientCoherentNoise(x, y, (int)seed, NoiseQuality);
				signal = 2.0 * System.Math.Abs(signal) - 1.0;
				value += signal * curPersistence;

				x *= lacunarity;
				y *= lacunarity;
				curPersistence *= persist;
			}

			value += 0.5;

			return value;
		}
		
		public override int TypeID{
			get{
				return 5;
			}
		}
		
	}
}
