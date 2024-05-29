Shader"Custom/SDFMorphingVolumeMesh"
{
    Properties
    {
        _SDF1 ("SDF Texture 1", 3D) = "" {}
        _SDF2 ("SDF Texture 2", 3D) = "" {}
        _Blend ("Blend Factor", Range(0, 1)) = 0.5
        _StepSize ("Raymarching Step Size", Range(0.001, 0.1)) = 0.01
        _DensityScale ("Density Scale", Range(0, 10)) = 1.0
        _MaxDensity ("Max Density", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"

struct appdata
{
    float4 vertex : POSITION;
    float3 normal : NORMAL;
    float2 uv : TEXCOORD0;
};

struct v2f
{
    float2 uv : TEXCOORD0;
    float4 vertex : SV_POSITION;
    float3 worldPos : TEXCOORD1;
    float3 viewDir : TEXCOORD2;
};

sampler3D _SDF1;
sampler3D _SDF2;
float _Blend;
float _StepSize;
float _DensityScale;
float _MaxDensity;

v2f vert(appdata v)
{
    v2f o;
    o.vertex = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
    o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
    o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos);
    return o;
}

float SampleSDF(float3 pos)
{
    float sdf1 = tex3D(_SDF1, pos).r;
    float sdf2 = tex3D(_SDF2, pos).r;
    return lerp(sdf1, sdf2, _Blend);
}

half4 frag(v2f i) : SV_Target
{
    float3 rayOrigin = i.worldPos;
    float3 rayDir = i.viewDir;

    float accumulatedDistance = 0.0;
    float totalDensity = 0.0;
    const float maxDistance = 10.0; // Adjust as necessary
    const int maxSteps = 256;
    int steps = 0;

    while (accumulatedDistance < maxDistance && steps < maxSteps)
    {
        float sdfValue = SampleSDF(rayOrigin + rayDir * accumulatedDistance);
        totalDensity += sdfValue;
        accumulatedDistance += _StepSize;
        steps++;
    }

    totalDensity *= _DensityScale / (float) steps;
    totalDensity = clamp(totalDensity, 0.0, _MaxDensity);

                // Debugging: Output density value as grayscale
    float gray = totalDensity / _MaxDensity;
    float4 color = float4(gray, gray, gray, 1.0);

                // Use a color ramp for better visualization
                // Uncomment the line below to use color ramp instead of grayscale
                // color = lerp(float4(0, 0, 1, 1), float4(1, 0, 0, 1), totalDensity / _MaxDensity);

    return color;
}
            ENDCG
        }
    }
FallBack"Diffuse"
}
