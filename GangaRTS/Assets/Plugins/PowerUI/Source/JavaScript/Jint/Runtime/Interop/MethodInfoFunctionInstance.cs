using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Jint.Native;
using Jint.Native.Array;
using Jint.Native.Function;
using JavaScript;

namespace Jint.Runtime.Interop
{
    public sealed class MethodInfoFunctionInstance : FunctionInstance
    {
		private readonly MethodInfo[] _sourceMethods;
        private readonly MethodInfo[] _methods;
		private readonly MethodInfo _paramsMethod;

		
        public MethodInfoFunctionInstance(Engine engine, MethodInfo[] methods)
            : base(engine, null, null, false)
        {
            _sourceMethods = methods;
			_methods = methods;
			List<MethodInfo> jsSpecificMethods = null;
			
			foreach (var methodInfo in _sourceMethods)
            {
				// Find the params method (if there is one):
				if(_paramsMethod == null)
				{
					var parameters = methodInfo.GetParameters();
					if (parameters.Any(p => p.HasAttribute<ParamArrayAttribute>()))
					{
						_paramsMethod = methodInfo;
					}
				}
				
				// If any are explicitly tagged as the JS method to use, use that:
				#if NETFX_CORE
				var attribute = (JavaScriptAttribute)methodInfo.GetCustomAttribute(typeof(JavaScriptAttribute),false);
				#else
				var attribute = (JavaScriptAttribute)Attribute.GetCustomAttribute(methodInfo,typeof(JavaScriptAttribute),false);
				#endif
				
				if(attribute != null){
					if(jsSpecificMethods == null){
						jsSpecificMethods = new List<MethodInfo>();
					}
					jsSpecificMethods.Add(methodInfo);
				}
				
			}
			
			if(jsSpecificMethods != null){
				_methods = jsSpecificMethods.ToArray();
			}
			
            Prototype = engine.Function.PrototypeObject;
        }

        public override JsValue Call(JsValue thisObject, JsValue[] arguments)
        {
            return Invoke(thisObject, arguments);
        }

        private JsValue Invoke(JsValue thisObject, JsValue[] jsArguments)
        {
            JsValue[] arguments;
			if(_paramsMethod == null) {
				arguments = jsArguments;
			} else {
				arguments = ProcessParamsArrays(jsArguments);
			}
			
            var methods = TypeConverter.FindBestMatch(Engine, _methods, arguments).ToList();
            var converter = Engine.ClrTypeConverter;
			var parameters = new object[arguments.Length];
			
            foreach (var method in methods)
            {
                
                var argumentsMatch = true;

                for (var i = 0; i < arguments.Length; i++)
                {
                    var parameterType = method.GetParameters()[i].ParameterType;

                    if (parameterType == typeof(JsValue))
                    {
                        parameters[i] = arguments[i];
                    }
                    else if (parameterType == typeof(JsValue[]) && arguments[i].IsArray())
                    {
                        // Handle specific case of F(params JsValue[])

                        var arrayInstance = arguments[i].AsArray();
                        var len = TypeConverter.ToInt32(arrayInstance.Get("length"));
                        var result = new JsValue[len];
                        for (var k = 0; k < len; k++)
                        {
                            var pk = k.ToString();
                            result[k] = arrayInstance.HasProperty(pk)
                                ? arrayInstance.Get(pk)
                                : JsValue.Undefined;
                        }

                        parameters[i] = result;
                    }
                    else
                    {
                        if (!converter.TryConvert(arguments[i].ToObject(), parameterType, CultureInfo.InvariantCulture, out parameters[i]))
                        {
                            argumentsMatch = false;
                            break;
                        }

                        var lambdaExpression = parameters[i] as LambdaExpression;
                        if (lambdaExpression != null)
                        {
                            parameters[i] = lambdaExpression.Compile();
                        }
                    }
                }

                if (!argumentsMatch)
                {
                    continue;
                }

                // todo: cache method info
                try
                {
                    return JsValue.FromObject(Engine, method.Invoke(thisObject.ToObject(), parameters.ToArray()));
                }
                catch (Exception exception)
                {
					UnityEngine.Debug.Log("Conversion fail for method " + method.Name);
                    var meaningfulException = exception.InnerException ?? exception;
                    var handler = Engine.Options._ClrExceptionsHandler;

                    if (handler != null && handler(meaningfulException))
                    {
                        throw new JavaScriptException(Engine.Error, meaningfulException.Message);
                    }

                    throw meaningfulException;
                }
            }

            throw new JavaScriptException(Engine.TypeError, "No public methods with the specified arguments were found.");
        }
		
        /// <summary>
        /// Reduces a flat list of parameters to a params array
        /// </summary>
        private JsValue[] ProcessParamsArrays(JsValue[] jsArguments)
        {
			var parameters = _paramsMethod.GetParameters();
			
			var nonParamsArgumentsCount = parameters.Length - 1;
			if (jsArguments.Length < nonParamsArgumentsCount)
				return jsArguments;

			var newArgumentsCollection = jsArguments.Take(nonParamsArgumentsCount).ToList();
			var argsToTransform = jsArguments.Skip(nonParamsArgumentsCount).ToList();

			if (argsToTransform.Count == 1 && argsToTransform.FirstOrDefault().IsArray())
				return jsArguments;

			var jsArray = Engine.Array.Construct(Arguments.Empty);
			Engine.Array.PrototypeObject.Push(jsArray, argsToTransform.ToArray());

			newArgumentsCollection.Add(new JsValue(jsArray));
			return newArgumentsCollection.ToArray();
        }

    }
}
