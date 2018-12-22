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
using Blaze;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Svg;
using Css;
using PowerUI;


namespace Svg{
	
	/// <summary>
	/// A base class for all SVG tag types. This is used to distictively identify them.
	/// </summary>
	
	[SVGNamespace]
	[Dom.TagName("Default")]
	public class SVGElement:Element, IRenderableNode{
		
		/// <summary>This elements style.</summary>
		public ElementStyle Style;
		/// <summary>The render context that this element belongs to.</summary>
		internal RenderContext Context;
		
		/// <summary>Objects fill transparent by default. This is the fill handler. Initiated when first required.</summary>
		private static Loonim.TextureNode _transparentFill=null;
		
		/// <summary>Objects fill transparent by default. This is the fill handler. Initiated when first required.</summary>
		public static Loonim.TextureNode TransparentFill{
			get{
				
				if(_transparentFill==null){
					
					// White transparent fill:
					_transparentFill=new Loonim.Property(new UnityEngine.Color(0f,0f,0f,0f));
					
				}
				
				return _transparentFill;
			}
		}
		
		public SVGElement(){
			Style=new ElementStyle(this);
		}
		
		/// <summary>This nodes computed style.</summary>
		public ComputedStyle ComputedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Called when this element goes offscreen.</summary>
		public void WentOffScreen(){
			
			RenderableData renderable=RenderData;
			renderable.WentOffScreen();
			
			// Apply to all virtual elements:
			VirtualElements virts=renderable.Virtuals;
			
			if(virts!=null){
				
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
				
					// Tell it that it's gone offscreen:
					IRenderableNode irn=(kvp.Value as IRenderableNode);
					
					if(irn!=null){
						irn.WentOffScreen();
					}
					
				}
				
			}
			
			if(childNodes_!=null){
				
				for(int i=0;i<childNodes_.length;i++){
					
					// Get as a HTML node:
					IRenderableNode htmlNode=(childNodes_[i] as IRenderableNode);
					
					if(htmlNode==null){
						return;
					}
					
					// Call offscreen:
					htmlNode.WentOffScreen();
					
				}
				
			}
			
		}
		
		public void OnRender(Renderman renderer){}
		
		/// <summary>Part of shrink-to-fit. Computes the maximum and minimum possible width for an element.
		/// This does not include the elements own padding/margin/border.</summary>
		public void GetWidthBounds(out float min,out float max){
			
			min=0f;
			max=0f;
			
			// For each child, get its width bounds too.
			if(RenderData.FirstBox==null){
				return;
			}
			
			if(childNodes_!=null){
				
				// Current line:
				float cMin=0f;
				float cMax=0f;
				
				for(int i=0;i<childNodes_.length;i++){
					
					Node child=childNodes_[i];
					
					IRenderableNode renderable=(child as IRenderableNode);
					
					if(renderable==null){
						continue;
					}
					
					float bMin;
					float bMax;
					
					if(child is PowerUI.RenderableTextNode){
						
						// Always get bounds:
						renderable.GetWidthBounds(out bMin,out bMax);
						
					}else{
						
						// Get the first box from the render data:
						RenderableData rd=renderable.RenderData;
						LayoutBox box=rd.FirstBox;
						
						if(box==null){
							continue;
						}
						
						// If it's inline (or float) then it's additive to the current line.
						if((box.DisplayMode & DisplayMode.OutsideBlock)!=0 && box.FloatMode==FloatMode.None){
							
							// Line break!
							cMin=0f;
							cMax=0f;
							
						}
						
						// Get an explicit width:
						bool wasAuto;
						bMin=rd.GetWidth(true,out wasAuto);
							
						if(bMin==float.MinValue){
							
							// Get the bounds:
							renderable.GetWidthBounds(out bMin,out bMax);
							
						}else{
							
							bMax=bMin;
							
						}
						
						// Add margins etc:
						float extraStyle=(
							box.Border.Left+box.Border.Right+
							box.Padding.Left+box.Padding.Right+
							box.Margin.Left+box.Margin.Right
						);
						
						bMin+=extraStyle;
						bMax+=extraStyle;
						
					}
					
					// Apply to line:
					cMin+=bMin;
					cMax+=bMax;
					
					// Longest line?
					if(cMin>min){
						min=cMin;
					}
					
					if(cMax>max){
						max=cMax;
					}
					
				}
				
			}
			
		}
		
