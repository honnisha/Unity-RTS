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

#if UNITY_IPHONE || UNITY_ANDROID || UNITY_WP8 || UNITY_BLACKBERRY
	#define MOBILE
#endif

using System;
using UnityEngine;
using Blaze;
using PowerUI;
using Dom;


namespace Css{
	
	/// <summary>
	/// Represents the background image of an element.
	/// May also be a video (pro only) or animation.
	/// </summary>
	
	public partial class BackgroundImage:DisplayableProperty{
	
		/// <summary>How much to move the image over by on the x axis. % or px.</summary>
		public Css.Value OffsetX;
		/// <summary>How much to move the image over by on the y axis. % or px.</summary>
		public Css.Value OffsetY;
		/// <summary>The width of the image (background-size property).</summary>
		public float OffsetOriginX;
		/// <summary>The origin that an offset is relative to.</summary>
		public float OffsetOriginY;
		/// <summary>The width of the image (background-size property).</summary>
		public Css.Value SizeX;
		/// <summary>The height of the image (background-size property).</summary>
		public Css.Value SizeY;
		/// <summary>True if the image should be repeated on the x axis.</summary>
		public bool RepeatX=true;
		/// <summary>True if the image should be repeated on the y axis.</summary>
		public bool RepeatY=true;
		/// <summary>The graphic to display.</summary>
		public ImagePackage Image;
		/// <summary>True if this image should be isolated regardless.</summary>
		public bool ForcedIsolate;
		/// <summary>The location of the image on an atlas if it's on one.</summary>
		public AtlasLocation ImageLocation;
		/// <summary>The filter mode to display the image with.</summary>
		public FilterMode Filtering=FilterMode.Point;
		
		
		/// <summary>Creates a new displayable background image for the given element.</summary>
		/// <param name='data'>The renderable object that will have a background image applied.</param>
		public BackgroundImage(RenderableData data):base(data){}
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				return 300;
			}
		}
		
		/// <summary>True if this paints across the whole background of the element.</summary>
		public override bool IsBackground{
			get{
				return true;
			}
		}
		
		/*
		public bool Overlaps(BackgroundImage image){
			
			// Get the first image block:
			MeshBlock block=image.FirstBlock;
			
			if(block==null || FirstBuffer==null){
				return false;
			}
			
			// Check if images verts are contained within any of mine.
			MeshBlock current=FirstBlock;
			
			while(current!=null){
				
				if(current.Overlaps(block)){
					
					return true;
				
				}
				
				current=current.LocalBlockAfter;
				
			}
			
			return false;
			
		}
		*/
		
		public override void OnBatchDestroy(){
			if(Image!=null){
				Image.GoingOffDisplay();
			}
			base.OnBatchDestroy();
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			// Any meshes in this elements queue should now change colour:
			Color colour=renderer.ColorOverlay;
			
			MeshBlock block=GetFirstBlock(renderer);
			
			// For each block..
			for(int i=0;i<BlockCount;i++){
				
				// Paint the colour:
				block.PaintColour(colour);
				
				// Go to next block:
				block.Next();
				
			}
			
		}
		
		/// <summary>Updates the contents of this image to the given texture.
		/// Does nothing if the texture hasn't changed.</summary>
		public void UpdateImage(Texture image){
			if(Image==null){
				SetImage(image);
				return;
			}
			
			// Get the picture format:
			PictureFormat picture=Image.Contents as PictureFormat;
			
			// Check if it changed:
			if(picture==null || picture.Image!=image){
				SetImage(image);
			}
			
		}
		
		/// <summary>Applies the given image to the background.</summary>
		/// <param name="image">The image to use.</param>
		public void SetImage(Texture image){
			Image=new ImagePackage(image);
			ImageReady(Image);
			ComputedStyle computed=RenderData.computedStyle;
			
			// Set the width:
			computed.ChangeTagProperty("width",Image.Width+"px");
			
			// And the height too:
			computed.ChangeTagProperty("height",Image.Height+"px");
			
			RenderData.Document.RequestLayout();
		}
		
		/// <summary>Manually apply an image package to the background of this element.</summary>
		public void SetImage(ImagePackage package){
			// Apply it:
			Image=package;
			
			// Let this object know the package is ready:
			ImageReady(package);
		}
		
		/// <summary>A callback used when the graphic has been loaded and is ready for display.</summary>
		public void ImageReady(ImagePackage package){
			if(Image==null || !Image.Loaded){
				return;
			}
			
			// Dispatch load (don't bubble, default):
			UIEvent e=new UIEvent("load");
			e.SetTrusted(false);
			RenderData.Node.dispatchEvent(e);
			
			RequestLayout();
			
			if(Image!=null && Filtering!=FilterMode.Point){
				Image.Contents.FilterMode=Filtering;
			}
		}
		
		internal override void NowOffScreen(){
			
			if(ImageLocation==null){
				return;
			}
			
			if(ImageLocation.DecreaseUsage()){
				ImageLocation=null;
			}
			
		}
		
		internal override bool NowOnScreen(){
			
			if(Image==null || !Image.Loaded){
				// Reject the visibility state change.
				ImageLocation=null;
				return false;
			}
			
			// Tell it that it's going on screen:
			Image.GoingOnDisplay(RenderData);
			
			// Must be PictureFormat to go onto the atlas:
			PictureFormat picture=Image.Contents as PictureFormat;
			
			if(picture==null){
				// Reject the visibility state change (only available to images).
				ImageLocation=null;
				return false;
			}
			
			if(Isolated){
				if(ImageLocation!=null){
					ImageLocation.DecreaseUsage();
					ImageLocation=null;
				}
				return true;
			}
			
			ImageLocation=RequireImage(Image);
			
			return true;
			
		}
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			if(Image==null || !Image.Loaded){
				return;
			}
			
			if(Clipping==BackgroundClipping.Text){
				return;
			}
			
			if(Image.Contents.Isolate || renderer.RenderMode==RenderMode.NoAtlas || Filtering!=FilterMode.Point || ForcedIsolate){
				// SPA is an animation format, so we need a custom texture atlas to deal with it.
				// This is because the frames of any animation would quickly exhaust our global texture atlas.
				// So to get a custom atlas, we must isolate this property.
				Isolate();
			}else{
				// Reverse isolation, if we are isolated already:
				Include();
			}
			
			// Get the full shape of the element:
			
			float width;
			float height;
			float minX;
			float minY;
			
			if(renderer.ViewportBackground){
				
				// Applying to whole background:
				BoxRegion viewport=renderer.Viewport;
				
				minY=(int)viewport.Y;
				minX=(int)viewport.X;
				width=(int)viewport.Width;
				height=(int)viewport.Height;
				
				renderer.ViewportBackground=false;
				
			}else{
				
				width=(int)(box.PaddedWidth);
				height=(int)(box.PaddedHeight);
				minY=(int)(box.Y+box.Border.Top);
				minX=(int)(box.X+box.Border.Left);
				
			}
			
			if(width==0||height==0){
				if(Visible){
					SetVisibility(false);
				}
				return;
			}
			
			// Tell the image that the box has likely changed - this allows it to redraw (e.g. SVGs):
			float trueImageWidth;
			float trueImageHeight;
			Image.Contents.OnLayout(RenderData,box,out trueImageWidth,out trueImageHeight);
			
			BoxRegion boundary=new BoxRegion(minX,minY,width,height);
			
			if(!boundary.Overlaps(renderer.ClippingBoundary)){
				
				if(Visible){
					SetVisibility(false);
				}
				
				return;
			}else if(!Visible){
				
				// ImageLocation will allocate here if it's needed.
				SetVisibility(true);
				
			}
			
			boundary.ClipBy(renderer.ClippingBoundary);
			
			// Texture time - get it's location on that atlas:
			AtlasLocation locatedAt=ImageLocation;
			
			if(locatedAt==null){
				// We're not using the atlas here.
				
				if(!Isolated){
					Isolate();
				}
				
				locatedAt=new AtlasLocation(trueImageWidth,trueImageHeight);
			}
			
			// Isolation is all done - safe to setup the batch now:
			renderer.SetupBatch(this,locatedAt.Atlas,null);
			
			// Great - Use locatedAt.Width/locatedAt.Height - this removes any risk of overflowing into some other image.
			
			int imageCountX=1;
			int imageCountY=1;
			float imageWidth=trueImageWidth * RenderData.ValueScale;
			float imageHeight=trueImageHeight * RenderData.ValueScale;
			bool autoX=false;
			bool autoY=false;
			
			if(SizeX!=null){
				
				if(SizeX.IsAuto){
					autoX=true;
				}else{
					imageWidth=SizeX.GetDecimal(RenderData,Css.Properties.BackgroundSize.GlobalPropertyX);
				}
				
			}
			
			if(SizeY!=null){
				
				if(SizeY.IsAuto){
					autoY=true;
				}else{
					imageHeight=SizeY.GetDecimal(RenderData,Css.Properties.BackgroundSize.GlobalPropertyY);
				}
				
			}
			
			if(autoX){
				
				imageWidth=imageHeight * trueImageWidth / trueImageHeight;
				
			}else if(autoY){
				
				imageHeight=imageWidth * trueImageHeight / trueImageWidth;
				
			}
			
			// offsetX and offsetY are the images position offset from where it should be (e.g. x of -200 means it's 200px left)
			
			// Apply the offset origin:
			float offsetX=OffsetOriginX * (width - imageWidth);
			float offsetY=OffsetOriginY * (height - imageHeight);
			
			float offset;
			
			// Resolve the offset values, if there is any:
			if(OffsetX!=null){
				
				offset=OffsetX.GetDecimal(RenderData,ValueAxis.X);
				
				if(OffsetOriginX==1f){
					offsetX-=offset;
				}else{
					offsetX+=offset;
				}
				
			}
			
			if(OffsetY!=null){
				
				offset=OffsetY.GetDecimal(RenderData,ValueAxis.Y);
				
				if(OffsetOriginY==1f){
					offsetY-=offset;
				}else{
					offsetY+=offset;
				}
				
			}
			
			if(RepeatX){
				// Get the rounded up number of images:
				imageCountX=(int)Math.Ceiling( width/imageWidth );
				
				if(offsetX!=0){
					// If we have an offset, another image is introduced.
					imageCountX++;
				}
			}
			
			if(RepeatY){
				// Get the rounded up number of images:
				imageCountY=(int)Math.Ceiling( height/imageHeight );
				if(offsetY!=0){
					// If we have an offset, another image is introduced.
					imageCountY++;
				}
			}
			
			float blockX=minX+offsetX;
			float blockY=minY+offsetY;
			
			if(RepeatX&&offsetX>0){
				// We're repeating and the image is offset by a +ve number.
				// This means a small gap, OffsetX px wide, is open on this left side.
				// So to fill it, we need to offset this first image by a much bigger number - the value imageWidth-OffsetX.
				blockX-=(imageWidth-offsetX);
				// This results in the first image having OffsetX pixels exposed in the box - this is what we want.
			}
			
			if(RepeatY&&offsetY>0){
				// Similar thing to above:
				blockY-=(imageHeight-offsetY);
			}
			
			BoxRegion screenRegion=new BoxRegion();

			bool first=true;
			float startX=blockX;
			Color colour=renderer.ColorOverlay;
			float zIndex=(RenderData.computedStyle.ZIndex-0.003f);
			
			Transformation transform=renderer.Transform;
			
			for(int y=0;y<imageCountY;y++){
				for(int x=0;x<imageCountX;x++){
					// Draw at blockX/blockY.
					screenRegion.Set(blockX,blockY,imageWidth,imageHeight);
					
					if(screenRegion.Overlaps(boundary)){
						// If the two overlap, this means it's actually visible.
						MeshBlock block=Add(renderer);
						
						if(first){
							
							first=false;
							
							if(Isolated){
								
								// Set current material:
								SetBatchMaterial(renderer,Image.Contents.GetImageMaterial(renderer.CurrentShaderSet.Isolated));
								
							}
							
						}
						
						// Set its colour:
						block.SetColour(colour);
						
						// And clip our meshblock to fit within boundary:
						block.TextUV=null;
						block.ImageUV=block.SetClipped(boundary,screenRegion,renderer,zIndex,locatedAt,block.ImageUV);
						
						block.Done(transform);
					}
					
					blockX+=imageWidth;
				}
				blockX=startX;
				blockY+=imageHeight;
			}
			
		}
		
	}
	
}