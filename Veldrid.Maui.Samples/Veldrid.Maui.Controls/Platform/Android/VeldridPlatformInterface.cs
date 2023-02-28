using Android.Runtime;
using Android.Views;
using Veldrid.Maui.Controls.Base;
using Veldrid.Utilities;

namespace Veldrid.Maui.Controls.Platforms.Android
{
    public class VeldridPlatformInterface : BaseVeldridPlatformInterface
    {
        private VeldridPlatformView _view;

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

            _view = view;
            _view.AndroidSurfaceCreated += CreateGraphicsDevice;
            _view.AndroidSurfaceChanged += OnViewSizeChanged;
            _view.AndroidSurfaceDestoryed += DestroyGraphicsDevice;
        }

        private void DestroyGraphicsDevice()
        {
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
            if (Options == null)
                Options = new GraphicsDeviceOptions(false, PixelFormat.R16_UNorm, false, ResourceBindingModel.Improved, true, true);

            if (_backend == GraphicsBackend.Vulkan)
            {
                SwapchainSource ss = SwapchainSource.CreateAndroidSurface(holder.Surface.Handle, JNIEnv.Handle);
                SwapchainDescription sd = new SwapchainDescription(
                    ss,
                    (uint)Width,
                    (uint)Height,
                    Options.Value.SwapchainDepthFormat,
                    Options.Value.SyncToVerticalBlank);

                if (_graphicsDevice == null)
                {
                    _graphicsDevice = GraphicsDevice.CreateVulkan(Options.Value, sd);
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
                    Options.Value.SwapchainDepthFormat,
                    Options.Value.SyncToVerticalBlank);
                _graphicsDevice = GraphicsDevice.CreateOpenGLES(Options.Value, sd);
                _swapChain = _graphicsDevice.MainSwapchain;
            }

            _resources = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            //_resources = _graphicsDevice.ResourceFactory;
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

        private void RenderLoop()
        {
            if (_graphicsDevice != null) InvokeRendering(16);
        }

    }
}
