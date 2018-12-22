using System;
using UnityEngine;
using Blaze;
using Values;

namespace Loonim
{
	
	/// <summary>
	/// Pulls in a property value from the surfaces property set.
	/// </summary>
	
    public class Property: TextureNode{
		
		/// <summary>True if the property is holding a texture.</summary>
		private bool IsTexture;
		/// <summary>The property this points at.</summary>
		public SurfaceProperty SurfaceProperty;
		/// <summary>The pulled value as a colour.</summary>
		public Color Colour;
		/// <summary>The pulled value as a double.</summary>
		public double Value;
		
		
		public Property(){}
		
		public Property(SurfaceProperty prop){
			SurfaceProperty=prop;
			
			// Pull the values right now:
			Prepare(null);
		}
		
		public Property(SurfaceTexture tex,string name){
			SurfaceProperty=tex.GetProperty(name,true);
			IsTexture=(SurfaceProperty.Value is TextureValue);
			
			// Pull the values right now:
			Prepare(null);
		}
		
		public Property(double value){
			Value=value;
			float v=(float)value;
			Colour=new Color(v,v,v,1f);
		}
		
		public Property(Color value){
			Colour=value;
			Value=(Colour.r + Colour.g + Colour.b) / 3.0;
		}
		
		public override void Read(TextureReader reader){
			
			// Property ID:
			int index=(int)reader.ReadCompressed();
			
			// Get from the current surface being loaded:
			SurfaceProperty=reader.CurrentSurface.Properties[index];
			IsTexture=(SurfaceProperty.Value is TextureValue);
			
		}
		
		/// <summary>Allocates GPU drawing meta now.</summary>
		public override DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			if(SurfaceProperty!=null){
				
				// Clear changed:
				SurfaceProperty.Value.Changed=false;
				
				if(IsTexture){
					
					// Always just a single const node:
					LiveStackNode constNode=new LiveStackNode(this,(SurfaceProperty.Value as TextureValue).Value);
					DrawStore=constNode;
					
					// Ok:
					return constNode;
					
				}
				
			}
			
			// Base:
			return base.Allocate(info,tex,ref stackID);
			
		}
		
		/// <summary>Updates the cached values if the SurfaceProperty this points at has changed.</summary>
		public bool UpdateIfChanged(DrawInfo info){
			
			if(SurfaceProperty!=null && SurfaceProperty.Value.Changed){
				
				// It's changed!
				SurfaceProperty.Value.Changed=false;
				
				// Reload:
				Prepare(info);
				
				return true;
				
			}
			
			return false;
			
		}
		
		public override void Draw(DrawInfo info){
			
			UpdateIfChanged(info);
			base.Draw(info);
			
		}
		
		internal override int OutputDimensions{
			get{
				if(IsTexture){
					// Texture node.
					return 2;
				}
				// Constant - no dimensions.
				return 0;
			}
		}
		
		public override void Prepare(DrawInfo info){
			
			// "Bake" surface property now, if we have one:
			if(SurfaceProperty==null){
				return;
			}
			
			PropertyValue value=SurfaceProperty.Value;
			
			// Numeric or colour?
			ColourValue col=value as ColourValue;
			
			if(col!=null){
				
				// It's a colour!
				Colour=col.Value;
				
				// Value is intensity:
				Value=(Colour.r + Colour.g + Colour.b) / 3.0;
				
			}else{
				
				// Try as numeric instead:
				NumericValue num=value as NumericValue;
				
				if(num!=null){
					
					// Get it as a double:
					Value=num.ToDouble();
					
					// Create a colour:
					float c=(float)Value;
					Colour=new Color(c,c,c,1f);
					
				}else{
					
					// White:
					Colour=new Color(1f,1f,1f,1f);
					Value=1.0;
					
				}
				
			}
			
			// Reset DrawStore:
			if(IsTexture){
				
				// Pull the texture:
				LiveStackNode lsn=(DrawStore as LiveStackNode);
				
				if(lsn!=null){
					
					// Set:
					lsn.Image=(SurfaceProperty.Value as TextureValue).Value;
					
					// We may now need to bump parent if we're in a Draw() call;
					// i.e. it needs to hook up this new image to its material.
					if(info.CurrentParent!=null && info.CurrentParent.DrawStore!=null){
						
						// Link away!
						Material material=info.CurrentParent.DrawStore.NodeMaterial;
						
						// Reset _srcX:
						material.SetTexture("_Src"+info.CurrentIndex,lsn.Image);
						
					}
					
				}
				
			}else{
				
				TextureStackNode tsn=(DrawStore as TextureStackNode);
				
				if(tsn!=null){
					
					// Bake:
					tsn.Bake();
					
				}
				
			}
			
		}
		
		public override UnityEngine.Color GetColour(double x,double y){
			
			return Colour;
			
		}
		
        public override double GetWrapped(double x, double y, int wrap){
			return Value;
		}
		
        public override double GetValue(double x, double y, double z){
			return Value;
		}
		
        public override double GetValue(double x, double y){
			return Value;
        }
		
		public override int TypeID{
			get{
				return 3;
			}
		}
		
    }
	
}
