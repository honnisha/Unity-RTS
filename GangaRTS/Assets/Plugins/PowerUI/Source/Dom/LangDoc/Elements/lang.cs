//--------------------------------------
//          Dom Framework
//
//        For documentation or 
//    if you have any issues, visit
//         wrench.kulestar.com
//
//    Copyright © 2013 Kulestar Ltd
//          www.kulestar.com
//--------------------------------------

namespace Dom{
	
	/// <summary>
	/// This handles the language meta tags which are present in the global languages.xml file and ignored everywhere else.
	/// </summary>
	
	[Dom.TagName("language,lang")]
	public class LangLanguageElement:LangElement{
		
		public override bool IsSelfClosing{
			get{
				return true;
			}
		}
		
		/// <summary>Applies this tag to the given language.</summary>
		/// <param name="language">The language that this tag came from.</param>
		public override void OnTagLoaded(){
			
			// Apply the language code if one is defined:
			string code=getAttribute("code");
			
			if(string.IsNullOrEmpty(code)){
				return;
			}
			
			code=(code).Trim().ToLower();
			
			if(group!=null){
				// Just ignore this tag. We're not in languages.xml.
				return;
			}
			
			// Create the meta:
			LanguageInfo meta=new LanguageInfo();
			
			if(code.IndexOf(',')!=-1){
				// More than one code for this language.
				string[] codes=code.Split(',');
				
				// The first one is the accepted HTML5 standard form:
				meta.code=codes[0];
				
				for(int i=0;i<codes.Length;i++){
					
					// Add it:
					LanguageInfo.All[codes[i]]=meta;
					
				}
				
			}else{
				meta.code=code;
				
				// Add it:
				LanguageInfo.All[code]=meta;
				
			}
			
			// Apply the name:
			meta.name=getAttribute("name");
			meta.localName=getAttribute("localName");
			
			// Apply text direction:
			string direction=getAttribute("direction");
			
			if(direction==null){
				direction=getAttribute("dir");
			}
			
			if(direction!=null){
				
				// Tidy it:
				direction=direction.Trim().ToLower();
				
				if(direction=="rtl" || direction=="righttoleft" || direction=="leftwards" || direction=="left"){
					meta.dir="rtl";
				}else{
					meta.dir="ltr";
				}
				
			}else{
				meta.dir="ltr";
			}
			
		}
		
	}
	
}