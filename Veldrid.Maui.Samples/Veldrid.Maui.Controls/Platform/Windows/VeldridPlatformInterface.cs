using System;
using System.Diagnostics;
using Veldrid;
using Veldrid.Utilities;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Veldrid.Maui.Controls.Base;

namespace Veldrid.Maui.Controls.Platforms.Windows
{
    public sealed partial class VeldridPlatformInterface :
        BaseVeldridPlatformInterface
    {
        public VeldridPlatformView _view;

        public VeldridPlatformInterface(VeldridPlatformView view)
        {
            _view = view;
            _view.CompositionScaleChanged += OnViewScaleChanged;
            _view.SizeChanged += OnViewSizeChanged;

            _view.Loaded += OnLoaded;
            _view.Unloaded += OnUnloaded;
        }

        public PlatformType PlatformType => PlatformType.Desktop;
        public override uint Width => (uint)(_view.RenderSize.Width * _view.CompositionScaleX);
        public override uint Height => (uint)(_view.RenderSize.Height * _view.CompositionScaleY);

        private void OnUnloaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            DestroyGraphicsDevice();
        }

        private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            CreateGraphicsDevice();
        }

        private void DestroyGraphicsDevice()
        {
            if (_graphicsDevice != null)
            {
                var tempDevice = _graphicsDevice;
                _graphicsDevice = null;//先设置null阻止渲染循环

                InvokeGraphicsDeviceDestroyed();
                tempDevice.WaitForIdle();
                (_resources as DisposeCollectorResourceFactory).DisposeCollector.DisposeAll();
                tempDevice.Dispose();
            }
        }

        private void CreateGraphicsDevice()
        {
            if(Options == null)
                Options = new GraphicsDeviceOptions(false, PixelFormat.R32_Float, true, ResourceBindingModel.Improved);
            var logicalDpi = 96.0f * _view.CompositionScaleX;
            var renderWidth = _view.RenderSize.Width;
            var renderHeight = _view.RenderSize.Height;
            _graphicsDevice = GraphicsDevice.CreateD3D11(Options.Value, this._view, renderWidth, renderHeight, logicalDpi);
            _resources = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            _swapChain = _graphicsDevice.MainSwapchain;
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

        /// <summary>
        /// View will still run it.
        /// </summary>
        private void RenderLoop()
        {
            if (_graphicsDevice != null)
            {
                try
                {
                    InvokeRendering(16);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Encountered an error while rendering: " + ex);
                    //throw;
                }
            }
        }

        private void OnViewSizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
        {
            if (_graphicsDevice != null)
            {
                _swapChain.Resize(Width, Height);
                InvokeResized();
            }
        }

        private void OnViewScaleChanged(SwapChainPanel sender, object args)
        {
            if (_graphicsDevice != null)
            {
                _swapChain.Resize(Width, Height);
                InvokeResized();
            }
        }
    }
}
