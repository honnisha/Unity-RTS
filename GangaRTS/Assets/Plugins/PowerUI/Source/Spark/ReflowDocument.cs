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
using System.Collections;
using System.Collections.Generic;
using PowerUI;


namespace Css{
	
	/// <summary>Reflow capable documents.</summary>
	public partial class ReflowDocument : Document{
		
		/// <summary>The worldUI this document is in.</summary>
		public WorldUI worldUI;
		/// <summary>This documents viewport. Defaults to the info from ScreenInfo.</summary>
		public Viewport Viewport;
		/// <summary>Base zoom value. Applies whenever CSS zoom is 'auto'.</summary>
		public float Zoom=1f;
		/// <summary>The currently focused element.</summary>
		public Element activeElement;
		/// <summary>A set of all fonts available to this renderer, indexed by font name.</summary>
		public Dictionary<string,DynamicFont> ActiveFonts;
		/// <summary>The renderer which will render this document.</summary>
		public Renderman Renderer;
		/// <summary>Stylesheets for embedded namespaces.</summary>
		private Dictionary<MLNamespace,StyleSheet> EmbeddedNamespaces;
		
		
		
		public ReflowDocument(WorldUI world){
			
			worldUI=world;
			
			// Create the default viewport:
			
			if(worldUI!=null){
				Viewport=new Viewport(worldUI.pixelWidth,worldUI.pixelHeight);
			}else{
				Viewport=new Viewport(ScreenInfo.ScreenXFloat,ScreenInfo.ScreenYFloat);
			}
			
			ActiveFonts=new Dictionary<string,DynamicFont>();
			
		}
		
        /// <summary>
        /// Gives the values of all the CSS properties of an element after
        /// applying the active stylesheets and resolving any basic computation
        /// those values may contain.
        /// </summary>
        /// <param name="element">The element to compute the style for.</param>
        /// <returns>The style declaration describing the element.</returns>
        public Css.ComputedStyle getComputedStyle(HtmlElement element){
			if(element==null){
				return null;
			}
			return element.getComputedStyle(null);
		}
		
		/// <summary>True if an element in the document has focus.</summary>
		public bool hasFocus{
			get{
				return activeElement!=null;
			}
		}
		
		/// <summary>Checks if the stylesheet for the namespace the
		/// given namespace is in is included in the document.</summary>
		public void RequireStyleSheet(Element e){
		   
			if(e.Namespace==Namespace){
				// Same namespace as the document - already included.
				return;
			}
			
			if(EmbeddedNamespaces==null){
				EmbeddedNamespaces=new Dictionary<MLNamespace,StyleSheet>();
			}
			
			// Try and obtain it:
			if(EmbeddedNamespaces.ContainsKey(e.Namespace)){
				return;
			}
			
			// Get the sheet:
			StyleSheet sheet=e.Namespace.DefaultStyleSheet;
			
			// Add it in:
			EmbeddedNamespaces[e.Namespace]=sheet;
			sheet.ReAddSheet(this);
			
		}
		
		/// <summary>Gets the element at the given coordinates.
		/// It first checks if the point is on screen (within Viewport). If it is then
		/// a fast lookup occurs. Otherwise, all elements in the DOM are tested.</summary>
		public Element elementFromPoint(float x,float y){
			
			// Off screen?
			if(x<0 || y<0 || x>Viewport.Width || y>Viewport.Height){
				
				return elementFromPointOffScreen(x,y);
				
			}
			
			return elementFromPointOnScreen(x,y);
			
		} 
		
		/// <summary>Gets the element at the given coordinates when the point is known to be on screen.
		/// This is a lot faster than OffScreen, particularly if there are large 
		/// parts of hidden scrollable content (most web pages).</summary>
		internal Element elementFromPointOnScreen(float x,float y){
			
			// Cell coords:
			int cellX=(int)(x / Renderer.InputGrid.CellSize);
			int cellY=(int)(y / Renderer.InputGrid.CellSize);
			
			// Use the input grid for this!
			InputGridCell cell=Renderer.InputGrid[cellX,cellY];
			
			if(cell!=null){
				
				RenderableData renderData=cell.Get(x,y);
				
				if(renderData!=null){
					
					// Return its node:
					return renderData.Node as Element;
					
				}
				
			}
			
			// Return the root node (note that it can be null):
			return documentElement as Element;
			
		}
		
