Shader "BlueCube/Door/DoorPattern" {
	Properties {
		_MainTex2 ("Albedo (RGB)", 2D) = "white" {}
		_PatternFill ("PatternFill", Range(0.91, 6.8)) = 1
		_Color ("Color", Vector) = (0.046,0.21,0.433,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = _Color.rgb;
			o.Alpha = _Color.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}