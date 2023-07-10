texture texture0 : register(s0);
sampler textureSampler0 = sampler_state
{
    texture = <texture0>;
};

texture texture1 : register(s1);
sampler textureSampler1 = sampler_state
{
    texture = <texture1>;
    MipFilter = LINEAR;
    MinFilter = LINEAR;
    MagFilter = LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;
};

float2 screenResolution;
float2 textPosition;
float time;
float repetitions;

float4 ShaderRarity(float4 screenSpace : VPOS, float2 textureCoords : TEXCOORD0, float4 sampleColor : COLOR0) : COLOR0
{
    float2 coords = (screenSpace.xy - textPosition) / screenResolution;

    float4 t0 = tex2D(textureSampler0, textureCoords);
    float4 t1 = tex2D(textureSampler1, coords * repetitions + float2(time, 0));

    t0.rgb *= t1.rgb;

    return t0 * sampleColor;
}

technique Technique1
{
    pass ShaderRarityPass
    {
        PixelShader = compile ps_3_0 ShaderRarity();
    }
}