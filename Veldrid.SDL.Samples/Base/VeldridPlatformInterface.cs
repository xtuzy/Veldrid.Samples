using System.Diagnostics;
using Veldrid;
using Veldrid.Maui.Controls.Base;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;
using Vulkan.Xlib;

namespace Veldrid.SDL.Samples.Base
{
    public sealed partial class VeldridPlatformInterface :
        BaseVeldridPlatformInterface
    {
        public VeldridPlatformWindow _view;
        GraphicsBackend _backend;

        public VeldridPlatformInterface(VeldridPlatformWindow view, GraphicsBackend backend = GraphicsBackend.Vulkan)
        {
            PlatformType = PlatformType.Desktop;

            _backend = backend;
            _view = view;
            _view.SizeChanged += OnViewSizeChanged;
            _view.Loaded += OnLoaded;
            _view.Unloaded += OnUnloaded;
        }

        public override uint Width => (uint)(_view.Width * 1);
        public override uint Height => (uint)(_view.Height * 1);

        private void OnUnloaded()
        {
            DestroyGraphicsDevice();
        }

        private void OnLoaded() => CreateGraphicsDevice();

        /// <summary>
        /// 设备的创建和销毁流程是一次性的, 而设置Drawable是可以多次的
        /// </summary>
        private void CreateGraphicsDevice()
        {
            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true,
                Debug = true,

                SwapchainDepthFormat = PixelFormat.R16_UNorm,
                SyncToVerticalBlank = true,
                ResourceBindingModel = ResourceBindingModel.Improved,
            };

            _graphicsDevice = VeldridStartup.CreateGraphicsDevice(_view.Window, options, _backend);
            _swapChain = _graphicsDevice.MainSwapchain;
            _resources = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            InvokeGraphicsDeviceCreated();

            Run();
        }

        #region Render
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

        private void RenderLoop()
        {
            if (_graphicsDevice != null)
            {
                InvokeRendering(frameTime);
            }
        }
        #endregion

        /// <summary>
        /// 释放GraphicsDevice和ResourceFactory
        /// </summary>
        private void DestroyGraphicsDevice()
        {
            _enableRun = false;
            if (_graphicsDevice != null)
            {
                InvokeGraphicsDeviceDestroyed();
                _graphicsDevice.WaitForIdle();
                (_resources as DisposeCollectorResourceFactory)?.DisposeCollector.DisposeAll();
                _graphicsDevice.Dispose();
                _graphicsDevice = null;
            }
        }

        private void OnViewSizeChanged()
        {
            if (_graphicsDevice != null)
            {
                _swapChain.Resize(Width, Height);
                InvokeResized();
            }
        }
    }
}
