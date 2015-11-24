Shader "Custom/Sprinkle" {
	Properties {
		//_MainTex ("Base (RGB)", 2D) = "white" {}
		//_Color ("color", Color) = (.5,.5,.5,1)
		//_Spec ("Spec", 2D) = "white" {}
		_Ramp ("Ramp", 2D) = "white" {}
		_Col ("col", Range(0,1)) = 0.5		
		//_Cube ("Cubemap", CUBE) = "" {}
		//_Glossiness ("Smoothness", Range(0,1)) = 0.5		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
	    #pragma surface surf Ramp

	    sampler2D _Ramp;	    
	    //float4 _Color;
	    float _Col;	 
	    //samplerCUBE _Cube;   

	    half4 LightingRamp (SurfaceOutput s, half3 lightDir, half atten) {
	        

	        half topLight = dot (s.Normal, lightDir);
	        half diff = saturate(topLight * 0.4 + 0.4) ;
    
	        half3 ramp = tex2D (_Ramp, float2(diff, _Col)).rgb;

	        
	        half4 c;	        
	        c.rgb = ramp;
	     	        
	        return c;
	    }
	    

	    struct Input {
	        //float2 uv_MainTex;
	        float3 worldRefl;
	        float3 viewDir;	        	        
        	INTERNAL_DATA
	    };
	    
	    //sampler2D _MainTex;
	    sampler2D _Spec;
	    
	    void surf (Input IN, inout SurfaceOutput o) {	    	
	    	        
	       
	       
	       half rim = saturate(dot (normalize(IN.viewDir), o.Normal));
	       half skyLight = saturate(dot (half3(0, 1, 0), o.Normal) * 0.5 + 0.5);
	       
	       	       
           o.Emission = 0.3 * rim * skyLight;
           //o.Emission += rim * texCUBE (_Cube, WorldReflectionVector (IN, o.Normal)).rgb * 0.3;
           
	    }
	    ENDCG
	} 
	FallBack "Diffuse"
}
