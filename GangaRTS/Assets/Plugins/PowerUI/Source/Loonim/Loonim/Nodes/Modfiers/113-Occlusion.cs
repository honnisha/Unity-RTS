using System;
using UnityEngine;

namespace Loonim{
	
	public enum OccludeQuality{
		Low = 0,
		Medium = 1,
		High = 2,
	}
	
	public class Occlusion : Std2InputNode{
		
		#if !NO_BLADE_RUNTIME
		/// <summary>The random vectors image. Set on demand (Both CPU and GPU draw use this).</summary>
		private static Texture2D RandomVectors;
		#endif
		/// <summary>The dimensions of the random vector image.</summary>
		private const int RandomVectorImageSize=64;
		/// <summary>The pixels from the RandomVectors image (CPU mode only).</summary>
		internal static Color[] RandomVectorsColours;
		
		
		/// <summary>The occlusion quality.</summary>
		public OccludeQuality Quality{
			get{
				return (OccludeQuality)MaterialID_;
			}
			set{
				MaterialID_=(int)value;
			}
		}
		
		public float Radius;
		public float MinZ;
		public float AttenuationPower;
		public float SSAOPower;
		private int MaterialID_=1; // Medium quality.
		
		/// <summary>The special _Data vector.</summary>
		private Vector4 DataVector{
			get{
				return new Vector4(
					Radius,
					MinZ,
					AttenuationPower,
					SSAOPower
				);
			}
		}
		
		/// <summary>By default, materials are named Loonim/Texture_node_id, however some nodes have "sub-materials"
		/// where they essentially have a bunch of different shaders. An example is the Blend node.</summary>
		public override int SubMaterialID{
			get{
				return MaterialID_;
			}
		}
		
		public TextureNode Depth{ // Depth
			get{
				return Sources[0];
			}
			set{
				Sources[0]=value;
			}
		}
		
		public TextureNode Normals{
			get{
				return Sources[1];
			}
			set{
				Sources[1]=value;
			}
		}
		
		internal override int OutputDimensions{
			get{
				// 2D.
				return 2;
			}
		}
		
		public override void Prepare(DrawInfo info){
			
			#if NO_BLADE_RUNTIME
			
			if(RandomVectorsColours==null){
				throw new Exception("Occlusion node requires RandomVectorsColours to be set");
			}
			
			#else
			
			if(info.Mode==SurfaceDrawMode.CPU){
				
				// Ensure the pixels are available.
				if(RandomVectors==null){
					RandomVectors=Resources.Load("Loonim-Random-Vectors") as Texture2D;
				}
				
				if(RandomVectorsColours==null){
					// Pull the pixels:
					RandomVectorsColours=RandomVectors.GetPixels();
				}
				
			}
			
			#endif
			
		}
		
		#if !NO_BLADE_RUNTIME
		
		public override DrawStackNode Allocate(DrawInfo info,SurfaceTexture tex,ref int stackID){
			
			// RandomVectors required.
			if(RandomVectors==null){
				RandomVectors=Resources.Load("Loonim-Random-Vectors") as Texture2D;
			}
			
			// Stack required.
			
			// Get a material:
			UnityEngine.Material material=GetMaterial(TypeID,SubMaterialID);
				
			// _Data:
			material.SetVector(
				"_Data",
				DataVector
			);
			
			// Allocate a target stack now:
			int targetStack=stackID;
			DrawStack stack=tex.GetStack(targetStack,info);
			stackID++;
			
			// Allocate sources:
			AllocateSources(material,info,tex,targetStack,2);
			
			// Random vectors are always SRC2:
			material.SetTexture("_Src2",RandomVectors);
			
			// Create our node:
			MaterialStackNode matNode=DrawStore as MaterialStackNode;
			
			if(matNode==null){
				matNode=new MaterialStackNode();
				DrawStore=matNode;
				matNode.Mesh=info.Mesh;
			}
			
			matNode.Material=material;
			matNode.Stack=stack;
			
			return matNode;
			
		}
		
		#endif
		
		public override double GetWrapped(double x, double y, int wrap){
			return GetValue(x,y);
		}
		
		public override double GetValue(double x, double y, double z){
			return GetValue(x,y,0f);
		}
		
