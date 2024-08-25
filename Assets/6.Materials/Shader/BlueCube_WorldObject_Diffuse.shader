Shader "BlueCube/WorldObject_Diffuse" {
	Properties {
		_MainTex ("3D_Texture", 2D) = "white" {}
		_MainTex4 ("2D_Texture2", 2D) = "white" {}
		_MainTex2 ("Emission_Texture", 2D) = "white" {}
		_Emission ("Emission_Power", Float) = 1
		[Toggle] _IsUse2DTexture ("IsUse2DTexture", Float) = 0
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
	Fallback "Mobile/VertexLit"
}