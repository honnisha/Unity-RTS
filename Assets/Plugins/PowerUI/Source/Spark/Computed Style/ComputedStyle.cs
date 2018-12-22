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
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Dom;
using PowerUI;


namespace Css{

	/// <summary>
	/// The computed style of a html element.
	/// This holds exactly where it should go and how it should look.
	/// </summary>

	public partial class ComputedStyle:Style{
		
		/// <summary>Information such as this nodes computed box model.</summary>
		public RenderableData RenderData;
		/// <summary>The depth of this element.</summary>
		public float ZIndex;
		/// <summary>The depth of this elements deepest child.</summary>
		public float MaxZIndex;
		
		/// <summary>The global offset from the top edge. Computed in secondary layout pass.</summary>
		public float OffsetTop{
			get{
				return RenderData.OffsetTop;
			}
		}
		
		/// <summary>The global offset from the left edge. Computed in secondary layout pass.</summary>
		public float OffsetLeft{
			get{
				return RenderData.OffsetLeft;
			}
		}
		
		/// <summary>The total width in pixels of this element.</summary>
		public float PixelWidth{
			get{
				return RenderData.PixelWidth;
			}
		}
		
		/// <summary>The total height in pixels of this element.</summary>
		public float PixelHeight{
			get{
				return RenderData.PixelHeight;
			}
		}
		
		/// <summary>The width of the content inside the box. Note that this is different from both InnerWidth and PixelWidth
		/// (which describe the "window" in which the content is seen).</summary>
		public float ContentWidth{
			get{
				return RenderData.ContentWidth;
			}
		}
		
		/// <summary>The width of the content inside the box. Note that this is different from both InnerWidth and PixelWidth
		/// (which describe the "window" in which the content is seen).</summary>
		public float ContentHeight{
			get{
				return RenderData.ContentHeight;
			}
		}
		
		/// <summary>The width of this element, excluding the border and padding.</summary>
		public float InnerWidth{
			get{
				return RenderData.InnerWidth;
			}
		}
		
		/// <summary>The height of this element, excluding the border and padding.</summary>
		public float InnerHeight{
			get{
				return RenderData.InnerHeight;
			}
		}
		
		/// <summary>The scroll offset (from the left) of this element</summary>
		public float ScrollLeft{
			get{
				return GetScrollBox().Left;
			}
		}
		
		/// <summary>The scroll offset (from the top) of this element</summary>
		public float ScrollTop{
			get{
				return GetScrollBox().Top;
			}
		}
		
		/// <summary>The first layout box. Can be null.</summary>
		public LayoutBox FirstBox{
			get{
				return RenderData.FirstBox;
			}
		}
		
		/// <summary>Computes the margin as it is right now.</summary>
		public BoxStyle GetMarginBox(int displayMode,int floatMode,ref bool dimsRequired){
			
			// Get the margin value:
			Css.Value margin=Resolve(Css.Properties.Margin.GlobalProperty);
			
			// Get parent element's computed style:
			IRenderableNode parentEl=Element.parentElement as IRenderableNode;
			ComputedStyle parent=(parentEl==null) ? null : parentEl.ComputedStyle;
			
			// Build the margin:
			return Css.Properties.Margin.GlobalProperty.Compute(margin,RenderData,parent,displayMode,floatMode,ref dimsRequired);
			
		}
		
		/// <summary>Computes the padding as it is right now.</summary>
		public BoxStyle GetPaddingBox(int displayMode){
			
			// Get the padding value:
			Css.Value padding=Resolve(Css.Properties.Padding.GlobalProperty);
			
			// Build the padding:
			return Css.Properties.Padding.GlobalProperty.Compute(padding,RenderData,displayMode);
			
		}
		
		/// <summary>Computes the border widths as it is right now.</summary>
		public BoxStyle GetBorderBox(int displayMode){
			
			// Got a border?
			Css.Value style=Resolve(Css.Properties.BorderStyle.GlobalProperty);
			
			// Get the border width value:
			Css.Value border=Resolve(Css.Properties.BorderWidth.GlobalProperty);
			
			// Build the border:
			return Css.Properties.BorderWidth.GlobalProperty.Compute(border,style,RenderData);
			
		}
		
		/// <summary>Computes the positions as it is right now (right, left etc).</summary>
		public BoxStyle GetPositionBox(int positionMode){
			
			// Get the pos value:
			Css.Value pos=Resolve(Css.Properties.PositionInternal.GlobalProperty);
			
			// Build the pos:
			return Css.Properties.PositionInternal.GlobalProperty.Compute(pos,RenderData,positionMode);
			
		}
		
		/// <summary>Computes the scroll values as it is right now (top and left only).</summary>
		public BoxStyle GetScrollBox(){
			
			// Get the scr value:
			Css.Value scr=Resolve(Css.Properties.Scroll.GlobalProperty);
			
			// Build the scr:
			return Css.Properties.Scroll.GlobalProperty.Compute(scr,RenderData);
			
		}
		
		/// <summary>The raw zoom value.</summary>
		public float ZoomX{
			get{
				Css.Value val=Resolve(Css.Properties.Zoom.GlobalProperty);
				
				// Special case for auto:
				if(val.IsAuto){
					return (Element.document as ReflowDocument).Zoom;
				}
				
				return val.GetDecimal(RenderData,Css.Properties.Zoom.GlobalProperty);
			}
		}
		
		/// <summary>The transform if there is one. Updates it if it has changed.</summary>
		public void TransformUpdate(){
			
			// Get cached transform:
			Css.Units.TransformValue existing=Resolve(Css.Properties.TransformProperty.GlobalProperty) as Css.Units.TransformValue;
			
			if(existing==null){
				return;
			}
			
			if(existing.Changed){
				// Recalc it now:
				existing.Recalculate(this);
			}
			
		}
		
