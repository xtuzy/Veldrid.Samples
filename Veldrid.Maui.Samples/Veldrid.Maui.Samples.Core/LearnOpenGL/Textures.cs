using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Maui.Controls.AssetPrimitives;
using Veldrid.Maui.Controls.AssetProcessor;
using Veldrid.Maui.Controls.Base;
using Veldrid.SPIRV;

namespace Veldrid.Maui.Samples.Core.LearnOpenGL
{
    public class Textures : BaseGpuDrawable
    {
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

        protected unsafe override void CreateResources(ResourceFactory factory)
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
            texture = new ImageProcessor().ProcessT(this.ReadEmbedAssetStream("Images.wall.jpg"), ".jpg");
            _surfaceTexture = texture.CreateDeviceTexture(GraphicsDevice, ResourceFactory, TextureUsage.Sampled);
            _surfaceTextureView = factory.CreateTextureView(_surfaceTexture);
            ResourceLayout textureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment))
                   );

            // create GraphicsPipeline
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
            pipelineDescription.Outputs = MainSwapchain.Framebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            // create CommandList
            _commandList = factory.CreateCommandList();
            // create ResourceSet for texture
            _textureSet = factory.CreateResourceSet(new ResourceSetDescription(
               textureLayout,
               _surfaceTextureView,
               GraphicsDevice.Aniso4xSampler));
        }

        protected override void Draw(float deltaSeconds)
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();

            _commandList.SetFramebuffer(MainSwapchain.Framebuffer);
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

            // End() must be called before commands can be submitted for execution.
            _commandList.End();
            GraphicsDevice?.SubmitCommands(_commandList);
            // Once commands have been submitted, the rendered image can be presented to the application window.
            GraphicsDevice?.SwapBuffers(MainSwapchain);
        }
    }
}
