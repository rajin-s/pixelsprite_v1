#include "UnityCG.cginc"
#define DIAG 1 //0.7071

fixed4 GetLUTColor(fixed4 IN, sampler2D LUT, float levels)
{
	float2 uv = float2(floor(min(IN.b, 0.99) * levels) / levels + (min(IN.r, 0.99) / levels), IN.g);
	return tex2D(LUT, uv);
}

float4 WorldClampedClipSpace(float4 IN, float gridDivision)
{
	float4 v = mul(unity_ObjectToWorld, IN);
	v.xy = round(v * gridDivision) / gridDivision;
	return mul(UNITY_MATRIX_VP, v);
}

float Outline(float2 IN, sampler2D tex, float2 size, float pixelSize)
{
	fixed4 col = tex2D(tex, IN);

	float T = tex2D(tex, IN + float2(0, size.y)).a;
	float R = tex2D(tex, IN + float2(size.x, 0)).a;
	float B = tex2D(tex, IN - float2(0, size.y)).a;
	float L = tex2D(tex, IN - float2(size.x, 0)).a;

	float TR = min(pixelSize - 1, 1) * tex2D(tex, IN + float2(size.x, size.y) * DIAG).a;
	float BR = min(pixelSize - 1, 1) * tex2D(tex, IN + float2(size.x, -size.y) * DIAG).a;
	float BL = min(pixelSize - 1, 1) * tex2D(tex, IN + float2(-size.x, -size.y) * DIAG).a;
	float TL = min(pixelSize - 1, 1) * tex2D(tex, IN + float2(-size.x, size.y) * DIAG).a;

	return floor(1 - col.a) * ceil(min(1, TL + BL + BR + TR + L + B + R + T));
}