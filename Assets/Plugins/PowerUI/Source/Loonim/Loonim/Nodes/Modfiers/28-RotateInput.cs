using System;
using UnityEngine;

namespace Loonim{
	
	public class RotateInput : TextureNode{
		
		public TextureNode XAngle{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		public TextureNode YAngle{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		public TextureNode ZAngle{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		private double[] Matrix;
		
		public RotateInput():base(4){}

		public override UnityEngine.Color GetColour(double x,double y){
			double nx = (Matrix[0] * x) + (Matrix[2] * y);
			double ny = (Matrix[1] * x) + (Matrix[3] * y);
			return SourceModule.GetColour(nx, ny);
		}
		
		public override void Prepare(DrawInfo info){
			
			double xAngle=XAngle.GetValue(0,0);
			double yAngle=YAngle.GetValue(0,0);
			double zAngle=ZAngle.GetValue(0,0);
			
			double xCos, yCos, zCos, xSin, ySin, zSin;
			xCos = System.Math.Cos(xAngle);
			yCos = System.Math.Cos(yAngle);
			zCos = System.Math.Cos(zAngle);
			xSin = System.Math.Sin(xAngle);
			ySin = System.Math.Sin(yAngle);
			zSin = System.Math.Sin(zAngle);
			
			Matrix=new double[4];
			
			Matrix[0] = ySin * xSin * zSin + yCos * zCos;
			Matrix[1] = xCos * zSin;
			Matrix[2] = ySin * xSin * zCos - yCos * zSin;
			Matrix[3] = xCos * zCos;
			
			// Prepare sources:
			base.Prepare(info);
			
		}

		public override double GetWrapped(double x, double y, int wrap){
			double nx = (Matrix[0] * x) + (Matrix[2] * y);
			double ny = (Matrix[1] * x) + (Matrix[3] * y);
			return SourceModule.GetWrapped(nx, ny,wrap);
		}
		
		public override double GetValue(double x, double y, double z){
			return GetValue(x,y);
		}
		
		public override double GetValue(double x, double y){
			double nx = (Matrix[0] * x) + (Matrix[2] * y);
			double ny = (Matrix[1] * x) + (Matrix[3] * y);
			return SourceModule.GetValue(nx, ny);
		}
		
		public override int TypeID{
			get{
				return 28;
			}
		}
		
	}
	
}
