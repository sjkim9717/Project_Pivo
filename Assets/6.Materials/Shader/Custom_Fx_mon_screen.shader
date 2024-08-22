Shader "Custom/Fx_mon_screen" {
	Properties {
		_MainTex ("noise", 2D) = "white" {}
		_MainTex2 ("distortion", 2D) = "white" {}
		_colorTest ("color", Vector) = (1,1,1,1)
		_scale ("Main range", Float) = 0
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