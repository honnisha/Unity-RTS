using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;


namespace WebAssembly{
	
	/// <summary>
	/// Contains static methods to ease reflection operations.
	/// </summary>
	internal static class ReflectionHelpers
	{
		
		// Control/ mem
		internal static MethodInfo Imports_GetMemory;
		internal static MethodInfo IntPtr_NativePointer;
		internal static MethodInfo Runtime_Trap;
		internal static MethodInfo Module_QueryMemory;
		internal static MethodInfo Module_GrowMemory;
		
		// Math (float)
		internal static MethodInfo Math_Abs32;
		internal static MethodInfo Math_Min32;
		internal static MethodInfo Math_Max32;
		
		// Math (double)
		internal static MethodInfo Math_Abs64;
		internal static MethodInfo Math_Ceiling64;
		internal static MethodInfo Math_Floor64;
		internal static MethodInfo Math_Min64;
		internal static MethodInfo Math_Max64;
		internal static MethodInfo Math_Sqrt64;
		internal static MethodInfo Math_Truncate64;
		internal static MethodInfo Math_Round64;
		
		// Other opcodes:
		internal static MethodInfo OpcodeMethods_IndexToMethod;
		internal static MethodInfo OpcodeMethods_Copysign64;
		internal static MethodInfo OpcodeMethods_Copysign32;
		internal static MethodInfo OpcodeMethods_Rotr64;
		internal static MethodInfo OpcodeMethods_Rotl64;
		internal static MethodInfo OpcodeMethods_Rotr32;
		internal static MethodInfo OpcodeMethods_Rotl32;
		internal static MethodInfo OpcodeMethods_Popcnt32;
		internal static MethodInfo OpcodeMethods_Clz32;
		internal static MethodInfo OpcodeMethods_Ctz32;
		internal static MethodInfo OpcodeMethods_Popcnt64;
		internal static MethodInfo OpcodeMethods_Clz64;
		internal static MethodInfo OpcodeMethods_Ctz64;
		
		// Conversions
		public static ConstructorInfo FloatBits_ConstructFloat;
		public static ConstructorInfo FloatBits_ConstructInt;
		public static ConstructorInfo DoubleBits_ConstructDouble;
		public static ConstructorInfo DoubleBits_ConstructULong;
		public static FieldInfo FloatBits_Int;
		public static FieldInfo FloatBits_Float;
		public static FieldInfo DoubleBits_Long;
		public static FieldInfo DoubleBits_Double;
		
		/// <summary>
		/// Initializes static members of this class.
		/// </summary>
		static ReflectionHelpers(){
			
			// Control/ mem
			Imports_GetMemory = GetStaticMethod(typeof(Imports), "GetMemory");
			IntPtr_NativePointer = GetInstanceMethod(typeof(IntPtr), "ToPointer");
			Runtime_Trap = GetStaticMethod(typeof(Runtime), "Trap");
			Module_QueryMemory = GetInstanceMethod(typeof(Module), "QueryMemory");
			Module_GrowMemory = GetInstanceMethod(typeof(Module), "GrowMemory",typeof(int));
			
			// Math (double)
			Math_Sqrt64 = GetStaticMethod(typeof(System.Math), "Sqrt",typeof(double));
			Math_Ceiling64 = GetStaticMethod(typeof(System.Math), "Ceiling",typeof(double));
			Math_Floor64 = GetStaticMethod(typeof(System.Math), "Floor",typeof(double));
			Math_Min64 = GetStaticMethod(typeof(System.Math), "Min",typeof(double),typeof(double));
			Math_Max64 = GetStaticMethod(typeof(System.Math), "Max",typeof(double),typeof(double));
			Math_Abs64 = GetStaticMethod(typeof(System.Math), "Abs",typeof(double));
			Math_Truncate64 = GetStaticMethod(typeof(System.Math), "Truncate",typeof(double));
			Math_Round64 = GetStaticMethod(typeof(System.Math), "Round",typeof(double));
			
			// Other extensions:
			OpcodeMethods_Copysign32 = GetStaticMethod(typeof(OpcodeMethods), "Copysign",typeof(float),typeof(float));
			OpcodeMethods_Copysign64 = GetStaticMethod(typeof(OpcodeMethods), "Copysign",typeof(double),typeof(double));
			OpcodeMethods_IndexToMethod = GetStaticMethod(typeof(OpcodeMethods),"IndexToMethod",typeof(int),typeof(Module));
			
			OpcodeMethods_Rotr64 = GetStaticMethod(typeof(OpcodeMethods), "Rotr64",typeof(ulong),typeof(int));
			OpcodeMethods_Rotl64 = GetStaticMethod(typeof(OpcodeMethods), "Rotl64",typeof(ulong),typeof(int));
			OpcodeMethods_Rotr32 = GetStaticMethod(typeof(OpcodeMethods), "Rotr32",typeof(uint),typeof(int));
			OpcodeMethods_Rotl32 = GetStaticMethod(typeof(OpcodeMethods), "Rotl32",typeof(uint),typeof(int));
			
			OpcodeMethods_Popcnt32 = GetStaticMethod(typeof(OpcodeMethods), "Popcnt32",typeof(int));
			OpcodeMethods_Clz32 = GetStaticMethod(typeof(OpcodeMethods), "Clz32",typeof(int));
			OpcodeMethods_Ctz32 = GetStaticMethod(typeof(OpcodeMethods), "Ctz32",typeof(int));
			OpcodeMethods_Popcnt64 = GetStaticMethod(typeof(OpcodeMethods), "Popcnt64",typeof(long));
			OpcodeMethods_Clz64 = GetStaticMethod(typeof(OpcodeMethods), "Clz64",typeof(long));
			OpcodeMethods_Ctz64 = GetStaticMethod(typeof(OpcodeMethods), "Ctz64",typeof(long));
			
			// Conversions:
			FloatBits_ConstructFloat = GetConstructor(typeof(FloatBits),typeof(float));
			FloatBits_ConstructInt = GetConstructor(typeof(FloatBits),typeof(uint));
			DoubleBits_ConstructDouble = GetConstructor(typeof(DoubleBits),typeof(double));
			DoubleBits_ConstructULong = GetConstructor(typeof(DoubleBits),typeof(ulong));
			FloatBits_Int = GetField(typeof(FloatBits),"U");
			FloatBits_Float = GetField(typeof(FloatBits),"F");
			DoubleBits_Long = GetField(typeof(FloatBits),"U");
			DoubleBits_Double = GetField(typeof(FloatBits),"F");
			
			// Math (float)
			Math_Abs32 = GetStaticMethod(typeof(System.Math), "Abs",typeof(float));
			Math_Min32 = GetStaticMethod(typeof(System.Math), "Min",typeof(float),typeof(float));
			Math_Max32 = GetStaticMethod(typeof(System.Math), "Max",typeof(float),typeof(float));
			
		}
		
