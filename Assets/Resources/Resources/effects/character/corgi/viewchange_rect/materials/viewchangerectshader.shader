Shader "PIVO/Effects/ViewChangeRect" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,1)
		_MainTex ("Tex1", 2D) = "white" {}
		_MainTex2 ("Tex2", 2D) = "white" {}
		_DT ("distortion", 2D) = "white" {}
		_Alpha ("Alpha", Range(0, 1)) = 1
		_EffectRange ("Effect_Range", Range(0, 10)) = 0
		_EffectRange2 ("Effect_Range2", Range(0, 10)) = 0
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		fixed4 _Color;
		struct Input
		{
			float2 uv_MainTex;
		};
		
		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Legacy Shader/Diffuse/Transparent/Vertexlit"
}