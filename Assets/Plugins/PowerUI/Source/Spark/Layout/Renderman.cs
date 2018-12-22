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
using Css;
using Blaze;
using Dom;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// This helps 'render' elements from the DOM into a set of 3D meshes.
	/// It also performs things such as alignment and line packing.
	/// </summary>
	
	public class Renderman{
		
		/// <summary>Set false to force partial style updates only. Used by flatten.</summary>
		public bool AllowLayout=true;
		/// <summary>The starting offset for all batches.</summary>
		public int RenderQueue=3000;
		/// <summary>The current depth the rendering is occuring at.</summary>
		public float Depth=0f;
		/// <summary>The deepest element rendered so far.</summary>
		public float MaxDepth=0f;
		/// <summary>Set to true when an element has been placed in the depth buffer.</summary>
		public bool DepthUsed;
		/// <summary>The current set of active CSS counters.</summary>
		public List<CssCounter> Counters;
		/// <summary>The "highest" update mode used.</summary>
		public UpdateMode HighestUpdateMode=UpdateMode.None;
		/// <summary>A colour that is applied over the top of this element.</summary>
		public Color ColorOverlay=Color.white;
		/// <summary>The nearest outer block ancestors or inner flow root (block, inline-block, table etc).</summary>
		public Stack<RenderableData> FlowRootAncestors=new Stack<RenderableData>();
		/// <summary>The nearest positioned (non 'static') ancestor. If this is null, use the viewport.</summary>
		public Stack<RenderableData> PositionedAncestors=new Stack<RenderableData>();
		/// <summary>The nearest transformed ancestor. If this is null, use the viewport.</summary>
		public Stack<RenderableData> TransformedAncestors=new Stack<RenderableData>();
		/// <summary>A stack of transformations to apply to elements. Updated during a layout event.</summary>
		public TransformationStack Transformations=new TransformationStack();
		
		/// <summary>True if this renderer is performing a complete reflow.</summary>
		internal bool FullReflow;
		/// <summary>The batch that we are currently rendering to.</summary>
		public UIBatch CurrentBatch;
		/// <summary>The active clipping boundary. Usually the bounds of the parent element.</summary>
		public BoxRegion ClippingBoundary;
		/// <summary>Essentially counts how many batches were issued. Used to define the render order.</summary>
		public int BatchDepth;
		/// <summary>The current top font aliasing value.</summary>
		public float FontAliasingTop;
		/// <summary>The current bottom font aliasing value.</summary>
		public float FontAliasingBottom;
		/// <summary>The unity layer that all meshes should go into.</summary>
		public int RenderLayer;
		/// <summary>A flag which notes if the vital parts of a layout are occuring. During this, the DOM delays updates.</summary>
		public bool LayoutOccuring;
		/// <summary>The tail of the batch linked list.</summary>
		public UIBatch LastBatch;
		/// <summary>The head of the batch linked list.</summary>
		public UIBatch FirstBatch;
		/// <summary>The base document being rendered.</summary>
		public ReflowDocument RootDocument;
		/// <summary>How far apart along z elements are placed on the UI.</summary>
		public float DepthResolution=0.05f;
		/// <summary>True if a layout event was requested.</summary>
		public bool DoLayout;
		/// <summary>True if we've handled the viewport background (originates from the first of either html or body).</summary>
		internal bool ViewportBackground;
		/// <summary>The start of a linked list of styles that need to be painted.
		/// This linked list helps prevent layouts occuring as many updates only affect the appearance.</summary>
		public RenderableData StylesToUpdate;
		/// <summary>The start of a linked list of styles that need to be recomputed.</summary>
		public ElementStyle StylesToRecompute;
		/// <summary>An optional gameobject to parent all content to.</summary>
		public GameObject Node;
		/// <summary>True if the screen should clip this renderer.</summary>
		public bool ScreenClip=true;
		/// <summary>Used if this renderer is generating a UI in the game world (e.g. on a billboard).</summary>
		public WorldUI InWorldUI;
		/// <summary>Used to halt the renderer from rendering anything else of a child (or anything else of a document etc).</summary>
		public int StallStatus;
		/// <summary>The active shader set. Expected to never be null.</summary>
		public ShaderSet CurrentShaderSet;
		/// <summary>The collider when accepting input.</summary>
		public Collider PhysicsModeCollider;
		/// <summary>The default filtering mode used by all images of this renderer.</summary>
		private FilterMode ImageFilterMode=FilterMode.Point;
		/// <summary>How this renderman renders images; either on an atlas or with them 'as is'.</summary>
		private static RenderMode UIRenderMode=RenderMode.Atlas;
		/// <summary>A shared block used as an interface for reading/ writing from meshes.</summary>
		public MeshBlock Block;
		/// <summary>The segment to draw. Used by inline elements when they're drawing e.g. a border.</summary>
		public int Segment=LineBoxSegment.All;
		/// <summary>A list of 'active' informers. Used by special effects like first-line.</summary>
		public SparkInformerNode FirstInformer;
		/// <summary>The informer for the first-letter CSS selector. If it's not null then a text node will deal with it.</summary>
		internal SparkInformerNode FirstLetter;
		/// <summary>The informer for the first-line CSS selector.</summary>
		internal SparkInformerNode FirstLine;
		
		#region Text properties
		internal int CharacterIndex;
		internal LayoutBox CurrentBox;
		internal Color FontColour;
		internal BoxRegion CurrentRegion;
		internal float TopOffset;
		internal float TextScaleFactor;
		internal float TextAscender;
		internal float TextDepth;
		#endregion
		
		/// <summary>True if this renderer is 'flat'.</summary>
		public bool IsFlat{
			get{
				return InWorldUI is FlatWorldUI;
			}
		}
		
		/// <summary>The active transformation.</summary>
		public Transformation Transform{
			get{
				return Transformations.Last;
			}
		}
		
		
		/// <summary>Creates a new renderer for rendering in the world.</summary>
		public Renderman(WorldUI worldUI){
			Node=worldUI.gameObject;
			InWorldUI=worldUI;
			Init();
		}
		
		/// <summary>Creates a new renderer and a new document.</summary>
		public Renderman(){
			Init();
		}
		
		/// <summary>Creates a new renderer with the given document.</summary>
		public Renderman(ReflowDocument root){
			RootDocument=root;
			Init();
		}
		
		/// <summary>Sets up this renderman.</summary>
		private void Init(){
			Block=new MeshBlock(this);
			ClippingBoundary=new BoxRegion(0,0,UnityEngine.Screen.width,UnityEngine.Screen.height);
			
			if(RootDocument==null){
				HtmlDocument hdoc=new HtmlDocument(this);
				hdoc.SetRawLocation( new Location("resources://",null) );
				RootDocument=hdoc;
			}
		}
		
		/// <summary>Creates a renderman for use in the Editor with AOT compiling Nitro.</summary>
		public Renderman(bool aot){}
		
		
		/// <summary>Is this a renderer for a WorldUI?</summary>
		public bool RenderingInWorld{
			get{
				return (InWorldUI!=null);
			}
		}
		
		/// <summary>Gets the parent gameobject for this renderman, if there is one.</summary>
		public GameObject Parent{
			get{
				if(Node!=null){
					return Node;
				}
				return UI.GUINode;
			}
		}
		
		/// <summary>Increments the named counter.</summary>
		public void IncrementCounter(string name,int amount){
			
			if(Counters==null){
				return;
			}
			
			// Find the latest counter with the given name:
			for(int i=Counters.Count-1;i>=0;i--){
				
				if(Counters[i].Name==name){
					
					// Match! Increase count:
					CssCounter counter=Counters[i];
					counter.Count+=amount;
					Counters[i]=counter;
				}
				
			}
			
		}
		
		/// <summary>Gets the count with the given name.</summary>
		public int GetCounter(string name){
			
			if(Counters==null){
				return 0;
			}
			
			// Find the latest counter with the given name:
			for(int i=Counters.Count-1;i>=0;i--){
				
				if(Counters[i].Name==name){
					
					// Match!
					return Counters[i].Count;
					
				}
				
			}
			
			return 0;
		}
		
		/// <summary>Pops the latest counter of the given name and anything after it.</summary>
		public void PopCounter(string name){
			
			if(Counters==null){
				return;
			}
			
			for(int i=Counters.Count-1;i>=0;i--){
				
				// Got a match?
				bool match=(Counters[i].Name==name);
				
				// Pop it:
				Counters.RemoveAt(i);
				
				if(match){
					return;
				}
				
			}
			
		}
		
		/// <summary>Resets the named counter.
		/// The previous counter with the same name is returned (or -1 if none).</summary>
		public void ResetCounter(string name){
			
			if(Counters==null){
				Counters=new List<CssCounter>();
			}
			
			Counters.Add(new CssCounter(name));
		}
		
		/// <summary>Called when the physics input mode changes. 
		/// Ensures all batches in this renderer are on the correct new input mode.</summary>
		/// <param name="mode">The new input mode to use.</param>
		public void SetInputMode(bool acceptInput){
			
			if(InWorldUI==null){
				return;
			}
			
			if(acceptInput){
				
				if(PhysicsModeCollider==null){
					// Let's give it a box collider:
					BoxCollider bc = Parent.AddComponent<BoxCollider>();
					bc.size = new Vector3(InWorldUI.pixelWidth,InWorldUI.PixelHeightF,0.01f);
					PhysicsModeCollider = bc;
				}
				
			}else if(PhysicsModeCollider!=null){
				GameObject.Destroy(PhysicsModeCollider);
			}
			
		}
		
		/// <summary>How this renderman renders images; either on an atlas or with them 'as is'.</summary>
		public RenderMode RenderMode{
			get{
				return UIRenderMode;
			}
			set{
				UIRenderMode=value;
				RequestLayout();
			}
		}
		
		/// <summary>The image filter mode. If you're using lots of WorldUI's or animations its best to have this on bilinear.</summary>
		public FilterMode FilterMode{
			get{
				return ImageFilterMode;
			}
			set{
				if(value==ImageFilterMode){
					return;
				}

				ImageFilterMode=value;
				AtlasStacks.Graphics.FilterMode=value;
			}
		}
		
		/// <summary>Increases the current depth value.</summary>
		public void IncreaseDepth(){
			Depth+=DepthResolution;
			if(Depth>MaxDepth){
				MaxDepth=Depth;
			}
		}
		
		/// <summary>Destroys this renderman when it's no longer needed.</summary>
		public void Destroy(){
			if(FirstBatch!=null){
				// Pool:
				UIBatchPool.AddAll(FirstBatch,LastBatch);
				
				// Hide:
				UIBatchPool.HideAll();
			}
			
			RootDocument.clear();
		}
		
		/// <summary>Sets up the current batch as a 'globally isolated' batch. This acts like a hybrid between isolated
		/// and shared.</summary>
		/// <param name="property">The displayable property which wants the batch.</param>
		/// <param name="fontTexture">The font texture to use with this batch.</param>
		public void SetupBatchGI(DisplayableProperty property,TextureAtlas graphics,TextureAtlas font){
			
			if(!property.Isolated){
				// Ordinary non-isolated batch:
				SetupBatch(property,graphics,font);
				return;
			}
			
			if(property.GotBatchAlready){
				
				// Re-use existing batch?
				
				if(font!=null){
					
					if(CurrentBatch.FontAtlas==null){
						// Didn't have one assigned before. Assign now:
						CurrentBatch.SetFontAtlas(font);
					}else if(font!=CurrentBatch.FontAtlas){
						// Font atlas changed. Can't share.
						CurrentBatch=null;
					}
					
				}
				
				if(graphics!=null){
					
					if(CurrentBatch.GraphicsAtlas==null){
						// Didn't have one assigned before. Assign now:
						CurrentBatch.SetGraphicsAtlas(graphics);
					}else if(graphics!=CurrentBatch.GraphicsAtlas){
						// Atlas changed. Can't share.
						CurrentBatch=null;
					}
					
				}
				
				if(CurrentBatch!=null){
					// Yep - reuse it.
					return;
				}
				
			}
			
			// First timer or new one required - create:
			CurrentBatch=UIBatchPool.Get(this);
			
			if(CurrentBatch==null){
				CurrentBatch=new UIBatch(this);
			}
			
			property.GotBatchAlready=true;
			
			// And push it to the active stack:
			AddBatch(CurrentBatch);
			
			// Use the global material, but set it as isolated
			// (note that we don't want to set CurrentBatch.IsolatedProperty as it may get unintentionally spammed):
			CurrentBatch.NotIsolated(graphics,font);
			CurrentBatch.Isolated=true;
			
			// Finally, prepare it for layout:
			CurrentBatch.PrepareForLayout();
			
		}
		
		/// <summary>Sets up the current batch based on the isolation settings requested by a property.</summary>
		/// <param name="property">The displayable property which wants the batch.</param>
		/// <param name="fontTexture">The font texture to use with this batch.</param>
		public void SetupBatch(DisplayableProperty property,TextureAtlas graphics,TextureAtlas font){
			
			if(property.Isolated){
				if(property.GotBatchAlready){
					// The property already got a batch on this layout - it doesn't need another.
					return;
				}
				
				// Isolated properties always get a new batch every time.
				CurrentBatch=UIBatchPool.Get(this);
				
				if(CurrentBatch==null){
					CurrentBatch=new UIBatch(this);
				}
				
				property.GotBatchAlready=true;
				
				// And push it to the active stack:
				AddBatch(CurrentBatch);
				
				// Make sure it knows it's isolated:
				CurrentBatch.IsIsolated(property);
				
			}else{
				
				if(CurrentBatch!=null && !CurrentBatch.Isolated){
					// Re-use existing batch?
					
					if(font!=null){
						
						if(CurrentBatch.FontAtlas==null){
							// Didn't have one assigned before. Assign now:
							CurrentBatch.SetFontAtlas(font);
						}else if(font!=CurrentBatch.FontAtlas){
							// Font atlas changed. Can't share.
							CurrentBatch=null;
						}
						
					}
					
					if(graphics!=null){
						
						if(CurrentBatch.GraphicsAtlas==null){
							// Didn't have one assigned before. Assign now:
							CurrentBatch.SetGraphicsAtlas(graphics);
						}else if(graphics!=CurrentBatch.GraphicsAtlas){
							// Atlas changed. Can't share.
							CurrentBatch=null;
						}
						
					}
					
					if(CurrentBatch!=null){
						// Yep - reuse it.
						return;
					}
					
				}
				
				// Pull a batch from the pool and set it to currentbatch. May need to generate a new one.
				CurrentBatch=UIBatchPool.Get(this);
				
				if(CurrentBatch==null){
					CurrentBatch=new UIBatch(this);
				}
				
				// And push it to the active stack:
				AddBatch(CurrentBatch);
				
				// Make sure it knows it's not isolated:
				CurrentBatch.NotIsolated(graphics,font);
				
			}
			
			// Finally, prepare it for layout:
			CurrentBatch.PrepareForLayout();
			
		}
		
		/// <summary>Adds the given batch to the main linked list for processing.</summary>
		public void AddBatch(UIBatch batch){
			if(FirstBatch==null){
				FirstBatch=LastBatch=batch;
			}else{
				LastBatch=LastBatch.BatchAfter=batch;
			}
		}
		
		/// <summary>The current stack of boxes that we're working on.</summary>
		public Stack<LineBoxMeta> BoxStack=new Stack<LineBoxMeta>();
		public BlockBoxMeta LastBlockBox;
		/// <summary>A fast grid for input lookups.</summary>
		public InputGrid InputGrid=new InputGrid();
		
		/// <summary>Sets up this renderer so that it's ready to start packing child elements of
		/// a given element into lines.</summary>
		/// <param name="renderable">The parent render data whose children will be packed.</param>
		public LineBoxMeta BeginLines(RenderableData renderable,VirtualElements virts,LayoutBox box,bool autoWidth){
			
			// Get CS:
			ComputedStyle cs=renderable.computedStyle;
			
			LineBoxMeta lineZone;
			
			InfiniText.FontFace face=box.FontFace;
			
			// Update line height:
			
			// Line height:
			Css.Value lineHeightValue=cs.LineHeightX;
			
			float cssLineHeight;
			
			if(lineHeightValue.IsType(typeof(Css.Keywords.Normal))){
				
				// Get from the metrics font now:
				cssLineHeight=box.FontFace.BaselineToBaseline * box.FontSize;
				
			}else if(
				lineHeightValue.Type!=Css.ValueType.RelativeNumber && 
				lineHeightValue.GetType()!=typeof(Css.Units.DecimalUnit)
			){
				
				// E.g. line-height:14px, but not line-height:1. It's just as-is:
				cssLineHeight=lineHeightValue.GetRawDecimal();
				
			}else{
				
				// Some multiple of the font size:
				cssLineHeight=lineHeightValue.GetRawDecimal() * box.FontSize;
				
			}
			
			// Check if it's a block context:
			if( box.DisplayMode==DisplayMode.Inline && LastBlockBox!=null ){
				
				// Anything else uses the nearest parent block element as the max.
				lineZone=new InlineBoxMeta( LastBlockBox,TopOfStackSafe,box,renderable );
				
				// Put up the 'strut':
				lineZone.LineHeight=cssLineHeight;
				box.Baseline=box.FontSize * face.Descender;
				
			}else{
				
				lineZone=LastBlockBox=new BlockBoxMeta(TopOfStackSafe,box,renderable);
				lineZone.MaxX=box.InnerWidth;
				
				if(virts!=null && virts.Has(ComputedStyle.VerticalScrollPriority)){
					lineZone.MaxX-=14;
					
					if(lineZone.MaxX<0){
						lineZone.MaxX=0;
					}
					
				}
				
				bool left=(cs.DrawDirectionX==DirectionMode.RTL);
				lineZone.GoingLeftwards=left;
				
				// H-align:
				int hAlign=cs.HorizontalAlignX;
				
				if(hAlign==HorizontalAlignMode.Auto){
					if(left){
						hAlign=HorizontalAlignMode.Right;
					}else{
						hAlign=HorizontalAlignMode.Left;
					}
				}
				
				if(hAlign==HorizontalAlignMode.Left){
					// Ok how it is (left by default).
					hAlign=0;
				}
				
				lineZone.HorizontalAlign=hAlign;
				
			}
			
			// Apply whitespace mode:
			lineZone.WhiteSpace=cs.WhiteSpaceX;
			
			// Apply line height:
			lineZone.CssLineHeight=cssLineHeight;
			
			// Update vertical-align:
			Css.Value vAlign=cs.Resolve(Css.Properties.VerticalAlign.GlobalProperty);
			
			// Get the complete value:
			float vAlignValue=vAlign.GetDecimal(renderable,Css.Properties.VerticalAlign.GlobalProperty);
			
			// If it's a keyword..
			if(vAlign is Css.CssKeyword){
				
				// It's a mode:
				lineZone.VerticalAlign=(int)vAlignValue;
				lineZone.VerticalAlignOffset=0f;
				
			}else{
				
				// It's a baseline offset:
				lineZone.VerticalAlign=VerticalAlignMode.Baseline;
				lineZone.VerticalAlignOffset=vAlignValue;
				
			}
			
			box.ContentWidth=0;
			box.ContentHeight=0;
			
			return lineZone;
			
		}
		
		/// <summary>Lets the renderer know that the given parent element has finished
		/// packing all of its kids. This allows alignment to occur next.</summary>
		/// <param name="renderable">The element that is done packing.</param>
		public void EndLines(LineBoxMeta lineZone,ComputedStyle computed,LayoutBox box){
			
			// Pop the box:
			BoxStack.Pop();
			
			// Complete the last line:
			lineZone.CompleteLine(LineBreakMode.NoBreak | LineBreakMode.Last);
			
			// If inline-block or float, clear:
			if(box.DisplayMode==DisplayMode.InlineBlock || box.FloatMode!=FloatMode.None){
				
				// Must clear:
				lineZone.ClearFloat(FloatMode.Both);
				
				lineZone.PenY+=lineZone.ClearY_;
				lineZone.ClearY_=0f;
				
			}
			
			// block, inline-block
			if(lineZone is BlockBoxMeta){
				
				bool heightChange=false;
				
				if(box.InnerHeight==-1f){
					heightChange=true;
					box.InnerHeight=lineZone.PenY;
					
					// Clip height now:
					box.InnerHeight=computed.ClipHeight(box.DisplayMode,box.InnerHeight);
					
				}
				
				box.ContentHeight=lineZone.PenY;
				box.ContentWidth=lineZone.LargestLineWidth;
				
				// If it's inline then we set the line width.
				if(box.InnerWidth==-1f){
					box.InnerWidth=lineZone.LargestLineWidth;
					
					// Apply valid width/height:
					box.SetDimensions(false,false);
					
				}else if(heightChange){
					
					// Apply valid width/height:
					box.SetDimensions(false,false);
					
				}
				
				bool inFlow=(box.PositionMode & PositionMode.InFlow)!=0;
				
				// Update position of the top-of-stack pen:
				LineBoxMeta tos=TopOfStackSafe;
				
				if(tos==null){
					
					LastBlockBox=null;
					
				}else{
					
					if(inFlow){
						// Advance the pen:
						tos.AdvancePen(box);
					}
					
					// Restore previous block:
					LastBlockBox=tos as BlockBoxMeta;
					
					if(LastBlockBox==null){
						// Rare - block inside inline.
						LastBlockBox=(tos as InlineBoxMeta).HostBlock;
					}
					
				}
				
			}
			
		}
		
		/// <summary>The current box meta on the TOS.</summary>
		public LineBoxMeta TopOfStack{
			get{
				return BoxStack.Peek();
			}
		}
		
		/// <summary>The current box meta on the TOS.</summary>
		public LineBoxMeta TopOfStackSafe{
			get{
				if(BoxStack.Count==0){
					return null;
				}
				
				return BoxStack.Peek();
			}
		}
		
		/// <summary>Checks if the given box coordinates are outside the current clipping boundary.
		/// If they are, the box is considered invisible.</summary>
		/// <param name="left">The x coordinate of the left edge of the box
		/// in pixels from the left of the screen.</param>
		/// <param name="top">The y coordinate of the top edge of the box
		/// in pixels from the top edge of the screen.</param>
		/// <param name="width">The width of the box.</param>
		/// <param name="height">The height of the box (extends down the screen).</param>
		/// <returns>True if the box was outside the current clipping boundary.</returns>
		public bool IsInvisible(float left,float top,float width,float height){
			return (left>ClippingBoundary.MaxX || top>ClippingBoundary.MaxY || (left+width)<ClippingBoundary.X || (top+height)<ClippingBoundary.Y);
		}
		
		/// <summary>Sets the clipping boundary from the given computed style.</summary>
		/// <param name="style">The computed style to find the clipping boundary from.</param>
		public void SetBoundary(ComputedStyle computed,LayoutBox box){
			bool visibleX=(box.OverflowX==VisibilityMode.Visible);
			bool visibleY=(box.OverflowY==VisibilityMode.Visible);
			
			if(visibleX && visibleY){
				return;
			}
			
			BoxRegion newBoundary=null;
			
			if(visibleX){
				newBoundary=new BoxRegion(ClippingBoundary.X,box.Y+box.FixedStyleOffsetTop,ClippingBoundary.Width,box.InnerHeight);
			}else if(visibleY){
				newBoundary=new BoxRegion(box.X+box.FixedStyleOffsetLeft,ClippingBoundary.Y,box.InnerWidth,ClippingBoundary.Height);
			}else{
				newBoundary=new BoxRegion(box.X+box.FixedStyleOffsetLeft,box.Y+box.FixedStyleOffsetTop,box.InnerWidth,box.InnerHeight);
			}
			
			// Should it be clipped?
			Css.Value clipValue=computed[Css.Properties.ClipMode.GlobalProperty];
			
			if(clipValue==null || clipValue.IsType(typeof(Css.Keywords.None)) || clipValue.GetBoolean(computed.RenderData,Css.Properties.ClipMode.GlobalProperty)){
				newBoundary.ClipBy(ClippingBoundary);
			}else{
				ScreenClip=false;
			}
			
			ClippingBoundary=newBoundary;
		}
		
		/// <summary>Nearest flow root. Null if you should use viewport.</summary>
		public RenderableData FlowRootAncestor{
			get{
				
				if(FlowRootAncestors.Count==0){
					
					// Relative to the viewport.
					return null;
				
				}
				
				// Relative to the top positioned ancestor.
				return FlowRootAncestors.Peek();
				
			}
		}
		
		/// <summary>Nearest positioned ancestor. Null if you should use viewport.</summary>
		public RenderableData PositionedAncestor{
			get{
				
				if(PositionedAncestors.Count==0){
					
					// Relative to the viewport.
					return null;
				
				}
				
				// Relative to the top positioned ancestor.
				return PositionedAncestors.Peek();
				
			}
		}
		
		/// <summary>Nearest transformed ancestor. Null if you should use viewport.</summary>
		public RenderableData TransformedAncestor{
			get{
				
				if(TransformedAncestors.Count==0){
					
					// Relative to the viewport.
					return null;
				
				}
				
				// Relative to the top transformed ancestor.
				return TransformedAncestors.Peek();
				
			}
		}
		
		/// <summary>The root screen viewport.</summary>
		public BoxRegion ScreenViewport=new BoxRegion();
		
		/// <summary>The current viewport. Note that this changes whenever a 
		/// new document starts rendering (such as inside an iframe).</summary>
		public BoxRegion Viewport;
		
		/// <summary>The max Y value. Essentially the virtual screens width.</summary>
		public float MaxX{
			get{
				return ScreenViewport.Width;
			}
		}
		
		/// <summary>The max Y value. Essentially the virtual screens height.</summary>
		public float MaxY{
			get{
				return ScreenViewport.Height;
			}
		}
		
		/// <summary>Resets the clipping boundary back to the whole screen.</summary>
		public void ResetBoundary(){
			
			// Update viewport:
			if(InWorldUI!=null){
				ScreenViewport.Width=InWorldUI.pixelWidth;
				ScreenViewport.Height=InWorldUI.pixelHeight;
			}else{
				ScreenViewport.Width=ScreenInfo.ScreenX;
				ScreenViewport.Height=ScreenInfo.ScreenY;
			}
			
			// Apply screen viewport as the root one:
			Viewport=ScreenViewport;
			
			if(InWorldUI!=null || ScreenClip){
				ClippingBoundary.Set(0,0,Viewport.Width,Viewport.Height);
			}else{
				ClippingBoundary.Set(-80000,-80000,160000,160000);
			}
			
		}
		
		/// <summary>Resets all values in the renderer. Called before each layout.</summary>
		public void Reset(){
			
			Depth=0f;
			MaxDepth=0f;
			ViewportBackground=true;
			BatchDepth=RenderQueue;
			PositionedAncestors.Clear();
			TransformedAncestors.Clear();
			FlowRootAncestors.Clear();
			ColorOverlay=Color.white;
			FontAliasingTop=InfiniText.Fonts.OutlineLocation+InfiniText.Fonts.Aliasing;
			FontAliasingBottom=InfiniText.Fonts.OutlineLocation-InfiniText.Fonts.Aliasing;
			BoxStack.Clear();
			LastBlockBox=null;
			ResetBoundary();
			DepthUsed=false;
			CurrentBatch=null;
			CurrentShaderSet=ShaderSet.Standard;
			FirstInformer=null;
			FirstLetter=null;
			FirstLine=null;
			
			if(Counters!=null){
				Counters.Clear();
			}
			
			// Reapply screen viewport as the root one:
			Viewport=ScreenViewport;
			
			// Clean input grid:
			InputGrid.Clean(Viewport.Width,Viewport.Height);
			
			if(InWorldUI==null){
				// This is the main UI renderer.
				
				// Clear the root node:
				Node=null;
				
			}
		}
		
		/// <summary>The layer to put this Renderer in. Simply an alias for RenderWithCamera.</summary>
		public int Layer{
			set{
				RenderWithCamera(value);
			}
			get{
				return RenderLayer;
			}
		}
		
		/// <summary>Puts all batches of this renderer into the given unity layer.</summary>
		/// <param name="id">The ID of the unity layer.</param>
		public void RenderWithCamera(int id){
			RenderLayer=id;
			
			// Set the layer of each batch:
			UIBatch current=FirstBatch;
			
			while(current!=null){
				current.RenderWithCamera(id);
				current=current.BatchAfter;
			}
		}
		
		/// <summary>Asks the renderer to perform a layout next update.</summary>
		public void RequestLayout(){
			DoLayout=true;
		}
		
		/// <summary>Asks the renderer to perform a paint on the given style object next update.</summary>
		/// <param name="style">The style to paint.</param>
		public void RequestUpdate(Css.RenderableData style,UpdateMode mode){
			
			if(DoLayout){
				// Full reflow is occuring anyway - do nothing:
				return;
			}
			
			// Ignore this request if it was for something lesser.
			int previousMode=(int)style.NextUpdateMode;
			
			if((int)mode>previousMode){
				// We're now requesting an update:
				style.NextUpdateMode=mode;
			}else{
				// (e.g. it has already got a reflow queued up but a paint was requested;
				// reflow performs a paint).
				return;
			}
			
			// Is it the highest update mode?
			if((int)mode > (int)HighestUpdateMode){
				
				// Update highest:
				HighestUpdateMode=mode;
				
			}
			
			if(previousMode!=0){
				// Already in the queue - don't add again.
				return;
			}
			
			if(StylesToUpdate==null){
				StylesToUpdate=style;
				style.Next=null;
			}else{
				style.Next=StylesToUpdate;
				StylesToUpdate=style;
			}
			
		}
		
		/// <summary>Update causes all changes to be applied and layouts to occur.</summary>
		public void Update(){
			
			if(DoLayout && AllowLayout){
				
				// Layout RootDocument.
				Layout();
				
			}else if(StylesToUpdate!=null){
				// Local update - these events typically fire from changes to things like colour/z-index etc
				// as well as for reflows of "flow root" nodes.
				// It's done down here incase a full layout request is made (above).
				// If a full layout request was made, it would cover all of these anyway.
				
				UpdateMode modeToUse=HighestUpdateMode;
				HighestUpdateMode=UpdateMode.None;
				
				bool anyReflowMode=((int)modeToUse > (int)UpdateMode.PaintAll);
				
				if(anyReflowMode){
					
					// We'll be re-rendering.
					Reset();
					
					// Invalidate input pointers:
					// (So they figure out what elements are under the mouse/fingers)
					PowerUI.Input.PointersInvalid=true;
					
					// First, push all batches to the pool - inlined for speed:
					// Note that no isolated batches enter either the queue or the pool until their no longer isolated.
					if(FirstBatch!=null){
						LastBatch.BatchAfter=UIBatchPool.First;
						UIBatchPool.First=FirstBatch;
					}
					
					FirstBatch=LastBatch=null;
					
					// Note: Batches are Prepared For Layout as they are added.
					LayoutOccuring=true;
					
				}
				
				Css.RenderableData style=StylesToUpdate;
				StylesToUpdate=null;
				
				while(style!=null){
					
					UpdateMode mode=style.NextUpdateMode;
					
					switch(mode){
						
						case UpdateMode.PaintAll:
							
							// Don't bother if we're doing either kind of reflow:
							if(anyReflowMode){
								continue;
							}
							
							// Repaint it:
							style.RepaintAll(this);
							
						break;
						case UpdateMode.Paint:
						
							// Repaint it:
							style.Repaint(this);
							
							// Must also repaint the nodes child text nodes too (as they share the same CS):
							NodeList kids=style.Node.childNodes_;
							
							if(kids!=null){
								
								// For each child node..
								for(int i=0;i<kids.length;i++){
									
									// Get it as a text node:
									Node child=kids[i];
									
									if(child is TextNode){
										
										// Repaint it too:
										(child as IRenderableNode).RenderData.Repaint(this);
										
									}
									
								}
								
							}
						
						break;
						case UpdateMode.Reflow:
						
							// Flow root reflow request.
							
							// Must setup stacks and any other renderer settings here!
							
							// Perform the initial reflow now:
							style.UpdateCss(this);
							style.Reflow(this);
							
						break;
						case UpdateMode.Render:
							
							// Only call render:
							style.Render(this);
							
						break;
						
						/*
						case UpdateMode.FastReflow:
							
							// Fast reflow request only requires a repaint.
							
						break;
						*/
						
					}
					
					// Clear its update mode:
					style.NextUpdateMode=UpdateMode.None;
					style=style.Next;
				}
				
				
				if(!anyReflowMode){
					
					// Only a flush is required:
					UIBatch toFlush=FirstBatch;
					
					while(toFlush!=null){
						toFlush.Flush();
						toFlush=toFlush.BatchAfter;
					}
					
				}else{
					
					// Position elements locally.
					// This sets their ParentOffset values and as a result finds their PixelWidth.
					IRenderableNode root=RootDocument.documentElement as IRenderableNode;
					
					if(root!=null){
						
						// Finally, position them globally:
						// This calculates OffsetLeft/Top and also fires the render event on the computed style object.
						root.RenderData.Render(this);
						
					}
					
					LayoutOccuring=false;
					
					// Tell each batch we're done laying them out:
					UIBatch currentBatch=FirstBatch;
					
					while(currentBatch!=null){
						currentBatch.CompletedLayout();
						currentBatch=currentBatch.BatchAfter;
					}
					
					// Hide all pool entries:
					UIBatchPool.HideAll();
					
				}
				
			}
			
		}
		
		/// <summary>A pool of layout boxes.</summary>
		public LayoutBox LastBoxPool;
		/// <summary>A pool of layout boxes.</summary>
		public LayoutBox FirstBoxPool;
		
		/// <summary>Adds a linked list of boxes to the box pool.</summary>
		public void PoolBoxes(LayoutBox firstBox,LayoutBox lastBox){
			
			if(firstBox==null){
				return;
			}
			
			if(LastBoxPool==null){
				FirstBoxPool=firstBox;
				LastBoxPool=lastBox;
			}else{
				LastBoxPool.NextInElement=firstBox;
				LastBoxPool=lastBox;
			}
			
			lastBox.NextInElement=null;
			
		}
		
		/// <summary>Gets a box from the pool or creates one.</summary>
		public LayoutBox PooledBox(){
			
			if(FirstBoxPool==null){
				return new LayoutBox();
			}
			
			LayoutBox front=FirstBoxPool;
			FirstBoxPool=front.NextInElement;
			
			if(LastBoxPool==front){
				// Clear end:
				LastBoxPool=null;
			}
			
			// Clear all hierarchy values:
			front.NextInElement=null;
			front.NextLineStart=null;
			front.NextOnLine=null;
			front.FirstChild=null;
			front.Parent=null;
			
			front.ParentOffsetTop=0f;
			front.ParentOffsetLeft=0f;
			front.OrdinaryInline=true;
			front.Baseline=0f;
			
			return front;
			
		}
		
		/// <summary>Relocates all DOM elements by calculating their onscreen position.
		/// Each element may allocate sections of the 3D mesh (blocks) which are then flushed out
		/// into the unity mesh and onto the screen.</summary>
		public void Layout(){
			DoLayout=false;
			FullReflow=true;
			HighestUpdateMode=UpdateMode.None;
			
			Reset();
			
			// Invalidate input pointers:
			// (So they figure out what elements are under the mouse/fingers)
			PowerUI.Input.PointersInvalid=true;
			
			// First, push all batches to the pool - inlined for speed:
			// Note that no isolated batches enter either the queue or the pool until their no longer isolated.
			if(FirstBatch!=null){
				LastBatch.BatchAfter=UIBatchPool.First;
				UIBatchPool.First=FirstBatch;
			}
			
			FirstBatch=LastBatch=null;
			
			// Note: Batches are Prepared For Layout as they are added.
			
			LayoutOccuring=true;
			
			// Position elements locally.
			// This sets their ParentOffset values and as a result finds their PixelWidth.
			IRenderableNode root=RootDocument.documentElement as IRenderableNode;
			
			if(root!=null){
				
				// Perform the initial reflow:
				RenderableData rd=root.RenderData;
				
				rd.UpdateCss(this);
				rd.Reflow(this);
				 
				// Next up, position them globally:
				// This calculates OffsetLeft/Top and also fires the render event on the computed style object.
				rd.Render(this);
				
			}
			
			LayoutOccuring=false;
			
			// Tell each batch we're done laying them out:
			UIBatch currentBatch=FirstBatch;
			
			while(currentBatch!=null){
				currentBatch.CompletedLayout();
				currentBatch=currentBatch.BatchAfter;
			}
			
			if(StylesToUpdate!=null){
				// Clear the isPainting flag.
				RenderableData style=StylesToUpdate;
				StylesToUpdate=null;
				
				while(style!=null){
					style.NextUpdateMode=UpdateMode.None;
					style=style.Next;
				}
			}
			
			// Hide all pool entries:
			UIBatchPool.HideAll();
			FullReflow=false;
		}
		
		/// <summary>Converts the given screen coordinate to world coordinates.</summary>
		/// <param name="px">The screen x coordinate in pixels from the left.</param>
		/// <param name="py">The screen y coordinate in pixels from the top.</param>
		/// <param name="depth">The z depth.</param>
		public Vector3 PixelToWorldUnit(float px,float py,float depth){
			if(InWorldUI==null){
				
				py=ScreenInfo.ScreenYFloat-py;
				float depthFactor=1f-(depth*ScreenInfo.DepthDepreciation);
				
				// The camera is placed on negative z so the actual depth value is inverted (as below).
				return new Vector3(
					(ScreenInfo.WorldScreenOrigin.x + px*ScreenInfo.WorldPerPixel.x) * depthFactor,
					(ScreenInfo.WorldScreenOrigin.y + py*ScreenInfo.WorldPerPixel.y) * depthFactor,
					-depth
								);
			}else{
				py=InWorldUI.PixelHeightF-py;
				
				return new Vector3(
					(InWorldUI.WorldScreenOrigin.x + px),
					(InWorldUI.WorldScreenOrigin.y + py),
					-depth
								);
			}
		}
		
	}
	
}