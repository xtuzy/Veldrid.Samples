using System.Numerics;
using System.Text;
using Veldrid;
using Veldrid.Maui.Controls.Base;
using Veldrid.SPIRV;

namespace Veldrid.Maui.Samples.Core.LearnOpenGL
{
    public class HelloTriangle_ElementBufferObject : BaseGpuDrawable
    {
        private DeviceBuffer _vertexBuffer;
        private Pipeline _pipeline;
        private CommandList _commandList;
        private Shader[] _shaders;
        private DeviceBuffer _indexBuffer;
        ushort[] quadIndices;
        protected unsafe override void CreateResources(ResourceFactory factory)
        {
            //vertices data of a quad
            Vector3[] vertices = new Vector3[]
            {
                new Vector3( 0.5f,  0.5f, 0.0f),// top right
                new Vector3( 0.5f, -0.5f, 0.0f),// bottom right
                new Vector3( -0.5f, -0.5f, 0.0f),// bottom left
                new Vector3( -0.5f, 0.5f, 0.0f), // top left 
            };

            // create Buffer for vertices data
            BufferDescription vbDescription = new BufferDescription(
                (uint)(vertices.Length * sizeof(Vector3)),
                BufferUsage.VertexBuffer);
            _vertexBuffer = factory.CreateBuffer(vbDescription);
            GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, vertices);// update data to Buffer

            // Index data (it specify how to use quadVertices)
            quadIndices = new ushort[] {
                0, 1, 3,// first triangle, order is Clockwise
                1, 2, 3,// second triangle
            };
            // create IndexBuffer
            BufferDescription ibDescription = new BufferDescription(
                (uint)(quadIndices.Length * sizeof(ushort)),
                BufferUsage.IndexBuffer);
            _indexBuffer = factory.CreateBuffer(ibDescription);
            GraphicsDevice.UpdateBuffer(_indexBuffer, 0, quadIndices);// update data to Buffer

            string vertexCode = @"
#version 460

layout(location = 0) in vec3 aPos;

void main()
{
    gl_Position = vec4(aPos.x, aPos.y, aPos.z, 1.0);
}";

            string fragmentCode = @"
#version 460

layout(location = 0) out vec4 FragColor;

void main()
{
    FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
}";
            var vertexShaderDesc = new ShaderDescription(ShaderStages.Vertex, Encoding.UTF8.GetBytes(vertexCode), "main");
            var fragmentShaderDesc = new ShaderDescription(ShaderStages.Fragment, Encoding.UTF8.GetBytes(fragmentCode), "main");

            if (factory.BackendType == GraphicsBackend.OpenGL)
            {
                var vertexShader = factory.CreateShader(vertexShaderDesc);
                var fragmentShader = factory.CreateShader(fragmentShaderDesc);
                _shaders = new Shader[] { vertexShader, fragmentShader };
            }
            else
                _shaders = factory.CreateFromSpirv(vertexShaderDesc, fragmentShaderDesc);


            // VertexLayout tell Veldrid we store wnat in Vertex Buffer, it need match with vertex.glsl

            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
               new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3));

            // create GraphicsPipeline
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.Disabled;
            pipelineDescription.RasterizerState = new RasterizerStateDescription(
                cullMode: FaceCullMode.Back,
                fillMode: GraphicsDevice.Features.FillModeWireframe? PolygonFillMode.Wireframe: PolygonFillMode.Solid,//draw outline or fill
                frontFace: FrontFace.Clockwise,//order of drawing point, see Indices array.
                depthClipEnabled: true,
                scissorTestEnabled: false);
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleStrip;//basis graphics is point,line,or triangle
            pipelineDescription.ResourceLayouts = System.Array.Empty<ResourceLayout>();
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);
            pipelineDescription.Outputs = MainSwapchain.Framebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            // create CommandList
            _commandList = factory.CreateCommandList();
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
