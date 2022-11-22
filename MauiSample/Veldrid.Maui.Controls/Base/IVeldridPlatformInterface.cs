using System;
using Veldrid;

namespace Veldrid.Maui.Controls.Base
{
    public interface IVeldridPlatformInterface
    {
        PlatformType PlatformType { get; }

        event Action<float> Rendering;
        event Action<GraphicsDevice, ResourceFactory, Swapchain> GraphicsDeviceCreated;
        event Action GraphicsDeviceDestroyed;
        event Action Resized;
        //event Action<KeyEvent> KeyPressed;

        uint Width { get; }
        uint Height { get; }
    }
}