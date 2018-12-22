//--------------------------------------
//	   Loonim Image Generator
//	Partly derived from LibNoise
//	See License.txt for more info
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;

namespace Loonim{
	
	public class Billow : TextureNode{
		
		internal override int OutputDimensions{
			get{
				// 2D image.
				return 2;
			}
		}
		
		public TextureNode Frequency{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode Persistence{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode OctaveCount{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode Lacunarity{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		public int Seed;
		public NoiseQuality NoiseQuality;
		
		private const int MaxOctaves = 30;

		public Billow() : base(4)
		{
			// Frequency = 1.0;
			// Lacunarity = 2.0;
			// mOctaveCount = 6;
			// Persistence = 0.5;
			NoiseQuality = NoiseQuality.Standard;
			Seed = 0;
		}
		
		public override int TypeID{
			get{
				return 89;
			}
		}
		
		public override double GetWrapped(double x, double y, int wrap){
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			//double nx, ny, nz;
			long seed;

			double lacunarity=Lacunarity.GetWrapped(x,y,wrap);
			double persist=Persistence.GetWrapped(x,y,wrap);
			double frequency=Frequency.GetWrapped(x,y,wrap);
			x *= frequency;
			y *= frequency;
			
			int mOctaveCount=(int)OctaveCount.GetWrapped(x,y,wrap);
			
			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
				/*nx = Math.MakeInt32Range(x);
				ny = Math.MakeInt32Range(y);
				nz = Math.MakeInt32Range(z);*/

				seed = (Seed + currentOctave) & 0xffffffff;
				signal = GradientNoise.GradientCoherentNoiseWrap(x, y, wrap, (int)seed, NoiseQuality);
				
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
			//double nx, ny, nz;
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
				/*nx = Math.MakeInt32Range(x);
				ny = Math.MakeInt32Range(y);
				nz = Math.MakeInt32Range(z);*/

				seed = (Seed + currentOctave) & 0xffffffff;
				signal = GradientNoise.GradientCoherentNoise(x, y, z, (int)seed, NoiseQuality);
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
		
		public override double GetValue(double x, double y){
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			//double nx, ny, nz;
			long seed;

			double lacunarity=Lacunarity.GetValue(x,y);
			double persist=Persistence.GetValue(x,y);
			double frequency=Frequency.GetValue(x,y);
			x *= frequency;
			y *= frequency;
			
			int mOctaveCount=(int)OctaveCount.GetValue(x,y);
			
			for (int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
				/*nx = Math.MakeInt32Range(x);
				ny = Math.MakeInt32Range(y);
				nz = Math.MakeInt32Range(z);*/

				seed = (Seed + currentOctave) & 0xffffffff;
				signal = GradientNoise.GradientCoherentNoise(x, y, (int)seed, NoiseQuality);
				signal = 2.0 * System.Math.Abs(signal) - 1.0;
				value += signal * curPersistence;

				x *= lacunarity;
				y *= lacunarity;
				curPersistence *= persist;
			}

			value += 0.5;

			return value;
		}
		
	}
}
