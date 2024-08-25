Shader "PIVO/StageSelect/Floor" {
	Properties {
		_Color0 ("Perfect Clear Color", Vector) = (1,1,1,1)
		_Color1 ("Clear Color", Vector) = (1,1,1,1)
		_Color2 ("Lock Color", Vector) = (1,1,1,1)
		_MainTex0 ("Default Texture", 2D) = "white" {}
		_MainTex1 ("Clear Texture", 2D) = "white" {}
		[Toggle] _IsPerfectClear ("IsPerfectClear", Float) = 0
		[Toggle] _IsClear ("IsClear", Float) = 0
		[Toggle] _IsUnlock ("IsUnlock", Float) = 0
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
	Fallback "Diffuse"
}