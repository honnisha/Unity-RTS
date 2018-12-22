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
using Dom;


namespace Css{
	
	/// <summary>Renderable nodes.</summary>
	public interface IRenderableNode{
		
		/// <summary>The box model for a node.</summary>
		RenderableData RenderData{get;}
		
		/// <summary>The computed style.</summary>
		ComputedStyle ComputedStyle{get;}
		
		/// <summary>Called whilst a CSS box is being recomputed.</summary>
		void OnComputeBox(Renderman renderer,LayoutBox box,ref bool widthUndefined,ref bool heightUndefined);
		
		/// <summary>Called when a font-face font loads.</summary>
		void FontLoaded(PowerUI.DynamicFont font);
		
		/// <summary>Called during a global render event on all elements.
		/// This knows exactly where the element is on screen.</summary>
		void OnRender(Renderman renderer);
		
		/// <summary>Called when an element is no longer on the screen.</summary>
		void WentOffScreen();
		
		#region CSS queries
		
		/// <summary>Gets the first element which matches the given selector.</summary>
		Element querySelector(string selector);
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		HTMLCollection querySelectorAll(string selector);
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		HTMLCollection querySelectorAll(string selector,bool one);
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selectors">The selectors to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		void querySelectorAll(Selector[] selectors,INodeList results,CssEvent e,bool one);
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.</summary>
		void GetWidthBounds(out float min,out float max);
		
		#endregion
		
	}
	
}