using System;
using Veldrid;

namespace Veldrid.Maui.Controls.Base
{
    public abstract class BaseVeldridPlatformInterface : IDisposable
    {
        BaseGpuDrawable drawable;
        public BaseGpuDrawable Drawable
        {
            get
            {
                return drawable;
            }
            set
            {
                var newDrawable = value;
                var oldDrawable = drawable;

                if (oldDrawable != null)
                {
                    oldDrawable.TryRemoveFrom(this);                  
                }
                if(newDrawable != null)
                    newDrawable.TryAddTo(this);
                drawable = newDrawable;
            }
        }

        public abstract  bool AutoReDraw { get;  set; }
        public GraphicsDeviceOptions? Options = null;

        public PlatformType PlatformType { get; protected set; }

        public event Action<float> Rendering;
        public event Action GraphicsDeviceCreated;
        public event Action GraphicsDeviceDestroyed;
        public event Action Resized;

        //event Action<KeyEvent> KeyPressed;
        public void InvokeRendering(float deltaMilliseconds) => Rendering?.Invoke(deltaMilliseconds);
        public void InvokeGraphicsDeviceCreated() => GraphicsDeviceCreated?.Invoke();
        public void InvokeGraphicsDeviceDestroyed() => GraphicsDeviceDestroyed?.Invoke();
        public void InvokeResized() => Resized?.Invoke();

        /// <summary>
        /// Pixel Width
        /// </summary>
        public abstract uint Width { get; }
        /// <summary>
        /// Pixel Height
        /// </summary>
        public abstract uint Height { get; }

        public GraphicsDevice _graphicsDevice { get; protected set; }
        public Swapchain _swapChain { get; protected set; }
        public ResourceFactory _resources { get; protected set;  }

        public virtual void Dispose()
        {
            _resources = null;
            _swapChain?.Dispose();
            _swapChain = null;
            _graphicsDevice?.Dispose();
            _graphicsDevice = null;
        }
    }
}
