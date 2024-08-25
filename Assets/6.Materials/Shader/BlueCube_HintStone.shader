Shader "BlueCube/HintStone" {
	Properties {
		_MainTex ("파란색", 2D) = "white" {}
		_MainTex2 ("굴절", 2D) = "white" {}
		_MainTex3 ("하얀띠", 2D) = "white" {}
		_Monotone ("흑백/컬러 조절", Range(0, 1)) = 0
		_FlowSpeed ("흐르는 속도", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	Fallback "Diffuse"
}