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
using PowerUI;
using System.Collections;
using System.Collections.Generic;


namespace Css{
	
	/// <summary>
	/// Some properties like background and text-shadow can have multiple renderers.
	/// When that happens, a group of those properties is created.
	/// </summary>
	
	public class DisplayablePropertyGroup:DisplayableProperty{
		
		/// <summary>The contents of the group.</summary>
		public List<DisplayableProperty> Contents=new List<DisplayableProperty>(2);
		
		
		public DisplayablePropertyGroup(RenderableData data,DisplayableProperty a,DisplayableProperty b):base(data){
			
			// Add the first two:
			Contents.Add(a);
			Contents.Add(b);
			
		}
		
		
		/// <summary>This property's draw order.</summary>
		public override int DrawOrder{
			get{
				return Contents[0].DrawOrder;
			}
		}
		
		/// <summary>True if this paints across the whole background of the element.</summary>
		public override bool IsBackground{
			get{
				return Contents[0].IsBackground;
			}
		}
		
		public override void Paint(LayoutBox box,Renderman renderer){
			
			for(int i=0;i<Contents.Count;i++){
				
				Contents[i].Paint(box,renderer);
				
			}
			
		}
		
		/// <summary>Transforms all the verts by the given delta matrix. Used during a Paint only.</summary>
		public override void ApplyTransform(Matrix4x4 delta,Renderman renderer){
			
			for(int i=0;i<Contents.Count;i++){
				
				Contents[i].ApplyTransform(delta,renderer);
				
			}
			
		}
		
		public override void OnBatchDestroy(){
			
			for(int i=0;i<Contents.Count;i++){
				
				Contents[i].OnBatchDestroy();
				
			}
			
		}
		
		internal override void NowOffScreen(){
			
			for(int i=0;i<Contents.Count;i++){
				
				Contents[i].NowOffScreen();
				
			}
			
		}
		
		internal override bool NowOnScreen(){
			
			bool res=false;
			
			for(int i=0;i<Contents.Count;i++){
				
				if(Contents[i].NowOnScreen()){
					res=true;
				}
				
			}
			
			return res;
			
		}
		
		internal override void Layout(LayoutBox box,Renderman renderer){
			
			for(int i=0;i<Contents.Count;i++){
				
				Contents[i].Layout(box,renderer);
				
			}
			
		}
		
	}
	
}