		/// <summary>Gets the first element which matches the given selector.</summary>
		public Element querySelector(string selector){
			HTMLCollection results=querySelectorAll(selector,true);
			
			if(results==null || results.length==0){
				return null;
			}
			
			return results[0] as Element;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector){
			return querySelectorAll(selector,false);
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selector">The selector string to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public HTMLCollection querySelectorAll(string selector,bool one){
		
			// Create results set:
			HTMLCollection results=new HTMLCollection();
			
			if(string.IsNullOrEmpty(selector)){
				// Empty set:
				return results;
			}
			
			// Create the lexer:
			Css.CssLexer lexer=new Css.CssLexer(selector,this);
			
			// Read a value:
			Css.Value value=lexer.ReadValue();
			
			// Read the selectors from the value:
			List<Selector> selectors=new List<Selector>();
			Css.CssLexer.ReadSelectors(null,value,selectors);
			
			// Create a blank event to store the targets, if any:
			CssEvent e=new CssEvent();
			
			// Perform the selection process:
			querySelectorAll(selectors.ToArray(),results,e,false);
			
			return results;
		}
		
		/// <summary>Gets all child elements with the given tag.</summary>
		/// <param name="selectors">The selectors to match.</param>
		/// <returns>The set of all tags with this tag.</returns>
		public void querySelectorAll(Selector[] selectors,INodeList results,CssEvent e,bool one){
			if(childNodes_==null){
				return;
			}
			
			for(int i=0;i<childNodes_.length;i++){
				Node node=childNodes_[i];
				Element child=node as Element;
				IRenderableNode irn=(child as IRenderableNode);
				
				if(child==null || irn==null){
					continue;
				}
				
				ComputedStyle cs=irn.ComputedStyle;
				
				for(int s=0;s<selectors.Length;s++){
					
					// Match?
					if(selectors[s].StructureMatch(cs,e)){
						// Yep!
						results.push(node);
						
						if(one){
							return;
						}
					}
					
				}
				
				irn.querySelectorAll(selectors,results,e,one);
				
				if(one && results.length==1){
					return;
				}
			}
			
		}
		
		/// <summary>This nodes render data.</summary>
		public RenderableData RenderData{
			get{
				return Style.Computed.RenderData;
			}
		}
		
		/// <summary>This elements style.</summary>
		public override ElementStyle style{
			get{
				return Style;
			}
		}
		
		/// <summary>Gets the computed style of this element.</summary>
		public Css.ComputedStyle computedStyle{
			get{
				return Style.Computed;
			}
		}
		
		/// <summary>Called during the box compute process. Useful if your element has clever dimensions, such as the img tag or words.</summary>
		public virtual void OnComputeBox(Renderman renderer,Css.LayoutBox box,ref bool widthUndefined,ref bool heightUndefined){
			
		}
		
		/// <summary>Called when a font-face is ready.</summary>
		public void FontLoaded(DynamicFont font){
			
		}
		
		/// <summary>Gets the value of the given property as a float.</summary>
		public float GetFloatAttribute(string property,float dflt){
			
			float result;
			string propValue=getAttribute(property);
			
			if(propValue==null || !float.TryParse(propValue,out result)){
				
				return dflt;
				
			}
			
			return result;
			
		}
		
		/// <summary>The parent font element, if there is one.</summary>
		public SVGFontElement ParentFont{
			get{
				// Go up the element hierarchy looking for a <font> element.
				SVGElement current=this;
				
				while(current!=null){
					
					SVGFontElement font=(current as SVGFontElement);
					
					if(font!=null){
						// Got it!
						return font;
					}
					
					current=current.parentNode as SVGElement;
				}
				
				return null;
				
			}
		}
		
		/// <summary>Finds the element by ID stored in this elements href attribute.</summary>
		public SVGElement TryResolveHref(){
			
			// ID to find:
			string id=getAttribute("href");
			
			if(string.IsNullOrEmpty(id)){
				// Nope!
				return null;
			}
			
			// Try getting it now:
			return document_.getElementById(id) as SVGElement;
			
		}
		
