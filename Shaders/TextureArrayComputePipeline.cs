using SharpShaders.Shaders.Compute;
using SharpShaders.Textures;
using System;
using System.Collections.Generic;
using System.Text;

namespace GPUAcceleratedNormalGenerator.Shaders
{
    public abstract class TextureArrayComputePipeline<T> where T : TextureArrayComputePipeline<T>, new()
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

        protected TextureArrayComputePipeline()
        {
            CreateTargetTexture(1024, 1024, 1);
        }

        // Added 'depth' parameter and changed target to Texture2DArray
        private void CreateTargetTexture(int width, int height, int depth)
        {
            _targetTexture?.Dispose();

            // Adjust arguments based on how SharpShaders implements Texture2DArray allocation
            _targetTexture = Texture.LoadFromSize(
                width: width,
                height: height,
                depth: depth,                             // <-- Pass depth/layers
                target: TextureTarget.Texture2DArray,     // <-- Target array instead of 2D
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

        // Added depth capacity checking
        private void EnsureCapacity(int width, int height, int depth)
        {
            if (_targetTexture == null ||
                width > _targetTexture.Width ||
                height > _targetTexture.Height ||
                depth > _targetTexture.Depth) // Assuming Texture class exposes Depth
            {
                CreateTargetTexture(width, height, depth);
            }
        }

        protected abstract void BindUniforms(Texture inputTexture);

        // Process now expects a Texture that is structured as an Array, and dispatches across Z
        public static Texture Process(Texture inputArrayTexture, int layerCount, Action<T>? configure = null)
        {
            var pipeline = Instance.Value;

            // Resize buffer if the incoming batch is larger than what's allocated
            pipeline.EnsureCapacity(inputArrayTexture.Width, inputArrayTexture.Height, layerCount);

            configure?.Invoke(pipeline);

            pipeline.Shader.Use();
            pipeline.BindUniforms(inputArrayTexture);

            pipeline.Shader.UnitManager
                .SetImageTexture(pipeline.TargetTexture.Handle, 0, format: pipeline.ImageFormat, access: TextureAccess.WriteOnly, layered: true)
                .ApplyTextures();

            // Dispatch using layerCount for the Z dimension
            pipeline.Shader.DispatchForTotalElements(inputArrayTexture.Width, inputArrayTexture.Height, layerCount);

            return pipeline.TargetTexture;
        }
    }
}
