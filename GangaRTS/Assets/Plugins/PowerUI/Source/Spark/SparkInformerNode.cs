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
using PowerUI;


namespace Css{
	
	/// <summary>Events triggered on an informer node.</summary>
	public delegate void InformNodeEvent(Renderman renderer,SparkInformerNode informer);
	
	/// <summary>'Informer' node. These are virtual nodes which can simply trigger some generic method when they get rendered.
	/// Used by special effects like first-letter.</summary>
	public class SparkInformerNode : Node,IRenderableNode{
		
		/// <summary>True if this is a virtual spark node and 
		/// should be destroyed whenever style reloads.</summary>
		public bool VirtSpark;
		/// <summary>End event.</summary>
		public InformNodeEvent OnEnd;
		/// <summary>Start event.</summary>
		public InformNodeEvent OnStart;
		/// <summary>On render event.</summary>
		public InformNodeEvent OnRenderInform;
		/// <summary>The computed style.</summary>
		internal ComputedStyle Computed;
		/// <summary>The next one in the list.</summary>
		public SparkInformerNode NextInformer;
		
		
		public SparkInformerNode(){
			Computed=new ComputedStyle(this);
		}
		
		public SparkInformerNode(InformNodeEvent onStart,InformNodeEvent onEnd){
			OnStart=onStart;
			OnEnd=onEnd;
			Computed=new ComputedStyle(this);
		}
		
		/// <summary>Called when the parent element has started rendering its kids.</summary>
		public void Start(Renderman renderer){
			
			// Push:
			NextInformer=renderer.FirstInformer;
			renderer.FirstInformer=this;
			
			if(OnStart!=null){
				OnStart(renderer,this);
			}
			
		}
		
		/// <summary>Called when the parent element has finished rendering its kids.</summary>
		public void End(Renderman renderer){
			
			if(OnEnd!=null){
				OnEnd(renderer,this);
			}
			
		}
		
		/// <summary>This nodes render data.</summary>
		public RenderableData RenderData{
			get{
				return Computed.RenderData;
			}
		}
		
		/// <summary>This nodes computed style.</summary>
		public ComputedStyle ComputedStyle{
			get{
				return Computed;
			}
		}
		
		public void OnRender(Renderman renderer){
			
			if(OnRenderInform!=null){
				OnRenderInform(renderer,this);
			}
			
		}
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.</summary>
		public void GetWidthBounds(out float min,out float max){
			min=0f;
			max=0f;
		}
		
		/// <summary>Gets the first element which matches the given selector.</summary>
		public Element querySelector(string selector){
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector){
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector,bool one){
			return null;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selectors">The selectors to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public void querySelectorAll(Selector[] selectors,INodeList results,CssEvent e,bool one){}
		
		public void WentOffScreen(){
			RenderData.WentOffScreen();
		}
		
		public void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			// This one never runs, but it's required by the interface.
		}
		
		/// <summary>Called when a @font-face font is done loading.</summary>
		public void FontLoaded(DynamicFont font){
			
			Css.TextRenderingProperty text=RenderData.Text;
			
			if(text!=null){
				
				text.FontLoaded(font);
				
			}
			
		}
		
	}
	
}