		/// <summary>The transform if there is one.</summary>
		public Transformation TransformX{
			get{
				// Get cached transform:
				Css.Units.TransformValue existing=Resolve(Css.Properties.TransformProperty.GlobalProperty) as Css.Units.TransformValue;
				
				if(existing==null){
					return null;
				}
				
				// Got one!
				return existing.Transform;
			}
		}
		
		/// <summary>The horizontal-align mode.</summary>
		public int HorizontalAlignX{
			get{
				Css.Value val=Resolve(Css.Properties.TextAlign.GlobalProperty);
				
				// Special case for auto:
				if(val.IsAuto){
					return HorizontalAlignMode.Auto;
				}
				
				return val.GetInteger(RenderData,Css.Properties.TextAlign.GlobalProperty);
			}
		}
		
		/// <summary>The horizontal-align-last mode.</summary>
		public int HorizontalAlignLastX{
			get{
				Css.Value val=Resolve(Css.Properties.TextAlignLast.GlobalProperty);
				
				// Special case for auto:
				if(val.IsAuto){
					return HorizontalAlignMode.Auto;
				}
				
				return val.GetInteger(RenderData,Css.Properties.TextAlignLast.GlobalProperty);
			}
		}
		
		/// <summary>The vertical-align mode.</summary>
		public int VerticalAlignX{
			get{
				return ResolveInt(Css.Properties.VerticalAlign.GlobalProperty);
			}
		}
		
		/// <summary>The white-space mode.</summary>
		public int WhiteSpaceX{
			get{
				return ResolveInt(Css.Properties.WhiteSpace.GlobalProperty);
			}
		}
		
		/// <summary>Resolves the given property at this style as an integer.</summary>
		internal int ResolveInt(CssProperty prop){
			return Resolve(prop).GetInteger(RenderData,prop);
		}
		
		/// <summary>Resolves the given property at this style as a float.</summary>
		internal float ResolveDecimal(CssProperty prop){
			return Resolve(prop).GetDecimal(RenderData,prop);
		}
		
		/// <summary>The z-index mode.</summary>
		public Css.Value ZIndexX{
			get{
				return Resolve(Css.Properties.ZIndex.GlobalProperty);
			}
		}
		
		/// <summary>The visibility mode.</summary>
		public int VisibilityX{
			get{
				// Note that each keyword has its own class
				return ResolveInt(Css.Properties.Visibility.GlobalProperty);
			}
		}
		
		/// <summary>The inner/ outer CSS display mode.</summary>
		public int DisplayX{
			get{
				// Note that each keyword has its own class
				return ResolveInt(Css.Properties.Display.GlobalProperty);
			}
		}
		
		/// <summary>Defines the position of this element. Static, fixed etc.</summary>
		public int PositionX{
			get{
				// Note that each keyword has its own class
				return ResolveInt(Css.Properties.Position.GlobalProperty);
			}
		}
		
		/// <summary>This elements draw direction. See DirectionMode.</summary>
		public int DrawDirectionX{
			get{
				return ResolveInt(Css.Properties.Direction.GlobalProperty);
			}
		}
		
		/// <summary>Resolves the font size for this style.</summary>
		public int FontWeightX{
			get{
				return ResolveInt(Css.Properties.FontWeight.GlobalProperty);
			}
		}
		
		/// <summary>Resolves the font family for this style. Note that this can be null.</summary>
		internal Css.Units.FontFamilyUnit FontFamilyX{
			get{
				
				Css.Value value=Resolve(Css.Properties.FontFamily.GlobalProperty);
				
				if(value is Css.Keywords.Inherit){
					// Get the inherited value:
					value=(value as Css.Keywords.Inherit).From;
				}
				
				// Get as a font family unit:
				return value as Css.Units.FontFamilyUnit;
				
			}
		}
		
		/// <summary>Resolves the minimum line height for this style.
		/// Returns the raw value so the font size/ font can be cached separately.</summary>
		public Css.Value LineHeightX{
			get{
				Css.Value value=Resolve(Css.Properties.LineHeight.GlobalProperty);
				
				// Special case for 'inherit':
				if(value is Css.Keywords.Inherit){
					// Set it as the from value (this is so we can correctly resolve 'normal'):
					value=(value as Css.Keywords.Inherit).From;
				}
				
				return value;
			}
		}
		
		/// <summary>Resolves the minimum line height for this style.</summary>
		public float LineHeightFullX{
			get{
				
				// Line height:
				Css.Value lineHeightValue=LineHeightX;
				
				float fontSize=FontSizeX;
				
				if(lineHeightValue.IsType(typeof(Css.Keywords.Normal))){
					
					// Should get from the font; we'll use 1.2 here (only affects values relative to line height):
					return 1.2f * fontSize;
					
				}else if(
					lineHeightValue.Type!=Css.ValueType.RelativeNumber && 
					lineHeightValue.GetType()!=typeof(Css.Units.DecimalUnit)
				){
					
					// Just as-is (but not e.g. line-height:1):
					return lineHeightValue.GetRawDecimal();
					
				}
				
				// Some multiple of the font size:
				return lineHeightValue.GetRawDecimal() * fontSize;
			}
		}
		
		/// <summary>Resolves the font size for this style.</summary>
		public float FontSizeX{
			get{
				return ResolveDecimal(Css.Properties.FontSize.GlobalProperty);
			}
		}
		
		/// <summary>Resolves the colour overlay from this style.</summary>
		public Color ColorOverlayX{
			get{
				return Resolve(Css.Properties.ColorOverlay.GlobalProperty).GetColour(RenderData,Css.Properties.ColorOverlay.GlobalProperty);
			}
		}
		
		/// <summary>The current scroll amount.</summary>
		public Css.Value Scroll{
			get{
				return Resolve(Css.Properties.Scroll.GlobalProperty);
			}
		}
		
