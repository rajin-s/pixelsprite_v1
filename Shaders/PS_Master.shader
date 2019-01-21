Shader "PixelSprite/Master"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		
		[PerRendererData]
		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline Width", Float) = 1
		
		[PerRendererData]
		_PaletteTexTex("Palette", 2D) = "white" {}
		
		_GridSize("Grid Size", Float) = 8

		_AlphaThreshold("Alpha Threshold", Float) = .99
	}
	SubShader
	{
		Tags
		{
			"RenderType"="Transparent"
			"DisableBatching"="True"
			"PreviewType" = "Plane"
		}

		Cull Off
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma shader_feature OUTLINE
			#pragma shader_feature PALETTE
			#pragma shader_feature GRIDSNAP
			#pragma shader_feature CUTOUT
			#pragma shader_feature PIXELSNAP
			
			#pragma vertex vert
			#pragma fragment frag

			
			#include "PS_Common.cginc"

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

			#if OUTLINE
			fixed4 _OutlineColor;
			float _OutlineWidth;
			float4 _MainTex_TexelSize;
			#endif

			#if PALETTE
			sampler2D _PaletteTex;
			float4 _PaletteTex_TexelSize;
			#endif

			#if GRIDSNAP
			float _GridSize;
			#endif

			#if CUTOUT
			float _AlphaThreshold;
			#endif

			v2f vert (appdata IN)
			{
				v2f OUT;

				#if GRIDSNAP
				OUT.vertex = WorldClampedClipSpace(IN.vertex, _GridSize);
				#else
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				#endif

				#if PIXELSNAP
				OUT.vertex = UnityPixelSnap(OUT.vertex);
				/*OUT.vertex.xy *= _ScreenParams.xy / 2;
				OUT.vertex.xy = floor(OUT.vertex.xy);
				OUT.vertex.xy /= _ScreenParams.xy / 2;*/
				#endif

				OUT.uv = IN.uv;
				OUT.color = IN.color;
				return OUT;
			}
			
			fixed4 frag (v2f IN) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, IN.uv);
				col *= IN.color;

				#if OUTLINE
				col += Outline(IN.uv, _MainTex, _MainTex_TexelSize.xy * _OutlineWidth, _OutlineWidth) * _OutlineColor;
				#endif

				#if CUTOUT
				clip(col.a - _AlphaThreshold);
				#endif

				#if PALETTE
				col.rgb = GetLUTColor(col, _PaletteTex, _PaletteTex_TexelSize.z / _PaletteTex_TexelSize.w);
				#endif

				return col;
			}
			ENDCG
		}
	}
	CustomEditor "PSMasterEditor"
}