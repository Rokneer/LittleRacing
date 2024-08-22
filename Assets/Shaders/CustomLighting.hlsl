#ifndef ADDITIONAL_LIGHT_INCLUDED
#define ADDITIONAL_LIGHT_INCLUDED

void MainLight_float(float3 WorldPos, out float3 Direction, out float3 Color, 
	out float DistanceAtten, out float ShadowAtten)
{
    #ifdef SHADERGRAPH_PREVIEW
        Direction = normalize(float3(1.0f, 1.0f, 0.0f));
        Color = 1.0f;
        DistanceAtten = 1.0f;
        ShadowAtten = 1.0f;
    #else
        
    #if SHADOWS_SCREEN
        half4 clipPos = TransformWorldToHClip(WorldPos);
        half4 shadowCoord = ComputeScreenPos(clipPos);
    #else
        half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    #endif
        Light mainLight = GetMainLight(shadowCoord);
        Direction = mainLight.direction;
        Color = mainLight.color;
        DistanceAtten = mainLight.distanceAttenuation;
        ShadowAtten = mainLight.shadowAttenuation;
    #endif
}

void AdditionalLight_float(float3 WorldPos, int lightID, out float3 Direction, out float3 Color, out float DistanceAtten, out float ShadowAtten)
{
    Direction = normalize(float3(1.0f, 1.0f, 0.0f));
    Color = 0.0f;
    DistanceAtten = 0.0f;
    ShadowAtten = 0.0f;

    #ifndef SHADERGRAPH_PREVIEW
        int lightCount = GetAdditionalLightsCount();
        if(lightID < lightCount)
        {
            Light light = GetAdditionalLight(lightID, WorldPos);
            Direction = light.direction;
            Color = light.color;
            DistanceAtten  = light.distanceAttenuation;
            ShadowAtten = light.shadowAttenuation;
        }
    #endif
}

void AllAdditionalLights_float(float3 WorldPos, float3 WorldNormal, float2 CutoffThresholds, out float3 LightColor)
{
     LightColor = 0.0f;

    #ifndef SHADERGRAPH_PREVIEW
        int lightCount = GetAdditionalLightsCount();

        for(int i = 0; i < lightCount; ++i)
        {
            Light light = GetAdditionalLight(i, WorldPos);

            float3 color = dot(light.direction, WorldNormal);
            color = smoothstep(CutoffThresholds.x, CutoffThresholds.y, color);
            color *= light.color;
            color *= light.distanceAttenuation;

            LightColor += color;
        } 
    #endif
}

#endif