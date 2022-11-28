#if NET && !__IOS__ && !__ANDROID__ && !WINDOWS
using Microsoft.Maui.Handlers;
using System;

namespace Veldrid.Maui.Controls.Handlers
{
    public partial class VeldridViewHandler : ViewHandler<IVeldridView, object>
    {
        protected override object CreatePlatformView() => throw new NotImplementedException();


        private static void MapDrawable(VeldridViewHandler arg1, IVeldridView arg2)
        {
            throw new NotImplementedException();
        }
    }
}
#endif