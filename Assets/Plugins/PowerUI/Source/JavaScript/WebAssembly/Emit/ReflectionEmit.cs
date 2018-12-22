#if UNITY_EDITOR || UNITY_WEBPLAYER || UNITY_STANDALONE || (UNITY_ANDROID && !ENABLE_IL2CPP)
	#define AbleToCompile
#endif

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Diagnostics;
using System.IO;


/*
	This file contains everything that uses the Reflection.Emit API.
	It's all in one file so we have only one place to platform test for support 
	(as we can't easily share #defines across multiple files).
*/

#if AbleToCompile

namespace WebAssembly
{
	
	/// <summary>
	/// Represents a label in IL code.
	/// </summary>
	public class ReflectionEmitILLabel : ILLabel
	{
		/// <summary>
		/// Creates a new label instance.
		/// </summary>
		/// <param name="label"> The underlying label. </param>
		public ReflectionEmitILLabel(System.Reflection.Emit.Label label)
		{
			this.UnderlyingLabel = label;
		}

		/// <summary>
		/// Gets the underlying label.
		/// </summary>
		public System.Reflection.Emit.Label UnderlyingLabel;
	}
	
	/// <summary>
	/// Represents a label in IL code.
	/// </summary>
	public class ReflectionEmitType : ILTypeBuilder
	{
		
		/// <summary>The host runtime. either a WebAssembly.Module or a ScriptEngine.</summary>
		public Runtime Runtime;
		
		/// <summary>
		/// The underlying type builder for this prototype.
		/// </summary>
		public TypeBuilder UnderlyingBuilder;
		
		/// <summary>
		/// Creates a new Type builder instance.
		/// </summary>
		/// <param name="type"> The underlying builder. </param>
		public ReflectionEmitType(TypeBuilder type,Runtime runtime)
		{
			UnderlyingBuilder = type;
			Runtime=runtime;
		}
		
		/// <summary>The actual type being built.</summary>
		public override Type Type{
			get{
				return UnderlyingBuilder;
			}
		}
		
		/// <summary>Defines the default constructor.</summary>
		public override ConstructorInfo DefineConstructor(){
			return UnderlyingBuilder.DefineDefaultConstructor(MethodAttributes.Public);
		}
		
		/// <summary>Defines the default public constructor.</summary>
		public override ILGenerator DefineConstructor(MethodAttributes attribs,Type[] parameters){
			
			// Create the method builder now:
			var ctrBuilder = UnderlyingBuilder.DefineConstructor(attribs,CallingConventions.Standard, parameters);
			
			// Generate the IL for the method.
			ILGenerator generator = new ReflectionEmitILGenerator(Runtime,ctrBuilder);
			
			if (Module.EnableILAnalysis)
			{
				// Replace the generator with one that logs.
				generator = new LoggingILGenerator(generator);
			}
			
			return generator;
		}
		
		/// <summary>Call this when you're done building the type.</summary>
		public override Type Close(){
			return UnderlyingBuilder.CreateType();
		}
		
		/// <summary>Define a method on this type.</summary>
		public override ILGenerator DefineMethod(
			string name,
			MethodAttributes attribs,
			Type returnType,
			Type[] parameters
		){
			
			// Create the method builder now:
			var methodBuilder = UnderlyingBuilder.DefineMethod(name,
				attribs,
				returnType, parameters);
			
			// Generate the IL for the method.
			ILGenerator generator = new ReflectionEmitILGenerator(Runtime,methodBuilder);
			
			if (Module.EnableILAnalysis)
			{
				// Replace the generator with one that logs.
				generator = new LoggingILGenerator(generator);
			}
			
			return generator;
		}
		
		/// <summary>Define a method on this type.</summary>
		public override FieldInfo DefineField(
			string name,
			Type fieldType,
			FieldAttributes attribs
		){
			return UnderlyingBuilder.DefineField(name,fieldType,attribs);
		}
		
	}

	internal class ReflectionEmitModuleInfo : ILModuleInfo
	{
		
		/// <summary>
		/// The type which holds all currently compiling methods.
		/// </summary>
		private ReflectionEmitType MainBuilder_;
		
		/// <summary>
		/// A reference to the static field holding the memory void* (WebAssembly only).
		/// </summary>
		private FieldInfo Memory_;
		
		/// <summary>
		/// When the script engine starts, it generates a static class to hold general info.
		/// This is the static reference to the *runtime* which is either 
		/// a ScriptEngine or a WebAssembly module.
		/// </summary>
		private FieldInfo GlobalRuntime_;
		
		/// <summary>The scope set as a static instance.</summary>
		private FieldInfo GlobalImports_;
		
		/// <summary>
		/// The global type which holds general static information such as a reference to the script engine.
		/// </summary>
		private Type GlobalType_;
		
		/// <summary>
		/// The type which holds all currently compiling methods.
		/// </summary>
		public override ILTypeBuilder MainBuilder{
			get{
				return MainBuilder_;
			}
		}
		
		/// <summary>A reference to the static field holding the memory void* (WebAssembly only).</summary>
		public override FieldInfo Memory{
			get{
				return Memory_;
			}
			set{
				Memory_=value;
			}
		}
		
		/// <summary>The global ScriptEngine or WebAssembly Module as a static instance.</summary>
		public override FieldInfo GlobalRuntime{
			get{
				return GlobalRuntime_;
			}
		}
		
		/// <summary>The imported global scope as a static instance.</summary>
		public override FieldInfo GlobalImports{
			get{
				return GlobalImports_;
			}
		}
		
		/// <summary>
		/// A reference to the current global variable ID.
		/// </summary>
		public int GlobalID;
		/// <summary>The WebAssembly runtime this belongs to (always a Module).</summary>
		public Runtime Runtime;
		/// <summary>Builds the assembly.</summary>
		public System.Reflection.Emit.AssemblyBuilder AssemblyBuilder;
		/// <summary>Builds the module.</summary>
		public System.Reflection.Emit.ModuleBuilder ModuleBuilder;
		/// <summary>Total types emitted.</summary>
		public int TypeCount;
		/// <summary>The path to a precompiled DLL, if required.</summary>
		private string DllPath;
		/// <summary>The engine name. Always just 'WebAssembly'.</summary>
		private string EngineName;
		
		
		internal ReflectionEmitModuleInfo(Runtime runtime){
			Runtime=runtime;
			EngineName=runtime.EngineName;
		}
		
