Shader "Unlit/CurlFromPerlin"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define vec2 float2
			#define vec3 float3
			#define vec4 float4
			#define fract frac
			#define mix lerp
			#define EPSILON 1e-3
			#include "UnityCG.cginc"
			#include "../Noise/ClassicNoise3D.hlsl"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			vec2 hash( vec2 x )  // replace this by something better
			{
			    const vec2 k = vec2( 0.3183099, 0.3678794 );
			    x = x*k + k.yx;
			    return -1.0 + 2.0*fract( 16.0 * k*fract( x.x*x.y*(x.x+x.y)));
			}

			float noise( in vec2 p )
			{
			    vec2 i = floor( p );
			    vec2 f = fract( p );
				
				vec2 u = f * f * f * (6 * f * f - 15 * f + 10);

			    return mix( mix( dot( hash( i + vec2(0.0,0.0) ), f - vec2(0.0,0.0) ), 
			                     dot( hash( i + vec2(1.0,0.0) ), f - vec2(1.0,0.0) ), u.x),
			                mix( dot( hash( i + vec2(0.0,1.0) ), f - vec2(0.0,1.0) ), 
			                     dot( hash( i + vec2(1.0,1.0) ), f - vec2(1.0,1.0) ), u.x), u.y);
			}
						

			float2 curlNoise(float3 p)
			{
				float3 dx = float3(EPSILON, 0.0, 0.0);
				float3 dy = float3(0.0, EPSILON, 0.0);

				float2 dpdx0 = cnoise(p - dx);
				float2 dpdx1 = cnoise(p + dx);
				float2 dpdy0 = cnoise(p - dy);
				float2 dpdy1 = cnoise(p + dy);

				float x = dpdy1.y - dpdy0.y;
				float y = -dpdx1.x + dpdy0.x;
				return float2(x, y) / EPSILON * 2.0;
			}

			fixed4 frag (v2f i) : SV_Target
			{				
				float2 c = curlNoise(float3(i.uv, _Time.y));
				return float4(c.x, c.y, 0, 1);
			}
			ENDCG
		}
	}
}