		/// <summary>Expected to almost never occur - this performs a full DOM scan in order to
		/// find the element at the given offscreen location.</summary>
		internal Element elementFromPointOffScreen(float x,float y){
			
			// Get the root node:
			Node node=documentElement;
			
			if(node==null){
				// Empty document.
				return null;
			}
			
			// We'll always return something - we don't need to check node's box (it must match the screen anyway).
			
			// Get kids ref:
			NodeList kids=node.childNodes_;
			
			while(kids!=null){
				
				// Loop backwards because the 'deepest' element is the one we want.
				// As soon as we find something that contains our point, we essentially stop.
				for(int i=kids.length-1;i>=0;i--){
					
					// Get the child node:
					Node child=kids[i];
					
					if(!(child is Element)){
						// Must be an element (unless it's a document).
						// Ignore text, doctype, comment etc.
						if(child is ReflowDocument){
							
							// Step into this though!
							child=(child as ReflowDocument).documentElement;
							
							if(child==null){
								// Not loaded yet
								continue;
							}
							
						}else{
							continue;
						}
						
					}
					
					// Unlike on screen we can't rely on the fast screen region
					// (which is only available for all on screen elements).
					// We'll need to always test the boxes.
					
					// Get the render data:
					RenderableData renderData=(child as IRenderableNode).RenderData;
					LayoutBox box=renderData.FirstBox;
					
					// Must be contained in one of them.
					bool notActuallyContained=true;
					
					while(box!=null){
						
						if(box.Contains(x,y)){
							// Ok!
							notActuallyContained=false;
							break;
						}
						
						// Advance to next one:
						box=box.NextInElement;
						
					}
					
					if(notActuallyContained){
						// Not actually inside the element - continue as if nothing happened:
						continue;
					}
					
					// The child contains our point! Step into it.
					// - If it has no kids, or none of the kids are elements, we'll end up returning child.
					// Note that we could trigger the event capture phase here to squeeze a tiny bit of extra performance.
					
					node=child;
					kids=child.childNodes_;
					goto SteppedNode;
					
				}
				
				// Didn't step into anything; stop here.
				break;
				
				SteppedNode:
					continue;
				
			}
			
			// 'Node' is our result:
			return node as Element;
			
		}
		
		/// <summary>Clears this document.</summary>
		public virtual void clear(){
		
			// Clear doc level events:
			ClearEvents();
			
			// Gracefully clear the innerHTML:
			empty();
			
			// Clear other caches:
			ClearStyle();
			
		}
		
		/// <summary>Gets the font with the given name. May load it from the cache or generate a new one.</summary>
		/// <param name="fontName">The name of the font to find.</param>
		/// <returns>A dynamic font if found; null otherwise.</returns>
		public DynamicFont GetOrCreateFont(string fontName){
			
			if(fontName==null){
				return null;
			}
			
			DynamicFont result;
			// Cache contains all available fonts for this document/ renderer.
			ActiveFonts.TryGetValue(fontName,out result);
			
			if(result==null){
				
				// Go get the font now:
				result=DynamicFont.Get(fontName);
				
				// And add it:
				ActiveFonts[fontName]=result;
				
			}
			
			return result;
		}
		
		/// <summary>Gets the font with the given name.</summary>
		/// <param name="fontName">The name of the font to find.</param>
		/// <returns>A dynamic font if found; null otherwise.</returns>
		public DynamicFont GetFont(string fontName){
			if(fontName==null){
				return null;
			}
			
			DynamicFont result;
			ActiveFonts.TryGetValue(fontName,out result);
			return result;
		}
		
		/// <summary>Called when an @font-face font loads.</summary>
		public void FontLoaded(DynamicFont font){
			
			if(childNodes_==null){
				return;
			}
			
			int count=childNodes_.length;
			
			for(int i=0;i<count;i++){
				IRenderableNode node=(childNodes_[i] as IRenderableNode);
				
				if(node==null){
					continue;
				}
				
				node.FontLoaded(font);
			}
			
		}
		
		/// <summary>Requests the document to re-layout.</summary>
		public void RequestLayout(){
			Renderer.DoLayout=true;
		}
		
	}
	
}