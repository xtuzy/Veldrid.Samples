using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using Veldrid.Maui.Controls.AssetPrimitives;
using Veldrid.Maui.Controls.AssetProcessor;
using Veldrid.Maui.Controls.Base;
using Veldrid.SPIRV;

namespace Veldrid.Maui.Samples.Core.LearnOpenGL
{
    public class CoordinateSystems_More3D : BaseGpuDrawable
    {
        private DeviceBuffer _vertexBuffer;
        private Pipeline _pipeline;
        private CommandList _commandList;
        private ResourceSet _transSet;
        private ResourceSet _textureSet;
        private Shader[] _shaders;
        private DeviceBuffer _indexBuffer;
        private ProcessedTexture texture1;
        private Texture _surfaceTexture1;
        private TextureView _surfaceTextureView1;
        private ProcessedTexture texture2;
        private Texture _surfaceTexture2;
        private TextureView _surfaceTextureView2;
        private DeviceBuffer _modelBuffer;
        private DeviceBuffer _viewBuffer;
        private DeviceBuffer _projectionBuffer;
        private float[] vertices;
        private ushort[] indices;

        [StructLayout(LayoutKind.Sequential)]
        struct VerticeData
        {
            Vector3 Position;
            Vector2 TextureCoord;

            public VerticeData(float x, float y, float z, float tx, float ty)
            {
                Position = new Vector3(x, y, z);
                TextureCoord = new Vector2(tx, ty);
            }
        }

