using Veldrid.Maui.Controls.Base;
using Veldrid.Maui.Controls.Handlers;

namespace Veldrid.Maui.Controls
{
    // All the code in this file is included in all platforms.
    public class VeldridView : View, IVeldridView
    {
        public static readonly BindableProperty DrawableProperty =
           BindableProperty.Create(nameof(Drawable), typeof(BaseGpuDrawable), typeof(VeldridView), null);
        public static readonly BindableProperty BackendProperty =
           BindableProperty.Create(nameof(Backend), typeof(GraphicsBackend), typeof(VeldridView), null);

        public BaseGpuDrawable Drawable
        {
            set
            {
                SetValue(DrawableProperty, value);
            }
            get { return (BaseGpuDrawable)GetValue(DrawableProperty); }
        }

        /// <summary>
        /// Windows中运行时提示SwrapChain不能设置背景
        /// </summary>
        public new Color Background
        {
            set
            {
                SetValue(DrawableProperty, value);
            }
            get { return (Color)GetValue(BackgroundProperty); }
        }

        public GraphicsBackend Backend
        {
            set
            {
                SetValue(BackendProperty, value);
            }
            get { return (GraphicsBackend)GetValue(BackendProperty); }
        }
    }
}