		/// <summary>The defined width, if there is one.</summary>
		public Css.Value WidthX{
			get{
				// Get width:
				return Resolve(Css.Properties.Width.GlobalProperty);
			}
		}
		
		/// <summary>The defined height, if there is one.</summary>
		public Css.Value HeightX{
			get{
				// Get height:
				return Resolve(Css.Properties.Height.GlobalProperty);
			}
		}
		
		/// <summary>Resolves the full declared height. It's -1 if it's undefined.</summary>
		public float HeightFullX{
			get{
				LayoutBox box=RenderData.FirstBox;
				
				if(box==null){
					return -1f;
				}
				
				// Get the initial height:
				Css.Value heightValue=HeightX;
				
				float height=0f;
				if(heightValue==null || heightValue.IsAuto){
					return -1f;
				}
				
				// Resolve it:
				height=heightValue.GetDecimal(RenderData,Css.Properties.Height.GlobalProperty);
				
				// Clip now:
				height=ClipHeight(box.DisplayMode,height);
				
				if(box.BorderBox){
					
					// Remove padding and border values:
					height-=box.Border.Top+box.Border.Bottom+box.Padding.Top+box.Padding.Bottom;
					
				}
				
				return height;
			}
		}
		
		/// <summary>True if this element is using the border-box model.</summary>
		public bool BorderBoxModel{
			get{
				
				Css.Value value=Resolve(Css.Properties.BoxSizing.GlobalProperty);
				
				if(value==null){
					return false;
				}
				
				return (value.Text=="border-box");
				
			}
		}
		
		/// <summary>Checks if max-height is defined; if so, the height should be clipped.</summary>
		/// <returns>True if the height should be clipped; false for width.</returns>
		internal bool ShouldClipHeight(){
			Css.Value maxHeight = Resolve(Css.Properties.MaxHeight.GlobalProperty);
			return maxHeight!=null && !maxHeight.IsType(typeof(Css.Keywords.None));
		}
		
		/// <summary>Clips a width value by min/max, if they're defined.
		/// Note that you'll need to be careful with regards to box-sizing.</summary>
		internal float ClipWidth(int displayMode,float width){
			
			if(displayMode==DisplayMode.Inline){
				// Ignored by inline.
				return width;
			}
			
			// Max:
			float bound;
			Css.Value bounds=Resolve(Css.Properties.MaxWidth.GlobalProperty);
			
			if(!(bounds.IsType(typeof(Css.Keywords.None)))){
				
				// Get the bound value:
				bound=bounds.GetDecimal(RenderData,Css.Properties.MaxWidth.GlobalProperty);
				
				if(width>bound){
					width=bound;
				}
				
			}
			
			// Min:
			bounds=Resolve(Css.Properties.MinWidth.GlobalProperty);
			
			// Get the bound value:
			bound=bounds.GetDecimal(RenderData,Css.Properties.MinWidth.GlobalProperty);
			
			if(width<bound){
				width=bound;
			}
			
			return width;
			
		}
		
		/// <summary>Clips a height value by min/max, if they're defined.
		/// Note that you'll need to be careful with regards to box-sizing.</summary>
		internal float ClipHeight(int displayMode,float height){
			
			if(displayMode==DisplayMode.Inline){
				// Ignored by inline.
				return height;
			}
			
			// Max:
			float bound;
			Css.Value bounds=Resolve(Css.Properties.MaxHeight.GlobalProperty);
			
			if(!(bounds.IsType(typeof(Css.Keywords.None)))){
				
				// Get the bound value:
				bound=bounds.GetDecimal(RenderData,Css.Properties.MaxHeight.GlobalProperty);
				
				if(height>bound){
					height=bound;
				}
				
			}
			
			// Min:
			bounds=Resolve(Css.Properties.MinHeight.GlobalProperty);
			
			// Get the bound value:
			bound=bounds.GetDecimal(RenderData,Css.Properties.MinHeight.GlobalProperty);
			
			if(height<bound){
				height=bound;
			}
			
			return height;
			
		}
		
		/// <summary>The set of style matches currently being applied here as a linked list.</summary>
		public MatchingRoot FirstMatch;
		/// <summary>The set of style matches currently being applied here as a linked list.</summary>
		public MatchingRoot LastMatch;
		
		
		/// <summary>Creates a new computed style for the given element.</summary>
		/// <param name="element">The element that this is a computed style for.</param>
		public ComputedStyle(Node element):base(element){
			
			// Create the render data now:
			RenderData=new RenderableData(element);
			
		}
		
		/// <summary>Applies all the matched styles now.</summary>
		public void ApplyMatchedStyles(){
			
			MatchingRoot match=FirstMatch;
			
			while(match!=null){
				
				if(!match.SelectorActive || !match.IsTarget){
					match=match.NextInStyle;
					continue;
				}
				
				// Get the style:
				Style style=match.Style;
				
				// Watch out for pseudo's (cs does not always equal 'this'):
				ComputedStyle cs=match.Selector.Target.computedStyle;
				
				// Try applying each of its properties:
				foreach(KeyValuePair<CssProperty,Css.Value> kvp in style.Properties){
					
					// Change it:
					cs.ChangeProperty(kvp.Key,kvp.Value);
					
				}
				
				match=match.NextInStyle;
			}
			
		}
		
		/// <summary>Used for debugging only. Lists selectors that matched this element (including pseudo's).</summary>
		public List<MatchingSelector> MatchedSelectors(){
			
			List<MatchingSelector> results=new List<MatchingSelector>();
			
			MatchingRoot match=FirstMatch;
			
			while(match!=null){
				
				if(match.IsTarget){
					results.Add(match.Selector);
				}
				
				match=match.NextInStyle;
				
			}
			
			return results;
			
		}
		