        protected unsafe override void CreateResources(ResourceFactory factory)
        {
            vertices = vertices0;
            indices = indices0;
            // create Buffer for vertices data
            BufferDescription vertexBufferDescription = new BufferDescription(
                (uint)(vertices.Length / 5f * sizeof(VerticeData)),
                BufferUsage.VertexBuffer);
            _vertexBuffer = factory.CreateBuffer(vertexBufferDescription);
            GraphicsDevice.UpdateBuffer(_vertexBuffer, 0, vertices);// update data to Buffer

            // create IndexBuffer
            BufferDescription indexBufferDescription = new BufferDescription(
                (uint)(indices.Length * sizeof(ushort)),
                BufferUsage.IndexBuffer);
            _indexBuffer = factory.CreateBuffer(indexBufferDescription);
            GraphicsDevice.UpdateBuffer(_indexBuffer, 0, indices);// update data to Buffer

            string vertexCode = @"
#version 450

layout(set = 0, binding = 0) uniform ModelTrans
{
  mat4 model;
};
layout(set = 0, binding = 1) uniform ViewTrans
{
  mat4 view;
};
layout(set = 0, binding = 2) uniform ProjectionTrans
{
  mat4 projection;
};

layout (location = 0) in vec3 aPos;
layout (location = 1) in vec2 aTexCoord;

layout (location = 0) out vec2 TexCoord;

void main()
{
    // note that we read the multiplication from right to left
    gl_Position = projection * view * model * vec4(aPos, 1.0);
    TexCoord = vec2(aTexCoord.x, 1 - aTexCoord.y);
}";

            string fragmentCode = @"
#version 450

layout (location = 0) out vec4 FragColor;
layout (location = 0) in vec2 TexCoord;

layout(set = 1, binding = 0) uniform texture2D Texture1;//In veldrid, use 'uniform' to input something need 'set' descriptor 
layout(set = 1, binding = 1) uniform texture2D Texture2;
layout(set = 1, binding = 2) uniform sampler Sampler;

void main()
{
    FragColor = mix(texture(sampler2D(Texture1, Sampler), TexCoord),texture(sampler2D(Texture2, Sampler),TexCoord) ,0.2);
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

            // VertexLayout tell Veldrid we store wnat in Vertex Buffer, it need match with vertex.glsl
            VertexLayoutDescription vertexLayout = new VertexLayoutDescription(
               new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
               new VertexElementDescription("TextureCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2));

            // from file load image as texture
            //texture1 = new ImageProcessor().ProcessT(this.ReadEmbedAssetStream("LearnOpenGL.Assets.Images.container.jpg"), ".jpg");
            texture1 = LoadEmbeddedAsset<ProcessedTexture>(this.ReadEmbedAssetPath("ProcessedImages.container.binary"));
            _surfaceTexture1 = texture1.CreateDeviceTexture(GraphicsDevice, ResourceFactory, TextureUsage.Sampled);
            _surfaceTextureView1 = factory.CreateTextureView(_surfaceTexture1);
            //texture2 = new ImageProcessor().ProcessT(this.ReadEmbedAssetStream("LearnOpenGL.Assets.Images.awesomeface.png"), ".png");
            texture2 = LoadEmbeddedAsset<ProcessedTexture>(this.ReadEmbedAssetPath("ProcessedImages.awesomeface.binary"));
            _surfaceTexture2 = texture2.CreateDeviceTexture(GraphicsDevice, ResourceFactory, TextureUsage.Sampled);
            _surfaceTextureView2 = factory.CreateTextureView(_surfaceTexture2);

            _modelBuffer = factory.CreateBuffer(new BufferDescription((uint)sizeof(Matrix4x4), BufferUsage.UniformBuffer));
            _viewBuffer = factory.CreateBuffer(new BufferDescription((uint)sizeof(Matrix4x4), BufferUsage.UniformBuffer));
            _projectionBuffer = factory.CreateBuffer(new BufferDescription((uint)sizeof(Matrix4x4), BufferUsage.UniformBuffer));
            ResourceLayout transLayout = factory.CreateResourceLayout(
               new ResourceLayoutDescription(
                   new ResourceLayoutElementDescription("model", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                   new ResourceLayoutElementDescription("view", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                   new ResourceLayoutElementDescription("projection", ResourceKind.UniformBuffer, ShaderStages.Vertex)
                   ));

            ResourceLayout textureLayout = factory.CreateResourceLayout(
                 new ResourceLayoutDescription(
                     new ResourceLayoutElementDescription("Texture1", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                     new ResourceLayoutElementDescription("Texture2", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                     new ResourceLayoutElementDescription("Sampler", ResourceKind.Sampler, ShaderStages.Fragment)
                    ));

            // create GraphicsPipeline
            GraphicsPipelineDescription pipelineDescription = new GraphicsPipelineDescription();
            pipelineDescription.BlendState = BlendStateDescription.SingleOverrideBlend;
            pipelineDescription.DepthStencilState = DepthStencilStateDescription.DepthOnlyLessEqual;
            pipelineDescription.RasterizerState = RasterizerStateDescription.CullNone;//not cull face
            pipelineDescription.PrimitiveTopology = PrimitiveTopology.TriangleList;//basis graphics is point,line,or triangle
            pipelineDescription.ResourceLayouts = new[] { transLayout, textureLayout };
            pipelineDescription.ShaderSet = new ShaderSetDescription(
                vertexLayouts: new VertexLayoutDescription[] { vertexLayout },
                shaders: _shaders);
            pipelineDescription.Outputs = MainSwapchain.Framebuffer.OutputDescription;

            _pipeline = factory.CreateGraphicsPipeline(pipelineDescription);
            // create CommandList
            _commandList = factory.CreateCommandList();
            // create ResourceSet for transform
            _transSet = factory.CreateResourceSet(new ResourceSetDescription(
              transLayout,
              _modelBuffer,
              _viewBuffer,
              _projectionBuffer
              ));
            // create ResourceSet for texture
            _textureSet = factory.CreateResourceSet(new ResourceSetDescription(
               textureLayout,
               _surfaceTextureView1,
               _surfaceTextureView2,
               GraphicsDevice.LinearSampler
               ));
        }

        private float _ticks;
        protected override void Draw(float deltaSeconds)
        {
            // Begin() must be called before commands can be issued.
            _commandList.Begin();

            _commandList.SetFramebuffer(MainSwapchain.Framebuffer);
            _commandList.ClearColorTarget(0, RgbaFloat.Black);
            _commandList.ClearDepthStencil(1f);

            _ticks += deltaSeconds;
            var angle = _ticks / 1000 * MathF.PI / 180 * 50;
            var model = Matrix4x4.CreateFromAxisAngle(Vector3.UnitX, angle)
                * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, angle);
            var view = Matrix4x4.CreateTranslation(0, 0, -3f);//var view = Matrix4x4.CreateLookAt(Vector3.UnitZ * 3f, Vector3.Zero, Vector3.UnitY);
            var projection = Matrix4x4.CreatePerspectiveFieldOfView(
                MathF.PI / 180 * 45,
                PlatformInterface.Width / PlatformInterface.Height,
                0.1f,
                100f);
            _commandList.UpdateBuffer(_modelBuffer, 0, model);
            _commandList.UpdateBuffer(_viewBuffer, 0, view);
            _commandList.UpdateBuffer(_projectionBuffer, 0, projection);

            _commandList.SetVertexBuffer(0, _vertexBuffer);
            _commandList.SetIndexBuffer(_indexBuffer, IndexFormat.UInt16);
            _commandList.SetPipeline(_pipeline);
            _commandList.SetGraphicsResourceSet(0, _transSet);
            _commandList.SetGraphicsResourceSet(1, _textureSet);
            _commandList.DrawIndexed(
                indexCount: (uint)indices.Length,
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

        float[] vertices0 = {
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,//back and right bottom triangle
             0.5f, -0.5f, -0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,//back and left top tria
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,//front and right bottom tria
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 1.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,
             0.5f, -0.5f, -0.5f,  1.0f, 1.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
             0.5f, -0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,  0.0f, 1.0f,

            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f,
             0.5f,  0.5f, -0.5f,  1.0f, 1.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
             0.5f,  0.5f,  0.5f,  1.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,  0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,  0.0f, 1.0f
        };

        ushort[] indices0 =
        {
            0,1,2, 3,4,5,
            6,7,8, 9,10,11,
            12,13,14, 15,16,17,
            18,19,20, 21,22,23,
            24,25,26, 27,28,29,
            30,31,32, 33,34,35
        };

        float[] vertices1 = new float[]
            {
                // Top
                -0.5f, +0.5f, -0.5f, 0, 0,
                +0.5f, +0.5f, -0.5f, 1, 0,
                +0.5f, +0.5f, +0.5f, 1, 1,
                -0.5f, +0.5f, +0.5f, 0, 1,
                // Bottom                                                             
                -0.5f,-0.5f, +0.5f, 0, 0,
                +0.5f,-0.5f, +0.5f, 1, 0,
                +0.5f,-0.5f, -0.5f, 1, 1,
                -0.5f,-0.5f, -0.5f, 0, 1,
                // Left                                                               
                -0.5f, +0.5f, -0.5f, 0, 0,
                -0.5f, +0.5f, +0.5f, 1, 0,
                -0.5f, -0.5f, +0.5f, 1, 1,
                -0.5f, -0.5f, -0.5f, 0, 1,
                // Right                                                              
                +0.5f, +0.5f, +0.5f, 0, 0,
                +0.5f, +0.5f, -0.5f, 1, 0,
                +0.5f, -0.5f, -0.5f, 1, 1,
                +0.5f, -0.5f, +0.5f, 0, 1,
                // Back                                                               
                +0.5f, +0.5f, -0.5f, 0, 0,
                -0.5f, +0.5f, -0.5f, 1, 0,
                -0.5f, -0.5f, -0.5f, 1, 1,
                +0.5f, -0.5f, -0.5f, 0, 1,
                // Front                                                              
                -0.5f, +0.5f, +0.5f, 0, 0,
                +0.5f, +0.5f, +0.5f, 1, 0,
                +0.5f, -0.5f, +0.5f, 1, 1,
                -0.5f, -0.5f, +0.5f, 0, 1,
            };

        ushort[] indices1 =
            {
                0,1,2, 0,2,3,
                4,5,6, 4,6,7,
                8,9,10, 8,10,11,
                12,13,14, 12,14,15,
                16,17,18, 16,18,19,
                20,21,22, 20,22,23,
            };
    }
}
