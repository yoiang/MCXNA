// World view projection matrix.
uniform extern float4x4 WorldViewProjection;

// A directional light.
uniform extern float3 g_vecLightDirection;
uniform extern float3 SunAmbientIntensity;

// A vector containing the different heights used to divide the terrain in ground, mud, rock and snow areas.
// Values are stored from the lowest to the highest heights.
uniform extern float3 g_vecHeights;

// Textures used for the terrain.
uniform extern texture g_texGround;
uniform extern texture g_texMud;
uniform extern texture g_texRock;
uniform extern texture g_texSnow;

// Texture samplers.
sampler2D g_smpGround = sampler_state
{
    texture		= <g_texGround>;
    MinFilter	= LINEAR;
    MagFilter	= LINEAR;
    MipFilter	= LINEAR;
    AddressU	= WRAP;
    AddressV	= WRAP;
};

sampler2D g_smpMud = sampler_state 
{
    texture		= <g_texMud>;
    MinFilter	= LINEAR;
    MagFilter	= LINEAR;
    MipFilter	= LINEAR;
    AddressU	= WRAP;
    AddressV	= WRAP;
};

sampler2D g_smpRock = sampler_state 
{
    texture		= <g_texRock>;
    MinFilter	= LINEAR;
    MagFilter	= LINEAR;
    MipFilter	= LINEAR;
    AddressU	= WRAP;
    AddressV	= WRAP;
};

sampler2D g_smpSnow = sampler_state 
{
    texture		= <g_texSnow>;
    MinFilter	= LINEAR;
    MagFilter	= LINEAR;
    MipFilter	= LINEAR;
    AddressU	= WRAP;
    AddressV	= WRAP;
};

//---------------------------------------------------------------------------------------------------------------------
// Constant & variables effect structures.
//---------------------------------------------------------------------------------------------------------------------

// Vertex shader input structure.
struct VS_INPUT
{
	float4	vPosition	:	POSITION;
	float3	vNormal		:	NORMAL;
	float2	vTexCoord	:	TEXCOORD0;
};

// Vertex shader output structure.
struct VS_OUTPUT
{
	float4 vPosition	: POSITION;
    float2 vTexCoord0	: TEXCOORD0;
    float2 vTexCoord1	: TEXCOORD1;
    float2 vTexCoord2	: TEXCOORD2;
    float2 vTexCoord3	: TEXCOORD3;
    float4 vDiffuse		: COLOR0;
};

// Pixel shader input structure.
struct PS_INPUT
{
	float2 vTexCoord0	: TEXCOORD0;
    float2 vTexCoord1	: TEXCOORD1;
    float2 vTexCoord2	: TEXCOORD2;
    float2 vTexCoord3	: TEXCOORD3;
    float4 vDiffuse		: COLOR0;
};

//---------------------------------------------------------------------------------------------------------------------
// Vertex shader.
//---------------------------------------------------------------------------------------------------------------------

VS_OUTPUT VS_Terrain(VS_INPUT suInput)
{
	VS_OUTPUT suOutput = (VS_OUTPUT)0;

	// Transform the vertices.
	suOutput.vPosition = mul(suInput.vPosition, WorldViewProjection);
	
	// Compute the diffuse color.
	float diffuse = 0.3f * saturate(dot(g_vecLightDirection, suInput.vNormal)) + 0.7f;
	
	// Compute the specular color.
	float specular = 0.0f;//saturate(3.0f * dot(g_vecLightDirection, suInput.vNormal));
	
	// Get the height from the input position y component.
	float height = suInput.vPosition.y;
	
	// Compute the ratio of each texture for this vertex and multiply it by the lighting.
	suOutput.vDiffuse.x = diffuse * saturate(100.0f * (g_vecHeights.x - height));
	suOutput.vDiffuse.y = diffuse * saturate(100.0f * (g_vecHeights.y - height)) - saturate(100.0f * (g_vecHeights.x - height));
	suOutput.vDiffuse.z = diffuse * saturate(100.0f * (g_vecHeights.z - height)) - saturate(100.0f * (g_vecHeights.y - height));
	suOutput.vDiffuse.w = (diffuse + specular) * 0.95f * saturate(100.0f * (height - g_vecHeights.z)) - saturate(100.0f * (g_vecHeights.z - height));
	
	// Scale down the texture coordinates and pass them to the pixel shader.
	float2 coordinates = suInput.vTexCoord * 0.2f;
	
	suOutput.vTexCoord0 = coordinates;
	suOutput.vTexCoord1 = coordinates;
	suOutput.vTexCoord2 = coordinates;
	suOutput.vTexCoord3 = coordinates;

	return suOutput;
}

//---------------------------------------------------------------------------------------------------------------------
// Pixel shader.
//---------------------------------------------------------------------------------------------------------------------

float4 PS_Terrain(PS_INPUT suInput) : COLOR
{
	// Sample the textures.
	float4 ground = tex2D(g_smpGround, suInput.vTexCoord0);
	float4 mud = tex2D(g_smpMud, suInput.vTexCoord1);
	float4 rock = tex2D(g_smpRock, suInput.vTexCoord2);
	float4 snow = tex2D(g_smpSnow, suInput.vTexCoord3);
	
	float4 color;
	// Compute the output color.
	if ( suInput.vDiffuse.x  > 0.5 )
	{
		color = ground * suInput.vDiffuse.x;
		color.x = color.x + SunAmbientIntensity.x;
		color.y = color.y + SunAmbientIntensity.y;
		color.z = color.z + SunAmbientIntensity.z;
	} else
	{
		color = mud * suInput.vDiffuse.y + rock * suInput.vDiffuse.z + snow * suInput.vDiffuse.w;
		color.x = color.x + SunAmbientIntensity.x;
		color.y = color.y + SunAmbientIntensity.y;
		color.z = color.z + SunAmbientIntensity.z;
	}
	
	return color;
}

//---------------------------------------------------------------------------------------------------------------------
// Default technique.
//---------------------------------------------------------------------------------------------------------------------

technique DefaultTechnique
{
	pass P0
	{
		VertexShader = compile vs_2_0 VS_Terrain();
		PixelShader = compile ps_2_0 PS_Terrain();
	}
}