		/// <summary>Used for debugging only. Builds all matched selectors into a single style object.</summary>
		public Css.Style BuildMatchedStyles(){
			
			Css.Style s=new Css.Style(null);
			
			MatchingRoot match=FirstMatch;
			
			while(match!=null){
				
				if(match.SelectorActive  && match.IsTarget){
					
					// Watch out for pseudo's (cs does not always equal 'this'):
					ComputedStyle cs=match.Selector.Target.computedStyle;
					
					if(cs==this){
						// Get the style and copy it into s:
						match.Style.CopyTo(s,StyleCopyMode.Specifity);
					}
					
				}
				
				match=match.NextInStyle;
				
			}
			
			return s;
		}
		
		/// <summary>Gets a virtual element which can be targeted with the psuedo-element CSS selectors
		/// such as ::before.</summary>
		public Node GetVirtualChild(string name){
			
			VirtualElements virts=RenderData.Virtuals;
			
			if(virts==null){
				return null;
			}
			
			int priority;
			
			switch(name){
				
				case "before":
					priority=Css.BeforeSelector.Priority;
				break;
				case "after":
					priority=Css.AfterSelector.Priority;
				break;
				case "marker":
					priority=Css.MarkerSelector.Priority;
				break;
				default:
					return null;
				
			}
			
			return virts.Get(priority);
			
		}
		
		/// <summary>Called when an attribute changed.</summary>
		public void AttributeChanged(string name){
			
			if(name=="id" || name=="class"){
				
				// Structural refresh:
				RefreshStructure();
				RefreshLocal();
				
			}else{
				
				// Unlikely that we will be re-matching. Because of the frequency of this call
				// It should be highly optimised. In short, ask the stylesheet if this named attrib
				// is used by *any* style selectors:
				if(reflowDocument.SelectorAttributes.ContainsKey(name)){
					
					// Now a lot more likely that this will cause a re-match.
					RefreshLocal();
					
				}else{
					return;
				}
				
			}
			
		}
		
		/// <summary>Refreshes pseudo classes such as :hover on this element.
		/// Optionally refreshes all parents too.
		/// Internally they're called "local matchers".</summary>
		public void RefreshLocal(bool bubble){
			
			// Refresh this:
			RefreshLocal();
			
			if(bubble){
				
				// Go down the DOM refreshing all parents too:
				Node current=Element.parentNode_;
				
				while(current!=null){
					
					// Get the renderable:
					IRenderableNode irn=current as IRenderableNode;
					
					if(irn==null){
						// Hit the document - this is fine to stop here.
						break;
					}
					
					// Refresh it too:
					irn.ComputedStyle.RefreshLocal();
					
					current=current.parentNode_;
				}
				
			}
			
			
		}
		
		/// <summary>Refreshes pseudo classes such as :hover on this element.
		/// Internally they're called "local matchers".</summary>
		public void RefreshLocal(){
			
			if(FirstMatch==null){
				
				// Structural refresh:
				RefreshStructure();
				
				if(FirstMatch==null){
					return;
				}
				
			}
			
			MatchingRoot match=FirstMatch;
			
			while(match!=null){
				
				// We *always* update this, regardless of this being the target or not.
				// It can potentially make some other element redraw (which is fine!)
				match.ResetActive();
				
				// Next:
				match=match.NextInStyle;
				
			}
			
		}
		
		/// <summary>Attempts to apply the given selector to this style (but only if it's a select or a partial select).</summary>
		public void TryApplyLate(StyleRule rule){
			
			// Try a structural match:
			if(rule.Selector.StructureMatch(this,StandardMatcher)){
				
				// Bake! Internally applies for us when it needs to.
				rule.Selector.BakeToTarget(this,StandardMatcher);
				
			}
			
		}
		
		private static CssEvent StandardMatcher=new CssEvent();
		
		/// <summary>Removes a virtual node.</summary>
		public void RemoveVirtual(int priority){
			
			// Get virtuals set:
			VirtualElements virts=RenderData.Virtuals;
			
			if(virts==null){
				return;
			}
			
			if(virts.remove(priority)){
				
				// Request a redraw:
				RequestLayout();
				
			}
			
			if(virts.Elements.Count==0){
				RenderData.Virtuals=null;
			}
			
		}
		
		/// <summary>Gets/ creates a virtual informer element of the given priority.</summary>
		/// <param name='virtSpark'>True if this informer is a virtual spark one and need to be destroyed
		/// when style refreshes.</param>
		public SparkInformerNode GetOrCreateVirtualInformer(int priority,bool virtSpark){
			
			// Get virtuals set:
			VirtualElements virts=RenderData.Virtuals;
			
			if(virts==null){
				virts=new VirtualElements();
				RenderData.Virtuals=virts;
			}
			
			// Already exists?
			SparkInformerNode ele=virts.Get(priority) as SparkInformerNode;
			
			if(ele==null){
				
				// Create it now:
				ele=new SparkInformerNode();
				ele.VirtSpark=virtSpark;
				
				// Apply doc:
				ele.document_=Element.document_;
				
				// Add it:
				virts.push(priority,ele);
			
				// They act as kids of Element:
				ele.parentNode_=Element;
				
				// Trigger added to DOM:
				ele.AddedToDOM();
				
			}
			
			return ele;
			
		}
		
		/// <summary>Gets/ creates a virtual element of the given priority.</summary>
		public Node GetOrCreateVirtual(int priority,string tag){
			return GetOrCreateVirtual(priority,tag,false);
		}
		
		/// <summary>Gets/ creates a virtual element of the given priority.</summary>
		public Node GetOrCreateVirtual(int priority,string tag,bool sparkVirtual){
			
			// Get virtuals set:
			VirtualElements virts=RenderData.Virtuals;
			
			if(virts==null){
				virts=new VirtualElements();
				RenderData.Virtuals=virts;
			}
			
			// Already exists?
			Node ele=virts.Get(priority);
			
			if(ele==null){
				
				// Create it now:
				ele=Element.document_.createElement(tag);
				
				if(sparkVirtual){
					// Mark as virtual:
					ele.setAttribute("spark-virt", "1");
				}
				
				// Add it:
				virts.push(priority,ele);
			
				// They act as kids of Element:
				ele.parentNode_=Element;
				
				// Trigger added to DOM:
				ele.AddedToDOM();
				
			}
			
			return ele;
			
		}
		
