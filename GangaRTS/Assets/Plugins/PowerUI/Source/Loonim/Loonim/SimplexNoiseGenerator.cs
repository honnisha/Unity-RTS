// Originated from https://gist.github.com/jstanden/1489447

using System;
using UnityEngine;
using System.Text;
using System.Collections;

namespace Loonim{


	public class SimplexNoiseGenerator{
		
		private const float TwoPi=(float)System.Math.PI*2f;
		
		private int[] A = new int[3];
		private float s, u, v, w;
		private int i, j, k;
		private float onethird = 0.333333333f;
		private float onesixth = 0.166666667f;
		private int[] T;
		public float CenterX;
		public float CenterZ;
		public const float TimeRadius=40f;
		
		
		public SimplexNoiseGenerator() {
			if (T == null) {
				System.Random rand = new System.Random();
				T = new int[8];
				for (int q = 0; q < 8; q++)
					T[q] = rand.Next();
			}
		}
		
		public SimplexNoiseGenerator(string seed) {
			byte[] seedBytes=Encoding.UTF8.GetBytes(seed);
			SetSeed(seedBytes);
		}
		
		public SimplexNoiseGenerator(byte[] seed){
			SetSeed(seed);
		}
		
		public SimplexNoiseGenerator(ulong seed){
			SetSeed(seed);
		}
		
		public SimplexNoiseGenerator(int[] seed){ // {0x16, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a}
			T=seed;
		}
		
		/// <summary>Little endian.</summary>
		private byte[] GetBytes(ulong value){
			byte[] result=new byte[8];
			result[0]=(byte)value;
			result[1]=(byte) (value>>8);
			result[2]=(byte) (value>>16);
			result[3]=(byte) (value>>24);
			result[4]=(byte) (value>>32);
			result[5]=(byte) (value>>40);
			result[6]=(byte) (value>>48);
			result[7]=(byte) (value>>56);
			return result;
		}

		public void SetSeed(ulong seed){
			SetSeed(GetBytes(seed));
		}
		
		public void SetSeed(byte[] seedBytes){
			bool flush=false;
			
			if(T==null){
				T=new int[8];
			}else{
				flush=true;
			}
			
			if(seedBytes==null){
				return;
			}
			
			int max=seedBytes.Length;
			if(max>T.Length){
				max=T.Length;
			}else if(flush){
				// Make sure max->T.Length is all zeros.
				for(int i=max;i<T.Length;i++){
					T[i]=0;
				}
			}
			
			for(int index=0;index<max;index++){
				T[index]=seedBytes[index];
			}
			
		}
		
		/// <summary>Time should be a value from 0->1.</summary>
		public void TimeOffset(float time){
			// Convert it into a rotation angle:
			time*=TwoPi;
			
			// Figure out the X and Z center offset:
			CenterX=TimeRadius*(float)System.Math.Cos(time);
			CenterZ=TimeRadius*(float)System.Math.Sin(time);
			
		}
		
		/// <summary>Puts the given time in the last sector of the seed.</summary>
		public void SeedTime(int time){
			T[7]=time;
			T[6]=time/2;
		}
		
		public string GetSeed(){
			if(T==null){
				return "";
			}
			
			byte[] tBytes=new byte[T.Length];
			
			for(int i=0;i<T.Length;i++){
				tBytes[i]=(byte)T[i];
			}
			
			return Encoding.UTF8.GetString(tBytes);
		}
		
		public float coherentNoise(float x, float y, float z){
			return coherentNoise(x,y,z,1,25,0.5f,2,0.9f);
		}
		
		public float coherentNoise(float x, float y, float z,int octaves){
			return coherentNoise(x,y,z,octaves,25,0.5f,2,0.9f);
		}
		
		public float coherentNoise(float x, float y, float z,int octaves,int multiplier){
			return coherentNoise(x,y,z,octaves,multiplier,0.5f,2,0.9f);
		}
		
