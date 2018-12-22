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


namespace Css.Properties{
	
	/// <summary>
	/// The internal -spark-writing-system: css property. (Don't set this! Use text-orientation instead).
	/// Stores a computed writing direction map which is used when applying logical CSS properties.
	/// </summary>
	
	public class SparkWritingSystem:CssProperty{
		
		public static SparkWritingSystem GlobalProperty;
		/// <summary>The available index mappings. See LoadIndex for more information.</summary>
		public static int[][] Index;
		
		/// <summary>Internal property.</summary>
		public override bool Internal{
			get{
				return true;
			}
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		/// <summary>There's 6 possible combinations of the writing order when using direction and writing-mode.
		/// We need to re-map e.g. marging-inline-start to the correct physical margin. This is where the index
		/// comes in. It deals with the 6 combinations by mapping them through to the physical margin.</summary>
		private static void LoadIndex(){
			
			// The 6 combinations originate from this:
			
			/*
			A=X, B=Y
			- Do nothing
			- Flip A
			- Flip B [Useless; Nobody writes upwards]
			- Flip both [Useless; Nobody writes upwards]
			
			A=Y, B=X
			- Do nothing
			- Flip A
			- Flip B
			- Flip both
			*/
			
			// It's things like this that make Spark go way faster than Webkit :)
			
			Index=new int[][]{
				
				new int[]{0,1,2,3}, // top, right, bottom, left (Our "identity" - a mapping that does nothing at all)
				new int[]{0,3,2,1}, // top, left, bottom, right
				
				new int[]{1,0,3,2}, // right, top, left, bottom
				new int[]{1,2,3,0}, // right, bottom, left, top
				new int[]{3,0,1,2}, // left, top, right, bottom
				new int[]{3,2,1,0}, // left, bottom, right, top
				
			};
			
		}
		
		public SparkWritingSystem(){
			
			GlobalProperty=this;
			
		}
		
		public override string[] GetProperties(){
			return new string[]{"-spark-writing-system"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>Requires a writing system mapping for the given computed style.</summary>
		public int[] RequireMap(Css.ComputedStyle style){
			
			// Get the map value:
			Css.Value v;
			style.Properties.TryGetValue(GlobalProperty,out v);
			
			if(v==null){
				
				// Get it now:
				int[] mapValue=GetMap(style);
				
				// Cache it:
				v=new Css.Units.WritingSystemMap(mapValue);
				style.Properties[GlobalProperty]=v;
				
				return mapValue;
			}
			
			// Get from the cached value:
			return (v as Css.Units.WritingSystemMap).Map;
			
		}
		
		/// <summary>Gets the most suitable mapping to use for the given computed style if one has already been set.</summary>
		public void UpdateMap(Css.ComputedStyle style){
			
			// Get the map value:
			Css.Value v;
			style.Properties.TryGetValue(GlobalProperty,out v);
			
			if(v!=null){
				// Update it:
				(v as Css.Units.WritingSystemMap).Map=GetMap(style);
			}
			
		}
		
		/// <summary>Gets the most suitable mapping to use for the given computed style.</summary>
		public int[] GetMap(Css.Style style){
			
			// 1. Pull orientation and writing mode:
			Css.Units.IntegerUnit orientationValue=style[Css.Properties.TextOrientation.GlobalProperty] as Css.Units.IntegerUnit;
			Css.Units.IntegerUnit writingModeValue=style[Css.Properties.WritingMode.GlobalProperty] as Css.Units.IntegerUnit;
			
			bool ltr=true;
			int writingMode=Css.WritingMode.HorizontalTB; // horizontal-tb 
			int orientation=0; // Mixed by default.
			
			// Got orient/ writing mode values?
			if(orientationValue!=null){
				orientation=(int)orientationValue.RawValue;
			}
			
			if(writingModeValue!=null){
				writingMode=(int)writingModeValue.RawValue;
			}
			
			// Set to upright?
			if(orientation!=TextOrientationMode.Upright){
				// Always forces LTR otherwise.
				
				Css.Value dirValue=style[Css.Properties.Direction.GlobalProperty];
				
				ltr=(dirValue==null || dirValue.Text=="ltr");
				
			}
			
			// Time to figure out what our mapping is going to be!
			// - We're all done with orientation already. It's just down to LTR and writingMode now.
			
			/*
			horizontal-tb	top,right,bottom,left (Index 0)
			+RTL			top,left,bottom,right (Index 1)

			vertical-rl		left,top,right,bottom (Index 4)
			+RTL			right,top,left,bottom (Index 2)

			vertical-lr		left,bottom,right,top (Index 5)
			+RTL			right,bottom,left,top (Index 3)

			sideways-rl		left,top,right,bottom (Index 4)
			+RTL			right,top,left,bottom (Index 2)

			sideways-lr		right,bottom,left,top (Index 3)
			+RTL			left,bottom,right,top (Index 5)
			*/
			
			int indexID;
			
			if(writingMode==Css.WritingMode.VerticalRL || writingMode==Css.WritingMode.SidewaysRL){
				
				indexID=ltr?4:2;
				
			}else if(writingMode==Css.WritingMode.SidewaysLR){
				
				indexID=ltr?3:5;
				
			}else if(writingMode==Css.WritingMode.VerticalLR){
				
				indexID=ltr?5:3;
				
			}else{ // Horizontal.
				
				indexID=ltr?0:1;
				
			}
			
			if(Index==null){
				// Index isn't ready yet - load now:
				LoadIndex();
			}
			
			// Get the index:
			return Index[indexID];
			
		}
		
	}
	
}

namespace Css.Units{
	
	/// <summary>
	/// Used internally by the writing system to cache the mapping from 
	/// e.g. margin-inline-end through to the physical margin.
	/// </summary>
	internal class WritingSystemMap:Css.Value{
		
		/// <summary>The mapping value.</summary>
		internal int[] Map;
		
		internal WritingSystemMap(int[] map){
			Map=map;
		}
		
	}
	
}



