using System;
using System.IO;
using System.Reflection;


namespace WebAssembly{
	
	/// <summary>A WebAssembly func_type.</summary>
	public class FuncType{
		
		/// <summary>The index (used as an ID).</summary>
		public int Index;
		/// <summary>The form of this function.</summary>
		public LanguageType Form;
		/// <summary>The types of the parameters.</summary>
		public LanguageType[] Parameters;
		/// <summary>Return values (always one or zero at the moment).</summary>
		public LanguageType[] ReturnValues;
		/// <summary>The compiling signature if it's available.</summary>
		internal MethodInfo Signature_;
		/// <summary>The generator for this methods signature.</summary>
		internal ILGenerator Generator;
		/// <summary>Parameters.Length or 0.</summary>
		public int ParameterCount;
		/// <summary>Cached return type.</summary>
		private Type ReturnType_;
		/// <summary>Cached parameters.</summary>
		private Type[] Parameters_;
		
		
		/// <summary>The methods returned params.</summary>
		public Type[] GetParameters(){
		
			if(Parameters_!=null){
				return Parameters_;
			}
			
			// Param types:
			Parameters_ = new Type[Parameters==null ? 0 : Parameters.Length];
			
			for(int i=0;i<Parameters_.Length;i++){
				
				// Get the param type:
				Parameters_[i] = LanguageTypes.ToType(Parameters[i]);
				
			}
			
			return Parameters_;
		}
		
		/// <summary>Gets the type of the parameter at the given index.</summary>
		public Type GetParameterType(int index){
			return GetParameters()[index];
		}
		
		/// <summary>True if there is a return type.</summary>
		public bool HasReturnType{
			get{
				return GetReturnType() != typeof(void);
			}
		}
		
		/// <summary>The methods return type.</summary>
		public Type GetReturnType(){
			
			if(ReturnType_!=null){
				return ReturnType_;
			}
			
			if(ReturnValues!=null && ReturnValues.Length>0){
				// Only one in the MVP.
				ReturnType_ = LanguageTypes.ToType(ReturnValues[0]);
			}else{
				ReturnType_ = typeof(void);
			}
			
			return ReturnType_;
		}
		
		/// <summary>Get the method builder (or creates it).</summary>
		internal ILGenerator GetGenerator(Module module){
			
			if(Generator==null){
				// Get sig:
				GetSignature(module);
			}
			
			return Generator;
		}
		
		/// <summary>Get the method signature (or creates it).</summary>
		public MethodInfo GetSignature(Module module){
			if(Signature_==null){
				Generator = module.DefineMethod("Func_"+Index,GetReturnType(),GetParameters());
				Signature_ = Generator.Method;
			}
			
			return Signature_;
		}
		
		/// <summary>Get the method signature (or creates it).</summary>
		public MethodInfo GetSignature(Module module,out int paramCount,out Type returnType){
			
			if(Signature_==null){
				
				returnType = GetReturnType();
				Type[] paramSet = GetParameters();
				paramCount = paramSet.Length;
				
				// Define it:
				Generator = module.DefineMethod("Func_"+Index,returnType,paramSet);
				Signature_ = Generator.Method;
				
			}else{
				
				returnType = ReturnType_;
				paramCount = Parameters_.Length;
				
			}
			
			return Signature_;
		}
		
		/// <summary>The name of the signature method.</summary>
		public string Name{
			get{
				return "Func_"+Index;
			}
		}
		
		public FuncType(){}
		
		public FuncType(Reader reader,int index){
			
			Index=index;
			
			// The form of this function:
			Form=reader.ValueType();
			
			// Parameters:
			int paramCount=(int)reader.VarUInt32();
			Parameters=reader.ValueTypes(paramCount);
			
			// Set count:
			ParameterCount=paramCount;
			
			// Returns:
			if(reader.VarUInt1()){
				
				// Just one:
				ReturnValues=reader.ValueTypes(1);
				
			}
			
		}
		
	}
	
}