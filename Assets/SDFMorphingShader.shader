Shader"Custom/SDFMorphing"
{
    Properties
    {
        _SDF1 ("SDF Texture 1", 3D) = "" {}
        _SDF2 ("SDF Texture 2", 3D) = "" {}
        _Blend ("Blend Factor", Range(0, 1)) = 0.5
    }
    SubShader
    {
        LOD 200
        Tags { "RenderType"="Opaque" } 


        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

sampler3D _SDF1;
sampler3D _SDF2;
float _Blend;

struct Input
{
    float3 worldPos;
    float4 screenPos;
};

half4 LightingStandard(SurfaceOutputStandard s, half3 lightDir, half atten)
{
    half3 normal = normalize(s.Normal);
    half diff = max(0, dot(normal, lightDir));
    half4 c;
    c.rgb = s.Albedo * _LightColor0.rgb * diff * atten;
    c.a = s.Alpha;
    return c;
}

void surf(Input IN, inout SurfaceOutputStandard o)
{
    float3 position = IN.worldPos;

    float sdf1 = tex3D(_SDF1, position).r;
    float sdf2 = tex3D(_SDF2, position).r;

    float blendedSDF = lerp(sdf1, sdf2, _Blend);
    float d = min(sdf1, sdf2); // smooth min blending

            // Calculate normal from SDF gradient
    float3 dx = float3(0.01, 0, 0);
    float3 dy = float3(0, 0.01, 0);
    float3 dz = float3(0, 0, 0.01);
    float3 normal;
    if (sdf1 < 0)
        sdf1 = sdf1 * -1;
    if (sdf2 < 0)
        sdf2 = sdf2 * -1;
    
    if ((sdf1 * _Blend) < (sdf2 * (1 - _Blend)))
    {
        normal = normalize(float3(
            tex3D(_SDF1, position + dx).r - tex3D(_SDF1, position - dx).r,
            tex3D(_SDF1, position + dy).r - tex3D(_SDF1, position - dy).r,
            tex3D(_SDF1, position + dz).r - tex3D(_SDF1, position - dz).r
        ));
    }
    else
    {
        normal = normalize(float3(
            tex3D(_SDF2, position + dx).r - tex3D(_SDF2, position - dx).r,
            tex3D(_SDF2, position + dy).r - tex3D(_SDF2, position - dy).r,
            tex3D(_SDF2, position + dz).r - tex3D(_SDF2, position - dz).r
        ));
    }



    o.Albedo = float4(normal, 1.0);
    o.Normal = normal;
    o.Alpha = 1.0 - smoothstep(0.0, 0.01, normal); // binary alpha based on SDF value
}
        ENDCG
    }
FallBack"Diffuse"
}
