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


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the '-spark-tag-specifity' keyword.
	/// </summary>
	
	public class SparkTagSpecifity:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 1;
		}
		
		public override string Name{
			get{
				return "-spark-tag-specifity";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the '-spark-class-specifity' keyword.
	/// </summary>
	
	public class SparkClassSpecifity:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 1<<8;
		}
		
		public override string Name{
			get{
				return "-spark-class-specifity";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the '-spark-id-specifity' keyword.
	/// </summary>
	
	public class SparkIDSpecifity:CssKeyword{
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			return 1<<16;
		}
		
		public override string Name{
			get{
				return "-spark-id-specifity";
			}
		}
		
	}
	
}