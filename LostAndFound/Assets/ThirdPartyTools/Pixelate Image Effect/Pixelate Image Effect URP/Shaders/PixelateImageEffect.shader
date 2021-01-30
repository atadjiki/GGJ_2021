Shader "Hidden/PixelateImageEffect"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float4 scrPos : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_TexelSize;

			uniform half _TileSize;

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.scrPos = ComputeScreenPos(o.pos);
				return o;
			}

			float2 fmod(float2 a, float2 b)
			{
				float2 c = frac(abs(a / b)) * abs(b);
				return abs(c);
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 fragCoord = (i.scrPos.xy/i.scrPos.w) * _ScreenParams.xy;
				float4 c = 0;
				
				float2 uv = fragCoord.xy / _ScreenParams.xy;
				float2 size = 1.0 / float2(_ScreenParams.x, _ScreenParams.y) * _TileSize;

				float2 Pbase = uv - fmod(uv, size);
				float2 PCenter = Pbase + size / 2.0;
				float2 st = (uv - Pbase) / size;

				float4 tileColor = tex2D(_MainTex, PCenter);
				c = tileColor;

				return c;
			}

			ENDCG
		}
	}
}