		/// <summary>Create a global.</summary>
		public override FieldInfo CreateGlobal(Type type){
			GlobalID++;
			return Define(MainBuilder_.UnderlyingBuilder,"__global_"+GlobalID,type);
		}
		
		public override void CreateMainType(string uniqueID){
			
			// Path to the cache file is..
			string filepath=Runtime.CachePath;
			
			if(filepath==null || uniqueID==null){
				
				// Don't cache.
				AssemblyBuilder = System.Threading.Thread.GetDomain().DefineDynamicAssembly(
					new AssemblyName(EngineName+"-"+uniqueID), AssemblyBuilderAccess.Run);
				
				ModuleBuilder=AssemblyBuilder.DefineDynamicModule(EngineName);
				
			}else{
				
				// Create it if needed:
				if(!Directory.Exists(filepath)){
					Directory.CreateDirectory(filepath);
				}
				
				// Create a dynamic assembly and module.
				AssemblyBuilder = System.Threading.Thread.GetDomain().DefineDynamicAssembly(
					new AssemblyName(EngineName+"-"+uniqueID), AssemblyBuilderAccess.RunAndSave, filepath);
				
				// Add the unique ID:
				filepath+="/"+uniqueID+".dll";
				
				// Create the module:
				ModuleBuilder = AssemblyBuilder.DefineDynamicModule(EngineName, uniqueID+".dll");
				
				// Save the DLL here:
				DllPath=filepath;
				
			}
			
			// Create a new type to hold our method.
			TypeBuilder entryBuilder = ModuleBuilder.DefineType(EngineName+"_EntryPoint", 
				System.Reflection.TypeAttributes.Public | System.Reflection.TypeAttributes.Class
			);
			
			MainBuilder_ = new ReflectionEmitType(entryBuilder,Runtime);
			
			TypeCount++;
			
		}
		
		public override Type Close()
		{
			Type result=MainBuilder_.Close();
			MainBuilder_=null;
			
			if(DllPath!=null){
				if(File.Exists(DllPath)){
					// Delete it!
					File.Delete(DllPath);
				}
				
				// save the builder:
				AssemblyBuilder.Save(System.IO.Path.GetFileName(DllPath));
			}
			
			return result;
		}
		
		public override void SetupGlobalCache(object imports)
		{
			if(GlobalType_!=null)
			{
				return;
			}
			
			// Create a new type to hold our method.
			var typeBuilder = ModuleBuilder.DefineType("GlobalCache", TypeAttributes.Public | TypeAttributes.Class);
			
			Define(typeBuilder,"Runtime",Runtime.GetType());
			
			if(imports!=null){
				Define(typeBuilder,"Imports",imports.GetType());
			}
			
			// Build it now:
			GlobalType_ = typeBuilder.CreateType();
			
			// Get the fields:
			GlobalRuntime_=Get("Runtime");
			GlobalRuntime_.SetValue(null,Runtime);
			
			if(imports==null){
				GlobalImports_=null;
			}else{
				GlobalImports_=Get("Imports");
				GlobalImports_.SetValue(null,imports);
			}
			
		}
		
		/// <summary>
		/// Defines a static field on the given type builder.
		/// </summary>
		private FieldInfo Define(TypeBuilder builder,string field,Type fieldType){
			return builder.DefineField(field,fieldType,FieldAttributes.Static | FieldAttributes.Public);
		}
		
		/// <summary>
		/// Gets the field info for the given field on the global cache object.
		/// </summary>
		private FieldInfo Get(string name){
			return GlobalType_.GetField(name);
		}
		
		/// <summary>Allocates a new type.</summary>
		public override ILTypeBuilder AllocateType(ref string name,Type baseType,bool isStatic){
			
			if(baseType==null){
				baseType=typeof(object);
			}
			
			if(name==null){
				name="__proto__"+TypeCount;
				TypeCount++;
			}
			
			TypeAttributes ta = TypeAttributes.Public | TypeAttributes.Class;
			
			if(isStatic){
				// Add sealed and abstract:
				ta |= TypeAttributes.Sealed | TypeAttributes.Abstract;
			}
			
			// Define the type now; note that methods are not declared on these (JS defined methods are always static):
			return new ReflectionEmitType(
				ModuleBuilder.DefineType(name,ta,baseType),
				Runtime
			);
			
		}
		
	}
	
	/// <summary>
	/// Represents a local variable in CIL code.
	/// </summary>
	internal class ReflectionEmitILLocalVariable : ILLocalVariable
	{
		private string name;
		
		/// <summary>
		/// Creates a new local variable instance.
		/// </summary>
		/// <param name="local"> The underlying local variable. </param>
		/// <param name="name"> The name of the local variable.  Can be <c>null</c>. </param>
		public ReflectionEmitILLocalVariable(System.Reflection.Emit.LocalBuilder local, string name)
		{
			if (local == null)
				throw new ArgumentNullException("local");
			this.UnderlyingLocal = local;
			this.name = name;
			//if (name != null)
			//	local.SetLocalSymInfo(name);
		}

		/// <summary>
		/// Gets the underlying local variable.
		/// </summary>
		public System.Reflection.Emit.LocalBuilder UnderlyingLocal;

		/// <summary>
		/// Gets the zero-based index of the local variable within the method body.
		/// </summary>
		public override int Index
		{
			get { return this.UnderlyingLocal.LocalIndex; }
		}

		/// <summary>
		/// Gets the type of the local variable.
		/// </summary>
		public override Type Type
		{
			get { return this.UnderlyingLocal.LocalType; }
		}

		/// <summary>
		/// Gets the local variable name, or <c>null</c> if a name was not provided.
		/// </summary>
		public override string Name
		{
			get { return this.name; }
		}
	}
	
	/// <summary>
	/// Represents a generator of CIL bytes.
	/// </summary>
	public class ReflectionEmitILGenerator : ILGenerator
	{
		
		private System.Reflection.Emit.ILGenerator generator;
		private System.Reflection.Emit.MethodBuilder builder;

