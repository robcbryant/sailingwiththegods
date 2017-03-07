Shader "Skybox/AlphaCutoutCubeMap" {
Properties {
    _Tint ("Tint Color", Color) = (.5, .5, .5, .5)
    _YRotation ("Y Rotation", Range(0, 360)) = 0
    _Blend ("Blend", Range(0.0,1.0)) = 0.5
    [NoScaleOffset] _Tex ("Cubemap   (HDR)", Cube) = "grey" {}
     _Cutoff ("Alpha cutoff", Range (0,1)) = 0.5
}
 
SubShader {
   // Tags { "Queue"="Transparent" "RenderType"="Background" "PreviewType"="Skybox" }
   // Cull Off ZWrite Off
   // Blend SrcAlpha OneMinusSrcAlpha
 
    Pass {
     
            AlphaTest Greater [_Cutoff]
            Material {
                Diffuse (1,1,1,1)
                Ambient (1,1,1,1)
            }
            Lighting On
            SetTexture [_MainTex] { combine texture * primary }
    }
}  
 
 
Fallback Off
 
}