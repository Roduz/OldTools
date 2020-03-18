texture imageTexture;
texture paletteTile;
texture paletteTileLine;

sampler TextureSamplerPoint = sampler_state
{
    // all samplers must specify which texture they are sampling from.
    Texture = <imageTexture>;
	//Point = low quality (but sometimes faster) mipmapping // Linear = higher quality mipmapping 
    //The MinFilter describes how the texture sampler will read for pixels larger than one texel.
    MinFilter = Point;
    //The MagFilter describes how the texture sampler will read for pixels smaller than one texel.
    MagFilter = Point;
    //The MipFilter describes how the texture sampler will combine different mip levels of the texture.
    MipFilter = Point;
    //The AddressU and AddressV values describe how the texture sampler will treat a texture coordinate that is outside the range of [0-1].
    //In this case, it "clamps", where all values less than 0 are treated as 0, and all values greater than 1 are treated as 1.  
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler PaletteSampler = sampler_state
{
    // all samplers must specify which texture they are sampling from.
    Texture = <paletteTile>;
	//Point = low quality (but sometimes faster) mipmapping // Linear = higher quality mipmapping 
    //The MinFilter describes how the texture sampler will read for pixels larger than one texel.
    MinFilter = Point;
    //The MagFilter describes how the texture sampler will read for pixels smaller than one texel.
    MagFilter = Point;
    //The MipFilter describes how the texture sampler will combine different mip levels of the texture.
    MipFilter = Point;
    //The AddressU and AddressV values describe how the texture sampler will treat a texture coordinate that is outside the range of [0-1].
    //In this case, it "clamps", where all values less than 0 are treated as 0, and all values greater than 1 are treated as 1.  
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler PaletteLineSampler = sampler_state
{
    // all samplers must specify which texture they are sampling from.
    Texture = <paletteTileLine>;
	//Point = low quality (but sometimes faster) mipmapping // Linear = higher quality mipmapping 
    //The MinFilter describes how the texture sampler will read for pixels larger than one texel.
    MinFilter = Point;
    //The MagFilter describes how the texture sampler will read for pixels smaller than one texel.
    MagFilter = Point;
    //The MipFilter describes how the texture sampler will combine different mip levels of the texture.
    MipFilter = Point;
    //The AddressU and AddressV values describe how the texture sampler will treat a texture coordinate that is outside the range of [0-1].
    //In this case, it "clamps", where all values less than 0 are treated as 0, and all values greater than 1 are treated as 1.  
    AddressU = Clamp;
    AddressV = Clamp;
};

struct PixelShaderInput
{
	float4 Color: COLOR0;
	//The interpolated texture coordinate for this pixel, calculated from the 
	//texture coordinate values passed to the rasterizer for the three vertices that
	//make up this triangle.
	float2 TextureCoordinate : TEXCOORD0;
};

float AlphaCombine(float shady, float liny)
{
	float aNew = 0;
	if (shady > 0)
		aNew = shady;
	else if (liny > shady)
		aNew = liny;
	return aNew;
};

float4 PixelShaderFunction(PixelShaderInput input) : COLOR
{
	//.r = line .g = color .b = shading .a = alpha from shading
    float4 final = {0, 0, 0, 0};
    float4 lineOut = {0, 0, 0, 0};
    float4 texPix = tex2D(TextureSamplerPoint, input.TextureCoordinate);

	float palAlpha;
	final = tex2D(PaletteSampler, float2(texPix.b, texPix.g));
	lineOut = tex2D(PaletteLineSampler, float2(0, texPix.a));
	final.a = AlphaCombine(final.a, 1-texPix.r);
	final.rgb = lerp(final.rgb, lineOut.rgb, 1-texPix.r);
	if (texPix.g==0 && texPix.r < 1)
		final.rgb = lineOut.rgb;

	return final;
}

technique ShaderRender
{
    pass P0
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}