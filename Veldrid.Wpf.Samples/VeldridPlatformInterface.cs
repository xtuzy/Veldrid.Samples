using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media;
using Veldrid.Maui.Controls.Base;
using Veldrid.Utilities;

namespace Veldrid.Wpf.Samples
{
    public sealed partial class VeldridPlatformInterface :
        BaseVeldridPlatformInterface
    {
        public VeldridPlatformView _view;

        public VeldridPlatformInterface(VeldridPlatformView view)
        {
            PlatformType = PlatformType.Desktop;

            _view = view;
            _view.SizeChanged += OnViewSizeChanged;
            _view.Loaded += OnLoaded;
            _view.Unloaded += OnUnloaded;
        }

        public override uint Width => (uint)(_view.RenderSize.Width * _view.CompositionScaleX);
        public override uint Height => (uint)(_view.RenderSize.Height * _view.CompositionScaleY);

        private void OnUnloaded()
        {
            CompositionTarget.Rendering -= RenderLoop;
            DestroyGraphicsDevice();
        }

        private void OnLoaded() => CreateGraphicsDevice();

        /// <summary>
        /// 设备的创建和销毁流程是一次性的, 而设置Drawable是可以多次的
        /// </summary>
        private void CreateGraphicsDevice()
        {
            _graphicsDevice = GraphicsDevice.CreateD3D11(new GraphicsDeviceOptions());
            double dpiScale = _view.CompositionScaleX;
            uint width = (uint)(_view.ActualWidth < 0 ? 0 : Math.Ceiling(_view.ActualWidth * dpiScale));
            uint height = (uint)(_view.ActualHeight < 0 ? 0 : Math.Ceiling(_view.ActualHeight * dpiScale));

            Module mainModule = typeof(VeldridPlatformView).Module;
            IntPtr hinstance = Marshal.GetHINSTANCE(mainModule);
            SwapchainSource win32Source = SwapchainSource.CreateWin32(_view.NativeHwnd, hinstance);
            SwapchainDescription scDesc = new SwapchainDescription(win32Source, width, height, PixelFormat.R32_Float, true);

            _swapChain = _graphicsDevice.ResourceFactory.CreateSwapchain(scDesc);

            CompositionTarget.Rendering += RenderLoop;

            _resources = new DisposeCollectorResourceFactory(_graphicsDevice.ResourceFactory);
            InvokeGraphicsDeviceCreated();
        }

        /// <summary>
        /// 释放GraphicsDevice和ResourceFactory
        /// </summary>
        private void DestroyGraphicsDevice()
        {
            if (_graphicsDevice != null)
            {
                InvokeGraphicsDeviceDestroyed();
                _graphicsDevice.WaitForIdle();
                (_resources as DisposeCollectorResourceFactory)?.DisposeCollector.DisposeAll();
                _graphicsDevice.Dispose();
                _graphicsDevice = null;
            }
        }

        /// <summary>
        /// View will still run it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RenderLoop(object? sender, EventArgs eventArgs)
        {
            if (_graphicsDevice != null) InvokeRendering(16);
        }

        private void OnViewSizeChanged()
        {
            if (_graphicsDevice != null)
            {
                double dpiScale = _view.CompositionScaleX;
                uint width = (uint)(_view.ActualWidth < 0 ? 0 : Math.Ceiling(_view.ActualWidth * dpiScale));
                uint height = (uint)(_view.ActualHeight < 0 ? 0 : Math.Ceiling(_view.ActualHeight * dpiScale));
                _swapChain.Resize(width, height);
                InvokeResized();
            }
        }
    }
}
