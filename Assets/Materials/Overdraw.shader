Shader "Custom/Overdraw"
{
    Properties{
    _MainTex("Base", 2D) = "white" {}
    }


    SubShader{
        Fog { Mode Off }
        ZWrite Off
        ZTest Always
        Blend One One // additive blending


        Pass {
            SetTexture[_MainTex] {
                constantColor(0.1, 0.04, 0.02, 0)
                combine constant, texture
            }
        }
    }
}
