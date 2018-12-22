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
using Blaze;
using UnityEngine;
using Css;
using Loonim;


namespace Svg{
	
	/// <summary>
	/// The base of SVG text tags.
	/// </summary>
	
	public class SVGTextContentElement:SVGGraphicsElement{
		
		/// <summary>Text fills black by default. This is the fill handler. Initiated when first required.</summary>
		private static TextureNode _blackFill=null;
		
		/// <summary>Text fills black by default. This is the fill handler. Initiated when first required.</summary>
		public static TextureNode BlackFill{
			get{
				
				if(_blackFill==null){
					_blackFill=new Property(Color.black);
				}
				
				return _blackFill;
			}
		}
		
		protected Css.ValueSet _x = new Css.ValueSet();
		protected Css.ValueSet _y = new Css.ValueSet();
		protected Css.ValueSet _dx = new Css.ValueSet();
		protected Css.ValueSet _dy = new Css.ValueSet();
		protected Css.Value _textLength = Css.Value.Empty;
		protected Css.Value _lengthAdjust = Css.Value.Empty;
		private float[] _rotations;
		
		
		/// <summary>
		/// Gets or sets the x.
		/// </summary>
		/// <value>The x.</value>
		public virtual Css.ValueSet X{
			
			get{
				return _x;
			}
			set{
				
				if(!_x.Equals(value)){
					this._x = value;
				}
				
			}
		}
		
		/// <summary>
		/// Gets or sets the y.
		/// </summary>
		/// <value>The y.</value>
		public virtual Css.ValueSet Y{
			
			get{
				return _y;
			}
			set{
				
				if(!_y.Equals(value)){
					this._y = value;
				}
				
			}
		}
		
		/// <summary>
		/// Gets or sets the dX.
		/// </summary>
		/// <value>The dX.</value>
		public virtual Css.Value TextLength{
			
			get{
				return _textLength;
			}
			set{
				
				if(!_textLength.Equals(value)){
					_textLength = value;
				}
				
			}
		}
		
		/// <summary>
		/// Gets or sets the dX.
		/// </summary>
		/// <value>The dX.</value>
		public virtual Css.Value LengthAdjust{
			
			get{
				return _lengthAdjust;
			}
			set{
				
				if(!_lengthAdjust.Equals(value)){
					_lengthAdjust = value;
				}
				
			}
		}
		
		/// <summary>Specifies spacing behavior between words.</summary>
		public virtual Css.Value LetterSpacing{
			get{
				return style.Computed["letter-spacing"];
			}
			set{
				style["letter-spacing"]=value;
			}
		}
		
		/// <summary>Specifies spacing behavior between words.</summary>
		public virtual Css.Value WordSpacing{
			get{
				return style.Computed["word-spacing"];
			}
			set{
				style["word-spacing"]=value;
			}
		}
		
		/// <summary>
		/// Gets or sets the fill.
		/// </summary>
		/// <remarks>
		/// <para>Unlike other <see cref="SvgGraphicsElement"/>s, <see cref="SvgText"/> has a default fill of black rather than transparent.</para>
		/// </remarks>
		/// <value>The fill.</value>
		public override TextureNode Fill{
			get{
				
				TextureNode fill=base.Fill;
				
				if(fill==null){
					return BlackFill;
				}
				
				return fill;
				
			}
		}
		
		/// <summary>
		/// Gets or sets the dX.
		/// </summary>
		/// <value>The dX.</value>
		public virtual Css.ValueSet Dx{
			
			get{
				return _dx;
			}
			set{
				
				if(!_dx.Equals(value)){
					this._dx = value;
				}
				
			}
		}
		
		/// <summary>
		/// Gets or sets the dY.
		/// </summary>
		/// <value>The dY.</value>
		public virtual Css.ValueSet Dy{
			
			get{
				return _dy;
			}
			set{
				
				if(!_dy.Equals(value)){
					this._dy = value;
				}
				
			}
		}
		
		/// <summary>
		/// The text to be rendered.
		/// </summary>
		public virtual string Text{
			
			get{
				return style.content;
			}
			
			set{
				style.content=value;
			}
			
		}
		
		public virtual string Rotate{
			
			get{
				return getAttribute("rotate");
			}
			set{
				
				// Load the values:
				string[] rots=value.Split(new char[] { ',', ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
				
				// Create rotations:
				_rotations=new float[rots.Length];
				
				// Load each one..
				for(int i=0;i<rots.Length;i++){
					
					_rotations[i]=float.Parse(rots[i]);
					
				}
				
			}
			
		}
		
		public override bool OnAttributeChange(string property){
			
			if(property=="x"){
				
				X=Css.Value.Load(getAttribute("x")).ToSet();
				
			}else if(property=="y"){
				
				Y=Css.Value.Load(getAttribute("y")).ToSet();
				
			}else if(property=="dx"){
				
				Dx=Css.Value.Load(getAttribute("dx")).ToSet();
				
			}else if(property=="dy"){
				
				Dy=Css.Value.Load(getAttribute("dy")).ToSet();
				
			}else if(property=="rotate"){
				
				Rotate=getAttribute("rotate");
				
			}else if(property=="textlength"){
				
				TextLength=Css.Value.Load(getAttribute("textlength"));
				
			}else if(property=="lengthadjust"){
				
				LengthAdjust=Css.Value.Load(getAttribute("lengthadjust"));
				
			}else if(!base.OnAttributeChange(property)){
				return false;
			}
			
			return true;
			
		}
		
		/// <summary>
		/// Gets the bounds of the element.
		/// </summary>
		/// <value>The bounds.</value>
		public override BoxRegion Bounds
		{
			get 
			{
				return GroupBounds;
			}
		}
		
		public override string ToString(){
			return this.Text;
		}
		
	}
	
}