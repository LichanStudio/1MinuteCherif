Shader "Custom/ProgressFill2D"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Fill Color", Color) = (1,0,0,1) // Rouge par défaut
        _Progress ("Fill Progress", Range(0, 1)) = 0.5
        _MinY ("Min Y (Base)", Float) = -1.0
        _MaxY ("Max Y (Top)", Float) = 1.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float4 vertex : SV_POSITION;
                float3 localPos : TEXCOORD0;
            };

            fixed4 _Color;
            float _Progress;
            float _MinY;
            float _MaxY;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.localPos = v.vertex.xyz; // On conserve la position locale Y
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Calcule la hauteur relative normalisée de 0 (base) à 1 (haut)
                float currentNormalizedY = (i.localPos.y - _MinY) / (_MaxY - _MinY);

                // Si le pixel dépasse la progression actuelle, on le masque (Alpha = 0)
                if (currentNormalizedY > _Progress)
                {
                    discard; // Annule le rendu du pixel
                }

                return _Color;
            }
            ENDCG
        }
    }
}