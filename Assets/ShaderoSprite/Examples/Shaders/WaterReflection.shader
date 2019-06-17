//////////////////////////////////////////////////////////////
/// Shadero Sprite: Sprite Shader Editor - by VETASOFT 2018 //
/// Shader generate with Shadero 1.9.6                      //
/// http://u3d.as/V7t #AssetStore                           //
/// http://www.shadero.com #Docs                            //
//////////////////////////////////////////////////////////////

Shader "Shadero Customs/WaterReflection"
{
Properties
{
[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
DistortionUV_WaveX_1("DistortionUV_WaveX_1", Range(0, 128)) = 10.4
DistortionUV_WaveY_1("DistortionUV_WaveY_1", Range(0, 128)) = 10
DistortionUV_DistanceX_1("DistortionUV_DistanceX_1", Range(0, 1)) = 0.129
DistortionUV_DistanceY_1("DistortionUV_DistanceY_1", Range(0, 1)) = 0.114
DistortionUV_Speed_1("DistortionUV_Speed_1", Range(-2, 2)) = -2
_Generate_Shape_PosX_1("_Generate_Shape_PosX_1", Range(-1, 2)) = 0.468
_Generate_Shape_PosY_1("_Generate_Shape_PosY_1", Range(-1, 2)) = 0.5
_Generate_Shape_Size_1("_Generate_Shape_Size_1", Range(-1, 1)) = 0.4
_Generate_Shape_Dist_1("_Generate_Shape_Dist_1", Range(0, 1)) = 1
_Generate_Shape_Side_1("_Generate_Shape_Side_1", Range(1, 32)) = 15
_Generate_Shape_Rotation_1("_Generate_Shape_Rotation_1", Range(-360, 360)) = 0
_Mask2uv_Fade_259("_Mask2uv_Fade_259", Range(0, 1)) = 1
ResizeUV_X_1("ResizeUV_X_1", Range(-1, 1)) = 0
ResizeUV_Y_1("ResizeUV_Y_1", Range(-1, 1)) = 0
ResizeUV_ZoomX_1("ResizeUV_ZoomX_1", Range(0.1, 3)) = 1
ResizeUV_ZoomY_1("ResizeUV_ZoomY_1", Range(0.1, 3)) = 1
_ColorRGBA_Color_1("_ColorRGBA_Color_1", COLOR) = (0.9056604,0.1751052,0.06407974,1)
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
float DistortionUV_WaveX_1;
float DistortionUV_WaveY_1;
float DistortionUV_DistanceX_1;
float DistortionUV_DistanceY_1;
float DistortionUV_Speed_1;
float _Generate_Shape_PosX_1;
float _Generate_Shape_PosY_1;
float _Generate_Shape_Size_1;
float _Generate_Shape_Dist_1;
float _Generate_Shape_Side_1;
float _Generate_Shape_Rotation_1;
float _Mask2uv_Fade_259;
float ResizeUV_X_1;
float ResizeUV_Y_1;
float ResizeUV_ZoomX_1;
float ResizeUV_ZoomY_1;
float4 _ColorRGBA_Color_1;

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


float2 DistortionUV(float2 p, float WaveX, float WaveY, float DistanceX, float DistanceY, float Speed)
{
Speed *=_Time*100;
p.x= p.x+sin(p.y*WaveX + Speed)*DistanceX*0.05;
p.y= p.y+cos(p.x*WaveY + Speed)*DistanceY*0.05;
return p;
}
float4 ColorRGBA(float4 txt, float4 color)
{
txt.rgb += color.rgb;
return txt;
}
float4 Generate_Shape(float2 uv, float posX, float posY, float Size, float Smooth, float number, float black, float rot)
{
uv = uv - float2(posX, posY);
float angle = rot * 0.01744444;
float a = atan2(uv.x, uv.y) +angle, r = 6.28318530718 / int(number);
float d = cos(floor(0.5 + a / r) * r - a) * length(uv);
float dist = 1.0 - smoothstep(Size, Size + Smooth, d);
float4 result = float4(1, 1, 1, dist);
if (black == 1) result = float4(dist, dist, dist, 1);
return result;
}
float2 FlipUV_V(float2 uv)
{
uv.y = 1 - uv.y;
return uv;
}
float2 ResizeUV(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(uv * float2(zoomx*zoomx, zoomy*zoomy), 1);
return uv;
}

float2 ResizeUVClamp(float2 uv, float offsetx, float offsety, float zoomx, float zoomy)
{
uv += float2(offsetx, offsety);
uv = fmod(clamp(uv * float2(zoomx*zoomx, zoomy*zoomy), 0.0001, 0.9999), 1);
return uv;
}
float4 frag (v2f i) : COLOR
{
float4 _MainTex_1 = tex2D(_MainTex, i.texcoord);
float2 DistortionUV_1 = DistortionUV(FlipUV_V(i.screenuv),DistortionUV_WaveX_1,DistortionUV_WaveY_1,DistortionUV_DistanceX_1,DistortionUV_DistanceY_1,DistortionUV_Speed_1);
float4 _Generate_Shape_1 = Generate_Shape(i.texcoord,_Generate_Shape_PosX_1,_Generate_Shape_PosY_1,_Generate_Shape_Size_1,_Generate_Shape_Dist_1,_Generate_Shape_Side_1,0,_Generate_Shape_Rotation_1);
float2 Mask2uv259 = lerp(DistortionUV_1,FlipUV_V(i.screenuv), lerp(_Generate_Shape_1.r, 1 - _Generate_Shape_1.r ,_Mask2uv_Fade_259));
float2 ResizeUV_1 = ResizeUV(Mask2uv259,ResizeUV_X_1,ResizeUV_Y_1,ResizeUV_ZoomX_1,ResizeUV_ZoomY_1);
float4 _GrabTexture_1 = tex2D(_GrabTexture,ResizeUV_1);
_GrabTexture_1.a = _MainTex_1.a;
float4 ColorRGBA_1 = ColorRGBA(_GrabTexture_1,_ColorRGBA_Color_1);
float4 FinalResult = ColorRGBA_1;
FinalResult.rgb *= i.color.rgb;
FinalResult.a = FinalResult.a * _SpriteFade * i.color.a;
return FinalResult;
}

ENDCG
}
}
Fallback "Sprites/Default"
}
