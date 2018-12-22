// Upgrade NOTE: replaced 'SeperateSpecular' with 'SeparateSpecular'

Shader "FX/VertexLit ShowThrough" {

	Properties{
		_Color("Main Color", Color) = (1,1,1,1)
		_MainTex("Base (RGB)", 2D) = "white" {}
		_OccludeColor("Occlusion Color", Color) = (0,0,1,1)
	}
	SubShader
	{
		Tags {"Queue" = "Geometry+5"}
		// occluded pass
		Pass {
			ZWrite Off
			Blend One Zero
			ZTest Greater
			Color[_OccludeColor]
		}
		// Vertex lights
		Pass {
			Tags {"LightMode" = "Vertex"}
			ZWrite On
			Lighting On
			SeparateSpecular On
			Material {
				Diffuse[_Color]
				Ambient[_Color]
		// Emission [_PPLAmbient]
		}
		SetTexture[_MainTex] {
			ConstantColor[_Color]
			Combine texture * primary DOUBLE, texture * constant
			}
		}
	}
	FallBack "Diffuse", 1
}