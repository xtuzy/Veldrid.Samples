using SkiaSharp;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Maui.Controls.AssetPrimitives;
using Veldrid.Maui.Controls.Base;
using Veldrid.SPIRV;

namespace Veldrid.Maui.Samples.Core.Headless
{
    public class HeaderlessTextures : //BaseGpuDrawable, 
        IDisposable
    {
        public HeaderlessTextures()
        {
            GraphicsDevice = HeaderlessGraphicsDevice.Init();
            ResourceFactory = GraphicsDevice.ResourceFactory;
            CreateResources(ResourceFactory);
            RenderDocCapture.StartCapture();
            var data = Draw(16);
            RenderDocCapture.EndCapture();
            SaveRgba32ToPng(data);//see output path, generate headerlessResult.png

        }

        GraphicsDevice GraphicsDevice;
        ResourceFactory ResourceFactory;
        int Width = 500;
        int Height = 500;
        Texture _offscreenReadOut;
        Texture _offscreenColor;
        Framebuffer _offscreenFB;

        private DeviceBuffer _vertexBuffer;
        private Pipeline _pipeline;
        private CommandList _commandList;
        private ResourceSet _textureSet;
        private Shader[] _shaders;
        private DeviceBuffer _indexBuffer;
        ushort[] quadIndices;
        private ProcessedTexture texture;
        private Texture _surfaceTexture;
        private TextureView _surfaceTextureView;

        protected unsafe void CreateResources(ResourceFactory factory)
        {
            //vertices data of a quad
            Vector3[] quadVertices = new Vector3[]
            {
                new Vector3( 0.5f,  0.5f, 0.0f),// top right
                new Vector3( 0.5f, -0.5f, 0.0f),// bottom right
                new Vector3( -0.5f, -0.5f, 0.0f),// bottom left
                new Vector3( -0.5f, 0.5f, 0.0f), // top left 
            };

            // create Buffer for vertices data
            BufferDescription vertexBufferDescription = new BufferDescription(
                (uint)(quadVertices.Length * sizeof(Vector3)),
                BufferUsage.VertexBuffer);
            _vertexBuffer = factory.CreateBuffer(vertexBufferDescription);
            GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, quadVertices);// update data to Buffer

            // Index data (it specify how to use quadVertices)
            quadIndices = new ushort[] {
                0, 1, 2,// first triangle, order is Clockwise
                2, 3, 0 // second triangle
            };
            // create IndexBuffer
            BufferDescription indexBufferDescription = new BufferDescription(
                (uint)(quadIndices.Length * sizeof(ushort)),
                BufferUsage.IndexBuffer);
            _indexBuffer = factory.CreateBuffer(indexBufferDescription);
            GraphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);// update data to Buffer

            string vertexCode = @"
#version 450

layout(location = 0) in vec3 aPos;
layout(location = 0) out vec2 Position;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
    Position = vec2(aPos.x, aPos.y);
}";

            string fragmentCode = @"
#version 450

layout(location = 0) in vec2 Position;
layout(location = 0) out vec4 FragColor;

layout(set = 0, binding = 0) uniform texture2D SurfaceTexture;//In veldrid, use 'uniform' to input something need 'set' descriptor 
layout(set = 0, binding = 1) uniform sampler SurfaceSampler;

