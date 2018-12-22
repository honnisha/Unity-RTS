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

/// <summary>
/// The render mode tells PowerUI how to render, either with an atlas or without one.
/// Depending on your platform, one or the other may be more suitable. No atlas uses less memory, but is slower to render.
/// </summary>

namespace Css{

	public enum RenderMode{
		
		/// <summary>Default. Textures are placed on an atlas. Fast rendering but higher memory usage.</summary>
		Atlas,
		/// <summary>Textures are rendered as they come. Slower rendering time but minimal memory usage.</summary>
		NoAtlas
		
	}

}