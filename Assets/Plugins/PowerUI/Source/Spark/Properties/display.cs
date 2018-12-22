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

namespace Css.Properties{
	
	/// <summary>
	/// Represents the display: css property.
	/// </summary>
	
	public class Display:CssProperty{
		
		public static Display GlobalProperty;
		
		public Display(){
			GlobalProperty=this;
			InitialValueText="inline";
		}
		
		public override string[] GetProperties(){
			return new string[]{"display"};
		}
		
		/// <summary>flow or flow-root.</summary>
		private const int InsideFlowOrRoot=DisplayMode.InsideFlow | DisplayMode.InsideFlowRoot;
		
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			int display=0;
			
			if(value==null){
				
				// Assume inline if blank:
				display=DisplayMode.Inline;
				
			}else if(value is ValueSet){
				
				// Outside + Inside or list-item
				int first=value[0].GetInteger(null,null);
				
				if(first==DisplayMode.ListItem){
					
					// Get outside mode:
					display=DisplayMode.InternalListItem | value[1].GetInteger(null,null);
					
					if(value.Count>2){
						
						// Get inside mode (just flow or flow-root allowed):
						int insideMode=value[2].GetInteger(null,null);
						
						if((insideMode & InsideFlowOrRoot)!=0){
							display|=insideMode;
						}
						
					}
					
				}else{
					
					display=first | value[1].GetInteger(null,null);
					
				}
				
			}else{
				
				// "Legacy" or other shortform.
				display=value.GetInteger(null,null);
				
				// special case for list-item as we'll be creating the marker too:
				if(display==DisplayMode.ListItem){
					
					// Update the marker selector:
					MarkerSelector.Update(style);
					
				}else{
					
					// (Remove marker if there is one)
					style.RemoveVirtual(MarkerSelector.Priority);
					
				}
				
			}
			
			// Write the display value to the style:
			style[GlobalProperty]=new Css.Units.CachedIntegerUnit(value,display);
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.ReloadValue;
			
		}
		
	}
	
}