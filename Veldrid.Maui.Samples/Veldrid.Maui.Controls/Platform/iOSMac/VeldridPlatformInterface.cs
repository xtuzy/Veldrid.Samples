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

        private readonly GraphicsBackend _backend;

        public override uint Width => (uint)(_view.Frame.Width * DeviceDisplay.Current.MainDisplayInfo.Density);
        public override uint Height => (uint)(_view.Frame.Height * DeviceDisplay.Current.MainDisplayInfo.Density);

        public VeldridPlatformInterface(VeldridPlatformView view, GraphicsBackend backend = GraphicsBackend.Metal)
        {
            PlatformType = PlatformType.Mobile;

            if (!(backend == GraphicsBackend.Metal || backend == GraphicsBackend.OpenGLES || backend == GraphicsBackend.Vulkan))
                throw new NotSupportedException($"Not support {backend} backend on iOS or Maccatalyst.");
            _backend = backend;

            _view = view;
            _view.ViewLoaded += CreateGraphicsDevice;
            _view.SizeChanged += OnViewSizeChanged;
            _view.ViewRemoved += DestroyGraphicsDevice;
        }

        private void RenderLoop()
        {
            if (_graphicsDevice != null)
            {
                try
                {
                    InvokeRendering(16);
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
            if (Options == null)
                //_options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved);
                Options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved, true, true);

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
                _graphicsDevice = GraphicsDevice.CreateMetal(Options.Value, scd);
                _swapChain = _graphicsDevice.MainSwapchain;
            }
            else if (_backend == GraphicsBackend.OpenGLES)
            {
                _graphicsDevice = GraphicsDevice.CreateOpenGLES(Options.Value, scd);
                _swapChain = _graphicsDevice.MainSwapchain;
            }
            else if (_backend == GraphicsBackend.Vulkan)
            {
                //need use MoltenVK nuget package
                _graphicsDevice = GraphicsDevice.CreateVulkan(Options.Value, scd);
                _swapChain = _graphicsDevice.MainSwapchain;
                //throw new NotImplementedException("Current not support Vulkan on iOS");
            }
            _resources = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            InvokeGraphicsDeviceCreated();

            Animator = new ValueAnimator();
            Animator.set(RenderLoop);
            if (AutoReDraw == true)
                Animator.start();
        }

        ValueAnimator Animator = null;
        bool autoReDraw = false;
        public override bool AutoReDraw
        {
            set
            {
                if (_graphicsDevice != null)//如果图形设备已经创建，那么动画对象已经创建，只需要判断是否开关
                {
                    if (value == true)
                    {
                        Animator.cancel();
                        Animator.start();
                    }
                    else
                        Animator.cancel();
                }
                autoReDraw = value;
            }

            get
            {
                return autoReDraw;
            }
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
