using Veldrid;

namespace Veldrid.macOS.Samples
{
    // Using Veldrid, only a single GraphicsDevice needs to be created,
    // even when rendering to many Swapchains in an application.
    // A Veldrid GraphicsDevice is responsible for creating all useful graphics
    // resources, including Swapchains. The GraphicsDevice in this class is used
    // to create a Swapchain in ViewController.ViewDidLoad.
    public static class AppGlobals
    {
        public static GraphicsDevice Device { get; private set; }

        public static void InitDevice()
        {
            GraphicsDeviceOptions options = new GraphicsDeviceOptions(
                debug: false,
                swapchainDepthFormat: null,
                syncToVerticalBlank: false,
                resourceBindingModel: ResourceBindingModel.Improved,
                preferDepthRangeZeroToOne: true,
                preferStandardClipSpaceYDirection: true);

            Device = GraphicsDevice.CreateMetal(options);
        }

        internal static void DisposeDevice()
        {
            Device.Dispose();
        }
    }
}