		/// <summary>
		/// Creates a new ReflectionEmitILGenerator instance.
		/// </summary>
		/// <param name="generator"> The ILGenerator that is used to output the IL. </param>
		public ReflectionEmitILGenerator(Runtime runtime,System.Reflection.Emit.MethodBuilder builder)
		{
			this.generator=builder.GetILGenerator();
			this.builder=builder;
			Runtime = runtime;
		}
		
		/// <summary>
		/// Creates a new ReflectionEmitILGenerator instance.
		/// </summary>
		/// <param name="generator"> The ILGenerator that is used to output the IL. </param>
		public ReflectionEmitILGenerator(Runtime runtime,System.Reflection.Emit.ConstructorBuilder builder)
		{
			this.generator=builder.GetILGenerator();
			Runtime = runtime;
		}
		
		public override MethodInfo Method{
			get{
				return builder;
			}
		}
		
		/// <summary>
		/// Emits a return statement and finalizes the generated code.  Do not emit any more
		/// instructions after calling this method.
		/// </summary>
		public override void Complete(string name, Type[] arguments, Type returnType, bool emitReturn)
		{
			if(emitReturn){
				Return();
			}
		}
		
		//	 STACK MANAGEMENT
		//_________________________________________________________________________________________

		/// <summary>
		/// Pops the value from the top of the stack.
		/// </summary>
		public override void Pop()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Pop);
		}

		/// <summary>
		/// Duplicates the value on the top of the stack.
		/// </summary>
		public override void Duplicate()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Dup);
		}



		//	 BRANCHING AND LABELS
		//_________________________________________________________________________________________

		/// <summary>
		/// Creates a label without setting its position.
		/// </summary>
		/// <returns> A new label. </returns>
		public override ILLabel CreateLabel()
		{
			return new ReflectionEmitILLabel(this.generator.DefineLabel());
		}

		/// <summary>
		/// Defines the position of the given label.
		/// </summary>
		/// <param name="label"> The label to define. </param>
		public override void DefineLabelPosition(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.MarkLabel(((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Unconditionally branches to the given label.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void Branch(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Br, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the value on the top of the stack is zero.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfZero(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Brfalse, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the value on the top of the stack is non-zero, true or
		/// non-null.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfNotZero(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Brtrue, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the two values on the top of the stack are equal.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfEqual(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Beq, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the two values on the top of the stack are not equal.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfNotEqual(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Bne_Un, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is greater than the second
		/// value on the stack.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfGreaterThan(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Bgt, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is greater than the second
		/// value on the stack.  If the operands are integers then they are treated as if they are
		/// unsigned.  If the operands are floating point numbers then a NaN value will trigger a
		/// branch.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfGreaterThanUnsigned(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Bgt_Un, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is greater than or equal to
		/// the second value on the stack.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfGreaterThanOrEqual(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Bge, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is greater than or equal to
		/// the second value on the stack.  If the operands are integers then they are treated as
		/// if they are unsigned.  If the operands are floating point numbers then a NaN value will
		/// trigger a branch.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfGreaterThanOrEqualUnsigned(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Bge_Un, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is less than the second
		/// value on the stack.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfLessThan(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Blt, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is less than the second
		/// value on the stack.  If the operands are integers then they are treated as if they are
		/// unsigned.  If the operands are floating point numbers then a NaN value will trigger a
		/// branch.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfLessThanUnsigned(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Blt_Un, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is less than or equal to
		/// the second value on the stack.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfLessThanOrEqual(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ble, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Branches to the given label if the first value on the stack is less than or equal to
		/// the second value on the stack.  If the operands are integers then they are treated as
		/// if they are unsigned.  If the operands are floating point numbers then a NaN value will
		/// trigger a branch.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void BranchIfLessThanOrEqualUnsigned(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ble_Un, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// Returns from the current method.  A value is popped from the stack and used as the
		/// return value.
		/// </summary>
		public override void Return()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ret);
		}

		/// <summary>
		/// Creates a jump table.  A value is popped from the stack - this value indicates the
		/// index of the label in the <paramref name="labels"/> array to jump to.
		/// </summary>
		/// <param name="labels"> A array of labels. </param>
		public override void Switch(ILLabel[] labels)
		{
			if (labels == null)
				throw new ArgumentNullException("labels");

			var reflectionLabels = new System.Reflection.Emit.Label[labels.Length];
			for (int i = 0; i < labels.Length; i++)
				reflectionLabels[i] = ((ReflectionEmitILLabel)labels[i]).UnderlyingLabel;
			this.generator.Emit(System.Reflection.Emit.OpCodes.Switch, reflectionLabels);
		}



		//	 LOCAL VARIABLES AND ARGUMENTS
		//_________________________________________________________________________________________

		/// <summary>
		/// Declares a new local variable.
		/// </summary>
		/// <param name="type"> The type of the local variable. </param>
		/// <param name="name"> The name of the local variable. Can be <c>null</c>. </param>
		/// <returns> A new local variable. </returns>
		public override ILLocalVariable DeclareVariable(Type type, string name)
		{
			return new ReflectionEmitILLocalVariable(this.generator.DeclareLocal(type), name);
		}

		/// <summary>
		/// Pushes the value of the given variable onto the stack.
		/// </summary>
		/// <param name="variable"> The variable whose value will be pushed. </param>
		public override void LoadVariable(ILLocalVariable variable)
		{
			if (variable as ReflectionEmitILLocalVariable == null)
				throw new ArgumentNullException("variable");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldloc, ((ReflectionEmitILLocalVariable)variable).UnderlyingLocal);
		}

		/// <summary>
		/// Pushes the address of the given variable onto the stack.
		/// </summary>
		/// <param name="variable"> The variable whose address will be pushed. </param>
		public override void LoadAddressOfVariable(ILLocalVariable variable)
		{
			if (variable as ReflectionEmitILLocalVariable == null)
				throw new ArgumentNullException("variable");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldloca, ((ReflectionEmitILLocalVariable)variable).UnderlyingLocal);
		}

		/// <summary>
		/// Pops the value from the top of the stack and stores it in the given local variable.
		/// </summary>
		/// <param name="variable"> The variable to store the value. </param>
		public override void StoreVariable(ILLocalVariable variable)
		{
			if (variable as ReflectionEmitILLocalVariable == null)
				throw new ArgumentNullException("variable");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Stloc, ((ReflectionEmitILLocalVariable)variable).UnderlyingLocal);
		}

		/// <summary>
		/// Pushes the value of the method argument with the given index onto the stack.
		/// </summary>
		/// <param name="argumentIndex"> The index of the argument to push onto the stack. </param>
		public override void LoadArgument(int argumentIndex)
		{
			if (argumentIndex < 0)
				throw new ArgumentOutOfRangeException("argumentIndex");
			switch (argumentIndex)
			{
				case 0:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_0);
					break;
				case 1:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_1);
					break;
				case 2:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_2);
					break;
				case 3:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_3);
					break;
				default:
					if (argumentIndex < 256)
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldarg_S, (byte)argumentIndex);
					else
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldarg, (short)argumentIndex);
					break;
			}
		}

		/// <summary>
		/// Pops a value from the stack and stores it in the method argument with the given index.
		/// </summary>
		/// <param name="argumentIndex"> The index of the argument to store into. </param>
		public override void StoreArgument(int argumentIndex)
		{
			if (argumentIndex < 0)
				throw new ArgumentOutOfRangeException("argumentIndex");
			if (argumentIndex < 256)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Starg_S, (byte)argumentIndex);
			else
				this.generator.Emit(System.Reflection.Emit.OpCodes.Starg, (short)argumentIndex);
		}



		//	 LOAD CONSTANT
		//_________________________________________________________________________________________

		/// <summary>
		/// Pushes <c>null</c> onto the stack.
		/// </summary>
		public override void LoadNull()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldnull);
		}

		/// <summary>
		/// Pushes a constant value onto the stack.
		/// </summary>
		/// <param name="value"> The integer to push onto the stack. </param>
		public override void LoadInt32(int value)
		{
			if (value >= -1 && value <= 8)
			{
				switch (value)
				{
					case -1:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_M1);
						break;
					case 0:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_0);
						break;
					case 1:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_1);
						break;
					case 2:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_2);
						break;
					case 3:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_3);
						break;
					case 4:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_4);
						break;
					case 5:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_5);
						break;
					case 6:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_6);
						break;
					case 7:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_7);
						break;
					case 8:
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_8);
						break;
				}
				
			}
			else if (value >= -128 && value < 128)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4_S, (byte)value);
			else
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I4, value);
		}

		/// <summary>
		/// Pushes a 64-bit constant value onto the stack.
		/// </summary>
		/// <param name="value"> The 64-bit integer to push onto the stack. </param>
		public override void LoadInt64(long value)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_I8, value);
		}

		/// <summary>
		/// Pushes a constant value onto the stack.
		/// </summary>
		/// <param name="value"> The number to push onto the stack. </param>
		public override void LoadDouble(double value)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_R8, value);
		}
		
		/// <summary>
		/// Pushes a constant value onto the stack.
		/// </summary>
		/// <param name="value"> The number to push onto the stack. </param>
		public override void LoadSingle(float value)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldc_R4, value);
		}
		
		/// <summary>
		/// Pushes a constant value onto the stack.
		/// </summary>
		/// <param name="value"> The string to push onto the stack. </param>
		public override void LoadString(string value)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldstr, value);
		}



		//	 RELATIONAL OPERATIONS
		//_________________________________________________________________________________________

		/// <summary>
		/// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
		/// is equal to the second, or <c>0</c> otherwise.  Produces <c>0</c> if one or both
		/// of the arguments are <c>NaN</c>.
		/// </summary>
		public override void CompareEqual()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ceq);
		}

		/// <summary>
		/// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
		/// is greater than the second, or <c>0</c> otherwise.  Produces <c>0</c> if one or both
		/// of the arguments are <c>NaN</c>.
		/// </summary>
		public override void CompareGreaterThan()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Cgt);
		}

		/// <summary>
		/// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
		/// is greater than the second, or <c>0</c> otherwise.  Produces <c>1</c> if one or both
		/// of the arguments are <c>NaN</c>.  Integers are considered to be unsigned.
		/// </summary>
		public override void CompareGreaterThanUnsigned()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Cgt_Un);
		}

		/// <summary>
		/// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
		/// is less than the second, or <c>0</c> otherwise.  Produces <c>0</c> if one or both
		/// of the arguments are <c>NaN</c>.
		/// </summary>
		public override void CompareLessThan()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Clt);
		}

		/// <summary>
		/// Pops two values from the stack, compares, then pushes <c>1</c> if the first argument
		/// is less than the second, or <c>0</c> otherwise.  Produces <c>1</c> if one or both
		/// of the arguments are <c>NaN</c>.  Integers are considered to be unsigned.
		/// </summary>
		public override void CompareLessThanUnsigned()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Clt_Un);
		}



		//	 ARITHMETIC AND BITWISE OPERATIONS
		//_________________________________________________________________________________________

		/// <summary>
		/// Pops two values from the stack, adds them together, then pushes the result to the
		/// stack.
		/// </summary>
		public override void Add()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Add);
		}

		/// <summary>
		/// Pops two values from the stack, subtracts the second from the first, then pushes the
		/// result to the stack.
		/// </summary>
		public override void Subtract()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Sub);
		}

		/// <summary>
		/// Pops two values from the stack, multiplies them together, then pushes the
		/// result to the stack.
		/// </summary>
		public override void Multiply()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Mul);
		}

		/// <summary>
		/// Pops two values from the stack, divides the first by the second, then pushes the
		/// result to the stack.
		/// </summary>
		public override void Divide()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Div);
		}

		/// <summary>
		/// Pops two values from the stack, divides the first by the second, then pushes the
		/// result to the stack.
		/// </summary>
		public override void DivideUnsigned()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Div_Un);
		}

		/// <summary>
		/// Pops two values from the stack, divides the first by the second, then pushes the
		/// remainder to the stack.
		/// </summary>
		public override void Remainder()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Rem);
		}
		
		/// <summary>
		/// Pops two values from the stack, divides the first (unsigned) by the second, then pushes the
		/// remainder to the stack.
		/// </summary>
		public override void RemainderUnsigned()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Rem_Un);
		}

		/// <summary>
		/// Pops a value from the stack, negates it, then pushes it back onto the stack.
		/// </summary>
		public override void Negate()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Neg);
		}

		/// <summary>
		/// Pops two values from the stack, ANDs them together, then pushes the result to the
		/// stack.
		/// </summary>
		public override void BitwiseAnd()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.And);
		}

		/// <summary>
		/// Pops two values from the stack, ORs them together, then pushes the result to the
		/// stack.
		/// </summary>
		public override void BitwiseOr()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Or);
		}

		/// <summary>
		/// Pops two values from the stack, XORs them together, then pushes the result to the
		/// stack.
		/// </summary>
		public override void BitwiseXor()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Xor);
		}

		/// <summary>
		/// Pops a value from the stack, inverts it, then pushes the result to the stack.
		/// </summary>
		public override void BitwiseNot()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Not);
		}

		/// <summary>
		/// Pops two values from the stack, shifts the first to the left, then pushes the result
		/// to the stack.
		/// </summary>
		public override void ShiftLeft()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Shl);
		}

		/// <summary>
		/// Pops two values from the stack, shifts the first to the right, then pushes the result
		/// to the stack.  The sign bit is preserved.
		/// </summary>
		public override void ShiftRight()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Shr);
		}

		/// <summary>
		/// Pops two values from the stack, shifts the first to the right, then pushes the result
		/// to the stack.  The sign bit is not preserved.
		/// </summary>
		public override void ShiftRightUnsigned()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Shr_Un);
		}



		//	 CONVERSIONS
		//_________________________________________________________________________________________

		/// <summary>
		/// Pops a value from the stack, converts it to an object reference, then pushes it back onto
		/// the stack.
		/// </summary>
		public override void Box(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.IsValueType == false)
				throw new ArgumentException("The type to box must be a value type.", "type");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Box, type);
		}

		/// <summary>
		/// Pops an object reference (representing a boxed value) from the stack, extracts the
		/// address, then pushes that address onto the stack.
		/// </summary>
		/// <param name="type"> The type of the boxed value.  This should be a value type. </param>
		public override void Unbox(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.IsValueType == false)
				throw new ArgumentException("The type of the boxed value must be a value type.", "type");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Unbox, type);
		}

		/// <summary>
		/// Pops an object reference (representing a boxed value) from the stack, extracts the value,
		/// then pushes the value onto the stack.
		/// </summary>
		/// <param name="type"> The type of the boxed value.  This should be a value type. </param>
		public override void UnboxAny(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			if (type.IsValueType == false)
				throw new ArgumentException("The type of the boxed value must be a value type.", "type");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Unbox_Any, type);
		}

		/// <summary>
		/// Pops a value from the stack, converts it to a signed byte, then pushes it back onto
		/// the stack.
		/// </summary>
		public override void ConvertToInt8()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_I1);
		}

		/// <summary>
		/// Pops a value from the stack, converts it to an unsigned byte, then pushes it back
		/// onto the stack.
		/// </summary>
		public override void ConvertToUnsignedInt8()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_U1);
		}
		
		/// <summary>
		/// Pops a value from the stack, converts it to a signed short, then pushes it back onto
		/// the stack.
		/// </summary>
		public override void ConvertToInt16()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_I2);
		}

		/// <summary>
		/// Pops a value from the stack, converts it to an unsigned short, then pushes it back
		/// onto the stack.
		/// </summary>
		public override void ConvertToUnsignedInt16()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_U2);
		}
		
		/// <summary>
		/// Pops a value from the stack, converts it to a signed integer, then pushes it back onto
		/// the stack.
		/// </summary>
		public override void ConvertToInt32()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_I4);
		}

		/// <summary>
		/// Pops a value from the stack, converts it to an unsigned integer, then pushes it back
		/// onto the stack.
		/// </summary>
		public override void ConvertToUnsignedInt32()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_U4);
		}

		/// <summary>
		/// Pops a value from the stack, converts it to a signed 64-bit integer, then pushes it
		/// back onto the stack.
		/// </summary>
		public override void ConvertToInt64()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_I8);
		}

		/// <summary>
		/// Pops a value from the stack, converts it to an unsigned 64-bit integer, then pushes it
		/// back onto the stack.
		/// </summary>
		public override void ConvertToUnsignedInt64()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_U8);
		}

		/// <summary>
		/// Pops a value from the stack, converts it to a float, then pushes it back onto
		/// the stack.
		/// </summary>
		public override void ConvertToSingle()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_R4);
		}
		
		/// <summary>
		/// Pops a value from the stack, converts it to a double, then pushes it back onto
		/// the stack.
		/// </summary>
		public override void ConvertToDouble()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_R8);
		}
		
		/// <summary>
		/// Pops an unsigned integer from the stack, converts it to a double, then pushes it back onto
		/// the stack.
		/// </summary>
		public override void ConvertUnsignedToDouble()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Conv_R_Un);
		}

		// POINTERS AND OTHER UNSAFE
		//_________________________________________________________________________________________
		
		/// <summary>
		/// Declares the following address load to be unaligned.
		/// </summary>
		public override void Unaligned(byte align){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Unaligned,align);
		}
		
		/// <summary>
		/// Reads an int8 from an address on the stack.
		/// </summary>
		public override void AddressInt8(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I1);
		}
		
		/// <summary>
		/// Reads an int16 from an address on the stack.
		/// </summary>
		public override void AddressInt16(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I2);
		}
		
		/// <summary>
		/// Reads an int32 from an address on the stack.
		/// </summary>
		public override void AddressInt32(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I4);
		}
		
		/// <summary>
		/// Reads an int64 from an address on the stack.
		/// </summary>
		public override void AddressInt64(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I8);
		}
		
		/// <summary>
		/// Reads an unsigned int8 from an address on the stack.
		/// </summary>
		public override void AddressUnsignedInt8(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I1);
		}
		
		/// <summary>
		/// Reads an unsigned int16 from an address on the stack.
		/// </summary>
		public override void AddressUnsignedInt16(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I2);
		}
		
		/// <summary>
		/// Reads an unsigned int32 from an address on the stack.
		/// </summary>
		public override void AddressUnsignedInt32(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I4);
		}
		
		/// <summary>
		/// Reads an unsigned int64 from an address on the stack.
		/// </summary>
		public override void AddressUnsignedInt64(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_I8);
		}
		
		/// <summary>
		/// Reads a float from an address on the stack.
		/// </summary>
		public override void AddressSingle(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_R4);
		}
		
		/// <summary>
		/// Reads a double from an address on the stack.
		/// </summary>
		public override void AddressDouble(){
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldind_R8);
		}
		
		
		//	 OBJECTS, METHODS, TYPES AND FIELDS
		//_________________________________________________________________________________________
		
		/// <summary>
		/// Pops the constructor arguments off the stack and creates a new instance of the object.
		/// </summary>
		/// <param name="constructor"> The constructor that is used to initialize the object. </param>
		public override void NewObject(System.Reflection.ConstructorInfo constructor)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Newobj, constructor);
		}

		/// <summary>
		/// Performs an indirect call.
		/// </summary>
		public override void CallIndirect(Type returnType,Type[] parameterTypes){
			this.generator.EmitCalli(
				System.Reflection.Emit.OpCodes.Calli,
				CallingConventions.Standard,
				returnType,
				parameterTypes,
				null
			);
		}
		
		/// <summary>
		/// Pops the method arguments off the stack, calls the given method, then pushes the result
		/// to the stack (if there was one).  This operation can be used to call instance methods,
		/// but virtual overrides will not be called and a null check will not be performed at the
		/// callsite.
		/// </summary>
		/// <param name="method"> The method to call. </param>
		public override void CallStatic(System.Reflection.MethodBase method)
		{
			if (method is System.Reflection.ConstructorInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Call, (System.Reflection.ConstructorInfo)method);
			else if (method is System.Reflection.MethodInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Call, (System.Reflection.MethodInfo)method);
			else
				throw new InvalidOperationException("Unsupported subtype of MethodBase.");
		}

		/// <summary>
		/// Pops the method arguments off the stack, calls the given method, then pushes the result
		/// to the stack (if there was one).  This operation cannot be used to call static methods.
		/// Virtual overrides are obeyed and a null check is performed.
		/// </summary>
		/// <param name="method"> The method to call. </param>
		/// <exception cref="ArgumentException"> The method is static. </exception>
		public override void CallVirtual(System.Reflection.MethodBase method)
		{
			if (method == null)
				throw new ArgumentNullException("method");
			if (method.IsStatic == true)
				throw new ArgumentException("Static methods cannot be called this method.", "method");
			if (method is System.Reflection.ConstructorInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Callvirt, (System.Reflection.ConstructorInfo)method);
			else if (method is System.Reflection.MethodInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Callvirt, (System.Reflection.MethodInfo)method);
			else
				throw new InvalidOperationException("Unsupported subtype of MethodBase.");
		}

		/// <summary>
		/// Pushes the value of the given field onto the stack.
		/// </summary>
		/// <param name="field"> The field whose value will be pushed. </param>
		public override void LoadField(System.Reflection.FieldInfo field)
		{
			if (field == null)
				throw new ArgumentNullException("field");
			if (field.IsStatic == true)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldsfld, field);
			else
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldfld, field);
		}

		/// <summary>
		/// Pops a value off the stack and stores it in the given field.
		/// </summary>
		/// <param name="field"> The field to modify. </param>
		public override void StoreField(System.Reflection.FieldInfo field)
		{
			if (field == null)
				throw new ArgumentNullException("field");
			if (field.IsStatic == true)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Stsfld, field);
			else
				this.generator.Emit(System.Reflection.Emit.OpCodes.Stfld, field);
		}

		/// <summary>
		/// Pops an object off the stack, checks that the object inherits from or implements the
		/// given type, and pushes the object onto the stack if the check was successful or
		/// throws an InvalidCastException if the check failed.
		/// </summary>
		/// <param name="type"> The type of the class the object inherits from or the interface the
		/// object implements. </param>
		public override void CastClass(Type type)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Castclass, type);
		}

		/// <summary>
		/// Pops an object off the stack, checks that the object inherits from or implements the
		/// given type, and pushes either the object (if the check was successful) or <c>null</c>
		/// (if the check failed) onto the stack.
		/// </summary>
		/// <param name="type"> The type of the class the object inherits from or the interface the
		/// object implements. </param>
		public override void IsInstance(Type type)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Isinst, type);
		}

		/// <summary>
		/// Pushes a RuntimeTypeHandle corresponding to the given type onto the evaluation stack.
		/// </summary>
		/// <param name="type"> The type to convert to a RuntimeTypeHandle. </param>
		public override void LoadToken(Type type)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldtoken, type);
		}

		/// <summary>
		/// Pushes a RuntimeMethodHandle corresponding to the given method onto the evaluation
		/// stack.
		/// </summary>
		/// <param name="method"> The method to convert to a RuntimeMethodHandle. </param>
		public override void LoadToken(System.Reflection.MethodBase method)
		{
			if (method is System.Reflection.ConstructorInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldtoken, (System.Reflection.ConstructorInfo)method);
			else if (method is System.Reflection.MethodInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldtoken, (System.Reflection.MethodInfo)method);
			else
				throw new InvalidOperationException("Unsupported subtype of MethodBase.");
		}

		/// <summary>
		/// Pushes a RuntimeFieldHandle corresponding to the given field onto the evaluation stack.
		/// </summary>
		/// <param name="field"> The type to convert to a RuntimeFieldHandle. </param>
		public override void LoadToken(System.Reflection.FieldInfo field)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldtoken, field);
		}

		/// <summary>
		/// Pushes a pointer to the native code implementing the given method onto the evaluation
		/// stack.  The virtual qualifier will be ignored, if present.
		/// </summary>
		/// <param name="method"> The method to retrieve a pointer for. </param>
		public override void LoadStaticMethodPointer(System.Reflection.MethodBase method)
		{
			if (method is System.Reflection.ConstructorInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldftn, (System.Reflection.ConstructorInfo)method);
			else if (method is System.Reflection.MethodInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldftn, (System.Reflection.MethodInfo)method);
			else
				throw new InvalidOperationException("Unsupported subtype of MethodBase.");
		}

		/// <summary>
		/// Pushes a pointer to the native code implementing the given method onto the evaluation
		/// stack.  This method cannot be used to retrieve a pointer to a static method.
		/// </summary>
		/// <param name="method"> The method to retrieve a pointer for. </param>
		/// <exception cref="ArgumentException"> The method is static. </exception>
		public override void LoadVirtualMethodPointer(System.Reflection.MethodBase method)
		{
			if (method != null && method.IsStatic == true)
				throw new ArgumentException("The given method cannot be static.", "method");
			if (method is System.Reflection.ConstructorInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldvirtftn, (System.Reflection.ConstructorInfo)method);
			else if (method is System.Reflection.MethodInfo)
				this.generator.Emit(System.Reflection.Emit.OpCodes.Ldvirtftn, (System.Reflection.MethodInfo)method);
			else
				throw new InvalidOperationException("Unsupported subtype of MethodBase.");
		}

		/// <summary>
		/// Pops a managed or native pointer off the stack and initializes the referenced type with
		/// zeros.
		/// </summary>
		/// <param name="type"> The type the pointer on the top of the stack is pointing to. </param>
		public override void InitObject(Type type)
		{
			if (type == null)
				throw new ArgumentNullException("type");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Initobj, type);
		}



		//	 ARRAYS
		//_________________________________________________________________________________________

		/// <summary>
		/// Pops the size of the array off the stack and pushes a new array of the given type onto
		/// the stack.
		/// </summary>
		/// <param name="type"> The element type. </param>
		public override void NewArray(Type type)
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Newarr, type);
		}

		/// <summary>
		/// Pops the array and index off the stack and pushes the element value onto the stack.
		/// </summary>
		/// <param name="type"> The element type. </param>
		public override void LoadArrayElement(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem_I1);
					break;
				case TypeCode.UInt16:
				case TypeCode.Int16:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem_I2);
					break;
				case TypeCode.UInt32:
				case TypeCode.Int32:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem_I4);
					break;
				case TypeCode.UInt64:
				case TypeCode.Int64:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem_I8);
					break;
				case TypeCode.Single:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem_R4);
					break;
				case TypeCode.Double:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem_R8);
					break;
				default:
					if (type.IsClass == true)
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem_Ref);
					else
						this.generator.Emit(System.Reflection.Emit.OpCodes.Ldelem, type);
					break;
			}
		}

		/// <summary>
		/// Pops the array, index and value off the stack and stores the value in the array.
		/// </summary>
		/// <param name="type"> The element type. </param>
		public override void StoreArrayElement(Type type)
		{
			switch (Type.GetTypeCode(type))
			{
				case TypeCode.Byte:
				case TypeCode.SByte:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem_I1);
					break;
				case TypeCode.UInt16:
				case TypeCode.Int16:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem_I2);
					break;
				case TypeCode.UInt32:
				case TypeCode.Int32:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem_I4);
					break;
				case TypeCode.UInt64:
				case TypeCode.Int64:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem_I8);
					break;
				case TypeCode.Single:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem_R4);
					break;
				case TypeCode.Double:
					this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem_R8);
					break;
				default:
					if (type.IsClass == true)
						this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem_Ref);
					else
						this.generator.Emit(System.Reflection.Emit.OpCodes.Stelem, type);
					break;
			}
		}

		/// <summary>
		/// Pops an array off the stack and pushes the length of the array onto the stack.
		/// </summary>
		public override void LoadArrayLength()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Ldlen);
		}



		//	 EXCEPTION HANDLING
		//_________________________________________________________________________________________

		/// <summary>
		/// Pops an exception object off the stack and throws the exception.
		/// </summary>
		public override void Throw()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Throw);
		}

		/// <summary>
		/// Begins a try-catch-finally block.  After issuing this instruction any following
		/// instructions are conceptually within the try block.
		/// </summary>
		public override void BeginExceptionBlock()
		{
			this.generator.BeginExceptionBlock();
		}

		/// <summary>
		/// Ends a try-catch-finally block.  BeginExceptionBlock() must have already been called.
		/// </summary>
		public override void EndExceptionBlock()
		{
			this.generator.EndExceptionBlock();
		}

		/// <summary>
		/// Begins a catch block.  BeginExceptionBlock() must have already been called.
		/// </summary>
		/// <param name="exceptionType"> The type of exception to handle. </param>
		public override void BeginCatchBlock(Type exceptionType)
		{
			this.generator.BeginCatchBlock(exceptionType);
		}

		/// <summary>
		/// Begins a finally block.  BeginExceptionBlock() must have already been called.
		/// </summary>
		public override void BeginFinallyBlock()
		{
			this.generator.BeginFinallyBlock();
		}

		/// <summary>
		/// Begins a filter block.  BeginExceptionBlock() must have already been called.
		/// </summary>
		public override void BeginFilterBlock()
		{
			this.generator.BeginExceptFilterBlock();
		}

		/// <summary>
		/// Begins a fault block.  BeginExceptionBlock() must have already been called.
		/// </summary>
		public override void BeginFaultBlock()
		{
			this.generator.BeginFaultBlock();
		}

		/// <summary>
		/// Unconditionally branches to the given label.  Unlike the regular branch instruction,
		/// this instruction can exit out of try, filter and catch blocks.
		/// </summary>
		/// <param name="label"> The label to branch to. </param>
		public override void Leave(ILLabel label)
		{
			if (label as ReflectionEmitILLabel == null)
				throw new ArgumentNullException("label");
			this.generator.Emit(System.Reflection.Emit.OpCodes.Leave, ((ReflectionEmitILLabel)label).UnderlyingLabel);
		}

		/// <summary>
		/// This instruction can be used from within a finally block to resume the exception
		/// handling process.  It is the only valid way of leaving a finally block.
		/// </summary>
		public override void EndFinally()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Endfinally);
		}

		/// <summary>
		/// This instruction can be used from within a filter block to indicate whether the
		/// exception will be handled.  It pops an integer from the stack which should be <c>0</c>
		/// to continue searching for an exception handler or <c>1</c> to use the handler
		/// associated with the filter.  EndFilter() must be called at the end of a filter block.
		/// </summary>
		public override void EndFilter()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Endfilter);
		}



		//	 DEBUGGING SUPPORT
		//_________________________________________________________________________________________

		/// <summary>
		/// Triggers a breakpoint in an attached debugger.
		/// </summary>
		public override void Breakpoint()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Break);
		}
		
		//	 MISC
		//_________________________________________________________________________________________

		/// <summary>
		/// Does nothing.
		/// </summary>
		public override void NoOperation()
		{
			this.generator.Emit(System.Reflection.Emit.OpCodes.Nop);
		}
	}
	
}

