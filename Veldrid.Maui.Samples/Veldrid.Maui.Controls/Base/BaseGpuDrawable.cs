using System.Runtime.InteropServices;
using Veldrid.Maui.Controls.AssetPrimitives;
using Veldrid.Utilities;

namespace Veldrid.Maui.Controls.Base
{
    public abstract class BaseGpuDrawable : IDisposable
    {
        private readonly Dictionary<Type, BinaryAssetSerializer> _serializers = DefaultSerializers.Get();

        protected BaseCamera _camera;

        public BaseVeldridPlatformInterface PlatformInterface { get; private set; }
        public GraphicsDevice GraphicsDevice => PlatformInterface?._graphicsDevice;
        public ResourceFactory ResourceFactory => PlatformInterface?._resources;
        public Swapchain MainSwapchain => PlatformInterface?._swapChain;

        public BaseGpuDrawable(BaseCamera camera = null)
        {
            _camera = camera;
        }

        /// <summary>
        /// 将Drawable添加到VeldridPlatformInterface时,会尝试调用这个方法.其判断Device是否已经创建.
        /// </summary>
        public virtual void TryAddTo(BaseVeldridPlatformInterface platformInterface)
        {
            PlatformInterface = platformInterface;
            if (_camera != null)
            {
                _camera.WindowResized(PlatformInterface.Width, PlatformInterface.Height);
            }

            if (GraphicsDevice != null)
            {
                OnGraphicsDeviceCreated();
            }

            platformInterface.GraphicsDeviceCreated += this.OnGraphicsDeviceCreated;
            platformInterface.Resized += this.OnViewResize;
            platformInterface.GraphicsDeviceDestroyed += this.OnGraphicsDeviceDestroyed;
            platformInterface.Rendering += this.OnRender;//渲染需要放最后, 因为一连上就会执行
        }

        public virtual void TryRemoveFrom(BaseVeldridPlatformInterface platformInterface)
        {
            platformInterface.Rendering -= this.OnRender;//先和渲染循环断开
            PlatformInterface = null;
            platformInterface.Resized -= this.OnViewResize;
            platformInterface.GraphicsDeviceCreated -= this.OnGraphicsDeviceCreated;
            platformInterface.GraphicsDeviceDestroyed -= this.OnGraphicsDeviceDestroyed;
        }

        /// <summary>
        /// 如果添加Drawable到VeldridPlatformInterface时还没有创建设备,那么在创建设备时会调用这个方法,可在其中设置资源
        /// </summary>
        protected virtual void OnGraphicsDeviceCreated()
        {
            CreateResources(ResourceFactory);
        }

        /// <summary>
        /// 子类可以在该方法中释放子类中使用的资源.
        /// </summary>
        protected virtual void OnGraphicsDeviceDestroyed()
        {
            //PlatformInterface = null;//直接设置为null在Android中退出到桌面在回来时会出错
            ReleaseResources();
        }

        protected virtual string GetTitle() => GetType().Name;

        /// <summary>
        /// 该方法被<see cref="OnGraphicsDeviceCreated"/>调用, 主要是为适配Veldrid的旧项目
        /// </summary>
        /// <param name="factory"></param>
        protected abstract void CreateResources(ResourceFactory factory);
        /// <summary>
        /// Release object created in <see cref="CreateResources(ResourceFactory)"/>
        /// </summary>
        public virtual void ReleaseResources()
        {
            (ResourceFactory as DisposeCollectorResourceFactory)?.DisposeCollector?.DisposeAll();
        }

        private void OnRender(float deltaMillisecond)
        {
            if (GraphicsDevice != null)
            {
                PreDraw(deltaMillisecond);
                Draw(deltaMillisecond);
            }
        }

        /// <summary>
        /// 其内部调用<see cref="BaseCamera.Update(float)"
        /// </summary>
        /// <param name="deltaMillisecond"></param>
        protected void PreDraw(float deltaMillisecond)
        {
            if(_camera != null)
                _camera.Update(deltaMillisecond);
        }

        /// <summary>
        /// 绘制命令在这里进行, 游戏循环里每帧都会调用它
        /// </summary>
        /// <param name="deltaMillisecond"></param>
        protected abstract void Draw(float deltaMillisecond);

        /// <summary>
        /// 视图或者Window大小改变时
        /// </summary>
        protected virtual void OnViewResize()
        {
            if (_camera != null)
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

        public virtual void Dispose()
        {
            ReleaseResources();
            GraphicsDevice?.WaitForIdle();//貌似会减少引用数
            PlatformInterface = null;
        }
    }
}
