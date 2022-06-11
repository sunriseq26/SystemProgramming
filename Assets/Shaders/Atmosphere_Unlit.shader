// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/Atmosphere_Unlit"
{
    Properties
    {
        _AtmoColor("Atmosphere Color", Color) = (0.5, 0.5, 1.0, 1)
        _Size("Size", Float) = 0.1
        _Falloff("Falloff", Float) = 5
        _Transparency("Transparency", Float) = 15
    }

    SubShader
    {
        Tags
        {
            "LightMode" = "Always"
            "Queue" = "Transparent+1"
            "RenderType" = "Transparent"
        }
    
        Pass
        {
            Name "AtmosphereBase"
            
            Cull Front
            Blend SrcAlpha One
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            
            uniform half4 _AtmoColor;
            uniform half _Size;
            uniform half _Falloff;
            uniform fixed _Transparency;
               
            struct v2f
            {
                half4 pos : SV_POSITION;
                fixed3 normal : TEXCOORD0;
                half3 worldvertpos : TEXCOORD1;
                fixed3 viewdir: TEXCOORD2;
                fixed4 color : COLOR;
            };
               
            v2f vert(appdata_base v)
            {
                v2f o;
                       
                v.vertex.xyz += v.normal*_Size;
                o.pos = UnityObjectToClipPos (v.vertex);
                o.normal = v.normal;
                o.worldvertpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewdir = normalize(o.worldvertpos - _WorldSpaceCameraPos).xyz;      
                o.color = _AtmoColor;
                o.color.a = pow(saturate(dot(o.viewdir, o.normal)), _Falloff);
                o.color.a *= _Transparency * dot(normalize(o.worldvertpos.xyz + _WorldSpaceLightPos0.xyz), o.normal);
                return o;                
            }
               
            float4 frag(v2f i) : COLOR
            {
                return i.color;
            }
			
            ENDCG
        }
    }
}
