Shader "Sprites/BladeHorizontal"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)

		_Size ("Blade Size", Float) = 1
		_ShineLocation("Shine Location", Range(-0.15,1.1)) = 0

		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			fixed4 _Color;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			sampler2D _MainTex;
			float _Size;
			float _ShineLocation;

			static const fixed FIRST_ZONE_END = 0.012;
			static const fixed SECOND_ZONE_END = 0.0355;
			static const fixed ZONE_DIFFERENCE = 0.0235;

			fixed4 frag(v2f IN) : SV_Target
			{
				float dist = distance(_ShineLocation, IN.texcoord.x);
				float2 sizeModifier = float2(1, _Size);

				fixed4 c = tex2D(_MainTex, IN.texcoord * sizeModifier) * IN.color;

				float2 minmax;
				float alphafactor;

				if (IN.texcoord.x < FIRST_ZONE_END) {			
					minmax = float2(0, 1);
					alphafactor = 0.5;
				}
				else if (IN.texcoord.x < SECOND_ZONE_END) {
					float p = (IN.texcoord.x - FIRST_ZONE_END) / (ZONE_DIFFERENCE);
					float l = lerp(0, 0.42, p);
					minmax = float2(l, 1-l);

					alphafactor = lerp(0.5, 1, p);
				}
				else {
					minmax = float2(0.42, 0.58);
					alphafactor = 1;
				}

				if (IN.texcoord.y > minmax.x && IN.texcoord.y < minmax.y) {
					float d = (1 - (dist*5))*alphafactor;
					d = max(0, d);
					c = (d)*float4(1, 1, 1, 1) + (1-d)*c;
				}

				return c;
			}
		ENDCG
		}
	}
}
