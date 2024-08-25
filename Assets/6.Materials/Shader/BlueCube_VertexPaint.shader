Shader "BlueCube/VertexPaint" {
	Properties {
		_MainTex3D ("3D_Texture", 2D) = "white" {}
		_MainTex2D ("2D_Texture", 2D) = "white" {}
		_ShadowColor ("DeepShadowColor", Vector) = (1,1,1,1)
		_ShadowColor2 ("DeepShadowColor2", Vector) = (1,1,1,1)
		[Toggle] _IsUse2DTexture ("IsUse2DTexture", Float) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
		}
		ENDCG
	}
	Fallback "Mobile/VertexLit"
}