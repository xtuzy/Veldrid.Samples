#if __IOS__
using Microsoft.Maui.Controls;
using Microsoft.Maui.Handlers;
using UIKit;
using Veldrid;
using Veldrid.Maui.Controls.Platforms.iOS;

namespace Veldrid.Maui.Controls.Handlers
{
    public partial class VeldridViewHandler : ViewHandler<VeldridView, VeldridPlatformView>
    {
        protected override VeldridPlatformView CreatePlatformView()
        {
            var view = new VeldridPlatformView();
            GraphicsBackend backend;
            if (VirtualView.Backend == default)
                backend = GraphicsBackend.Metal;
            else
                backend = VirtualView.Backend;
            window = new VeldridPlatformInterface(view, backend);
            return view;
        }

        public override Size GetDesiredSize(double widthConstraint, double heightConstraint)
        {
            var size = base.GetDesiredSize(widthConstraint, heightConstraint);
            return size;
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
