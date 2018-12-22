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

using Dom;
using UnityEngine;
using Blaze;
using Css;


namespace Svg{
	
	/// <summary>
	/// Provides shapes to the CSS system. Primarily used by SVG.
	/// </summary>
	
	public class ShapeProvider{
		
		/// <summary>The cached path.</summary>
		protected VectorPath _Path;
		
		/// <summary>The cached path.</summary>
		public VectorPath Path{
			get{
				return _Path;
			}
		}
		
		/// <summary>Clears the cached path.</summary>
		public void ClearCache(){
			_Path=null;
		}
		
		/// <summary>Sets the cached path.</summary>
		public void SetPath(VectorPath path){
			_Path=path;
		}
		
		/// <summary>Gets or rebuilds the cached path.</summary>
		public virtual VectorPath GetPath(SVGElement context,RenderContext ctx){
			
			return _Path;
			
		}
		
		/// <summary>Gets the defined shape as a region. Allows optimisations for rectangles.</summary>
		public virtual ScreenRegion GetRegion(SVGElement context,RenderContext ctx){
			
			// Get the path:
			VectorPath path=GetPath(context,ctx);
			
			if(path==null || path.FirstPathNode==null){
				// None!
				return null;
			}
			
			// Create a shape region for it:
			return new PathScreenRegion(path);
			
		}
		
    }
	
}