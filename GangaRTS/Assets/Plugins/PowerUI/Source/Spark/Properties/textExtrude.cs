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
using Css.Properties;
using System.Collections;
using System.Collections.Generic;
using InfiniText;
using Blaze;
using PowerUI;


namespace Css.Properties{
	
	/// <summary>
	/// Represents the text-extrude: css property. This makes text 3D - it's best used in WorldUI's.
	/// Usage is text-extrude:[extrusion, float]. e.g. text-extrude:2.4
	/// </summary>
	
	public class TextExtrude:CssProperty{
		
		/// <summary>The CSS extrude property.</summary>
		public static TextExtrude GlobalProperty;
		/// <summary>Is the triangulator in inverse mode? You get interesting "inverted" shapes if you flip this!</summary>
		public const bool Inverse=false;
		/// <summary>Used to guage how many points in 3D space are used along a curve. The maximum "distance" in unscaled units between points.</summary>
		// public static float CurveAccuracy=0.1f;
		/// <summary>The triangulator that gets used. Cached and shared.</summary>
		private static Triangulator Triangulator_;
		
		/// <summary>The triangulator that gets used. Cached and shared.</summary>
		public static Triangulator Triangulator{
			get{
				
				if(Triangulator_==null){
					// Create triangulator:
					Triangulator_=new Triangulator(null,0,0);
					
					Triangulator_.Clockwise=Inverse;
				}
				
				return Triangulator_;
				
			}
		}
		
		public TextExtrude(){
			IsTextual=true;
			GlobalProperty=this;
			Inherits=true;
		}
		
		/// <summary>True if this property is specific to Spark.</summary>
		public override bool NonStandard{
			get{
				return true;
			}
		}
		
		public override string[] GetProperties(){
			return new string[]{"text-extrude"};
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			// Get the text:
			TextRenderingProperty text=GetText(style);
			
			if(text==null){
				// No text engine at all yet.
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			// Get extrude amount:
			float ext=0f;
			
			if(value!=null){
				
				ext=value.GetDecimal(style.RenderData,this);
				
				if(ext<0f){
					ext=0f;
				}
				
			}
			
			// if this is a flat UI, always set it to zero:
			if(((style.Element as Dom.Element).document as ReflowDocument).Renderer.IsFlat){
				ext=0f;
			}
			
			// Is it a Text3D?
			TextRenderingProperty3D text3D=text as TextRenderingProperty3D;
			
			if(text3D==null){
				
				if(ext==0f){
					// No extrude anyway.
					
					// Ok!
					return ApplyState.Ok;
					
				}
				
				// Time to convert it! Content was set before text-extrude.
				text3D=new TextRenderingProperty3D(style.RenderData);
				
				// Setup (applying it to the element etc).
				text3D.Setup(style);
				
			}else if(ext==0f){
				
				// Converting it back to an ordinary text engine.
				text=new TextRenderingProperty(style.RenderData);
				
				// Setup:
				text.Setup(style);
				
				// Ok!
				return ApplyState.Ok;
				
			}
			
			// Apply extrude:
			text3D.Extrude=ext;
			
			// Apply the changes:
			text3D.ClearText();
			text3D.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
	}
	
}

namespace Css{
	
	/// <summary>
	/// A TRP for rendering text in 3D. Gets created when CSS text-extrude is non-zero.
	/// </summary>
	public class TextRenderingProperty3D : TextRenderingProperty{
		
		/// <summary>The computed 3D extruded text, if there is any.</summary>
		public MeshBufferExtruded Mesh;
		/// <summary>The computed 3D extruded underline, if there is one.</summary>
		public MeshBufferExtruded Underline;
		/// <summary>How far this text is being extruded.</summary>
		public float Extrude;
		
		
		/// <summary>Creates a new text rendering property. Note that this must not be called directly.
		/// Set content: instead; if you're doing that from a tag, take a look at BR.</summary>
		/// <param name="data">The renderable object that this is rendering 3D text for.</param>
		public TextRenderingProperty3D(RenderableData data):base(data){}
		
		
		internal override void NowOffScreen(){
			base.NowOffScreen();
			
			if(Mesh!=null){
				Mesh.Destroy();
			}
			
			if(Underline!=null){
				Underline.Destroy();
			}
			
		}
		
		/// <summary>Draws a character with x-inverted UV's. Used for rendering e.g. "1 < 2" in right-to-left.</summary>
		protected override void DrawInvertCharacter(ref float left,Renderman renderer){
			
			float top=renderer.TopOffset;
			int index=renderer.CharacterIndex;
			
			Glyph character=Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Kerning!=null){
				left+=Kerning[index] * FontSize;
			}
			
			// Have we got a path at all? (Something like a space does not):
			if(character.FirstPathNode!=null){
				
				float y=top+renderer.TextAscender;
				
				float x=left + (character.LeftSideBearing * FontSize);
				
				// Map the point to worldspace:
				Vector3 worldspace=renderer.PixelToWorldUnit(x,y,RenderData.computedStyle.ZIndex);
				
				// Build the characters mesh now, using the shared triangulator:
				Mesh.AddMesh(character,worldspace.x,worldspace.y,TextExtrude.Triangulator);
				
			}
			
