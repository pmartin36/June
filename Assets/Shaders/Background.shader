Shader "Unlit/Background"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DisplacementTex("Displacement Texture", 2D) = "white" {}
		_DisplacementMagnitude("Displacement Magnitude", float) = 0
		_Color("Color", Color) = (1,1,1,1)
		_BlurSize("Blur Size", Vector) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		ZTest Always
		ZWrite On

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			sampler2D _DisplacementTex;
			float _DisplacementMagnitude;

			float4 _BlurSize;
			float4 _MainTex_TexelSize;

			float4 _Color;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed2 disp = tex2D(_DisplacementTex, i.uv).gr * _DisplacementMagnitude;
				float2 displacement = float2(0.3, 1) * _SinTime.x * disp; 
				fixed2 newuv = i.uv + displacement;

				fixed4 s = tex2D(_MainTex, newuv);

				return s * _Color;
			}
			ENDCG
		}
	}
}
