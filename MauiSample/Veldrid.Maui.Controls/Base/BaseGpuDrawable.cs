using System.Runtime.InteropServices;
using Veldrid.Maui.Controls.AssetPrimitives;

namespace Veldrid.Maui.Controls.Base
{
    public abstract class BaseGpuDrawable
    {
        private readonly Dictionary<Type, BinaryAssetSerializer> _serializers = DefaultSerializers.Get();

        protected Camera _camera;

        public BaseVeldridPlatformInterface PlatformInterface { get; private set; }
        public GraphicsDevice GraphicsDevice => PlatformInterface?._graphicsDevice;
        public ResourceFactory ResourceFactory => PlatformInterface?._resources;
        public Swapchain MainSwapchain => PlatformInterface?._swapChain;

        public BaseGpuDrawable()
        {
        }

        /// <summary>
        /// 将Drawable添加到VeldridPlatformInterface时,会尝试调用这个方法.其判断Device是否已经创建.
        /// </summary>
        public virtual void TryAddTo(BaseVeldridPlatformInterface platformInterface)
        {
            PlatformInterface = platformInterface;
            platformInterface.Resized += this.OnViewResize;
            platformInterface.GraphicsDeviceCreated += this.OnGraphicsDeviceCreated;
            platformInterface.GraphicsDeviceDestroyed += this.OnGraphicsDeviceDestroyed;
            platformInterface.Rendering += this.OnRender;
            _camera = new Camera(PlatformInterface.Width, PlatformInterface.Height);

            if (GraphicsDevice != null)
            {
                OnGraphicsDeviceCreated();
            }
        }

        public virtual void TryRemoveFrom(BaseVeldridPlatformInterface platformInterface)
        {
            platformInterface.Resized -= this.OnViewResize;
            platformInterface.GraphicsDeviceCreated -= this.OnGraphicsDeviceCreated;
            platformInterface.GraphicsDeviceDestroyed -= this.OnGraphicsDeviceDestroyed;
            platformInterface.Rendering -= this.OnRender;
            PlatformInterface = null;
        }

        /// <summary>
        /// 如果添加Drawable到VeldridPlatformInterface时还没有创建设备,那么在创建设备时会调用这个方法
        /// </summary>
        private void OnGraphicsDeviceCreated()
        {
            CreateResources(ResourceFactory);
            CreateSwapchainResources(ResourceFactory);
        }

        /// <summary>
        /// 子类可以在该方法中释放子类中使用的资源.
        /// </summary>
        protected virtual void OnGraphicsDeviceDestroyed()
        {
            //PlatformInterface = null;//直接设置为null在Android中退出到桌面在回来时会出错
        }

        protected virtual string GetTitle() => GetType().Name;

        protected abstract void CreateResources(ResourceFactory factory);

        protected virtual void CreateSwapchainResources(ResourceFactory factory) { }

        private void OnRender(float deltaSeconds)
        {
            if (GraphicsDevice != null)
            {
                PreDraw(deltaSeconds);
                Draw(deltaSeconds);
            }
        }
        protected void PreDraw(float deltaSeconds)
        {
            _camera.Update(deltaSeconds);
        }

        protected abstract void Draw(float deltaSeconds);

        protected virtual void OnViewResize()
        {
            _camera.WindowResized(PlatformInterface.Width, PlatformInterface.Height);
        }

        //protected virtual void OnKeyDown(KeyEvent ke) { }

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
    }
}
