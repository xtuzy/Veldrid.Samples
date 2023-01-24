using Veldrid.Maui.Controls.Base;

namespace Veldrid.Maui.Controls.Handlers
{
    public interface IVeldridView : IView//, ApplicationWindow
    {
        //IGPUDrawable  Drawable
        BaseGpuDrawable Drawable { set; get; }

        /// <summary>
        /// Set Backend which you want use.
        /// On Android, if you don't set it, Veldrid will detect if Vulkan works, if not, use OpenGL.
        /// On iOS, if you don't set it, it use Metal, current not support Vulkan, notice Apple have deprecated OpenGL.
        /// On Windows, you only use D3D11.
        /// </summary>
        GraphicsBackend Backend { set; get; }

        /// <summary>
        /// 是否使用像游戏循环一样不断调用<see cref="BaseGpuDrawable.Draw(float)"/>
        /// </summary>
        bool AutoReDraw { set; get; }

        GraphicsDeviceOptions? Options { set; get; }
    }
}
