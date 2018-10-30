// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:7,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:2,rntp:3,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33773,y:32630,varname:node_4795,prsc:2|emission-5017-OUT,clip-3666-OUT;n:type:ShaderForge.SFN_Power,id:3666,x:32822,y:32978,varname:node_3666,prsc:2|VAL-8986-OUT,EXP-1497-OUT;n:type:ShaderForge.SFN_Slider,id:1497,x:32482,y:33148,ptovrint:False,ptlb:Fresnel,ptin:_Fresnel,varname:node_1497,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:5;n:type:ShaderForge.SFN_OneMinus,id:8986,x:32639,y:32978,varname:node_8986,prsc:2|IN-4571-OUT;n:type:ShaderForge.SFN_Dot,id:4571,x:32452,y:32978,varname:node_4571,prsc:2,dt:3|A-3866-XYZ,B-7361-OUT;n:type:ShaderForge.SFN_NormalVector,id:7361,x:32253,y:32998,prsc:2,pt:False;n:type:ShaderForge.SFN_Transform,id:3866,x:32253,y:32842,varname:node_3866,prsc:2,tffrom:3,tfto:0|IN-1461-OUT;n:type:ShaderForge.SFN_Vector3,id:1461,x:32053,y:32842,varname:node_1461,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Color,id:9661,x:32355,y:32456,ptovrint:False,ptlb:node_9661,ptin:_node_9661,varname:node_9661,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Multiply,id:4951,x:32772,y:32452,varname:node_4951,prsc:2|A-343-OUT,B-3666-OUT;n:type:ShaderForge.SFN_Multiply,id:343,x:32562,y:32328,varname:node_343,prsc:2|A-2692-OUT,B-9661-RGB;n:type:ShaderForge.SFN_Slider,id:2692,x:32060,y:32297,ptovrint:False,ptlb:node_2692,ptin:_node_2692,varname:node_2692,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:1,cur:1,max:2;n:type:ShaderForge.SFN_Color,id:5443,x:32999,y:33173,ptovrint:False,ptlb:BaseColour,ptin:_BaseColour,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8014706,c2:0.8897059,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:6380,x:32914,y:33451,ptovrint:False,ptlb:HighlightOutlineColour,ptin:_HighlightOutlineColour,varname:_Color_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5147059,c2:0.7303922,c3:1,c4:1;n:type:ShaderForge.SFN_Fresnel,id:1747,x:32683,y:33672,varname:node_1747,prsc:2;n:type:ShaderForge.SFN_Lerp,id:5017,x:33374,y:33311,varname:node_5017,prsc:2|A-5443-RGB,B-6380-RGB,T-3958-OUT;n:type:ShaderForge.SFN_ValueProperty,id:4139,x:32890,y:33727,ptovrint:False,ptlb:Outline-Cutoff,ptin:_OutlineCutoff,varname:node_4139,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.3;n:type:ShaderForge.SFN_Vector1,id:6372,x:32890,y:33800,varname:node_6372,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:7310,x:32890,y:33859,varname:node_7310,prsc:2,v1:0;n:type:ShaderForge.SFN_If,id:3958,x:33136,y:33669,varname:node_3958,prsc:2|A-1747-OUT,B-4139-OUT,GT-6372-OUT,EQ-6372-OUT,LT-7310-OUT;proporder:1497-9661-2692-5443-6380-4139;pass:END;sub:END;*/

Shader "Shader Forge/Highlight" {
    Properties {
        _Fresnel ("Fresnel", Range(0, 5)) = 0
        _node_9661 ("node_9661", Color) = (1,1,1,1)
        _node_2692 ("node_2692", Range(1, 2)) = 1
        _BaseColour ("BaseColour", Color) = (0.8014706,0.8897059,1,1)
        _HighlightOutlineColour ("HighlightOutlineColour", Color) = (0.5147059,0.7303922,1,1)
        _OutlineCutoff ("Outline-Cutoff", Float ) = 0.3
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One OneMinusSrcAlpha
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float _Fresnel;
            uniform float4 _BaseColour;
            uniform float4 _HighlightOutlineColour;
            uniform float _OutlineCutoff;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float node_3666 = pow((1.0 - abs(dot(mul( float4(float3(0,0,1),0), UNITY_MATRIX_V ).xyz.rgb,i.normalDir))),_Fresnel);
                clip(node_3666 - 0.5);
////// Lighting:
////// Emissive:
                float node_1747 = (1.0-max(0,dot(normalDirection, viewDirection)));
                float node_3958_if_leA = step(node_1747,_OutlineCutoff);
                float node_3958_if_leB = step(_OutlineCutoff,node_1747);
                float node_6372 = 1.0;
                float3 emissive = lerp(_BaseColour.rgb,_HighlightOutlineColour.rgb,lerp((node_3958_if_leA*0.0)+(node_3958_if_leB*node_6372),node_6372,node_3958_if_leA*node_3958_if_leB));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
