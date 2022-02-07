Shader "Custom/Overdraw3"
{
    Properties{
        _MainTex("Base", 2D) = "white" {}
        _Color("Main Color", Color) = (0.1,0.04,0.02,0.0)
    }


    SubShader{
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

        Fog { Mode Off }
        ZWrite On
        Cull Back
        ZTest Always
        Blend OneMinusDstColor One // additive blending
        LOD 100

        Pass {
            SetTexture[_MainTex] {
                constantColor[_Color]
                combine constant, texture
            }
        }
    }
}
