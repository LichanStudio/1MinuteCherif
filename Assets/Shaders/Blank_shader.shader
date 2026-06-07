Shader "Custom/SpriteFlash"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _FlashColor ("Flash Color", Color) = (1,1,1,1) // L'alpha de cette couleur va maintenant servir d'opacité max
        _FlashAmount ("Flash Amount", Range(0.0, 1.0)) = 0.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
        Cull Off Lighting Off ZWrite Off Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t { float4 vertex : POSITION; float4 color : COLOR; float2 texcoord : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; fixed4 color : COLOR; float2 texcoord : TEXCOORD0; };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _FlashColor;
            float _FlashAmount;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color;
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                // 1. On récupère la couleur d'origine du sprite
                fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                
                // 2. On calcule l'intensité réelle du flash en multipliant 
                // le Flash Amount (du script) par l'Alpha de la couleur (de l'inspecteur)
                float realFlashAmount = _FlashAmount * _FlashColor.a;

                // 3. On applique le lerp (mélange) avec cette nouvelle intensité maximale
                c.rgb = lerp(c.rgb, _FlashColor.rgb, realFlashAmount);

                // 4. On réapplique l'alpha d'origine du sprite pour ne pas colorer le vide
                c.rgb *= c.a; 
                
                return c;
            }
            ENDCG
        }
    }
}