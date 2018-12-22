using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Percentile filter (such as median).
	/// </summary>
	
	public class Percentile: BitmapNode{
		
		private Color[] Buffer2;
		
		/// <summary>Horizontal radius.</summary>
		public TextureNode RadiusX{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>Vertical radius.</summary>
		public TextureNode RadiusY{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>Percentile.</summary>
		public TextureNode Percent{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		
		public Percentile():base(4){}
		
		public Percentile(TextureNode src,TextureNode rx,TextureNode ry,TextureNode prc):base(4){
			SourceModule=src;
			RadiusX=rx;
			RadiusY=ry;
			Percent=prc;
		}
		
		public override void Prepare(DrawInfo info){
			
			// Prepare inputs:
			base.Prepare(info);
			
			if(info.Mode==SurfaceDrawMode.CPU){
				
				// Prerender now! Bitmap node will sample it as needed.
				
				int width=info.ImageX;
				int height=info.ImageY;
				
				// Set buffer etc:
				Setup(info);
				
				// Setup secondary buffer:
				if(Buffer2==null || Buffer2.Length!=Buffer.Length){
					Buffer2=new Color[Buffer.Length];
				}
				
				Width=width;
				Height=height;
				
				// Render into buffer2:
				SourceModule.DrawCPU(info,Buffer2);
				
				// Get radii:
				int hRadius=(int)( width * RadiusX.GetValue(0.0,0.0) );
				int vRadius=(int)( height * RadiusY.GetValue(0.0,0.0) );
				
				// Do percentile filter now:
				PercentileFilter.Filter( Buffer2, Buffer, width, height, hRadius, vRadius,Percent);
				
			}
			
		}
		
		public override int TypeID{
			get{
				return 31;
			}
		}
		
	}
	
}