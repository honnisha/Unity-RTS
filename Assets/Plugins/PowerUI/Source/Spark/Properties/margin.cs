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


namespace Css.Properties{
	
	/// <summary>
	/// Represents the margin: css property.
	/// </summary>
	
	public class Margin:CssProperty{
		
		public static Margin GlobalProperty;
		public static CssProperty Top;
		public static CssProperty Right;
		public static CssProperty Bottom;
		public static CssProperty Left;
		
		
		public Margin(){
			GlobalProperty=this;
			InitialValue=ZERO;
		}
		
		public override string[] GetProperties(){
			return new string[]{"margin"};
		}
		
		public override void Aliases(){
			// E.g. margin-top:
			SquareAliases();
			
			// Quick references:
			Top=GetAliased(0);
			Right=GetAliased(1);
			Bottom=GetAliased(2);
			Left=GetAliased(3);
			
		}
		
		public override ApplyState Apply(ComputedStyle style,Value value){
			
			style.RequestLayout();
			
			// Ok!
			return ApplyState.Ok;
			
		}
		
		/// <summary>Computes the box for the given element now.</summary>
		public BoxStyle Compute(Css.Value value,RenderableData context,ComputedStyle parent,int display,int floatMode,ref bool dimsRequired){
			
			// Result:
			BoxStyle result=new BoxStyle();
			
			if(value==null){
				return result;
			}
			
			float marginSpace;
			
			// Right and left:
			Css.Value a=value[1];
			Css.Value b=value[3];
			
			// Default here is '0':
			if(a==null){
				a=Css.Value.Empty;
			}
			
			if(b==null){
				b=Css.Value.Empty;
			}
			
			if(a.IsAuto || b.IsAuto){
				
				// One or both are auto.
				
				// If we are an inline level element or we're floating then any 'auto' values are 0.
				if( floatMode!=0 || (display & DisplayMode.OutsideInline)!=0 ){
					
					if(!a.IsAuto){
						
						// Left auto, right is a number.
						result.Right=a.GetDecimal(context,Right);
						
					}else if(!b.IsAuto){
						
						// Right auto, left is a number.
						result.Left=b.GetDecimal(context,Left);
						
					}
					
				}else{
					
					if(!dimsRequired){
						// Dimensions are required - quit here.
						dimsRequired=true;
						return result;
					}
					
					if(parent!=null){
						
						while(parent.DisplayX==DisplayMode.Inline){
							
							// Special case - InnerWidth is invalid.
							
							// Go up the DOM to reach a flow-level element:
							IRenderableNode parentNode=parent.Element.parentNode as IRenderableNode;
							
							if(parentNode==null){
								break;
							}
							
							parent=parentNode.ComputedStyle;
							
						}
						
					}
					
					if(parent==null){
						
						// Parent is actually the document.
						marginSpace=context.Document.Viewport.Width-context.FirstBox.BorderedWidth;
						
					}else{
						
						// Get available margin space.
						marginSpace=parent.InnerWidth-context.FirstBox.BorderedWidth;
						
					}
					
					if(!a.IsAuto){
						
						// Left auto, right is a number.
						result.Right=a.GetDecimal(context,Right);
						result.Left=marginSpace-result.Right;
						
						// Auto doesn't create negatives:
						if(result.Left<0f){
							result.Left=0f;
						}
						
					}else if(!b.IsAuto){
						
						// Right auto, left is a number.
						result.Left=b.GetDecimal(context,Left);
						result.Right=marginSpace-result.Left;
						
						// Auto doesn't create negatives:
						if(result.Right<0f){
							result.Right=0f;
						}
						
					}else if(marginSpace>0f){ // No negatives allowed
						
						// Centering:
						result.Right=result.Left=marginSpace/2f;
						
					}
					
				}
				
			}else{
				
				// Both are ordinary numbers:
				result.Right=a.GetDecimal(context,Right);
				result.Left=b.GetDecimal(context,Left);
				
			}
			
			// Ignore margin top/bottom entirely on inline, in-flow elements.
			if(display!=DisplayMode.Inline){
				
				// Top and bottom:
				a=value[0];
				b=value[2];
				
				// Default here is '0':
				if(a==null){
					a=Css.Value.Empty;
				}
				
				if(b==null){
					b=Css.Value.Empty;
				}
				
				if(a.IsAuto || b.IsAuto){
					
					// One or both are auto.
					
					// If we are an inline level element or floating then any 'auto' values are 0.
					if( floatMode!=0 || (display & DisplayMode.OutsideInline)!=0 ){
						
						if(!a.IsAuto){
							
							// Bottom auto, top is a number.
							result.Top=a.GetDecimal(context,Top);
							
						}else if(!b.IsAuto){
							
							// Top auto, bottom is a number.
							result.Bottom=b.GetDecimal(context,Bottom);
							
						}
						
					}else{
						
						if(!dimsRequired){
							// Dimensions are required - quit here.
							dimsRequired=true;
							return result;
						}
						
						if(parent!=null){
						
							while(parent.DisplayX==DisplayMode.Inline){
								
								// Special case - InnerHeight is invalid.
								
								// Go up the DOM to reach a flow-level element:
								IRenderableNode parentNode=parent.Element.parentNode as IRenderableNode;
								
								if(parentNode==null){
									break;
								}
								
								parent=parentNode.ComputedStyle;
								
							}
							
						}
						
						if(parent==null){
							
							// Parent is actually the document.
							marginSpace=context.Document.Viewport.Height-context.FirstBox.BorderedHeight;
							
						}else{
							
							// Get available margin space.
							marginSpace=parent.InnerHeight-context.FirstBox.BorderedHeight;
							
						}
						
						if(!a.IsAuto){
							
							// Bottom auto, top is a number.
							result.Top=a.GetDecimal(context,Top);
							result.Bottom=marginSpace-result.Top;
							
							// Auto doesn't create negatives:
							if(result.Bottom<0f){
								result.Bottom=0f;
							}
							
						}else if(!b.IsAuto){
							
							// Top auto, bottom is a number.
							result.Bottom=b.GetDecimal(context,Bottom);
							result.Top=marginSpace-result.Bottom;
							
							// Auto doesn't create negatives:
							if(result.Top<0f){
								result.Top=0f;
							}
							
						}else if(marginSpace>0f){ // No negatives allowed
							
							// Centering:
							result.Top=result.Bottom=marginSpace/2f;
							
						}
						
					}
					
				}else{
					
					// Both are ordinary numbers:
					result.Top=a.GetDecimal(context,Top);
					result.Bottom=b.GetDecimal(context,Bottom);
					
				}
				
			}
			
			return result;
			
		}
		
	}
	
}



