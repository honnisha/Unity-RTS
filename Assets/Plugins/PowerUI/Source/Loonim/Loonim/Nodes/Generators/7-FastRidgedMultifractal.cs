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
	
	public class FastRidgedMultifractal: FastNoiseBasis{
		
		private const int MaxOctaves=30;
		
		public override TextureNode Lacunarity
		{
			get { return base.Lacunarity; }
			set
			{
				base.Lacunarity = value;
				CalculateSpectralWeights();
			}
		}
		
		internal override int OutputDimensions{
			get{
				// 2D image.
				return 2;
			}
		}
		
		public override int MaterialID{
			get{
				// Use the rmf shader.
				return 10;
			}
		}
		
		private double[] SpectralWeights = new double[MaxOctaves];
		
		public FastRidgedMultifractal():base(3){}
		
		public override double GetWrapped(double x, double y, int wrap){
			
			double lacunarity=Lacunarity.GetWrapped(x,y,wrap);
			double frequency=Frequency.GetWrapped(x,y,wrap);
			x *= frequency;
			y *= frequency;

			double signal = 0.0;
			double value = 0.0;
			double weight = 1.0;

			// These parameters should be user-defined; they may be exposed in a
			// future version of libnoise.
			double offset = 1.0;
			double gain = 2.0;

			int mOctaveCount=(int)OctaveCount.GetWrapped(x,y,wrap);
			
			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
 
				long seed = (Seed + currentOctave) & 0x7fffffff;
				signal = GradientCoherentNoiseWrap(x, y, wrap, (int)seed, NoiseQuality);

				// Make the ridges.
				signal = System.Math.Abs(signal);
				signal = offset - signal;

				// Square the signal to increase the sharpness of the ridges.
				signal *= signal;

				// The weighting from the previous octave is applied to the signal.
				// Larger values have higher weights, producing sharp points along the
				// ridges.
				signal *= weight;

				// Weight successive contributions by the previous signal.
				weight = signal * gain;
				if (weight > 1.0)
				{
					weight = 1.0;
				}
				if (weight < 0.0)
				{
					weight = 0.0;
				}

				// Add the signal to the output value.
				value += (signal * SpectralWeights[currentOctave]);

				// Go to the next octave.
				x *= lacunarity;
				y *= lacunarity;
			}

			return (value * 1.25) - 1.0;
		}
		
		public override double GetValue(double x, double y, double z){
			
			double lacunarity=Lacunarity.GetValue(x,y,z);
			double frequency=Frequency.GetValue(x,y,z);
			x *= frequency;
			y *= frequency;
			z *= frequency;

			int mOctaveCount=(int)OctaveCount.GetValue(x,y,z);
			
			double signal = 0.0;
			double value = 0.0;
			double weight = 1.0;

			// These parameters should be user-defined; they may be exposed in a
			// future version of libnoise.
			double offset = 1.0;
			double gain = 2.0;

			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
 
				long seed = (Seed + currentOctave) & 0x7fffffff;
				signal = GradientCoherentNoise(x, y, z, (int)seed, NoiseQuality);

				// Make the ridges.
				signal = System.Math.Abs(signal);
				signal = offset - signal;

				// Square the signal to increase the sharpness of the ridges.
				signal *= signal;

				// The weighting from the previous octave is applied to the signal.
				// Larger values have higher weights, producing sharp points along the
				// ridges.
				signal *= weight;

				// Weight successive contributions by the previous signal.
				weight = signal * gain;
				if (weight > 1.0)
				{
					weight = 1.0;
				}
				if (weight < 0.0)
				{
					weight = 0.0;
				}

				// Add the signal to the output value.
				value += (signal * SpectralWeights[currentOctave]);

				// Go to the next octave.
				x *= lacunarity;
				y *= lacunarity;
				z *= lacunarity;
			}

			return (value * 1.25) - 1.0;
		}
		
		public override double GetValue(double x, double y)
		{
			double lacunarity=Lacunarity.GetValue(x,y);
			double frequency=Frequency.GetValue(x,y);
			x *= frequency;
			y *= frequency;

			int mOctaveCount=(int)OctaveCount.GetValue(x,y);
			
			double signal = 0.0;
			double value = 0.0;
			double weight = 1.0;

			// These parameters should be user-defined; they may be exposed in a
			// future version of libnoise.
			double offset = 1.0;
			double gain = 2.0;

			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
 
				long seed = (Seed + currentOctave) & 0x7fffffff;
				signal = GradientCoherentNoise(x, y, (int)seed, NoiseQuality);

				// Make the ridges.
				signal = System.Math.Abs(signal);
				signal = offset - signal;

				// Square the signal to increase the sharpness of the ridges.
				signal *= signal;

				// The weighting from the previous octave is applied to the signal.
				// Larger values have higher weights, producing sharp points along the
				// ridges.
				signal *= weight;

				// Weight successive contributions by the previous signal.
				weight = signal * gain;
				if (weight > 1.0)
				{
					weight = 1.0;
				}
				if (weight < 0.0)
				{
					weight = 0.0;
				}

				// Add the signal to the output value.
				value += (signal * SpectralWeights[currentOctave]);

				// Go to the next octave.
				x *= lacunarity;
				y *= lacunarity;
			}

			return (value * 1.25) - 1.0;
		}
		
		private void CalculateSpectralWeights()
		{
			double h = 1.0;
			
			double lacunarity=Lacunarity.GetValue(0,0);
			
			double frequency = 1.0;
			for (int i = 0; i < MaxOctaves; i++)
			{
				// Compute weight for each frequency.
				SpectralWeights[i] = System.Math.Pow(frequency, -h);
				frequency *= lacunarity;
			}
		}
		
		public override int TypeID{
			get{
				return 7;
			}
		}
		
	}
}
