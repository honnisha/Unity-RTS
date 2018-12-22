using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Blaze;
using Css;


namespace Svg{
	
	/// <summary>
	/// Defines a path that can be used by other elements.
	/// </summary>
	[Dom.TagName("clippath")]
	public class SVGClipPathElement:SVGElement{
		
		/// <summary>Specifies the coordinate system for the clipping path.</summary>
		public CoordinateUnits ClipPathUnits=CoordinateUnits.UserSpaceOnUse;
		
		/// <summary>The computed clip path.</summary>
		private VectorPath _computedPath;
		
		/// <summary>
		/// Gets the clip path.
		/// </summary>
		public override VectorPath GetPath(SVGElement context,RenderContext renderer){
			
			if(_computedPath==null){
				
				Matrix4x4 transform;
				bool applyExtra;
				
				if (ClipPathUnits == CoordinateUnits.ObjectBoundingBox){
					
					BoxRegion bounds = context.Bounds;
					
					transform = Matrix4x4.TRS(Vector3.zero,Quaternion.identity,new Vector3(
						bounds.Width, bounds.Height, 0f
					));
					
					transform*=Matrix4x4.TRS(new Vector3(
						bounds.X, bounds.Y, 0f
					),Quaternion.identity,Vector3.one);
					
					applyExtra=true;
					
				}else{
					
					transform=Matrix4x4.identity;
					applyExtra=false;
					
				}
				
				// For each child which is a PathBase, append it.
				_computedPath=new VectorPath();
				
				AddChildPaths(this,_computedPath,renderer,transform,applyExtra);
				
			}
			
			return _computedPath;
			
		}
		
		/// <summary>
		/// Renders the <see cref="SVGElement"/> and contents to the specified <see cref="ISvgRenderer"/> object.
		/// </summary>
		/// <param name="renderer">The <see cref="ISvgRenderer"/> object to render to.</param>
		public override void BuildFilter(RenderContext ctx){
			
			// Do nothing
			
		}
		
	}
	
}
