Shader "MrRabbit/RewardClothes"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite",2D)="white"{}
        _Tint("Clothing colour",Color)=(1,.72,.2,1)
        _Color("Tint",Color)=(1,1,1,1)
        _StencilComp("Stencil comparison",Float)=8
        _Stencil("Stencil ID",Float)=0
        _StencilOp("Stencil operation",Float)=0
        _StencilWriteMask("Stencil write",Float)=255
        _StencilReadMask("Stencil read",Float)=255
        _ColorMask("Colour mask",Float)=15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip("Alpha clip",Float)=0
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "CanUseSpriteAtlas"="True"}
        Stencil {Ref [_Stencil] Comp [_StencilComp] Pass [_StencilOp] ReadMask [_StencilReadMask] WriteMask [_StencilWriteMask]}
        Cull Off Lighting Off ZWrite Off ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha ColorMask [_ColorMask]
        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP
            #include "UnityCG.cginc"
            #include "UnityUI.cginc"
            struct appdata {float4 vertex:POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;};
            struct v2f {float4 vertex:SV_POSITION;float4 color:COLOR;float2 uv:TEXCOORD0;float4 world:TEXCOORD1;};
            // Full precision also avoids the mobile compiler's mixed min-precision bytecode bug.
            sampler2D _MainTex;float4 _Tint,_Color,_TextureSampleAdd;float4 _ClipRect;
            v2f vert(appdata v){v2f o;o.world=v.vertex;o.vertex=UnityObjectToClipPos(v.vertex);o.uv=v.uv;o.color=v.color*_Color;return o;}
            float4 frag(v2f i):SV_Target
            {
                float4 c=tex2D(_MainTex,i.uv)+_TextureSampleAdd;
                // Preserve the pink fur, eyes, brown outlines and existing gold ornaments.
                float blue=max(step(c.r*1.2,c.b)*step(c.r*1.1,c.g),step(c.r*1.1,c.b)*step(c.g*1.15,c.b));
                float light=max(c.r,max(c.g,c.b));c.rgb=lerp(c.rgb,_Tint.rgb*(.18+light*.82),blue);c*=i.color;
                #ifdef UNITY_UI_CLIP_RECT
                c.a*=UnityGet2DClipping(i.world.xy,_ClipRect);
                #endif
                #ifdef UNITY_UI_ALPHACLIP
                clip(c.a-.001);
                #endif
                return c;
            }
            ENDCG
        }
    }
}