#endif

namespace WebAssembly{
	
	public partial class Runtime{
		
		/// <summary>
		/// Gets or sets information needed to generate the module.
		/// </summary>
		internal ILModuleInfo ModuleInfo{
			get{
				if (moduleGenerationInfo == null)
				{
					#if AbleToCompile
					moduleGenerationInfo = new ReflectionEmitModuleInfo(this);
					#else
					moduleGenerationInfo = null;
					throw new Exception("Can't compile on this platform.");
					#endif
				}
				return moduleGenerationInfo;
			}
		}
		
		/// <summary>
		/// Gets or sets information needed to generate the module.
		/// </summary>
		protected ILModuleInfo moduleGenerationInfo;
		
	}
	
	public partial class Module{
		
		/// <summary>The memory to use.</summary>
		public Memory Memory;
		/// <summary>The compiled assembly.</summary>
		private Assembly Compiled;
		/// <summary>True if it's already been started.</summary>
		private bool Started;
		
		/// <summary>Sets up the given assembly.</summary>
		protected override void SetupScopes(Assembly asm){
			
			// GlobalCache class stores values which 
			// are important to the JS engine itself:
			Type cache = asm.GetType("GlobalCache");
			
			if(cache!=null){
				
				// Apply the values:
				cache.GetField("Runtime").SetValue(null,this);
				
			}
			
		}
		
