using CoreAnimation;
using Foundation;
using Intents;
using System.Diagnostics;
using Veldrid.Maui.Controls.Base;
using Veldrid.Utilities;

namespace Veldrid.Maui.Controls.Platforms.iOS
{
    public class VeldridPlatformInterface : //UIViewController, 
        BaseVeldridPlatformInterface
    {
        private VeldridPlatformView _view;

        private readonly GraphicsDeviceOptions _options;
        private readonly GraphicsBackend _backend;
        private CADisplayLink _timer;

        public override uint Width => (uint)(_view.Frame.Width * DeviceDisplay.Current.MainDisplayInfo.Density);
        public override uint Height => (uint)(_view.Frame.Height * DeviceDisplay.Current.MainDisplayInfo.Density);

        public VeldridPlatformInterface(VeldridPlatformView view, GraphicsBackend backend = GraphicsBackend.Metal)
        {
            PlatformType = PlatformType.Desktop;

            if (!(backend == GraphicsBackend.Metal || backend == GraphicsBackend.OpenGLES))
                throw new NotSupportedException($"Not support {backend} backend on iOS or Maccatalyst.");
            _backend = backend;

            _options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved);

            _view = view;
            _view.ViewLoaded += CreateGraphicsDevice;
            _view.SizeChanged += OnViewSizeChanged;
            _view.ViewRemoved += DestroyGraphicsDevice;
        }

        /// <summary>
        /// 在<see cref="CreateGraphicsDevice"/>后自动执行, 开使渲染循环.
        /// </summary>
        public void Run()
        {
            _timer = CADisplayLink.Create(RenderLoop);
            if (DeviceInfo.Current.Version < new Version(10, 0))
            {
                _timer.FrameInterval = 1;
            }
            else if (DeviceInfo.Current.Version < new Version(15, 0))
            {
                _timer.PreferredFramesPerSecond = 60;
            }
            else
            {
                _timer.PreferredFrameRateRange = CAFrameRateRange.Default;
            }
            _timer.AddToRunLoop(NSRunLoop.Main, NSRunLoopMode.Default);
        }

        private void RenderLoop()
        {
            float elapsed = (float)(_timer.TargetTimestamp - _timer.Timestamp);
            if (_graphicsDevice != null)
            {
                try
                {
                    InvokeRendering(elapsed);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("Encountered an error while rendering: " + e);
                    //throw;
                }
            }
        }

        private void DestroyGraphicsDevice()
        {
            if (_graphicsDevice != null)
            {
                var tempDevice = _graphicsDevice;
                _graphicsDevice = null;//先设置null阻止渲染循环

                InvokeGraphicsDeviceDestroyed();
                tempDevice.WaitForIdle();
                (_resources as DisposeCollectorResourceFactory)?.DisposeCollector.DisposeAll();
                tempDevice.Dispose();
            }
        }

        private void CreateGraphicsDevice()
        {
            SwapchainSource ss = SwapchainSource.CreateUIView(_view.Handle);
            SwapchainDescription scd = new SwapchainDescription(
                ss,
                (uint)_view.Frame.Width,//MTLSwapchain内部自动转换成Pixel
                (uint)_view.Frame.Height,
                PixelFormat.R32_Float,
                false);
            if (_backend == GraphicsBackend.Metal)
            {
                //_gd = GraphicsDevice.CreateMetal(_options);
                //_sc = _gd.ResourceFactory.CreateSwapchain(ref scd);
                _graphicsDevice = GraphicsDevice.CreateMetal(_options, scd);
                _swapChain = _graphicsDevice.MainSwapchain;
            }
            else if (_backend == GraphicsBackend.OpenGLES)
            {
                _graphicsDevice = GraphicsDevice.CreateOpenGLES(_options, scd);
                _swapChain = _graphicsDevice.MainSwapchain;
            }
            else if (_backend == GraphicsBackend.Vulkan)
            {
                //Future maybe use MoltenVK
                throw new NotImplementedException("Current not support Vulkan on iOS");
            }
            _resources = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            InvokeGraphicsDeviceCreated();

            Run();
        }

        private void OnViewSizeChanged()
        {
            if (_graphicsDevice != null)
            {
                //MTLSwapchain内部自动转换成Pixel
                _swapChain.Resize((uint)_view.Frame.Width, (uint)_view.Frame.Height);
                InvokeResized();
            }
        }
    }
}
