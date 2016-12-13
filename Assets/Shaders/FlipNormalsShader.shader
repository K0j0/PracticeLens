Shader "Custom/Flip Normals" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Main Color", Color) = (1,1,1,1)
	}
		SubShader{

		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Cull Front

		CGPROGRAM

#pragma surface surf Lambert vertex:vert alpha:fade
		sampler2D _MainTex;
		fixed4 _Color;

	struct Input {
		float2 uv_MainTex;
		float4 color : COLOR;
	};


	void vert(inout appdata_full v)
	{
		v.normal.xyz = v.normal * -1;
	}

	void surf(Input IN, inout SurfaceOutput o) {
		//fixed3 result = tex2D(_MainTex, IN.uv_MainTex);
		//o.Albedo = result.rgb;
		//o.Alpha = .1; // _Alpha;

		half4 result = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = result.rgb;
		o.Alpha = result.a;
	}

	ENDCG

	}

		Fallback "Diffuse"
}