using SharpShaders.Shaders.Compute;
using SharpShaders.Textures;
using System.Resources;

namespace GPUAcceleratedNormalGenerator.Shaders
{
    public class HeightMap : TextureArrayComputePipeline<HeightMap>
    {
        protected override string ShaderManifestResource => Resources.Shaders.HeightMap_comp;
        protected override PixelInternalFormat InternalFormat => PixelInternalFormat.R32f;
        protected override PixelFormat PixelFormat => PixelFormat.Red;
        protected override PixelType PixelType => PixelType.Float;
        protected override SizedInternalFormat ImageFormat => SizedInternalFormat.R32f;
        protected override string TextureDebugName => "Heightmap_R32F_Array";

        protected override void BindUniforms(Texture inputTexture)
        {
            // Note: Ensure your compute shader uses a sampler2DArray or image2DArray 
            // to correctly sample/store the layered texture slices.
            Shader.UniformManager.SetTexture("u_InputTexture", inputTexture, TextureUnit.Texture0);
        }

        /// <summary>
        /// Processes a batch of textures stored within a Texture2DArray.
        /// </summary>
        /// <param name="heightmapArrayTexture">The packed Texture2DArray.</param>
        /// <param name="layerCount">The number of slices/layers inside the array.</param>
        public static Texture Convert(Texture heightmapArrayTexture, int layerCount)
        {
            return Process(heightmapArrayTexture, layerCount);
        }
    }
}