		public override double GetValue(double x, double y){
			
			// Get random samples:
			Vector3[] samples=RandomSamples[MaterialID_];
			
			// read random normal from noise texture:
			// From 0-1 range to 0-RandomVectorImageSize, clamp, then (y*xSize)+x
			int pixelIndex=(
				((int)(y * RandomVectorImageSize) % RandomVectorImageSize ) * RandomVectorImageSize + 
				((int)(x * RandomVectorImageSize) % RandomVectorImageSize)
			);
			
			// Get the colour:
			Color randNC=RandomVectorsColours[pixelIndex];
			
			Vector3 randN =new Vector3(randNC.r,randNC.g,randNC.b);
			
			// read scene depth/normal
			Color viewNormC = Normals.GetColour(x,y);
			Vector3 viewNorm=new Vector3(viewNormC.r,viewNormC.g,viewNormC.b);
			
			float depth = (float)Depth.GetValue(x,y);
			float scale = SSAOPower / depth;

			// accumulated occlusion factor
			float occ = 0f;
			int sampleCount=samples.Length;

			for(int s = 0; s < sampleCount; s++){
				
				// Reflect sample direction around a random vector
				Vector3 randomDir = Vector3.Reflect(samples[s], randN);
				
				// Make it point to the upper hemisphere
				float flip = (Vector3.Dot(viewNorm,randomDir)>=0f) ? 1f : -1f;
				
				randomDir *= flip;
				
				// Add a bit of normal to reduce self shadowing
				randomDir.x += viewNorm.x * 0.3f;
				randomDir.y += viewNorm.y * 0.3f;
				randomDir.z += viewNorm.z * 0.3f;
				
				float sD = depth - (randomDir.z * Radius);
				
				// Sample depth at offset location:
				float sampleD = (float)Depth.GetValue(x + (randomDir.x * scale),y + (randomDir.y * scale));
				
				float zd = (sD-sampleD);
				
				if(zd > MinZ){
					
					// Clamp zd such that it's no higher than 1.
					if(zd>1f){
						zd=1f;
					}
					
					// This sample occludes, contribute to occlusion
					occ += (float)System.Math.Pow(1f-zd,AttenuationPower); // + 1.0-saturate(pow(1.0 - zd, 11.0) + zd) + 1.0/(1.0+zd*zd*10); // iq
				}
				
			}

			return 1f - ( occ / (float)sampleCount);
		}
		
		public override int TypeID{
			get{
				return 113;
			}
		}
		
		/// <summary>The random samples for CPU mode; low, medium and high quality.</summary>
		internal readonly static Vector3[][] RandomSamples = new Vector3[][]{
			new Vector3[]{
				new Vector3(0.01305719f,0.5872321f,-0.119337f),
				new Vector3(0.3230782f,0.02207272f,-0.4188725f),
				new Vector3(-0.310725f,-0.191367f,0.05613686f),
				new Vector3(-0.4796457f,0.09398766f,-0.5802653f),
				new Vector3(0.1399992f,-0.3357702f,0.5596789f),
				new Vector3(-0.2484578f,0.2555322f,0.3489439f),
				new Vector3(0.1871898f,-0.702764f,-0.2317479f),
				new Vector3(0.8849149f,0.2842076f,0.368524f)
			},
			
			new Vector3[]{
				new Vector3(0.4010039f,0.8899381f,-0.01751772f),
				new Vector3(0.1617837f,0.1338552f,-0.3530486f),
				new Vector3(-0.2305296f,-0.1900085f,0.5025396f),
				new Vector3(-0.6256684f,0.1241661f,0.1163932f),
				new Vector3(0.3820786f,-0.3241398f,0.4112825f),
				new Vector3(-0.08829653f,0.1649759f,0.1395879f),
				new Vector3(0.1891677f,-0.1283755f,-0.09873557f),
				new Vector3(0.1986142f,0.1767239f,0.4380491f),
				new Vector3(-0.3294966f,0.02684341f,-0.4021836f),
				new Vector3(-0.01956503f,-0.3108062f,-0.410663f),
				new Vector3(-0.3215499f,0.6832048f,-0.3433446f),
				new Vector3(0.7026125f,0.1648249f,0.02250625f),
				new Vector3(0.03704464f,-0.939131f,0.1358765f),
				new Vector3(-0.6984446f,-0.6003422f,-0.04016943f)
			},
			
			new Vector3[]{
				new Vector3(0.2196607f,0.9032637f,0.2254677f),
				new Vector3(0.05916681f,0.2201506f,-0.1430302f),
				new Vector3(-0.4152246f,0.1320857f,0.7036734f),
				new Vector3(-0.3790807f,0.1454145f,0.100605f),
				new Vector3(0.3149606f,-0.1294581f,0.7044517f),
				new Vector3(-0.1108412f,0.2162839f,0.1336278f),
				new Vector3(0.658012f,-0.4395972f,-0.2919373f),
				new Vector3(0.5377914f,0.3112189f,0.426864f),
				new Vector3(-0.2752537f,0.07625949f,-0.1273409f),
				new Vector3(-0.1915639f,-0.4973421f,-0.3129629f),
				new Vector3(-0.2634767f,0.5277923f,-0.1107446f),
				new Vector3(0.8242752f,0.02434147f,0.06049098f),
				new Vector3(0.06262707f,-0.2128643f,-0.03671562f),
				new Vector3(-0.1795662f,-0.3543862f,0.07924347f),
				new Vector3(0.06039629f,0.24629f,0.4501176f),
				new Vector3(-0.7786345f,-0.3814852f,-0.2391262f),
				new Vector3(0.2792919f,0.2487278f,-0.05185341f),
				new Vector3(0.1841383f,0.1696993f,-0.8936281f),
				new Vector3(-0.3479781f,0.4725766f,-0.719685f),
				new Vector3(-0.1365018f,-0.2513416f,0.470937f),
				new Vector3(0.1280388f,-0.563242f,0.3419276f),
				new Vector3(-0.4800232f,-0.1899473f,0.2398808f),
				new Vector3(0.6389147f,0.1191014f,-0.5271206f),
				new Vector3(0.1932822f,-0.3692099f,-0.6060588f),
				new Vector3(-0.3465451f,-0.1654651f,-0.6746758f),
				new Vector3(0.2448421f,-0.1610962f,0.1289366f)
			}
			
		};
		
	}
	
}
