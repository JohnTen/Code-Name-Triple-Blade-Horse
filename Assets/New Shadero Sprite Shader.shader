//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/New Shadero Sprite Shader"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
_NewTex_1("NewTex_1(RGB)", 2D) = "white" { }
_Displacement_Value_1("_Displacement_Value_1", Range(-0.3, 0.3)) = 0.167
_SpriteFade("SpriteFade", Range(0, 1)) = 1.0

// required for UI.Mask
[HideInInspector]_StencilComp("Stencil Comparison", Float) = 8
[HideInInspector]_Stencil("Stencil ID", Float) = 0
[HideInInspector]_StencilOp("Stencil Operation", Float) = 0
[HideInInspector]_StencilWriteMask("Stencil Write Mask", Float) = 255
[HideInInspector]_StencilReadMask("Stencil Read Mask", Float) = 255
[HideInInspector]_ColorMask("Color Mask", Float) = 15

}

SubShader
{

Tags {"Queue" = "Transparent" "IgnoreProjector" = "true" "RenderType" = "Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
ZWrite Off Blend SrcAlpha OneMinusSrcAlpha Cull Off 

GrabPass { "_GrabTexture"  } 

// required for UI.Mask
Stencil
{
Ref [_Stencil]
Comp [_StencilComp]
Pass [_StencilOp]
ReadMask [_StencilReadMask]
WriteMask [_StencilWriteMask]
}

Pass
{

CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma fragmentoption ARB_precision_hint_fastest
#include "UnityCG.cginc"

struct appdata_t{
float4 vertex   : POSITION;
float4 color    : COLOR;
float2 texcoord : TEXCOORD0;
};

struct v2f
{
float2 texcoord  : TEXCOORD0;
float4 vertex   : SV_POSITION;
float2 screenuv : TEXCOORD1;
float4 color    : COLOR;
};

sampler2D _GrabTexture;
sampler2D _MainTex;
float _SpriteFade;
sampler2D _NewTex_1;
float _Displacement_Value_1;

v2f vert(appdata_t IN)
{
v2f OUT;
OUT.vertex = UnityObjectToClipPos(IN.vertex);
float4 screenpos = ComputeGrabScreenPos(OUT.vertex);
OUT.screenuv = screenpos.xy / screenpos.w;
OUT.texcoord = IN.texcoord;
OUT.color = IN.color;
return OUT;
}


float4 DisplacementUV(float2 uv,sampler2D source,float x, float y, float value)
{
return tex2D(source,lerp(uv,uv+float2(x,y),value));
}
float4 frag (v2f i) : COLOR
{
float4 NewTex_1 = tex2D(_NewTex_1, i.texcoord);
float4 _Displacement_1 = DisplacementUV(i.screenuv,_GrabTexture,NewTex_1.r*NewTex_1.a,NewTex_1.g*NewTex_1.a,_Displacement_Value_1);
float4 FinalResult = _Displacement_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