		/// <summary>The combined bounds of the child nodes.</summary>
		public BoxRegion GroupBounds{
			get{
				BoxRegion r = new BoxRegion();
				
				if(childNodes_==null){
					return r;
				}
				
				bool first=true;
				
				foreach(Node c in childNodes_){
					
					// Get it as svg:
					SVGElement handler=c as SVGElement;
					
					if(handler==null){
						continue;
					}
					
					// First one?
					
					if (first){
						
						// Yep! Straight set bounds:
						first=false;
						
						r = handler.Bounds;
						
					}else{
						
						BoxRegion childBounds = handler.Bounds;
						
						if (childBounds.IsNotEmpty){
							
							// Add it in:
							r.Combine(childBounds);
							
						}
					}
					
				}
				
				return r;
			}
		}
		
		/// <summary>The bounds of this element.</summary>
		public virtual BoxRegion Bounds{
			get{
				return style.Computed.GetBounds();
			}
		}
		
		public virtual void BuildFilter(RenderContext ctx){
			
			BuildFilter(ctx,true);
			
		}
		
		/// <summary>
		/// Gets or sets the fill.
		/// </summary>
		/// <value>The fill.</value>
		public virtual Loonim.TextureNode Fill{
			get{
				
				TextureNodeValue psv=ComputedStyle.Resolve(Css.Properties.Fill.GlobalProperty) as TextureNodeValue;
				
				if(psv==null){
					return null;
				}
				
				return psv.TextureNode;
				
			}
			set{}
		}
		
		public virtual Loonim.TextureNode Stroke{
			get{
				
				TextureNodeValue psv=ComputedStyle.Resolve(Css.Properties.Stroke.GlobalProperty) as TextureNodeValue;
				
				if(psv==null){
					return null;
				}
				
				return psv.TextureNode;
				
			}
			set{}
		}
		
		/// <summary>The VisibilityMode of this element.</summary>
		public int Visibility{
			get{
				
				// Get the value as an int:
				return ComputedStyle.ResolveInt(Css.Properties.Visibility.GlobalProperty);
				
			}
		}
		
		/// <summary>Renders the child nodes of this tag.</summary>
		protected void BuildChildren(RenderContext renderer){
			
			// Render child nodes as well:
			if(childNodes_==null){
				return;
			}
			
			foreach(Node child in childNodes_){
				
				// Render it too:
				SVGElement svg=(child as SVGElement);
				
				if(svg==null){
					continue;
				}
				
				svg.BuildFilter(renderer);
				
			}
			
		}
		
		protected virtual void PopTransforms(RenderContext renderer){
			
			renderer.PopTransform(this);
			
		}
		
		protected virtual bool PushTransforms(RenderContext renderer){
			
			renderer.PushTransform(this);
			return true;
			
		}
		
		private void BuildFilter(RenderContext renderer,bool withCssFilter){
			
			// Get a path to draw:
			VectorPath path=GetPath(this,renderer);
			
			if (Visibility==VisibilityMode.Hidden || !PushTransforms(renderer)){
				return;
			}
			
			if (withCssFilter){
				
				// Get the filter CSS property:
				Css.Value filterValue=style.Computed[Css.Properties.Filter.GlobalProperty];
				
				if(filterValue!=null){
					
					string filterPath=filterValue.Text;
					
					Element filter = document.getElementById(filterPath) as SVGFilterElement;
					
					if(filter != null){
						PopTransforms(renderer);
						
						// filter.ApplyFilter(this, renderer, (r) => this.BuildFilter(r, false));
						
						// Don't render normally.
						return;
						
					}
					
				}
				
			}
			
			SetClip(renderer);
			
			if(path!=null){
				
				BuildFill(path,renderer);
				BuildStroke(path,renderer);
				
			}else{
				BuildChildren(renderer);
			}
			
			ResetClip(renderer);
			PopTransforms(renderer);
			
		}
		
		internal override void AddedToDOM(){
			
			Css.ReflowDocument doc=document as Css.ReflowDocument;
			
			if(doc!=null){
				
				if(doc.AttributeIndex!=null){
					// Index element if needed:
					AddToAttributeLookups();
				}
				
				// Make sure the stylesheet is present:
				doc.RequireStyleSheet(this);
				
			}
			
			// Update its css by telling it the parent changed.
			// This affects inherit, height/width etc.
			Style.Computed.ParentChanged();
			
			// Obtain the render context:
			SVGElement svgParent=(parentNode as SVGElement);
			
			if(svgParent!=null){
				
				if(Context==null){
					// Get the context:
					Context=svgParent.Context;
				}
				
				if(Context!=null){
					// Request a rebuild:
					Context.RequestRebuild();
				}
				
			}
			
		}
		
