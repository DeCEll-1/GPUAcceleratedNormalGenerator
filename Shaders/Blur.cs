using SharpShaders.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPUAcceleratedNormalGenerator.Shaders
{
    public class Blur : TextureArrayComputePipeline<Blur>
    {
        protected override string ShaderManifestResource => Resources.Shaders.Blur_comp;
        protected override PixelInternalFormat InternalFormat => PixelInternalFormat.R32f;
        protected override PixelFormat PixelFormat => PixelFormat.Red;
        protected override PixelType PixelType => PixelType.Float;
        protected override SizedInternalFormat ImageFormat => SizedInternalFormat.R32f;
        protected override string TextureDebugName => "BlurMap_R32F_Array";

        private float _blurRadius = 1.0f;

        /// <summary>
        /// Configures the blur radius uniform.
        /// </summary>
        public Blur WithRadius(float radius)
        {
            _blurRadius = radius;
            return this;
        }

        protected override void BindUniforms(Texture inputTexture)
        {
            Shader.UniformManager.SetTexture("u_InputTexture", inputTexture, TextureUnit.Texture0);
            Shader.UniformManager.SetFloat("u_BlurRadius", _blurRadius);
        }

        /// <summary>
        /// Applies a compute-based blur to a Texture2DArray heightmap.
        /// </summary>
        public static Texture Convert(Texture heightmapArrayTexture, int layerCount, float radius = 1.0f)
        {
            return Process(heightmapArrayTexture, layerCount, pipeline => pipeline.WithRadius(radius));
        }
    }
}
