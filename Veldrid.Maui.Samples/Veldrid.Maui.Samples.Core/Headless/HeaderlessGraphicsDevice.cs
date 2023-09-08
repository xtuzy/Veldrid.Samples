using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Veldrid.Maui.Samples.Core.Headless
{
    internal class HeaderlessGraphicsDevice
    {
        #region Constructors

        /// <summary>
        /// Initialize using Vulkan drivers
        /// </summary>
        /// <returns>New render</returns>
        public static GraphicsDevice InitFromVulkan(bool invertBGR = false)
        {
            var options = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved, true, true);

            var gd = GraphicsDevice.CreateVulkan(options);
            return gd;
        }

        /// <summary>
        /// Initialize using Direct3D 11 drivers
        /// </summary>
        /// <returns>New render</returns>
        public static GraphicsDevice InitFromD3D11(bool invertBGR = false)
        {
            var options = new GraphicsDeviceOptions(false, null, true, ResourceBindingModel.Improved)
            {
                HasMainSwapchain =false,
                
            };

            var gd = GraphicsDevice.CreateD3D11(options);
            return gd;
        }

        /// <summary>
        /// Initialize using Metal drivers
        /// </summary>
        /// <param name="handle">Provide a MetalKit View handle</param>
        /// <returns>Metal based render</returns>
        public static GraphicsDevice InitFromMetal(IntPtr handle)
        {
            try
            {
                var gd = GraphicsDevice.CreateMetal(new GraphicsDeviceOptions(), new SwapchainDescription(SwapchainSource.CreateUIView(handle), 20, 20, null, false));
                gd.ResourceFactory.CreateSwapchain(new SwapchainDescription(SwapchainSource.CreateUIView(handle), 20, 20, null, false)).Resize(20, 20);
                gd.WaitForIdle();

                return gd;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        public static GraphicsDevice InitFromMetal()
        {
            try
            {
#if IOS
                //GPU won't always load without these
                Veldrid.MetalBindings.MTLDevice.MTLCreateSystemDefaultDevice();
                Metal.MTLDevice.SystemDefault.Dispose();

                SwapchainSource scs = SwapchainSource.CreateUIView(new UIView().Handle);//Apparently necessary to not native crash when creating Color buffer on A10 processors(A12 won't need)
                //SwapchainSource scs = SwapchainSource.CreateUIView(UIApplication.SharedApplication.KeyWindow.RootViewController.View.Handle);//Apparently necessary to not native crash when creating Color buffer on A10 processors(A12 won't need);

                var gd = GraphicsDevice.CreateMetal(new GraphicsDeviceOptions(), new SwapchainDescription(scs, 20, 20, null, false));

                gd.WaitForIdle();

                gd.ResizeMainWindow(270, 800);

                return gd;
#endif
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }

        public static GraphicsDevice InitDesk()
        {
            if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan))
            {
                return InitFromVulkan();
            }
            else if (GraphicsDevice.IsBackendSupported(GraphicsBackend.Direct3D11))
            {
                return InitFromD3D11();
            }
            throw new NotSupportedException("No supported GPU device found for auto");
        }
        #endregion
    }
}
