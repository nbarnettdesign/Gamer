// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:32716,y:32678,varname:node_4795,prsc:2|emission-2393-OUT,alpha-798-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32479,y:32576,varname:node_2393,prsc:2|A-799-OUT,B-2053-RGB;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32225,y:32717,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Multiply,id:798,x:32472,y:33022,varname:node_798,prsc:2|A-2053-A,B-1760-A;n:type:ShaderForge.SFN_Tex2d,id:3867,x:31844,y:32495,ptovrint:False,ptlb:node_3867,ptin:_node_3867,varname:node_3867,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3b3446f9a00494945a038b3cb1e2d894,ntxv:0,isnm:False|UVIN-6479-OUT;n:type:ShaderForge.SFN_Tex2d,id:9351,x:31840,y:32773,ptovrint:False,ptlb:node_3867_copy,ptin:_node_3867_copy,varname:_node_3867_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3b3446f9a00494945a038b3cb1e2d894,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:6723,x:32090,y:32565,varname:node_6723,prsc:2|A-3867-RGB,B-9351-RGB;n:type:ShaderForge.SFN_Tex2d,id:1760,x:32140,y:33020,ptovrint:False,ptlb:node_1760,ptin:_node_1760,varname:node_1760,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:3b3446f9a00494945a038b3cb1e2d894,ntxv:0,isnm:False;n:type:ShaderForge.SFN_TexCoord,id:9966,x:31437,y:32376,varname:node_9966,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_Vector2,id:1999,x:31437,y:32276,varname:node_1999,prsc:2,v1:-0.1,v2:-0.3;n:type:ShaderForge.SFN_Add,id:6479,x:31637,y:32339,varname:node_6479,prsc:2|A-1999-OUT,B-9966-UVOUT;n:type:ShaderForge.SFN_Add,id:799,x:32075,y:32442,varname:node_799,prsc:2|A-3867-RGB,B-9351-RGB;proporder:1760-3867-9351;pass:END;sub:END;*/

Shader "Shader Forge/Fire" {
    Properties {
        _node_1760 ("node_1760", 2D) = "white" {}
        _node_3867 ("node_3867", 2D) = "white" {}
        _node_3867_copy ("node_3867_copy", 2D) = "white" {}
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform sampler2D _node_3867; uniform float4 _node_3867_ST;
            uniform sampler2D _node_3867_copy; uniform float4 _node_3867_copy_ST;
            uniform sampler2D _node_1760; uniform float4 _node_1760_ST;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.vertexColor = v.vertexColor;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float2 node_6479 = (float2(-0.1,-0.3)+i.uv0);
                float4 _node_3867_var = tex2D(_node_3867,TRANSFORM_TEX(node_6479, _node_3867));
                float4 _node_3867_copy_var = tex2D(_node_3867_copy,TRANSFORM_TEX(i.uv0, _node_3867_copy));
                float3 emissive = ((_node_3867_var.rgb+_node_3867_copy_var.rgb)*i.vertexColor.rgb);
                float3 finalColor = emissive;
                float4 _node_1760_var = tex2D(_node_1760,TRANSFORM_TEX(i.uv0, _node_1760));
                fixed4 finalRGBA = fixed4(finalColor,(i.vertexColor.a*_node_1760_var.a));
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
