Shader "Unlit/Brush"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}

		_BrushColor("BrushColor", Color) = (1.,1.,1.,1.)
		_Radius("Radius", Range(0,1)) = 0.5
		_Blur("Blur", Range(0., 1.)) = 0.1
    }
    SubShader
    {
		Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
		LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

			float4 _BrushColor;
			float _Radius;
			float _Blur;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv = (o.uv - float2(.5, .5))*2.;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float a = 1. - smoothstep(_Radius*(1. - _Blur),_Radius*(1 + _Blur),length(i.uv));
				fixed4 col = a * _BrushColor;
                return col;
            }
            ENDCG
        }
    }
}
