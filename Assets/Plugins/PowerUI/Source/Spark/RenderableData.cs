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
using System.Reflection;
using Dom;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Holds computed layout boxes for nodes and any style information.
	/// </summary>
	
	public partial class RenderableData{
		
		/// <summary>The node that this is CSS data for.</summary>
		public Node Node;
		/// <summary>The zoom scale for all pixel based values on this style.</summary>
		public float ValueScale=1f;
		/// <summary>True if this element is being repainted on the next frame.</summary>
		public UpdateMode NextUpdateMode=UpdateMode.None;
		/// <summary>The first layout box. Forms a linked list using NextInElement.</summary>
		public LayoutBox FirstBox;
		/// <summary>The last layout box. Forms a linked list using NextInElement.</summary>
		public LayoutBox LastBox;
		/// <summary>Used for paint events. The next element style to paint.</summary>
		public RenderableData Next;
		/// <summary>This data's rendering ancestor. Used to resolve relative units and for rapid reflow.
		/// Usually set to Node.parentNode in the normal flow.</summary>
		public RenderableData Ancestor;
		
		/// <summary>All stacking contexts can have child stacking contexts.
		/// This exists if this is a stacking context.
		/// It will always contain at least 1 entry (at index 0) which refers to *this* node.
		/// That essentially enables negative z-index values to render before the normal flow.
		/// </summary>
		public SortedDictionary<int,RenderableData> StackingContexts;
		/// <summary>The set of virtual elements if it exists.</summary>
		public VirtualElements Virtuals;
		/// <summary>The region that this element takes on the screen. Null if it's offscreen.</summary>
		internal ScreenRegion OnScreenRegion;
		
		
		/// <summary>The ValueScale relative to the parent node.</summary>
		public float ValueScaleRelative{
			get{
				RenderableData parent=parentData;
				
				if(parent==null){
					return ValueScale;
				}
				
				return ValueScale / parent.ValueScale;
			}
		}
		
		/// <summary>True if this is a stacking context.</summary>
		public bool IsStackingContext{
			get{
				return (StackingContexts!=null);
			}
			set{
				if(value){
					
					if(StackingContexts!=null){
						return;
					}
					
					// Create it:
					StackingContexts=new SortedDictionary<int,RenderableData>();
					
					// Add this:
					StackingContexts[0]=this;
					
				}else{
					
					// Just clear the set:
					StackingContexts=null;
					
				}
			}
		}
		
		/// <summary>Requests that the document repaints this element, when possible.</summary>
		public void RequestPaintAll(){
			
			if(Ready){
				
				// Request a paint now:
				Document.Renderer.RequestUpdate(this,UpdateMode.PaintAll);
			
			}else{
				
				// Nope - Full layout first:
				Document.Renderer.RequestLayout();
				
			}
			
		}
		
		/// <summary>Requests that the document repaints this element, when possible.</summary>
		public void RequestPaint(){
			
			// Has it been drawn at all?
			if(Ready){
				
				// Request a paint now:
				Document.Renderer.RequestUpdate(this,UpdateMode.Paint);
			
			}else{
				
				// Nope - Full layout first:
				Document.Renderer.RequestLayout();
				
			}
			
		}
		
		/// <summary>True if this is not positioned in the flow</summary>
		public bool IsOutOfFlow{
			get{
				return ((FirstBox.PositionMode & PositionMode.InFlow)==0);
			}
		}
		
		/// <summary>Requests that the renderer performs a layout on the next update.
		/// Note that layouts are more expensive than a paint. Paints simply update vertex colours
		/// and uvs where as layouts rebuild the whole mesh.</summary>
		public void RequestLayout(){
			
			/*
			// Has it been drawn at all?
			if(Ready){
				
				RenderableData reflowAncestor;
				
				if((FirstBox.PositionMode & PositionMode.InFlow)==0){
					
					// This box is the reflow ancestor:
					reflowAncestor=this;
					
				}else if(Ancestor!=null){
					
					reflowAncestor=Ancestor;
					
				}else{
					
					// Full layout required:
					Document.Renderer.RequestLayout();
					return;
					
				}
				
				// Request a full reflow now:
				Document.Renderer.RequestUpdate(reflowAncestor,UpdateMode.Reflow);
			
			}else{
				
				// Nope - Full layout first:
				Document.Renderer.RequestLayout();
				
			}
			*/
			
			// Disabled for now - just a full layout:
			Document.Renderer.RequestLayout();
			
		}
		
		/// <summary>Requests that the renderer performs a shortform layout on the next update.</summary>
		public void RequestFastLayout(){
			
			/*
			// Has it been drawn at all?
			if(Ready){
				
				// Request a fast reflow now:
				Document.Renderer.RequestUpdate(this,UpdateMode.FastReflow);
			
			}else{
				
				// Nope - Full layout first:
				Document.Renderer.RequestLayout();
				
			}
			*/
			
			// Disabled for now - just a full layout:
			Document.Renderer.RequestLayout();
			
		}
		
		/// <summary>A reflow-capable document.</summary>
		public ReflowDocument Document{
			get{
				return Node.document as ReflowDocument;
			}
		}
		
		/// <summary>Gets the parent node.</summary>
		public Node parentNode{
			get{
				return Node.parentNode_;
			}
		}
		
		/// <summary>Counts the number of boxes in the box linked list (starting at FirstBox).</summary>
		public int BoxCount{
			get{
				int result=0;
				
				LayoutBox box=FirstBox;
				while(box!=null){
					result++;
					box=box.NextInElement;
				}
				
				return result;
			}
		}
		
		/// <summary>The parent renderable data.</summary>
		public RenderableData parentData{
			get{
				IRenderableNode renderable=(Node.parentNode_ as IRenderableNode);
				
				if(renderable==null){
					return null;
				}
				
				return renderable.RenderData;
			}
		}
		
		/// <summary>The computed style to use.</summary>
		public ComputedStyle computedStyle{
			get{
				return (Node as IRenderableNode).ComputedStyle;
			}
		}
		
		/// <summary>The global offset from the top edge. Computed in secondary layout pass.</summary>
		public float OffsetTop{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.Y;
			}
		}
		
		/// <summary>Maps the given pixel value to an amount in world units.</summary>
		public float ScaleToWorldX(float value){
			
			if(Document.worldUI!=null){
				return (-(float)value);
			}
			
			return ((-(float)value)*ScreenInfo.WorldPerPixel.x);
			
		}
		
		/// <summary>Maps the given pixel value to an amount in world units.</summary>
		public float ScaleToWorldY(float value){
			
			if(Document.worldUI!=null){
				return (-(float)value);
			}
			
			return ((-(float)value)*ScreenInfo.WorldPerPixel.y);
			
		}
		
		/// <summary>Repaints this and all its childnodes.</summary>
		public void RepaintAll(Renderman renderer){
			
			// Repaint this now:
			Repaint(renderer);
			
			// Must also repaint child/virtual nodes too (as they share the same CS or inherit values):
			NodeList kids=Node.childNodes_;
			
			VirtualElements virts=Virtuals;
			
			if(virts!=null && !virts.AllowDrawKids){
				kids=null;
			}
			
			if(virts!=null){
				
				// For each virtual element..
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
					
					// Repaint it too:
					IRenderableNode irn=(kvp.Value as IRenderableNode);
					
					if(irn!=null){
						
						// Has it requested a repaint itself?
						// If it did, don't bother repainting it - it'll handle itself
						if(irn.RenderData.NextUpdateMode==UpdateMode.None){
							
							// Repaint it and all kids:
							irn.RenderData.RepaintAll(renderer);
						
						}
						
					}
					
				}
				
			}
			
			if(kids!=null){
				
				// For each child node..
				for(int i=0;i<kids.length;i++){
					
					// Get it as a text node:
					Node child=kids[i];
					
					// Repaint it too:
					IRenderableNode irn=(child as IRenderableNode);
					
					if(irn!=null){
						
						// Has it requested a repaint itself?
						// If it did, don't bother repainting it - it'll handle itself
						if(irn.RenderData.NextUpdateMode==UpdateMode.None){
							
							// Repaint it and all kids:
							irn.RenderData.RepaintAll(renderer);
						
						}
						
					}
					
				}
				
			}
			
		}
		
		/// <summary>Repaints this at OffsetLeft/OffsetTop with the given PixelWidth and PixelHeight values.
		/// A paint is less intensive than a layout as it only updates the uv/vert colours of the mesh.</summary>
		public void Repaint(Renderman renderer){
			
			// Get the color overlay:
			Color parentOverlay=renderer.ColorOverlay;
			
			// Get the preferred computed style:
			ComputedStyle cs=computedStyle;
			
			// Apply our new one (note that it's already composite):
			renderer.ColorOverlay = cs.ColorOverlayX;
			
			// Get the transform (note that it's always tied to any inherited transforms during the layout process):
			Transformation transform=cs.TransformX;
			
			// For each box, trigger a repaint using cs:
			LayoutBox box=FirstBox;
			
			if(transform!=null && transform.Changed){
				// The transform changed.
				// To deal with this, we need to:
				// - Recalc the transform matrix and the matrix of all its derivatives.
				// - Apply the 'delta' matrix to all the childNodes.
				
				// Get the previous matrix:
				Matrix4x4 previous=transform.Matrix;
				
				// Recalculate it now:
				transform.RecalculateMatrix(cs,box);
				
				// Get the delta matrix:
				Matrix4x4 delta=transform.Matrix * previous.inverse;
				
				// Apply:
				ApplyTransform(delta,renderer);
				
			}
			
			while(box!=null){
				
				// Paint everything:
				if(RenderProperties!=null){
					
					// For each one..
					for(int p=0;p<RenderProperties.Length;p++){
						
						// Paint it:
						RenderProperties[p].Paint(box,renderer);
						
					}
					
				}
				
				// Go to next one:
				box=box.NextInElement;
			}
			
			// Restore overlay:
			renderer.ColorOverlay = parentOverlay;
			
		}
		
		/// <summary>Applies the transform (rotate, scale etc) of this element to its background/border/content.</summary>
		private void ApplyTransform(Matrix4x4 delta,Renderman renderer){
			
			// If this child has a transform, we must update it (as it's a derivative).
			// We don't, however, change delta though [unless we're not preserving 3D].
			
			// Transform everything:
			if(RenderProperties!=null){
				
				// For each one..
				for(int p=0;p<RenderProperties.Length;p++){
					
					// Transform it:
					RenderProperties[p].ApplyTransform(delta,renderer);
					
				}
				
			}
			
			// Apply transform for all kids and virtual kids.
			NodeList kids=Node.childNodes_;
			
			VirtualElements virts=Virtuals;
			
			if(virts!=null && !virts.AllowDrawKids){
				kids=null;
			}
			
			if(virts!=null){
				
				// For each virtual element..
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
					
					IRenderableNode el=kvp.Value as IRenderableNode;
					
					if(el==null){
						continue;
					}
					
					// If this elements computed style is different to mine
					// then recalc its transformation matrix too:
					ComputedStyle childCS=el.ComputedStyle;
					
					if(childCS!=computedStyle){
						
						// Try getting the transform:
						Transformation transform=childCS.TransformX;
						
						if(transform!=null){
							
							// Recalculate it now (always recalculate it, without updating delta):
							// Note that if it has also changed, it has its own paint event anyway.
							transform.RecalculateMatrix(childCS,childCS.FirstBox);
							
						}
						
					}
					
					el.RenderData.ApplyTransform(delta,renderer);
					
				}
				
			}
			
			if(kids!=null){
				
				for(int i=0;i<kids.length;i++){
					
					IRenderableNode el=kids[i] as IRenderableNode;
					
					if(el==null){
						continue;
					}
					
					// If this elements computed style is different to mine
					// then recalc its transformation matrix too:
					ComputedStyle childCS=el.ComputedStyle;
					
					if(childCS!=computedStyle){
						
						// Try getting the transform:
						Transformation transform=childCS.TransformX;
						
						if(transform!=null){
							
							// Recalculate it now (always recalculate it, without updating delta):
							// Note that if it has also changed, it has its own paint event anyway.
							transform.RecalculateMatrix(childCS,childCS.FirstBox);
							
						}
						
					}
					
					el.RenderData.ApplyTransform(delta,renderer);
					
				}
				
			}
			
		}
		
		/// <summary>Gets the most suitable parent inner width.</summary>
		internal float ParentInnerWidth(){
			
			Node parent=Node.parentNode;
			
			LayoutBox parentBox=null;
			
			if(parent is IRenderableNode){
				
				parentBox=(parent as IRenderableNode).RenderData.FirstBox;
				
				while(parentBox.DisplayMode==DisplayMode.Inline){
					
					// Special case - InnerHeight is invalid.
					
					// Go up the DOM to reach a block-level element:
					parent=parent.parentNode;
					
					if(parent is IRenderableNode){
						
						parentBox=(parent as IRenderableNode).RenderData.FirstBox;
						
					}else{
						
						parentBox=null;
						break;
						
					}
					
				}
				
			}
			
			float relativeTo;
			
			if(parentBox==null){
				
				// Use the document viewport:
				relativeTo=Document.Viewport.Width;
				
			}else{
				
				// We now know which width we're relative to:
				relativeTo=parentBox.InnerWidth;
				
			}
			
			return relativeTo;
			
		}
		
		/// <summary>Gets the most suitable parent inner height.</summary>
		private float ParentInnerHeight(){
			
			Node parent=Node.parentNode;
			
			LayoutBox parentBox=null;
			
			if(parent is IRenderableNode){
				
				parentBox=(parent as IRenderableNode).RenderData.FirstBox;
				
				while(parentBox.DisplayMode==DisplayMode.Inline){
					
					// Special case - InnerHeight is invalid.
					
					// Go up the DOM to reach a block-level element:
					parent=parent.parentNode;
					
					if(parent is IRenderableNode){
						
						parentBox=(parent as IRenderableNode).RenderData.FirstBox;
						
					}else{
						
						parentBox=null;
						break;
						
					}
					
				}
				
			}
			
			float relativeTo;
			
			if(parentBox==null){
				
				// Use the document viewport:
				relativeTo=Document.Viewport.Height;
				
			}else{
				
				// We now know which height we're relative to:
				relativeTo=parentBox.InnerHeight;
				
			}
			
			return relativeTo;
			
		}
		
		/// <summary>The global offset from the left edge. Computed in secondary layout pass.</summary>
		public float OffsetLeft{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.X;
			}
		}
		
		/// <summary>The total width in pixels of this element.</summary>
		public float PixelWidth{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.Width;
			}
		}
		
		/// <summary>The total height in pixels of this element.</summary>
		public float PixelHeight{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.Height;
			}
		}
		
		/// <summary>The width of the content inside the box. Note that this is different from both InnerWidth and PixelWidth
		/// (which describe the "window" in which the content is seen).</summary>
		public float ContentWidth{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.ContentWidth;
			}
		}
		
		/// <summary>The width of the content inside the box. Note that this is different from both InnerWidth and PixelWidth
		/// (which describe the "window" in which the content is seen).</summary>
		public float ContentHeight{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.ContentHeight;
			}
		}
		
		/// <summary>The width of this element, excluding the border and padding.</summary>
		public float InnerWidth{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.InnerWidth;
			}
		}
		
		/// <summary>The height of this element, excluding the border and padding.</summary>
		public float InnerHeight{
			get{
				LayoutBox box=FirstBox;
				
				if(box==null){
					return 0f;
				}
				
				return box.InnerHeight;
			}
		}
		
		/// <summary>True if the box model is ready and loaded.</summary>
		public bool Ready{
			get{
				return FirstBox!=null;
			}
		}
		
		/// <summary>This handles rendering the border around this element, if any.</summary>
		public BorderProperty Border{
			get{
				return GetProperty(typeof(BorderProperty)) as BorderProperty;
			}
			set{
				AddOrReplaceProperty(value,typeof(BorderProperty));
			}
		}
		
		/// <summary>This handles rendering a tiled, clipped, offset background image for this element.</summary>
		public BackgroundImage BGImage{
			get{
				return GetProperty(typeof(BackgroundImage)) as BackgroundImage;
			}
			set{
				AddOrReplaceProperty(value,typeof(BackgroundImage));
			}
		}
		
		/// <summary>This handles rendering a solid background colour for this element.</summary>
		public BackgroundColour BGColour{
			get{
				return GetProperty(typeof(BackgroundColour)) as BackgroundColour;
			}
			set{
				AddOrReplaceProperty(value,typeof(BackgroundColour));
			}
		}
		
		/// <summary>The width in pixels of the last whitespace of this element, if it's got one.</summary>
		public int EndSpaceSize{
			get{
				
				if(Text==null){
					return 0;
				}
				
				return Text.EndSpaceSize;
			}
		}
		
		/// <summary>This handles rendering text contained by this element.</summary>
		public TextRenderingProperty Text{
			get{
				return GetProperty(typeof(TextRenderingProperty)) as TextRenderingProperty;
			}
			set{
				AddOrReplaceProperty(value,typeof(TextRenderingProperty));
			}
		}
		
		/// <summary>This handles rendering 3D text contained by this element. Use text-extrude CSS property (a number).</summary>
		public TextRenderingProperty3D Text3D{
			get{
				return Text as TextRenderingProperty3D;
			}
			set{
				Text=value;
			}
		}
		
		/// <summary>Creates a TextRenderingProperty if one doesn't exist yet. Note that it can be created as a 3D one
		/// if text-extrude is set.</summary>
		public TextRenderingProperty RequireTextProperty(){
			
			// Get the text property:
			TextRenderingProperty text=Text;
			
			if(text==null){
				
				ComputedStyle style=computedStyle;
				
				// Create one:
				Css.Value extr=style[Css.Properties.TextExtrude.GlobalProperty];
				float extrAmount=0f;
				
				if(extr!=null){
					extrAmount=extr.GetDecimal(this,Css.Properties.TextExtrude.GlobalProperty);
				}
				
				
				if(extrAmount!=0f){
					
					// Drawing in 3D:
					TextRenderingProperty3D t3d=new TextRenderingProperty3D(this);
					
					// Apply extrude:
					t3d.Extrude=extrAmount;
					
					text=t3d;
					
				}else{
					
					// Drawing in 2D:
					text=new TextRenderingProperty(this);
					
				}
				
				// Set it up now:
				text.Setup(style);
				
			}
			
			return text;
			
		}
		
		/// <summary>Gets the box which contains the given x/y coords.</summary>
		public LayoutBox BoxAt(float x,float y){
			
			LayoutBox box=FirstBox;
			
			while(box!=null){
				
				// Contains it?
				if(box.Contains(x,y)){
					
					// Great ok, we've got it.
					return box;
					
				}
				
				// Next one:
				box=box.NextInElement;
			}
			
			return null;
			
		}
		
		/// <summary>Selection zone.</summary>
		public SelectionRenderingProperty Selection{
			get{
				return GetProperty(typeof(SelectionRenderingProperty)) as SelectionRenderingProperty;
			}
			set{
				AddOrReplaceProperty(value,typeof(SelectionRenderingProperty));
			}
		}
		
		/// <summary>The set of renderable properties on this element.</summary>
		public DisplayableProperty[] RenderProperties;
		
		/// <summary>True if this element has any form of background.</summary>
		public bool HasBackground{
			get{
				
				if(RenderProperties==null){
					return false;
				}
				
				for(int i=0;i<RenderProperties.Length;i++){
					
					if(RenderProperties[i].IsBackground){
						return true;
					}
					
				}
				
				// Nope!
				return false;
				
			}
		}
		
		
		public RenderableData(Node node){
			Node=node;
		}
		
		/// <summary>Gets a render property of the given type.</summary>
		public DisplayableProperty GetProperty(Type propertyType){
			
			if(RenderProperties==null){
				return null;
			}
			
			for(int i=0;i<RenderProperties.Length;i++){
				
				// Is the property a child of/ equal to propertyType?
				#if NETFX_CORE
				if(propertyType.GetTypeInfo().IsAssignableFrom( RenderProperties[i].GetType().GetTypeInfo() )){
				#else
				if(propertyType.IsAssignableFrom( RenderProperties[i].GetType() )){
				#endif
					
					// Got it!
					return RenderProperties[i];
					
				}
				
			}
			
			// Not found.
			return null;
			
		}
		
		/// <summary>Gets the index of a render property of the given type. -1 if it's not found.</summary>
		public int GetPropertyIndex(Type propertyType){
			
			if(RenderProperties==null){
				return -1;
			}
			
			for(int i=0;i<RenderProperties.Length;i++){
				
				#if NETFX_CORE
				if(propertyType.GetTypeInfo().IsAssignableFrom( RenderProperties[i].GetType().GetTypeInfo() )){
				#else
				if(propertyType.IsAssignableFrom( RenderProperties[i].GetType() )){
				#endif
					
					// Got it!
					return i;
					
				}
				
			}
			
			// Not found.
			return -1;
			
		}
		
		/// <summary>Gets the index of a given render property. -1 if it's not found.</summary>
		public int GetPropertyIndex(DisplayableProperty property){
			
			if(RenderProperties==null){
				return -1;
			}
			
			for(int i=0;i<RenderProperties.Length;i++){
				
				if(property==RenderProperties[i]){
					
					// Got it!
					return i;
					
				}
				
			}
			
			// Not found.
			return -1;
			
		}
		
		/// <summary>Removes the given property.</summary>
		public void RemoveProperty(DisplayableProperty property){
			
			int index=GetPropertyIndex(property);
			
			if(index==-1){
				return;
			}
			
			// Remove it:
			RemoveProperty(index);
			
		}
		
		/// <summary>Adds or replaces a render property of the given type.</summary>
		public void AddOrReplaceProperty(DisplayableProperty property,Type propertyType){
			
			// Get the index:
			int index=GetPropertyIndex(propertyType);
			
			if(property==null){
				
				// Removing it.
				if(index!=-1){
					RemoveProperty(index);
				}
				
				return;
			}
			
			if(index==-1){
				// Adding it.
				AddProperty(property);
			}else{
				// Replacing it.
				RenderProperties[index]=property;
			}
			
		}
		
		/// <summary>Removes the render property at the given index. Use AddOrReplace instead.</summary>
		private void RemoveProperty(int index){
			
			if(RenderProperties.Length==1){
				// Only one able to get removed.
				RenderProperties=null;
				return;
			}
			
			// Create the set:
			DisplayableProperty[] newSet=new DisplayableProperty[RenderProperties.Length-1];
			
			// Transfer values before index:
			if(index>0){
				System.Array.Copy(RenderProperties,0,newSet,0,index);
			}
			
			// After index:
			int afterCount=newSet.Length-index;
			
			if(afterCount>0){
				System.Array.Copy(RenderProperties,index+1,newSet,index,afterCount);
			}
			
			// Done!
			RenderProperties=newSet;
			
		}
		
		/// <summary>Adds the given render property. Use AddOrReplace instead.</summary>
		private void AddProperty(DisplayableProperty property){
			
			if(RenderProperties==null){
				RenderProperties=new DisplayableProperty[]{property};
				return;
			}
			
			// Create the set:
			DisplayableProperty[] newSet=new DisplayableProperty[RenderProperties.Length+1];
			
			// Get it's order (inserted ascending):
			int drawOrder=property.DrawOrder;
			
			// Transfer values, ensuring we insert the new one by draw-order:
			int currentIndex=0;
			bool placed=false;
			
			for(int i=0;i<RenderProperties.Length;i++){
				
				// Get current prop:
				DisplayableProperty current=RenderProperties[i];
				
				// Is the one being added before it?
				if(drawOrder < current.DrawOrder && !placed){
					
					// New one goes right here!
					placed=true;
					newSet[currentIndex]=property;
					currentIndex++;
				}
				
				// Transfer:
				newSet[currentIndex]=current;
				
				// Increase index:
				currentIndex++;
				
			}
			
			if(!placed){
				// Add new one at the end:
				newSet[RenderProperties.Length]=property;
			}
			
			// Done!
			RenderProperties=newSet;
			
		}
		
		/// <summary>Draws this at OffsetLeft/OffsetTop with the given PixelWidth and PixelHeight values.</summary>
		public void Render(bool first,LayoutBox box,Renderman renderer){
			
			if(RenderProperties==null){
				return;
			}
			
			// Need a post process?
			bool post=false;
			
			// For each one..
			for(int p=0;p<RenderProperties.Length;p++){
				
				// Render and transform it:
				post|=RenderProperties[p].Render(first,box,renderer);
				
				// Check if it has been stalled:
				if(renderer.StallStatus!=0){
					return;
				}
				
			}
			
			if(post){
				
				// For each one..
				for(int p=0;p<RenderProperties.Length;p++){
					
					// Call PP on it:
					RenderProperties[p].PostProcess(box,renderer);
					
				}
				
			}
			
		}
		
		/// <summary>Resolves the value from the parent of this node for the given property.</summary>
		internal float ResolveParentDecimal(CssProperty prop){
			
			// Get the parent:
			Node node=Node.parentNode_;
			
			// Get its style:
			ComputedStyle parentStyle=(node as IRenderableNode).ComputedStyle;
			
			if(parentStyle==null){
				return prop.InitialValue.GetDecimal(null,prop);
			}
			
			return parentStyle.Resolve(prop).GetDecimal(parentStyle.RenderData,prop);
			
		}
		
		/// <summary>This is the second pass of layout requests.
		/// It positions the element in global screen space and also fires the render events
		/// which in turn generate or reallocates mesh blocks. This call applies to all it's
		/// children elements too.</summary>
		/// <param name="relativeTo">The current style we are positioning relative to.</param>
		public void Render(Renderman renderer){
			
			ComputedStyle computed=computedStyle;
			
			if(FirstBox==null){
				// Display was none or vis is 'hidden'.
				OnScreenRegion=null;
				return;
			}
			
			// Get the color overlay:
			Color parentOverlay=renderer.ColorOverlay;
			
			// Apply our new one (note that it's already composite):
			renderer.ColorOverlay = computed.ColorOverlayX;
			
			// Push the transform to our stack, if we have one.
			Transformation transform;
			
			if(this is TextRenderableData){
				transform=null;
			}else{
				transform=computed.TransformX;
			}
			
			if(transform!=null){
				
				// Add it to the stack:
				renderer.Transformations.Push(transform);
				
			}
			
			// Custom shaders?
			ShaderSet shaders=renderer.CurrentShaderSet;
			ShaderSet customShaders=computed.CustomShaders;
			
			if(customShaders!=null){
				
				if(renderer.CurrentShaderSet==customShaders){
					
					// Clear:
					customShaders=null;
					
				}else{
					
					// It's got custom shaders. Update renderman:
					renderer.CurrentShaderSet=customShaders;
					
					// Break the batching system:
					renderer.CurrentBatch=null;
					
				}
				
			}
			
			// For each box..
			LayoutBox box=FirstBox;
			
			float screenMaxX=renderer.MaxX;
			float screenMaxY=renderer.MaxY;
			
			// The computed screen region (containing all boxes):
			BoxRegion screenRegion=null;
			
			while(box!=null){
				
				// Nearest ancestor (if it's null, use the viewport):
				LayoutBox ancestor=null;
				LayoutBox parentBox=box.Parent;
				
				// Handle out of flow position modes now:
				int positionMode=box.PositionMode;
				bool isScrolled=true;
				
				if((positionMode & PositionMode.InFlow)==0){
					
					// Out of flow - position:fixed and absolute. 
					
					// Position:fixed?
					isScrolled=(positionMode!=PositionMode.Fixed);
					
					if((positionMode & PositionMode.SparkAbsoluteModes)!=0){
						
						if(positionMode==PositionMode.SparkAbsoluteFixed){
							
							// Fix it (to ignore scrolling)
							isScrolled=false;
							
						}
						
						ancestor=parentBox;
						
					}else if(Ancestor!=null){
						ancestor=Ancestor.FirstBox;
					}
					
					// X axis:
					
					if(box.Position.Right==float.MaxValue && box.Position.Left==float.MaxValue){
						
						// It gets fixed at its in-flow location (but isn't actually placed on a line).
						if(parentBox==null){
							box.X=box.ParentOffsetLeft + renderer.Viewport.X;
						}else{
							box.X=box.ParentOffsetLeft + parentBox.X + parentBox.StyleOffsetLeft;
						}
						
					}else if(box.Position.Left==float.MaxValue){
						
						if(ancestor==null){
							// Relative to the right of the viewport.
							box.X=renderer.Viewport.X+renderer.Viewport.Width-(box.Position.Right+box.Width+box.Margin.Right);
						}else{
							// Relative to the right of our ancestor.
							box.X=ancestor.InnerEndX-(box.Position.Right+box.Width+box.Margin.Right);
						}
						
					}else if(ancestor==null){
						// Relative to the left of the viewport.
						box.X=renderer.Viewport.X+box.Position.Left+box.Margin.Left;
					}else{
						// Relative to the left of our ancestor.
						box.X=ancestor.InnerStartX+box.Position.Left+box.Margin.Left;
					}
					
					// Y axis:
					
					if(box.Position.Top==float.MaxValue && box.Position.Bottom==float.MaxValue){
						
						// It gets fixed at its in-flow location (but isn't actually placed on a line).
						if(parentBox==null){
							box.Y=box.ParentOffsetTop + renderer.Viewport.Y;
						}else{
							box.Y=box.ParentOffsetTop + parentBox.Y + parentBox.StyleOffsetTop;
						}
						
					}else if(box.Position.Top==float.MaxValue){
						
						if(ancestor==null){
							// Relative to the bottom of the viewport.
							box.Y=renderer.Viewport.Y+renderer.Viewport.Height-(box.Position.Bottom+box.Height+box.Margin.Bottom);
						}else{
							// Relative to the bottom of our ancestor.
							box.Y=ancestor.InnerEndY-(box.Position.Bottom+box.Height+box.Margin.Bottom);
						}
					
					}else if(ancestor==null){
						// Relative to the top of the viewport.
						box.Y=renderer.Viewport.Y+box.Position.Top+box.Margin.Top;
					}else{
						// Relative to the top of our ancestor.
						box.Y=ancestor.InnerStartY+box.Position.Top+box.Margin.Top;
					}
					
				}else{
					
					// The parent is our ancestor:
					ancestor=parentBox;
					
					if(positionMode==PositionMode.Relative){
						
						// Offset ParentOffsetTop/Left by our position values.
						
						if(box.Position.Left!=float.MaxValue){
							// Left first (it takes priority over right if both are defined).
							box.ParentOffsetLeft+=box.Position.Left;
						}else if(box.Position.Right!=float.MaxValue){
							// Right:
							box.ParentOffsetLeft-=box.Position.Right;
						}
						
						if(box.Position.Top!=float.MaxValue){
							// Top first (it takes priority over bottom if both are defined).
							box.ParentOffsetTop+=box.Position.Top;
						}else if(box.Position.Bottom!=float.MaxValue){
							// Bottom:
							box.ParentOffsetTop-=box.Position.Bottom;
						}
						
					}
					
					// In flow.
					if(parentBox==null){
						box.X=box.ParentOffsetLeft + renderer.Viewport.X;
						box.Y=box.ParentOffsetTop + renderer.Viewport.Y;
					}else{
						box.X=box.ParentOffsetLeft + parentBox.X + parentBox.StyleOffsetLeft;
						box.Y=box.ParentOffsetTop + parentBox.Y + parentBox.StyleOffsetTop;
					}
					
				}
				
				if(isScrolled && ancestor!=null){
					
					// Use the accumulated scroll value from the ancestor:
					box.X-=ancestor.Scroll.Left;
					box.Y-=ancestor.Scroll.Top;
					
				}
				
				// Update max:
				box.MaxX=box.X+box.Width;
				box.MaxY=box.Y+box.Height;
				
				// Apply the segment to draw:
				int segment=LineBoxSegment.Middle;
				
				// First?
				if(box==FirstBox){
					segment|=LineBoxSegment.Start;
					
				}
				
				// Is the box on screen at all?
				if(box.X<screenMaxX && box.Y<screenMaxY && box.MaxX>0 && box.MaxY>0){
					
					// On screen.
					
					if(screenRegion==null){
						
						// Create the screen region now:
						screenRegion=new BoxRegion(box.X,box.Y,box.Width,box.Height);
						
					}else{
						
						// Combine it in:
						if(box.X<screenRegion.X){
							screenRegion.X=box.X;
						}
						
						if(box.Y<screenRegion.Y){
							screenRegion.Y=box.Y;
						}
						
						if(box.MaxX>screenRegion.MaxX){
							screenRegion.MaxX=box.MaxX;
						}
						
						if(box.MaxY>screenRegion.MaxY){
							screenRegion.MaxY=box.MaxY;
						}
						
					}
					
				}
				
				// Last?
				if(box.NextInElement==null){
					segment|=LineBoxSegment.End;
				}
				
				if(transform!=null && box==FirstBox){
					
					// Update it:
					transform.RecalculateMatrix(computed,box);
					
				}
				
				renderer.Segment=segment;
				
				if(screenRegion!=null){
					// Great, it's good to go!
					Render(box==FirstBox,box,renderer);
				}
				
				// Advance to the next box:
				box=box.NextInElement;
				
			}
			
			// Call layout method:
			(Node as IRenderableNode).OnRender(renderer);
			
			if(screenRegion!=null){
				
				// Update w/h:
				screenRegion.Width=screenRegion.MaxX-screenRegion.X;
				screenRegion.Height=screenRegion.MaxY-screenRegion.Y;
				
			}
			
			// Set screen region:
			OnScreenRegion=screenRegion;
			
			// Push into input grid:
			renderer.InputGrid.Push(this);
			
			if(Node.ClearBackground){
				renderer.ViewportBackground=false;
			}
			
			NodeList kidsToRender=Node.childNodes_;
			VirtualElements virts=Virtuals;
			
			if(virts!=null && !virts.AllowDrawKids){
				kidsToRender=null;
			}
			
			if(renderer.StallStatus!=0){
				
				if(renderer.StallStatus==2){
					kidsToRender=null;
				}
				
				renderer.StallStatus=0;
				
			}
			
			if(kidsToRender!=null || virts!=null){
				
				BoxRegion parentBoundary=renderer.ClippingBoundary;
				
				if(FirstBox.DisplayMode!=DisplayMode.Inline){
					
					// Change the clipping boundary:
					renderer.SetBoundary(computed,FirstBox);
				
				}
				
				if(kidsToRender!=null){
					
					for(int i=0;i<kidsToRender.length;i++){
						
						IRenderableNode child=kidsToRender[i] as IRenderableNode;
						
						if(child!=null){
							child.RenderData.Render(renderer);
						}else{
							
							// It's not renderable - it can potentially be a document though:
							ReflowDocument ird=kidsToRender[i] as ReflowDocument;
							
							if(ird!=null && screenRegion!=null){
								
								// This usually means we're entering an iframe.
								
								// Get it as a renderable node:
								child=(ird.documentElement as IRenderableNode);
								
								if(child!=null){
									
									// Cache screen viewport:
									BoxRegion prevViewport=renderer.Viewport;
									
									// Update viewport:
									renderer.Viewport=screenRegion;
									
									// Handle iframe backgrounds:
									renderer.ViewportBackground=true;
									
									// Render it:
									child.RenderData.Render(renderer);
									
									// Restore viewport:
									renderer.Viewport=prevViewport;
									
								}
								
							}
							
						}
						
					}
					
				}
				
				if(virts!=null){
					
					foreach(KeyValuePair<int,Node> kvp in virts.Elements){
						
						IRenderableNode child=kvp.Value as IRenderableNode;
						
						if(child!=null){
							child.RenderData.Render(renderer);
						}
						
					}
					
				}
				
				if(FirstBox.DisplayMode!=DisplayMode.Inline){
					
					// Restore the previous boundary before this one:
					// [Note - can't use SetBoundary here as it would destroy the box.]
					renderer.ClippingBoundary=parentBoundary;
					
				}
				
			}
			
			if(customShaders!=null){
				
				// Reverse shaders:
				renderer.CurrentShaderSet=shaders;
				
				// Break the batching system:
				renderer.CurrentBatch=null;
				
			}
			
			if(transform!=null){
				// Pop it off again:
				renderer.Transformations.Pop();
			}
			
			// Restore color overlay:
			renderer.ColorOverlay = parentOverlay;
			
		}
		
		public void WentOffScreen(){
			
			if(RenderProperties==null){
				return;
			}
			
			// For each one..
			for(int p=0;p<RenderProperties.Length;p++){
				
				// Inform it:
				RenderProperties[p].WentOffScreen();
				
			}
			
		}
		
		/// <summary>Called just before reflow to update CSS.</summary>
		public virtual void UpdateCss(Renderman renderer){
			
			if(!renderer.FullReflow && IsOutOfFlow && Ready && NextUpdateMode==UpdateMode.None){
				// Do nothing!
				// - This is a partial reflow
				// - This node is isolated from parent reflows
				// - This node has boxes (is ready)/ has already been drawn
				// - This node didn't request an update
				//   (If it did request an update, it would be in the partial reflow queue and be successfully processed)
				
				// Essentially, it's happy how it is!
				return;
			}
			
			// Pool the boxes:
			renderer.PoolBoxes(FirstBox,LastBox);
			
			// Get CS:
			ComputedStyle computed=computedStyle;
			
			// Get the display mode. It may be affected by float.
			int displayMode=computed.DisplayX;
			
			if(displayMode==DisplayMode.None){
				FirstBox=null;
				LastBox=null;
				return;
			}
			
			// Apply our initial box to this elements first available box
			// (which will act as either our line box or block box)
			
			// Get the box and apply it:
			LayoutBox box=renderer.PooledBox();
			FirstBox=box;
			LastBox=box;
			
			// Compute the initial box (grabbing these computes for us).
			int positionMode=computed.PositionX;
			bool borderBox=computed.BorderBoxModel;
			BoxStyle scroll=computed.GetScrollBox();
			
			// Update ancestor based on position mode.
			switch(positionMode){
				case PositionMode.Fixed:
					
					// Nearest transformed:
					Ancestor=renderer.TransformedAncestor;
					
				break;
				case PositionMode.Absolute:
					
					// Nearest positioned:
					Ancestor=renderer.PositionedAncestor;
					
				break;
				default:
					
					// Just the nearest flow root:
					Ancestor=renderer.FlowRootAncestor;
					
				break;
			}
			
			// Resolve scale:
			ValueScale=computed.ZoomX;
			
			// Get the float mode:
			int floatMode=computed.FloatMode;
			
			// If float is set, it forces the display mode to change.
			if(floatMode!=FloatMode.None){
				
				// Force change to block, unless we're using flex:
				if((displayMode & DisplayMode.InsideFlex)!=0){
					
					// Flex - float ignored:
					floatMode=FloatMode.None;
					
				}else if((displayMode & DisplayMode.InsideTable)!=0){
					
					// Table modes - force (block) table display mode:
					displayMode=DisplayMode.Table;
					
				}else{
					
					// Force block:
					displayMode=DisplayMode.Block;
					
				}
				
			}
			
			// Positioned? If so, update nearest positioned ancestor:
			if(positionMode!=PositionMode.Static){
				renderer.PositionedAncestors.Push(this);
			}
			
			// Flow root?
			if((displayMode & DisplayMode.FlowRoot)!=0){
				renderer.FlowRootAncestors.Push(this);
			}
			
			// Same for transformed:
			Transformation transform=computed.TransformX;
			
			if(transform!=null){
				renderer.TransformedAncestors.Push(this);
			}
			
			// Resolve font-size:
			float fontSize=computed.FontSizeX;
			
			// Get the font family:
			Css.Units.FontFamilyUnit family=computed.FontFamilyX;
			
			// Build the font flags:
			int fontWeight=computed.ResolveInt(Css.Properties.FontWeight.GlobalProperty);
			int fontFlags=(fontWeight/100)<<3;
			
			// Font style:
			fontFlags |= computed.ResolveInt(Css.Properties.FontStyle.GlobalProperty);
			
			// Get the font synthesis mode:
			Css.Value synthValue=computed[Css.Properties.FontSynthesis.GlobalProperty];
			int synthMode;
			
			if(synthValue==null){
				synthMode=InfiniText.FontSynthesisFlags.All;
			}else{
				synthMode=synthValue.GetInteger(this,Css.Properties.FontSynthesis.GlobalProperty);
			}
			
			// Get a particular font face now:
			InfiniText.FontFace face=null;
			
			if(family!=null){
				face=family.GetFace(fontFlags,synthMode);
			}
			
			if(face==null){
				// Fallback:
				face=Css.Units.FontFamilyUnit.DefaultUnit.GetFace(fontFlags,synthMode);
			}
			
			// Apply all the properties now:
			box.FloatMode=floatMode;
			box.FontSize=fontSize;
			box.FontFace=face;
			box.PositionMode=positionMode;
			box.BorderBox=borderBox;
			box.Scroll=scroll;
			box.DisplayMode=displayMode;
			box.EndSpaceSize=computed.EndSpaceSize;
			
			// Table-cell and table:
			if((displayMode & DisplayMode.TableCellOrTable)!=0){
				
				// Create table meta:
				if(box.TableMeta==null){
					
					// Create it now:
					box.TableMeta=new TableMeta();
					
				}
				
				if(displayMode==DisplayMode.Table){
					
					// Set flag saying cols are not ready:
					box.TableMeta.ColumnsReady=false;
					
				}
				
				// Compute the actual columns in the Reflow pass.
				
			}else{
				
				// Clear table meta:
				box.TableMeta=null;
				
			}
			
			// Resolve overflow:
			int overflowX;
			int overflowY;
			Css.Value overflowValue=computed.Resolve(Css.Properties.Overflow.GlobalProperty);
			
			Css.Properties.Overflow.GetOverflow(overflowValue,out overflowX,out overflowY);
			
			box.OverflowX=overflowX;
			box.OverflowY=overflowY;
			
			// Set align values:
			box.Visibility=computed.VisibilityX;
			bool drawKids=true;
			
			if(Virtuals!=null){
				
				drawKids=Virtuals.AllowDrawKids;
				
				// Before zone. Anything that is non-zero.
				foreach(KeyValuePair<int,Node> kvp in Virtuals.Elements){
					
					// Reflow it:
					IRenderableNode irn=(kvp.Value as IRenderableNode);
					
					if(irn!=null){
						
						(kvp.Value as IRenderableNode).RenderData.UpdateCss(renderer);
						
					}
					
				}
				
			}
			
			// Grab a reference to childNodes
			// (we don't want to collide with some other thread)
			NodeList kidsToRender=Node.childNodes_;
			
			if(drawKids && kidsToRender!=null){
				
				for(int i=0;i<kidsToRender.length;i++){
					
					// Get as a renderable node:
					IRenderableNode node=kidsToRender[i] as IRenderableNode;
					
					if(node==null){
						
						// It's not renderable - it can potentially be a document though:
						ReflowDocument ird=kidsToRender[i] as ReflowDocument;
						
						if(ird!=null){
							
							// Update it!
							node=(ird.documentElement as IRenderableNode);
							
							if(node!=null){
								node.RenderData.UpdateCss(renderer);
							}
							
						}
						
						
					}else{
						
						node.RenderData.UpdateCss(renderer);
						
					}
					
				}
				
			}
			
			// Restore nearest positioned ancestor:
			if(positionMode!=PositionMode.Static){
				renderer.PositionedAncestors.Pop();
			}
			
			if((displayMode & DisplayMode.FlowRoot)!=0){
				renderer.FlowRootAncestors.Pop();
			}
			
			if(transform!=null){
				renderer.TransformedAncestors.Pop();
			}
			
		}
		
		/// <summary>The shrink to fit width including the elements padding etc.</summary>
		public float ShrinkToFitPadded(){
			
			LayoutBox box=FirstBox;
			
			if(box==null){
				return 0f;
			}
			
			float preferredMinimumWidth;
			float preferredWidth;
			(Node as IRenderableNode).GetWidthBounds(out preferredMinimumWidth,out preferredWidth);
			
			float extraStyle=(
				box.Border.Left+box.Border.Right+
				box.Padding.Left+box.Padding.Right+
				box.Margin.Left+box.Margin.Right
			);
			
			preferredMinimumWidth+=extraStyle;
			preferredWidth+=extraStyle;
			
			float availableWidth=ParentInnerWidth();
			
			return Math.Min(Math.Max(preferredMinimumWidth, availableWidth), preferredWidth);
			
		}
		
		/// <summary>This nodes 'shrink to fit' width. Does not include its own padding/ margin etc.</summary>
		public float ShrinkToFit(){
			
			float preferredMinimumWidth;
			float preferredWidth;
			(Node as IRenderableNode).GetWidthBounds(out preferredMinimumWidth,out preferredWidth);
			
			float availableWidth=ParentInnerWidth();
			
			return Math.Min(Math.Max(preferredMinimumWidth, availableWidth), preferredWidth);
			
		}
		
		/// <summary>Obtains a defined width for non-inline elements. (Special case for things like img though).</summary>
		public float GetWidth(bool noAuto,out bool wasAuto){
			
			if(FirstBox==null){
				wasAuto=false;
				return float.MinValue;
			}
			
			ComputedStyle computed=computedStyle;
			int displayMode=FirstBox.DisplayMode;
			int positionMode=FirstBox.PositionMode;
			
			// Get w:
			Css.Value widthValue=computed.WidthX;
			
			float width;
			
			if(widthValue==null || widthValue.IsAuto){
			
				wasAuto=true;
				
				if(noAuto){
					return float.MinValue;
				}
				
				// Auto width. A few options here depending on if we're floating or a block element.
				
				BoxStyle position=FirstBox.Position;
				BoxStyle border=FirstBox.Border;
				BoxStyle padding=FirstBox.Padding;
				BoxStyle margin=FirstBox.Margin;
				
				// If both left and right positions are declared..
				if(position.Right!=float.MaxValue && position.Left!=float.MaxValue){
					
					// Special case! Apply width like so:
					width=ParentInnerWidth() - (position.Right + position.Left);
					
					// Border-box mode. We'll need to clip the width before we remove padding/border:
					width=computed.ClipWidth(displayMode,width);
					
					// Remove padding, margin and border (this is the border-box part):
					width-=(border.Left+border.Right+padding.Left+padding.Right+margin.Left+margin.Right);
					
				}else if( 
					FirstBox.FloatMode!=0 || 
					positionMode==PositionMode.Absolute || 
					((displayMode & DisplayMode.ShrinkToFit)!=0) 
				){
					
					// Floating element, positioned or inline block. "shrink to fit" width.
					if(FirstBox.BorderBox){
						width=ShrinkToFitPadded();
					}else{
						width=ShrinkToFit();
					}
					
				}else if(displayMode==DisplayMode.Inline){
					
					// Width is always undefined:
					return float.MinValue;
					
				}else{
					
					// Act like border-box 100% if the display mode is block, normal flow (==just "Block")
					width=ParentInnerWidth();
					
					// Border-box mode. We'll need to clip the width before we remove padding/border:
					width=computed.ClipWidth(displayMode,width);
					
					// Remove padding, margin and border (this is the border-box part):
					width-=(border.Left+border.Right+padding.Left+padding.Right+margin.Left+margin.Right);
					
				}
				
			}else{
				
				wasAuto=false;
				
				width=widthValue.GetDecimal(this,Css.Properties.Width.GlobalProperty);
				
				// Clip now:
				width=computed.ClipWidth(displayMode,width);
				
				if(FirstBox.BorderBox){
					
					BoxStyle border=FirstBox.Border;
					BoxStyle padding=FirstBox.Padding;
					
					// Remove padding and border values:
					width-=border.Left+border.Right+padding.Left+padding.Right;
					
				}
				
			}
			
			return width;
			
		}
		
		/// <summary>Positions this node and all its children relative to their parent.</summary>
		public virtual void Reflow(Renderman renderer){
			
			///// PRIMARY LAYOUT SYSTEM //////
			
			// This is the most important stage of PowerUI's layout routines.
			// It computes the box model and positions it on the screen.
			
			// CSS Display Module (Level 3) forms the primary basis of this layout system.
			
			// https://www.w3.org/TR/css-display-3/#outer-role
			
			// The spec is quite complex due to the large number of ways in which
			// terms can be combined, so let's try and make this as easy to follow as possible!
			
			// First, an optimization - don't reflow this element
			// if its a flow root (not affected by its parent) and doesn't need reflowing:
			if(!renderer.FullReflow && IsOutOfFlow && Ready && NextUpdateMode==UpdateMode.None){
				// Do nothing!
				// - This is a partial reflow
				// - This node is isolated from parent reflows
				// - This node has boxes (is ready)/ has already been drawn
				// - This node didn't request an update
				//   (If it did request an update, it would be in the partial reflow queue and be successfully processed)
				
				// Essentially, it's happy how it is!
				return;
			}
			
			// Is this element actually being displayed?
			if(FirstBox==null){
				// Nope! Don't draw this element or it's kids.
				return;
			}
			
			// First, a quick reference to the computed style:
			// - It contains the inherited and applied CSS values which we'll be using.
			ComputedStyle computed=computedStyle;
			
			LayoutBox box=FirstBox;
			
			// Get the float mode:
			int floatMode=box.FloatMode;
			int positionMode=box.PositionMode;
			int displayMode=box.DisplayMode;
			
			bool heightUndefined=false;
			bool autoWidth;
			bool autoHeight=false;
			
			// Get position info:
			BoxStyle padding=computed.GetPaddingBox(displayMode);
			BoxStyle border=computed.GetBorderBox(displayMode);
			BoxStyle position=computed.GetPositionBox(positionMode);
			
			box.Position=position;
			box.Padding=padding;
			box.Border=border;
			
			// Compute the initial margin:
			bool marginAuto=false;
			box.Margin=computed.GetMarginBox(displayMode,floatMode,ref marginAuto);
			
			bool borderBox=box.BorderBox;
			
			// Next we'll compute the best-guess width and height, if at all possible:
			float width=GetWidth(false,out autoWidth);
			
			// Get the initial height:
			Css.Value heightValue=computed.HeightX;
			
			float height=0f;
			if(heightValue==null || heightValue.IsAuto){
				
				// If both top and bottom positions are declared on block/inline block
				// then we have a special case for height:
				if((displayMode & DisplayMode.BlockWidth)!=0 && position.Top!=float.MaxValue && position.Bottom!=float.MaxValue){
					
					// Special case! Apply height like so:
					height=ParentInnerHeight() - (position.Top + position.Bottom);
					
					if(borderBox){
						
						// Border-box mode. We'll need to clip the width before we remove padding/border:
						height=computed.ClipHeight(displayMode,height);
						
						// Remove padding and border (this is the border-box part):
						height-=border.Top+border.Bottom+padding.Top+padding.Bottom;
						
					}else{
						
						// Remove padding and border (this is the border-box part):
						height-=border.Top+border.Bottom+padding.Top+padding.Bottom;
					
						// Clip now:
						height=computed.ClipHeight(displayMode,height);
						
					}
					
				}else{
					// Don't know the height at this point. Make sure the renderer sets it.
					heightUndefined=true;
				}
				
				autoHeight=true;
				
			}else{
				
				height=heightValue.GetDecimal(this,Css.Properties.Height.GlobalProperty);
				
				// Clip now:
				height=computed.ClipHeight(displayMode,height);
				
				if(borderBox){
					
					// Remove padding and border values:
					height-=border.Top+border.Bottom+padding.Top+padding.Bottom;
					
				}
				
			}
			
			bool widthUndefined=(width==float.MinValue);
			
			// Ensure w/h hasn't gone negative.
			if(width<0f){
				width=0f;
			}
			
			if(height<0f){
				height=0f;
			}
			
			box.InnerWidth=width;
			box.InnerHeight=height;
			
			// Important note: "Position" may contain MaxValue entries at this stage.
			// This is expected; it is used to spot when a top/right/left/bottom value is defined or not
			// which is vital information for the global positioning of the element.
			
			// Get the z-index value:
			Css.Value zIndex=computed.ZIndexX;
			bool fixedDepth=(zIndex!=null && !zIndex.IsAuto);
			
			// Let the tag know that we're now setting up the box:
			(Node as IRenderableNode).OnComputeBox(renderer,box,ref autoWidth,ref autoHeight);
			
			// Update undefined:
			if(!autoWidth){
				widthUndefined=false;
			}
			
			if(!autoHeight){
				heightUndefined=false;
			}
			
			// Compute initial width/height now:
			box.SetDimensions(widthUndefined,heightUndefined);
			
			// Compute the final margin (which often requires knowing the dimensions):
			if(marginAuto){
				box.Margin=computed.GetMarginBox(displayMode,floatMode,ref marginAuto);
			}
			
			float depth=renderer.Depth;
			bool elementPositioned=false;
			
			if(!fixedDepth){
				
				elementPositioned=(box.IsOffset() || (positionMode & PositionMode.InFlow)==0);
				
				if(elementPositioned){
					// This element has been positioned - make sure it's on top of the current highest element:
					renderer.Depth=renderer.MaxDepth;
					
					if(renderer.DepthUsed){
						renderer.IncreaseDepth();
					}else{
						renderer.DepthUsed=true;
					}
					
					computed.ZIndex=renderer.Depth;
					
				}else{
					computed.ZIndex=depth;
				}
				
			}else{
				computed.ZIndex=zIndex.GetDecimal(this,Css.Properties.ZIndex.GlobalProperty) * renderer.DepthResolution;
			}
			
			// Add it to the parent:
			LineBoxMeta topOfStack=renderer.TopOfStackSafe;
			bool inFlow=(positionMode & PositionMode.InFlow)!=0;
			
			// Start the lines (must always do this, even if there's no kids):
			LineBoxMeta lineZone=renderer.BeginLines(this,Virtuals,box,autoWidth);
			renderer.BoxStack.Push(lineZone);
			
			// If width/height are not defined then set them to -1:
			if(widthUndefined){
				box.InnerWidth=-1f;
			}
			
			if(heightUndefined){
				box.InnerHeight=-1f;
			}
			
			// Flush a transform unit now if there is one:
			computed.TransformUpdate();
			
			// If we have a table (not a table cell), 
			// then we'll now go ahead and compute the columns.
			if(box.TableMeta!=null){
				
				if(box.DisplayMode==DisplayMode.Table){
					
					// Calculate the columns:
					box.TableMeta.CalculateColumns(this);
					
				}else{
					// Table cell - apply the column width now!
					
					// Get the table and the column that we are:
					TableMeta hostTable=box.TableMeta.Table;
					int colID=box.TableMeta.StartColumn;
					
					// Make sure everything is available:
					if(hostTable!=null && hostTable.Columns!=null && colID<hostTable.Columns.Count){
						
						// Get the column information:
						TableColumnMeta columnMeta=hostTable.Columns[colID];
						
						// Apply width (the defined width must have padding etc taken from it):
						float innerWidth=columnMeta.Width - (box.Padding.Left+box.Padding.Right+box.Border.Right+box.Border.Left);
						
						box.Width=columnMeta.Width;
						box.InnerWidth=innerWidth;
						
					}
					
				}
				
			}
			
			if(topOfStack!=null){
				
				BlockBoxMeta bbm=(topOfStack as BlockBoxMeta);
				
				if(
					floatMode==FloatMode.None && 
					inFlow && 
					(topOfStack.WhiteSpace & WhiteSpaceMode.Wrappable) !=0 &&
					(
						((displayMode&DisplayMode.OutsideBlock)!=0) || ( box.Width>(topOfStack.MaxX-topOfStack.PenX) )
					)
				){
					
					// Break before a block element.
					topOfStack.CompleteLine(LineBreakMode.Normal);
					
					// Clear?
					int clearMode=computed.ResolveInt(Css.Properties.Clear.GlobalProperty);
					
					if(clearMode!=0){
						
						// Increase PenY to clear left/right/both.
						topOfStack.ClearFloat(clearMode);
						
					}
					
				}
				
				if(bbm!=null){
					
					// Has it got a prev margin? Must be in-flow, not floating and 
					if(bbm.PreviousMargin!=0f){
						
						if(floatMode==FloatMode.None && inFlow && ((displayMode&DisplayMode.OutsideBlock)!=0)){
							
							// Get 'my' margin:
							float other=box.Margin.Top;
							
							if(other!=0f){
								
								// Collapse the top margin.
								
								if(other<0f){
									
									if(bbm.PreviousMargin<0f){
										
										// Both negative - the smallest one wins.
										
										if(other>bbm.PreviousMargin){
											
											box.Margin.Top=0f;
									
										}else{
											
											box.Margin.Top-=bbm.PreviousMargin;
											
										}
										
									}else{
										
										box.Margin.Top=0f;
									
									}
									
								}else if(bbm.PreviousMargin<0f){
									
									// Deduct it from the positive margin:
									box.Margin.Top+=bbm.PreviousMargin;
									
								}else if(other>bbm.PreviousMargin){
									
									box.Margin.Top-=bbm.PreviousMargin;
									
								}else{
									
									box.Margin.Top=0f;
									
								}
								
							}
						
						}
						
						// Clear it:
						bbm.PreviousMargin=0f;
						
					}
					
				}
				
				// Add the node to the current line:
				topOfStack.AddToLine(box);
				
			}
			
			// Grab a reference to childNodes
			// (we don't want to collide with some other thread)
			NodeList kidsToRender=Node.childNodes_;
			
			if(Virtuals!=null && !Virtuals.AllowDrawKids){
				kidsToRender=null;
			}
			
			if(kidsToRender!=null || Virtuals!=null){
				
				if(fixedDepth){
					// Set the depth buffer to this element so it's kids are at the right height; restore it after.
					
					// Offset by the document's depth if we're in a document in a document (e.g. iframe):
					
					if(Node.parentNode==Document && Document.parentNode_!=null){
						
						IRenderableNode irn=(Document.parentNode_ as IRenderableNode);
						
						if(irn!=null){
							computed.ZIndex+=irn.ComputedStyle.ZIndex;
						}
						
					}
					
					renderer.Depth=computed.ZIndex;
					
					if(computed.ZIndex>renderer.MaxDepth){
						renderer.MaxDepth=renderer.Depth;
					}
					
				}else if(HasBackground){
					// Only increase the depth if the element has a background image/colour to get it's kids away from.
					renderer.IncreaseDepth();
				}
				
				// An iterator through the virtual elements:
				IEnumerator<KeyValuePair<int,Node>> virtualIterator=null;
				
				if(Virtuals!=null){
					
					// Get an iterator:
					virtualIterator=Virtuals.Elements.GetEnumerator();
					
					// Before zone. Anything that is non-zero.
					while(virtualIterator.MoveNext()){
						
						// Get the current entry:
						KeyValuePair<int,Node> kvp=virtualIterator.Current;
					
						if(kvp.Key>=0){
							// Stop there.
							break;
						}
						
						// Reflow it:
						if(kvp.Value is SparkInformerNode){
							
							// Special informer node:
							SparkInformerNode informer=(kvp.Value as SparkInformerNode);
							
							// Start it:
							informer.Start(renderer);
							
						}else{
							(kvp.Value as IRenderableNode).RenderData.Reflow(renderer);
						}
						
					}
					
				}
				
				if(kidsToRender!=null){
					
					for(int i=0;i<kidsToRender.length;i++){
						
						// Get as a renderable node:
						IRenderableNode node=kidsToRender[i] as IRenderableNode;
						
						if(node==null){
							
							// It's not renderable - it can potentially be a document though:
							ReflowDocument ird=kidsToRender[i] as ReflowDocument;
							
							if(ird!=null){
								
								// Reflow it!
								node=(ird.documentElement as IRenderableNode);
								
							}
							
							if(node==null){
								continue;
							}
							
						}
						
						node.RenderData.Reflow(renderer);
						
					}
					
				}
				
				if(Virtuals!=null){
					
					// Get the current entry:
					KeyValuePair<int,Node> kvp=virtualIterator.Current;
					
					if(kvp.Key>=0 && kvp.Value!=null){
						
						// Reflow it:
						(kvp.Value as IRenderableNode).RenderData.Reflow(renderer);
						
						// There's possibly more too.
						
						// Before zone. Anything that is non-zero.
						while(virtualIterator.MoveNext()){
							
							// Get the current entry:
							kvp=virtualIterator.Current;
							
							// Reflow it:
							(kvp.Value as IRenderableNode).RenderData.Reflow(renderer);
							
						}
						
					}
					
					if(renderer.FirstInformer!=null){	
						
						// End them all:
						SparkInformerNode informer=renderer.FirstInformer;
						renderer.FirstInformer=null;
						
						while(informer!=null){
							
							// End:
							informer.End(renderer);
							
							informer=informer.NextInformer;
							
						}
						
					}
					
				}
				
			}
			
			// Update max depth:
			computed.MaxZIndex=renderer.MaxDepth;
			
			// Finish the lines:
			renderer.EndLines(lineZone,computed,box);
			
			if(elementPositioned){
				// This element has been positioned - make sure it's on top of the current highest element:
				renderer.Depth=renderer.MaxDepth;
				
				if(renderer.DepthUsed){
					renderer.IncreaseDepth();
				}else{
					renderer.DepthUsed=true;
				}
				
			}
			
			// May now need to recompute margin:auto (it would've acted like '0' until now).
			
			if(topOfStack!=null && ((displayMode&DisplayMode.OutsideBlock)!=0) && floatMode==FloatMode.None && inFlow){
				
				// A second break after the block too.
				topOfStack.CompleteLine(LineBreakMode.Normal);
				
				// Take note of the margin (for margin collapsing):
				BlockBoxMeta bbm=(topOfStack as BlockBoxMeta);
				
				if(bbm!=null){
					bbm.PreviousMargin=box.Margin.Bottom;
				}
				
			}
			
			// --- We now know the entire box (as we've also computed all of its contents and set the height) ---
			
		}
		
		/// <summary>Refreshes this elements css style if the given selector matches its own.</summary>
		/// <param name="type">The type of the given selector.</param>
		/// <param name="rule">The selector to match with.</param>
		public void RuleRemoved(Css.StyleRule rule){
			
			ComputedStyle computed=computedStyle;
			
			MatchingRoot match=computed.FirstMatch;
			
			while(match!=null){
				
				if(match.Rule==rule){
					
					// Remove this specific one:
					match.Selector.Remove();
					
					break;
				}
				
				match=match.NextInStyle;
			}
			
			if(Node.childNodes_!=null){
				
				for(int i=0;i<Node.childNodes_.length;i++){
					
					// Renderable only:
					IRenderableNode el=(Node.childNodes_[i] as IRenderableNode);
					
					if(el==null || !(el is Element)){
						continue;
					}
					
					// Call removed:
					el.RenderData.RuleRemoved(rule);
					
				}
				
			}
			
		}
		
		/// <summary>Refreshes this elements css style if the given selector matches its own.</summary>
		/// <param name="type">The type of the given selector.</param>
		/// <param name="rule">The selector to match with.</param>
		public void RuleAdded(Css.StyleRule rule){
			
			// Try matching this element:
			computedStyle.TryApplyLate(rule);
			
			// Pass on to the kids:
			if(Node.childNodes_!=null){
				
				for(int i=0;i<Node.childNodes_.length;i++){
					
					IRenderableNode el=(Node.childNodes_[i] as IRenderableNode);
					
					// Must be an element (with a unique computed style):
					if(el==null || !(el is Element)){
						continue;
					}
					
					el.RenderData.RuleAdded(rule);
				}
				
			}
			
		}
		
	}
	
}
	