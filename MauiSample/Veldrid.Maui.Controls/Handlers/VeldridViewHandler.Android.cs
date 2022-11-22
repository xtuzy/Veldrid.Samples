#if __ANDROID__
using Android.Content;
using Microsoft.Maui.Handlers;
using Veldrid;
using Veldrid.Maui.Controls.Platforms.Android;

namespace Veldrid.Maui.Controls.Handlers
{
    public partial class VeldridViewHandler : ViewHandler<VeldridView, VeldridPlatformView>
    {
        protected override VeldridPlatformView CreatePlatformView()
        {
            GraphicsBackend backend;
            if (VirtualView.Backend == default)
                backend = GraphicsDevice.IsBackendSupported(GraphicsBackend.Vulkan) ? GraphicsBackend.Vulkan : GraphicsBackend.OpenGLES;
            else
                backend = VirtualView.Backend;
            var view = new VeldridPlatformView(Context);
            window = new VeldridPlatformInterface(view, backend);

            return view;
        }

        private static void MapDrawable(VeldridViewHandler handler, IVeldridView mauiView)
        {
            handler.window.Drawable = mauiView.Drawable;
        }

        protected override void ConnectHandler(VeldridPlatformView platformView)
        {
            base.ConnectHandler(platformView);
        }
    }
}
#endif
