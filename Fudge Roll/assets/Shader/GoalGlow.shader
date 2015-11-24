 Shader "Custom/GoalGlow" {
    Properties {
      _MainTex ("Texture", 2D) = "white" {}
	  _Color1 ("Color1", Color) = (0,0,0,1)
	  _Color2 ("Color2", Color) = (0,0,0,1)
    }
    SubShader {
       //Tags { "Queue"="Transparent" "RenderType"="Transparent" }
	   Tags { "Queue"="Transparent" "RenderType"="Additive" }
		CGPROGRAM
		// #pragma surface surf SimpleLambert alpha
		//#pragma surface surf NoLighting decal:add
		#pragma surface surf NoLighting alpha
		
		#include "UnityCG.cginc"
      
		half4 LightingNoLighting (SurfaceOutput s, fixed3 lightDir, fixed atten) {
			          
			return half4(s.Albedo, s.Alpha);
		}
		struct Input {
			float2 uv_MainTex;
			//float4 _Time;
			float3 viewDir;          
		};
		
		
		uniform float4 _Color1;
		uniform float4 _Color2;
		
		
		sampler2D _MainTex;
		void surf (Input IN, inout SurfaceOutput o) {
      
			float c = tex2D (_MainTex, IN.uv_MainTex).r ;
			
			half falloff = saturate(dot (o.Normal, IN.viewDir));
			falloff *= falloff;
			
			c *= falloff;
			//half alpha = c.r * falloff;
			
			float3 col = lerp(_Color1, _Color2, c);
			
			
			o.Albedo = col;
			//o.Albedo = dep;
			o.Alpha = c;
		}
		ENDCG
    } 
    Fallback "Diffuse"
}