		/// <summary>Called when a matcher has changed and the 
		/// given style must now be applied to this CS. Dealing with pseudo's is handled by the callee.</summary>
		/// <returns>True if a property changed.</returns>
		public bool MatchChanged(Style style,bool active){
			
			bool changed=false;
			
			if(active){
				
				// Try applying each of its properties:
				foreach(KeyValuePair<CssProperty,Css.Value> kvp in style.Properties){
					
					// Change it:
					ChangeProperty(kvp.Key,kvp.Value);
					changed=true;
					
				}
				
			}else{
				
				// Remove the style
				// Doesn't matter if it was actually active or not - this is safe.
				
				// For each of it's properties..
				foreach(KeyValuePair<CssProperty,Css.Value> kvp in style.Properties){
					
					// Match?
					Css.Value v;
					if(Properties.TryGetValue(kvp.Key,out v)){
						
						if( v==kvp.Value || ( v.IsCached && v.CachedOrigin==kvp.Value )){
							
							// Remove it:
							ChangeProperty(kvp.Key,null);
							changed=true;
							
						}
						
					}
					
				}
				
				if(changed){
					// We now need to re-apply all matched styles.
					// This is because one we just removed might have
					// replaced some other value:
					ApplyMatchedStyles();
				}
				
			}
			
			return changed;
			
		}
		
		/// <summary>Refreshes this elements selectors.</summary>
		public void RefreshStructure(){
			
			// Note: This doesn't also call RefreshLocal. This is fine
			// as it internally performs the same process anyway.
			
			// Clear non-permanent virtuals:
			if(RenderData.Virtuals!=null){
				
				// (Don't remove things like scrollbars or a caret!)
				List<int> toRemove=null;
				
				VirtualElements virts=RenderData.Virtuals;
				
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
					
					Element e=(kvp.Value as Element);
					
					if(e==null){
						
						// E.g. informers:
						SparkInformerNode informer=kvp.Value as SparkInformerNode;
						
						if(informer!=null && informer.VirtSpark){
							
							// Remove this!
							if(toRemove==null){
								toRemove=new List<int>(virts.Elements.Count);
							}
							
							toRemove.Add(kvp.Key);
							
						}
						
						continue;
					}
					
					if(e.Tag!="span"){
						// E.g. carets and scrollbars
						continue;
					}
					
					// Is it a spark virt node?
					if(e.getAttribute("spark-virt")=="1"){
						
						// Yes - Remove this!
						if(toRemove==null){
							toRemove=new List<int>(virts.Elements.Count);
						}
						
						toRemove.Add(kvp.Key);
						
					}
					
				}
				
				if(toRemove!=null){
					
					// There's a high chance we're removing the whole thing:
					if(toRemove.Count==virts.Elements.Count){
						
						// Great! Just invoke RemovedFromDOM for all of them:
						virts.RemovedAll();
						RenderData.Virtuals=null;
						
					}else{
						
						// Remove each one:
						for(int i=0;i<toRemove.Count;i++){
							
							// Safely remove that one:
							virts.remove(toRemove[i]);
							
						}
						
					}
					
				}
				
			}
			
			// Must remove all values which originate from a matched selector.
			MatchingRoot currentMatch=FirstMatch;
			
			while(currentMatch!=null){
				
				if(!currentMatch.IsTarget){
					// Non-local root. We must make sure this root 
					// is still structurally valid (and remove the selector if it's not)
					
					if(currentMatch.StructuralMatch()){
						// All ok with this one!
						currentMatch=currentMatch.NextInStyle;
						continue;
					}
					
				}
				
				// Remove the match nodes:
				currentMatch.Selector.Remove();
				
				// Step to the next one:
				currentMatch=currentMatch.NextInStyle;
			}
			
			// Start selecting now!
			ReflowDocument doc=reflowDocument;
			
			if(doc==null){
				return;
			}
			
			CssEvent matcher=StandardMatcher;
			
			List<StyleRule> styles=doc.AnySelectors;
			RunMatch(matcher,styles);
			
			// Tag:
			string match=(Element as Element).Tag;
			
			if(match==null){
				match="span";
			}
			
			if(doc.SelectorStubs.TryGetValue(match,out styles)){
				
				// Match:
				RunMatch(matcher,styles);
				
			}
			
			// Classes:
			match=Element.className;
			
			if(!string.IsNullOrEmpty(match)){
				
				int nextSpace=match.IndexOf(' ');
				
				if(nextSpace!=-1){
					
					int currentStart=0;
					int currentEnd=nextSpace;
					
					// While there's still more text..
					while(currentStart<match.Length){
						
						string className="."+match.Substring(currentStart,currentEnd-currentStart);
						
						if(doc.SelectorStubs.TryGetValue(className,out styles)){
							
							// Match:
							RunMatch(matcher,styles);
							
						}
						
						// Move start:
						currentStart=currentEnd+1;
						
						if(currentStart>=match.Length){
							// All done.
							break;
						}
						
						// Next space is at..
						nextSpace=match.IndexOf(' ',currentStart);
						
						if(nextSpace==-1){
							currentEnd=match.Length;
						}else{
							currentEnd=nextSpace;
						}
						
					}
					
				}else{
					
					string className="."+match;
					
					if(doc.SelectorStubs.TryGetValue(className,out styles)){
						
						// Match:
						RunMatch(matcher,styles);
						
					}
					
				}
				
			}
			
			// ID:
			match=Element.id;
			
