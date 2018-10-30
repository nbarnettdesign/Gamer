// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:1,grmd:0,uamb:True,mssp:True,bkdf:True,hqlp:False,rprd:True,enco:False,rmgx:True,imps:False,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:1,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:6,dpts:6,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:True,qofs:1,qpre:4,rntp:5,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.1610384,fgcg:0.143058,fgcb:0.1985294,fgca:1,fgde:0.005,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:2865,x:33909,y:33227,varname:node_2865,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:1632,x:31141,y:33015,ptovrint:False,ptlb:U depth speed,ptin:_Udepthspeed,varname:node_1632,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Append,id:9155,x:31313,y:33015,varname:node_9155,prsc:2|A-1632-OUT,B-1211-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1211,x:31141,y:33095,ptovrint:False,ptlb:V depth speed,ptin:_Vdepthspeed,varname:node_1211,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.4;n:type:ShaderForge.SFN_Multiply,id:1665,x:31487,y:33015,varname:node_1665,prsc:2|A-9155-OUT,B-5952-T;n:type:ShaderForge.SFN_Time,id:5952,x:31313,y:33168,varname:node_5952,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:208,x:31487,y:33146,varname:node_208,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Add,id:5476,x:31670,y:33015,varname:node_5476,prsc:2|A-1665-OUT,B-208-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:419,x:31834,y:33015,ptovrint:False,ptlb:DepthTexture,ptin:_DepthTexture,varname:node_419,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:553b45442f130fb408f44686cafa54c5,ntxv:0,isnm:False|UVIN-5476-OUT;n:type:ShaderForge.SFN_Color,id:6904,x:31834,y:33202,ptovrint:False,ptlb:Depth Color,ptin:_DepthColor,varname:node_6904,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.8551724,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4458,x:32125,y:33102,varname:node_4458,prsc:2|A-419-RGB,B-6904-RGB;n:type:ShaderForge.SFN_ValueProperty,id:6128,x:31834,y:33564,ptovrint:False,ptlb:color power,ptin:_colorpower,varname:node_6128,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.7;n:type:ShaderForge.SFN_Multiply,id:6139,x:32125,y:33405,varname:node_6139,prsc:2|A-419-A,B-6904-RGB,C-6128-OUT;n:type:ShaderForge.SFN_Multiply,id:5813,x:31480,y:33608,varname:node_5813,prsc:2|A-2038-RGB,B-7731-OUT;n:type:ShaderForge.SFN_Tex2d,id:2038,x:31288,y:33608,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:_DepthTexture_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2214d1db241136e468109ee7452bc926,ntxv:0,isnm:False|UVIN-17-OUT;n:type:ShaderForge.SFN_Add,id:17,x:31057,y:33583,varname:node_17,prsc:2|A-5257-OUT,B-4331-UVOUT;n:type:ShaderForge.SFN_Multiply,id:5257,x:30827,y:33583,varname:node_5257,prsc:2|A-3174-OUT,B-5099-T;n:type:ShaderForge.SFN_Lerp,id:6699,x:32695,y:33268,varname:node_6699,prsc:2|A-4458-OUT,B-8227-OUT,T-419-A;n:type:ShaderForge.SFN_Lerp,id:8227,x:32459,y:33364,varname:node_8227,prsc:2|A-6139-OUT,B-4219-OUT,T-9689-OUT;n:type:ShaderForge.SFN_Clamp01,id:4219,x:31859,y:33672,varname:node_4219,prsc:2|IN-5813-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7731,x:31288,y:33798,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:_FresnelPower_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.15;n:type:ShaderForge.SFN_DepthBlend,id:9689,x:32211,y:33728,varname:node_9689,prsc:2|DIST-1662-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1662,x:32038,y:33834,ptovrint:False,ptlb:Depth Blend,ptin:_DepthBlend,varname:_colorpower_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.15;n:type:ShaderForge.SFN_Time,id:5099,x:30629,y:33743,varname:node_5099,prsc:2;n:type:ShaderForge.SFN_TexCoord,id:4331,x:30862,y:33733,varname:node_4331,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:9587,x:30408,y:33582,ptovrint:False,ptlb:u speed,ptin:_uspeed,varname:_Vdepthspeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Append,id:3174,x:30629,y:33582,varname:node_3174,prsc:2|A-9587-OUT,B-4380-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4380,x:30408,y:33677,ptovrint:False,ptlb:v speed,ptin:_vspeed,varname:_uspeed_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:-0.1;n:type:ShaderForge.SFN_Add,id:7489,x:33367,y:33157,varname:node_7489,prsc:2|A-1924-RGB,B-6699-OUT;n:type:ShaderForge.SFN_Tex2d,id:1924,x:32971,y:33072,ptovrint:False,ptlb:node_1924,ptin:_node_1924,varname:node_1924,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:553b45442f130fb408f44686cafa54c5,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:6976,x:33470,y:33479,varname:node_6976,prsc:2;proporder:1632-1211-419-6904-6128-2038-7731-1662-9587-4380-1924;pass:END;sub:END;*/

Shader "Shader Forge/IntersectEnergy" {
    Properties {
        _Udepthspeed ("U depth speed", Float ) = 0
        _Vdepthspeed ("V depth speed", Float ) = -0.4
        _DepthTexture ("DepthTexture", 2D) = "white" {}
        _DepthColor ("Depth Color", Color) = (0.5,0.8551724,1,1)
        _colorpower ("color power", Float ) = 0.7
        _Texture ("Texture", 2D) = "white" {}
        _Emission ("Emission", Float ) = 0.15
        _DepthBlend ("Depth Blend", Float ) = 0.15
        _uspeed ("u speed", Float ) = 0
        _vspeed ("v speed", Float ) = -0.1
        _node_1924 ("node_1924", 2D) = "white" {}
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Overlay+1"
            "RenderType"="Overlay"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcColor
            Cull Off
            ZTest Always
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos( v.vertex );
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
                float3 finalColor = 0;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
