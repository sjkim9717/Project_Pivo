Shader "BlueCube/SkyBox" {
	Properties {
		_Tint ("Tint Color", Vector) = (0.5,0.5,0.5,0.5)
		[Gamma] _Exposure ("Exposure", Range(0, 8)) = 1
		_Rotation ("Rotation", Range(0, 360)) = 0
		_Tex ("Spherical  (HDR)", 2D) = "grey" {}
		_Tex2 ("Spherical  (HDR)", 2D) = "grey" {}
		_Tex3 ("Spherical  (HDR)", 2D) = "grey" {}
		_Tex4 ("Spherical  (HDR)", 2D) = "grey" {}
		[KeywordEnum(6 Frames Layout, Latitude Longitude Layout)] _Mapping ("Mapping", Float) = 1
		[Enum(360 Degrees, 0, 180 Degrees, 1)] _ImageType ("Image Type", Float) = 0
		[Toggle] _MirrorOnBack ("Mirror on Back", Float) = 0
		[Enum(None, 0, Side by Side, 1, Over Under, 2)] _Layout ("3D Layout", Float) = 0
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
	//CustomEditor "SkyboxPanoramicBetaShaderGUI"
}