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

namespace Css{
	
	/// <summary>
	/// Selectors consist of a bunch of 'roots' which match with elements.
	/// When they match, one of these instances is created.
	/// Note that it's a structural match - just because it has matched structurally
	/// does not mean it is actually *active*.
	/// ComputedStyle holds a list of these.
	/// <summary>

	public class MatchingRoot{
		
		/// <summary>True if this matched root is 'active'. If all a selectors roots
		/// are active then the selector itself is active.</summary>
		public bool Active;
		/// <summary>The root that matched.</summary>
		public RootMatcher Root;
		/// <summary>The actual node this is on.</summary>
		public Dom.Node Node;
		/// <summary>The selector *instance*. We instance these so we can track if the selector is active or not.</summary>
		public MatchingSelector Selector;
		/// <summary>A linked list of all the matching roots in a ComputedStyle.</summary>
		public MatchingRoot NextInStyle;
		/// <summary>A linked list of all the matching roots in a ComputedStyle.</summary>
		public MatchingRoot PreviousInStyle;
		
		
		/// <summary>True if the selector is active.</summary>
		public bool SelectorActive{
			get{
				return Selector.Active;
			}
		}
		
		/// <summary>True if this node still matches the structure of the node it's on.</summary>
		public bool StructuralMatch(){
			
			// Try matching it again:
			return Root.TryMatch(Node);
			
		}
		
		/// <summary>True if this element is the target.</summary>
		public bool IsTarget{
			get{
				return Root.IsTarget;
			}
		}
		
		/// <summary>Gets the rule.</summary>
		public StyleRule Rule{
			get{
				return Selector.Selector.Rule;
			}
		}
		
		/// <summary>Gets the style.</summary>
		public Style Style{
			get{
				return Selector.Selector.Rule.Style;
			}
		}
		
		/// <summary>The local matchers (if any - may be null). All of these must match
		/// in order for this matched root to be active.</summary>
		public LocalMatcher[] LocalMatchers{
			get{
				return Root.LocalMatchers;
			}
		}
		
		/// <summary>Figures out if this root is active or not.
		/// That may, in turn, figure out if the whole selector is/ is not.</summary>
		public void ResetActive(){
			
			// Get the local matchers:
			LocalMatcher[] locals=Root.LocalMatchers;
			
			bool active=true;
			
			if(locals!=null){
				
				// For each one..
				for(int i=0;i<locals.Length;i++){
					
					// Is it active?
					if(!locals[i].TryMatch(Node)){
						active=false;
						break;
					}
					
				}
				
			}
			
			if(active==Active){
				return;
			}
			
			// Active changed!
			Active=active;
			
			// Tell the selector:
			int activeCount=Selector.ActiveRoots;
			
			if(active){
				activeCount++;
			}else{
				activeCount--;
			}
			
			Selector.ActiveRoots=activeCount;
			
			if(activeCount==Selector.RootCount){
				
				// They're all active!
				Selector.Active=true;
				
				if(Selector.Target!=null){
					
					// Apply target:
					ComputedStyle cs=Selector.Target.computedStyle;
					
					// Apply now!
					cs.MatchChanged(Style,true);
					
				}
				
			}else if(Selector.Active){
				
				// The selector is no longer active.
				Selector.Active=false;
				
				if(Selector.Target!=null){
					
					// Apply target:
					ComputedStyle cs=Selector.Target.computedStyle;
					
					// Apply now!
					cs.MatchChanged(Style,false);
					
				}
				
			}
			
		}
		
	}
	
}