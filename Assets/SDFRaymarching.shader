Shader"Custom/SDFRayMarching"
{
    Properties
    {
        _MainTex ("3D SDF Texture", 3D) = "" {}
        _MaxSteps ("Max Ray Marching Steps", Int) = 100
        _StepSize ("Ray Step Size", Float) = 0.01
        _Threshold ("Surface Threshold", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD     100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

sampler3D _MainTex;
int _MaxSteps;
float _StepSize;
float _Threshold;

struct appdata
{
    float4 vertex : POSITION;
    float3 uv : TEXCOORD0;
};

struct v2f
{
    float4 vertex : SV_POSITION;
    float3 uvw : TEXCOORD0;
};

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uvw = v.uv;
    return o;
}

float4 frag(v2f i) : SV_Target
{
    float3 rayOrigin = i.uvw;
    float3 rayDirection = normalize(rayOrigin - float3(0.5, 0.5, 0.5));
    float3 pos = rayOrigin;
                
                [unroll(100)]
    for (int step = 0; step < _MaxSteps; step++)
    {
        float sdfValue = tex3D(_MainTex, pos).r;
        if (sdfValue < _Threshold)
        {
            return float4(1.0, 1.0, 1.0, 1.0); // Hit surface, return white color
        }
        pos += rayDirection * _StepSize;
    }
                
    return float4(0.0, 0.0, 0.0, 0.0); // No hit, return black color
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