		public float coherentNoise(float x, float y, float z,int octaves,int multiplier,float amplitude){
			return coherentNoise(x,y,z,octaves,multiplier,amplitude,2,0.9f);
		}
		
		public float coherentNoise(float x, float y, float z,int octaves,int multiplier,float amplitude,float lacunarity){
			return coherentNoise(x,y,z,octaves,multiplier,amplitude,lacunarity,0.9f);
		}
		
		public float coherentNoise(float x, float y, float z, int octaves, int multiplier, float amplitude, float lacunarity, float persistence) {
			float val = 0;
			x/=multiplier;
			y/=multiplier;
			z/=multiplier;
			
			for (int n = 0; n < octaves; n++) {
			  val += noise(x,y,z) * amplitude;
			  x *= lacunarity;
			  y *= lacunarity;
			  z *= lacunarity;
			  amplitude *= persistence;
			}
			return val;
		}
		
		public int getDensity(Vector3 loc) {
			float val = coherentNoise(loc.x, loc.y, loc.z);
			return (int)(255f * val);
		}
		
		public float timedNoise(float x,float y,float z){
			
			x+=CenterX;
			z+=CenterZ;
			
			return noise(x,y,z);
		}
		
		// Simplex Noise Generator
		public float noise(float x, float y, float z) {
			s = (x + y + z) * onethird;
			i = fastfloor(x + s);
			j = fastfloor(y + s);
			k = fastfloor(z + s);

			s = (i + j + k) * onesixth;
			u = x - i + s;
			v = y - j + s;
			w = z - k + s;
			
			bool uw=(u>=w);
			bool uv=(u>=v);
			bool vw=(v>=w);
			
			A[0] = 0; A[1] = 0; A[2] = 0;
			
			int hi;
			int lo;
			
			if(uw){
				
				if(uv){
					hi=0;
				}else{
					hi=1;
				}
				
				if(vw){
					lo=2;
				}else{
					lo=1;
				}
				
			}else{
				
				if(vw){
					hi=1;
				}else{
					hi=2;
				}
				
				if(uv){
					lo=1;
				}else{
					lo=0;
				}
				
			}
			
			int index=3 - hi - lo;
			
			return kay(hi) + kay(index) + kay(lo) + kay(0);
		}

		float kay(int a) {
			s = (A[0] + A[1] + A[2]) * onesixth;
			float x = u - A[0] + s;
			float y = v - A[1] + s;
			float z = w - A[2] + s;
			float t = 0.6f - x * x - y * y - z * z;
			int h = shuffle(i + A[0], j + A[1], k + A[2]);
			A[a]++;
			
			if (t < 0){
				return 0;
			}
			
			int b5 = h >> 5 & 1;
			int b4 = h >> 4 & 1;
			int b3 = h >> 3 & 1;
			int b2 = h >> 2 & 1;
			int b1 = h & 3;

			float p = b1 == 1 ? x : b1 == 2 ? y : z;
			float q = b1 == 1 ? y : b1 == 2 ? z : x;
			float r = b1 == 1 ? z : b1 == 2 ? x : y;

			p = b5 == b3 ? -p : p;
			q = b5 == b4 ? -q : q;
			r = b5 != (b4 ^ b3) ? -r : r;
			t *= t;
			return 8 * t * t * (p + (b1 == 0 ? q + r : b2 == 0 ? q : r));
		}

		int shuffle(int i, int j, int k) {
			return b(i, j, k, 0) + b(j, k, i, 1) + b(k, i, j, 2) + b(i, j, k, 3) + b(j, k, i, 4) + b(k, i, j, 5) + b(i, j, k, 6) + b(j, k, i, 7);
		}

		int b(int i, int j, int k, int B) {
			return T[b(i, B) << 2 | b(j, B) << 1 | b(k, B)];
		}

		int b(int N, int B) {
			return N >> B & 1;
		}
		
		int fastfloor(float n) {
			return n > 0 ? (int)n : (int)n - 1;
		}
		
	}
	
}