void main()
{
    FragColor = texture(sampler2D(SurfaceTexture, SurfaceSampler), Position);
}";
            var vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), "main");
            var fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), "main");

            // OpenGL no layout(set), so we need use Spirv to convert
            /*if (factory.BackendType == GraphicsBackend.OpenGL)
            {
                var vertexShader = factory.CreateShader(vertexShaderDesc);
                var fragmentShader = factory.CreateShader(fragmentShaderDesc);
                _shaders = new Shader[] { vertexShader, fragmentShader };
            }
            else*/
            _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);

            // VertexLayout tell Veldrid we store what in Vertex Buffer, it need match with vertex.glsl
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
               new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));

            // from file load image as texture
            //texture = new ImageProcessor().ProcessT(this.ReadEmbedAssetStream("Images.wall.jpg"), ".jpg");
            texture = LoadEmbeddedAsset<ProcessedTexture>(this.ReadEmbedAssetPath("ProcessedImages.wall.binary"));
            _surfaceTexture = texture.CreateDeviceTexture(GraphicsDevice, ResourceFactory, TextureUsage.Sampled);
            _surfaceTextureView = factory.CreateTextureView(_surfaceTexture);
            ResourceLayout textureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment))
                   );

            // create GraphicsPipeline
            _offscreenReadOut = factory.CreateTexture(TextureDescription.Texture2D((uint)Width, (uint)Height, 1, 1, PixelFormat.R32_G32_B32_A32_Float, TextureUsage.Staging));

            _offscreenColor = factory.CreateTexture(TextureDescription.Texture2D((uint)Width, (uint)Height, 1, 1, PixelFormat.R32_G32_B32_A32_Float, TextureUsage.RenderTarget));

            _offscreenFB = factory.CreateFramebuffer(new FramebufferDescription(null, _offscreenColor));

            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.Disabled;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,//draw outline or fill
                frontFace: FrontFace.Clockwise,//order of drawing point, see Indices array.
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;//basis graphics is point,line,or triangle
            pipelineDescription.ResourceLayouts = new[] { textureLayout };
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);
            pipelineDescription.Outputs = _offscreenFB.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            // create CommandList
            _commandList = factory.CreateCommandList();
            // create ResourceSet for texture
            _textureSet = factory.CreateResourceSet(new ResourceSetDescription(
               textureLayout,
               _surfaceTextureView,
               GraphicsDevice.LinearSampler));
        }

        protected byte[] Draw(float deltaSeconds)
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();

            _commandList.SetFramebuffer(_offscreenFB);
            _commandList.SetFullViewports();
            _commandList.ClearColorTarget(0, RgbaFloat.Black);

            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetPipeline(_pipeline);
            _commandList.SetGraphicsResourceSet(0, _textureSet);
            _commandList.DrawIndexed(
                indexCount: (uint)quadIndices.Length,
                instanceCount: 1,
                indexStart: 0,
                vertexOffset: 0,
                instanceStart: 0);

            //transfer GPU drawing to CPU readable one
            _commandList.CopyTexture(_offscreenFB.ColorTargets[0].Target, _offscreenReadOut);

            // End() must be called before commands can be submitted for execution.
            _commandList.End();
            GraphicsDevice?.SubmitCommands(_commandList);
            // Once commands have been submitted, the rendered image can be presented to the application window.
            //GraphicsDevice?.SwapBuffers(MainSwapchain);
            GraphicsDevice?.WaitForIdle();
            MappedResourceView<byte> view = GraphicsDevice.Map<byte>(_offscreenReadOut, MapMode.Read);

            byte[] tmp = new byte[view.SizeInBytes];

            Marshal.Copy(view.MappedResource.Data, tmp, 0, (int)view.SizeInBytes);

            GraphicsDevice.Unmap(_offscreenReadOut);

            return tmp;
        }

        public void Dispose()
        {
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _pipeline?.Dispose();
            _commandList?.Dispose();
            if (_shaders != null)
                foreach (var shader in _shaders)
                    shader?.Dispose();
            _surfaceTexture?.Dispose();
            _surfaceTextureView?.Dispose();
            _textureSet?.Dispose();
        }

        void SaveRgba32ToPng(byte[] bytes)
        {
            var flipVertical =  GraphicsDevice.BackendType == GraphicsBackend.Vulkan;
            using SKImage img = SKImage.FromPixelCopy(new SKImageInfo(bytes.Length / 16 / Height, Height, SKColorType.RgbaF32), bytes);
            using SKBitmap bmp = new SKBitmap(img.Width, img.Height);
            using SKCanvas surface = new SKCanvas(bmp);
            surface.Scale(1, flipVertical ? -1 : 1,  0, flipVertical ? Height / 2f : 0);
            surface.DrawImage(img, 0, 0);
            var imageData = SKImage.FromBitmap(bmp).Encode(SKEncodedImageFormat.Png, 99).ToArray();
            File.WriteAllBytes(Path.Combine( AppDomain.CurrentDomain.BaseDirectory, "headerlessResult.png"), imageData);
        }

        #region Load resource
        public Stream OpenEmbeddedAssetStream(string name) => GetType().Assembly.GetManifestResourceStream(name);

        public Shader LoadShader(ResourceFactory factory, string set, ShaderStages stage, string entryPoint)
        {
            string name = $"{set}-{stage.ToString().ToLower()}.{GetExtension(factory.BackendType)}";
            return factory.CreateShader(new ShaderDescription(stage, ReadEmbeddedAssetBytes(name), entryPoint));
        }

        public byte[] ReadEmbeddedAssetBytes(string name)
        {
            using (Stream stream = OpenEmbeddedAssetStream(name))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }

        private static string GetExtension(GraphicsBackend backendType)
        {
            bool isMacOS = RuntimeInformation.OSDescription.Contains("Darwin");

            return (backendType == GraphicsBackend.Direct3D11)
                ? "hlsl.bytes"
                : (backendType == GraphicsBackend.Vulkan)
                    ? "450.glsl.spv"
                    : (backendType == GraphicsBackend.Metal)
                        ? isMacOS ? "metallib" : "ios.metallib"
                        : (backendType == GraphicsBackend.OpenGL)
                            ? "330.glsl"
                            : "300.glsles";
        }

        private readonly Dictionary<Type, BinaryAssetSerializer> _serializers = DefaultSerializers.Get();

        public T LoadEmbeddedAsset<T>(string name)
        {
            if (!_serializers.TryGetValue(typeof(T), out BinaryAssetSerializer serializer))
            {
                throw new InvalidOperationException("No serializer registered for type " + typeof(T).Name);
            }

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    throw new InvalidOperationException("No embedded asset with the name " + name);
                }

                BinaryReader reader = new BinaryReader(stream);
                return (T)serializer.Read(reader);
            }
        }
        #endregion
    }
}
