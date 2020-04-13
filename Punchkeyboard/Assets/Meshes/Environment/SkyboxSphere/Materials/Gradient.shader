// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Gradient"
{
	Properties {
	    _Color ("Bottom Color", Color) = (1,1,1,1)
	    _Color2 ("Top Color", Color) = (1,1,1,1)
	    _Scale ("Scale", Float) = 1
	}
	 
	SubShader {
	    Tags {"Queue"="Background"  "IgnoreProjector"="True"}
	    LOD 100
	 
	    ZWrite On
	 
	    Pass {
	        CGPROGRAM
	        #pragma vertex vert  
	        #pragma fragment frag
	        #include "UnityCG.cginc"
	 
	        fixed4 _Color;
	        fixed4 _Color2;
	        fixed  _Scale;
	 
	        struct v2f {
	            float4 pos : SV_POSITION;
	            fixed4 col : COLOR;
	        };
	 
	        v2f vert (appdata_full v)
	        {
	            v2f o;
	            o.pos = UnityObjectToClipPos (v.vertex);
	            o.col = lerp(_Color,_Color2, v.vertex.y * _Scale);
	            return o;
	        }
	       
	 
	        float4 frag (v2f i) : COLOR {
	            float4 c = i.col;
	            c.a = 1;
	            return c;
	        }
	            ENDCG
	        }
	    }
	}