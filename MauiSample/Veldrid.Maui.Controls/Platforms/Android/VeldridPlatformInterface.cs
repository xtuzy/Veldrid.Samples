using Android.Runtime;
using Android.Views;
using System.Diagnostics;
using Veldrid.Maui.Controls.Base;
using Veldrid.Utilities;

namespace Veldrid.Maui.Controls.Platforms.Android
{
    public class VeldridPlatformInterface : BaseVeldridPlatformInterface
    {
        private VeldridPlatformView _view;

        private readonly GraphicsDeviceOptions _options;
        private readonly GraphicsBackend _backend;

        // This is supposed to be a DisposeCollectorResourceFactory but it crashes mono, so change it.
        //private DisposeCollectorResourceFactory _resources;
        //private ResourceFactory _resources;

        //Android的View在代码中宽高本身使用像素为单位, 无需转换
        public override uint Width => (uint)_view.Width;
        public override uint Height => (uint)_view.Height;

        public VeldridPlatformInterface(VeldridPlatformView view, GraphicsBackend backend = GraphicsBackend.OpenGLES)
        {
            PlatformType = PlatformType.Mobile;

            if (!(backend == GraphicsBackend.Vulkan || backend == GraphicsBackend.OpenGLES))
            {
                throw new NotSupportedException($"{backend} is not supported on Android.");
            }
            _backend = backend;
            bool debug = false;
#if DEBUG
            debug = true;
#endif
            _options = new GraphicsDeviceOptions(debug, PixelFormat.R16_UNorm, false, ResourceBindingModel.Improved, true, true);

            _view = view;
            _view.AndroidSurfaceCreated += CreateGraphicsDevice;
            _view.AndroidSurfaceChanged += OnViewSizeChanged;
            _view.AndroidSurfaceDestoryed += DestroyGraphicsDevice;
        }

        private void DestroyGraphicsDevice()
        {
            _enableRun = false;
            if (_graphicsDevice != null)
            {
                var tempDevice = _graphicsDevice;
                _graphicsDevice = null;//先设置null阻止渲染循环

                InvokeGraphicsDeviceDestroyed();
                tempDevice.WaitForIdle();
                //_resources.DisposeCollector.DisposeAll();
                tempDevice.Dispose();
            }
        }

        private void OnViewSizeChanged()
        {
            if (_graphicsDevice != null)
            {
                _swapChain.Resize((uint)Width, (uint)Height);
                InvokeResized();
            }
        }

        private void CreateGraphicsDevice(ISurfaceHolder holder)
        {
            if (_backend == GraphicsBackend.Vulkan)
            {
                SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
                SwapchainDescription sd = new SwapchainDescription(
                    ss,
                    (uint)Width,
                    (uint)Height,
                    _options.SwapchainDepthFormat,
                    _options.SyncToVerticalBlank);

                if (_graphicsDevice == null)
                {
                    _graphicsDevice = GraphicsDevice.CreateVulkan(_options, sd);
                }

                //_swapChain = GraphicsDevice.ResourceFactory.CreateSwapchain(sd);
                _swapChain = _graphicsDevice.MainSwapchain;
            }
            else
            {
                SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
                SwapchainDescription sd = new SwapchainDescription(
                    ss,
                    (uint)Width,
                    (uint)Height,
                    _options.SwapchainDepthFormat,
                    _options.SyncToVerticalBlank);
                _graphicsDevice = GraphicsDevice.CreateOpenGLES(_options, sd);
                _swapChain = _graphicsDevice.MainSwapchain;
            }

            //_resources = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            _resources = _graphicsDevice.ResourceFactory;
            InvokeGraphicsDeviceCreated();

            Run();
        }

        private void RenderLoop()
        {
            if (_graphicsDevice != null) InvokeRendering(frameTime);
        }

        bool _enableRun = false;
        private void Run()
        {
            _enableRun = true;
            Task.Factory.StartNew(() => Loop(), TaskCreationOptions.LongRunning);
        }

        int frameTime = 1000 / 60;//每秒60帧
        private void Loop()
        {
            while (_enableRun)
            {
                try
                {
                    Thread.Sleep(frameTime);

                    RenderLoop();
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Encountered an error while rendering: " + e);
                    //throw;
                }
            }
        }
    }
}
