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
using Blaze;
using PowerUI;


namespace Css{
	
	/// <summary>
	/// Base class for any property of an element that can be visually displayed, e.g. backgrounds and borders.
	/// </summary>
	
	public class DisplayableProperty{
		
		/// <summary>True if this is currently visible on screen.</summary>
		public bool Visible;
		/// <summary>True if this property is isolated and has a seperate mesh and material from everything else.</summary>
		public bool Isolated;
		/// <summary>The number of mesh blocks that this property has allocated.</summary>
		public short BlockCount;
		/// <summary>The parent render data that this is a property of.</summary>
		public RenderableData RenderData;
		/// <summary>The batch that this was allocated to.</summary>
		internal UIBatch Batch;
		/// <summary>The block index in the first buffer. With this, all blocks belonging to this property can be quickly discovered.</summary>
		internal int FirstBlockIndex;
		/// <summary>Only applies to Isolated properties. Set to true when this property has allocated a UIBatch on the current layout.</summary>
		public bool GotBatchAlready;
		
		
		/// <summary>Creates a new displayable property for the given render data.</summary>
		public DisplayableProperty(RenderableData data){
			RenderData=data;
		}
		
		/// <summary>Gets the first rendered block of this property. Used during Paint passes.</summary>
		public MeshBlock GetFirstBlock(Renderman renderer){
			
			if(Batch==null){
				// Layout required.
				return null;
			}
			
			// Use the shared block:
			MeshBlock block=renderer.Block;
			
			// Load into it now:
			block.SetBatchIndex(Batch,FirstBlockIndex);
			
			// Return it:
			return block;
			
		}
		
		/// <summary>This property's draw order.</summary>
		public virtual int DrawOrder{
			get{
				return 0;
			}
		}
		
		/// <summary>Call this when the visibility of this property as a whole changes.</summary>
		public void SetVisibility(bool visible){
			
			// Fire visibility events now.
			if(visible){
				
				if(NowOnScreen()){
					// Otherwise it rejected it. That can happen when an image hasn't loaded yet (nothing to see anyway).
					Visible=true;
				}
				
			}else{
				Visible=false;
				NowOffScreen();
			}
			
		}
		
		/// <summary>Called when this element goes on screen.</summary>
		internal virtual bool NowOnScreen(){
			return true;
		}
		
		/// <summary>Called when this element goes off screen (or is removed from the DOM).</summary>
		internal virtual void NowOffScreen(){}
		
		/// <summary>Called when the isolation batch for this property gets removed.</summary>
		public virtual void OnBatchDestroy(){
		}
		
		/// <summary>Called when a named css property changes.</summary>
		/// <param name="property">The css property that changed.</param>
		/// <param name="newValue">The properties new value. It may also be null.</param>
		public void Change(string property,Value newValue){
			OnChange(property,newValue);
		}
		
		/// <summary>Called when a named css property changes.</summary>
		/// <param name="property">The css property that changed.</param>
		/// <param name="newValue">The properties new value. It may also be null.</param>
		protected virtual void OnChange(string property,Value newValue){}
		
		/// <summary>Clears all mesh blocks that this property has allocated.</summary>
		public void ClearBlocks(){
			BlockCount=0;
			Batch=null;
			GotBatchAlready=false;
		}
		
		public void WentOffScreen(){
			ClearBlocks();
			
			if(Visible){
				SetVisibility(false);
			}
		}
		
		/// <summary>Make this property visible by forcing it to redraw.</summary>
		public virtual bool Render(bool first,LayoutBox box,Renderman renderer){
			
			if(first){
				ClearBlocks();
			}
			
			Layout(box,renderer);
			return false;
		}
		
		/// <summary>Transforms all the verts by the given delta matrix. Used during a Paint only.</summary>
		public virtual void ApplyTransform(Matrix4x4 delta,Renderman renderer){
			
			// Get the first block:
			MeshBlock block=GetFirstBlock(renderer);
			
			for(int i=0;i<BlockCount;i++){
				
				// Transform the verts:
				block.TransformVertices(delta);
				
				// Seek to the next one:
				block.Next();
				
			}
			
		}
		
		/// <summary>True if this paints across the whole background of the element.</summary>
		public virtual bool IsBackground{
			get{
				return false;
			}
		}
		
		/// <summary>Requests for a paint event to occur. Note that paint events are more efficient than a layout
		/// as they only refresh the mesh colours and uvs rather than the whole mesh.</summary>
		public virtual void RequestPaint(){
			
			if(Batch==null){
				
				RenderData.RequestLayout();
				
			}else{
				
				RenderData.RequestPaint();
				
			}
			
		}
		
		/// <summary>Sets the current batches material.</summary>
		public void SetBatchMaterial(Renderman renderer,Material material){
			
			renderer.CurrentBatch.Mesh.SetMaterial(material);
			
		}
		
		/// <summary>Requests for a layout event to occur. Note that paint events are more efficient than a layout
		/// as they only refresh the mesh colours and uvs rather than the whole mesh.</summary>
		public void RequestLayout(){
			RenderData.Document.RequestLayout();
		}
		
		/// <summary>Called when a paint event occurs. Paint events don't relocate the whole UI so are quick and efficient.</summary>
		public virtual void Paint(LayoutBox box,Renderman renderer){
		}
		
		/// <summary>Called when a layout event occurs. Layout events relocate the whole UI so should be used less frequently than a paint event.</summary>
		internal virtual void Layout(LayoutBox box,Renderman renderer){
			//Add, Remove or change any blocks.
		}
		
		public MeshBlock Add(Renderman renderer){
			
			// Allocate the block:
			MeshBlock block=renderer.CurrentBatch.Mesh.Allocate(renderer);
			
			if(Batch==null){
				Batch=renderer.CurrentBatch;
				FirstBlockIndex=block.Buffer.BlocksBefore + block.BlockIndex;
			}
			
			BlockCount++;
			
			return block;
			
		}
		
		/// <summary>Checks if this property can be repainted.</summary>
		/// <returns>True if this property has blocks allocated as they can be repainted; false otherwise.</returns>
		public bool Paintable{
			get{
				return (Batch!=null);
			}
		}
		
		/// <summary>Called after a render pass.</summary>
		public virtual void PostProcess(LayoutBox box,Renderman renderer){}
		
		/// <summary> Isolates this property from the rest of the UI such that it can have a custom mesh/shader/texture etc.</summary>
		public void Isolate(){
			Isolated=true;
		}
		
		/// <summary>Reverses <see cref="Css.DisplayableProperty.Isolate"/> by
		/// re-including this property in the main UI batch.</summary>
		public void Include(){
			Isolated=false;
		}
		
		/// <summary>Removes this property.</summary>
		public void Remove(){
			RenderData.RemoveProperty(this);
		}
		
		/// <summary>Checks if this is an isolated property - that's one which is seperate and takes its own drawcall.</summary>
		/// <returns>True if it is isolated; false otherwise.</returns>
		public bool IsIsolated(){
			return Isolated;
		}
		
		public AtlasLocation RequireImage(AtlasEntity image){
			
			// Get the image from the global atlas stack.
			AtlasLocation location=AtlasStacks.Graphics.RequireImage(image);
			
			if(location==null){
				// It's separate from the atlas. Too big to fit/ not worth being on an atlas.
				Isolate();
			}else{
				location.UsageCount++;
				Include();
			}
			
			return location;
			
		}
		
	}
	
}