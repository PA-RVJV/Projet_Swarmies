// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Custom/WireframeSurfaceShader"
{
    Properties {
        _Color ("Main Color", Color) = (.5,.5,.5,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _Outline ("Outline width", Range (.002, 0.03)) = .005
    }

    SubShader {
        Tags {"Queue"="Overlay" }
        LOD 100

        Pass {
            Name "OUTLINE"
            Tags {"LightMode"="Always"}

            Cull Front

            ZWrite On
            ZTest LEqual

            ColorMask RGB

            Blend SrcAlpha OneMinusSrcAlpha

            Offset 5,5

            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }
            ENDCG
        }
    }

    SubShader {
        Tags {"Queue"="Overlay+1" }
        LOD 100

        Blend SrcAlpha OneMinusSrcAlpha

        Pass {
            Name "OUTLINE"
            Tags {"LightMode"="Always"}

            Cull Front

            ZWrite On
            ZTest LEqual

            ColorMask RGB

            Blend SrcAlpha OneMinusSrcAlpha

            Offset 5,5

            CGPROGRAM
            #pragma vertex vert
            #pragma exclude_renderers gles xbox360 ps3

            struct appdata {
                float4 vertex : POSITION;
            };

            struct v2f {
                float4 pos : POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 _OutlineColor;

            fixed4 frag(v2f i) : COLOR {
                fixed4 c = _OutlineColor;
                c.a = 1;
                return c;
            }
            ENDCG
        }
    }



    FallBack "Diffuse"
}
