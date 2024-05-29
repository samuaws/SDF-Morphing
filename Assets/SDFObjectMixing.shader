Shader"Custom/SDFObjectMixing"
{
    Properties
    {
        _MainTex1("SDF Texture 1", 3D) = "white" {}
        _MainTex2("SDF Texture 2", 3D) = "white" {}
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
#include "UnityCG.cginc"
            
struct appdata
{
    float4 vertex : POSITION;
};
            
struct v2f
{
    float4 pos : SV_POSITION;
};
            
sampler3D _MainTex1;
sampler3D _MainTex2;
            
float4 _MainTex1_ST;
float4 _MainTex2_ST;
            
v2f vert(appdata v)
{
    v2f o;
    o.pos = UnityObjectToClipPos(v.vertex);
    return o;
}
            
fixed4 frag(v2f i) : SV_Target
{
    float3 worldPos = mul(unity_ObjectToWorld, i.pos).xyz;
    float sdf1 = tex3D(_MainTex1, worldPos).r;
    float sdf2 = tex3D(_MainTex2, worldPos).r;
    float mixedSDF = min(sdf1, sdf2);
    float _MaxSDFValue = 0;

    // Update MaxSDFValue if necessary
    if (mixedSDF > _MaxSDFValue)
    {
        _MaxSDFValue = mixedSDF;
    }

    // Map mixed SDF value to a color gradient
    float3 color = lerp(float3(1.0, 0.0, 0.0), float3(0.0, 0.0, 1.0), saturate(mixedSDF / _MaxSDFValue));

    return float4(color, 1.0);
}

            ENDCG
        }
    }
}
