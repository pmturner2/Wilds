Shader "Custom/SphereGrow" {
	Properties {
		[HideInInspector]
		_Center("Center", Vector) = (0, 0, 0, 0)

		_MainTex ("Base (RGB)", 2D) = "white" {}		
		//_ColorMerge ("Color Merge", Range(0.1,20)) = 8
		
		_Ramp ("Toon Ramp (RGB)", 2D) = "gray" {} 


		// Grass Texture that is overlaid
		_GrowthTex("Growth Tex", 2D) = "white" {}

		// Tint Color for Main Diffuse Texture
		_MainColor("BaseTint", Color) = (1,1,1,1)	

		// Tint Color for Grass Growth Texture
		_GrowthColor("GrowthTint", Color) = (1,1,1,1)

				//[HideInInspector]
		_BoundY("BoundY", Float) = 1
		//S[HideInInspector]
		_LBoundY("LBoundY", Float) = 1

		//[HideInInspector]
		_MaxX("MaxX", Float) = 1
		//[HideInInspector]	
		_MinX("MinX", Float) = 1
	
		[HideInInspector]	
		_TotalGrowth("TotalGrowth", Float) = 0
	}

	CGINCLUDE
			
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#include "AutoLight.cginc"

		sampler2D _MainTex;
		float4 _MainTex_ST;
		
		sampler2D _Ramp;
		
		sampler2D _GrowthTex;
		float4 _GrowthTex_ST;

		float _ColorMerge;
		float4 _MainColor;
		float4 _GrowthColor;

		fixed _LBoundY;
		fixed _BoundY;
		fixed _MaxX;
		fixed _MinX;
			
		fixed _TotalGrowth;

		float4 _Center;
		
	ENDCG

	SubShader {		
		Tags { "RenderType"="Opaque" }
		LOD 200
		Pass {
			Lighting On
			Tags { "LightMode"="ForwardBase" }
			 //Blend One One// OneMinusSrcAlpha // use alpha blending
		
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fwdbase

		
		struct VSOut
		{
			float4 wPos;
			float4 normal;
			float4 pos : SV_POSITION;			
			float2 texcoord : TEXCOORD;
			float2 texcoord1 : TEXCOORD1;
			LIGHTING_COORDS(3,4)
		};

		VSOut vert ( appdata_full v )
		{
			VSOut o;
			const float radius = 2;
			const float maxCamDist = 83;

			float3 tempVertex = v.vertex.xyz;
			float3 sphereVertex = _Center.xyz + normalize(v.vertex.xyz - _Center.xyz );
			
			o.wPos = mul(_Object2World, v.vertex);

			float t = clamp(distance(_WorldSpaceCameraPos, o.wPos) / maxCamDist, 0, 1);
			t = clamp(_SinTime.w, 0, 1);
			tempVertex = lerp(tempVertex, sphereVertex, 1 - t);
			o.wPos = mul(_Object2World, float4(tempVertex, 1));


			o.pos = mul(UNITY_MATRIX_MVP, float4(tempVertex, 1));
			
			o.normal = mul( float4(v.normal, 0), UNITY_MATRIX_IT_MV );
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _GrowthTex);
			TRANSFER_SHADOW(o);
			//TRANSFER_VERTEX_TO_FRAGMENT(o);
			return o;
		}

		half4 frag ( VSOut i ) : COLOR
		{
			// Sample original texture
			half4 col = tex2D(_MainTex, i.texcoord);
			return col;

			float3 varyingNormal = normalize(i.normal).xyz;
			//float3 viewDirection = normalize(_WorldSpaceCameraPos - float3(i.wPos));
			 
			// BASE Pass is DIRECTIONAL Light
			//float attenuation = 1.0; // No Attenuation
			float  attenuation = LIGHT_ATTENUATION(i);

			// Sample original texture
			//half4 col = tex2D(_MainTex, i.texcoord);
			// Sample growth texture
			half4 gcol = tex2D(_GrowthTex, i.texcoord1);

			// Midpoint of our range in x. Min and Max show the extents of our interaction with the terrain
			fixed midX = (_MaxX + _MinX) * 0.5f;

			// Distance from this pixel to the midpoint (in worldspace)
			fixed distToMid = abs(i.wPos.x - midX);

			// Distance from midpoint to the Bounds
			fixed distToBound = abs(_MaxX - midX);

			// X Influence
			fixed percent = 1- clamp(distToMid / distToBound, 0, 1);
				
			// Y Influence (more grass near top. As y decreases, it fades out)
			fixed ypercent = clamp((i.wPos.y - _LBoundY) / (_BoundY - _LBoundY), 0, 1);
				
			fixed totalPercent = clamp (ypercent * ypercent * ypercent*ypercent* _TotalGrowth * (percent) * 1, 0, 1);
			col = col * _MainColor;// * 2;
			gcol = gcol * _GrowthColor;// * 2;
			col = (totalPercent) * gcol + (1 - totalPercent) * col;
			
			//col.rgb = (round(col.rgb * _ColorMerge) / _ColorMerge);//*_ColorMerge)/_ColorMerge);
		
			//col.rgb = (floor(col.rgb));//*_ColorMerge)/_ColorMerge);
			
			//Based on the ambient light
			float3 lightColor = UNITY_LIGHTMODEL_AMBIENT.xyz;
			
			float3 lightDirection = normalize(float3(_WorldSpaceLightPos0));
			/*			
			float intensity = clamp(dot(normalize(lightDirection), i.normal), 0.0, 1.0);
			*/
			//Angle to the light
			//float n_dot_l = saturate (dot (varyingNormal, normalize(lightDirection))); 
			float n_dot_l =  dot (varyingNormal, lightDirection) * 0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2(n_dot_l,n_dot_l)).rgb;
			//float diff = tex2D(_Ramp, float2(n_dot_l, 0.5));
			//Update the colour
			lightColor += _LightColor0.rgb * (ramp * attenuation * 2); 
			//Product the final color
			col.rgb = lightColor * col.rgb;
			

			return col;// * attenuation;
			// Sample growth texture
			//fixed4 gcol = tex2D(_GrowthTex, i.gtexcoord);
		}

		ENDCG
		}
		/*
			Pass {
			Lighting On
			Tags { "LightMode"="ForwardAdd" }
			 Blend One One
		
		CGPROGRAM

		#pragma vertex vert
		#pragma fragment frag
		#pragma multi_compile_fwdadd

		
		struct VSOut
		{
			float4 wPos;
			float4 normal;
			float4 pos : SV_POSITION;			
			float2 texcoord : TEXCOORD;
			float2 texcoord1 : TEXCOORD1;
			LIGHTING_COORDS(3,4)
		};

		VSOut vert ( appdata_full v )
		{
			VSOut o;
			const float radius = 2;
			const float maxCamDist = 33;

			float3 tempVertex = v.vertex.xyz;
			float3 sphereVertex = normalize(v.vertex.xyz);
			
			o.wPos = mul(_Object2World, v.vertex);

			float t = clamp(distance(_WorldSpaceCameraPos, o.wPos) / maxCamDist, 0, 1);
			t = 0;
			tempVertex = lerp(tempVertex, sphereVertex, t);
			o.wPos = mul(_Object2World, float4(tempVertex, 1));


			o.pos = mul(UNITY_MATRIX_MVP, float4(tempVertex, 1));
			
			o.normal = mul( float4(v.normal, 0), UNITY_MATRIX_IT_MV );
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _GrowthTex);
			TRANSFER_SHADOW(o);
			//TRANSFER_VERTEX_TO_FRAGMENT(o);
			return o;
		}

		half4 frag ( VSOut i ) : COLOR
		{

			float3 varyingNormal = normalize(i.normal).xyz;
			//float3 viewDirection = normalize(_WorldSpaceCameraPos - float3(i.wPos));
			 
			// BASE Pass is DIRECTIONAL Light
			//float attenuation = 1.0; // No Attenuation
			float  attenuation = LIGHT_ATTENUATION(i);

			// Sample original texture
			half4 col = tex2D(_MainTex, i.texcoord);
			// Sample growth texture
			half4 gcol = tex2D(_GrowthTex, i.texcoord1);

			// Midpoint of our range in x. Min and Max show the extents of our interaction with the terrain
			fixed midX = (_MaxX + _MinX) * 0.5f;

			// Distance from this pixel to the midpoint (in worldspace)
			fixed distToMid = abs(i.wPos.x - midX);

			// Distance from midpoint to the Bounds
			fixed distToBound = abs(_MaxX - midX);

			// X Influence
			fixed percent = 1- clamp(distToMid / distToBound, 0, 1);
				
			// Y Influence (more grass near top. As y decreases, it fades out)
			fixed ypercent = clamp((i.wPos.y - _LBoundY) / (_BoundY - _LBoundY), 0, 1);
				
			fixed totalPercent = clamp (ypercent * ypercent * ypercent*ypercent* _TotalGrowth * (percent) * 1, 0, 1);
			col = col * _MainColor;// * 2;
			gcol = gcol * _GrowthColor;// * 2;
			col = (totalPercent) * gcol + (1 - totalPercent) * col;

			//col.rgb = (round(col.rgb * _ColorMerge) / _ColorMerge);//*_ColorMerge)/_ColorMerge);
									
			float3 lightDirection = normalize(float3(_WorldSpaceLightPos0) - float3(i.wPos));
			
			//float n_dot_l = saturate (dot (varyingNormal, normalize(lightDirection)));  
			float n_dot_l =  dot (varyingNormal, lightDirection) * 0.5 + 0.5;
			half3 ramp = tex2D (_Ramp, float2(n_dot_l,n_dot_l)).rgb;
			if (n_dot_l > 0)
            {
				float3 pointToLight = float3(_WorldSpaceLightPos0) - i.wPos;
				float diff = tex2D(_Ramp, float2(n_dot_l, 0.5));
				float3 lightColor = _LightColor0.rgb * (ramp * attenuation * 2); //_LightColor0.rgb * (diff * attenuation); 
				//Product the final color
				//col = fixed4( lightColor, 1);// * col.rgb * 2;
				col.rgb = lightColor * col.rgb;
			}
			else
			{
				col = fixed4(0,0,0,1);
			}
			return col;
			
		}

		ENDCG
		}*/
	} 
	FallBack "Diffuse"
}
