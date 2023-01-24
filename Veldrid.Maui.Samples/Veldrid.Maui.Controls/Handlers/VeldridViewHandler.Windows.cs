#if WINDOWS
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using Microsoft.UI.Xaml;
using Veldrid;
using Veldrid.Maui.Controls.Platforms.Windows;

namespace Veldrid.Maui.Controls.Handlers
{
    public partial class VeldridViewHandler : ViewHandler<VeldridView, VeldridPlatformView>
    {
        protected override VeldridPlatformView CreatePlatformView()
        {
            var view = new VeldridPlatformView();
            window = new VeldridPlatformInterface(view);
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
