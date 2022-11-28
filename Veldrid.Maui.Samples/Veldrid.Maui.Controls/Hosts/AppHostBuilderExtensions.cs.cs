using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid.Maui.Controls.Handlers;

namespace Veldrid.Maui.Controls.Hosts
{
    public static class AppHostBuilderExtensions
    {
        public static MauiAppBuilder UseVeldridView(this MauiAppBuilder builder)
        {
            builder.ConfigureMauiHandlers(handlers =>
            {
                handlers.AddHandler(typeof(VeldridView), typeof(VeldridViewHandler));
            });

            return builder;
        }
    }
}