		/// <summary>Call to start this module now.</summary>
		public bool Start(){
			
			if(Compiled==null){
				throw new Exception("Module has not been compiled!");
			}
			
			if(Started){
				return true;
			}
			
			if(Memory==null){
				
				// Get the mem section:
				var ms = MemorySection;
				
				if(ms!=null){
					// Create the memory block now (MVP has one entry in MemSection):
					Memory = new Memory((int)ms.Entries[0].Limits.Initial);
				}
				
			}
			
			// Apply mem to the imported set:
			Imports.Memory = Memory;
			
			// Update global FieldInfo and func MethodInfo's now:
			CollectMethods();
			
			// Note: invoking the main method makes the static constructor run too.
			// That will make it collect the memory object as a *readonly value*.
			// As it's a static readonly, any platforms running the JIT will get awesome
			// performance bonuses on every memory access because the JIT will remove the address add.
			// This works because it's relying on C#'s lazy static loading (spec, 17.11)
			// which is implemented by IL2CPP too.
			CompiledCode cc = GetMain(Compiled);
			
			if(cc!=null){
				Started=true;
				
				// Run now (to run the static CC):
				cc.Execute();
			}
			
			return Started;
		}
		
		/// <summary>Compiles this WebAssembly module into a .NET DLL.</summary>
		public void Compile(){
			Compile(null);
		}
		
