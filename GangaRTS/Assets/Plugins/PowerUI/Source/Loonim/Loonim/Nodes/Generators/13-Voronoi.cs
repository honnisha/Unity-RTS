using System;
using UnityEngine;

namespace Loonim{
	
	/// <summary>The available voronoi functions. Note: (n%4)+1 describes how many nodes a method requires.</summary>
	public enum VoronoiMethod:int{ // 4 bits
		
		F1=0, // 1 node.
		F2=1, // 2 nodes.
		F3=2, // 3 nodes.
		F4=3, // 4 nodes.
		F2minusF1=4 + VoronoiMethod.F2, // Minus uses bit 4. So, minus and 2 nodes.
		F3minusF2=4 + VoronoiMethod.F3, // Minus uses bit 4. So, minus and 3 nodes.
		F4minusF3=4 + VoronoiMethod.F4, // Minus uses bit 4. So, minus and 4 nodes.
		Average2=8 + VoronoiMethod.F2, // Average(f1,f2)
		Average3=8 + VoronoiMethod.F3, // Average(f1,f2,f3)
		Average4=8 + VoronoiMethod.F4, // Average(f1,f2,f3,f4)
		TwoF3minusF2minusF1=12 + VoronoiMethod.F3
		
	}
	
	public enum VoronoiDraw:int{ // 2 bits
		Normal=0,
		Flat=1,
		Solid=2
	}
	
	public enum VoronoiDistance:int{ // 2 bits
		Euclidean=0,
		Manhattan=1,
		Chebyshev=2,
		Minkowski=3
	}
	
	public class Voronoi: TextureNode{
		
		public int Seed;
		private int MaterialID_;
		
