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


namespace Svg{

	/// <summary>
	/// Description of SvgAspectRatio.
	/// </summary>
	public class AspectRatio{
		
		public bool Slice;
		public bool Defer;
		public SVGPreserveAspectRatio Align;
		
		
		public AspectRatio() : this(SVGPreserveAspectRatio.none){}
		
		public AspectRatio(SVGPreserveAspectRatio align): this(align, false){}
		
		public AspectRatio(SVGPreserveAspectRatio align, bool slice){
			Align = align;
			Slice = slice;
			Defer = false;
		}
		
		public AspectRatio(string text){
			LoadFrom(text);
		}
		
		public void LoadFrom(string text){
			
			SVGPreserveAspectRatio eAlign = SVGPreserveAspectRatio.none;
			bool bDefer = false;
			bool bSlice = false;

			string[] sParts = text.Split(new char[] {' '}, StringSplitOptions.RemoveEmptyEntries);
			int nAlignIndex = 0;
			
			if (sParts[0].Equals("defer")){
				
				bDefer = true;
				nAlignIndex++;
				
				if(sParts.Length < 2){
					return;
				}
				
			}

			eAlign = (SVGPreserveAspectRatio)Enum.Parse(typeof(SVGPreserveAspectRatio), sParts[nAlignIndex]);
			
			nAlignIndex++;

			if (sParts.Length > nAlignIndex){
				
				switch (sParts[nAlignIndex]){
					
					case "meet":
						break;
					case "slice":
						bSlice = true;
						break;
					default:
						return;
					
				}
				
			}
			
			Align=eAlign;
			Slice = bSlice;
			Defer = bDefer;

		}
		
		public override string ToString()
		{
			return Align.ToString() + (Slice ? " slice" : "");
		}

	}
	
	public enum SVGPreserveAspectRatio
	{
		xMidYMid, //default
		none,
		xMinYMin,
		xMidYMin,
		xMaxYMin,
		xMinYMid,
		xMaxYMid,
		xMinYMax,
		xMidYMax,
		xMaxYMax
	}
}