		/// <summary>Called when this element got removed from the DOM.</summary>
		internal override void RemovedFromDOM(){
			
			Css.ReflowDocument reflowDocument=document as Css.ReflowDocument;
			
			if(reflowDocument.AttributeIndex!=null){
				
				// Remove this from the DOM attribute cache:
				reflowDocument.RemoveCachedElement(this);
				
			}
			
			// Remove handler:
			// OnRemovedFromDOM();
			
			// Let the style know we went offscreen:
			RenderableData renderable=RenderData;
			renderable.WentOffScreen();
			
			// Apply to all virtual elements:
			VirtualElements virts=renderable.Virtuals;
			
			if(virts!=null){
			
				foreach(KeyValuePair<int,Node> kvp in virts.Elements){
				
					// Remove it:
					kvp.Value.RemovedFromDOM();
					
				}
				
			}
			
			base.RemovedFromDOM();
			
		}
		
		public override bool OnAttributeChange(string property){
			
			// Global CSS properties. Width, height, fill, stroke, visibility, font-family,font-style etc are handled here.
			Css.CssProperty cssProperty=Css.CssProperties.Get(property);
			
			// Style refresh:
			if(Style.Computed.FirstMatch!=null){
				// This is a runtime attribute change.
				// We must consider if it's affecting the style or not:
				Style.Computed.AttributeChanged(property);
			}
			
			if(cssProperty!=null){
				
				// It's a CSS property! Apply to style:
				Css.Value value=ValueHelpers.Get(getAttribute(property));
				style[cssProperty]=value;
				
			}else if(property=="x"){
				
				style.left=getAttribute("x");
				
			}else if(property=="y"){
				
				style.top=getAttribute("y");
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
		}
		
		/// <summary>Resolves a float value.</summary>
		public float GetDecimal(Css.CssProperty property){
			return style.Computed.ResolveDecimal(property);
		}
		
		/// <summary>Resolves an integer from the computed style.</summary>
		public int GetInt(Css.CssProperty property){
			return style.Computed.ResolveInt(property);
		}
		
		/// <summary>The parent svg element.</summary>
		public SVGElement ownerSVGElement{
			get{
				
				// Get the tag:
				SVGSVGElement tag=this as SVGSVGElement;
				
				if(tag==null){
					
					// Get the next up svg element:
					SVGElement svgHandler=parentElement as SVGElement;
					
					if(svgHandler==null){
						// Not in an SVG doc.
						return null;
					}
					
					return svgHandler.ownerSVGElement;
					
				}
				
				// Got it!
				return tag;
				
			}
		}
		
        /// <summary>
		/// Renders the fill of the <see cref="SvgVisualElement"/> to the specified <see cref="RenderContext"/>
		/// </summary>
		protected internal virtual void BuildFill(VectorPath path,RenderContext renderer)
		{
			
			// Get the fill:
			Loonim.TextureNode fill=Fill;
			
			if(fill==null){
				// Fill isn't active.
				return;
			}
			
			// Get as a property:
			Loonim.Property prop=(fill as Loonim.Property);
			
			if(prop!=null){
				// Check if it's transparent:
				if(prop.Colour.a==0f){
					return;
				}
			}
			
			ApplyOpacity(FillOpacity,prop,ref fill);
			
			// Fill now!
			renderer.FillPath(fill,path,(FillRule=="evenodd"));
			
		}
		
		/// <summary>Applies an opacity value to a Loonim node.</summary>
		private void ApplyOpacity(float opacity,Loonim.Property prop,ref Loonim.TextureNode fill){
			
			opacity=(float)Math.Min(Math.Max(opacity * Opacity, 0), 1);
			
			if(opacity==1f){
				return;
			}
			
			// If the value is simply a property, apply opacity straight to that:
			
			if(prop==null){
				
				// Must create a multiply by..
				Color mult=new Color(1f,1f,1f,opacity);
				
				fill=new Loonim.Multiply(
					fill,
					new Loonim.Property(mult)
				);
				
			}else{
				
				// Must overwrite (because this can be called multiple times):
				prop.Colour.a=opacity;
				
			}
			
		}
		
		/// <summary>Renders a stroke with markers.</summary>
		protected bool BuildStrokeMarkers(VectorPath path,RenderContext renderer){
			
			// Default (base) stroke first:
			bool result = BuildDefaultStroke(path,renderer);
			
			/*
			if (MarkerStart != null){
				SvgMarker marker = this.OwnerDocument.GetElementById<SvgMarker>(this.MarkerStart.ToString());
				marker.RenderMarker(renderer, this, path.PathPoints[0], path.PathPoints[0], path.PathPoints[1]);
			}

			if (MarkerMid != null){
				SvgMarker marker = this.OwnerDocument.GetElementById<SvgMarker>(this.MarkerMid.ToString());
				for (int i = 1; i <= path.PathPoints.Length - 2; i++)
					marker.RenderMarker(renderer, this, path.PathPoints[i], path.PathPoints[i - 1], path.PathPoints[i], path.PathPoints[i + 1]);
			}

			if (MarkerEnd != null){
				SvgMarker marker = this.OwnerDocument.GetElementById<SvgMarker>(this.MarkerEnd.ToString());
				marker.RenderMarker(renderer, this, path.PathPoints[path.PathPoints.Length - 1], path.PathPoints[path.PathPoints.Length - 2], path.PathPoints[path.PathPoints.Length - 1]);
			}
			*/
			
			return result;
			
		}
		
		/// <summary>
		/// Renders the stroke of the <see cref="SvgVisualElement"/> to the specified <see cref="RenderContext"/>
		/// </summary>
		/// <param name="renderer">The <see cref="RenderContext"/> object to render to.</param>
		protected internal virtual bool BuildStroke(VectorPath path,RenderContext renderer){
			
			// Default stroke:
			return BuildDefaultStroke(path,renderer);
			
		}
		
		private bool BuildDefaultStroke(VectorPath path,RenderContext renderer){
			
			Loonim.TextureNode stroke=Stroke;
			
			if(stroke==null || path.FirstPathNode==null){
				return false;
			}
			
			// Get as a property:
			Loonim.Property prop=(stroke as Loonim.Property);
			
			if(prop!=null){
				// Check if it's transparent:
				if(prop.Colour.a==0f){
					return false;
				}
			}
			
			float strokeWidth = StrokeWidth;
			
			if(strokeWidth<=0){
				return false;
			}
			
			ApplyOpacity(StrokeOpacity,prop,ref stroke);
			
			if(path.Width==0f){
				path.RecalculateBounds();
			}
			
			// Stroke now!
			renderer.StrokePath(stroke,path,strokeWidth,this);
			
			// Ok:
			return true;
			
		}
		
		/// <summary>
		/// Gets the path this tag represents.
		/// Note that it can potentially be a clipping path.
		/// </summary>
		public virtual VectorPath GetPath(SVGElement context,RenderContext renderer){
			
			return null;
			
		}
		
		/// <summary>
		/// Sets the clipping region of the specified <see cref="RenderContext"/>.
		/// </summary>
		/// <param name="renderer">The <see cref="RenderContext"/> to have its clipping region set.</param>
		protected virtual void SetClip(RenderContext renderer){
			
			Css.Value clip=ClipPath;
			
			if(clip == null){
				return;
			}
			
			_previousClip = renderer.ClipRegion;
			
			// Get the shape from the clip value:
			Css.Value nextValue;
			ShapeProvider shape=clip.ToShape(this,renderer,out nextValue);
			
			if(clip!=nextValue){
				// Overwrite the raw clip var:
				ClipPath=nextValue;
			}
			
			if(shape!=null){
				
				// Get the region now:
				renderer.SetClip(shape.GetRegion(this,renderer), false);
				
			}
			
		}
		
		/// <summary>Adds all child paths together.</summary>
		protected VectorPath GetPaths(Node parent,RenderContext renderer){
			
			// Create:
			VectorPath bakeInto=new VectorPath();
			
			// Add:
			AddChildPaths(parent,bakeInto,renderer,Matrix4x4.identity,false);
			
			return bakeInto;
			
		}
		
		/// <summary>Adds all child paths together.</summary>
		protected void AddChildPaths(Node parent,VectorPath bakeInto,RenderContext renderer,Matrix4x4 extraTransform,bool applyExtra){
			
			if(parent.childNodes_==null){
				return;
			}
			
			foreach(Node child in parent.childNodes_){
				
				SVGGeometryElement pathBase=child as SVGGeometryElement;
				
				// Got a path?
				if(pathBase!=null){
					
					// Yep! Get the path itself:
					VectorPath path=pathBase.GetPath(pathBase,renderer);
					
					if(path!=null){
						
						// Copy it:
						path=path.CopyPath();
						
						// Push transform:
						pathBase.PushTransforms(renderer);
						
						// Get the resolved matrix:
						Matrix4x4 transform=renderer.Transform.Matrix;
						
						// Apply extra transform:
						if(applyExtra){
							transform*=extraTransform;
						}
						
						// Pop it again:
						pathBase.PopTransforms(renderer);
						
						// Transform the path:
						path.Transform(transform);
						
						// Add into bakeInto:
						bakeInto.Append(path);
						
					}
					
				}
				
				// Apply to this child too:
				AddChildPaths(child,bakeInto,renderer,extraTransform,applyExtra);
				
			}
			
		}
		
		/// <summary>
		/// Resets the clipping region of the specified <see cref="RenderContext"/> back to where it was before the <see cref="SetClip"/> method was called.
		/// </summary>
		/// <param name="renderer">The <see cref="RenderContext"/> to have its clipping region reset.</param>
		protected virtual void ResetClip(RenderContext renderer){
			
			if (_previousClip == null){
				return;
			}
			
			renderer.ClipRegion=_previousClip;
			_previousClip = null;
			
		}
		
		private ScreenRegion _previousClip;
		
		/// <summary>
		/// Gets the associated <see cref="SvgClipPath"/> if one has been specified.
		/// </summary>
		public virtual Css.Value ClipPath{
			get { return style.Computed["clip-path"]; }
			set { style["clip-path"] = value; }
		}
		
		/// <summary>
		/// Gets or sets the algorithm which is to be used to determine the clipping region.
		/// </summary>
		public string ClipRule{
			get { return style.Computed["clip-rule"].Text; }
			set { style["clip-rule"] = Css.Value.Load(value); }
		}
		
		/// <summary>
		/// Gets the associated <see cref="SvgClipPath"/> if one has been specified.
		/// </summary>
		public virtual Css.Value Filter{
			get { return style.Computed["clip-rule"]; }
			set { style["clip-rule"] = value; }
		}
		
		/// <summary>Element opacity.</summary>
        public virtual float Opacity{	
			get{
				return 1f;// GetDecimal(Css.Properties.Opacity.GlobalProperty);
			}
		}
		
		/// <summary>Fill opacity.</summary>
        public float FillOpacity{	
			get{
				return GetDecimal(Css.Properties.FillOpacity.GlobalProperty);
			}
		}
		
		/// <summary>Stroke opacity.</summary>
        public float StrokeOpacity{	
			get{
				return GetDecimal(Css.Properties.StrokeOpacity.GlobalProperty);
			}
		}
		
		/// <summary>Stroke width.</summary>
		public float StrokeWidth{	
			get{
				return GetDecimal(Css.Properties.StrokeWidth.GlobalProperty);
			}
		}
		
		/// <summary>Stroke line cap.</summary>
		public int StrokeLineCap{	
			get{
				return GetInt(Css.Properties.StrokeLineCap.GlobalProperty);
			}
		}
		
		/// <summary>Stroke line join.</summary>
		public int StrokeLineJoin{	
			get{
				return GetInt(Css.Properties.StrokeLineJoin.GlobalProperty);
			}
		}
		
		/// <summary>Stroke dash offset.</summary>
		public Css.Value StrokeDashOffset{	
			get{
				return style.Computed[Css.Properties.StrokeDashOffset.GlobalProperty];
			}
		}
		
		/// <summary>Stroke dash array.</summary>
		public Css.Value StrokeDashArray{	
			get{
				return style.Computed[Css.Properties.StrokeDashArray.GlobalProperty];
			}
		}
		
		/// <summary>Stroke-miterlimit.</summary>
		public float StrokeMiterLimit{	
			get{
				return GetDecimal(Css.Properties.StrokeMiterLimit.GlobalProperty);
			}
		}
		
		/// <summary>Fill rule (e.g. nonzero).</summary>
        public string FillRule{
			get{
				
				Css.Value fillRule=style.Computed["fill-rule"];
				
				if(fillRule==null){
					return "nonzero";
				}
				
				return fillRule.Text;
			}
		}
		
	}
	
}