		public TextureNode Frequency{
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		/// <summary>Minkowski number.</summary>
		public TextureNode MinkowskiNumber{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		/// <summary>The draw mode (Maps to VoronoiDraw)</summary>
		public TextureNode DrawMode{
			get{
				return Sources[2];
			}
			set{
				Sources[2]=value;
			}
		}
		
		/// <summary>The Voronoi noise distance function (Maps to VoronoiDistance).</summary>
		public TextureNode Distance{
			get{
				return Sources[3];
			}
			set{
				Sources[3]=value;
			}
		}
		
		/// <summary>The Voronoi function to use (Maps to VoronoiMethod).</summary>
		private TextureNode Method{
			get{
				return Sources[4];
			}
			set{
				Sources[4]=value;
			}
		}
		
		/// <summary>The jitter amount (affects how distorted it is).</summary>
		public TextureNode Jitter{
			get{
				return Sources[5];
			}
			set{
				Sources[5]=value;
			}
		}
		
		/// <summary>A shared buffer used for computing the n required nodes.</summary>
		private double[] DistanceBuffer;
		
		
		public Voronoi():base(6)
		{
			// Frequency = 1.0;
			Seed = 0;
		}
		
		/// <summary>The function to use. E.g. F1, F2-F1.</summary>
		public TextureNode Function{
			get{
				return Method;
			}
			set{
				
				// Update method:
				Method=value;
				
				// Clear buffer:
				DistanceBuffer=null;
				
			}
		}
		
		internal override int OutputDimensions{
			get{
				// 2D.
				return 2;
			}
		}
		
		/// <summary>The special _Data vector used by the noise group of materials.</summary>
		private Vector4 DataVector{
			get{
				return new Vector4(
					(float)Seed,
					(float)Frequency.GetValue(0,0),
					1f,
					(float)Jitter.GetValue(0,0)
				);
			}
		}
		
		#if !NO_BLADE_RUNTIME
		public override DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			// Stack required.
			
			// Allocate a target stack now:
			int targetStack=stackID;
			DrawStack stack=tex.GetStack(targetStack,info);
			stackID++;
			
			int subID=SubMaterialID;
			
			// Create the material:
			Material material=GetMaterial(TypeID,subID);
			
			// _Data (Seed, Frequency, Amplitude, Jitter):
			material.SetVector(
				"_Data",
				DataVector
			);
			
			if(subID==3){
				
				// Minkowski number required.
				
				// Get the input node:
				TextureNode input=MinkowskiNumber;
				
				// Allocate it now (must not allocate targetStack in the direct kids):
				int inputStacks=(targetStack==0)?1:0;
				DrawStackNode drawNode=input.Allocate(info,tex,ref inputStacks);
				
				// Apply it to our material:
				material.SetTexture("_Src0",drawNode.Texture);
				
			}
			
			// Create our node:
			MaterialStackNode matNode=new MaterialStackNode();
			DrawStore=matNode;
			matNode.Mesh=info.Mesh;
			matNode.Material=material;
			matNode.Stack=stack;
			
			return matNode;
			
		}
		
		#endif
		
		public override void Prepare(DrawInfo info){
			
			if(DistanceBuffer==null){
				
				// Get the number of nodes required:
				int nodesRequired=((int)Method.GetValue(0,0) % 4)+1;
				
				// Update buffer. Each entry holds the nearest distance and the selected points "min" value:
				DistanceBuffer=new double[nodesRequired * 2];
				
			}
			
			// Prepare sources:
			base.Prepare(info);
			
		}
		
		/// <summary>By default, materials are named Loonim/Texture_node_id, however some nodes have "sub-materials"
		/// where they essentially have a bunch of different shaders. An example is the Blend node.</summary>
		public override int SubMaterialID{
			get{
				return MaterialID_;
			}
		}
		
		public override void Draw(DrawInfo info){
			
			// Always pull the latest mode, checking if it's changed:
			Property pv=Distance as Property;
			
			if(pv!=null){
				
				// Update now if we can:
				pv.UpdateIfChanged(info);
			
			}
			
			int materialMode=(int)Distance.GetValue(0,0);
			
			if(materialMode!=MaterialID_){
				
				// Change!
				MaterialID_=materialMode;
				
				// Allocate shader now:
				SubMaterialChanged();
				
			}
			
			// Check for changes on jitter and frequency:
			bool updateData=false;
			pv=Jitter as Property;
			
			if(pv!=null){
				
				// Update now if we can:
				if(pv.UpdateIfChanged(info)){
					
					updateData=true;
					
				}
				
			}
			
			pv=Frequency as Property;
			
			if(pv!=null){
				
				// Update now if we can:
				if(pv.UpdateIfChanged(info)){
					
					updateData=true;
					
				}
				
			}
			
			if(updateData){
				
				MaterialStackNode msn=DrawStore as MaterialStackNode;
				
				if(msn!=null){
					
					msn.Material.SetVector("_Data",DataVector);
					
				}
				
			}
			
			// Draw now:
			base.Draw(info);
			
		}
		
		public override double GetWrapped(double x, double y,int wrap){
			return GetValue(x,y);
		}
		
		public override double GetValue(double x, double y,double z){
			return GetValue(x,y);
		}
		
		public override double GetValue(double x, double y){
			
			double frequency=Frequency.GetValue(x,y);
			x *= frequency;
			y *= frequency;

			int xInt = (x > 0.0 ? (int)x : (int)x - 1);
			int yInt = (y > 0.0 ? (int)y : (int)y - 1);
			
			// Get mode/method:
			VoronoiMethod method=(VoronoiMethod)((int)Method.GetValue(x,y));
			VoronoiDistance distance=(VoronoiDistance)((int)Distance.GetValue(x,y));
			VoronoiDraw drawMode=(VoronoiDraw)((int)DrawMode.GetValue(x,y));
			
			// Get the buffer:
			double[] buffer=DistanceBuffer;
			int bufferLength=buffer.Length;
			
			// Reset mins:
			for(int i=0;i<bufferLength;i+=2){
				
				// Up to max: 
				buffer[i]=double.MaxValue;
				
			}
			
			// Inside each unit cube, there is a seed point at a random position.  Go
			// through each of the nearby cubes until we find a cube with a seed point
			// that is closest to the specified position.
			for (int yCur = yInt - 2; yCur <= yInt + 2; yCur++)
			{
				for (int xCur = xInt - 2; xCur <= xInt + 2; xCur++)
				{

					// Calculate the position and distance to the seed point inside of
					// this unit cube.
					double xPos = xCur + ValueNoiseBasis.ValueNoise(xCur, yCur, Seed);
					double yPos = yCur + ValueNoiseBasis.ValueNoise(xCur, yCur, Seed + 1);
					double xDist = xPos - x;
					double yDist = yPos - y;
					double dist;
					
					// Sample and return at the successful point:
					int x0 = (xPos > 0.0 ? (int)xPos : (int)xPos - 1);
					int y0 = (yPos > 0.0 ? (int)yPos : (int)yPos - 1);
					
					// Min is..
					double min=((double)ValueNoiseBasis.ValueNoise(x0, y0)+1.0) * 0.5;
					
					switch(distance){
						
						default:
						case VoronoiDistance.Euclidean:
						
							dist=xDist * xDist + yDist * yDist;
							
							if(drawMode==VoronoiDraw.Normal){
								
								dist+=1.0-min;
								
							}
							
						break;
						
						case VoronoiDistance.Manhattan:
							
							if(yDist<0){
								yDist=-yDist;
							}
							
							if(xDist<0){
								xDist=-xDist;
							}
							
							dist=xDist + yDist;
							
							if(drawMode==VoronoiDraw.Normal){
								
								dist+=1.0-min;
								
							}
							
							dist/=2.0;
							
						break;
						
						case VoronoiDistance.Chebyshev:
							
							if(yDist<0){
								yDist=-yDist;
							}
							
							if(xDist<0){
								xDist=-xDist;
							}
							
							if(yDist>xDist){
								dist=yDist;
							}else{
								dist=xDist;
							}
							
							if(drawMode==VoronoiDraw.Normal){
								
								if(dist<min){
									dist=min;
								}
								
							}
							
						break;
						
						case VoronoiDistance.Minkowski:
							
							if(yDist<0){
								yDist=-yDist;
							}
							
							if(xDist<0){
								xDist=-xDist;
							}
							
							double minkowskiNumber=MinkowskiNumber.GetValue(x,y);
							
							dist=System.Math.Pow( ( System.Math.Pow(xDist,minkowskiNumber) + System.Math.Pow(yDist,minkowskiNumber) ) , 1.0 / minkowskiNumber );
							
							if(drawMode==VoronoiDraw.Normal){
								
								if(dist<min){
									dist=min;
								}
								
							}
							
						break;
						
					}
					
					
					
					// Note: The nearest is at [0].
					for(int i=0;i<bufferLength;i+=2){
						
						// Up to max:
						if (dist < buffer[i]){
							
							// This seed point is closer than the one at buffer[i/2].
							// Push i onwards over by 2 places as the entry here
							// and all after it are now further away.
							int offset=i+2;
							
							if(bufferLength!=offset){
								Array.Copy(buffer,i,buffer,offset,bufferLength-offset);
							}
							
							// Write this:
							buffer[i] = dist;
							buffer[i+1] = min;
							
							// Stop there.
							break;
							
						}
						
					}
					
				}
			}
			
			// Buffer now contains n nodes. The nearest one is at the start of the buffer.
			
			
			double value;
			
			// Special case for euclidean - we used basic lengths for speed:
			
			if(distance==VoronoiDistance.Euclidean){
				
				// Must compute the full distance. So, for each one..
				for(int i=0;i<bufferLength;i+=2){
					
					// Get complete value:
					buffer[i] = (System.Math.Sqrt(buffer[i])) * Math.Sqrt3 - 1.0;
					
				}
				
			}
			
			// Next, compute the point and offset values.
			
			if(drawMode==VoronoiDraw.Solid){
			
				switch(method){
					
					default:
					case VoronoiMethod.F1:
						
						// Best distance was..
						
						value=buffer[1];
						
					break;
					
					case VoronoiMethod.F2:
					
						// 2nd best distance was..
						
						value=buffer[3];
						
					break;
					
					case VoronoiMethod.F3:
					
						// 3rd best distance was..
						
						value=buffer[5];
						
					break;
					
					case VoronoiMethod.F4:
					
						// 4th best distance was..
						
						value=buffer[7];
						
					break;
					
					case VoronoiMethod.F2minusF1:
					case VoronoiMethod.F3minusF2:
					case VoronoiMethod.F4minusF3:
						
						// fN - fNm1 (but inverted):
						value=buffer[bufferLength-3] - buffer[bufferLength-1] + 1.0;
						
					break;
					case VoronoiMethod.Average2:
					case VoronoiMethod.Average3:
					case VoronoiMethod.Average4:
						
						// Sum all of them together:
						value=0.0;
						
						for(int i=1;i<bufferLength;i+=2){
							
							value+=buffer[i];
							
						}
						
						// Average is then..
						value/=(double)bufferLength;
						
					break;
					
					case VoronoiMethod.TwoF3minusF2minusF1:
						
						// 2 * f3 - f2 - f1:
						value=1.0 -( (2.0 * buffer[5]) - buffer[3] - buffer[1] );
						
					break;
			
				}
				
			}else{
				
				switch(method){
					
					default:
					case VoronoiMethod.F1:
						
						// Best distance was..
						
						value=buffer[0];
						
					break;
					
					case VoronoiMethod.F2:
					
						// 2nd best distance was..
						
						value=buffer[2];
						
					break;
					
					case VoronoiMethod.F3:
					
						// 3rd best distance was..
						
						value=buffer[4];
						
					break;
					
					case VoronoiMethod.F4:
					
						// 4th best distance was..
						
						value=buffer[6];
						
					break;
					
					case VoronoiMethod.F2minusF1:
					case VoronoiMethod.F3minusF2:
					case VoronoiMethod.F4minusF3:
						
						// fN - fNm1 (but inverted):
						value=buffer[bufferLength-4] - buffer[bufferLength-2] + 1.0;
						
					break;
					case VoronoiMethod.Average2:
					case VoronoiMethod.Average3:
					case VoronoiMethod.Average4:
						
						// Sum all of them together:
						value=0.0;
						
						for(int i=0;i<bufferLength;i+=2){
							
							value+=buffer[i];
							
						}
						
						// Average is then..
						value/=(double)bufferLength;
						
					break;
					
					case VoronoiMethod.TwoF3minusF2minusF1:
						
						// 2 * f3 - f2 - f1:
						value=1.0 -( (2.0 * buffer[4]) - buffer[2] - buffer[0] );
						
					break;
			
				}
				
			}
			
			// Return the calculated distance.
			
			if(distance==VoronoiDistance.Euclidean){
				
				return (1.0-value)/2.0;
				
			}
			
			return 1.0-value;
			
		}
		
		public override int TypeID{
			get{
				return 13;
			}
		}
		
	}
	
}