		/// <summary>Defines a static global field.</summary>
		public FieldInfo DefineField(string name,Type fieldType){
			
			#if AbleToCompile
			// Attributes:
			System.Reflection.FieldAttributes attribs=
				System.Reflection.FieldAttributes.Public | 
				System.Reflection.FieldAttributes.Static;
			
			return ModuleInfo.MainBuilder.DefineField(name,fieldType,attribs);
			#else
			throw new Exception("Can't define a field on this platform.");
			#endif
			
		}
		
		/// <summary>Starts compiling a WebAssembly method.</summary>
		public ILGenerator DefineMethod(string name, Type returnType, Type[] parameters){
			
			#if AbleToCompile
			// Attributes:
			System.Reflection.MethodAttributes attribs=
				System.Reflection.MethodAttributes.HideBySig | 
				System.Reflection.MethodAttributes.Public | 
				System.Reflection.MethodAttributes.Static;
			
			return ModuleInfo.MainBuilder.DefineMethod(name,attribs,returnType,parameters);
			#else
			throw new Exception("Can't define a method on this platform.");
			#endif
			
		}
		
		/// <summary>Compiles this WebAssembly module into a .NET DLL, optionally caching it.
		/// Shares generation with the main Nitrassic engine.</summary>
		public void Compile(string uniqueID){
			
			if(Compiled!=null){
				// Already ready!
				return;
			}
			
			// Try loading it first:
			Compiled = LoadAssembly(uniqueID);
			
			if(Compiled!=null){
				return;
			}
			
			#if !AbleToCompile
			throw new Exception("Can't compile WebAssembly on this platform!");
			#else
			
			// Create the main type - it contains the entry point and a memory reference:
			ModuleInfo.CreateMainType(uniqueID);
			
			// Setup the global cache immediately (which makes both the ScriptEngine and Wasm module available):
			ModuleInfo.SetupGlobalCache(null);
			
			// Add the memory reference next - it's a static readonly IntPtr.
			// The actual value comes from the WebAssembly.Imports.Memory field.
			ModuleInfo.Memory = ModuleInfo.MainBuilder.DefineField(
				"MEM",
				typeof(void).MakePointerType(),
				FieldAttributes.Static | FieldAttributes.Public | FieldAttributes.InitOnly
			);
			
			// Start defining a constructor:
			var gen = ModuleInfo.MainBuilder.DefineConstructor(MethodAttributes.Static,new Type[0]);
			
			var ptrLocal = gen.DeclareVariable(typeof(IntPtr),"ptr");
			
			// Get its IntPtr:
			gen.Call(WebAssembly.ReflectionHelpers.Imports_GetMemory);
			
			// Store in a local and load the address:
			gen.StoreVariable(ptrLocal);
			
			// Load its address:
			gen.LoadAddressOfVariable(ptrLocal);
			
			// Convert to a native ptr:
			gen.Call(WebAssembly.ReflectionHelpers.IntPtr_NativePointer);
			
			// Store the void*:
			gen.StoreField(ModuleInfo.Memory);
			
			// Setup global init values:
			if(GlobalSection!=null){
				GlobalSection.Compile(gen);
			}
			
			// Done!
			gen.Complete("cctor",null,typeof(void),true);
			
			// For each method, compile it now:
			if(CodeSection!=null){
				
				// Compile it now:
				CodeSection.Compile();
				
			}
			
			if(StartSection==null){
				// Empty start section:
				Add(new StartSection(true));
			}
			
			// Compile main:
			StartSection.Compile();
			
			// Build the types now:
			Compiled = ModuleInfo.Close().Assembly;
			
			#endif
			
		}
		
	}
	
}
