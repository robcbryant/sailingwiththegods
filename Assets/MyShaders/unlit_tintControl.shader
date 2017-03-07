Shader "Samo_Shaders/unlit_tintControlTransparent" {
        Properties {
            _Color ("Main Color", Color) = (1,1,1,1)
            _MainTex ("Base (RGB)", 2D) = "white" {}
        }
        Category {
           Lighting Off
           ZWrite On
           Cull Back
           SubShader {
           Tags { "Queue"="Transparent" }
                Pass {
                   SetTexture [_MainTex] {
                        constantColor [_Color]
                        Combine texture * constant, texture * constant
                     }
                }
            }
        }
    }
