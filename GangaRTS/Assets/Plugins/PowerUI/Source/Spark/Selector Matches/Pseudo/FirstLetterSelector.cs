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
using Css.Units;
using Dom;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Handles the first-letter pseudo selector.
	/// <summary>

	sealed class FirstLetter:CssKeyword{
		
		public override string Name{
			get{
				return "first-letter";
			}
		}
		
		public override SelectorMatcher GetSelectorMatcher(){
			return new FirstLetterSelector();
		}
		
	}
	
	/// <summary>
	/// Describes the first-letter pseudo-selector.
	/// <summary>
	
	public class FirstLetterSelector:PseudoSelectorMatch{
		
		// It goes before everything.
		public const int Priority=VirtualElements.BEFORE_ZONE-10;
		
		public override void Select(CssEvent e){
			
			// Get the CS:
			ComputedStyle cs=e.SelectorTarget.computedStyle;
			
			// Create and apply using an informer:
			SparkInformerNode informer=cs.GetOrCreateVirtualInformer(Priority,true);
			
			if(informer.OnStart==null){
				
				// Add a text node as the first child:
				RenderableTextNode textNode=new RenderableTextNode();
				informer.appendChild(textNode);
				
				// Setup the delegates!
				informer.OnStart=delegate(Renderman renderer,SparkInformerNode node){
					
					// Apply first letter - it'll be consumed by the first text node to encounter it:
					renderer.FirstLetter=node;
					
				};
				
				informer.OnEnd=delegate(Renderman renderer,SparkInformerNode node){
					
					// Just in case it wasn't used up:
					renderer.FirstLetter=null;
					
				};
				
			}
			
			e.SelectorTarget=informer.RenderData;
			
		}
		
	}
	
}