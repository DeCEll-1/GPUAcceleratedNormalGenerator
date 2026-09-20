using SharpShaders.Shaders.Compute;
using SharpShaders.Textures;

namespace GPUAcceleratedNormalGenerator.Shaders
{
    public class NormalMap : TextureArrayComputePipeline<NormalMap>
    {
        protected override string ShaderManifestResource => Resources.Shaders.NormalMap_comp;
        protected override PixelInternalFormat InternalFormat => PixelInternalFormat.Rgba8;
        protected override PixelFormat PixelFormat => PixelFormat.Rgba;
        protected override PixelType PixelType => PixelType.UnsignedByte;
        protected override SizedInternalFormat ImageFormat => SizedInternalFormat.Rgba8;
        protected override string TextureDebugName => "NormalMap_RGBA8_Array";

        public float Strength { get; set; } = 5.0f;

        protected override void BindUniforms(Texture inputTexture)
        {
            // Note: In your compute shader, the sampler must now be a sampler2DArray 
            // to match the incoming texture array type.
            Shader.UniformManager.SetTexture("u_InputTexture", inputTexture, TextureUnit.Texture0);
            Shader.UniformManager.SetFloat("u_Strength", Strength);
        }

        /// <summary>
        /// Processes a batch of heightmaps stored within a Texture2DArray.
        /// </summary>
        /// <param name="heightmapArrayTexture">The packed Texture2DArray of heightmaps.</param>
        /// <param name="layerCount">The number of slices/layers inside the array.</param>
        /// <param name="strength">The normal map intensity strength.</param>
        public static Texture Convert(Texture heightmapArrayTexture, int layerCount, float strength = 5.0f)
        {
            return Process(heightmapArrayTexture, layerCount, pipeline => pipeline.Strength = strength);
        }
    }
}