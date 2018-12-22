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
using Dom;
using PowerUI;


namespace Css.Units{
	
	/// <summary>
	/// Represents an instance of a % unit.
	/// </summary>
	
	public class PercentUnit:DecimalUnit{
		
		public PercentUnit(){
			// Relative:
			Type=ValueType.RelativeNumber;
		}
		
		public PercentUnit(float range01):this(){
			RawValue=range01;
		}
		
		public override void OnValueReady(CssLexer lexer){
			// Map to 0-1 range:
			RawValue /= 100f;
		}
		
		public override float DisplayNumber{
			get{
				return (RawValue * 100f);
			}
		}
		
		public override string ToString(){
			return DisplayNumber+"%";
		}
		
		protected override Value Clone(){
			PercentUnit result=new PercentUnit();
			result.RawValue=RawValue;
			return result;
		}
		
		public override float GetDecimal(RenderableData context,CssProperty property){
			
			if(property==null || context==null){
				return RawValue;
			}
			
			RenderableData parent;
			Viewport port;
			ComputedStyle parentStyle;
			ComputedStyle computed=context.computedStyle;
			
			// Is this along x?
			float relative;
			
			switch(property.RelativeTo){
				
				case ValueRelativity.None:
					
					// Just raw value:
					return RawValue;
					
				case ValueRelativity.Self:
					
					parent=context.Ancestor;
					
					if(parent==null){
						
						relative=0;
						
					}else{
						
						// Relative to itself:
						parentStyle=parent.computedStyle;
						Css.Value parentValue=parentStyle[property];
						
						if(parentValue==null){
							// Read the default value:
							relative=property.InitialValue.GetDecimal(parent,property);
						}else{
							// Get the relative value:
							relative=parentValue.GetDecimal(parent,property);
						}
						
					}
					
				break;
				
				case ValueRelativity.FontSize:
					
					// Read from the parent:
					parent=context.Ancestor;
					
					if(parent==null){
						
						// E.g. a document.
						relative=16f;
						
					}else{
						
						parentStyle=parent.computedStyle;
						relative=parentStyle.FontSizeX;
						
					}
					
				break;
				
				case ValueRelativity.LineHeight:
					
					// Read from the parent:
					parent=context.Ancestor;
					
					if(parent==null){
						
						// E.g. a document.
						relative=1.2f;
						
					}else{
						
						parentStyle=parent.computedStyle;
						relative=parentStyle.LineHeightFullX;
						
					}
					
				break;
				
				default:
				case ValueRelativity.Dimensions:
					
					// Get the parent:
					parent=context.Ancestor;
					
					if(parent==null){
						
						// No parent - use the viewport (e.g. fixed):
						
						// Get the viewport for the document:
						port=context.Document.Viewport;
						
						switch(property.Axis){
							
							default:
							case ValueAxis.None:
								// Diagonal
								relative=port.Diagonal;
							break;
							case ValueAxis.X:
								// X!
								relative=port.Width;
							break;
							case ValueAxis.Y:
								// Y!
								relative=port.Height;
							break;
						}
						
					}else{
						
						parentStyle=parent.computedStyle;
						LayoutBox box=parentStyle.FirstBox;
						
						if(box==null){
							relative=0f;
						}else if(property.Axis==ValueAxis.X){
							// Yep!
							relative=box.InnerWidth;
						}else{
							// Nope!
							relative=box.InnerHeight;
						}
						
					}
					
				break;
				case ValueRelativity.SelfDimensions:
					
					// Note: This is only used during rendering after all elements know their sizes.
					
					if(computed.FirstBox==null){
						relative=0f;
					}else if(property.Axis==ValueAxis.X){
						// Yep!
						relative=computed.FirstBox.PaddedWidth;
					}else{
						// Nope!
						relative=computed.FirstBox.PaddedHeight;
					}
					
				break;
				case ValueRelativity.Viewport:
					
					// Get the viewport for the document:
					port=context.Document.Viewport;
					
					switch(property.Axis){
						
						default:
						case ValueAxis.None:
							// Diagonal
							relative=port.Diagonal;
						break;
						case ValueAxis.X:
							// X!
							relative=port.Width;
						break;
						case ValueAxis.Y:
							// Y!
							relative=port.Height;
						break;
					}
					
				break;
			}
			
			return RawValue * relative;
			
		}
		
		public override string[] PostText{
			get{
				return new string[]{"%"};
			}
		}
		
	}
	
}



