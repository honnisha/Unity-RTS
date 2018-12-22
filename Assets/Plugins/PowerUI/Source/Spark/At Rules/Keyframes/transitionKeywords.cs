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
using Blaze;


namespace Css.Keywords{
	
	/// <summary>
	/// Represents an instance of the ease keyword.
	/// </summary>
	
	public class Ease:CssKeyword{
		
		/// <summary>The cached path for this keyword.</summary>
		private static RasterVectorPath SharedPath_;
		
		/// <summary>The path for this keyword.</summary>
		public static RasterVectorPath SharedPath{
			get{
				
				// Need to build the path?
				if(SharedPath_==null){
					
					// Build it now:
					SharedPath_=new RasterVectorPath();
					
					// Add the curve:
					SharedPath_.CurveTo(
						0.25f,0.1f,
						0.25f,1f,
						1f,1f
					);
					
					SharedPath.ToStraightLines();
					
				}
				
				return SharedPath_;
				
			}
		}
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			return SharedPath;
			
		}
		
		public override string Name{
			get{
				return "ease";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the linear keyword.
	/// </summary>
	
	public class Linear:CssKeyword{
		
		/// <summary>The cached path for this keyword.</summary>
		private static RasterVectorPath SharedPath_;
		
		/// <summary>The path for this keyword.</summary>
		public static RasterVectorPath SharedPath{
			get{
				
				// Need to build the path?
				if(SharedPath_==null){
					
					// Build it now:
					SharedPath_=new RasterVectorPath();
					
					// Add the line:
					SharedPath_.LineTo(1f,1f);
					
				}
				
				return SharedPath_;
				
			}
		}
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			return SharedPath;
			
		}
		
		public override string Name{
			get{
				return "linear";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the ease-in keyword.
	/// </summary>
	
	public class EaseIn:CssKeyword{
		
		/// <summary>The cached path for this keyword.</summary>
		private static RasterVectorPath SharedPath_;
		
		/// <summary>The path for this keyword.</summary>
		public static RasterVectorPath SharedPath{
			get{
				
				// Need to build the path?
				if(SharedPath_==null){
					
					// Build it now:
					SharedPath_=new RasterVectorPath();
					
					// Add the curve:
					SharedPath_.CurveTo(
						0.42f,0f,
						1f,1f,
						1f,1f
					);
					
					SharedPath.ToStraightLines();
					
				}
				
				return SharedPath_;
				
			}
		}
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			return SharedPath;
			
		}
		
		public override string Name{
			get{
				return "ease-in";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the ease-out keyword.
	/// </summary>
	
	public class EaseOut:CssKeyword{
		
		/// <summary>The cached path for this keyword.</summary>
		private static RasterVectorPath SharedPath_;
		
		/// <summary>The path for this keyword.</summary>
		public static RasterVectorPath SharedPath{
			get{
				
				// Need to build the path?
				if(SharedPath_==null){
					
					// Build it now:
					SharedPath_=new RasterVectorPath();
					
					// Add the curve:
					SharedPath_.CurveTo(
						0f,0f,
						0.58f,1f,
						1f,1f
					);
					
					SharedPath.ToStraightLines();
					
				}
				
				return SharedPath_;
				
			}
		}
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			return SharedPath;
			
		}
		
		public override string Name{
			get{
				return "ease-out";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the ease-in-out keyword.
	/// </summary>
	
	public class EaseInOut:CssKeyword{
		
		/// <summary>The cached path for this keyword.</summary>
		private static RasterVectorPath SharedPath_;
		
		/// <summary>The path for this keyword.</summary>
		public static RasterVectorPath SharedPath{
			get{
				
				// Need to build the path?
				if(SharedPath_==null){
					
					// Build it now:
					SharedPath_=new RasterVectorPath();
					
					// Add the curve:
					SharedPath_.CurveTo(
						0.42f,0f,
						0.58f,1f,
						1f,1f
					);
					
					SharedPath.ToStraightLines();
					
				}
				
				return SharedPath_;
				
			}
		}
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			return SharedPath;
			
		}
		
		public override string Name{
			get{
				return "ease-in-out";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the step-start keyword.
	/// </summary>
	
	public class StepStart:CssKeyword{
		
		/// <summary>The cached path for this keyword.</summary>
		private static RasterVectorPath SharedPath_;
		
		/// <summary>The path for this keyword.</summary>
		public static RasterVectorPath SharedPath{
			get{
				
				// Need to build the path?
				if(SharedPath_==null){
					
					// Build it now:
					SharedPath_=new RasterVectorPath();
					
					// Move:
					SharedPath_.MoveTo(0f,1f);
					
					// Add the line:
					SharedPath_.LineTo(1f,1f);
					
				}
				
				return SharedPath_;
				
			}
		}
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			return SharedPath;
			
		}
		
		public override string Name{
			get{
				return "step-start";
			}
		}
		
	}
	
	/// <summary>
	/// Represents an instance of the step-end keyword.
	/// </summary>
	
	public class StepEnd:CssKeyword{
		
		/// <summary>The cached path for this keyword.</summary>
		private static RasterVectorPath SharedPath_;
		
		/// <summary>The path for this keyword.</summary>
		public static RasterVectorPath SharedPath{
			get{
				
				// Need to build the path?
				if(SharedPath_==null){
					
					// Build it now:
					SharedPath_=new RasterVectorPath();
					
					// Add the lines:
					SharedPath_.LineTo(1f,0f);
					SharedPath_.LineTo(1f,1f);
					
				}
				
				return SharedPath_;
				
			}
		}
		
		public override VectorPath GetPath(RenderableData context,CssProperty property){
			
			return SharedPath;
			
		}
		
		public override string Name{
			get{
				return "step-end";
			}
		}
		
	}
	
}