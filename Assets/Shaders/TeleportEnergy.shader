// Shader created with Shader Forge v1.38 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.38;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0.1610384,fgcg:0.143058,fgcb:0.1985294,fgca:1,fgde:0.005,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True,fsmp:False;n:type:ShaderForge.SFN_Final,id:4795,x:33327,y:32751,varname:node_4795,prsc:2|emission-2164-OUT,alpha-2724-A,clip-3624-OUT;n:type:ShaderForge.SFN_TexCoord,id:2985,x:31630,y:32507,varname:node_2985,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:1245,x:31203,y:32362,ptovrint:False,ptlb:X panner,ptin:_Xpanner,varname:node_1245,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_ValueProperty,id:5643,x:31203,y:32464,ptovrint:False,ptlb:Y panner,ptin:_Ypanner,varname:_node_1245_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Append,id:6670,x:31427,y:32362,varname:node_6670,prsc:2|A-1245-OUT,B-5643-OUT;n:type:ShaderForge.SFN_Time,id:3174,x:31427,y:32507,varname:node_3174,prsc:2;n:type:ShaderForge.SFN_Multiply,id:905,x:31630,y:32362,varname:node_905,prsc:2|A-6670-OUT,B-3174-T;n:type:ShaderForge.SFN_Add,id:7433,x:31829,y:32362,varname:node_7433,prsc:2|A-905-OUT,B-2985-UVOUT;n:type:ShaderForge.SFN_Tex2d,id:1544,x:31604,y:33276,ptovrint:False,ptlb:Mask,ptin:_Mask,varname:node_1544,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:07d6d22864f91c34884a9b2f16c18964,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:9090,x:30983,y:32969,ptovrint:False,ptlb:DissolveAmount,ptin:_DissolveAmount,varname:node_9090,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_RemapRange,id:9421,x:31538,y:32968,varname:node_9421,prsc:2,frmn:0,frmx:1,tomn:-0.5,tomx:0.5|IN-9090-OUT;n:type:ShaderForge.SFN_RemapRange,id:8406,x:32186,y:32755,varname:node_8406,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-5366-OUT;n:type:ShaderForge.SFN_Tex2d,id:4221,x:31538,y:32754,ptovrint:False,ptlb:noise map ,ptin:_noisemap,varname:node_4221,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:5366,x:31781,y:32754,varname:node_5366,prsc:2|A-4221-RGB,B-9421-OUT;n:type:ShaderForge.SFN_Color,id:2724,x:32749,y:32491,ptovrint:False,ptlb:node_2724,ptin:_node_2724,varname:node_2724,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.625,c2:0.1148897,c3:0.1148897,c4:1;n:type:ShaderForge.SFN_Multiply,id:2164,x:33025,y:32715,varname:node_2164,prsc:2|A-2724-RGB,B-678-OUT;n:type:ShaderForge.SFN_Subtract,id:3624,x:32806,y:32981,varname:node_3624,prsc:2|A-678-OUT,B-1572-OUT;n:type:ShaderForge.SFN_Multiply,id:9492,x:31871,y:33282,varname:node_9492,prsc:2|A-1544-RGB,B-7998-OUT;n:type:ShaderForge.SFN_RemapRange,id:7998,x:31484,y:33493,varname:node_7998,prsc:2,frmn:0,frmx:1,tomn:1,tomx:1.5|IN-9090-OUT;n:type:ShaderForge.SFN_Tex2d,id:8245,x:32060,y:32362,ptovrint:False,ptlb:noise 03,ptin:_noise03,varname:_node_4221_copy,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:28c7aad1372ff114b90d330f8a2dd938,ntxv:0,isnm:False|UVIN-7433-OUT;n:type:ShaderForge.SFN_Add,id:678,x:32468,y:32737,varname:node_678,prsc:2|A-7653-OUT,B-8406-OUT;n:type:ShaderForge.SFN_Multiply,id:1572,x:32200,y:33119,varname:node_1572,prsc:2|A-9492-OUT,B-2179-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2179,x:31851,y:33450,ptovrint:False,ptlb:Radial Multiplier,ptin:_RadialMultiplier,varname:node_2179,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:6;n:type:ShaderForge.SFN_OneMinus,id:173,x:32186,y:32632,varname:node_173,prsc:2|IN-1572-OUT;n:type:ShaderForge.SFN_Add,id:7653,x:32510,y:32376,varname:node_7653,prsc:2|A-8245-RGB,B-173-OUT;proporder:5643-1245-1544-9090-4221-2724-8245-2179;pass:END;sub:END;*/

Shader "Shader Forge/TeleportEnergy" {
    Properties {
        _Ypanner ("Y panner", Float ) = 0.1
        _Xpanner ("X panner", Float ) = 0.1
        _Mask ("Mask", 2D) = "white" {}
        _DissolveAmount ("DissolveAmount", Range(0, 1)) = 1
        _noisemap ("noise map ", 2D) = "white" {}
        _node_2724 ("node_2724", Color) = (0.625,0.1148897,0.1148897,1)
        _noise03 ("noise 03", 2D) = "white" {}
        _RadialMultiplier ("Radial Multiplier", Float ) = 6
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
            Blend One One
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
            uniform float _Xpanner;
            uniform float _Ypanner;
            uniform sampler2D _Mask; uniform float4 _Mask_ST;
            uniform float _DissolveAmount;
            uniform sampler2D _noisemap; uniform float4 _noisemap_ST;
            uniform float4 _node_2724;
            uniform sampler2D _noise03; uniform float4 _noise03_ST;
            uniform float _RadialMultiplier;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                float4 node_3174 = _Time;
                float2 node_7433 = ((float2(_Xpanner,_Ypanner)*node_3174.g)+i.uv0);
                float4 _noise03_var = tex2D(_noise03,TRANSFORM_TEX(node_7433, _noise03));
                float4 _Mask_var = tex2D(_Mask,TRANSFORM_TEX(i.uv0, _Mask));
                float3 node_1572 = ((_Mask_var.rgb*(_DissolveAmount*0.5+1.0))*_RadialMultiplier);
                float4 _noisemap_var = tex2D(_noisemap,TRANSFORM_TEX(i.uv0, _noisemap));
                float3 node_678 = ((_noise03_var.rgb+(1.0 - node_1572))+((_noisemap_var.rgb+(_DissolveAmount*1.0+-0.5))*2.0+-1.0));
                clip((node_678-node_1572) - 0.5);
////// Lighting:
////// Emissive:
                float3 emissive = (_node_2724.rgb*node_678);
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,_node_2724.a);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0.1610384,0.143058,0.1985294,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
    CustomEditor "ShaderForgeMaterialInspector"
}
