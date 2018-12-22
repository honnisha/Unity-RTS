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
	
	public class Perlin : TextureNode{
		
		public TextureNode Frequency{ // Scale
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode OctaveCount{ // Detail = 0 is 1 octave, and 100 is 11 octaves.
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode Lacunarity{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode Persistence{ // Roughness / 100. Low = smooth.
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		internal override int OutputDimensions{
			get{
				// 2D.
				return 2;
			}
		}
		
		public int Seed;
		public NoiseQuality NoiseQuality;
		
		private const int MaxOctaves = 30;

		public Perlin() : base(4)
		{
			//Frequency = 1.0;
			//Lacunarity = 2.0;
			//OctaveCount = 6;
			//Persistence = 0.5;
			//NoiseQuality = NoiseQuality.Standard;
			Seed = 0;
		}
		
		#if !NO_BLADE_RUNTIME
		public override DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			// Stack required.
			
			// Allocate a target stack now:
			int targetStack=stackID;
			DrawStack stack=tex.GetStack(targetStack,info);
			stackID++;
			
			float curAmplitude=1f;
			float curFrequency=(float)Frequency.GetValue(0,0);
			float lacunarity=(float)Lacunarity.GetValue(0,0);
			float persistence=(float)Persistence.GetValue(0,0);
			int mOctaveCount=(int)OctaveCount.GetValue(0,0);
			Material[] materials=new Material[mOctaveCount];
			
			// Stack up "OctaveCount" quads.
			for(int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
				
				// Update seed:
				long seed = (Seed + currentOctave) & 0xffffffff;
				
				// And a material.
				UnityEngine.Material material=GetMaterial(TypeID,SubMaterialID);
				
				// _Data (Seed, Frequency, Amplitude, Jitter):
				material.SetVector("_Data",new Vector4(seed,curFrequency,curAmplitude,0f));
				
				// Add to set:
				materials[currentOctave]=material;
				
				curFrequency *= lacunarity;
				curAmplitude *= persistence;
			}
			
			// Create our node:
			BlockStackNode bsn=new BlockStackNode();
			DrawStore=bsn;
			bsn.Mesh=info.Mesh;
			bsn.Materials=materials;
			bsn.Stack=stack;
			
			return bsn;
			
		}
		
		#endif
		
		public override double GetWrapped(double x, double y, int wrap){
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			//double nx, ny, nz;
			long seed;

			double lacunarity=Lacunarity.GetWrapped(x,y,wrap);
			double persistence=Persistence.GetWrapped(x,y,wrap);
			double frequency=Frequency.GetWrapped(x,y,wrap);
			x*=frequency;
			y*=frequency;
			
			int mOctaveCount=(int)OctaveCount.GetWrapped(x,y,wrap);
			
			for(int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
				seed = (Seed + currentOctave) & 0xffffffff;

				signal = GradientNoise.GradientCoherentNoiseWrap(x, y ,wrap, (int)seed, NoiseQuality);
				
				value += signal * curPersistence;

				x *= lacunarity;
				y *= lacunarity;
				curPersistence *= persistence;
			}

			return value;
		}
		
		public override double GetValue(double x, double y, double z){
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			//double nx, ny, nz;
			long seed;

			double lacunarity=Lacunarity.GetValue(x,y,z);
			double persistence=Persistence.GetValue(x,y,z);
			double frequency=Frequency.GetValue(x,y,z);
			x *= frequency;
			y *= frequency;
			z *= frequency;
			
			int mOctaveCount=(int)OctaveCount.GetValue(x,y,z);
			
			for(int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
				seed = (Seed + currentOctave) & 0xffffffff;

				signal = GradientNoise.GradientCoherentNoise(x, y , z, (int)seed, NoiseQuality);
				
				value += signal * curPersistence;

				x *= lacunarity;
				y *= lacunarity;
				z *= lacunarity;
				curPersistence *= persistence;
			}

			return value;
		}
		
		public override double GetValue(double x, double y)
		{
			double value = 0.0;
			double signal = 0.0;
			double curPersistence = 1.0;
			//double nx, ny, nz;
			long seed;

			double lacunarity=Lacunarity.GetValue(x,y);
			double persistence=Persistence.GetValue(x,y);
			double frequency=Frequency.GetValue(x,y);
			x *= frequency;
			y *= frequency;
			
			int mOctaveCount=(int)OctaveCount.GetValue(x,y);
			
			for(int currentOctave = 0; currentOctave < mOctaveCount; currentOctave++)
			{
				seed = (Seed + currentOctave) & 0xffffffff;

				signal = GradientNoise.GradientCoherentNoise(x, y, (int)seed, NoiseQuality);
				
				value += signal * curPersistence;

				x *= lacunarity;
				y *= lacunarity;
				curPersistence *= persistence;
			}

			return value;
		}
		
		public override int TypeID{
			get{
				return 9;
			}
		}
		
	}
}
