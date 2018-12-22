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


namespace Css.Functions{
	
	/// <summary>
	/// Represents the attr() css function.
	/// </summary>
	
	public class Attr:CssFunction{
		
		public Attr(){
			
			Name="attr";
			Type=ValueType.Text;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"attr"};
		}
		
		protected override Css.Value Clone(){
			Attr result=new Attr();
			result.Values=CopyInnerValues();
			return result;
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			string result="";
			
			int count=Count;
			
			for(int i=0;i<count;i++){
				
				// Grab the value:
				Css.Value par=this[i];
				
				if(par==null){
					continue;
				}
				
				// Add the textual value to result:
				result+=par.GetText(context,property);
				
			}
			
			// Great - map to attrib:
			result=context.Node.getAttribute(result);
			
			return result;
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			float result;
			float.TryParse(GetText(context,property),out result);
			return result;
			
		}
		
		public override bool GetBoolean(RenderableData context,CssProperty property){
			
			string text=GetText(context,property);
			
			return (!string.IsNullOrEmpty(text) && text!="false" && text!="FALSE" && text!="0" && text!="no");
			
		}
		
	}
	
}