		/// <summary>
		/// Gets an instance or static field.
		/// </summary>
		public static FieldInfo GetField(Type type,string name){
			FieldInfo result = type.GetField(name);
			if (result == null)
				throw new InvalidOperationException(string.Format("The field '{1}' does not exist on type '{0}'.", type, name));
			return result;
		}
		
		/// <summary>
		/// Gets a constructor.
		/// </summary>
		public static ConstructorInfo GetConstructor(Type type,params Type[] parameterTypes){
			#if NETFX_CORE
			ConstructorInfo result = type.GetConstructor(parameterTypes);
			#else
			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
			ConstructorInfo result = type.GetConstructor(flags,null,parameterTypes,null);
			#endif
			if (result == null)
				throw new InvalidOperationException(string.Format("The constructor {0} does not exist.", type.FullName));
			return result;
		}
		
		/// <summary>
		/// Gets the MethodInfo for an instance method.  Throws an exception if the search fails.
		/// </summary>
		/// <param name="type"> The type to search. </param>
		/// <param name="name"> The name of the method to search for. </param>
		/// <param name="parameterTypes"> The types of the parameters accepted by the method. </param>
		/// <returns> The MethodInfo for the method. </returns>
		public static MethodInfo GetInstanceMethod(Type type, string name, params Type[] parameterTypes){
			#if NETFX_CORE
			MethodInfo result = type.GetMethod(name,parameterTypes);
			#else
			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
			MethodInfo result = type.GetMethod(name, flags, null, parameterTypes, null);
			#endif
			if (result == null)
				throw new InvalidOperationException(string.Format("The instance method {0}.{1} does not exist.", type.FullName, name));
			return result;
		}
		
		/// <summary>
		/// Gets the MethodInfo for a static method.  Throws an exception if the search fails.
		/// </summary>
		/// <param name="type"> The type to search. </param>
		/// <param name="name"> The name of the method to search for. </param>
		/// <param name="parameterTypes"> The types of the parameters accepted by the method. </param>
		/// <returns> The MethodInfo for the method. </returns>
		public static MethodInfo GetStaticMethod(Type type, string name, params Type[] parameterTypes){
			#if NETFX_CORE
			MethodInfo result = type.GetMethod(name,parameterTypes);
			#else
			var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.DeclaredOnly;
			MethodInfo result = type.GetMethod(name, flags, null, parameterTypes, null);
			#endif
			if (result == null)
				throw new InvalidOperationException(string.Format("The static method {0}.{1} does not exist.", type.FullName, name));
			return result;
		}
		
	}

}