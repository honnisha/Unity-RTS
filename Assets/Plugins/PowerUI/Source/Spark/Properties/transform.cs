//--------------------------------------
//               PowerUI
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the transform: css property.
	/// </summary>
	
	public class TransformProperty:CssProperty{
		
		public static TransformProperty GlobalProperty;
		
		public TransformProperty(){
			GlobalProperty=this;
		}
		
		
		public override string[] GetProperties(){
			return new string[]{"transform"};
		}
		
		/// <summary>Checks if the given transform is 3D. True if any of the functions within it are 3D.</summary>
		public static bool Is3D(Css.Value value){
			
			if(value==null){
				return false;
			}
			
			
			Css.Functions.Transformation f;
			
			if(value is CssFunction){
				// Only one function.
				f=(value as Css.Functions.Transformation);
				
				return (f!=null && f.Is3D);
			}
			
			// Either a set of functions, just one or e.g. 'none'.
			for(int i=0;i<value.Count;i++){
				
				// Get as transformation func:
				f=(value[i] as Css.Functions.Transformation);
				
				if(f!=null && f.Is3D){
					// Only takes one!
					return true;
				}
				
			}
			
			// Nope!
			return false;
			
		}
		
		/// <summary>Computes a Matrix4x4 from a valid transform value.</summary>
		public static Matrix4x4 Compute(Css.Value value,RenderableData renderData){
			
			Matrix4x4 matrix=Matrix4x4.identity;
			
			if(value==null){
				return matrix;
			}
			
			// For each function in value, combine into matrix.
			// We either have a set of [TransformFunction]'s or a transform function
			Css.Functions.Transformation transformation=value as Css.Functions.Transformation;
			
			if(transformation!=null){
				// Single value.
				matrix=transformation.CalculateMatrix(renderData);
			}else if(value.Type==ValueType.Set){
				
				// Set of transformation functions:
				ApplySet(value,renderData,ref matrix);
				
			}
			
			return matrix;
		}
		
		private static void ApplySet(Value value,RenderableData renderData,ref Matrix4x4 matrix){
			
			int count=value.Count;

			for(int i=0;i<count;i++){
				
				// Get the child value:
				Value childValue=value[i];
				
				// Get it as a transformation:
				Css.Functions.Transformation transformation=childValue as Css.Functions.Transformation;
				
				if(transformation!=null){
					
					// Apply to matrix:
					matrix*=transformation.CalculateMatrix(renderData);
					
				}else if(childValue.Type==ValueType.Set){
					
					// This occurs if they used an awkward combo of ' ' and ,.
					// No problems though - it's supported :)
					ApplySet(childValue,renderData,ref matrix);
					
				}
				
			}
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Sets up a transform if value is non-null:
			if(value==null || value.IsType(typeof(Css.Keywords.None))){
				
				// Unchanged - remove:
				style.Properties.Remove(GlobalProperty);
				
			}else{
				
				// Cache transform now:
				Css.Units.TransformValue existing=style[GlobalProperty] as Css.Units.TransformValue;
				
				if(existing==null){
					
					// Build and cache it now:
					existing=new Css.Units.TransformValue( style,value );
					
					// Cache it now:
					style[GlobalProperty]=existing;
					
				}
				
				// Apply matrix:
				existing.Changed=true;
				
			}
			
			style.RequestPaintAll();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}

namespace Css.Units{
	
	/// <summary>
	/// Stores a computed transformation matrix.
	/// </summary>
	
	public class TransformValue:Css.Value{
		
		public Css.Value Origin;
		public bool Changed;
		public Transformation Transform;
		
		
		public TransformValue(ComputedStyle style,Css.Value value){
			
			Transform=new Transformation();
			Origin=value;
			Specifity=value.Specifity;
			
			// Get origin and position:
			Css.Value origin=style[Css.Properties.TransformOrigin.GlobalProperty];
			Css.Value position=style[Css.Properties.TransformOriginPosition.GlobalProperty];
			
			// Set them:
			Transform.SetOriginOffset(origin);
			Transform.OriginPosition=Css.Properties.TransformOriginPosition.GetPosition(position);
			
		}
		
		/// <summary>Recalculates the matrix.</summary>
		public void Recalculate(ComputedStyle style){
			
			Changed=false;
			Transform.RawMatrix=Css.Properties.TransformProperty.Compute(Origin,style.RenderData);
			
		}
		
		public override string ToString(){
			return Origin.ToString();
		}
		
	}
	
}



