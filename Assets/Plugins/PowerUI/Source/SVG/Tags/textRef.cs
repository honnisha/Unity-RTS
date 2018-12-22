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

using Dom;

namespace Svg{
	
	/// <summary>
	/// A text reference tag.
	/// </summary>
	
	[Dom.TagName("tref")]
	public class SVGTRefElement : SVGTextPositioningElement{
		
		/// <summary>The element targeted with the href.</summary>
		private SVGElement _targetElement;
		
		/// <summary>The element targeted with the href.</summary>
		public SVGElement Target{
			get{
				
				if(_targetElement==null){
					_targetElement=TryResolveHref();
				}
				
				return _targetElement;
			}
		}
		
		/// <summary>The URL of the ref'd element.</summary>
		public string ReferencedElement{
			get{
				return getAttribute("href");
			}
			set{
				setAttribute("href", value);
			}
		}
		
		/*
		internal override IEnumerable<ISvgNode> GetContentNodes()
		{
			var refText = Document.GetElementById(ReferencedElement) as SVGTextContentElement;
			IEnumerable<ISvgNode> contentNodes = null;

			if (refText == null)
			{
				contentNodes = base.GetContentNodes();
			}
			else
			{
				contentNodes = refText.GetContentNodes();
			}

			contentNodes = contentNodes.Where(o => !(o is ISvgDescriptiveElement));

			return contentNodes;
		}
		*/
		
		
		
		public override bool OnAttributeChange(string property){
			
			if(property=="href"){
				
				// Try resolve now:
				_targetElement=TryResolveHref();
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			
			return true;
		}
		
	}
}
