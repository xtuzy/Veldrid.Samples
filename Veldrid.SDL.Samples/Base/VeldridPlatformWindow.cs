using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Veldrid.SDL.Samples.Base
{
    public class VeldridPlatformWindow
    {
        public Sdl2Window Window;
        public int Width => Window.Width;
        public int Height => Window.Height;

        public Action SizeChanged;
        public Action Loaded;
        public Action Unloaded;

        public VeldridPlatformWindow(int x = 100, int y = 100, int w = 500, int h = 500)
        {
            WindowCreateInfo windowCI = new WindowCreateInfo()
            {
                X = x,
                Y = y,
                WindowWidth = w,
                WindowHeight = h,
                WindowTitle = "VeldridLearning"
            };
            Window = VeldridStartup.CreateWindow(ref windowCI);
            Window.Shown += Window_Exposed;
            Window.Resized += Window_Resized;
            Window.Closing += Window_Closing;
        }

        private void Window_Closing()
        {
            Unloaded?.Invoke();
        }

        private void Window_Exposed()
        {
            Loaded?.Invoke();
        }

        private void Window_Resized()
        {
            SizeChanged?.Invoke();
        }

        public double CompositionScaleX => 1;
        public double CompositionScaleY => 1;
    }
}
