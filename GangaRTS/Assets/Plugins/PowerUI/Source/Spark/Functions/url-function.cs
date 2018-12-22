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
	/// Represents the url() css function.
	/// </summary>
	
	public partial class UrlFunction:CssFunction{
		
		/// <summary>The cached URL.</summary>
		public string Url;
		/// <summary>The sheet that this originates from.</summary>
		public StyleSheet Sheet;
		
		
		public UrlFunction(){
			
			Name="url";
			Type=ValueType.Text;
			LiteralValue=true;
			
		}
		
		public override void OnValueReady(CssLexer lexer){
			
			// Cache the sheet:
			Sheet=lexer.Sheet;
			
		}
		
		public override string[] GetNames(){
			return new string[]{"url","url-prefix","domain","local"};
		}
		
		protected override Css.Value Clone(){
			UrlFunction result=new UrlFunction();
			result.Values=CopyInnerValues();
			return result;
		}
		
		private string GetUrl(RenderableData context,CssProperty property){
			
			int length=Count;
			
			if(length==0){
				return "";
			}
			
			string result;
			
			if(length==1){
				
				result=this[0].GetText(context,property);
			
			}else{
				
				result="";
				
				for(int i=0;i<length;i++){
					
					// Grab the value:
					Css.Value par=this[i];
					
					// Add the textual value to result:
					result+=par.GetText(context,property);
					
				}
				
			}
			
			// Make result relative to the stylesheet that this was declared in:
			if(Sheet!=null){
				
				if(Sheet.Location!=null){
					
					// Make relative now:
					result=(new Dom.Location(result,Sheet.Location)).absoluteNoHash;
					
				}
				
			}
			
			return result;
			
		}
		
		public override string GetText(RenderableData context,CssProperty property){
			
			if(Url==null){
				Url=GetUrl(context,property);
			}
			
			return Url;
			
		}
		
	}
	
}