			if(!string.IsNullOrEmpty(match)){
				
				match="#"+match;
				
				if(doc.SelectorStubs.TryGetValue(match,out styles)){
					
					// Match:
					RunMatch(matcher,styles);
					
				}
				
			}
			
		}
		
		private bool RunMatch(CssEvent matcher,StyleRule rule,List<Selector> selectors){
			
			bool result=false;
			
			foreach(Selector selector in selectors){
				
				// Try a structural match:
				if(selector.StructureMatch(this,matcher)){
					
					// Bake! Internally applies for us when it needs to.
					selector.BakeToTarget(this,matcher);
					
				}
				
			}
			
			return result;
		}
		
		private bool RunMatch(CssEvent matcher,List<StyleRule> styles){
			
			bool result=false;
			
			foreach(StyleRule style in styles){
				
				// Try a structural match:
				if(style.Selector.StructureMatch(this,matcher)){
					
					// Bake! Internally applies for us when it needs to.
					style.Selector.BakeToTarget(this,matcher);
					
				}
				
			}
			
			return result;
		}
		
		public BoxRegion GetBounds(){
			
			return FirstBox;
			
		}
		
		/*
		public bool TransformedOverlap(ComputedStyle style){
		
			if(BGImage==null || style.BGImage==null){
				return false;
			}
			
			return BGImage.Overlaps(style.BGImage);
			
		}
		*/
		
