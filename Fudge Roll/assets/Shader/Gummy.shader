Shader "Custom/Gummy" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		//_Color ("color", Color) = (.5,.5,.5,1)
		_Spec ("Spec", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "white" {}
		_Col ("col", Range(0,1)) = 0.5		
		_Cube ("Cubemap", CUBE) = "" {}
		//_BumpMap ("Normals", 2D) = "bump" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
	    #pragma surface surf Ramp

	    sampler2D _Ramp;	    
	    float4 _Color;
	    float _Col;	 	    
	    samplerCUBE _Cube;   

	    half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten) {
	        half col = (floor(_Col * 4) + 0.5) * 0.25;
	        //half topLight = dot (s.Normal, float3(0, 1, 0));
	        half topLight = dot (s.Normal, lightDir);
	        half diff = saturate(topLight * 0.4 + 0.4) ;
	        //diff *= diff * diff;	        
	        half3 ramp = tex2D (_Ramp, float2(diff * s.Gloss, col)).rgb;
	        //half3 spec = tex2D (_Spec, IN.uv_MainTex).rgb;
	        //half3 ramp = tex2D (_Ramp, float2(0.7, _Col)).rgb;
	        //diff = diff * (s.Gloss + 0.2);
	        //half3 glitter = diff * (s.Gloss + half3(0.2, 0.1, 0));
	       // half3 glitter = s.Gloss * 0.3;
	        half4 c;	        
	        c.rgb = ramp;// + glitter * glitter * glitter;
	        
	        //c.rgb = saturate(c.rgb + ramp * float3(0.2, 0.08, 0.06));
	        
	        //half light = saturate(dot (s.Normal, lightDir));
	        //light *= light;	       
	        //c.rgb = c.rgb + light * ramp;
	        
	        
	        
	        //c.rgb = s.Albedo * _LightColor0.rgb * ramp * (atten * 2);
	        //c.rgb = ramp * _LightColor0.rgb * (atten * 2);
	        //c.rgb = ramp ;
	        //c *=c;	        
	        //c.a = s.Alpha;
	        
	        return c;
	    }
	    

	    struct Input {
	        float2 uv_MainTex;
	        //float2 uv2_BumpMap;
	        float3 worldRefl;
	        float3 viewDir;	        	        
        	INTERNAL_DATA
	    };
	    
	    //sampler2D _MainTex;
	    sampler2D _Spec;
	    //sampler2D _BumpMap;
	    
	    void surf (Input IN, inout SurfaceOutput o) {	    	
	    	
	    	float3 speckles = tex2D (_Spec, IN.uv_MainTex).rgb;
	    	//o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv2_BumpMap));
	    	
	    	o.Gloss = speckles.r;
	        //o.Albedo = tex2D (_MainTex, float2(0.3, _Col)).rgb;
	       // o.Emission = speckles.g * speckles.g * texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb;
	       
	       //half3 rimCol = tex2D (_Ramp, float2(0.5, _Col)).rgb;
	       
	       half rim = saturate(dot (normalize(IN.viewDir), o.Normal));
	       half skyLight = saturate(dot (half3(0, 1, 0), o.Normal) * 0.5 + 0.5);
	       
	       rim *= rim;
	       rim = 1.0 - rim;
           //o.Emission = pow (rimCol * rim, 2);
           //o.Emission = 0.4 * rim * (normalize(IN.viewDir).z * 0.5 + 0.5);
           o.Emission = (speckles.g + speckles.b) * rim * skyLight;
           o.Emission += speckles.g * texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb * 5;
           
	    }
	    ENDCG
	} 
	FallBack "Diffuse"
}
