using SharpShaders.Shaders.Compute;
using SharpShaders.Textures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GPUAcceleratedNormalGenerator.Shaders
{
    public abstract class TextureComputePipeline<T> where T : TextureComputePipeline<T>, new()
    {
        private static readonly Lazy<T> Instance = new(() => new T());

        protected abstract string ShaderManifestResource { get; }
        protected abstract PixelInternalFormat InternalFormat { get; }
        protected abstract PixelFormat PixelFormat { get; }
        protected abstract PixelType PixelType { get; }
        protected abstract SizedInternalFormat ImageFormat { get; }
        protected abstract string TextureDebugName { get; }

        private ComputeShader? _shader;
        protected ComputeShader Shader => _shader ??= new ComputeShader(Misc.ReadFromManifest(ShaderManifestResource));

        private Texture? _targetTexture;
        protected Texture TargetTexture => _targetTexture!;

        protected TextureComputePipeline()
        {
            CreateTargetTexture(1024, 1024);
        }

        private void CreateTargetTexture(int width, int height)
        {
            _targetTexture?.Dispose();
            _targetTexture = Texture.LoadFromSize(
                width: width,
                height: height,
                target: TextureTarget.Texture2D,
                pixelInternalFormat: InternalFormat,
                pixelFormat: PixelFormat,
                pixelType: PixelType,
                textureSWrapMode: TextureWrapMode.ClampToEdge,
                textureTWrapMode: TextureWrapMode.ClampToEdge,
                textureMinFilter: TextureMinFilter.Linear,
                textureMagFilter: TextureMagFilter.Linear,
                name: TextureDebugName
            );
        }

        private void EnsureCapacity(int width, int height)
        {
            if (_targetTexture == null || width > _targetTexture.Width || height > _targetTexture.Height)
            {
                CreateTargetTexture(width, height);
            }
        }

        protected abstract void BindUniforms(Texture inputTexture);

        public static Texture Process(Texture inputTexture, Action<T>? configure = null)
        {
            var pipeline = Instance.Value;
            pipeline.EnsureCapacity(inputTexture.Width, inputTexture.Height);

            configure?.Invoke(pipeline);

            pipeline.Shader.Use();
            pipeline.BindUniforms(inputTexture);

            pipeline.Shader.UnitManager
                .SetImageTexture(pipeline.TargetTexture.Handle, 0, format: pipeline.ImageFormat, access: TextureAccess.WriteOnly)
                .ApplyTextures();

            pipeline.Shader.DispatchForTotalElements(inputTexture.Width, inputTexture.Height, 1);

            return pipeline.TargetTexture;
        }
    }
}
