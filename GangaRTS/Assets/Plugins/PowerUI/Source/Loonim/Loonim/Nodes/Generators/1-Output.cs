//--------------------------------------
//	   Loonim Image Generator
//	Partly derived from LibNoise
//	See License.txt for more info
//
//	Copyright © 2013 Kulestar Ltd
//		  www.kulestar.com
//--------------------------------------

using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>
	/// Describes a series of fully packed textures. Usually 2 of them in a normal PBR setup.
	/// It can define the following typical channels plus any custom ones:
	/// - Albedo (Always Colour.RGB)
	/// - Alpha (Always Colour.A)
	/// - Metallicness (Typically Channels0.R) [Required by the metallicness workflow]
	/// - Height map (Typically Channels0.G) [Optional]
	/// - Occlusion map (Typically Channels0.B) [Optional]
	/// - Smoothness (Typically Channels0.A) [Optional]
	/// - Emission (Typically Channels1.RGB) [Optional]
	/// - Mask/Stencil (Typically Channels1.A) [Optional]
	/// - Specular (Typically Channels2.RGB) [Required by the specular workflow]
	/// - Subsurface colour (Typically Channels3.RGB) [Optional]
	/// Note that the channel allocations are defined in the surface's property set.
	/// </summary>
	
	public class Output : TextureNode{
		
		internal override int OutputDimensions{
			get{
				// Multiple 2D images.
				return 2;
			}
		}
		
		/// <summary>The colour (aka diffuse aka albedo) and any transparency. This one is required.</summary>
		public TextureNode Colour{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		/// <summary>The first group of PBR meta (Source 1). Channel assignments are described in properties.</summary>
		public TextureNode Channels0{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>The second group of PBR meta (Source 2). Channel assignments are described in properties.</summary>
		public TextureNode Channels1{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The third group of PBR meta (Source 3). Channel assignments are described in properties.</summary>
		public TextureNode Channels2{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		/// <summary>The fourth group of PBR meta (Source 4). Channel assignments are described in properties.</summary>
		public TextureNode Channels3{
			get{
				return Sources[4];
			}
			set{
				Sources[4]=value;
			}
		}
		
		public override int TypeID{
			get{
				return 1;
			}
		}
		
		public Output():base(5){}
		
	}
	
}
