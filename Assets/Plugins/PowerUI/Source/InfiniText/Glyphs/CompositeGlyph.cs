//--------------------------------------
//             InfiniText
//
//        For documentation or 
//    if you have any issues, visit
//        powerUI.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

using System;
using Blaze;


namespace InfiniText{

	public class CompositeGlyph:Glyph{
		
		public VectorTransform LastComponent;
		public VectorTransform FirstComponent;
		
		
		public CompositeGlyph(FontFace parent):base(parent){
			
		}
		
		public override bool RequiresLoad{
			get{
				return (FirstComponent!=null && FirstPathNode==null);
			}
		}
		
		public override void LoadNow(){
			
			// Clear:
			FirstPathNode=null;
			LatestPathNode=null;
			PathNodeCount=0;
			
			Glyph[] glyphs=Font.ParserGlyphs;
			
			VectorTransform current=FirstComponent;
			
			while(current!=null){
				
				Glyph componentGlyph=glyphs[current.Index];
				
				if(componentGlyph!=null){
					
					if(componentGlyph.RequiresLoad){
						// Load it:
						componentGlyph.LoadNow();
					}
					
				}
				
				if(componentGlyph.WasHoleSorted){
					// Sort all of them.
					HoleSorted=true;
				}
				
				current=current.Next;
				
			}
			
			// Load fully:
			LoadFully(glyphs);
			
			// Reduce the amount of unloaded glyphs:
			Font.UnloadedGlyphs--;
			
			if(Font.UnloadedGlyphs<=0){
				// Let the font know that every glyph is now loaded:
				Font.AllGlyphsLoaded();
			}
			
		}
		
		public void AddComponent(VectorTransform component){
			
			if(FirstComponent==null){
				
				// Set as only one:
				FirstComponent=LastComponent=component;
				
			}else{
				
				// Push to the end:
				LastComponent=LastComponent.Next=component;
				
			}
			
		}
		
		public override bool IsComposite{
			get{
				return true;
			}
		}
		
		public override void LoadFully(Glyph[] glyphs){
			
			VectorTransform current=FirstComponent;
			
			while(current!=null){
				
				Glyph componentGlyph=glyphs[current.Index];
				
				if(componentGlyph!=null){
					
					// Transform the points of the component glyph into this one: 
					TransformPoints(componentGlyph,current);
					
				}
				
				current=current.Next;
				
			}
			
		}
		
		private void TransformPoints(Glyph fromGlyph,VectorTransform transform){
			
			if(HoleSorted && !fromGlyph.WasHoleSorted){
				// We must sort all components:
				fromGlyph.HoleSort();
			}
			
			VectorPoint current=fromGlyph.FirstPathNode;
			
			while(current!=null){
				
				// Create a new one:
				VectorPoint newPoint = current.Copy();
				
				// Apply transformed pos:
				newPoint.Transform(transform);
				
				// Add it:
				AddPathNode(newPoint);
				
				current=current.Next;
				
				if(current==null){
					// Last one - ensure it's closed:
					newPoint.IsClose=true;
				}
				
			}
			
		}
		
	}

}