using Microsoft.Maui.Handlers;
using Veldrid.Maui.Controls.Base;

namespace Veldrid.Maui.Controls.Handlers
{
    public partial class VeldridViewHandler
    {
        BaseVeldridPlatformInterface window;

        public static IPropertyMapper<IVeldridView, VeldridViewHandler> Mapper = new PropertyMapper<IVeldridView, VeldridViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(IVeldridView.Drawable)] = MapDrawable,
            [nameof(IVeldridView.Background)] = MapBackground,
            [nameof(IVeldridView.AutoReDraw)] = MapAutoReDraw,
            [nameof(IVeldridView.Options)] = MapOptions
        };

        private static void MapBackground(VeldridViewHandler arg1, IVeldridView arg2)
        {
            //throw new NotImplementedException();
        }

        private static void MapAutoReDraw(VeldridViewHandler handler, IVeldridView mauiView)
        {
            handler.window.AutoReDraw = mauiView.AutoReDraw;
        }

        private static void MapOptions(VeldridViewHandler handler, IVeldridView mauiView)
        {
            handler.window.Options = mauiView.Options;
        }

        public static CommandMapper<IVeldridView, VeldridViewHandler> CommandMapper = new(ViewCommandMapper)
        {
        };

        public VeldridViewHandler() : base(Mapper, CommandMapper)
        {
        }

        public VeldridViewHandler(IPropertyMapper? mapper = null, CommandMapper? commandMapper = null)
            : base(mapper ?? Mapper, commandMapper ?? CommandMapper)
        {
        }
    }
}
