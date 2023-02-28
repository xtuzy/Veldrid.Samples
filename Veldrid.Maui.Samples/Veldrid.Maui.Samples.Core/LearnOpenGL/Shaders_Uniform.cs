using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Maui.Controls.Base;
using Veldrid.SPIRV;

namespace Veldrid.Maui.Samples.Core.LearnOpenGL
{
    public class Shaders_Uniform : BaseGpuDrawable
    {
        private DeviceBuffer _vertexBuffer;
        private Pipeline _pipeline;
        private CommandList _commandList;
        private ResourceSet _graphicsResourceSet;
        private Shader[] _shaders;
        private DeviceBuffer _indexBuffer;
        ushort[] quadIndices;
        private DeviceBuffer _ourColorBuffer;

        protected unsafe override void CreateResources(ResourceFactory factory)
        {
            //vertices data of a triangle
            Vector3[] vertices = new Vector3[]
            {
                new Vector3( -0.5f, -0.5f, 0.0f ),
                new Vector3(0.5f, -0.5f, 0.0f),
                new Vector3(0.0f, 0.5f, 0.0f),
            };

            // create Buffer for vertices data
            BufferDescription vbDescription = new BufferDescription(
                (uint)(vertices.Length * sizeof(Vector3)),
                BufferUsage.VertexBuffer);
            _vertexBuffer = factory.CreateBuffer(vbDescription);
            GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, vertices);

            // Index data
            quadIndices = new ushort[] { 0, 1, 2 };
            // create IndexBuffer
            BufferDescription ibDescription = new BufferDescription(
                (uint)(quadIndices.Length * sizeof(ushort)),
                BufferUsage.IndexBuffer);
            _indexBuffer = factory.CreateBuffer(ibDescription);
            GraphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);

            string vertexCode = @"
#version 450

layout(location = 0) in vec3 aPos;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
}";
            var vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), "main");
            
            string fragmentCode = @"
#version 450

layout(set = 0, binding = 0) uniform OurColor
{
   vec4 Color;
};

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = Color;
}";
            var fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), "main");
            

            if (factory.BackendType == GraphicsBackend.OpenGL)
            {
                var vertexShader = factory.CreateShader(vertexShaderDesc);
                var fragmentShader = factory.CreateShader(fragmentShaderDesc);
                _shaders = new Shader[] { vertexShader, fragmentShader };
            }
            else
                _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);


            // VertexLayout tell Veldrid we store what in Vertex Buffer, it need match with vertex.glsl
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
               new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));

            // create buffer for uniform
            _ourColorBuffer = factory.CreateBuffer(new BufferDescription(16, BufferUsage.UniformBuffer));
            // uniform of Resource, we need specity order of uniform 
            ResourceLayout ourColorLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("OurColor", ResourceKind.UniformBuffer, ShaderStages.Fragment)));

            // create GraphicsPipeline
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.Disabled;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: PolygonFillMode.Solid,//draw outline or fill
                frontFace: FrontFace.CounterClockwise,//order of drawing point, see Indices array.
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;
            pipelineDescription.ResourceLayouts = new ResourceLayout[] { ourColorLayout };
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);
            pipelineDescription.Outputs = MainSwapchain.Framebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(ref pipelineDescription);
            // create CommandList
            _commandList = factory.CreateCommandList();
            // create ResourceSet for uniform
            _graphicsResourceSet = factory.CreateResourceSet(new ResourceSetDescription(
               ourColorLayout, new[] { _ourColorBuffer }));
        }

        float x = 0;
        protected override void Draw(float deltaSeconds)
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();
            // update uniform color
            if (x >= Math.PI)
                x = 0;
            x = x + 0.02f;
            var green = Math.Abs(Math.Sin(x));
            Vector4 color = new Vector4(0, (float)green, 0, 1.0F);
            _commandList.UpdateBuffer(_ourColorBuffer, 0, color);

            _commandList.SetFramebuffer(MainSwapchain.Framebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);

            _commandList.SetPipeline(_pipeline);
            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetGraphicsResourceSet(0, _graphicsResourceSet);
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
            GraphicsDevice.WaitForIdle();
        }

        public override void ReleaseResources()
        {
            _indexBuffer?.Dispose();
            _vertexBuffer?.Dispose();
            _pipeline?.Dispose();
            _commandList?.Dispose();
            foreach (var shader in _shaders)
                shader?.Dispose();
        }
    }
}
