using System;
using System.Runtime.InteropServices;


namespace WebAssembly{
	
	/// <summary>
	/// Implementations for the opcodes which aren't natively available in .NET.
	/// </summary>
	
	public static class OpcodeMethods{
		
		/// <summary>
		/// Converts a method index into a MethodInfo handle for a calli invoke.
		/// </summary>
		public static IntPtr IndexToMethod(int index,Module module){
			
			// Read the index from the table:
			// module.Table[index];
			return default(IntPtr);
			
		}
		
		/// <usmmary>Counts the ones.</summary>
		public static int Popcnt64(long x){
			// Count the ones:
			x -= (x >> 1) & 0x5555555555555555;
			x = (x & 0x3333333333333333) + ((x >> 2) & 0x3333333333333333);
			x = (x + (x >> 4)) & 0x0f0f0f0f0f0f0f0f;
			return 64 - (int)((x * 0x0101010101010101) >> 56);
		}
		
		/// <usmmary>Counts the ones.</summary>
		public static int Popcnt32(int x){
			x -= x >> 1 & 0x55555555;
			x = (x >> 2 & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			x += x >> 16;
			return (x & 0x0000003f);
		}
		
		/// <summary>Counts the number of leading zeroes.</summary>
		public static int Clz64(long x){
			// Smear it:
			x |= x >> 1; 
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			x |= x >> 32;
			
			// Count the ones:
			x -= (x >> 1) & 0x5555555555555555;
			x = (x & 0x3333333333333333) + ((x >> 2) & 0x3333333333333333);
			x = (x + (x >> 4)) & 0x0f0f0f0f0f0f0f0f;
			return 64 - (int)((x * 0x0101010101010101) >> 56);
		}
		
		/// <summary>Counts the number of leading zeroes.</summary>
		public static int Clz32(int x){
			// Smear it:
			x |= x >> 1; 
			x |= x >> 2;
			x |= x >> 4;
			x |= x >> 8;
			x |= x >> 16;
			
			// Count the ones:
			x -= x >> 1 & 0x55555555;
			x = (x >> 2 & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			x += x >> 16;
			
			// Subtract # of 1s from 32
			return 32 - (x & 0x0000003f);
		}
		
		/// <summary>Count the number of trailing zeroes.</summary>
		public static int Ctz64(long x){
			// ..do this (http://aggregate.org/MAGIC/#Trailing%20Zero%20Count):
			x = (x & -x) - 1;
			
			// Count the ones:
			x -= (x >> 1) & 0x5555555555555555;
			x = (x & 0x3333333333333333) + ((x >> 2) & 0x3333333333333333);
			x = (x + (x >> 4)) & 0x0f0f0f0f0f0f0f0f;
			return 64 - (int)((x * 0x0101010101010101) >> 56);
		}
		
		/// <summary>Counts the number of trailing zeroes.</summary>
		public static int Ctz32(int x){
			// ..do this (http://aggregate.org/MAGIC/#Trailing%20Zero%20Count):
			x = (x & -x) - 1;
			
			// Count the ones:
			x -= x >> 1 & 0x55555555;
			x = (x >> 2 & 0x33333333) + (x & 0x33333333);
			x = (x >> 4) + x & 0x0f0f0f0f;
			x += x >> 8;
			x += x >> 16;
			
			// Subtract # of 1s from 32
			return 32 - (x & 0x0000003f);
		}
		
		/// <summary>Rotates the given number rightwards.</summary>
		public static ulong Rotr64(ulong original,int bits){
			return (original << bits) | (original >> (64 -bits));
		}
		
		/// <summary>Rotates the given number leftwards.</summary>
		public static ulong Rotl64(ulong original,int bits){
			return (original << bits) | (original >> (64 -bits));
		}
		
		/// <summary>Rotates the given number rightwards.</summary>
		public static uint Rotr32(uint original,int bits){
			return (original << bits) | (original >> (32 -bits));
		}
		
		/// <summary>Rotates the given number leftwards.</summary>
		public static uint Rotl32(uint original,int bits){
			return (original << bits) | (original >> (32 -bits));
		}
		
		/// <summary>
		/// Applies the sign of y onto the abs of x.
		/// </summary>
		public static double Copysign(double x,double y){
			// It's friendlier than it looks!
			DoubleBits xbits = new DoubleBits(x);
			xbits.U = xbits.U & ~((ulong)1<<63) | new DoubleBits(y).U & ((ulong)1<<63);
			return xbits.F;
		}
		
		/// <summary>
		/// Applies the sign of y onto the abs of x.
		/// </summary>
		public static float Copysign(float x,float y){
			// It's friendlier than it looks!
			FloatBits xbits = new FloatBits(x);
			xbits.U = xbits.U & ~((uint)1<<31) | new FloatBits(y).U & ((uint)1<<31);
			return xbits.F;
		}
		
	}
	
	
	/// <summary>A struct for rapidly accessing the bits of an f32 in a safe context.</summary>
	[StructLayout(LayoutKind.Explicit)]
	internal struct FloatBits{
		
		/// <summary>Accessing it as a float</summary>
		[FieldOffset(0)]
		public float F;
		/// <summary>Accessing it as a uint.</summary>
		[FieldOffset(0)]
		public uint U;
		
		public FloatBits(float f){
			U=0;
			F=f;
		}
		
		public FloatBits(uint u){
			F=0;
			U=u;
		}
		
	}
	
	/// <summary>A struct for rapidly accessing the bits of an f64 in a safe context.</summary>
	[StructLayout(LayoutKind.Explicit)]
	internal struct DoubleBits{
		
		/// <summary>Accessing it as a double</summary>
		[FieldOffset(0)]
		public double F;
		/// <summary>Accessing it as a ulong.</summary>
		[FieldOffset(0)]
		public ulong U;
		
		
		public DoubleBits(double f){
			U=0;
			F=f;
		}
		
		public DoubleBits(ulong u){
			F=0;
			U=u;
		}
		
	}
	
}