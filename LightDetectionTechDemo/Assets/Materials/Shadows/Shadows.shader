Shader "Custom/Shadows" {
	Properties {
        _Color ("Color", Color) = (1,1,1,1)
        [PerRendererData]_MainTex ("Sprite Texture", 2D) = "white" {}
        _Cutoff("Shadow alpha cutoff", Range(0,1)) = 0.5
		_NormalMap ("Bumpmap", 2D) = "bump" {}
		
		_AlphaCutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags
        {
            "Queue"="Geometry"
            "RenderType"="TransparentCutout"
        }
        LOD 200
 
        Cull Off
 
        CGPROGRAM
        // Lambert lighting model, and enable shadows on all light types
        #pragma surface surf Lambert addshadow fullforwardshadows
 
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0
 
        sampler2D _MainTex;
		sampler2D _NormalMap;
        fixed4 _Color;
        fixed _Cutoff;
		fixed _AlphaCutoff;
 
        struct Input
        {
            float2 uv_MainTex;
			float2 uv_NormalMap;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
			
			o.Normal = UnpackNormal (tex2D (_NormalMap, IN.uv_NormalMap));
			float ca = tex2D(_MainTex, IN.uv_MainTex).a;

			if (ca > _AlphaCutoff)
				o.Alpha = c.a;
			else
				o.Alpha = 0;
            clip(o.Alpha - _Cutoff);
        }
        ENDCG
    }
    FallBack "Diffuse"
}