			left+=(character.AdvanceWidth * FontSize)+LetterSpacing;
		}
		
		/// <summary>Draws a character and advances the pen onwards.</summary>
		protected override void DrawCharacter(ref float left,Renderman renderer){
			
			float top=renderer.TopOffset;
			int index=renderer.CharacterIndex;
			
			Glyph character=Characters[index];
			
			if(character==null){
				return;
			}
			
			if(Kerning!=null){
				left+=Kerning[index] * FontSize;
			}
			
			if(character.Image!=null){
				base.DrawEmoji(character,ref left,renderer);
				return;
			}
			
			// Have we got a path at all? (Something like a space does not):
			if(character.FirstPathNode!=null){
			
				float y=top+renderer.TextAscender;
				
				float x=left + (character.LeftSideBearing * FontSize);
				
				// Map the point to worldspace:
				Vector3 worldspace=renderer.PixelToWorldUnit(x,y,RenderData.computedStyle.ZIndex);
				
				// Build the characters mesh now, using the shared triangulator:
				Mesh.AddMesh(character,worldspace.x,worldspace.y,TextExtrude.Triangulator);
				
			}
			
			left+=(character.AdvanceWidth * FontSize)+LetterSpacing;
			
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			Color fontColour=BaseColour * renderer.ColorOverlay;
			
			if(Mesh!=null){
				// Update it's verts to match the font colour.
				Mesh.PaintColours(fontColour,fontColour,fontColour);
			}
			
			if(Underline!=null){
				// Update it's verts to match the line colour.
				Color lineColour=fontColour;
				
				if(TextLine!=null && TextLine.ColourOverride){
					lineColour=TextLine.BaseColour * renderer.ColorOverlay;
				}
				
				Underline.PaintColours(lineColour,lineColour,lineColour);
			}
			
		}
		
		public override void RequestPaint(){
			
			if(Mesh==null){
				RenderData.RequestLayout();
			}else{
				RenderData.RequestPaint();
			}
			
		}
		
		/// <summary>Draws an underline (or a strikethrough).</summary>
		public override void DrawUnderline(Renderman renderer){
			
			// Setup the mesh:
			if(Underline==null){
				
				// We'll need the root gameObject - this should always be on a TextNode, but the parentNode
				// will always be an Element. Therefore we can grab the gameObject via that parentNode:
				HtmlElement parent=RenderData.Node.parentNode as HtmlElement;
				
				Underline=new MeshBufferExtruded(parent.rootGameObject.transform);
				
				// Update material:
				Underline.SetMaterial(new Material(renderer.CurrentShaderSet.Extruded));
				
				Underline.ExtrudeAmount=Extrude;
				
			}
			
			// Update colours:
			Underline.FrontColor=renderer.FontColour;
			Underline.BackColor=renderer.FontColour;
			Underline.SideColor=renderer.FontColour;
			
			BoxRegion region=renderer.CurrentRegion;
			
			// 4 corners (back face):
			Vector3 a=renderer.PixelToWorldUnit(region.X,region.Y,renderer.TextDepth);
			Vector3 b=renderer.PixelToWorldUnit(region.MaxX,region.Y,renderer.TextDepth);
			Vector3 c=renderer.PixelToWorldUnit(region.MaxX,region.MaxY,renderer.TextDepth);
			Vector3 d=renderer.PixelToWorldUnit(region.X,region.MaxY,renderer.TextDepth);
			
			// Build a simple cube:
			Underline.BuildCube(a,b,c,d);
			
		}
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			if(Characters==null){
				return;
			}
			
			// Setup the mesh:
			if(Mesh==null){
				
				// We'll need the root gameObject - this should always be on a TextNode, but the parentNode
				// will always be an Element. Therefore we can grab the gameObject via that parentNode:
				HtmlElement parent=RenderData.Node.parentNode as HtmlElement;
				
				Mesh=new MeshBufferExtruded(parent.rootGameObject.transform);
				
				// Update material:
				Mesh.SetMaterial(new Material(renderer.CurrentShaderSet.Extruded));
				
				Mesh.ExtrudeAmount=Extrude;
				
			}
			
			// Get the font colour:
			Color fontColour=BaseColour * renderer.ColorOverlay;
			
			// Update colours:
			Mesh.FrontColor=fontColour;
			Mesh.BackColor=fontColour;
			Mesh.SideColor=fontColour;
			
			// Update size:
			Mesh.XScaleFactor=FontSize;
			Mesh.YScaleFactor=FontSize;
			
			int verts=0;
			int tris=0;
			
			for(int i=0;i<Characters.Length;i++){
				
				// Add vert/ tri count:
				Glyph character=Characters[i];
				
				if(character==null){
					continue;
				}
				
				int localTriCount;
				verts+=Mesh.GetVertCount(character,out localTriCount);
				
				tris+=localTriCount;
				
			}
			
			// Apply the size:
			Mesh.RequireSize(verts,tris);
			
			// Draw now!
			base.Layout(box,renderer);
			
			// Flush the mesh:
			Mesh.Flush();
			
		}
		
	}
	
}