		public bool BoxOverlap(ComputedStyle style){
			
			float remoteMax=style.OffsetLeft + style.PixelWidth;
			
			if(OffsetLeft >= remoteMax){
				return false;
			}
			
			float max=OffsetLeft + PixelWidth;
			
			if(max <= style.OffsetLeft){
				return false;
			}
			
			remoteMax=style.OffsetTop + style.PixelHeight;
			
			if(OffsetTop >= remoteMax){
				return false;
			}
			
			max=OffsetTop + PixelHeight;
			
			if(max <= style.OffsetTop){
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>Gets the position of the midpoint on the x axis.</summary>
		public float GetMidpointX(){
			return OffsetLeft+(PixelWidth/2f);
		}
		
		/// <summary>Gets the position of the midpoint on the y axis.</summary>
		public float GetMidpointY(){
			return OffsetTop+(PixelHeight/2f);
		}
		
		/// <summary>Called when a specific property of the parent was changed.</summary>
		public void ParentPropertyChanged(CssProperty property,RenderableData context,Css.Value newValue){
			
			// If I have this property, potentially re-inherit it.
			// If I re-inherited it, then do the same for my kids.
			Css.Value result;
			bool inherits;
			if(Properties.TryGetValue(property,out result)){
				
				Css.Keywords.Inherit inh=result as Css.Keywords.Inherit;
				
				if(inh==null){
					inherits=false;
				}else{
					inherits=true;
					
					// Update From:
					inh.SetFrom(context,newValue);
					
				}
				
			}else{
				
				// Default value - does it inherit by default?
				inherits=property.Inherits;
				result=newValue;
				
			}
			
			if(inherits){
				
				// I changed! Apply the value here too:
				property.Apply(this,result);
				
				// Same for the kids too!
				NodeList kids=Element.childNodes_;
				
				if(kids!=null){
					
					// Foreach child, request that it is informed too:
					for(int i=0;i<kids.length;i++){
						
						// Flag it that a parent changed a property:
						IRenderableNode el=kids[i] as IRenderableNode;
						
						if(el==null || !(el is Element)){
							// Skip text nodes.
							continue;
						}
						
						// Nudge it too:
						ComputedStyle cs=el.ComputedStyle;
						
						if(cs!=this){
							cs.ParentPropertyChanged(property,context,newValue);
						}
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Called when the parent of the element was changed.
		/// Doesn't apply when the element is first created and wasn't on anything anyway.
		/// It updates all the inherited properties to make sure they're now inheriting the right thing.</summary>
		public void ParentChanged(){
			ParentChanged(true);
		}
		
		/// <summary>Called when the parent of the element was changed.
		/// Doesn't apply when the element is first created and wasn't on anything anyway.
		/// It updates all the inherited properties to make sure they're now inheriting the right thing.</summary>
		private void ParentChanged(bool structure){
			
			if(structure){
				// Reload its style:
				RefreshStructure();
			}
			
			// For each of my properties, re-inherit them.
			// Do this for all of my kids too.
			
			foreach(KeyValuePair<CssProperty,Css.Value> kvp in Properties){
				
				// Is it inherit?
				Css.Keywords.Inherit inherit=kvp.Value as Css.Keywords.Inherit;
				
				if(inherit==null){
					continue;
				}
				
				// Re-evaluate it:
				inherit.ReEvaluate(Element,kvp.Key);
				
			}
			
			VirtualElements virts=RenderData.Virtuals;
			
			if(virts!=null){
				
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
					
					IRenderableNode el=kvp.Value as IRenderableNode;
					
					if(el==null){
						continue;
					}
					
					// Nudge it too:
					ComputedStyle cs=el.ComputedStyle;
					
					if(cs!=this){
						cs.ParentChanged(false);
					}
					
				}
				
			}
			
			// Tell all decendents (including virtuals) their parent has changed too
			// so they can also re-evaluate any inherited properties.
			NodeList kids=Element.childNodes_;
			
			if(kids!=null){
				
				// Foreach child, request that it is recomputed too:
				for(int i=0;i<kids.length;i++){
					
					// Flag it that a parent changed a property:
					IRenderableNode el=kids[i] as IRenderableNode;
					
					if(el==null){
						continue;
					}
					
					// Nudge it too:
					ComputedStyle cs=el.ComputedStyle;
					
					if(cs!=this){
						cs.ParentChanged(true);
					}
					
				}
				
			}
			
		}
		
		/// <summary>The width in pixels of the last whitespace of this element, if it's got one.</summary>
		public int EndSpaceSize{
			get{
				return RenderData.EndSpaceSize;
			}
		}
		
		/// <summary>How much of this elements horizontal content is currently visible? Used by scrolling.</summary>
		/// <returns>A value from 0-1 of how much of the horizontal content is visible. 1 is all of it.</returns>
		public float VisiblePercentageX(){
			return FirstBox.VisiblePercentageX();
		}
		
		/// <summary>How much of this elements vertical content is currently visible? Used by scrolling.</summary>
		/// <returns>A value from 0-1 of how much of the vertical content is visible. 1 is all of it.</returns>
		public float VisiblePercentageY(){
			return FirstBox.VisiblePercentageY();
		}
		
		/// <summary>This is called to change the named property on this element.</summary>
		/// <param name="property">The css property being changed.</param>
		/// <param name="newValue">The new property value.</param>
		public void ChangeProperty(CssProperty property,Css.Value newValue){
		
			Css.Value current=property.GetValue(this);
			
			if(newValue==null && current==null){
				return;
			}
			
			// Precedence:
			if(newValue!=null && current!=null){
				
				if(newValue.Specifity < current.Specifity){
					// Reject - it's less specific.
					
					// Special case if we're attempting to overwrite a set though.
					// e.g. overflow:hidden overwriting overflow-y:auto.
					// The result would be "hidden auto" as the overflow-x component has a lower specifity.
					
					if(current is Css.ValueSet && !(current is CssFunction) && property.HasAliases){
						
						// Check each component.
						for(int i=0;i<current.Count;i++){
							
							if(newValue[i].Specifity >= current[i].Specifity){
								
								// Allow this component!
								CssProperty alias=property.GetAliased(i,false);
								
								if(alias!=null){
									
									// Change just that internal value:
									ChangeProperty(alias,newValue[i]);
									
								}
								
							}
							
						}
						
					}
					
					return;
					
				}
				
			}
			
			if(newValue!=current){
				// Make sure they're not the same object.
				// If they are the same object, we just simply want the value to be changed for rendering only.
				if(newValue!=null){
					// This computes percentage values, including ones contained in rectangles. 
					// It also looks for property:inherit.
					
					if(newValue is Css.Keywords.Inherit){
						
						int specifity=newValue.Specifity;
						
						// Can't share these - create a new inherit object:
						newValue=new Css.Keywords.Inherit(Element,property);
						
						// Transfer specifity:
						newValue.Specifity=specifity;
						
					}else if(newValue is Css.Keywords.Initial){
						
						int specifity=newValue.Specifity;
						
						// Can't share these either - create a new one:
						newValue=new Css.Keywords.Initial(property);
						
						// Transfer specifity:
						newValue.Specifity=specifity;
						
					}
					
				}
				
				if(newValue!=null && current!=null && newValue.Equals(current)){
					return;
				}
				
				// They're different - we have a new value. Write it out:
				this[property]=newValue;
				
			}
			
			if(property.IsAlias){
				
				// Apply the host value:
				property=(property as CssPropertyAlias).Target;
				
				// Pull the raw value:
				newValue=property.GetValue(this);
				
			}
			
			if(newValue==null){
				// It was removed. Apply the default.
				newValue=property.InitialValue;
			}
			
			// Apply it now (apply its value):
			property.Apply(this,newValue);
			
			// Update inheritance - it might be inherited by child nodes:
			NodeList kids=Element.childNodes_;
			
			if(kids!=null){
				
				// Foreach child, request that it is informed too:
				for(int i=0;i<kids.length;i++){
					
					// Flag it that a parent changed a property:
					IRenderableNode el=kids[i] as IRenderableNode;
					
					if(el==null || !(el is Element)){
						// Skip text nodes.
						continue;
					}
					
					// Nudge it too:
					ComputedStyle cs=el.ComputedStyle;
					
					if(cs!=this){
						cs.ParentPropertyChanged(property,RenderData,newValue);
					}
					
				}
				
			}
			
		}
		
		/// <summary>True if vertical values are mapped to being horizontal by the writing system
		/// and vice-versa.</summary>
		public bool WritingSystemInverted{
			get{
				
				// Get the writing system map (4 entries):
				int[] wsMap=WritingSystemMap;
				
				// The standard CSS order is Top, Right, Bottom, Left
				// A map which does nothing simply maps top to top, down to down etc.
				// (which it does by being [0,1,2,3]).
				
				// The system is inverted if a vertical property, like top, actually refers to
				// a horizontal property; i.e. the index for top is left or right:
				
				return wsMap[0]==3 || wsMap[0]==1;
				
			}
		}
		
		/// <summary>Gets the mapping for the writing system.</summary>
		public int[] WritingSystemMap{
			get{
				// Get the mapping (typically pulls a cached version):
				return Css.Properties.SparkWritingSystem.GlobalProperty.RequireMap(this);
			}
		}
		
		/// <summary>Checks if the given overflow type requires a scrollbar.</summary>
		/// <param name="type">The overflow type to check.</param>
		/// <returns>True if it's auto or scroll; false otherwise.</returns>
		private bool NeedsScrollbar(int type){
			return (type&VisibilityMode.AutoOrScroll)!=0;
		}
		
		/// <summary>Generates a new scrollbar with the given orientation.</summary>
		/// <param name="horizontal">True for a horizontal scrollbar, false for vertical.</param>
		private HtmlScrollbarElement MakeScrollbar(bool horizontal,bool both,bool auto){
			
			int priority=horizontal ? HorizontalScrollPriority : VerticalScrollPriority;
			
			// Create the scrollbar element:
			HtmlScrollbarElement bar=GetOrCreateVirtual(
				priority,
				"scrollbar"
			) as HtmlScrollbarElement;
			
			// Apply type:
			string scrollDir=horizontal?"horizontal":"vertical";
			bar.setAttribute("orient", scrollDir);
			bar.style.position="-spark-absolute-fixed";
			
			if(horizontal){
				bar.style.bottom="0px";
				bar.style.left="0px";
				
				if(both){
					bar.style.width="calc(100% - 17px)";
				}else{
					bar.style.width="100%";
				}
				
			}else{
				bar.style.right="0px";
				bar.style.top="0px";
				
				if(both){
					bar.style.height="calc(100% - 17px)";
				}else{
					bar.style.height="100%";
				}
				
			}
			
			if(auto){
				
				// Add an informer so we can check when it's visible again:
				SparkInformerNode inf=GetOrCreateVirtualInformer(
					-priority,
					false
				);
				
				// Hook up the on render method:
				inf.OnRenderInform=delegate(Renderman r,SparkInformerNode informer){
					
					if(!bar.Hidden){
						return;
					}
					
					// Get the target CS:
					ComputedStyle computed=bar.scrollTarget.Style.Computed;
					
					// Get the first box:
					LayoutBox cBox=computed.FirstBox;
					
					if(cBox==null){
						// Not visible or hasn't been drawn yet.
						return;
					}
					
					// Check the size:
					float vis=bar.IsVertical?cBox.VisiblePercentageY() : cBox.VisiblePercentageX();
					
					if(vis<1f){
						
						// Not hidden anymore:
						bar.Hidden=false;
						
						// Make the bar visible again:
						bar.Style.display="block";
						
					}
					
				};
				
			}
			
			return bar;
		}
		
		/// <summary>Resets the scrollbar elements for this element.</summary>
		public void ResetScrollbars(int overflowX,int overflowY){
			
			VirtualElements virts=RenderData.Virtuals;
			Node hScroll;
			Node vScroll;
			bool hasHBar;
			bool hasVBar;
			
			if(virts==null){
				hScroll=null;
				vScroll=null;
				hasHBar=false;
				hasVBar=false;
			}else{
				hScroll=virts.Get(HorizontalScrollPriority);
				vScroll=virts.Get(VerticalScrollPriority);
				hasHBar=(hScroll!=null);
				hasVBar=(vScroll!=null);
			}
			
			bool needsHBar=NeedsScrollbar(overflowX);
			bool needsVBar=NeedsScrollbar(overflowY);
			
			if(hasHBar==needsHBar && hasVBar==needsVBar){
				return;
			}
			
			bool both=(needsVBar && needsHBar);
			
			if(hasHBar!=needsHBar){
				
				if(needsHBar){
					
					// Generate a new scrollbar:
					MakeScrollbar(true,both,overflowX==VisibilityMode.Auto);
					
				}else{
					
					// Both the bar and an informer:
					virts.remove(HorizontalScrollPriority);
					virts.remove(-HorizontalScrollPriority);
					
				}
				
			}
			
			if(hasVBar!=needsVBar){
				
				if(needsVBar){
					
					// Generate a new scrollbar:
					MakeScrollbar(false,both,overflowY==VisibilityMode.Auto);
					
				}else{
					
					// Both the bar and an informer:
					virts.remove(VerticalScrollPriority);
					virts.remove(-VerticalScrollPriority);
					
				}
				
			}
			
			if(both){
				
				// Make a resizer:
				GetOrCreateVirtual(ResizerPriority,"resizer");
				
			}else if(virts!=null){
				
				// Delete resizer if there is one:
				virts.remove(ResizerPriority);
				
			}
			
			RequestLayout();
			
		}
		
		public const int HorizontalScrollPriority=VirtualElements.SCROLLBAR_ZONE;
		public const int VerticalScrollPriority=HorizontalScrollPriority+2;
		public const int ResizerPriority=VerticalScrollPriority+2;
		
		/// <summary>The current reflow capable document.</summary>
		public ReflowDocument reflowDocument{
			get{
				return (Element.document_ as ReflowDocument);
			}
		}
		
		/// <summary>Requests that the document repaints this element (and all kids) when possible.</summary>
		public void RequestPaintAll(){
			RenderData.RequestPaintAll();
		}
		
		/// <summary>Requests that the document repaints this element, when possible.</summary>
		public void RequestPaint(){
			RenderData.RequestPaint();
		}
		
		/// <summary>Requests that the renderer performs a layout on the next update.
		/// Note that layouts are more expensive than a paint. Paints simply update vertex colours
		/// and uvs where as layouts rebuild the whole mesh.</summary>
		public void RequestLayout(){
			RenderData.RequestLayout();
		}
		
		/// <summary>Requests that the renderer performs a shortform layout on the next update.</summary>
		public void RequestFastLayout(){
			RenderData.RequestLayout();
		}
		
		/// <summary>If this style defines the named property then it gets returned.
		/// Otherwise, it'll either return an inherited value or the default, depending on the properties own settings.</summary>
		public Css.Value Resolve(CssProperty property){
			
			// Very similar to CssProperty.GetOrCreateValue - just we essentially always ignore aliases.
			
			// Pull the value from the block:
			Css.Value result;
			if(Properties.TryGetValue(property,out result)){
				
				return result;
				
			}
			
			// Does it inherit?
			if(property.Inherits){
				
				// Yep - inherit is our result - we will cache these ones:
				result=new Css.Keywords.Inherit(Element,property);
				
				// It's implied so we'll use -1 specifity (everything, including *, will override it):
				result.Specifity=-1;
				
				// Cache it now (we don't want to keep re-resolving the inherit chains):
				Properties[property]=result;
				
				return result;
				
			}
			
			// Just return the initial value:
			return property.InitialValue;
			
		}
		
		/// <summary>Gets or sets the parsed value of this style by property name.</summary>
		/// <param name="property">The property to get the value for.</param>
		public override Value this[CssProperty property]{
			get{
				return Resolve(property);
			}
			set{
				property.OnReadValue(this,value);
			}
		}
		
	}
	
}