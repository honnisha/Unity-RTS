using System;


namespace Loonim{
	
    /// <summary>
    /// Generates a noisy curve. Ranges from 0.5 + (amplitude * value). Amplitude of 1 makes it range from 0-1.
    /// </summary>
    public class NoiseCurve:TextureNode{
		
		private Perlin PerlinProvider;
		
		
		internal override int OutputDimensions{
			get{
				// 1D.
				return 1;
			}
		}
		
		public TextureNode AmplitudeModule{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode Octaves{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
				PerlinProvider.OctaveCount=value;
			}
		}
		
		public TextureNode LacunarityModule{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
				PerlinProvider.Lacunarity=value;
			}
		}
		
		public TextureNode PersistenceModule{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
				PerlinProvider.Persistence=value;
			}
		}
		
		
		public NoiseCurve():base(4){
			PerlinProvider=new Perlin();
		}
		
		public override double GetValue(double x,double y){
			
			double amplitude=AmplitudeModule.GetValue(x) * 0.5;
			
			return 0.5 + (PerlinProvider.GetValue(x,0.0)+1) * amplitude;
			
		}
		
		public override double GetValue(double x){
			
			double amplitude=AmplitudeModule.GetValue(x) * 0.5;
			
			return 0.5 + (PerlinProvider.GetValue(x,0.0)+1) * amplitude;
			
		}
		
		public override int TypeID{
			get{
				return 108;
			}
		}
		
	}
	
}