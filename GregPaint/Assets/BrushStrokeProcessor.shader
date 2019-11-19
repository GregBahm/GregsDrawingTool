Shader "BrushStrokeProcessor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

			float2 _StartPos;
			float2 _EndPos;
			float4 _StartColor;
			float4 _EndColor;
			float _StartWeight;
			float _EndWeight;

            sampler2D _MainTex;


			float GetSegmentAlpha(float2 uv)
			{
				float2 PixeltoStart = uv - _StartPos;
				float2 StartToEnd = _EndPos - _StartPos;

				float segLength = length(StartToEnd);
				float segMagnitude = segLength * segLength;
				float theDot = dot(PixeltoStart, StartToEnd);
				float segmentParam = theDot / segMagnitude;

				if (segmentParam < 0 || segmentParam > 1)
				{
					return 0;
				}

				float2 pointOnSeg = lerp(_StartPos, _EndPos, segmentParam);
				float ret = length(uv - pointOnSeg);
				return 1 - ret;
			}

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 oldPixel = tex2D(_MainTex, i.uv);
				float segAlpha = GetSegmentAlpha(i.uv);

                return max(oldPixel, segAlpha);
            }
            ENDCG
        }
    }
}
